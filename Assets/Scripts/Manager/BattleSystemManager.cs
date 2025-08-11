#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 전반적인 전투 로직을 관리한다.
/// </summary>
public class BattleSystemManager : BaseMonoManager<BattleSystemManager>
{
    #region Property
    public bool InBattle => battleInfo.BattleState == BattleState.Playing;
    public int CurrentWave => battleInfo.CurrentWave;
    public Vector3 CurrentCharacterPos => battleInfo.CurrentCharacter.transform.position;
    #endregion

    #region Value
    [SerializeField]
    private Transform objectContainer;

    private BattleExpGainer expGainer;
    private DamageNumbersGroup damageNumbersGroup;
    private BattleCardDrawer cardDrawer;
    private BattleInfo battleInfo;
    #endregion

    public async UniTask Prepare(BattleInfo battleInfo)
    {
        this.battleInfo = battleInfo;

        cardDrawer = new BattleCardDrawer();
        cardDrawer.Initialize();

        await InitilalizeExpGainer(battleInfo.CurrentCharacter);
        await InitializeDamageGroup();
    }

    public async UniTask StartBattle()
    {
        await ShowBattleView();
        await ActiveAllyCharacters();

        battleInfo.SetBattleState(BattleState.Playing);
    }

    private async UniTask ShowBattleView()
    {
        var battleViewController = new BattleViewController();

        BattleViewModel viewModel = new BattleViewModel();
        viewModel.SetByBattleInfo(battleInfo);
        viewModel.SetBattleStartTime(battleInfo.BattleStartTime);

        battleViewController.SetModel(viewModel);

        await UIManager.Instance.ChangeView(battleViewController, true);
    }

    private async UniTask ActiveAllyCharacters()
    {
        foreach (var character in battleInfo.BattleTeam.CharacterUnits)
        {
            character.Initialize();
            character.Activate();
        }
    }

    private async UniTask InitilalizeExpGainer(CharacterUnit characterUnit)
    {
        expGainer = characterUnit.GetComponent<BattleExpGainer>();

        if (expGainer == null)
        {
            expGainer = await AddressableManager.Instance.InstantiateAddressableMonoAsync<BattleExpGainer>(
                PathDefine.CHARACTER_EXP_GAINER, characterUnit.transform);

            expGainer.SetModel(new BattleExpGainerModel());
        }

        expGainer.Model.SetOnExpGain(OnExpGain);
        expGainer.Model.SetOwner(characterUnit.Model);
        expGainer.Activate(true);
    }

    private async UniTask InitializeDamageGroup()
    {
        if (damageNumbersGroup == null)
        {
            damageNumbersGroup = await AddressableManager.Instance.
                InstantiateAddressableMonoAsync<DamageNumbersGroup>(PathDefine.DAMAGE_GROUP, objectContainer);
        }

        damageNumbersGroup.PrewarmPool();
    }

    private Color GetHitColorByDamageType(DamageType damageType)
    {
        return damageType switch
        {
            DamageType.Heal => new Color32(100, 255, 100, 255),
            DamageType.Poison => new Color32(255, 100, 255, 255),
            _ => new Color32(255, 100, 100, 255),
        };
    }

    #region OnEvent
    private void OnPause()
    {
        Time.timeScale = 0;
        battleInfo.SetBattleState(BattleState.Paused);
        Logger.BattleLog("Game PAUSED");
    }

    private void OnResume()
    {
        Time.timeScale = 1;
        battleInfo.SetBattleState(BattleState.Playing);
        Logger.BattleLog("Game RESUME");
    }

    private void OnExpGain(float exp)
    {
        int prevLevel = battleInfo.Level;

        battleInfo.OnExpGain(exp);

        BattleObserverParam param = new BattleObserverParam();
        param.SetBattleInfo(battleInfo);

        // 경험치 획득 시 UI 갱신해준다.
        ObserverManager.NotifyObserver(BattleObserverID.ExpGain, param);

        if (battleInfo.Level > prevLevel)
            OnLevelUp();
    }

    private void OnLevelUp()
    {
        var drawnCards = cardDrawer.DrawBattleCards(battleInfo.Level);

        if (drawnCards == null || drawnCards.Length == 0)
        {
            Logger.Error("Draw Failed!");
            return;
        }

        BattleCardSelectController battleCardSelectController = new BattleCardSelectController();
        var battleCardSelectModel = new BattleCardSelectViewModel();
        battleCardSelectModel.SetOnCompleteSelect(OnResume);
        battleCardSelectModel.SetBattleCardUnitModels(drawnCards);
        battleCardSelectModel.SetOnSelectBattleCard(OnSelectBattleCard);

        battleCardSelectController.SetModel(battleCardSelectModel);

        OnPause();
        UIManager.Instance.OpenPopup(battleCardSelectController).Forget();
    }

    private void OnSelectBattleCard(DataBattleCard card)
    {
        switch (card.CardType)
        {
            case BattleCardType.GetAbility:
                OnGetAbility((int)card.Ability);
                break;

            case BattleCardType.ExpGainRangeUp:
                OnExpGainRangeUp();
                break;
        }
    }

    private void OnGetAbility(int abilityDataId)
    {
        var currentCharacter = battleInfo.CurrentCharacter;

        if (currentCharacter == null)
            return;

        var characterModel = currentCharacter.Model;
        characterModel.AbilityProcessor.AddAbility(abilityDataId);
    }

    private void OnExpGainRangeUp()
    {
        if (expGainer == null)
            return;

        int prevLevel = 0;
        var model = expGainer.Model;
        model.SetLevel(model.Level + 1);

        expGainer.UpdateRadius();

        Logger.BattleLog($"Exp gainer level up : {prevLevel} => {model.Level}");
    }

    private void OnDefeat()
    {
    }

    private void OnVictory()
    {
    }
    #endregion

    #region Public OnEvent
    public void OnDamage(CharacterUnitModel sender, CharacterUnitModel receiver, float value, DamageType damageType)
    {
        if (receiver == null || receiver.IsDead)
            return;

        var damageAmount = Formula.GetDamageAmount(sender, receiver, value, damageType);

        var finalDamage = damageAmount;
        receiver.AddDamage(finalDamage);

        Logger.BattleLog($"{sender.CharacterDefine} -> {receiver.CharacterDefine} : {finalDamage} / Remain HP : {receiver.Hp}");

        // 적인 경우에만 대미지를 표시.
        if (receiver.TeamTag == TeamTag.Enemy)
            damageNumbersGroup.ShowDamage(damageType, receiver.Transform, finalDamage.ToString());

        OnHitEffect(receiver, damageType);
    }

    public void OnHitEffect(CharacterUnitModel receiver, DamageType damageType)
    {
        if (!receiver.IsDead)
            receiver.ActionHandler.OnHitEffectAsync(GetHitColorByDamageType(damageType)).Forget();
    }

    public void OnDeadCharacter(CharacterUnitModel deadCharacterModel)
    {
        if (deadCharacterModel.TeamTag == TeamTag.Enemy)
        {
            battleInfo.AddKill();

            BattleObserverParam param = new BattleObserverParam();
            param.SetBattleInfo(battleInfo);

            ObserverManager.NotifyObserver(BattleObserverID.EnemyKilled, param);

            // 죽은 캐릭터가 보스라면 승리
            if (deadCharacterModel.CharacterDefine == battleInfo.BossCharacterDefine)
                OnVictory();
        }
        else if (deadCharacterModel == battleInfo.CurrentCharacter.Model)
        {
            // 죽은 캐릭터가 메인캐릭터라면 패배
            OnDefeat();
        }
    }

    public void OnCheatLevelUp()
    {
#if UNITY_EDITOR
        var nextExp = battleInfo.NextBattleExp;
        var currentExp = battleInfo.BattleExp;
        var diff = nextExp - currentExp;

        OnExpGain(currentExp + diff);
#endif
    }
    #endregion
}
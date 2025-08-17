#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 전반적인 전투 로직을 관리한다.
/// </summary>
public class BattleSystemManager : BaseMonoManager<BattleSystemManager>
{
    #region Property
    public static bool InBattle => instance ? instance.inBattle : false;
    public int CurrentWave => battleInfo.CurrentWave;
    public Vector3 CurrentCharacterPos => battleInfo.CurrentCharacter.transform.position;
    private bool inBattle => battleInfo == null ?
        false : battleInfo.BattleState is BattleState.Playing or BattleState.Paused;
    #endregion

    #region Value
    [SerializeField]
    private Transform objectContainer;

    private BattleExpGainer expGainer;
    private DamageNumbersGroup damageNumbersGroup;
    private BattleCardDrawer cardDrawer;
    private BattleInfo battleInfo;
    private BattleObserverParam observerParam = new();
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

        StartWaveAddAsync().Forget();
    }

    public void Stop()
    {
        TokenPool.Cancel(GetHashCode());
    }

    private async UniTask StartWaveAddAsync()
    {
        while (inBattle && battleInfo.CurrentWave < IntDefine.MAX_DUNGEON_WAVE_COUNT)
        {
            var waveSeconds = GetWaveSeconds();
            await UniTaskUtils.DelaySeconds(waveSeconds, TokenPool.Get(GetHashCode()));

            AddWave();
        }
    }

    private void AddWave()
    {
        if (battleInfo != null)
        {
            battleInfo.SetCurrentWave(battleInfo.CurrentWave + 1);
            RefreshViewModel();
        }

        Logger.BattleLog($"Current Wave => {battleInfo.CurrentWave}");
    }

    private float GetWaveSeconds()
    {
        float waveSeconds = 60;

#if CHEAT
        if (CheatManager.CheatConfig.ToggleWaveBoostX2)
            waveSeconds = waveSeconds / 2;
#endif

        return waveSeconds;
    }

    private async UniTask ShowBattleView()
    {
        var battleViewController = new BattleViewController();

        BattleViewModel viewModel = new BattleViewModel();
        RefreshViewModel(viewModel);

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

    private void RefreshViewModel(BattleViewModel viewModel = null)
    {
        if (viewModel == null)
            viewModel = UIManager.instance.GetCurrentViewController().GetModel<BattleViewModel>();

        viewModel.SetLevel(battleInfo.Level);
        viewModel.SetBattleExp(battleInfo.BattleExp);
        viewModel.SetNextBattleExp(battleInfo.NextBattleExp);
        viewModel.SetKillCount(battleInfo.KillCount);
        viewModel.SetCurrentWave(battleInfo.CurrentWave);
        viewModel.SetBattleStartTime(battleInfo.BattleStartTime);
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
#if CHEAT
        if (CheatManager.CheatConfig.ToggleExpBoostX2)
            exp *= 2;
#endif

        int prevLevel = battleInfo.Level;

        battleInfo.OnExpGain(exp);

        RefreshViewModel();

        if (battleInfo.Level > prevLevel)
            OnLevelUp();
    }

    private void OnLevelUp(BattleCardTier firstDraw = BattleCardTier.Common)
    {
        // firstDraw => 첫장에 특정 티어 확정 출연 (Common이면 적용 X)

        var abProcessor = battleInfo.CurrentCharacter.Model.AbilityProcessor;
        var drawnCards = cardDrawer.DrawBattleCards(battleInfo.Level, abProcessor, firstDraw);

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

        RefreshViewModel();
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

    private void OnBattleEnd(BattleResult result)
    {
        battleInfo.SetBattleResult(result);
        battleInfo.SetBattleState(BattleState.End);

        ObserverManager.NotifyObserver(BattleObserverID.BattleEnd, observerParam);

        var battleResultController = new BattleResultController();
        var battleResultModel = new BattleResultViewModel();
        battleResultModel.SetByBattleInfo(battleInfo);
        battleResultModel.SetOnReturnToTown(OnReturnToTown);
        battleResultController.SetModel(battleResultModel);

        UIManager.instance.OpenPopup(battleResultController).Forget();
    }

    private void OnReturnToTown()
    {
        var currentTown = PlayerManager.instance.User.CurrentTown;

        FlowManager.Instance.ChangeCurrentTownFlow(currentTown).Forget();
    }
    #endregion

    #region Public OnEvent
    public void OnDamage(CharacterUnitModel sender, CharacterUnitModel receiver, float value, DamageType damageType = DamageType.Normal)
    {
        if (receiver == null || receiver.IsDead)
            return;

        var damageAmount = Formula.GetDamageAmount(sender, receiver, value, damageType);

        var finalDamage = damageAmount;
        receiver.AddDamage(finalDamage);

        // 적인 경우에만 대미지를 표시.
        if (receiver.TeamTag == TeamTag.Enemy)
            damageNumbersGroup.ShowDamage(damageType, receiver.Transform, finalDamage.ToString());

        OnHitBodyColor(receiver, damageType);
    }

    public void OnHeal(CharacterUnitModel sender, CharacterUnitModel receiver, float value)
    {
        var damageType = DamageType.Heal;

        receiver.AddHp(value);

        damageNumbersGroup.ShowDamage(damageType, receiver.Transform, value.ToString());

        OnHitBodyColor(receiver, damageType);
    }

    private void OnHitBodyColor(CharacterUnitModel receiver, DamageType damageType)
    {
        if (!receiver.IsDead)
            receiver.ActionHandler.OnHitBodyColorAsync(GetHitColorByDamageType(damageType)).Forget();
    }

    public void OnDeadCharacter(CharacterUnitModel deadCharacterModel)
    {
        if (deadCharacterModel.TeamTag == TeamTag.Enemy)
        {
            battleInfo.AddKill();

            RefreshViewModel();

            var newObserverParam = new BattleObserverParam();
            newObserverParam.SetModelValue(deadCharacterModel);
            ObserverManager.NotifyObserver(BattleObserverID.EnemyKilled, newObserverParam);

            // 죽은 캐릭터가 보스라면 승리
            if (deadCharacterModel.CharacterDefine == battleInfo.FinalBoss)
                OnBattleEnd(BattleResult.Victory);
        }
        else if (deadCharacterModel == battleInfo.CurrentCharacter.Model)
        {
            // 죽은 캐릭터가 메인캐릭터라면 패배
            OnBattleEnd(BattleResult.Defeat);
        }
    }

#if CHEAT
    public void OnCheatLevelUp()
    {
        OnCheatLevelUpWithDraw();
    }

    public void OnCheatLevelUpWithDraw(BattleCardTier tier = BattleCardTier.Common)
    {
        var nextExp = battleInfo.NextBattleExp;
        var currentExp = battleInfo.BattleExp;
        var diff = nextExp - currentExp;

        var exp = currentExp + diff;

        int prevLevel = battleInfo.Level;

        battleInfo.OnExpGain(exp);

        RefreshViewModel();

        if (battleInfo.Level > prevLevel)
            OnLevelUp(tier);
    }

    public void OnCheatAddWave()
    {
        AddWave();
    }
#endif

#endregion
}
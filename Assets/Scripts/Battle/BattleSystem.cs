#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전반적인 전투 로직을 관리한다.
/// </summary>
public class BattleSystem : MonoBehaviour, IObserver
{
    #region Property
    private bool InBattle => battleInfo == null ?
        false : battleInfo.BattleState is BattleState.Playing or BattleState.Paused;
    #endregion

    #region Value
    [SerializeField]
    private Transform objectContainer;

    private BattleExpGainer expGainer;
    private DamageNumbersGroup damageNumbersGroup;
    private BattleCardDrawer cardDrawer;
    private BattleInfo battleInfo;

    // 값 필요 없는 param인 경우 new()하지말고 이거 재사용
    private BattleObserverParam noValueObserverParam = new();
    private BattleViewController battleViewController;
    #endregion

    public async UniTask Prepare(BattleInfo battleInfo)
    {
        this.battleInfo = battleInfo;

        cardDrawer = new BattleCardDrawer();
        cardDrawer.Initialize();

        await InitilalizeExpGainer(battleInfo.MainCharacter);
        await InitializeDamageGroup();

        ObserverManager.AddObserver(BattleObserverID.DeadCharacter, this);
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
        OnResume();
        ObserverManager.RemoveObserver(BattleObserverID.DeadCharacter, this);
        TokenPool.Cancel(GetHashCode());
    }

    private async UniTask StartWaveAddAsync()
    {
        while (InBattle && battleInfo.CurrentWave < IntDefine.MAX_DUNGEON_WAVE_COUNT)
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
            NotifyChangedBattleInfo();
        }

        var observerParam = new BattleObserverParam();
        observerParam.SetIntValue(battleInfo.CurrentWave);
        ObserverManager.NotifyObserver(BattleObserverID.ChangeWave, observerParam);

        Logger.BattleLog($"Current Wave => {battleInfo.CurrentWave}");
    }

    private float GetWaveSeconds()
    {
        float waveSeconds = BattleDefine.DEFAULT_WAVE_DURATION_SECONDS;

#if CHEAT
        if (CheatManager.CheatConfig.ToggleWaveBoostX2)
            waveSeconds = waveSeconds * BattleDefine.CHEAT_WAVE_DURATION_MULTIPLIER;
#endif

        return waveSeconds;
    }

    private async UniTask ShowBattleView()
    {
        battleViewController = new BattleViewController();

        BattleViewModel viewModel = new BattleViewModel();
        battleViewController.SetModel(viewModel);

        battleViewController.RefreshBattleInfo(new BattleInfoParam(battleInfo));

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

    // BattleInfo 직접 노출 안시키도록
    private void NotifyChangedBattleInfo(BattleInfoParam param = null)
    {
        if (battleViewController == null)
            battleViewController = UIManager.Instance.GetCurrentViewController<BattleViewController>();

        if (param == null)
            param = new BattleInfoParam(battleInfo);

        battleViewController.RefreshBattleInfo(param);
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
            exp *= BattleDefine.CHEAT_EXP_MULTIPLIER;
#endif

        if (!InBattle)
            return;

        int prevLevel = battleInfo.Level;

        battleInfo.OnExpGain(exp);

        NotifyChangedBattleInfo();

        if (battleInfo.Level > prevLevel)
            OnLevelUp();
    }

    private void OnLevelUp(BattleCardTier firstDraw = BattleCardTier.Common)
    {
        // firstDraw => 첫장에 특정 티어 확정 출연 (Common이면 적용 X)

        var abProcessor = battleInfo.MainCharacter.Model.AbilityProcessor;
        var cards = cardDrawer.DrawBattleCards(battleInfo.Level, abProcessor, firstDraw);

        if (cards == null || cards.Length == 0)
        {
            Logger.Error("Draw Failed!");
            return;
        }

        BattleCardSelectController battleCardSelectController = new BattleCardSelectController();
        var battleCardSelectModel = new BattleCardSelectViewModel();
        var cardUnitModelList = new List<BattleCardUnitModel>();
        var abilityContainer = DataManager.Instance.GetDataContainer<DataAbility>();

        if (cards != null && cards.Length != 0)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                var card = cards[i];
                int expectedLevel = 0;
                int abilityId = (int)card.Ability;

                if (abProcessor.IsContain(abilityId))
                    expectedLevel = abProcessor.GetLevel(abilityId) + 1;

                var model = new BattleCardUnitModel();
                model.SetCardData(card);
                model.SetTier(card.Tier);

                if (card.CardType == BattleCardType.GetAbility)
                {
                    var abilityData = abilityContainer.GetById((int)card.Ability);
                    model.SetIconPath(abilityData.IconPath);
                }
                else
                {
                    model.SetIconPath(card.IconPath);
                }

                model.SetNameText(LocalizationManager.GetLocalization(card.Name));
                model.SetDescriptionText(LocalizationManager.GetLocalization(card.Desc));
                model.SetLevel(expectedLevel);

                cardUnitModelList.Add(model);
            }

            battleCardSelectModel.SetBattleCardUnitModels(cardUnitModelList);
            battleCardSelectModel.SetOnCompleteSelect(OnResume);
            battleCardSelectModel.SetOnSelectBattleCard(OnSelectBattleCard);

            battleCardSelectController.SetModel(battleCardSelectModel);

            UIManager.Instance.OpenPopup(battleCardSelectController).Forget();
        }

        OnPause();
    }

    private void OnSelectBattleCard(DataBattleCard card)
    {
        switch (card.CardType)
        {
            case BattleCardType.GetAbility:
                OnGetAbility((int)card.Ability);
                break;
        }
    }

    private void OnGetAbility(int abilityDataId)
    {
        var currentCharacter = battleInfo.MainCharacter;

        if (currentCharacter == null)
            return;

        var characterModel = currentCharacter.Model;
        var abProcessor = characterModel.AbilityProcessor;

        if (abProcessor.IsContain(abilityDataId))
        {
            characterModel.AbilityProcessor.LevelUpAbility(abilityDataId);
        }
        else
        {
            characterModel.AbilityProcessor.AddAbility(abilityDataId);
        }

        var currentAbilitySlotDic = characterModel.AbilityProcessor.GetAbilitySlotDic();

        BattleInfoParam param = new BattleInfoParam(battleInfo);
        param.AbilitySlotDic = currentAbilitySlotDic;

        NotifyChangedBattleInfo(param);
    }
            
    private void OnHitBodyColor(CharacterUnitModel receiver, DamageType damageType)
    {
        if (!receiver.IsDead)
            receiver.ActionHandler.OnHitBodyColorAsync(GetHitColorByDamageType(damageType)).Forget();
    }

    private async UniTask OnBattleEnd(BattleResult result)
    {
        await UniTaskUtils.DelaySeconds(FloatDefine.DEFAULT_BATTLE_START_DELAY);

        SoundManager.Instance.StopCurrentSoloSound(SoundType.Bgm).Forget();

        battleInfo.SetBattleResult(result);
        battleInfo.SetBattleState(BattleState.End);

        NotifyChangedBattleInfo();
        ObserverManager.NotifyObserver(BattleObserverID.BattleEnd, noValueObserverParam);

        var battleResultController = new BattleResultController();
        var battleResultModel = new BattleResultViewModel();
        battleResultModel.SetByBattleInfo(battleInfo);
        battleResultModel.SetOnReturnToTown(OnReturnToTown);
        battleResultController.SetModel(battleResultModel);

        OnPause();

        UIManager.Instance.OpenPopup(battleResultController).Forget();
    }

    private void OnReturnToTown()
    {
        Time.timeScale = 1;

        FlowManager.Instance.ChangeCurrentTownFlow().Forget();
    }

    private void OnDeadCharacter(CharacterUnitModel deadCharacterModel)
    {
        if (!InBattle || deadCharacterModel == null)
            return;

        if (deadCharacterModel.TeamTag == TeamTag.Enemy)
        {
            if (deadCharacterModel.DistanceToTarget < DistanceToTarget.OutOfRange)
                battleInfo.AddKill();

            NotifyChangedBattleInfo();

            // 죽은 캐릭터가 보스라면 승리
            if (deadCharacterModel.CharacterDefine == battleInfo.FinalBoss)
                OnBattleEnd(BattleResult.Victory).Forget();
        }
        else if (deadCharacterModel == battleInfo.MainCharacter.Model)
        {
            // 죽은 캐릭터가 메인캐릭터라면 패배
            OnBattleEnd(BattleResult.Defeat).Forget();
        }
    }

    void IObserver.HandleMessage(Enum observerMessage, IObserverParam observerParam)
    {
        if (observerParam is not BattleObserverParam battleObserverParam)
            return;

        switch (observerMessage)
        {
            case BattleObserverID.DeadCharacter:
                OnDeadCharacter(battleObserverParam.ModelValue);
                break;
        }
    }
    #endregion

    #region Public OnEvent
    public void OnDamage(CharacterUnitModel sender, CharacterUnitModel receiver, float value, DamageType damageType = DamageType.Normal)
    {
        if (!InBattle)
            return;

        if (sender == null || sender.IsDead || receiver == null || receiver.IsDead)
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
        if (!InBattle)
            return;

        if (sender == null || sender.IsDead || receiver == null || receiver.IsDead)
            return;

        var damageType = DamageType.Heal;

        receiver.AddHp(value);

        damageNumbersGroup.ShowDamage(damageType, receiver.Transform, value.ToString());

        OnHitBodyColor(receiver, damageType);
    }

    public void OnExpGainRangeUp()
    {
        if (expGainer == null)
            return;

        int prevLevel = 0;
        var model = expGainer.Model;
        model.SetLevel(model.Level + 1);

        expGainer.UpdateRadius();

        Logger.BattleLog($"Exp gainer level up : {prevLevel} => {model.Level}");
    }


#if CHEAT
    public void CheatLevelUp()
    {
        CheatLevelUpWithDraw();
    }

    public void CheatLevelUpWithDraw(BattleCardTier tier = BattleCardTier.Common)
    {
        var nextExp = battleInfo.NextBattleExp;
        var currentExp = battleInfo.BattleExp;
        var diff = nextExp - currentExp;

        var exp = currentExp + diff;

        int prevLevel = battleInfo.Level;

        battleInfo.OnExpGain(exp);

        NotifyChangedBattleInfo();

        if (battleInfo.Level > prevLevel)
            OnLevelUp(tier);
    }

    public void CheatAddWave()
    {
        AddWave();
    }
#endif

    #endregion
}
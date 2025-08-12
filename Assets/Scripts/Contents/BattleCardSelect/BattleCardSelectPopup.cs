using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class BattleCardSelectPopup : BaseView
{
    #region Property
    public BattleCardSelectViewModel Model => GetModel<BattleCardSelectViewModel>();
    #endregion

    #region Value

    [SerializeField]
    private BattleCardUnit[] battleCards;

    [SerializeField]
    private DoTweenSupport popupPanelTween;

    [SerializeField]
    private CanvasGroup clickBlocker;

    [SerializeField]
    private GameObject dim;

    private int lastSelectIndex = 0;
    #endregion

    #region Function
    public override async UniTask ShowAsync()
    {
        dim.SafeSetActive(true);
        clickBlocker.gameObject.SafeSetActive(true);

        await PreloadCards();
        await ShowCardUnits();

        clickBlocker.gameObject.SafeSetActive(false);
    }

    private async UniTask PreloadCards()
    {
        var tasks = new UniTask[battleCards.Length];
        for (int i = 0; i < battleCards.Length; i++)
        {
            if (i < Model.CardUnitModelList.Count)
            {
                var card = battleCards[i];
                card.SetModel(Model.CardUnitModelList[i]);
                tasks[i] = card.LoadAsync();
            }
        }

        await UniTask.WhenAll(tasks);
    }

    private async UniTask ShowCardUnits()
    {
        await popupPanelTween.ShowAsync();

        for (int i = 0; i < battleCards.Length; i++)
            await battleCards[i].ShowAsync();
    }

    public override async UniTask HideAsync()
    {
        dim.SafeSetActive(false);

        // 1. 선택안한 카드 먼저 숨김
        var cardHideTasks = new List<UniTask>();

        for (int i = 0; i < battleCards.Length; i++)
        {
            if (i == lastSelectIndex)
                continue;

            cardHideTasks.Add(battleCards[i].HideAsync());
        }

        await UniTask.WhenAll(cardHideTasks);

        // 2. 팝업패널 숨김
        await popupPanelTween.HideAsync();

        // 3. 선택한 카드 숨김
        await battleCards[lastSelectIndex].HideAsync();
    }

    #region OnEvent
    public void OnSelectCard(int index)
    {
        OnSelectCardAsync(index).Forget();
    }

    private async UniTask OnSelectCardAsync(int index)
    {
        lastSelectIndex = index;
        clickBlocker.gameObject.SafeSetActive(true);

        await battleCards[index].ShowSelectTweenAsync();
        Model.OnClickBattleCard(index);
    }
    #endregion
    #endregion
}

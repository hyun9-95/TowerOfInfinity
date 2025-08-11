using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : BaseMonoManager<UIManager>
{
    /// <summary>
    /// 캔버스와 Rect는 1:1 매칭
    /// </summary>
    [SerializeField]
    private Canvas[] canvases;

    [SerializeField]
    private Transform[] uiTransformRects;

    // Base가 되는 단일 View
    // 히스토리 관리하지 않고 Flow당 하나의 View가 있어야한다.
    public UIType CurrentView => currentViewController == null ? UIType.None : currentViewController.UIType;

    // 히스토리로 관리되는 팝업
    public UIType CurrentOpenUI => uiHistory.Count > 0 ? uiHistory.Peek().UIType : UIType.None;

    private Stack<BaseController> uiHistory = new Stack<BaseController>();
    private Dictionary<UIType, BaseView> viewHistoryDic = new Dictionary<UIType, BaseView>();
    private BaseController currentViewController;

    public async UniTask ChangeView(BaseController controller, bool addressable = true)
    {
        if (!controller.IsView)
            return;

        if (currentViewController != null)
            await ClearCurrentView();

        var isSuccess = await EnterUI(controller, addressable, true);

        if (isSuccess)
        {
            currentViewController = controller;

            Logger.Success($"[Change View] Success! {controller.UIType} => {CurrentView}");
        }
        else
        {
            Logger.Error($"[Change View] Fail.. {controller.UIType}");
        }
    }

    public async UniTask OpenPopup(BaseController controller, bool addressable = true)
    {
        if (controller.IsView)
            return;

        var isSuccess = await EnterUI(controller, addressable);

        if (isSuccess)
        {
            Logger.Success($"[Enter Popup] Success! {controller.UIType} => {CurrentView}");
        }
        else
        {
            Logger.Error($"[Enter Popup] Fail.. {controller.UIType}");
        }
    }

    private async UniTask<bool> EnterUI(BaseController controller, bool addressable = true, bool transition = false)
    {
        string address = string.Format(PathDefine.UI_VIEW_FORMAT, controller.UIType);

        BaseView view = GetViewHistory(controller.UIType);

        if (view == null)
        {
            if (addressable)
            {
                view = await AddressableManager.Instance.InstantiateAddressableMonoAsync<BaseView>(address, GetUITargetRect(controller.UICanvasType));
            }
            else
            {
                var viewObject = Resources.Load<GameObject>(address);

                if (viewObject == null)
                {
                    Logger.Null(address);
                    return false;
                }

                var instantiatedView = Instantiate(viewObject, GetUITargetRect(controller.UICanvasType));

                if (instantiatedView == null)
                    return false;

                view = instantiatedView.GetComponent<BaseView>();
            }
        }

        if (view == null)
        {
            Logger.Null(address);
            return false;
        }

        controller.SetView(view);

        controller.Enter();
        await controller.LoadingProcess();
        view.gameObject.SafeSetActive(true);
        await controller.Process();

        uiHistory.Push(controller);
        viewHistoryDic[controller.UIType] = view;
        return true;
    }

    public async UniTask ClearCurrentView()
    {
        if (currentViewController == null)
            return;

        await currentViewController.Exit();
        currentViewController.DestroyView();

        currentViewController = null;

        while (uiHistory.Count > 0)
        {
            var controller = uiHistory.Pop();
            controller.DestroyView();
        }

        uiHistory.Clear();
        viewHistoryDic.Clear();
    }

    public async UniTask Back()
    {
        // View는 Back 불가.
        if (CurrentView == uiHistory.Peek().UIType)
            return;

        if (uiHistory == null || uiHistory.Count <= 1)
        {
            Logger.Error($"[Back] UIHistory is empty! CurrentView: {CurrentView}");
            return;
        }
            
        var currentController = uiHistory.Pop();
        await currentController.Exit();

        var prevController = uiHistory.Peek();
        BaseView view = GetViewHistory(prevController.UIType);
        prevController.SetView(view);

        await prevController.LoadingProcess();
        await prevController.Refresh();
    }

    public BaseController GetCurrentViewController()
    {
        return currentViewController;
    }

    public async UniTask RefreshCurrentUI()
    {
        if (currentViewController == null)
            return;

        await currentViewController.Refresh();
    }

    private Transform GetUITargetRect(UICanvasType uICanvasType)
    {
        int index = (int)uICanvasType;

        if (index >= uiTransformRects.Length)
            return null;

        return uiTransformRects[index];
    }

    private BaseView GetViewHistory(UIType uiType)
    {
        if (!viewHistoryDic.ContainsKey(uiType))
            return null;

        return viewHistoryDic[uiType];
    }
}

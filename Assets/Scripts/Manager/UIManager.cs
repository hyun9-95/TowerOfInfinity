using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

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
    public UIType CurrentOpenUI = UIType.None;

    private Stack<BaseController> uiHistory = new Stack<BaseController>();
    private Dictionary<UIType, BaseView> viewHistoryDic = new Dictionary<UIType, BaseView>();
    private BaseController currentViewController;

    public async UniTask ChangeView(BaseController controller, bool addressable = true)
    {
        if (!controller.IsView)
            return;

        await ExitPreviousView();

        var isSuccess = await EnterUI(controller, addressable, true);

        if (isSuccess)
        {
            currentViewController = controller;
            CurrentOpenUI = controller.UIType;

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
            CurrentOpenUI = controller.UIType;

            Logger.Success($"[Enter Popup] Success! {controller.UIType} => {CurrentView}");
        }
        else
        {
            Logger.Error($"[Enter Popup] Fail.. {controller.UIType}");
        }
    }

    private async UniTask<bool> EnterUI(BaseController controller, bool addressable = true, bool transition = false)
    {
        string name = controller.UIType.ToString();

        BaseView view = GetViewHistory(controller.UIType);

        if (view == null)
        {
            if (addressable)
            {
                view = await AddressableManager.Instance.InstantiateAddressableMonoAsync<BaseView>(name, GetUITargetRect(controller.UICanvasType));
            }
            else
            {
                string replaceText = controller.IsView ? "View" : "Popup";
                string resourcesFolderName = name.Replace(replaceText, "");
                string resourcesName = string.Format(PathDefine.Resources_UI_View, resourcesFolderName, name);
                var viewObject = Resources.Load<GameObject>(resourcesName);

                if (viewObject == null)
                {
                    Logger.Null(resourcesName);
                    return false;
                }

                var instantiatedView = Instantiate(viewObject, GetUITargetRect(controller.UICanvasType));

                if (instantiatedView == null)
                    return false;

                view = instantiatedView.GetComponent<BaseView>();
            }

            controller.SetView(view);
        }

        if (view == null)
        {
            Logger.Null(name);
            return false;
        }

        controller.Enter();
        await controller.LoadingProcess();
        view.gameObject.SafeSetActive(true);
        await controller.Process();

        viewHistoryDic[controller.UIType] = view;

        return true;
    }

    private async UniTask ExitPreviousView()
    {
        if (currentViewController == null)
            return;

        // Exit할 때 View의 어드레서블 Release
        await currentViewController.Exit();
        currentViewController = null;
    }

    public async UniTask Back()
    {
        if (uiHistory == null || uiHistory.Count == 0)
        {
            Logger.Error($"[Back] UIHistory is empty! CurrentView: {CurrentView}");
            return;
        }
            
        // View는 Back 불가.
        if (CurrentView == uiHistory.Peek().UIType)
            return;

        var currentController = uiHistory.Pop();

        // Exit할 때 View의 어드레서블 Release
        await currentController.Exit();

        if (uiHistory.Count == 0)
        {
            Logger.Error("Empty UIHistory!!");
            return;
        }

        var prevController = uiHistory.Peek();
        await prevController.LoadingProcess();
        await prevController.Process();

        CurrentOpenUI = prevController.UIType;
    }

    public BaseController GetCurrentController()
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

using UnityEngine;

public class InputManager : BaseMonoManager<InputManager>
{
    #region Property
    public static InputWrapper InputInfo => Instance.inputInfo;
    #endregion

    #region Value
    [SerializeField]
    private MoveInput moveInput;

    [SerializeField]
    private ActionButton[] actionButtons;

    private readonly InputWrapper inputInfo = new();
    private ActionInput actionInput = ActionInput.None;
    private bool enableInput = false;
    #endregion

    #region Function
    private void Awake()
    {
        SetInstance(this);
    }

    private void Update()
    {
        if (!enableInput)
            return;

        GetActionInput();
        SetInputInfo();
    }
    
    private void GetActionInput()
    {
        var tempInput = GetKeyboardActionInput();

        if (tempInput != ActionInput.None)
        {
            var actionButton = actionButtons[(int)tempInput];
               
            if (!actionButton.IsCoolTime && !actionButton.IsClicked)
            {
                actionButton.OnAction(true);
                actionInput = tempInput;
                return;
            }
        }    

        tempInput = GetActionButtonInput();

        if (tempInput != ActionInput.None)
        {
            actionInput = tempInput;
            return;
        }

        actionInput = ActionInput.None;
    }

    private ActionInput GetKeyboardActionInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            return ActionInput.Roll;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            return ActionInput.Attack;
        }

        return ActionInput.None;
    }

    private ActionInput GetActionButtonInput()
    {
        if (actionButtons == null || actionButtons.Length == 0)
            return ActionInput.None;

        foreach (var button in actionButtons)
        {
            if (button != null && button.IsClicked)
                return button.InputType;
        }

        return ActionInput.None;
    }

    private void SetInputInfo()
    {
        if (inputInfo == null)
            return;

        InputInfo.SetIsMove(moveInput.IsMoving);

        if (moveInput.IsMoving)
            inputInfo.SetMovement(new Vector2(moveInput.MoveX, moveInput.MoveY));

        InputInfo.SetActionInput(actionInput);
    }


    public static void EnableMoveInput(bool value)
    {
        if (Instance.moveInput == null)
            return;

        var platform = GameManager.Platform;
        
        Instance.moveInput.EnableInput(value, platform.IsMobile);
        Instance.enableInput = value;

        if (!value)
            InputInfo.Reset();
    }

    public static void EnableActionButtons(bool value)
    {
        if (Instance.actionButtons == null)
            return;

        foreach (var button in Instance.actionButtons)
        {
            if (button != null)
            {
                if (value)
                {
                    var coolTime = Instance.inputInfo.GetActionInputCoolTime(button.InputType);
                    button.Enable(coolTime);
                }
                else
                {
                    button.gameObject.SetActive(false);
                }
            }
        }
    }

    public static void SetActionCoolTime(ActionInput actionType, float coolTime)
    {
        if (Instance.inputInfo == null)
            return;

        Instance.inputInfo.SetActionInputCoolTime(actionType, coolTime);
    }
    #endregion
}

using UnityEngine;

public class MoveInput : MonoBehaviour
{
    #region Property
    public bool IsMoving => moveX != 0 || moveY != 0;
    public float MoveX => moveX;
    public float MoveY => moveY;
    #endregion

    #region Value
    [SerializeField]
    private bl_Joystick joystick;

    private float moveX;
    private float moveY;
    private bool IsInput = false;
    #endregion Value

    #region Function
    public void EnableInput(bool value, bool isEnableJoystick)
    {
        IsInput = value;

        if (isEnableJoystick)
        {
            joystick.EnableInput(value);
        }
        else
        {
            joystick.EnableInput(false);
        }
    }

    private void Update()
    {
        GetInput();
    }
    
    private void GetInput()
    {
        if (!IsInput)
        {
            moveX = 0;
            moveY = 0;
        }

        if (joystick.IsAvailable && joystick.IsUsing)
        {
            GetJoystickInput();
        }
        else
        {
            GetKeyboradInput();
        }
    }

    private void GetKeyboradInput()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
    }

    private void GetJoystickInput()
    {
        if (!joystick.IsAvailable)
            return;

        moveX = joystick.Horizontal;
        moveY = joystick.Vertical;
    }
    #endregion Function
}

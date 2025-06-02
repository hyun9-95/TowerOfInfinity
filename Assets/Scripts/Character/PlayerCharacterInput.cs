using UnityEngine;

public class PlayerCharacterInput : MonoBehaviour
{
    #region Property
    public CharacterUnitModel OwnerModel { get; private set; }
    public bool IsMoving => moveX != 0 || moveY != 0;
    public float MoveX => moveX;
    public float MoveY => moveY;
    public PlayerInput PlayerInput => playerInput;
    #endregion

    #region Value
    private float moveX;
    private float moveY;
    private PlayerInput playerInput;
    #endregion Value

    #region Function
    public void Initialize(CharacterUnitModel ownerModel)
    {
        OwnerModel = ownerModel;
        OwnerModel.EnableInput(true);
    }

    private void FixedUpdate()
    {
        GetInput();
        SetInputWrapper();
    }
    
    private void GetInput()
    {
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            playerInput = PlayerInput.Roll;
            return;
        }
        else if (Input.GetMouseButton(0))
        {
            playerInput = PlayerInput.Attack;
            return;
        }
        else
        {
            playerInput = PlayerInput.None;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            playerInput = PlayerInput.SkillSlot_1;
    }

    private void SetInputWrapper()
    {
        if (OwnerModel == null)
            return;

        OwnerModel.InputWrapper.SetIsMove(IsMoving);
        OwnerModel.InputWrapper.SetPlayerInput(PlayerInput);

        if (MoveX != 0 || MoveY != 0)
            OwnerModel.InputWrapper.SetMovement(new Vector2(MoveX, MoveY));
    }
    #endregion Function
}

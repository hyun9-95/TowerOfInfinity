using UnityEngine;

public class InputWrapper
{
    public bool IsMove { get; private set; }

    public PlayerInput PlayerInput { get; private set; }

    public bool IsSkillInput => PlayerInput is PlayerInput.SkillSlot_1 or PlayerInput.SkillSlot_2 or PlayerInput.SkillSlot_3;

    public Vector2 Movement { get; private set; }

    public void SetIsMove(bool value)
    {
        IsMove = value;
    }

    public void SetPlayerInput(PlayerInput playerInput)
    {
        PlayerInput = playerInput;
    }

    public void SetMovement(Vector2 movement)
    {
        Movement = movement;
    }
}

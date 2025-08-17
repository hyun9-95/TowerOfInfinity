using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class InputWrapper
{
    public bool IsMove { get; private set; }

    public ActionInput ActionInput { get; private set; }

    public Vector2 Movement { get; private set; }

    private Dictionary<ActionInput, float> actionInputCoolTime = new();


    public void Reset()
    {
        IsMove = false;
        ActionInput = ActionInput.None;
        Movement = Vector2.zero;
        actionInputCoolTime.Clear();
    }

    public void SetIsMove(bool value)
    {
        IsMove = value;
    }

    public void SetActionInput(ActionInput playerInput)
    {
        ActionInput = playerInput;
    }

    public void SetMovement(Vector2 movement)
    {
        Movement = movement;
    }

    public void SetActionInputCoolTime(ActionInput actionInput, float coolTime)
    {
        actionInputCoolTime[actionInput] = coolTime;
    }

    public float GetActionInputCoolTime(ActionInput actionInput)
    {
        if (actionInputCoolTime.TryGetValue(actionInput, out float coolTime))
            return coolTime;

        return 0f;
    }
}

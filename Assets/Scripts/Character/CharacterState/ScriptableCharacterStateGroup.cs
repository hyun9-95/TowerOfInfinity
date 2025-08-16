using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableCharacterStateGroup", menuName = "Scriptable Objects/ScriptableCharacterStateGroup")]
public class ScriptableCharacterStateGroup : ScriptableObject
{
    public IReadOnlyList<ScriptableCharacterState> StateList => stateList;

    [SerializeField]
    protected List<ScriptableCharacterState> stateList = new List<ScriptableCharacterState>();

    private bool isSorted = false;

    private int CompareStates(ScriptableCharacterState a, ScriptableCharacterState b)
    {
        int compare = b.Priority.CompareTo(a.Priority);

        if (compare == 0)
            return stateList.IndexOf(a).CompareTo(stateList.IndexOf(b));

        return compare;
    }

    public void Sort()
    {
        if (isSorted)
            return;

        stateList.Sort(CompareStates);
        isSorted = true;
    }
}

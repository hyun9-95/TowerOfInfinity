using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "ScriptableCharacterInfo")]
public class ScriptableCharacterInfo : ScriptableObject
{
    public ScriptableCharacterStateGroup StateGroup => characterStateGroup;
    public ScriptableCharacterModuleGroup ModuleGroup => characterModuleGroup;

    [SerializeField]
    private ScriptableCharacterStateGroup characterStateGroup;

    [SerializeField]
    private ScriptableCharacterModuleGroup characterModuleGroup;
}

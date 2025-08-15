using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableCharacterModule/SpawnExpGem")]
public class SpawnExpGemModule : ScriptableCharacterModule
{
    public override ModuleType GetModuleType() => ModuleType.SpawnExpGem;

    public override IModuleModel CreateModuleModel()
    {
        return new SpawnExpGemModuleModel();
    }
    #region Property
    #endregion

    #region Value
    #endregion

    #region Function
    public override void OnCharacterDeactivate(CharacterUnitModel model, IModuleModel IModuleModel)
    {
        // 죽어서 비활성화된게 아니면 경험치 드랍 X
        if (model.Hp > 0)
            return;

        SpawnExpGem(model).Forget();
    }

    private async UniTask SpawnExpGem(CharacterUnitModel targetModel)
    {
        var expGemModel = new BattleExpGemModel();

        var exp = await ObjectPoolManager.Instance.SpawnPoolableMono<BattleExpGem>(PathDefine.BATTLE_EXP_GEM, targetModel.Transform.position, Quaternion.identity);
        exp.SetModel(expGemModel);

        var bounceDir = targetModel.IsFlipX ? Vector2.right : Vector2.left;
        exp.BounceAsync(bounceDir).Forget();
    }
    #endregion
}

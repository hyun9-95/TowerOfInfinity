using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Module/Check Target Distance")]
public class CheckTargetDistanceModule : ScriptableCharacterModule
{
    private float cameraVisibleDistance = 0;
    private float farDistance = 0;
    private float veryFarDistance = 0;

    private void OnEnable()
    {
        if (CameraManager.Instance == null)
            return;

        cameraVisibleDistance = CameraManager.Instance.DiagonalLengthFromCenter + 0.5f;
        farDistance = cameraVisibleDistance * BattleDefine.CAMERA_DISTANCE_FAR_MULTIPLIER;
        veryFarDistance = cameraVisibleDistance * BattleDefine.CAMERA_DISTANCE_VERY_FAR_MULTIPLIER;
    }

    public override ModuleType GetModuleType()
    {
        return ModuleType.DynamicUpdateInterval;
    }

    public override void OnEventUpdate(CharacterUnitModel model, IModuleModel IModuleModel)
    {
        if (model.Target == null)
            return;

        var distanceToTarget = DistanceToTarget.None;

        // 거리에 따라 업데이트 주기를 갱신
        float distanceSqr = (model.Target.Transform.position - model.Transform.position).sqrMagnitude;
        model.SetDistanceToTargetSqr(distanceSqr);

        if (distanceSqr >= (veryFarDistance * veryFarDistance))
        {
            distanceToTarget = model.CharacterType == CharacterType.Enemy ?
                DistanceToTarget.OutOfRange : DistanceToTarget.VeryFar;
        }
        else if (distanceSqr >= (farDistance * farDistance))
        {
            distanceToTarget = DistanceToTarget.Far;
        }
        else if (distanceSqr >= (cameraVisibleDistance * cameraVisibleDistance))
        {
            distanceToTarget = DistanceToTarget.Nearby;
        }
        else
        {
            distanceToTarget = DistanceToTarget.Close;
        }

        model.SetDistanceToTarget(distanceToTarget);

        if (distanceToTarget == DistanceToTarget.OutOfRange)
            model.ActionHandler.OnDeactivate();
    }

}

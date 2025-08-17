
using UnityEngine;

public struct CopiedTransformData
{
    public bool IsNull => Position == Vector3.zero || EulerAngles == Vector3.zero || Scale == Vector3.zero;

    public Vector3 Position;
    public Vector3 EulerAngles;
    public Vector3 Scale;
}

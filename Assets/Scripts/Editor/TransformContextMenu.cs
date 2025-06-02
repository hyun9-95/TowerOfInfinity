using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public static class TransformContextMenu
{
    [MenuItem("CONTEXT/Transform/Copy/Copy World Transform Data")]
    public static void CreateWorldTransformData(MenuCommand menuCommand)
    {
        Transform transform = menuCommand.context as Transform;
    
        if (transform != null)
        {
            CopiedTransformData copiedTransformData = new CopiedTransformData
            {
                Position = transform.position,
                EulerAngles = transform.eulerAngles,
                Scale = transform.localScale,
            };
    
            GUIUtility.systemCopyBuffer = GetSerializedTransformData(copiedTransformData);
        }
    }

    [MenuItem("CONTEXT/Transform/Copy/Copy Local Transform Data")]
    public static void CreateLocalTransformData(MenuCommand menuCommand)
    {
        Transform transform = menuCommand.context as Transform;

        if (transform != null)
        {
            CopiedTransformData copiedTransformData = new CopiedTransformData
            {
                Position = transform.localPosition,
                EulerAngles = transform.localEulerAngles,
                Scale = transform.localScale,
            };

            GUIUtility.systemCopyBuffer = GetSerializedTransformData(copiedTransformData);
        }
    }

    [MenuItem("CONTEXT/Transform/Paste/Paste Local Transform Data")]
    public static void PasteLocalTransformData(MenuCommand menuCommand)
    {
        Transform transform = menuCommand.context as Transform;

        if (transform != null)
        {
            CopiedTransformData copiedTransformData = JsonConvert.DeserializeObject<CopiedTransformData>(GUIUtility.systemCopyBuffer);

            transform.localPosition = copiedTransformData.Position;
            transform.localEulerAngles = copiedTransformData.EulerAngles;
            transform.localScale = copiedTransformData.Scale;
        }
    }

    [MenuItem("CONTEXT/Transform/Paste/Paste World Transform Data")]
    public static void PasteWorldTransformData(MenuCommand menuCommand)
    {
        Transform transform = menuCommand.context as Transform;
    
        if (transform != null)
        {
            CopiedTransformData copiedTransformData = JsonConvert.DeserializeObject<CopiedTransformData>(GUIUtility.systemCopyBuffer);
    
            transform.position = copiedTransformData.Position;
            transform.eulerAngles = copiedTransformData.EulerAngles;
            transform.localScale = copiedTransformData.Scale;
        }
    }

    private static string GetSerializedTransformData(CopiedTransformData copiedTransformData)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        return JsonConvert.SerializeObject(copiedTransformData, settings);
    }

}
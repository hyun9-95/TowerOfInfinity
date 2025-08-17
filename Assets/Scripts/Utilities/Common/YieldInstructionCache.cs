using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal static class YieldInstructionCache
{
    class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y)
        {
            return x == y;
        }
        int IEqualityComparer<float>.GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }

    public static readonly WaitForEndOfFrame waitForEndOfFrame = new();
    public static readonly WaitForFixedUpdate waitForFixedUpdate = new();

    private static readonly Dictionary<float, WaitForSeconds> timeInterval = new(new FloatComparer());
    private static readonly Dictionary<float, WaitForSecondsRealtime> timeIntervalReal = new(new FloatComparer());

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        WaitForSeconds waitForSeconds;

        if (!timeInterval.TryGetValue(seconds, out waitForSeconds))
            timeInterval.Add(seconds, waitForSeconds = new WaitForSeconds(seconds));
        return waitForSeconds;
    }

    public static WaitForSecondsRealtime WaitForSecondsRealtime(float seconds)
    {
        WaitForSecondsRealtime waitForSecondsRealTime;

        if (!timeIntervalReal.TryGetValue(seconds, out waitForSecondsRealTime))
        {
            timeIntervalReal.Add(seconds, waitForSecondsRealTime = new WaitForSecondsRealtime(seconds));
        }
        else
        {
            waitForSecondsRealTime.Reset();
        }
        return waitForSecondsRealTime;
    }
}
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public static class UniTaskUtils
{
    public static async UniTask DelaySeconds(float value, CancellationToken cancellationToken = default)
    {
        await UniTask.Delay((int)(value * IntDefine.MILLI_SECOND), cancellationToken: cancellationToken).SuppressCancellationThrow();
    }

    public static async UniTask DelayAction(float delay, Action action, CancellationToken cancellationToken)
    {
        await DelaySeconds(delay, cancellationToken);
        action?.Invoke();
    }

    public static async UniTask NextFrame(CancellationToken cancellationToken = default)
    {
        await UniTask.NextFrame(cancellationToken: cancellationToken).SuppressCancellationThrow();
    }

    public static async UniTask WaitForFixedUpdate(CancellationToken cancellationToken = default)
    {
        await UniTask.WaitForFixedUpdate(cancellationToken: cancellationToken).SuppressCancellationThrow();
    }
}

using Cysharp.Threading.Tasks;
using System.Threading;

public static class UniTaskUtils
{
    public static async UniTask DelaySeconds(float value, CancellationToken cancellationToken = default)
    {
        await UniTask.Delay((int)(value * IntDefine.MILLI_SECOND), cancellationToken: cancellationToken).SuppressCancellationThrow();
    }
}

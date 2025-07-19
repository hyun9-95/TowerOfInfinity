#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Threading;

public class Weapon : Ability
{
    public bool IsProcessing => isProcessing;
    private CancellationToken token;

    private bool isProcessing = false;
    public async UniTask Activate()
    {
        token = TokenPool.Get(GetHashCode());

        while (!token.IsCancellationRequested)
        {
            isProcessing = true;
            OnProcess();

            await UniTaskUtils.DelaySeconds(Model.CoolTime, cancellationToken: token);
            isProcessing = false;
        }
    }

    public async UniTask ActivateOneTime(float delay = 0)
    {
        isProcessing = true;
        token = TokenPool.Get(GetHashCode());

        if (delay > 0)
            await UniTaskUtils.DelaySeconds(delay, cancellationToken: token);

        OnProcess();
        await UniTaskUtils.DelaySeconds(Model.CoolTime, cancellationToken: token);
        isProcessing = false;
    }

    public void Cancel()
    {
        TokenPool.Cancel(GetHashCode());
    }
}

using System.Collections.Generic;
using System.Threading;

public static class TokenPool
{
    #region Coding rule : Property
    #endregion Coding rule : Property

    #region Coding rule : Value
    private static Dictionary<int, CancellationTokenSource> updateToken = new Dictionary<int, CancellationTokenSource>();
    #endregion Coding rule : Value
    #region Coding rule : Function
    public static void CancelAll()
    {
        foreach (var token in updateToken.Values)
            token.Cancel();

        updateToken.Clear();
    }

    public static void Cancel(int hashCode)
    {
        if (!updateToken.ContainsKey(hashCode))
            return;

        updateToken[hashCode].Cancel();

        updateToken.Remove(hashCode);
    }

    public static void Dispose(int hashCode)
    {
        if (!updateToken.ContainsKey(hashCode))
            return;

        updateToken[hashCode].Dispose();

        updateToken.Remove(hashCode);
    }

    public static CancellationToken Get(int hashCode)
    {
        if (!updateToken.ContainsKey(hashCode))
            updateToken.Add(hashCode, new CancellationTokenSource());

        return updateToken[hashCode].Token;
    }
    #endregion Coding rule : Function
}

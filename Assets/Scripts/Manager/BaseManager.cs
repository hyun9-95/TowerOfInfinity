
public class BaseManager<T> where T : BaseManager<T>, new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = new T();

            return instance;
        }
    }
}

namespace JSONWebTokenAPI.Authentication
{

    public static class LazyResolutions
    {
        internal class LazyService<T> : Lazy<T> 
        {
            public LazyService(IServiceProvider provider)
                : base(() => provider.GetRequiredService<T>())
            {
            }
        }

    }
}

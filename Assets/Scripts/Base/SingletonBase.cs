namespace XCHY
{
    public class SingletonBase<T> where T:new()
    {
        private static T instance;
        // 多线程安全机制
        private static readonly object locker = new object();
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                            instance = new T();
                    }
                }
                return instance;
            }
        }
    }

}
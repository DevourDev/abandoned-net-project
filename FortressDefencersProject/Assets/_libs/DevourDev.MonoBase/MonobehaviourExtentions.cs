using System;

namespace DevourDev.MonoExtentions
{
    public static class MonobehaviourExtentions
    {
        public static void InvokeOnMainThread(this object o, Action a)
        {
            MonoBase.UnityMainThreadDispatcher.InvokeOnMainThread(a);
        }
        public static void InvokeOnMainThread<T>(this object o, Action<T> a, T arg)
        {
            MonoBase.UnityMainThreadDispatcher.InvokeOnMainThread(() => a(arg));
        }
        public static void InvokeOnMainThread<T0, T1>(this object o, Action<T0, T1> a, T0 arg0, T1 arg1)
        {
            MonoBase.UnityMainThreadDispatcher.InvokeOnMainThread(() => a(arg0, arg1));
        }
        public static void InvokeOnMainThread<T0, T1, T2>(this object o, Action<T0, T1, T2> a, T0 arg0, T1 arg1, T2 arg2)
        {
            MonoBase.UnityMainThreadDispatcher.InvokeOnMainThread(() => a(arg0, arg1, arg2));
        }
    }

}

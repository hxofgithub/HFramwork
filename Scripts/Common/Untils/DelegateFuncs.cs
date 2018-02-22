namespace HFramework.Core.Delegates
{
    public delegate void DelegateAction();
    public delegate void DelegateAction<T>(T arg);
    public delegate void DelegateAction<T1,T2>(T1 arg1,T2 arg2);
    public delegate void DelegateAction<T1,T2,T3>(T1 arg1,T2 arg2,T3 arg3);
    public delegate void DelegateAction<T1,T2,T3,T4>(T1 arg1,T2 arg2,T3 arg3,T4 arg4);


    public delegate TResult DelegateFunc<TResult>();
    public delegate TResult DelegateFunc<T,TResult>(T arg);
    public delegate TResult DelegateFunc<T1,T2,TResult>(T1 arg1,T2 arg2);
    public delegate TResult DelegateFunc<T1,T2,T3,TResult>(T1 arg1,T2 arg2,T3 arg3);
    public delegate TResult DelegateFunc<T1,T2,T3,T4,TResult>(T1 arg1,T2 arg2,T3 arg3,T4 arg4);

    public delegate object DelegateFunc(params object[] args);

}
using System;


namespace ZXMAK2.Dependency
{
    public interface IResolver : IDisposable
    {
        T Resolve<T>(params Argument[] args);
        T Resolve<T>(string name, params Argument[] args);
        T TryResolve<T>(params Argument[] args);
        T TryResolve<T>(string name, params Argument[] args);
        bool CheckAvailable<T>(params Argument[] args);
        bool CheckAvailable<T>(string name, params Argument[] args);

        void RegisterInstance<T>(string name, T instance);
    }
}

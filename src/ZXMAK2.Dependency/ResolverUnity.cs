using System;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;


namespace ZXMAK2.Dependency
{
    public sealed class ResolverUnity : IResolver
    {
        private readonly IUnityContainer _container;
        private bool _isDisposed;


        public ResolverUnity()
        {
            _container = new UnityContainer();
            _container.LoadConfiguration();
            _container.RegisterInstance<IResolver>(this);
        }

        public ResolverUnity(string containerName)
        {
            _container = new UnityContainer();
            _container.LoadConfiguration(containerName);
            _container.RegisterInstance<IResolver>(this);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                // we registered in contaner, 
                // so we need to prevent reentrancy
                return;
            }
            _isDisposed = true;
            _container.Dispose();
        }

        public T Resolve<T>(params Argument[] args)
        {
            if (args.Length > 0)
            {
                var poArgs = args.Select(arg => new ParameterOverride(arg.Name, arg.Value));
                return _container.Resolve<T>(poArgs.ToArray());
            }
            return _container.Resolve<T>();
        }

        public T Resolve<T>(string name, params Argument[] args)
        {
            if (args.Length > 0)
            {
                var poArgs = args.Select(arg => new ParameterOverride(arg.Name, arg.Value));
                return _container.Resolve<T>(name, poArgs.ToArray());
            }
            return _container.Resolve<T>(name);
        }

        public T TryResolve<T>(params Argument[] args)
        {
            try
            {
                if (!CheckAvailable<T>())
                {
                    return default(T);
                }
                return Resolve<T>(args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return default(T);
            }
        }

        public T TryResolve<T>(string name, params Argument[] args)
        {
            try
            {
                if (!CheckAvailable<T>(name))
                {
                    return default(T);
                }
                return Resolve<T>(name, args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return default(T);
            }
        }

        public bool CheckAvailable<T>(params Argument[] args)
        {
            return _container.IsRegistered<T>();
        }

        public bool CheckAvailable<T>(string name, params Argument[] args)
        {
            return _container.IsRegistered<T>(name);
        }

        public void RegisterInstance<T>(string name, T instance)
        {
            _container.RegisterInstance<T>(name, instance);
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Presentation.Interfaces;
using System.ComponentModel;


namespace ZXMAK2.Host.Presentation
{
    public class Launcher : ILauncher
    {
        private readonly IResolver m_resolver;
        
        public Launcher(IResolver resolver)
        {
            m_resolver = resolver;
        }
        
        public void Run(string[] args)
        {
            var service = m_resolver.TryResolve<IUserMessage>();
            try
            {
                args = ProcessHostOption(args);
                var viewResolver = m_resolver.Resolve<IResolver>("View");
                var view = viewResolver.Resolve<IMainView>();
                if (view==null)
                {
                    if(service != null)
                    {
                        service.Error("Cannot create IMainView");
                    }
                    return;
                }
                using (view)
                {
                    var list = new List<Argument>();
                    list.Add(new Argument("view", view));
                    list.Add(new Argument("args", args));
                    using (var viewModel = m_resolver.Resolve<IMainViewModel>(list.ToArray()))
                    {
                        var synchronizeInvoke = view as ISynchronizeInvoke;
                        if (synchronizeInvoke != null)
                        {
                            viewModel.Attach(synchronizeInvoke);
                        }
                        view.DataContext = viewModel;
                        viewModel.Run();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                if (service != null)
                {
                    service.ErrorDetails(ex);
                }
            }
        }

        private string[] ProcessHostOption(string[] args)
        {
            var argsList = new List<string>(args);
            var hostSwitch = argsList.FirstOrDefault(arg => arg.StartsWith("/host:", StringComparison.InvariantCultureIgnoreCase));
            if (hostSwitch != null)
            {
                argsList.Remove(hostSwitch);
                hostSwitch = hostSwitch.Substring(6);
                m_resolver.RegisterInstance<string>("viewType", hostSwitch);
                return argsList.ToArray();
            }
            return args;
        }
    }
}

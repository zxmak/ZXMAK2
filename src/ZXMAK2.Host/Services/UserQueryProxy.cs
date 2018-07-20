using System;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;


namespace ZXMAK2.Host.Services
{
    public class UserQueryProxy : IUserQuery
    {
        private readonly IResolver m_resolver;
        
        public UserQueryProxy(IResolver resolver)
        {
            m_resolver = resolver;
        }


        public DlgResult Show(
            string message, 
            string caption, 
            DlgButtonSet buttonSet, 
            DlgIcon icon)
        {
            var service = GetService();
            if (service == null)
            {
                return DlgResult.Cancel;
            }
            return service.Show(message, caption, buttonSet, icon);
        }

        public bool QueryText(
            string caption, 
            string text, 
            ref string value)
        {
            var service = GetService();
            if (service == null)
            {
                return false;
            }
            return service.QueryText(caption, text, ref value);
        }

        public bool QueryValue(
            string caption, 
            string text, 
            string format, 
            ref int value, 
            int min, 
            int max)
        {
            var service = GetService();
            if (service == null)
            {
                return false;
            }
            return service.QueryValue(caption, text, format, ref value, min, max);
        }

        public object ObjectSelector(object[] objArray, string caption)
        {
            var service = GetService();
            if (service == null)
            {
                return null;
            }
            return service.ObjectSelector(objArray, caption);
        }

        private IUserQuery GetService()
        {
            var viewResolver = m_resolver.TryResolve<IResolver>("View");
            if (viewResolver == null)
            {
                return null;
            }
            var service = viewResolver.TryResolve<IUserQuery>();
            if (service != null && service.GetType() == GetType())
            {
                Logger.Error("UserQueryProxy: circular dependency detected");
                return null;
            }
            return service;
        }
    }
}

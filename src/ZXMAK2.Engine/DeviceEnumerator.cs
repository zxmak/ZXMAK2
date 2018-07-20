using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Engine.Entities;
using System.Xml;


namespace ZXMAK2.Engine
{
    public static class DeviceEnumerator
    {
        private static readonly object s_syncRoot = new object();
        private static IList<BusDeviceDescriptor> s_descriptors;

        public static IList<BusDeviceDescriptor> Descriptors
        {
            get
            {
                lock (s_syncRoot)
                {
                    if (s_descriptors == null)
                    {
                        Refresh();
                    }
                }
                return s_descriptors;
            }
        }
        
        public static void Refresh()
        {
            lock (s_syncRoot)
            {
                PreLoadPlugins();
                var listDescriptors = new List<BusDeviceDescriptor>();
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        var refName = typeof(BusDeviceBase).Assembly.GetName().FullName;
                        var hasRef = asm.GetName().FullName==refName ||
                            asm.GetReferencedAssemblies()
                                .Any(name => name.FullName == refName);
                        if (!hasRef)
                        {
                            // skip assemblies without reference on assembly which contains BusDeviceBase 
                            continue;
                        }
                        foreach (var type in asm.GetTypes())
                        {
                            if (type.IsClass &&
                                !type.IsAbstract &&
                                typeof(BusDeviceBase).IsAssignableFrom(type))
                            {
                                try
                                {
                                    BusDeviceBase device = (BusDeviceBase)Activator.CreateInstance(type);
                                    var bdd = new BusDeviceDescriptor(
                                        type,
                                        device.Category,
                                        device.Name,
                                        device.Description);
                                    listDescriptors.Add(bdd);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex, type.FullName);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, asm.FullName);
                    }
                }
                s_descriptors = listDescriptors;
            }
        }

        public static IEnumerable<BusDeviceDescriptor> SelectWithout(
            IEnumerable<Type> ignoreList)
        {
            var list = new List<BusDeviceDescriptor>();
            foreach (var bdd in Descriptors)
            {
                var ignore = false;
                foreach (var type in ignoreList)
                {
                    if (type==bdd.Type)
                    {
                        ignore = true;
                        break;
                    }
                }
                if (!ignore)
                {
                    list.Add(bdd);
                }
            }
            return list;
        }

        public static IEnumerable<BusDeviceDescriptor> SelectByCategoryWithout(
            BusDeviceCategory category, 
            IEnumerable<Type> ignoreList)
        {
            var list = new List<BusDeviceDescriptor>();
            foreach (var bdd in Descriptors)
            {
                if (bdd.Category != category)
                {
                    continue;
                }
                var ignore = false;
                foreach (var type in ignoreList)
                {
                    if (type == bdd.Type)
                    {
                        ignore = true;
                        break;
                    }
                }
                if (!ignore)
                {
                    list.Add(bdd);
                }
            }
            return list;
        }

        public static IEnumerable<BusDeviceDescriptor> SelectByType<T>()
        {
            var list = new List<BusDeviceDescriptor>();
            foreach (var bdd in Descriptors)
            {
                if (typeof(T).IsAssignableFrom(bdd.Type))
                {
                    list.Add(bdd);
                }
            }
            return list;
        }

        #region Private

        private static void PreLoadPlugins()
        {
            var configName = Path.Combine(Utils.GetAppFolder(), "plugins.config");
            if (!File.Exists(configName))
            {
                return;
            }
            var xml = new XmlDocument();
            xml.Load(configName);
            var asms = xml.DocumentElement.ChildNodes
                .OfType<XmlNode>()
                .Where(node=>string.Compare(node.Name, "Assembly", true)==0);
            foreach (var node in asms)
            {
                var path = node.Attributes
                    .OfType<XmlAttribute>()
                    .FirstOrDefault(attr=>string.Compare(attr.Name, "path", true)==0);
                if (path==null)
                {
                    continue;
                }
                var fileName = path.InnerXml;
                if (string.IsNullOrEmpty(fileName) ||
                    !File.Exists(fileName))
                {
                    continue;
                }
                try
                {
                    Assembly.LoadFrom(fileName);
                }
                catch (Exception ex)
                {
                    Logger.Error (ex);
                    Locator.Resolve<IUserMessage>()
                        .Warning("Load plugin failed!\n\n{0}", fileName);
                }
            }

            //var folderName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //LoadAssemblies(folderName, "ZXMAK2.Hardware*.dll", SearchOption.TopDirectoryOnly);
            //folderName = Path.Combine(folderName, "Plugins");
            //LoadAssemblies(folderName, "*.dll", SearchOption.AllDirectories);
        }

        //private static void LoadAssemblies(string folderName, string pattern, SearchOption options)
        //{
        //    if (!Directory.Exists(folderName))
        //    {
        //        return;
        //    }
        //    foreach (var fileName in Directory.GetFiles(folderName, pattern, options))
        //    {
        //        try
        //        {
        //            var asm = Assembly.LoadFrom(fileName);
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.Error(ex);
        //            Locator.Resolve<IUserMessage>()
        //                .Warning("Load plugin failed!\n\n{0}", fileName);
        //        }
        //    }
        //}

        #endregion Private
    }
}

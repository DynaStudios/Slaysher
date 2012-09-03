using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SlaysherNetworking.Plugin
{
    public class MissingPluginException : Exception
    {
        public MissingPluginException(string message)
            : base(message)
        {
        }
    }

    public interface IPlugin<T>
    {
        void Init(T extedable);
    }

    public class PluginLoader<T>
    {
        public bool UseRelativePath { get; set; }
        public string SearchPath { get; set; }
        public string PluginExtension { get; set; }

        private List<IPlugin<T>> plugins = new List<IPlugin<T>>();

        public PluginLoader()
        {
            UseRelativePath = true;
        }

        public void LoadPlugins ()
        {
            FileInfo[] potentialPluginFiles = GetPotentialPluginFiles();
            if (potentialPluginFiles == null)
            {
                return;
            }
            foreach (FileInfo potentialPluginFile in potentialPluginFiles)
            {
                TryLoadingPluginsFromFile(potentialPluginFile);
            }
        }

        public void InitPlugins(T extendable)
        {
            foreach (IPlugin<T> plugin in plugins)
            {
                plugin.Init(extendable);
            }
        }

        private bool ImplementsIServerPlugin(Type type)
        {
            Type iServerPlugin = typeof(IPlugin<T>);
            Type[] interfaces = type.GetInterfaces();
            foreach (Type inter in interfaces)
            {
                if (inter == iServerPlugin)
                {
                    return true;
                }
            }
            return false;
        }

        private void LoadPluginFromType(Type type)
        {
            ConstructorInfo ctor = type.GetConstructor(new Type[0]);
            IPlugin<T> plugin = ctor.Invoke(new object[0]) as IPlugin<T>;
            if (plugin != null)
            {
                plugins.Add(plugin);
            }
        }

        private void LoadPluginsFromTypes(Type[] types)
        {
            foreach (Type type in types)
            {
                if (ImplementsIServerPlugin(type))
                {
                    LoadPluginFromType(type);
                }
            }
        }

        private void TryLoadingPluginsFromFile(FileInfo potentialPluginFile)
        {
            Assembly assembly;
            try {
                assembly = Assembly.LoadFile(potentialPluginFile.FullName);
            } catch {
                return;
            }
            LoadPluginsFromTypes(assembly.GetTypes());
        }

        private DirectoryInfo GetSearchPathAsDirectoryInfo()
        {
            string path;
            if (SearchPath == null) {
                path = GetExecutingDir();
            } else if (UseRelativePath) {
                path = Path.Combine(GetExecutingDir(), SearchPath);
            } else {
                path = SearchPath;
            }

            return new DirectoryInfo(path);
        }

        private FileInfo[] GetPotentialPluginFiles()
        {
            DirectoryInfo path = GetSearchPathAsDirectoryInfo();
            if (!path.Exists)
            {
                return null;
            }
            string pluginExtension = PluginExtension ?? "dll";

            return path.GetFiles(string.Format("*.{0}", pluginExtension));
        }

        private string GetExecutingDir()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            FileInfo executingFile = new FileInfo(executingAssembly.Location);

            return executingFile.Directory.FullName;
        }
    }
}

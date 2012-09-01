using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SlaysherServer
{
    public class MissingPluginException : Exception
    {
        public MissingPluginException(string message)
            : base(message)
        {
        }
    }

    public interface IServerPlugin
    {
        void Init(Server server);
    }

    public class PluginLoader
    {
        public bool UseRelativePath { get; set; }
        public string SearchPath { get; set; }
        public string PluginExtension { get; set; }

        private List<IServerPlugin> plugins = new List<IServerPlugin>();

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

        public void InitPlugins(Server server)
        {
            foreach (IServerPlugin plugin in plugins)
            {
                plugin.Init(server);
            }
        }

        private bool ImplementsIServerPlugin(Type type)
        {
            Type iServerPlugin = typeof(IServerPlugin);
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
            IServerPlugin plugin = ctor.Invoke(new object[0]) as IServerPlugin;
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

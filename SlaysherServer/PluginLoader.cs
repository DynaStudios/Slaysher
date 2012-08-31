using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SlaysherServer
{
    public interface IServerPlugin
    {
        void Init(Server server);
    }

    public class PluginLoader
    {
        public Server server;
        public string SearchPath { get; set; }
        public string PluginExtension { get; set; }

        private List<IServerPlugin> plugins = new List<IServerPlugin>();

        public PluginLoader (Server server)
        {
            this.server = server;
        }

        public void LoadPlugins ()
        {
            FileInfo[] potentialPluginFiles = GetPotentialPluginFiles();
            foreach (FileInfo potentialPluginFile in potentialPluginFiles)
            {
                TryLoadingPluginsFromFile(potentialPluginFile);
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
            if (plugin == null)
            {
                return;
            }
            plugin.Init(server);
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

        private FileInfo[] GetPotentialPluginFiles()
        {
            DirectoryInfo path = new DirectoryInfo(SearchPath ?? GetExecutingDir());
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

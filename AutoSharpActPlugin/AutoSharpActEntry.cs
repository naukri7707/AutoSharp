using Advanced_Combat_Tracker;
using AutoSharp;
using System.IO;
using System.Linq;
using System;
using System.Windows.Forms;

namespace AutoSharpActPlugin
{
    public class AutoSharpActEntry : IActPluginV1
    {
        private string pluginsFolder;

        private string modulesFolder;

        public string PluginsFolder => pluginsFolder;

        public string ModulesFolder => modulesFolder;

        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            pluginScreenSpace.Text = "Auto Sharp";
            pluginScreenSpace.Controls.Add(new AutoSharpGUI(this));
            // Create the plugins and modules folder
            var rootFolder = GetPluginDirectory();
            pluginsFolder = Path.Combine(rootFolder, "Plugins");
            modulesFolder = Path.Combine(rootFolder, "Modules");
            Directory.CreateDirectory(PluginsFolder);
            Directory.CreateDirectory(modulesFolder);
            // Resolve assemblies in the plugins folder
            var resolver = new AssemblyResolver(rootFolder, PluginsFolder);
            resolver.Resolve();
            // Start AutoSharp life cycle, we need to start it at another method, otherwise the resolver can not resolve the assemblies
            StartAutoSharp();
            pluginStatusText.Text = "Plugin Inited.";
        }

        public void Reload()
        {
            StopAutoSharp();
            StartAutoSharp();
        }

        public void DeInitPlugin()
        {
            StopAutoSharp();
        }

        private void StartAutoSharp()
        {
            AutoSharpLifeCycle.Start(OnAutoSharp_Init, OnAutoSharp_DeInit);
        }

        private void StopAutoSharp()
        {
            AutoSharpLifeCycle.Stop();
        }

        private void OnLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            Notify.Broadcast(logInfo.logLine);
        }

        private void OnAutoSharp_Init()
        {
            // Load and enable modules
            ModuleLoader.LoadAndEnableAllModule(modulesFolder);

            // Register the log line event and AutoSharp init
            ActGlobals.oFormActMain.OnLogLineRead += OnLogLineRead;
        }

        private void OnAutoSharp_DeInit()
        {
        }

        private string GetPluginDirectory()
        {
            var plugin = ActGlobals.oFormActMain.ActPlugins.Where(it => it.pluginObj == this).FirstOrDefault()
                ?? throw new Exception("plugin not found.");

            return Path.GetDirectoryName(plugin.pluginFile.FullName);
        }
    }
}

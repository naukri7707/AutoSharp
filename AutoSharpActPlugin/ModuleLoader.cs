using AutoSharp;
using System.IO;

namespace AutoSharpActPlugin
{
    public static class ModuleLoader
    {
        public static void Load(string moduleFolder)
        {
            var dllFiles = Directory.GetFiles(moduleFolder, "*.dll", SearchOption.AllDirectories);
            foreach (var dllFile in dllFiles)
            {
                Module.Create(dllFile);
            }
        }

        public static void LoadAndEnableAllModule(string moduleFolder)
        {
            var dllFiles = Directory.GetFiles(moduleFolder, "*.dll", SearchOption.AllDirectories);
            // load all modules
            foreach (var dllFile in dllFiles)
            {
                Module.Create(dllFile);
            }
            // enable all modules
            foreach (var module in Module.GetModules())
            {
                foreach (var component in module.GetComponents<Component>())
                {
                    component.Enabled = true;
                }

                module.Enabled = true;
            }
        }
    }
}

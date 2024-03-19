using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoSharpActPlugin
{
    public class AssemblyResolver
    {
        public AssemblyResolver(params string[] pluginsFolders)
        {
            this.pluginsFolders = pluginsFolders;
        }

        private string[] pluginsFolders;

        public void Resolve()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // Get the name of the assembly being requested
            string assemblyName = args.Name.Split(',')[0];

            foreach (var pluginsFolder in pluginsFolders)
            {
                // Construct the path to search for the assembly
                string assemblyPath = Path.Combine(pluginsFolder, assemblyName + ".dll");

                // Check if the assembly exists in the specified folder
                if (File.Exists(assemblyPath))
                {
                    try
                    {
                        // Load and return the assembly
                        return Assembly.LoadFrom(assemblyPath);
                    }
                    catch (Exception ex)
                    {
                        // Handle any potential exceptions
                        Console.WriteLine("Error loading assembly: " + ex.Message);
                    }
                }
            }
            // If the assembly is not found or cannot be loaded, return null
            return null;
        }
    }
}

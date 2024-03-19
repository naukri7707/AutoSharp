using AutoSharp;
using AutoSharp.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AutoSharpDevTester
{
    internal class Program
    {
        internal static readonly char[] separator = new[] { ' ' };

        private static bool running = false;

        private static Assembly _autoSharpDevEnvAssembly;

        private static Assembly AutoSharpDevEnvAssembly
        {
            get
            {
                if (_autoSharpDevEnvAssembly == null)
                {
                    // load AutoSharpDevEnv assembly
                    var testLoader = typeof(AutoSharpDevEnv.Tests.TestLoader);
                    var asm = AppDomain.CurrentDomain.GetAssemblies();
                    _autoSharpDevEnvAssembly = AppDomain.CurrentDomain.GetAssemblies().First(it => it.GetName().Name == "AutoSharpDevEnv");
                }
                return _autoSharpDevEnvAssembly;
            }
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("Welcome to AutoSharp Develop Tester!");
            AutoSharpLifeCycle.Start(() => running = true);

            while (!running)
                ;

            var module = CreateTestModule();

            Console.WriteLine("AutoSharp started.");

            for (; ; )
            {
                Console.Write("> ");
                var cmdtext = Console.ReadLine();
                var cmd = SplitCommandLine(cmdtext);
                var method = cmd[0].ToLower();
                var param = cmd.Skip(1).ToArray();

                if (method == "send")
                {
                    if (param.Length > 0)
                    {
                        var log = param[0];
                        Notify.Broadcast(log);
                    }
                }
                else if (method == "loc")
                {
                    var loc = AutoSharpDevEnvAssembly.Location;

                    if (param.Length > 0)
                    {
                        if (param[0].Equals("-o", StringComparison.CurrentCultureIgnoreCase))
                        {
                            var dir = System.IO.Path.GetDirectoryName(loc);
                            Process.Start(dir);
                        }
                    }

                    Console.WriteLine(loc);
                }
            }
        }

        private static string[] SplitCommandLine(string cmdtext)
        {
            var args = new List<string>();
            var inQuote = false;
            var currentArg = "";

            for (var i = 0; i < cmdtext.Length; i++)
            {
                var c = cmdtext[i];

                if (c == ' ' && !inQuote)
                {
                    if (!string.IsNullOrEmpty(currentArg))
                    {
                        args.Add(currentArg);
                        currentArg = "";
                    }
                }
                else if (c == '"' || c == '\'')
                {
                    inQuote = !inQuote;
                }
                else
                {
                    currentArg += c;
                }
            }

            if (!string.IsNullOrEmpty(currentArg))
            {
                args.Add(currentArg);
            }

            return args.ToArray();
        }

        private static AutoSharp.Module CreateTestModule()
        {
            var moudule = new AutoSharp.Module("Test");
            //
            var componentType = typeof(Component);
            foreach (var type in AutoSharpDevEnvAssembly.GetTypes())
            {
                if (type.IsSubclassOf(componentType) && !type.IsAbstract && !type.IsGenericType)
                {
                    var component = moudule.AddComponent(type);
                    component.Enabled = true;
                }
            }
            moudule.Enabled = true;

            return moudule;
        }
    }
}

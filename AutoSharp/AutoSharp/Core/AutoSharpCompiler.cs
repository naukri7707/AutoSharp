// ================================
// This code requires Microsoft.CodeAnalysis.CSharp (Roslyn). for simplicity, we remove the reference and comment out the code.
// The version of Microsoft.CodeAnalysis.CSharp using before is 4.3.1
// ================================
//
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.Text;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.Versioning;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace AutoSharp.Core
//{
//    /// <summary>
//    /// The compiler of AutoSharp scripts.
//    /// </summary>
//    public partial class AutoSharpCompiler : IDisposable
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="AutoSharpCompiler"/> class
//        /// </summary>
//        public AutoSharpCompiler()
//            : this("") { }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="AutoSharpCompiler"/> class
//        /// with <paramref name="outputDirectory"/>
//        /// </summary>
//        /// <param name="outputDirectory">The directory of file to which the PE image will be written.</param>
//        public AutoSharpCompiler(string outputDirectory)
//        {
//            this.outputDirectory = outputDirectory;
//        }

//        public OnCompiledEvent OnCompiled;

//        private readonly HashSet<string> namespaces = new HashSet<string>(DefaultNamespaces);

//        private readonly string outputDirectory;

//        private readonly HashSet<MetadataReference> references = new HashSet<MetadataReference>(DefaultReferences);

//        private CancellationTokenSource cts;

//        /// <summary>
//        /// Compile DLL from <paramref name="codes"/>.
//        /// </summary>
//        /// <param name="assemblyName">The compiled <see cref="Assembly"/> name.</param>
//        /// <param name="codes">The source codes.</param>
//        public void Compile(string assemblyName, params string[] codes)
//        {
//            Compile(assemblyName, codes.AsEnumerable());
//        }

//        /// <summary>
//        /// Compile DLL from <paramref name="codes"/>.
//        /// </summary>
//        /// <param name="assemblyName">The compiled <see cref="Assembly"/> name.</param>
//        /// <param name="codes">The source codes.</param>
//        public void Compile(string assemblyName, IEnumerable<string> codes)
//        {
//            CompileImpl(assemblyName, "dll", codes.AsEnumerable(), OutputKind.DynamicallyLinkedLibrary);
//        }

//        /// <summary>
//        /// Compile EXE from <paramref name="codes"/>.
//        /// </summary>
//        /// <param name="assemblyName">The compiled <see cref="Assembly"/> name.</param>
//        /// <param name="codes">The source codes.</param>
//        public void CompileExe(string assemblyName, params string[] codes)
//        {
//            CompileExe(assemblyName, codes.AsEnumerable());
//        }

//        /// <summary>
//        /// Compile EXE from <paramref name="codes"/>.
//        /// </summary>
//        /// <param name="assemblyName">The compiled <see cref="Assembly"/> name.</param>
//        /// <param name="codes">The source codes.</param>
//        public void CompileExe(string assemblyName, IEnumerable<string> codes)
//        {
//            var codesWithEntry = codes.Append(EntryCode(assemblyName));
//            CompileImpl(assemblyName, "exe", codesWithEntry.AsEnumerable(), OutputKind.ConsoleApplication);
//        }

//        /// <summary>
//        /// Compile EXE from <paramref name="directory"/>.
//        /// </summary>
//        /// <param name="assemblyName">The compiled <see cref="Assembly"/> name.</param>
//        /// <param name="directory">The directory of the source codes.</param>
//        /// <param name="includeSubDirectory">Include sub folder inside <paramref name="directory"/>.</param>
//        public void CompileExeFromDirectory(string assemblyName, string directory, bool includeSubDirectory = false)
//        {
//            var searchOption = includeSubDirectory ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
//            var codes = Directory.EnumerateFiles(directory, "*.cs", searchOption).Select(it => File.ReadAllText(it));
//            CompileExe(assemblyName, codes);
//        }

//        /// <summary>
//        /// Compile EXE from <paramref name="files"/>.
//        /// </summary>
//        /// <param name="assemblyName">The compiled <see cref="Assembly"/> name.</param>
//        /// <param name="files">The paths of the source code files.</param>
//        public void CompileExeFromFiles(string assemblyName, params string[] files)
//        {
//            var codes = files.Select(it => File.ReadAllText(it));
//            CompileExe(assemblyName, codes);
//        }

//        /// <summary>
//        /// Compile DLL from <paramref name="directory"/>.
//        /// </summary>
//        /// <param name="assemblyName">The compiled <see cref="Assembly"/> name.</param>
//        /// <param name="directory">The directory of the source codes.</param>
//        /// <param name="includeSubDirectory">Include sub folder inside <paramref name="directory"/>.</param>
//        public void CompileFromDirectory(string assemblyName, string directory, bool includeSubDirectory = false)
//        {
//            var searchOption = includeSubDirectory ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
//            var codes = Directory.EnumerateFiles(directory, "*.cs", searchOption).Select(it => File.ReadAllText(it));
//            Compile(assemblyName, codes);
//        }

//        /// <summary>
//        /// Compile DLL from <paramref name="files"/>.
//        /// </summary>
//        /// <param name="assemblyName">The compiled <see cref="Assembly"/> name.</param>
//        /// <param name="files">The paths of the source code files.</param>
//        public void CompileFromFiles(string assemblyName, params string[] files)
//        {
//            var codes = files.Select(it => File.ReadAllText(it));
//            Compile(assemblyName, codes);
//        }

//        public void Dispose()
//        {
//            cts?.Dispose();
//            GC.SuppressFinalize(this);
//        }

//        /// <summary>
//        /// Stop compiling.
//        /// </summary>
//        public void StopCompile()
//        {
//            cts?.Cancel();
//        }

//        /// <summary>
//        /// Add assembly references to compiler.
//        /// </summary>
//        /// <param name="assemblies">The assembly references.</param>
//        /// <returns>The instance of <see cref="AutoSharpCompiler"/> self.</returns>
//        public AutoSharpCompiler WithAssemblyReferences(params Assembly[] assemblies)
//        {
//            var locations = assemblies.Select(it => it.Location).ToArray();
//            return WithFileReferences(locations);
//        }

//        /// <summary>
//        /// Add assembly reference files to compiler by path.
//        /// </summary>
//        /// <param name="referencesFiles">The assembly reference files path.</param>
//        /// <returns>The instance of <see cref="AutoSharpCompiler"/> self.</returns>
//        public AutoSharpCompiler WithFileReferences(params string[] referencesFiles)
//        {
//            foreach (var referenceFile in referencesFiles)
//            {
//                var reference = MetadataReference.CreateFromFile(referenceFile);
//                references.Add(reference);
//            }
//            return this;
//        }

//        /// <summary>
//        /// Add global using namespace to compiler.
//        /// </summary>
//        /// <param name="imports">The import namespace name.</param>
//        /// <returns>The instance of <see cref="AutoSharpCompiler"/> self.</returns>
//        public AutoSharpCompiler WithImports(params string[] imports)
//        {
//            foreach (var import in imports)
//            {
//                namespaces.Add(import);
//            }
//            return this;
//        }

//        private async void CompileImpl(string assemblyName, string extensionName, IEnumerable<string> codes, OutputKind outputKind)
//        {
//            await Task.Yield();
//            Debug.Log($"Compiling module '{assemblyName}'...");
//            var syntaxTrees = GetSyntaxTrees(codes);
//            var compilationOptions = new CSharpCompilationOptions(outputKind)
//               .WithOverflowChecks(true)
//               .WithOptimizationLevel(OptimizationLevel.Release)
//               .WithUsings(namespaces);
//            //
//            var compilation = CSharpCompilation.Create(
//                assemblyName,
//                syntaxTrees,
//                references,
//                compilationOptions
//                );
//            using (cts = new CancellationTokenSource())
//            {
//                var path = Path.Combine(outputDirectory, $@"{assemblyName}.{extensionName}");
//                var emitResult = compilation.Emit(
//                    path,
//                    cancellationToken: cts.Token
//                    );
//                if (emitResult.Success)
//                {
//                    var args = new OnCompiledArgs(OnCompiledArgs.Result.Success, assemblyName, path);
//                    OnCompiled.Invoke(args);
//                }
//                else
//                {
//                    var args = new OnCompiledArgs(OnCompiledArgs.Result.Fail, assemblyName, path);
//                    OnCompiled.Invoke(args);
//                    foreach (var diagnostic in emitResult.Diagnostics)
//                    {
//                        if (diagnostic.Severity == DiagnosticSeverity.Error)
//                        {
//                            var e = diagnostic.GetMessage();
//                            throw new Exception(e);
//                        }
//                    }
//                }
//            }
//            cts = null;
//        }

//        public delegate void OnCompiledEvent(OnCompiledArgs args);
//    }

//    partial class AutoSharpCompiler
//    {
//        private static readonly string framework = Assembly.GetEntryAssembly().GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName;

//        private static readonly string libraryDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);

//        /// <summary>
//        /// The framework of application.
//        /// </summary>
//        public static string Framework => framework;

//        private static string[] DefaultNamespaces => new[]
//        {
//            "System",
//            "System.Collections.Generic",
//            "System.Linq",
//        };

//        private static MetadataReference[] DefaultReferences => new MetadataReference[]
//                {
//            MetadataReference.CreateFromFile(Path.Combine(libraryDirectory, "mscorlib.dll")),
//            MetadataReference.CreateFromFile(Path.Combine(libraryDirectory, "System.Runtime.dll")),
//            MetadataReference.CreateFromFile(Path.Combine(libraryDirectory, "System.Core.dll")),
//            MetadataReference.CreateFromFile(Path.Combine(libraryDirectory, "System.dll")),
//            MetadataReference.CreateFromFile(typeof(object).Assembly.Location), // System.Private.CoreLib.dll (NET 60)
//            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location), // System.Linq
//        };

//        private static string EntryCode(string namespaceName)
//        {
//            return
//$@"using System;
//using System.Reflection;
//using AutoSharpCore;

//namespace {namespaceName}
//{{
//    public static class Entry
//    {{
//        static void Main(string[] args)
//        {{
//            AutoSharpLifeCycle.Start(() =>
//            {{
//                var asm = Assembly.GetEntryAssembly();
//                var module = AutoSharpCore.Module.Load(asm);
//                module.Enable = true;
//                foreach (var component in module.GetComponents())
//                {{
//                    component.Enable = true;
//                }}
//            }});
//            for (; ; )
//            {{
//                var msg = Console.ReadLine();
//                Notify.Broadcast(msg);
//            }}
//        }}
//    }}
//}}
//";
//        }

//        private static IEnumerable<SyntaxTree> GetSyntaxTrees(IEnumerable<string> codes)
//        {
//            foreach (var code in codes)
//            {
//                var assemblyCSharpCode = SourceText.From(code, Encoding.UTF8);
//                yield return SyntaxFactory.ParseSyntaxTree(
//                    assemblyCSharpCode,
//                    CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3),
//                    ""
//                    );
//            }
//        }
//    }
//}

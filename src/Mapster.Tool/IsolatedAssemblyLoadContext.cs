using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using CommandLine;
using ExpressionDebugger;
using Mapster.Models;
using Mapster.Utils;
using System;
using System.Reflection;
using System.Runtime.Loader;

namespace Mapster.Tool
{
    public class IsolatedAssemblyContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver resolver;

        public IsolatedAssemblyContext(string assemblyPath)
        {
            resolver = new AssemblyDependencyResolver(assemblyPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }

        public static Assembly LoadAssemblyFrom(string relativePath)
        {
            // Navigate up to the solution root
            string root = Path.GetFullPath(
                Path.Combine(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(
                                    Path.GetDirectoryName(typeof(Program).Assembly.Location)
                                )
                            )
                        )
                    )
                )
            );

            string assemblyLocation = Path.GetFullPath(
                Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar))
            );
            Console.WriteLine($"Loading commands from: {assemblyLocation}");
            IsolatedAssemblyContext loadContext = new IsolatedAssemblyContext(assemblyLocation);
            return loadContext.LoadFromAssemblyName(
                new AssemblyName(Path.GetFileNameWithoutExtension(assemblyLocation))
            );
        }
    }
}

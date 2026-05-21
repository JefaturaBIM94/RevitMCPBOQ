using System;
using System.IO;
using System.Reflection;

namespace NavisBOQ.Revit.Plugin.Infrastructure
{
    public static class PluginAssemblyResolver
    {
        private static bool _registered;

        public static void Register()
        {
            if (_registered)
                return;

            _registered = true;

            AppDomain.CurrentDomain.AssemblyResolve += ResolveFromPluginFolder;
        }

        private static Assembly ResolveFromPluginFolder(object sender, ResolveEventArgs args)
        {
            try
            {
                AssemblyName requestedAssembly = new AssemblyName(args.Name);

                string pluginDir = Path.GetDirectoryName(
                    typeof(PluginAssemblyResolver).Assembly.Location);

                if (string.IsNullOrWhiteSpace(pluginDir))
                    return null;

                string candidatePath = Path.Combine(
                    pluginDir,
                    requestedAssembly.Name + ".dll");

                if (!File.Exists(candidatePath))
                    return null;

                return Assembly.LoadFrom(candidatePath);
            }
            catch
            {
                return null;
            }
        }
    }
}

using System.Runtime.Loader;

namespace UnityCommander.SystemMetrics
{
    public static class PluginUnloadDebugger
    {
        private static readonly Dictionary<string, WeakReference> _pluginReferences = new();

        public static void MonitorUnload(
            AssemblyLoadContext loadContext,
            string pluginId)
        {
            if (loadContext == null || string.IsNullOrEmpty(pluginId))
                return;

            _pluginReferences[pluginId] =
                new WeakReference(loadContext, trackResurrection: true);
        }

        public static bool IsUnloaded(string pluginId)
        {
            if (!_pluginReferences.TryGetValue(pluginId, out var weakRef))
                return false;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            if (!weakRef.IsAlive)
            {
                _pluginReferences.Remove(pluginId);
                return true;
            }

            return false;
        }

        public static Dictionary<string, bool> CheckAll()
        {
            var result = new Dictionary<string, bool>();

            foreach (var pluginId in _pluginReferences.Keys.ToList())
            {
                result[pluginId] = IsUnloaded(pluginId);
            }

            return result;
        }
    }
}

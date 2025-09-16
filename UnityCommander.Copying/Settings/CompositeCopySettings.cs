using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Settings
{
    public enum SettingPriority
    {
        Default = 100,
        Session = 200,
        User = 300
    }

    public class CompositeCopySettings : ICopySetting
    {
        private readonly List<(int Priority, Action<CopyOptions> Action)> _applyActions = new();

        public CompositeCopySettings(IEnumerable<(int Priority, Action<CopyOptions>)> applyActions)
        {
            _applyActions.AddRange(applyActions);
        }

        public CompositeCopySettings() { }

        public void Add(SettingPriority priority, Action<CopyOptions> action)
        {
            _applyActions.Add(((int)priority, action));
        }

        public void Apply(ref CopyOptions options)
        {
            foreach (var (_, action) in _applyActions.OrderBy(x => x.Priority))
            {
                action(options);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Settings
{
    public class CompositeCopySettings : ICopySetting
    {
        private readonly IEnumerable<Action<CopyOptions>> _applyActions;

        public CompositeCopySettings(IEnumerable<Action<CopyOptions>> applyActions)
        {
            _applyActions = applyActions;
        }

        public void Apply(ref CopyOptions options)
        {
            foreach (var action in _applyActions)
            {
                action(options);
            }
        }
    }
}

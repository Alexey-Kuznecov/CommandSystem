using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Settings
{
    public class CompositeCopySettings : ICopySetting
    {
        private readonly IEnumerable<ICopySetting> _settings;

        public CompositeCopySettings(IEnumerable<ICopySetting> settings)
        {
            _settings = settings;
        }

        public void Apply(ref CopyOptions options)
        {
            foreach (var setting in _settings)
            {
                setting.Apply(ref options);
            }
        }
    }
}

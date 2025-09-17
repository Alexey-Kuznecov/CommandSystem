using Svetokop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Settings;

namespace Svetokop.Services
{
    public class UserSettingsSource : ICopySetting
    {
        private readonly SettingsViewModel _vm;

        public UserSettingsSource(SettingsViewModel vm)
        {
            _vm = vm;
        }

        public void Apply(ref CopyOptions options)
        {
            options.MaxConсurrentTasks = _vm.MaxConcurrentTasks;
            options.UseMultiThreading = _vm.UseMultiThreading;
            options.OverwriteExistingFiles = _vm.OverwriteExistingFiles;
        }
    }
}

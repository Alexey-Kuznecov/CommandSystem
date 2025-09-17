using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying;

namespace CommandSystem.CopyTester.ViewModels
{
    public class HistoryViewModel
    {
        private readonly CopyManager _copyManager;

        public HistoryViewModel(CopyManager copyManager)
        {
            _copyManager = copyManager;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Strategies
{
    public class WinApiFileCopier : IFileCopier
    {
        public async Task CopyFileAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken)
        {
            // Вызов CopyFileEx через P/Invoke
            // cancellationToken проверяешь сам, т.к. WinAPI может не поддерживать отмену
        }
    }
}

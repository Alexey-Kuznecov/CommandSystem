using IOCommand.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOCommand
{
    public class CopyManager
    {
        private readonly ICopySource _source;
        private readonly ICopyDestination _destination;
        private readonly ICopyStrategy _strategy;
        private readonly CopySettings _settings;
        private readonly ICopyProgressReporter? _progressReporter;

        public CopyManager(ICopySource source, ICopyDestination destination, ICopyStrategy strategy, CopySettings settings, ICopyProgressReporter? progressReporter = null)
        {
            _source = source;
            _destination = destination;
            _strategy = strategy;
            _settings = settings;
            _progressReporter = progressReporter;
        }

        public async Task CopyAsync(CancellationToken cancellationToken = default)
        {
            _destination.Prepare();

            var files = _source.GetFiles();
            foreach (var file in files)
            {
                if (_settings.Filters.All(f => f.ShouldCopy(file)))
                {
                    var targetPath = _destination.GetTargetPath(file);
                    _strategy.Copy(file, targetPath, cancellationToken);
                    _progressReporter?.Report(file.Path);
                }
            }
        }
    }

}

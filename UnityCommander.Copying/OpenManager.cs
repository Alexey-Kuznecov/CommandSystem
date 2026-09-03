
using System.Reactive.Subjects;
using UnityCommander.Copying.Category;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Progress;
using UnityCommander.Copying.Reporting;
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Settings;
using UnityCommander.Logging.Contracts;

namespace UnityCommander.Copying
{
    public class OpenManager
    {
        private readonly IFileCopyPlanner _planner;
        private readonly ICopyExecutor _executor;
        private readonly IFileCopierFactory _copierFactory;
        private readonly IProgressTracker _tracker;
        private readonly IProgressReporter _reporter;
        private readonly IFileCategorizer _categorizer;
        private readonly ICopyMetricsCollector _metrics;
        private readonly ILogger _logger;
        private readonly Subject<ProgressInfo> _progressSubject = new();
        public IObservable<ProgressInfo> ProgressStream => _progressSubject;
        public OpenManager(
            IFileCopyPlanner planner,
            ICopyExecutor executor,
            IFileCopierFactory copierFactory,
            IProgressTracker tracker,
            IProgressReporter reporter,
            IFileCategorizer categorizer,
            ICopyMetricsCollector metrics,
            ILogger logger)
        {
            _planner = planner;
            _executor = executor;
            _copierFactory = copierFactory;
            _tracker = tracker;
            _reporter = reporter;
            _categorizer = categorizer;
            _metrics = metrics;
            _logger = logger;

            _reporter.ProgressChanged += info => _progressSubject.OnNext(info);
        }

        public async Task StartCopyAsync(
      string source,
      string target,
      CopySessionService session,
      CompositeCopySettings settings)
        {
            var options = new CopyOptions();
            settings.Apply(ref options);

            var context = new CopyContext(
                _tracker,
                _reporter, // репортер
                _metrics,
                _categorizer,
                _copierFactory,
                _logger,
                session.CancellationToken);

            Func<CancellationToken, IAsyncEnumerable<DiscoveredItem>> provider =
                options.UseProgressiveDiscovery
                    ? ct => _planner.GetDiscoveredItemsAsyncEnumerable(source, target, options, ct)
                    : ct => GetDiscoveredItemsEnumerableAsync(ct);

            async IAsyncEnumerable<DiscoveredItem> GetDiscoveredItemsEnumerableAsync(CancellationToken ct)
            {
                var result = await _planner.GetDiscoveredItems(source, target, options, ct);
                foreach (var item in result)
                    yield return item;
            }

            // --- Подготовка списка файлов для репортера ---
            var discoveredList = new List<(string source, string destination, long size)>();
            await foreach (var item in provider(session.CancellationToken))
            {
                if (item.Type == DiscoveredItemType.File) // только файлы
                {
                    discoveredList.Add((item.Source, item.Destination, item.FileSize));
                }
            }

            // Передаём весь список в репортер
            if (session.FileReporter is ICopyReporter concreteReporter)
                concreteReporter.PrepareFileList(discoveredList);

            // Теперь запускаем фактическое копирование
            await _executor.ExecuteAsync(provider, context, options, session);
        }
    }
}

using AlexeyKuznetsov.Logger;
using System;
using System.Collections.Generic;

using UnityCommander.Copying.Category;
using UnityCommander.Copying.Handler;
using UnityCommander.Copying.Progress;
using UnityCommander.Copying.Reporting;

namespace UnityCommander.Copying.Core
{
    public sealed class CopyContext
    {
        public IProgressTracker ProgressTracker { get; }
        public IProgressReporter ProgressReporter { get; }
        public ICopyMetricsCollector Metrics { get; }
        public IFileCategorizer Categorizer { get; }
        public IFileCopierFactory CopierFactory { get; }
        public ILogger Logger { get; }
        public CancellationToken CancellationToken { get; }

        // Для потокобезопасного репорта прогресса
        public void ReportProgress(DiscoveredItem item, long bytesCopied) { /* агрегирует */ }
        public void ReportError(DiscoveredItem item, Exception ex) { /* вызывает ErrorHandler */ }
        public void ReportSuccess(DiscoveredItem item) { /* вызывает SuccessHandler */ }

        public CopyContext(
            IProgressTracker tracker,
            IProgressReporter reporter,
            ICopyMetricsCollector metrics,
            IFileCategorizer categorizer,
            IFileCopierFactory copierFactory,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            ProgressTracker = tracker;
            ProgressReporter = reporter;
            Metrics = metrics;
            Categorizer = categorizer;
            CopierFactory = copierFactory;
            Logger = logger;
            CancellationToken = cancellationToken;
        }
    }
}

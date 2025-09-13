using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying;
using UnityCommander.Copying.Reporting;
using UnityCommander.Copying.Sessions;

namespace CommandSystem.CopyTester.ViewModels
{
    public class QueueManager
    {
        //private readonly ObservableCollection<QueueItem> _queue = new();

        //public IReadOnlyCollection<QueueItem> Queue => _queue;

        //public void AddToQueue(string source, string destination)
        //{
        //    _queue.Add(new QueueItem { Source = source, Destination = destination });
        //}

        //public void RemoveFromQueue(QueueItem item) => _queue.Remove(item);

        //// Выполнить копирование всех элементов очереди
        //public async Task ProcessQueueAsync(CopyManager copyManager, IProgressReporter reporter)
        //{
        //    foreach (var item in _queue)
        //    {
        //        var session = new CopySessionService
        //        {
        //            SourcePath = item.Source,
        //            TargetPath = item.Destination
        //        };

        //        reporter.ProgressChanged += info =>
        //        {
        //            session.BytesCopied = info.BytesCopied;
        //            session.FilesCopied = info.FilesCopied;
        //        };

        //        var token = new CancellationTokenSource();
        //        //await copyManager.CopyFilesAsync(session, token.Token);
        //    }
        //    _queue.Clear();
        //}
    }

    public class QueueItem
    {
        public string Source { get; set; }
        public string Destination { get; set; }
    }
}

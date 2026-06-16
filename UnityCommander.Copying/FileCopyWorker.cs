
using System.Diagnostics;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying
{
    public class FileCopyWorker
    {
        private readonly CopyContext _context;
        private readonly CopySessionService _sessionService;
        private readonly CopyOptions _options;

        public FileCopyWorker(CopyContext context, CopySessionService sessionService, CopyOptions options)
        {
            _context = context;
            _sessionService = sessionService;
            _options = options;
        }

        public async Task CopyOneAsync(DiscoveredItem item)
        {
            var copier = _context.CopierFactory.CreateFor(item, _options);
            var sw = Stopwatch.StartNew();

            _context.ProgressTracker.StartFile(item.Source, new FileInfo(item.Source).Length);
            //_sessionService.OnFileStarted(item.Source, item.Destination, item.FileSize);

            try
            {
                await copier.CopyFileAsync(
                    item.Source,
                    item.Destination,
                    _options.BufferSize,
                    async bytes =>
                    {
                        await _sessionService.Controller.WaitIfPausedAsync(_sessionService.CancellationToken);
                        _context.ProgressTracker.UpdateProgress(bytes);
                        _context.ProgressReporter.Report(_context.ProgressTracker.GetProgressInfo());
                        //_sessionService.UpdateFileProgress(item.Source, bytes);
                    },
                    _sessionService.CancellationToken,
                    _sessionService
                );
            }
            catch (OperationCanceledException)
            {
                //_sessionService.UpdateFileStatus(item.Source, FileCopyStatus.Cancelled);
            }
            catch (Exception ex)
            {
                //_sessionService.UpdateFileStatus(item.Source, FileCopyStatus.Failed);
                _context.Metrics?.OnError(item.Source, ex);
            }
            finally
            {
                _context.ProgressTracker.CompleteFile();
                //_sessionService.UpdateFileStatus(item.Source, FileCopyStatus.Completed);
                sw.Stop();
                _context.Metrics?.OnFileCopyCompleted(item.Source, item.Destination, item.FileSize, sw.Elapsed);
            }
        }
    }

}

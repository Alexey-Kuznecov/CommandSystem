
using AlexeyKuznetsov.Logger;
using System.Diagnostics;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying
{
    public static class FileCopyWorker
    {
        public static async Task CopyOneAsync(
            DiscoveredItem item,
            CopyContext ctx,
            CopyOptions opt,
            CopySessionService sessionService)
        {
            var copier = ctx.CopierFactory.CreateFor(item, opt);
            var sw = Stopwatch.StartNew();
            ctx.ProgressTracker.StartFile(item.Source, new FileInfo(item.Source).Length);
            sessionService.OnFileStarted(item.Source, item.Destination, item.FileSize);
            try
            {
                await copier.CopyFileAsync(
                item.Source, item.Destination, opt.BufferSize,
                async bytes => {
                    await sessionService.Controller.WaitIfPausedAsync(sessionService.CancellationToken);
                    ctx.ProgressTracker.UpdateProgress(bytes);
                    ctx.ProgressReporter.Report(ctx.ProgressTracker.GetProgressInfo());
                    sessionService.UpdateFileProgress(item.Source, bytes);
                },
                sessionService.CancellationToken, sessionService);
            }
            catch (OperationCanceledException)
            {
                sessionService.UpdateFileStatus(item.Source, FileCopyStatus.Cancelled);
            }
            catch (Exception ex)
            {
                sessionService.UpdateFileStatus(item.Source, FileCopyStatus.Failed);
            }
            finally
            {
                ctx.ProgressTracker.CompleteFile();
                sessionService.UpdateFileStatus(item.Source, FileCopyStatus.Completed);
                sw.Stop();
                ctx.Metrics?.OnFileCopyCompleted(item.Source, item.Destination, item.FileSize, sw.Elapsed);
            }
        }
    }
}

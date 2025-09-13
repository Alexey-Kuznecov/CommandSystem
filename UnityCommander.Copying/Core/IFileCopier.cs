namespace UnityCommander.Copying.Core
{
    public interface IFileCopier
    {
        Task CopyFileAsync(string sourcePath, string destinationPath, int bufferSize, Action<long> onBytesCopied, CancellationToken cancellationToken, Action waitIfPaused);
    }
}

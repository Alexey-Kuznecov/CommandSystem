namespace UnityCommander.Copying.Core
{
    public interface IFileFilter
    {
        bool ShouldCopy(string filePath);
    }
}

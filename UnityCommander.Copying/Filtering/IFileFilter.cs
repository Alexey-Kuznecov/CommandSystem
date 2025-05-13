
namespace UnityCommander.Copying.Filtering
{
    public interface IFileFilter
    {
        bool ShouldCopy(string filePath);
    }
}

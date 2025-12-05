
using System.Text;

namespace CommandSystem.Testing.Executor.Mock
{
    public class MockFileSystem : IFileSystem
    {
        private readonly Dictionary<string, string> _files = new Dictionary<string, string>();

        public bool Exists(string path)
        {
            return _files.ContainsKey(path);
        }

        public void CreateFile(string path, string content)
        {
            _files[path] = content;
        }

        public async Task CopyAsync(string source, string destination, CancellationToken cancellationToken)
        {
            if (!_files.ContainsKey(source))
                throw new FileNotFoundException("Source file not found.", source);

            var content = _files[source];
            var copiedContent = new StringBuilder();

            int chunkSize = 10; // Копируем по 10 символов за итерацию
            int totalChunks = (int)Math.Ceiling((double)content.Length / chunkSize);

            for (int i = 0; i < totalChunks; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                int start = i * chunkSize;
                int length = Math.Min(chunkSize, content.Length - start);
                copiedContent.Append(content.Substring(start, length));

                await Task.Delay(300, cancellationToken); // Имитируем время на копирование части данных
            }

            _files[destination] = copiedContent.ToString();
        }

        public void DeleteFile(string path)
        {
            if (_files.ContainsKey(path))
            {
                _files.Remove(path);
            }
        }
    }
}

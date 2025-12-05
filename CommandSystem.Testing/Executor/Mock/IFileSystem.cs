
namespace CommandSystem.Testing.Executor.Mock
{
    public interface IFileSystem
    {
        bool Exists(string path); // Проверка, существует ли файл
        void CreateFile(string path, string content); // Создание файла с контентом
        Task CopyAsync(string sourcePath, string destPath, CancellationToken cancellationToken); // Асинхронное копирование файла
        void DeleteFile(string path); // Удаление файла
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Category
{
    public class NeuralCategorizer : IFileCategorizer
    {
        private Dictionary<string, string> _userCorrections = new ();

        public Task<string> CategorizeAsync(FileInfo file)
        {
            string category = file.Extension.ToLower() switch
            {
                // Изображения
                ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".tiff" or ".jfif" => "Images",

                // Текстовые и документы
                ".txt" or ".doc" or ".docx" or ".pdf" or ".odt" or ".rtf" or ".chm" or ".xlsx" or ".md" or ".csv" or ".xlsm" => "Documents",

                // Архивы
                ".zip" or ".rar" or ".7z" or ".tar" or ".gz" => "Archives",

                // Программы и исполняемые файлы
                ".exe" or ".msi" or ".bat" or ".cmd" or ".sh" => "Programs",

                // Базы данных
                ".db" or ".sqlite" or ".sql" or ".mdb" => "Databases",

                // Аудио
                ".mp3" or ".wav" or ".flac" or ".aac" => "Audio",

                // Видео
                ".mp4" or ".avi" or ".mkv" or ".mov" => "Video",

                // Остальное
                _ => "Other"
            };
            return Task.FromResult(category);
        }

        public void LearnCorrection(FileInfo file, string correctCategory)
        {
            _userCorrections[file.FullName] = correctCategory;
        }
    }
}

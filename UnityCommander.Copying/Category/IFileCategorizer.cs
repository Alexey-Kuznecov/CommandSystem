using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Category
{
    public interface IFileCategorizer
    {
        //string Categorize(FileInfo file); // возвращает имя подпапки или категории
        Task<string> CategorizeAsync(FileInfo file); // для тяжелых файлов
    }
}

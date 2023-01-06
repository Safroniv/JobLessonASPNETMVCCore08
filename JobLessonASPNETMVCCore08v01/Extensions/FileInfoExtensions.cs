using System.Diagnostics;

namespace JobLessonASPNETMVCCore08v01.Extensions
{
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Метод-расширение для класса FileInfo. Позволяет выполнять запуск файла.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Process? Execute(this FileInfo file)
        {
            var processStartInfo = new ProcessStartInfo(file.FullName)
            {
                UseShellExecute = true
            };

            return Process.Start(processStartInfo);
        }

    }
}

using PottyLogger.Shared.Utilities;
using System;
using System.IO;
using System.Text;

namespace PottyLogger.Shared.Models.Logging
{
    public abstract class Log
    {
        public const string FILE_EXTENSION = ".txt";
        public const string DATE_FORMAT = "yyyyMMdd-HHmmss";

        private static readonly object _lock = new object();

        public Log(string fileName)
        {
            FileName = fileName;

            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }

            if (File.Exists(FilePath))
            {
                var fileInfo = new FileInfo(FilePath);

                FileSize = fileInfo.Length;
                CreationDate = fileInfo.CreationTime;
                ModifiedDate = fileInfo.LastWriteTime;
            }
        }

        public string FileName { get; set; }
        public DateTime CreationDate { get; private set; }
        public DateTime ModifiedDate { get; private set; }
        public long FileSize { get; private set; }

        public abstract string LogDirectory { get; }

        public string FilePath => Path.Combine(LogDirectory, FileName + FILE_EXTENSION);

        public void Append(string line)
        {
            lock (_lock)
            {
                File.AppendAllLines(FilePath, (DateTime.Now + " - " + line).Yield());
            }
        }

        public void Append(Exception ex, string additionalMessage = "")
        {
            lock (_lock)
            {
                var builder = new StringBuilder(DateTime.Now + " - ");

                if (!string.IsNullOrEmpty(additionalMessage))
                {
                    builder.Append(additionalMessage + " - ");
                }

                builder.Append(ex);

                File.AppendAllLines(FilePath, builder.ToString().Yield());
            }
        }

        public void Delete() => File.Delete(FilePath);
    }
}
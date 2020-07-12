using System;
using System.Collections.Generic;
using System.IO;

namespace PottyLogger.Shared.Models
{
    public class PottyLog
    {
        public List<PottyLogEntry> Entries { get; }

        /*public static PottyLog Create(string filePath)
        {
            File.Create(filePath);
        }

        public static PottyLog ParseFromFile(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                var line = reader.ReadLine();
                var headers = line.Split(",");

                while (!string.IsNullOrEmpty(line))
                {
                    var entry = PottyLogEntry.ParseCSVRow(line);
                    Entries.Add(entry);
                }
            }
        }*/
    }
}
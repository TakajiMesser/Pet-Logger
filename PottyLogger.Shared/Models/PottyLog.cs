using System;
using System.Collections.Generic;

namespace PottyLogger.Shared.Models
{
    public class PottyLog
    {
        public const string FILE_NAME = "PottyLog.csv";

        public List<PottyLogEntry> Entries { get; }

        
    }
}
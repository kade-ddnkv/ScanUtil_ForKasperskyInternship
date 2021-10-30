using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtil_ForKasperskyInternship
{
    /// <summary>
    /// Результат сканирования директории. Парсится из json, переданного из scan_service.
    /// </summary>
    public class DirScanStatus
    {
        public string Directory { get; set; }
        public long ProcessedFilesCount
        {
            get
            {
                return StatusesCount.Values.Sum();
            }
        }
        public Dictionary<FileScanStatus, int> StatusesCount { get; set; }
        public TimeSpan ExecutionTime { get; set; }
    }
}

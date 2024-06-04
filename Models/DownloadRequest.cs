using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PzenaAssessment.Models
{
    public class DownloadRequest
    {
        public string Url { get; set; }
        public string ZipPath { get; set; }
        public string CsvFilePath { get; set; }
    }
}

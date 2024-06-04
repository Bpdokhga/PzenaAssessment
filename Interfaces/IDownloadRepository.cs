using PzenaAssessment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PzenaAssessment.Interfaces
{
    public interface IDownloadRepository
    {
        Task DownloadFileAsync(DownloadRequest request, string tableName, CancellationToken cancelToken);
    }
}

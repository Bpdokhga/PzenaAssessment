using PzenaAssessment.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PzenaAssessment.Models
{
    public class Price : IPrice
    {
        public string Ticker { get; set; } 
        public DateTime Date { get; set; }
        public decimal? OpenPrice { get; set; } 
        public decimal? HighPrice { get; set; } 
        public decimal? LowPrice { get; set; } 
        public decimal? ClosePrice { get; set; }  
        public long Volume { get; set; }
        public decimal? CloseAdj { get; set; } 
        public decimal? CloseUnadj { get; set; } 
        public DateTime? LastUpdated { get; set; }
    }
}

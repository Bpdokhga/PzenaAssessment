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
        public DateTime? Date { get; set; }
        public double? OpenPrice { get; set; } 
        public double? HighPrice { get; set; } 
        public double? LowPrice { get; set; } 
        public double? ClosePrice { get; set; }  
        public double? Volume { get; set; }
        public double? CloseAdj { get; set; } 
        public double? CloseUnadj { get; set; } 
        public DateTime? LastUpdated { get; set; }
    }
}

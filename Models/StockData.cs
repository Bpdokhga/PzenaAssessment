using PzenaAssessment.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PzenaAssessment.Models
{
    public class StockData : IStockData
    {
        public string TableName { get; set; }
        public string Permaticker { get; set; }
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string? Exchange { get; set; }
        public char IsDelisted { get; set; } 
        public string? Category { get; set; }
        public string? Cusips { get; set; }
        public string? SicCode { get; set; }
        public string? SicSector { get; set; }
        public string? SicIndustry { get; set; }
        public string? FamaSector { get; set; }
        public string? FamaIndustry { get; set; }
        public string? Sector { get; set; }
        public string? Industry { get; set; }
        public string? ScaleMarketCap { get; set; }
        public string? ScaleRevenue { get; set; }
        public string? RelatedTickers { get; set; }
        public string? Currency { get; set; }
        public string? Location { get; set; }
        public DateTime? LastUpdated { get; set; } 
        public DateTime? FirstAdded { get; set; } 
        public DateTime? FirstPriceDate { get; set; }
        public DateTime? LastPriceDate { get; set; } 
        public DateTime? FirstQuarter { get; set; }
        public DateTime? LastQuarter { get; set; }
        public string? SecFilings { get; set; }
        public string? CompanySite { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace MadWin.Core.DTOs.Factors
{
    public class FactorFilterParameter
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public int? FactorId { get; set; }
        public string FullName { get; set; }
        public decimal? FromPrice { get; set; }
        public decimal? ToPrice { get; set; }
    }
}

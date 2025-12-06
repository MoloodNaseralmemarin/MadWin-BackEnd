namespace MadWin.Core.DTOs.Factors
{
    public class OrderFilterParameter
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public int? OrderId { get; set; }
        public string FullName { get; set; }
        public decimal? FromPrice { get; set; }
        public decimal? ToPrice { get; set; }
    }
}

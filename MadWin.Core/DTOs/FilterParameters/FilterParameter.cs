using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadWin.Core.DTOs.FilterParameters
{
    public class FilterParameter
    {
        public string FullName { get; set; }
        public int? OrderId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? FromPrice { get; set; }
        public int? ToPrice { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MadWin.Application.DTOs.Reports
{
    public class UserOrderPrintDto
    {
        public string OrderName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set;  }
        public string Address { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedAt {  get; set; }
    }

    public class UserFactorPrintDto
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Address { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

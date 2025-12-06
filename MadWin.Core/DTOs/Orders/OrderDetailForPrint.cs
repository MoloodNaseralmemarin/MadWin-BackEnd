using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadWin.Core.DTOs.Orders
{
    public class OrderDetailForPrint
    {
        public int OrderId { get;set; }

        public string FullName { get; set; }

        public string OrderName { get; set;}
        public string Address { get; set;}
        public string Description { get;set;}
    }
}

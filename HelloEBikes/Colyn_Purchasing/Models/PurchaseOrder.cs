using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colyn_Purchasing.Models
{
    public class PurchaseOrder
    {
        public int? PurchaseOrderId { get; set; }
        public int PurchaseOrderNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public List<Part> PurchaseOrderDetails { get; set; }
        public List<Part> PartsInventory { get; set; }
        public decimal GST { get; set; }
    }
}

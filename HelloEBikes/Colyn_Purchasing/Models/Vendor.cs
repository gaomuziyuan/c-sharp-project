using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colyn_Purchasing.Models
{
    //public record VendorInfo(int VendorId, string VendorName, string Phone, string Address, string City);
    public class Vendor
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public List<Part> Parts { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
    }
}

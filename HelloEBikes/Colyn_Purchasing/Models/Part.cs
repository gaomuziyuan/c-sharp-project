﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colyn_Purchasing.Models
{
    public class Part
    {
        public int PartId { get; set; }
        public string Description { get; set; }
        public int QuantityOnHand { get; set; }
        public int ReorderLevel { get; set; }
        public int QuantityOnOrder { get; set; }
        public int Buffer { get; set; }
        public decimal Price { get; set; }
        public decimal SellingPrice { get; set; }
        public int QuantityToOrder { get; set; }
    }
}

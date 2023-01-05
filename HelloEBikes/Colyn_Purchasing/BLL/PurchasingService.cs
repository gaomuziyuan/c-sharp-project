using Colyn_Purchasing.DAL;
using Colyn_Purchasing.Entities;
using Colyn_Purchasing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colyn_Purchasing.BLL
{
    public class PurchasingService
    {
        #region Constructor and DI fields
        private readonly EbikePurchasingContext _context;
        internal PurchasingService(EbikePurchasingContext context)
        {
            _context = context;
        }
        #endregion

        #region Query
        public List<VendorInfo> ListAllVendors()
        {
            var result = _context.Vendors.Select(x => new VendorInfo(x.VendorId, x.VendorName));
            return result.ToList();
        }

        public Models.Vendor LookupVendor(int currentVendorId)
        {
            Models.Vendor currentVendor = _context.Vendors
                                                    .Where(x => x.VendorId == currentVendorId)
                                                    .Select(x => new Models.Vendor
                                                    {
                                                        VendorId = x.VendorId,
                                                        VendorName = x.VendorName,
                                                        Phone = x.Phone,
                                                        City = x.City,
                                                        Address = x.Address,
                                                        Parts = x.Parts
                                                                    .Select(x => new Models.Part
                                                                    {
                                                                        PartId = x.PartId,
                                                                        Description = x.Description,
                                                                        QuantityOnHand = x.QuantityOnHand,
                                                                        ReorderLevel = x.ReorderLevel,
                                                                        QuantityOnOrder = x.QuantityOnOrder,
                                                                        Buffer = x.ReorderLevel - (x.QuantityOnHand + x.QuantityOnOrder),
                                                                        Price = x.PurchasePrice,
                                                                        SellingPrice = x.SellingPrice
                                                                    }).ToList()
                                                    }
                                                    ).FirstOrDefault();

            Models.PurchaseOrder currentOrder = _context.PurchaseOrders
                                                            .Where(x => x.OrderDate == null && x.VendorId == currentVendorId)
                                                            .Select(x => new Models.PurchaseOrder
                                                            {
                                                                PurchaseOrderId = x.PurchaseOrderId,
                                                                PurchaseOrderNumber = x.PurchaseOrderNumber,
                                                                PurchaseOrderDetails = x.PurchaseOrderDetails
                                                                                        .Select(x => new Models.Part
                                                                                        {
                                                                                            PartId = x.PartId,
                                                                                            Description = x.Part.Description,
                                                                                            QuantityOnHand = x.Part.QuantityOnHand,
                                                                                            ReorderLevel = x.Part.ReorderLevel,
                                                                                            QuantityOnOrder = x.Part.QuantityOnOrder,
                                                                                            QuantityToOrder = x.Quantity,
                                                                                            Price = x.PurchasePrice,
                                                                                            SellingPrice = x.Part.SellingPrice
                                                                                        }).ToList(),
                                                                GST = x.TaxAmount
                                                            }).FirstOrDefault();

            if (currentOrder == null)
            {
                currentOrder = new Models.PurchaseOrder
                {
                    PurchaseOrderDetails = currentVendor.Parts.Where(x => x.Buffer > 0).ToList(),
                    PartsInventory = currentVendor.Parts.Where(x => x.Buffer < 0).ToList()
                };
            }
            else
            {
                currentOrder.PartsInventory = currentVendor.Parts.ToList();
                foreach (var item in currentOrder.PurchaseOrderDetails)
                {
                    var match = currentOrder.PartsInventory.SingleOrDefault(x => x.PartId == item.PartId);
                    if (match != null)
                        currentOrder.PartsInventory.Remove(match);
                }
            }

            currentVendor.PurchaseOrder = currentOrder;

            return currentVendor;
        }
        #endregion

        #region CRUD
        public int InsertPurchaseOrder(Models.Vendor currentVendor, decimal currentSubtotal)
        {
            Entities.PurchaseOrder newOrder = new()
            {
                EmployeeId = 1,
                PurchaseOrderNumber = _context.PurchaseOrders.Select(x=>x.PurchaseOrderNumber).Max() + 1,
                VendorId = currentVendor.VendorId,
                OrderDate = null,
                SubTotal = currentSubtotal
            };
            _context.PurchaseOrders.Add(newOrder);
            _context.SaveChanges();
            return newOrder.PurchaseOrderId;
        }

        public void PlacePurchaseOrder(Models.PurchaseOrder currentOrder, List<Models.Part> currentPurchaseOrderDetails)
        {
            var existingOrder = _context.PurchaseOrders.Find(currentOrder.PurchaseOrderId);
            if (existingOrder is null)
                throw new ArgumentException("Could not find the specified purchase order");
            existingOrder.OrderDate = DateTime.Today;

            foreach(var item in currentPurchaseOrderDetails)
            {
                var existingOrderDetails = _context.PurchaseOrderDetails.Find(item.PartId);
                existingOrderDetails.Quantity = item.QuantityToOrder + item.QuantityOnOrder;
            }
            
            _context.SaveChanges();
        }

        public void DeletePurchaseOrder(Models.PurchaseOrder currentOrder)
        {
            var existingOrder = _context.PurchaseOrders.Find(currentOrder.PurchaseOrderId);
            if (existingOrder is null)
                throw new ArgumentException("Could not find the specified purchase order");
            _context.PurchaseOrders.Remove(existingOrder);
            _context.SaveChanges();
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colyn_Purchasing.BLL;
using Colyn_Purchasing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Admin
{
    public class PurchasingModel : PageModel
    {
        #region Constructor and Dependencies
        private readonly PurchasingService _service;
        public PurchasingModel(PurchasingService service)
        {
            _service = service;
        }
        #endregion

        [TempData]
        public string FeedBackMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }
        //a get property that returns the result of the lamda action
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
        public bool HasFeedBack => !string.IsNullOrWhiteSpace(FeedBackMessage);

        public List<VendorInfo> AllVendors { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedVendor { get; set; }

        [BindProperty]
        public Vendor CurrentVendor { get; set; }
        [BindProperty]
        public List<Part> PurchaseOrderDetails { get; set; }
        [BindProperty]
        public List<Part> PartsInventory { get; set; }

        [BindProperty]
        public decimal CurrentSubtotal { get; set; }
        [BindProperty]
        public decimal CurrentGST { get; set; }
        [BindProperty]
        public decimal CurrentTotal { get; set; }

        [BindProperty]
        public int PartToAdd { get; set; }
        [BindProperty]
        public int PartToRemove { get; set; }
        [BindProperty]
        public int PartToUpdate { get; set; }


        public void OnGet()
        {
            AllVendors = _service.ListAllVendors();

            if (SelectedVendor.HasValue)
            {
                CurrentVendor = _service.LookupVendor(SelectedVendor.Value);
                PurchaseOrderDetails = CurrentVendor.PurchaseOrder.PurchaseOrderDetails;
                PartsInventory = CurrentVendor.PurchaseOrder.PartsInventory;
            }
        }

        public IActionResult OnPostFindOrder()
        {
            return RedirectToPage(new { SelectedVendor = SelectedVendor });
        }

        public void OnPostAddPart()
        {
            var partItem = PartsInventory.FirstOrDefault(x => x.PartId == PartToAdd);
            PurchaseOrderDetails.Add(partItem);
            PartsInventory.Remove(partItem);
            PopulateDropDownData();
        }
        public void OnPostRemovePart()
        {
            var partItem = PurchaseOrderDetails.FirstOrDefault(x => x.PartId == PartToRemove);
            PurchaseOrderDetails.Remove(partItem);
            PartsInventory.Add(partItem);
            PopulateDropDownData();
        }
        public void OnPostRefresh()
        {
            PopulateDropDownData();
            foreach (var item in PurchaseOrderDetails)
            {
                CurrentSubtotal += item.Price * item.QuantityToOrder;
            }
            CurrentGST = CurrentVendor.PurchaseOrder.GST;
            CurrentTotal = CurrentSubtotal + CurrentGST;
        }

        
        public IActionResult OnPostInsert()
        {
            try
            {
                PopulateDropDownData();
                int newPurchaseOrderId = _service.InsertPurchaseOrder(CurrentVendor, CurrentSubtotal);
                FeedBackMessage = $"Successfully added {newPurchaseOrderId} to the list of Purchase Orders.";
                return RedirectToPage(new { SelectedVendor = SelectedVendor });
            }
            catch (Exception ex)
            {
                // The response to the browser is the result of this POST processing
                Exception innermost = ex;
                while (innermost.InnerException != null)
                    innermost = innermost.InnerException;
                ErrorMessage = innermost.Message;
                return Page();
            }
        }

        public void OnPostUpdate()
        {
            try
            {
                PopulateDropDownData();
            }
            catch (Exception ex)
            {
                // The response to the browser is the result of this POST processing
                Exception innermost = ex;
                while (innermost.InnerException != null)
                    innermost = innermost.InnerException;
                ErrorMessage = innermost.Message;
            }
        }

        public IActionResult OnPostPlace()
        {
            try
            {
                PopulateDropDownData();
                foreach (var item in PurchaseOrderDetails)
                {
                    CurrentSubtotal += item.Price * item.QuantityToOrder;
                }
                CurrentGST = CurrentVendor.PurchaseOrder.GST;
                CurrentTotal = CurrentSubtotal + CurrentGST;

                _service.PlacePurchaseOrder(CurrentVendor.PurchaseOrder, PurchaseOrderDetails);
                FeedBackMessage = $"The order will no longer be able to be altered";
                return RedirectToPage(new { SelectedVendor = SelectedVendor });
            }
            catch (Exception ex)
            {
                // The response to the browser is the result of this POST processing
                Exception innermost = ex;
                while (innermost.InnerException != null)
                    innermost = innermost.InnerException;
                ErrorMessage = innermost.Message;
                return Page();
            }
            
        }

        public IActionResult OnPostDelete()
        {
            try
            {
                PopulateDropDownData();
                _service.DeletePurchaseOrder(CurrentVendor.PurchaseOrder);
                return RedirectToPage(new { SelectedVendor = SelectedVendor });
            }
            catch (Exception ex)
            {
                // The response to the browser is the result of this POST processing
                Exception innermost = ex;
                while (innermost.InnerException != null)
                    innermost = innermost.InnerException;
                ErrorMessage = innermost.Message;
                return Page();
            }
        }

        public IActionResult OnPostClear()
        {
            return RedirectToPage(new { SelectedVendor = (int?)null });
        }

        private void PopulateDropDownData()
        {
            AllVendors = _service.ListAllVendors();

            if (SelectedVendor.HasValue)
            {
                CurrentVendor = _service.LookupVendor(SelectedVendor.Value);
            }
        }

        private Exception GetInnerException(Exception ex)
        {
            while (ex.InnerException != null)
                ex = ex.InnerException;
            return ex;
        }
    }
}

﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Colyn_Purchasing.Entities
{
    internal partial class Employee
    {
        public Employee()
        {
            PurchaseOrders = new HashSet<PurchaseOrder>();
        }

        [Key]
        [Column("EmployeeID")]
        public int EmployeeId { get; set; }
        [Required]
        [StringLength(9)]
        public string SocialInsuranceNumber { get; set; }
        [Required]
        [StringLength(30)]
        public string LastName { get; set; }
        [Required]
        [StringLength(30)]
        public string FirstName { get; set; }
        [StringLength(40)]
        public string Address { get; set; }
        [StringLength(20)]
        public string City { get; set; }
        [StringLength(2)]
        public string Province { get; set; }
        [StringLength(6)]
        public string PostalCode { get; set; }
        [Required]
        [StringLength(12)]
        public string ContactPhone { get; set; }
        public bool Textable { get; set; }
        [Required]
        [StringLength(30)]
        public string EmailAddress { get; set; }
        [Column("PositionID")]
        public int PositionId { get; set; }

        [InverseProperty(nameof(PurchaseOrder.Employee))]
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    }
}
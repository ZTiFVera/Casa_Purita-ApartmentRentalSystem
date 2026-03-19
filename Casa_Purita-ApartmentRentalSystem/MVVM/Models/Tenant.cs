using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casa_Purita_ApartmentRentalSystem.MVVM.Model
{
    public class Tenant
    {
        public int TenantId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string UnitNumber { get; set; } = string.Empty;
        public DateTime MoveInDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public bool IsDeleted { get; set; } = false; // Soft delete flag
 
        public string FullName => $"{FirstName} {LastName}";
    }
}

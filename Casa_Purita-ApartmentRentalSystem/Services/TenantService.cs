using Casa_Purita_ApartmentRentalSystem.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Casa_Purita_ApartmentRentalSystem.Services
{
    public class TenantService
    {
        private readonly HttpClient _httpClient;

        // FIX: Use the full MockAPI URL directly — no BaseAddress to avoid conflicts
        private const string BaseUrl = "https://TenantId.mockapi.io/api/tenants";

        public TenantService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // FIX: Removed BaseAddress — using full URL in each method instead
        }

        public async Task<List<Tenant>> GetAllTenantsAsync()
        {
            var tenants = await _httpClient.GetFromJsonAsync<List<Tenant>>(BaseUrl);
            return tenants?.Where(t => !t.IsDeleted).ToList() ?? new List<Tenant>();
        }

        public async Task<Tenant?> GetTenantByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Tenant>($"{BaseUrl}/{id}");
        }

        // FIX: Accept object so we can send an anonymous type without TenantId
        public async Task<bool> CreateTenantAsync(object tenant)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, tenant);

            // Debug: log the error if it fails
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[CreateTenant] Failed: {response.StatusCode} - {error}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateTenantAsync(int id, Tenant tenant)
        {
            // FIX: Send formatted date string to avoid DateTime serialization issues
            var payload = new
            {
                firstName = tenant.FirstName,
                lastName = tenant.LastName,
                email = tenant.Email,
                phoneNumber = tenant.PhoneNumber,
                unitNumber = tenant.UnitNumber,
                moveInDate = tenant.MoveInDate.ToString("yyyy-MM-dd"),
                monthlyRent = tenant.MonthlyRent,
                isDeleted = tenant.IsDeleted
            };

            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SoftDeleteTenantAsync(int id)
        {
            var tenant = await GetTenantByIdAsync(id);
            if (tenant == null) return false;

            tenant.IsDeleted = true;
            return await UpdateTenantAsync(id, tenant);
        }

        public async Task<bool> HardDeleteTenantAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
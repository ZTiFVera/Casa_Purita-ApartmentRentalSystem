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

        private const string BaseUrl = "https://localhost:7000/api/tenants";

        public TenantService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7000/");
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

        public async Task<bool> CreateTenantAsync(Tenant tenant)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, tenant);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateTenantAsync(int id, Tenant tenant)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", tenant);
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

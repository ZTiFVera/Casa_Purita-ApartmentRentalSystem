using Casa_Purita_ApartmentRentalSystem.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Casa_Purita_ApartmentRentalSystem.Services
{

    public class TenantService
    {
        private readonly HttpClient _httpClient;

        private const string BaseUrl = "https://69c48b2e8a5b6e2dec2ac3be.mockapi.io/api/tenants";

        public TenantService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Tenant>> GetAllTenantsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(BaseUrl);
                var raw = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[GetAll] Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[GetAll] Body: {raw}");

                if (!response.IsSuccessStatusCode)
                    return new List<Tenant>();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var tenants = JsonSerializer.Deserialize<List<Tenant>>(raw, options);

                // Log each tenant's Id to verify it's being read correctly
                if (tenants != null)
                    foreach (var t in tenants)
                        System.Diagnostics.Debug.WriteLine($"[GetAll] Tenant: {t.FullName} | Id='{t.Id}'");

                return tenants?.Where(t => !t.IsDeleted).ToList() ?? new List<Tenant>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetAll] Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<Tenant?> GetTenantByIdAsync(string id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[GetById] Fetching id='{id}'");
                var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
                var raw = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[GetById] Status: {response.StatusCode} | Body: {raw}");

                if (!response.IsSuccessStatusCode) return null;

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<Tenant>(raw, options);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetById] Exception: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateTenantAsync(Tenant tenant)
        {
            try
            {
                var payload = new
                {
                    firstName = tenant.FirstName,
                    lastName = tenant.LastName,
                    email = tenant.Email,
                    phoneNumber = tenant.PhoneNumber,
                    unitNumber = tenant.UnitNumber,
                    moveInDate = tenant.MoveInDate.ToString("yyyy-MM-dd"),
                    monthlyRent = tenant.MonthlyRent,
                    isDeleted = false
                };

                System.Diagnostics.Debug.WriteLine($"[Create] Posting: {JsonSerializer.Serialize(payload)}");
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, payload);
                var raw = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[Create] Status: {response.StatusCode} | Body: {raw}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Create] Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateTenantAsync(string id, Tenant tenant)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    System.Diagnostics.Debug.WriteLine("[Update] ERROR: id is null or empty!");
                    return false;
                }

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

                var url = $"{BaseUrl}/{id}";
                System.Diagnostics.Debug.WriteLine($"[Update] PUT {url}");
                System.Diagnostics.Debug.WriteLine($"[Update] Payload: {JsonSerializer.Serialize(payload)}");

                var response = await _httpClient.PutAsJsonAsync(url, payload);
                var raw = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[Update] Status: {response.StatusCode} | Body: {raw}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Update] Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> SoftDeleteTenantAsync(string id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[SoftDelete] id='{id}'");

                if (string.IsNullOrEmpty(id))
                {
                    System.Diagnostics.Debug.WriteLine("[SoftDelete] ERROR: id is null or empty!");
                    return false;
                }

                var tenant = await GetTenantByIdAsync(id);
                if (tenant == null)
                {
                    System.Diagnostics.Debug.WriteLine($"[SoftDelete] Tenant not found for id='{id}'");
                    return false;
                }

                tenant.IsDeleted = true;
                return await UpdateTenantAsync(id, tenant);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SoftDelete] Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> HardDeleteTenantAsync(string id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[HardDelete] id='{id}'");

                if (string.IsNullOrEmpty(id))
                {
                    System.Diagnostics.Debug.WriteLine("[HardDelete] ERROR: id is null or empty!");
                    return false;
                }

                var url = $"{BaseUrl}/{id}";
                System.Diagnostics.Debug.WriteLine($"[HardDelete] DELETE {url}");

                var response = await _httpClient.DeleteAsync(url);
                var raw = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[HardDelete] Status: {response.StatusCode} | Body: {raw}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[HardDelete] Exception: {ex.Message}");
                throw;
            }
        }
        public async Task<List<Tenant>> GetDeletedTenantsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(BaseUrl);
                var raw = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return new List<Tenant>();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var tenants = JsonSerializer.Deserialize<List<Tenant>>(raw, options);

                return tenants?.Where(t => t.IsDeleted).ToList() ?? new List<Tenant>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetDeleted] Exception: {ex.Message}");
                throw;
            }
        }
    }
}
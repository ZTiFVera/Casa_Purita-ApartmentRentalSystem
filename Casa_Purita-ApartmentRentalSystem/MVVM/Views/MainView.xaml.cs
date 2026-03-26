using Casa_Purita_ApartmentRentalSystem.MVVM.Model;
using Casa_Purita_ApartmentRentalSystem.MVVM.Views;
using Casa_Purita_ApartmentRentalSystem.Services;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;

namespace Casa_Purita_ApartmentRentalSystem.MVVM.Views
{

    public partial class MainView : ContentPage
    {
        private readonly TenantService _tenantService;

        public MainView(TenantService tenantService)
        {
            InitializeComponent();
            _tenantService = tenantService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTenants();
        }

        private async Task LoadTenants()
        {
            try
            {
                var tenants = await _tenantService.GetAllTenantsAsync();
                TenantsCollectionView.ItemsSource = tenants;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load tenants: {ex.Message}", "OK");
            }
            finally
            {
                refreshView.IsRefreshing = false;
            }
        }

        private async void OnRefreshing(object sender, EventArgs e)
        {
            await LoadTenants();
        }

        private async void OnAddTenantClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateView(_tenantService));
        }

        private async void OnEditClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Tenant tenant)
            {
                // Guard: make sure Id loaded correctly before navigating
                if (string.IsNullOrEmpty(tenant.Id))
                {
                    await DisplayAlert("Error", "Tenant ID is missing. Please refresh and try again.", "OK");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[Edit] Opening tenant Id='{tenant.Id}'");
                await Navigation.PushAsync(new UpdateView(_tenantService, tenant));
            }
        }

        private async void OnSoftDeleteClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Tenant tenant)
            {
                if (string.IsNullOrEmpty(tenant.Id))
                {
                    await DisplayAlert("Error", "Tenant ID is missing. Please refresh and try again.", "OK");
                    return;
                }

                bool confirm = await DisplayAlert(
                    "Soft Delete",
                    $"Hide {tenant.FullName} from the list? They will NOT be permanently removed.",
                    "Yes, Hide", "Cancel");

                if (confirm)
                {
                    System.Diagnostics.Debug.WriteLine($"[SoftDelete] Tenant Id='{tenant.Id}'");
                    bool success = await _tenantService.SoftDeleteTenantAsync(tenant.Id);
                    if (success)
                    {
                        await DisplayAlert("Success", $"{tenant.FullName} has been hidden.", "OK");
                        await LoadTenants();
                    }
                    else
                        await DisplayAlert("Error", "Soft delete failed. Please try again.", "OK");
                }
            }
        }

        private async void OnHardDeleteClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Tenant tenant)
            {
                if (string.IsNullOrEmpty(tenant.Id))
                {
                    await DisplayAlert("Error", "Tenant ID is missing. Please refresh and try again.", "OK");
                    return;
                }

                bool confirm = await DisplayAlert(
                    "⚠️ Hard Delete",
                    $"Permanently delete {tenant.FullName}? This CANNOT be undone!",
                    "Yes, Delete Forever", "Cancel");

                if (confirm)
                {
                    System.Diagnostics.Debug.WriteLine($"[HardDelete] Tenant Id='{tenant.Id}'");
                    bool success = await _tenantService.HardDeleteTenantAsync(tenant.Id);
                    if (success)
                    {
                        await DisplayAlert("Deleted", $"{tenant.FullName} has been permanently removed.", "OK");
                        await LoadTenants();
                    }
                    else
                        await DisplayAlert("Error", "Hard delete failed. Please try again.", "OK");
                }
            }
        }
    }
}
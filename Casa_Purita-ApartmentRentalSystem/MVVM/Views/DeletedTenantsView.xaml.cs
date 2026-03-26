using Casa_Purita_ApartmentRentalSystem.MVVM.Model;
using Casa_Purita_ApartmentRentalSystem.Services;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;

namespace Casa_Purita_ApartmentRentalSystem.MVVM.Views
{
    public partial class DeletedTenantsView : ContentPage
    {
        private readonly TenantService _tenantService;

        public DeletedTenantsView(TenantService tenantService)
        {
            InitializeComponent();
            _tenantService = tenantService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadDeletedTenants();
        }

        private async Task LoadDeletedTenants()
        {
            try
            {
                var deleted = await _tenantService.GetDeletedTenantsAsync();
                DeletedTenantsCollectionView.ItemsSource = deleted;

                int count = deleted?.Count ?? 0;
                CountLabel.Text = count == 0
                    ? "No hidden residents"
                    : $"{count} hidden resident{(count == 1 ? "" : "s")}";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load: {ex.Message}", "OK");
            }
            finally
            {
                refreshView.IsRefreshing = false;
            }
        }

        private async void OnRefreshing(object sender, EventArgs e)
        {
            await LoadDeletedTenants();
        }

        private async void OnRestoreClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Tenant tenant)
            {
                bool confirm = await DisplayAlert(
                    "Restore Resident",
                    $"Restore {tenant.FullName} back to the active list?",
                    "Yes, Restore", "Cancel");

                if (confirm)
                {
                    tenant.IsDeleted = false;
                    bool success = await _tenantService.UpdateTenantAsync(tenant.Id, tenant);

                    if (success)
                    {
                        await DisplayAlert("Restored", $"{tenant.FullName} is now visible again.", "OK");
                        await LoadDeletedTenants();
                    }
                    else
                    {
                        await DisplayAlert("Error", "Restore failed. Please try again.", "OK");
                    }
                }
            }
        }

        private async void OnHardDeleteClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Tenant tenant)
            {
                bool confirm = await DisplayAlert(
                    "⚠️ Delete Forever",
                    $"Permanently delete {tenant.FullName}? This CANNOT be undone!",
                    "Yes, Delete Forever", "Cancel");

                if (confirm)
                {
                    bool success = await _tenantService.HardDeleteTenantAsync(tenant.Id);

                    if (success)
                    {
                        await DisplayAlert("Deleted", $"{tenant.FullName} has been permanently removed.", "OK");
                        await LoadDeletedTenants();
                    }
                    else
                    {
                        await DisplayAlert("Error", "Delete failed. Please try again.", "OK");
                    }
                }
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
using Casa_Purita_ApartmentRentalSystem.MVVM.Model;
using Casa_Purita_ApartmentRentalSystem.Services;

namespace Casa_Purita_ApartmentRentalSystem.MVVM.Views
{
    public partial class CreateView : ContentPage
    {
        private readonly TenantService _tenantService;

        public CreateView(TenantService tenantService)
        {
            InitializeComponent();
            _tenantService = tenantService;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(FirstNameEntry.Text) ||
                string.IsNullOrWhiteSpace(LastNameEntry.Text) ||
                string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                string.IsNullOrWhiteSpace(PhoneEntry.Text) ||
                string.IsNullOrWhiteSpace(UnitNumberEntry.Text) ||
                string.IsNullOrWhiteSpace(MonthlyRentEntry.Text))
            {
                await DisplayAlert("Validation Error", "Please fill in all fields.", "OK");
                return;
            }

            if (!decimal.TryParse(MonthlyRentEntry.Text, out decimal rent))
            {
                await DisplayAlert("Validation Error", "Please enter a valid rent amount.", "OK");
                return;
            }

            // Build tenant object
            var newTenant = new Tenant
            {
                FirstName = FirstNameEntry.Text.Trim(),
                LastName = LastNameEntry.Text.Trim(),
                Email = EmailEntry.Text.Trim(),
                PhoneNumber = PhoneEntry.Text.Trim(),
                UnitNumber = UnitNumberEntry.Text.Trim(),
                MoveInDate = MoveInDatePicker.Date,
                MonthlyRent = rent,
                IsDeleted = false
            };

            try
            {
                bool success = await _tenantService.CreateTenantAsync(newTenant);

                if (success)
                {
                    await DisplayAlert("Success", "Tenant added successfully!", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "Failed to save tenant. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
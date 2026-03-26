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

            // FIX 1: Do NOT set TenantId — MockAPI auto-generates it.
            // FIX 2: Convert MoveInDate to string to avoid serialization issues.
            var newTenant = new
            {
                firstName = FirstNameEntry.Text.Trim(),
                lastName = LastNameEntry.Text.Trim(),
                email = EmailEntry.Text.Trim(),
                phoneNumber = PhoneEntry.Text.Trim(),
                unitNumber = UnitNumberEntry.Text.Trim(),
                moveInDate = MoveInDatePicker.Date.ToString("yyyy-MM-dd"),
                monthlyRent = rent,
                isDeleted = false
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
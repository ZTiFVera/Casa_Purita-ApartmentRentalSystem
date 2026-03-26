using Casa_Purita_ApartmentRentalSystem.MVVM.Model;
using Casa_Purita_ApartmentRentalSystem.Services;

namespace Casa_Purita_ApartmentRentalSystem.MVVM.Views;

public partial class UpdateView : ContentPage
{
    private readonly TenantService _tenantService;
    private readonly Tenant _tenant;
 
    public UpdateView(TenantService tenantService, Tenant tenant)
    {
        InitializeComponent();
        _tenantService = tenantService;
        _tenant = tenant;
 
        System.Diagnostics.Debug.WriteLine($"[UpdateView] Loaded tenant Id='{tenant.Id}'");
 
        FirstNameEntry.Text = tenant.FirstName;
        LastNameEntry.Text = tenant.LastName;
        EmailEntry.Text = tenant.Email;
        PhoneEntry.Text = tenant.PhoneNumber;
        UnitNumberEntry.Text = tenant.UnitNumber;
        MoveInDatePicker.Date = tenant.MoveInDate;
        MonthlyRentEntry.Text = tenant.MonthlyRent.ToString();
    }
 
    private async void OnUpdateClicked(object sender, EventArgs e)
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
 
        _tenant.FirstName = FirstNameEntry.Text.Trim();
        _tenant.LastName = LastNameEntry.Text.Trim();
        _tenant.Email = EmailEntry.Text.Trim();
        _tenant.PhoneNumber = PhoneEntry.Text.Trim();
        _tenant.UnitNumber = UnitNumberEntry.Text.Trim();
        _tenant.MoveInDate = MoveInDatePicker.Date;
        _tenant.MonthlyRent = rent;
 
        System.Diagnostics.Debug.WriteLine($"[UpdateView] Updating Id='{_tenant.Id}'");
 
        try
        {
            bool success = await _tenantService.UpdateTenantAsync(_tenant.Id, _tenant);
 
            if (success)
            {
                await DisplayAlert("Success", "Tenant updated successfully!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Failed to update tenant. Please try again.", "OK");
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
using Casa_Purita_ApartmentRentalSystem.Services;

namespace Casa_Purita_ApartmentRentalSystem.MVVM.Views
{
    public partial class HomeView : ContentPage
    {
        private readonly TenantService _tenantService;

        public HomeView(TenantService tenantService)
        {
            InitializeComponent();
            _tenantService = tenantService;
        }

        private async void OnGetStartedClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainView(_tenantService));
        }
    }
}
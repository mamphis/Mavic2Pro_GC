using DJI.WindowsSDK;
using DJI.WindowsSDK.Components;

namespace Mavic2Pro_GC.ViewModel
{
    internal class CurrentConnectionStateViewModel : ViewModelBase
    {

        private string _currentConnectedAircraft;
        private string _aircraftName;


        private ProductHandler productHandler;
        private FlightControllerHandler flightControllerHandler;
        public CurrentConnectionStateViewModel()
        {
            this.productHandler = DJISDKManager.Instance.ComponentManager.GetProductHandler(0);
            this.flightControllerHandler = DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0);

            this.productHandler.ProductTypeChanged += this.ProductHandler_ProductTypeChanged;
            this.flightControllerHandler.AircraftNameChanged += this.FlightControllerHandler_AircraftNameChanged;

            this.InitializeDefaultValues();
        }


        public string CurrentConnectedAircraft
        {
            get => _currentConnectedAircraft;
            set
            {
                _currentConnectedAircraft = value;
                OnPropertyChanged();
            }
        }

        public string AircraftName
        {
            get => _aircraftName;
            set
            {
                _aircraftName = value;
                OnPropertyChanged();
            }
        }

        private async void InitializeDefaultValues()
        {
            this.CurrentConnectedAircraft = "- none -";
            var productType = (await DJISDKManager.Instance.ComponentManager.GetProductHandler(0).GetProductTypeAsync()).value;
            if (productType != null && productType?.value != ProductType.UNRECOGNIZED)
            {
                this.CurrentConnectedAircraft = productType.ToString();

                this.AircraftName = (await this.flightControllerHandler.GetAircraftNameAsync()).value?.value ?? "";
            }
        }

        private void FlightControllerHandler_AircraftNameChanged(object sender, StringMsg? value)
        {
            this.AircraftName = value?.value ?? "";
        }

        private void ProductHandler_ProductTypeChanged(object sender, ProductTypeMsg? value)
        {
            var productType = value?.value;

            if (productType == null)
            {
                return;
            }

            if (productType.Value != ProductType.UNRECOGNIZED)
            {
                this.CurrentConnectedAircraft = productType.Value.ToString();

            }
        }

        ~CurrentConnectionStateViewModel()
        {
            this.productHandler.ProductTypeChanged -= this.ProductHandler_ProductTypeChanged;
        }
    }
}

using DJI.WindowsSDK;
using DJI.WindowsSDK.Components;

namespace Mavic2Pro_GC.ViewModel
{
    internal class CurrentConnectionStateViewModel : ViewModelBase
    {
        private string _currentConnectedAircraft;
        private string _aircraftName;
        private bool _isConnected;
        private int _satelliteCount;
        private int _signalStrength;
        private float _powerConsumption;
        private float _batteryVoltage;
        private int _batteryChargeRemaining;
        private float _batteryTemperature;

        private readonly ProductHandler productHandler;
        private readonly FlightControllerHandler flightControllerHandler;
        private readonly BatteryHandler batteryHandler;
        public CurrentConnectionStateViewModel()
        {
            this.productHandler = DJISDKManager.Instance.ComponentManager.GetProductHandler(0);
            this.flightControllerHandler = DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0);
            this.batteryHandler = DJISDKManager.Instance.ComponentManager.GetBatteryHandler(0, 0);
            this.InitializeDefaultValues();
        }

        public string CurrentConnectedAircraft
        {
            get => this._currentConnectedAircraft;
            set => this.SetProperty(ref this._currentConnectedAircraft, value);
        }

        public string AircraftName
        {
            get => this._aircraftName;
            set => this.SetProperty(ref this._aircraftName, value);
        }

        public bool IsConnected
        {
            get => this._isConnected;
            set => this.SetProperty(ref this._isConnected, value);
        }

        public int SatelliteCount
        {
            get => this._satelliteCount;
            set => this.SetProperty(ref this._satelliteCount, value);
        }

        public int SignalStrength
        {
            get => this._signalStrength;
            set => this.SetProperty(ref this._signalStrength, value);
        }

        public string BatteryInformation
        {
            get => $"{_powerConsumption:0.00}A, {_batteryVoltage:0.00}V, {_batteryChargeRemaining}%, {_batteryTemperature:0.0}°C";
        }

        private async void InitializeDefaultValues()
        {
            ProductTypeMsg? productType = (await this.productHandler.GetProductTypeAsync()).value;
            if (productType != null && productType?.value != ProductType.UNRECOGNIZED)
            {
                this.CurrentConnectedAircraft = productType.ToString();
            }
            else
            {
                this.CurrentConnectedAircraft = "- none -";
            }

            this.AircraftName = (await this.flightControllerHandler.GetAircraftNameAsync()).value?.value ?? "";
            this.SatelliteCount = (await this.flightControllerHandler.GetSatelliteCountAsync()).value?.value ?? 0;
            this.IsConnected = (await this.flightControllerHandler.GetConnectionAsync()).value?.value ?? false;
            this.SignalStrength = ((int?)(await this.flightControllerHandler.GetGPSSignalLevelAsync()).value?.value) ?? 0;

            this._powerConsumption = ((await this.batteryHandler.GetCurrentAsync()).value?.value ?? 0) / 1000f;
            this._batteryVoltage = ((await this.batteryHandler.GetVoltageAsync()).value?.value ?? 0) / 1000f;
            this._batteryChargeRemaining = (await this.batteryHandler.GetChargeRemainingInPercentAsync()).value?.value ?? 0;
            this._batteryTemperature = (float)((await this.batteryHandler.GetBatteryTemperatureAsync()).value?.value ?? 0);
            this.OnPropertyChanged("BatteryInformation");

            this.productHandler.ProductTypeChanged += this.ProductHandler_ProductTypeChanged;
            this.flightControllerHandler.ConnectionChanged += this.FlightControllerHandler_ConnectionChanged;
            this.flightControllerHandler.AircraftNameChanged += this.FlightControllerHandler_AircraftNameChanged;
            this.flightControllerHandler.SatelliteCountChanged += this.FlightControllerHandler_SatelliteCountChanged;
            this.flightControllerHandler.GPSSignalLevelChanged += this.FlightControllerHandler_GPSSignalLevelChanged;
            this.batteryHandler.CurrentChanged += this.BatteryHandler_CurrentChanged;
            this.batteryHandler.VoltageChanged += this.BatteryHandler_VoltageChanged;
            this.batteryHandler.ChargeRemainingInPercentChanged += this.BatteryHandler_ChargeRemainingInPercentChanged;
            this.batteryHandler.BatteryTemperatureChanged += this.BatteryHandler_BatteryTemperatureChanged;
        }

        private void BatteryHandler_BatteryTemperatureChanged(object sender, DoubleMsg? value)
        {
            this._batteryTemperature = (float)(value?.value ?? 0);
            this.OnPropertyChanged("BatteryInformation");
        }

        private void BatteryHandler_ChargeRemainingInPercentChanged(object sender, IntMsg? value)
        {
            this._batteryChargeRemaining = value?.value ?? 0;
            this.OnPropertyChanged("BatteryInformation");
        }

        private void BatteryHandler_VoltageChanged(object sender, IntMsg? value)
        {
            this._batteryVoltage = (value?.value ?? 0) / 1000f;
            this.OnPropertyChanged("BatteryInformation");
        }

        private void BatteryHandler_CurrentChanged(object sender, IntMsg? value)
        {
            this._powerConsumption = (value?.value ?? 0) / 1000f;
            this.OnPropertyChanged("BatteryInformation");
        }

        private void FlightControllerHandler_GPSSignalLevelChanged(object sender, FCGPSSignalLevelMsg? value)
        {
            this.SignalStrength = (int?)(value?.value) ?? 0;
        }

        private void FlightControllerHandler_SatelliteCountChanged(object sender, IntMsg? value)
        {
            this.SatelliteCount = value?.value ?? 0;
        }

        private void FlightControllerHandler_ConnectionChanged(object sender, BoolMsg? value)
        {
            this.IsConnected = value?.value ?? false;
        }

        private void FlightControllerHandler_AircraftNameChanged(object sender, StringMsg? value)
        {
            this.AircraftName = value?.value ?? "";
        }

        private void ProductHandler_ProductTypeChanged(object sender, ProductTypeMsg? value)
        {
            ProductType? productType = value?.value;

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
            this.flightControllerHandler.ConnectionChanged -= this.FlightControllerHandler_ConnectionChanged;
            this.flightControllerHandler.AircraftNameChanged -= this.FlightControllerHandler_AircraftNameChanged;
            this.flightControllerHandler.SatelliteCountChanged -= this.FlightControllerHandler_SatelliteCountChanged;
            this.flightControllerHandler.GPSSignalLevelChanged -= this.FlightControllerHandler_GPSSignalLevelChanged;
        }
    }
}

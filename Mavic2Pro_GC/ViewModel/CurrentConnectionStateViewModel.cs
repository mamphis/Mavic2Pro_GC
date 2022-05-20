using DJI.WindowsSDK;
using DJI.WindowsSDK.Components;
using System.Linq;

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
        private string _imuStateMessage;
        private string _compassStateMessage;
        private bool _compassHasError;

        private readonly ProductHandler productHandler;
        private readonly FlightControllerHandler flightControllerHandler;
        private readonly BatteryHandler batteryHandler;
        private readonly FlightAssistantHandler flightAssistantHandler;


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

        public string CompassStateMessage
        {
            get => this._compassStateMessage;
            set => SetProperty(ref this._compassStateMessage, value);
        }

        public string IMUStateMessage
        {
            get => this._imuStateMessage;
            set => SetProperty(ref this._imuStateMessage, value);
        }

        private async void InitializeDefaultValues()
        {
            ProductTypeMsg? productType = (await this.productHandler.GetProductTypeAsync()).value;

            this.CurrentConnectedAircraft = productType.ToString();
            this.AircraftName = (await this.flightControllerHandler.GetAircraftNameAsync()).value?.value ?? "";
            this.SatelliteCount = (await this.flightControllerHandler.GetSatelliteCountAsync()).value?.value ?? 0;
            this.IsConnected = (await this.flightControllerHandler.GetConnectionAsync()).value?.value ?? false;
            this.SignalStrength = ((int?)(await this.flightControllerHandler.GetGPSSignalLevelAsync()).value?.value) ?? 0;

            this._powerConsumption = ((await this.batteryHandler.GetCurrentAsync()).value?.value ?? 0) / 1000f;
            this._batteryVoltage = ((await this.batteryHandler.GetVoltageAsync()).value?.value ?? 0) / 1000f;
            this._batteryChargeRemaining = (await this.batteryHandler.GetChargeRemainingInPercentAsync()).value?.value ?? 0;
            this._batteryTemperature = (float)((await this.batteryHandler.GetBatteryTemperatureAsync()).value?.value ?? 0);
            this.OnPropertyChanged("BatteryInformation");

            // 
            var compassHasError = (await this.flightControllerHandler.GetCompassHasErrorAsync()).value?.value ?? false;
            if (compassHasError)
            {
                this.flightControllerHandler.StartCompasCalibrationAsync();
            }

            var value = (await this.flightControllerHandler.GetCompassCalibrationStateAsync()).value;
            this.CompassStateMessage = (value?.value.ToString()) ?? "";
            this.IMUStateMessage = (await this.flightControllerHandler.GetIMUStateAsync()).value?.value.Select(state => state.calibrationState.ToString() + ", " + state.multipleOrientationCalibrationHint.state.ToString()).Aggregate((a, b) => a + "; " + b);
            //switch (a.Value)
            //{
            //    case FCCompassCalibrationState.IDLE:
            //        break;
            //    case FCCompassCalibrationState.HORIZONTAL:
            //        break;
            //    case FCCompassCalibrationState.VERTICAL:
            //        break;
            //    case FCCompassCalibrationState.SUCCEEDED:
            //        break;
            //    case FCCompassCalibrationState.FAILED:
            //        break;
            //    case FCCompassCalibrationState.UNKNOWN:
            //        break;
            //    default:
            //        break;
            //}

            this.productHandler.ProductTypeChanged += this.ProductHandler_ProductTypeChanged;
            this.flightControllerHandler.ConnectionChanged += this.FlightControllerHandler_ConnectionChanged;
            this.flightControllerHandler.AircraftNameChanged += this.FlightControllerHandler_AircraftNameChanged;
            this.flightControllerHandler.SatelliteCountChanged += this.FlightControllerHandler_SatelliteCountChanged;
            this.flightControllerHandler.GPSSignalLevelChanged += this.FlightControllerHandler_GPSSignalLevelChanged;
            this.batteryHandler.CurrentChanged += this.BatteryHandler_CurrentChanged;
            this.batteryHandler.VoltageChanged += this.BatteryHandler_VoltageChanged;
            this.batteryHandler.ChargeRemainingInPercentChanged += this.BatteryHandler_ChargeRemainingInPercentChanged;
            this.batteryHandler.BatteryTemperatureChanged += this.BatteryHandler_BatteryTemperatureChanged;
            this.flightControllerHandler.CompassCalibrationStateChanged += this.FlightControllerHandler_CompassCalibrationStateChanged;
            this.flightControllerHandler.IMUStateChanged += this.FlightControllerHandler_IMUStateChanged;
            this.flightControllerHandler.CompassHasErrorChanged += this.FlightControllerHandler_CompassHasErrorChanged;
        }

        private void FlightControllerHandler_CompassHasErrorChanged(object sender, BoolMsg? value)
        {
            var compassHasError = value?.value ?? false;
            if (compassHasError)
            {
                this.flightControllerHandler.StartCompasCalibrationAsync();
            }
        }

        private void FlightControllerHandler_IMUStateChanged(object sender, IMUStates? value)
        {
            this.IMUStateMessage = value?.value.Select(state => state.calibrationState.ToString() + ", " + state.multipleOrientationCalibrationHint.state.ToString()).Aggregate((a, b) => a + "; " + b);
        }

        private void FlightControllerHandler_CompassCalibrationStateChanged(object sender, FCCompassCalibrationStateMsg? value)
        {
            this.CompassStateMessage = (value?.value.ToString()) ?? "";
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

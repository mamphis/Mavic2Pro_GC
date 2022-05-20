using DJI.WindowsSDK;
using DJI.WindowsSDK.Components;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;

namespace Mavic2Pro_GC.ViewModel
{
    internal class SimpleTakeOffAndLandingViewModel : ViewModelBase
    {
        public SimpleTakeOffAndLandingViewModel(CurrentConnectionStateViewModel currentConnectionState)
        {
            this.currentConnectionState = currentConnectionState;
            this.flightController = DJISDKManager.Instance.ComponentManager.GetFlightControllerHandler(0, 0);
            this.Initialize();
        }

        public bool IsFlying
        {
            get => this._isFlying;
            set => SetProperty(ref this._isFlying, value);
        }
        public bool IsLandingConfirmationNeeded
        {
            get => this._isLandingConfirmationNeeded;
            set => SetProperty(ref this._isLandingConfirmationNeeded, value);
        }

        private readonly CurrentConnectionStateViewModel currentConnectionState;
        private readonly FlightControllerHandler flightController;
        private bool _isMotorOn = false;
        private bool _isFlying = false;
        private bool _isLandingConfirmationNeeded = false;

        private async void Initialize()
        {
            var isMotorOnMsg = (await this.flightController.GetAreMotorsOnAsync()).value;
            this._isMotorOn = isMotorOnMsg?.value == true;

            var isFlyingMsg = (await this.flightController.GetIsFlyingAsync()).value;
            this.IsFlying = isFlyingMsg?.value == true;

            var isLandingConfirmationNeeded = (await this.flightController.GetIsLandingConfirmationNeededAsync()).value;
            this.IsLandingConfirmationNeeded = isLandingConfirmationNeeded?.value == true;

            this.flightController.AreMotorsOnChanged += this.FlightController_AreMotorsOnChanged;
            this.flightController.IsFlyingChanged += this.FlightController_IsFlyingChanged;
            this.flightController.IsLandingConfirmationNeededChanged += this.FlightController_IsLandingConfirmationNeededChanged;
        }

        private void FlightController_IsLandingConfirmationNeededChanged(object sender, BoolMsg? isLandingConfirmationNeeded)
        {
            this.IsLandingConfirmationNeeded = isLandingConfirmationNeeded?.value == true;
            this.UpdateExecuteState();
        }

        private void FlightController_IsFlyingChanged(object sender, BoolMsg? isFlyingMsg)
        {
            this.IsFlying = isFlyingMsg?.value == true;
            this.UpdateExecuteState();
        }

        private void FlightController_AreMotorsOnChanged(object sender, BoolMsg? isMotorOnMsg)
        {
            this._isMotorOn = isMotorOnMsg?.value == true;
            this.UpdateExecuteState();
        }

        private void UpdateExecuteState()
        {
            this._confirmLandCommand?.RaiseCanExecuteChanged();
            this._landCommand?.RaiseCanExecuteChanged();
            this._takeOffCommand?.RaiseCanExecuteChanged();
        }

        private bool CanTakeOff()
        {
            return this.FlightControllerReady() && !this._isMotorOn;
        }

        private bool CanLand()
        {
            return this.FlightControllerReady() && this._isMotorOn && this._isFlying;
        }

        private bool CanConfirmLand()
        {
            return this.FlightControllerReady() && this._isLandingConfirmationNeeded;
        }

        private bool FlightControllerReady()
        {
            return this.currentConnectionState.CurrentConnectedAircraft != null && this.flightController != null;
        }

        public RelayCommand _confirmLandCommand;
        public ICommand ConfirmLandCommand
        {
            get
            {
                if (_confirmLandCommand == null)
                {
                    _confirmLandCommand = new RelayCommand(async () => 
                    {
                        await this.flightController.ConfirmLandingAsync();
                    }, () => this.CanConfirmLand());
                }
                return _confirmLandCommand;
            }
        }

        public RelayCommand _landCommand;
        public ICommand LandCommand
        {
            get
            {
                if (_landCommand == null)
                {
                    _landCommand = new RelayCommand(async () => 
                    {
                        await this.flightController.StartAutoLandingAsync();
                    }, () => this.CanLand());
                }
                return _landCommand;
            }
        }
        public RelayCommand _takeOffCommand;
        public ICommand TakeOffCommand
        {
            get
            {
                if (_takeOffCommand == null)
                {
                    _takeOffCommand = new RelayCommand(async () =>
                    {
                        await this.flightController.StartTakeoffAsync();
                    }, () => this.CanTakeOff());
                }
                return _takeOffCommand;
            }
        }
    }
}

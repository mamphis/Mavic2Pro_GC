using DJI.WindowsSDK;
using GalaSoft.MvvmLight.Threading;
using Mavic2Pro_GC.View;
using Mavic2Pro_GC.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace Mavic2Pro_GC
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly Dictionary<string, Type> navigationItems = new Dictionary<string, Type>()
        {
            // { "Login", typeof(UserLogin) },
            { "Simple Takeoff and Land", typeof(SimpleTakeOffAndLanding) }
        };

        public MainPage()
        {
            this.InitializeComponent();
            DispatcherHelper.Initialize();
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            DJISDKManager.Instance.SDKRegistrationStateChanged += this.Instance_SDKRegistrationEvent;
            string key = File.ReadAllText("API.key");

            DJISDKManager.Instance.RegisterApp(key);
        }

        internal CurrentConnectionStateViewModel ConnectionStateViewModel { get; set; }

        private async void Instance_SDKRegistrationEvent(SDKRegistrationState state, SDKError resultCode)
        {
            if (resultCode == SDKError.NO_ERROR)
            {
                System.Diagnostics.Debug.WriteLine("Register app successfully.");
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {

                    foreach (KeyValuePair<string, Type> navItem in this.navigationItems)
                    {
                        this.NavView.MenuItems.Add(navItem.Key);
                    }

                    this.NavView.UpdateLayout();
                    this.ConnectionStateViewModel = new CurrentConnectionStateViewModel();
                    this.stackPanelConnectionStatus.DataContext = this.ConnectionStateViewModel;
                });
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Register SDK failed, the error is: ");
                System.Diagnostics.Debug.WriteLine(resultCode.ToString());
            }
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            string invokedItem = args.InvokedItem as string;
            if (this.navigationItems.ContainsKey(invokedItem))
            {
                Type page = this.navigationItems[invokedItem];
                if (this.ContentFrame.SourcePageType != page)
                {
                    this.ContentFrame.Navigate(page, this.ConnectionStateViewModel);
                }
            }
        }
    }
}

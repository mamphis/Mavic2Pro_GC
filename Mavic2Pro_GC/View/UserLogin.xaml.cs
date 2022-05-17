// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Mavic2Pro_GC.View
{
    using DJI.WindowsSDK;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Windows.Foundation;
    using Windows.Foundation.Collections;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class UserLogin : Page
    {
        public UserLogin()
        {
            this.InitializeComponent();

            var loginWebView = DJISDKManager.Instance.UserAccountManager.CreateLoginView(false);
            if (contentGrid.Children.Count == 0)
            {
                contentGrid.Children.Add(loginWebView);
            }
        }
    }
}

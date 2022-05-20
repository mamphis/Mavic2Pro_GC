using Mavic2Pro_GC.ViewModel;
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

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Mavic2Pro_GC.View
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class SimpleTakeOffAndLanding : Page
    {
        public SimpleTakeOffAndLanding()
        {
            this.InitializeComponent();
        }

        internal SimpleTakeOffAndLandingViewModel VM { get; private set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var currentConnectionState = e.Parameter as CurrentConnectionStateViewModel;
            if (currentConnectionState != null)
            {
                this.DataContext = new SimpleTakeOffAndLandingViewModel(currentConnectionState);
            }
        }
    }
}

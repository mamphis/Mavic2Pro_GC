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

        private void btnTakeOff_Click(object sender, RoutedEventArgs e)
        {
            var msgbox = new ContentDialog
            {
                Title = "Button",
                Content = "The aircraft will now take off ;)",
                CloseButtonText = "OK"
            };
            msgbox.ShowAsync();
        }

        private void btnLand_Click(object sender, RoutedEventArgs e)
        {
            var msgbox = new ContentDialog
            {
                Title = "Button",
                Content = "The aircraft will now try to land ;)",
                CloseButtonText = "OK"
            };
            msgbox.ShowAsync();

        }
    }
}

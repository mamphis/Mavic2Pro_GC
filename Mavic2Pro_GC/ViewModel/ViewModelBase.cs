using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mavic2Pro_GC.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string fieldName = null)
        {
            System.Diagnostics.Debug.WriteLine($"Changing {fieldName} from {field} to {value}");
            field = value;
            this.OnPropertyChanged(fieldName);
        }

        /// <summary>
        /// Occurs immediately after a property of this instance has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        protected async void OnPropertyChanged(string changedPropertyName)
        {
            // The OnPropertyChanged is not virtual itself because of the [CallerMemberName] attribute which in case overridden should be put in every override - quite error-prone.
            this.PropertyChangedOverride(changedPropertyName);
            await DispatcherHelper.UIDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(changedPropertyName));
            });
        }

        /// <summary>
        /// Provides an entry point for inheritors to provide additional logic over the PropertyChanged routine.
        /// </summary>
        protected virtual void PropertyChangedOverride(string changedPropertyName)
        {
        }
    }
}

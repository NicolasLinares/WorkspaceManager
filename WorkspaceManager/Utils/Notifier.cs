using System.ComponentModel;
using System.Linq;

namespace WorkspaceManagerTool.Util
{
    /// <summary>    
    /// Implement the notification interface of changes in the property values.
    /// </summary>
    /// <remarks>
    /// It enable the observers to be notified with the value changes in certain properties.    
    /// </remarks>
    public abstract class Notifier : INotifyPropertyChanged {
        private PropertyChangedEventHandler propertyChanged;

        /// <summary>        
        /// It triggers the event that will notify the observers about a property value change.
        /// </summary>
        /// <remarks>        
        /// The method <see cref="SetValue"/> sets the value to the property and call this method.
        /// </remarks>
        /// <param name="nombre">Name of the property..</param>
        protected void Notify(string nombre) {
            if (propertyChanged != null) {
                propertyChanged(this, new PropertyChangedEventArgs(nombre));
            }
        }

        /// <summary>
        /// Any client using ViewModel pattern should call this method in order to update a property value.
        /// This guarantee a correct updating o the view components.
        /// Any
        /// </summary>
        /// <typeparam name="T">Property type.</typeparam>
        /// <param name="campo">Reference to a private member storing the property value.</param>
        /// <param name="valor">New property value.</param>
        /// <param name="nombre">Name of the property (not of the private member).</param>
        protected void SetValue<T>(ref T campo, T valor, string nombre) {
            campo = valor;
            Notify(nombre);
        }

        // ----------- //
        // PROPIEDADES //
        // ----------- //

        /// <summary>
        /// Subscribers will receive events for each property value changes in the ViewModel.        
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged {
            add {
                if (propertyChanged == null || !propertyChanged.GetInvocationList().Contains(value)) {
                    propertyChanged += value;
                }
            }
            remove {
                propertyChanged -= value;
            }
        }
    }
}

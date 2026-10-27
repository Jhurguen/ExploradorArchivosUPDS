using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExploradorArchivos.ViewModels.Base
{
    /// <summary>
    /// Clase base para implementar el patrón INotifyPropertyChanged de manera limpia,
    /// permitiendo actualizar la interfaz de usuario automáticamente ante cambios en los ViewModels.
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? nombrePropiedad = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombrePropiedad));
        }

        protected bool SetProperty<T>(ref T campoAlmacenamiento, T valorNuevo, [CallerMemberName] string? nombrePropiedad = null)
        {
            if (Equals(campoAlmacenamiento, valorNuevo)) return false;

            campoAlmacenamiento = valorNuevo;
            OnPropertyChanged(nombrePropiedad);
            return true;
        }
    }
}
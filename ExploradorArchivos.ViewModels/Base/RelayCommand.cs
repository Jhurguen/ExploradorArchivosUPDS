using System;
using System.Windows.Input;

namespace ExploradorArchivos.ViewModels
{
    /// <summary>
    /// Implementación estándar de ICommand para delegar la lógica de los comandos 
    /// desde la vista hacia el ViewModel en la arquitectura MVVM.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _ejecutarAccion;
        private readonly Predicate<object> _puedeEjecutarPredicado;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action<object> ejecutarAccion, Predicate<object> puedeEjecutarPredicado = null)
        {
            _ejecutarAccion = ejecutarAccion ?? throw new ArgumentNullException(nameof(ejecutarAccion));
            _puedeEjecutarPredicado = puedeEjecutarPredicado;
        }

        public bool CanExecute(object parametro)
        {
            return _puedeEjecutarPredicado == null || _puedeEjecutarPredicado(parametro);
        }

        public void Execute(object parametro)
        {
            _ejecutarAccion(parametro);
        }
    }
}
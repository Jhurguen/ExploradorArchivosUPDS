using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ExploradorArchivos.Core.Estructuras;
using ExploradorArchivos.Core.Modelos;
using ExploradorArchivos.Core.Servicios;
using ExploradorArchivos.ViewModels.Base;

namespace ExploradorArchivos.ViewModels
{
    /// <summary>
    /// ViewModel principal para coordinar la lógica de la interfaz y las estructuras de datos del explorador.
    /// </summary>
    public class VentanaPrincipalViewModel : ObservableObject
    {
        private readonly GestorSistemaArchivos _gestorArchivos;
        private readonly PilaHistorial _historialNavegacion;

        private string _rutaActual;
        public string RutaActual
        {
            get => _rutaActual;
            set => SetProperty(ref _rutaActual, value);
        }

        private string _nombreNuevoElemento = string.Empty;
        public string NombreNuevoElemento
        {
            get => _nombreNuevoElemento;
            set => SetProperty(ref _nombreNuevoElemento, value);
        }

        private string _tipoArchivoSeleccionado = ".txt";
        public string TipoArchivoSeleccionado
        {
            get => _tipoArchivoSeleccionado;
            set => SetProperty(ref _tipoArchivoSeleccionado, value);
        }

        private ElementoArchivo? _elementoSeleccionado;
        public ElementoArchivo? ElementoSeleccionado
        {
            get => _elementoSeleccionado;
            set => SetProperty(ref _elementoSeleccionado, value);
        }

        private string _mensajeEstado = string.Empty;
        public string MensajeEstado
        {
            get => _mensajeEstado;
            set => SetProperty(ref _mensajeEstado, value);
        }

        // Colecciones enlazadas a la Interfaz de Usuario (WPF)
        public ObservableCollection<ElementoArchivo> ElementosActuales { get; set; }
        public ObservableCollection<ElementoArchivo> UnidadesDisponibles { get; set; }
        public IReadOnlyList<string> TiposArchivoDisponibles { get; } =
            [".txt", ".docx", ".xlsx", ".pptx", ".pdf", ".json", ".xml", ".html", ".css", ".js", ".cs", ".md"];

        // Comandos de navegación y acciones
        public ICommand NavegarComando { get; }
        public ICommand AtrasComando { get; }
        public ICommand AdelanteComando { get; }
        public ICommand CrearCarpetaComando { get; }
        public ICommand CrearArchivoComando { get; }
        public ICommand RenombrarComando { get; }
        public ICommand EliminarComando { get; }

        public VentanaPrincipalViewModel()
        {
            _gestorArchivos = new GestorSistemaArchivos();
            _historialNavegacion = new PilaHistorial();

            ElementosActuales = new ObservableCollection<ElementoArchivo>();
            UnidadesDisponibles = new ObservableCollection<ElementoArchivo>();

            // Inicializar Comandos
            NavegarComando = new RelayCommand(parametro => CargarCarpeta((string)parametro));
            AtrasComando = new RelayCommand(_ => EjecutarAtras(), _ => _historialNavegacion.TieneAtras());
            AdelanteComando = new RelayCommand(_ => EjecutarAdelante(), _ => _historialNavegacion.TieneAdelante());
            CrearCarpetaComando = new RelayCommand(_ => CrearCarpeta());
            CrearArchivoComando = new RelayCommand(_ => CrearArchivo());
            RenombrarComando = new RelayCommand(_ => RenombrarElemento());
            EliminarComando = new RelayCommand(_ => EliminarElemento());

            // Cargar unidades al iniciar la aplicación
            CargarUnidades();
        }

        private void CargarUnidades()
        {
            UnidadesDisponibles.Clear();
            var unidades = _gestorArchivos.ObtenerUnidades();
            foreach (var unidad in unidades)
            {
                UnidadesDisponibles.Add(unidad);
            }

            // Si hay unidades, cargar la primera por defecto
            if (UnidadesDisponibles.Count > 0)
            {
                CargarCarpeta(UnidadesDisponibles[0].RutaAbsoluta);
            }
        }

        public void CargarCarpeta(string nuevaRuta)
        {
            if (string.IsNullOrEmpty(nuevaRuta) || !Directory.Exists(nuevaRuta))
                return;

            // Registrar visita en la Pila de Historial si ya había una ruta anterior
            if (!string.IsNullOrEmpty(RutaActual))
            {
                _historialNavegacion.RegistrarVisita(RutaActual, nuevaRuta);
            }

            RutaActual = nuevaRuta;
            ElementosActuales.Clear();

            var elementos = _gestorArchivos.ObtenerElementosDeRuta(nuevaRuta);
            foreach (var el in elementos)
            {
                ElementosActuales.Add(el);
            }
        }

        private void EjecutarAtras()
        {
            if (!_historialNavegacion.TieneAtras()) return;

            string rutaAnterior = _historialNavegacion.Retroceder(RutaActual);
            if (!string.IsNullOrEmpty(rutaAnterior))
            {
                RutaActual = rutaAnterior;
                ElementosActuales.Clear();
                var elementos = _gestorArchivos.ObtenerElementosDeRuta(rutaAnterior);
                foreach (var el in elementos)
                {
                    ElementosActuales.Add(el);
                }
            }
        }

        private void EjecutarAdelante()
        {
            if (!_historialNavegacion.TieneAdelante()) return;

            string rutaSiguiente = _historialNavegacion.Avanzar(RutaActual);
            if (!string.IsNullOrEmpty(rutaSiguiente))
            {
                RutaActual = rutaSiguiente;
                ElementosActuales.Clear();
                var elementos = _gestorArchivos.ObtenerElementosDeRuta(rutaSiguiente);
                foreach (var el in elementos)
                {
                    ElementosActuales.Add(el);
                }
            }
        }

        private void ActualizarElementos()
        {
            ElementosActuales.Clear();
            foreach (var el in _gestorArchivos.ObtenerElementosDeRuta(RutaActual))
            {
                ElementosActuales.Add(el);
            }
        }

        private void EjecutarOperacionArchivo(Action operacion, string mensajeExito)
        {
            if (string.IsNullOrEmpty(RutaActual)) return;

            try
            {
                operacion();
                MensajeEstado = mensajeExito;
                NombreNuevoElemento = string.Empty;
                ActualizarElementos();
            }
            catch (Exception ex) when (ex is IOException or ArgumentException or UnauthorizedAccessException)
            {
                MensajeEstado = ex.Message;
            }
        }

        private void CrearCarpeta()
        {
            EjecutarOperacionArchivo(() => _gestorArchivos.CrearCarpeta(RutaActual, NombreNuevoElemento), "Carpeta creada correctamente.");
        }

        private void CrearArchivo()
        {
            var nombreBase = NombreNuevoElemento.Trim();
            if (string.IsNullOrEmpty(nombreBase))
            {
                MensajeEstado = "Escriba un nombre para el archivo.";
                return;
            }

            var extension = TipoArchivoSeleccionado ?? string.Empty;
            var nombreCompleto = nombreBase.EndsWith(extension, StringComparison.OrdinalIgnoreCase) ? nombreBase : nombreBase + extension;

            EjecutarOperacionArchivo(() => _gestorArchivos.CrearArchivo(RutaActual, nombreCompleto), "Archivo creado correctamente.");
        }

        private void RenombrarElemento()
        {
            if (ElementoSeleccionado == null)
            {
                MensajeEstado = "Seleccione un elemento para renombrar.";
                return;
            }

            var elemento = ElementoSeleccionado;
            EjecutarOperacionArchivo(() => _gestorArchivos.RenombrarElemento(elemento.RutaAbsoluta, NombreNuevoElemento), "Elemento renombrado correctamente.");
        }

        private void EliminarElemento()
        {
            if (ElementoSeleccionado == null)
            {
                MensajeEstado = "Seleccione un elemento para eliminar.";
                return;
            }

            var elemento = ElementoSeleccionado;
            var confirmacion = MessageBox.Show(
                $"¿Eliminar '{elemento.Nombre}'? Esta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmacion != MessageBoxResult.Yes) return;

            EjecutarOperacionArchivo(() => _gestorArchivos.EliminarElemento(elemento.RutaAbsoluta), "Elemento eliminado correctamente.");
        }
    }
}
using System.Collections.Generic;

namespace ExploradorArchivos.Core.Estructuras
{
    /// <summary>
    /// Gestiona el historial de navegación de rutas utilizando la estructura de datos Pila (Stack).
    /// El funcionamiento es LIFO (Last In, First Out / Último en entrar, primero en salir).
    /// </summary>
    public class PilaHistorial
    {
        // Pila para registrar las rutas visitadas anteriormente (Botón Atrás)
        private Stack<string> PilaAtras;

        // Pila para registrar las rutas a las que se puede volver a avanzar (Botón Adelante)
        private Stack<string> PilaAdelante;

        /// <summary>
        /// Constructor para inicializar las pilas de historial.
        /// </summary>
        public PilaHistorial()
        {
            PilaAtras = new Stack<string>();
            PilaAdelante = new Stack<string>();
        }

        /// <summary>
        /// Registra una nueva visita a una ruta. Limpia la pila de adelante porque 
        /// al navegar a un sitio nuevo se interrumpe el flujo hacia adelante.
        /// </summary>
        public void RegistrarVisita(string rutaActual, string nuevaRuta)
        {
            if (!string.IsNullOrEmpty(rutaActual))
            {
                PilaAtras.Push(rutaActual);
            }
            PilaAdelante.Clear();
        }

        /// <summary>
        /// Retrocede a la ruta anterior. Mueve la ruta actual a la pila de adelante.
        /// </summary>
        public string Retroceder(string rutaActual)
        {
            if (!TieneAtras()) return null;

            PilaAdelante.Push(rutaActual);
            return PilaAtras.Pop();
        }

        /// <summary>
        /// Avanza a la siguiente ruta. Mueve la ruta actual de regreso a la pila de atrás.
        /// </summary>
        public string Avanzar(string rutaActual)
        {
            if (!TieneAdelante()) return null;

            PilaAtras.Push(rutaActual);
            return PilaAdelante.Pop();
        }

        /// <summary>
        /// Indica si es posible retroceder en el historial.
        /// </summary>
        public bool TieneAtras()
        {
            return PilaAtras.Count > 0;
        }

        /// <summary>
        /// Indica si es posible avanzar en el historial.
        /// </summary>
        public bool TieneAdelante()
        {
            return PilaAdelante.Count > 0;
        }

        /// <summary>
        /// Limpia por completo el historial de navegación.
        /// </summary>
        public void LimpiarHistorial()
        {
            PilaAtras.Clear();
            PilaAdelante.Clear();
        }
    }
}
using System;

namespace ExploradorArchivos.Core.Modelos
{
    /// <summary>
    /// Representa una tarea individual que será encolada (usando nuestra ColaTareas)
    /// para ejecutarse de forma ordenada (ej: copiar, mover o eliminar).
    /// </summary>
    public class TareaArchivo
    {
        public string IdTarea { get; set; }
        public string Descripcion { get; set; }
        public string RutaOrigen { get; set; }
        public string RutaDestino { get; set; }
        public string TipoOperacion { get; set; } // "Copiar", "Mover", "Eliminar"
        public DateTime FechaCreacion { get; set; }

        public TareaArchivo(string descripcion, string rutaOrigen, string rutaDestino, string tipoOperacion)
        {
            IdTarea = Guid.NewGuid().ToString().Substring(0, 8);
            Descripcion = descripcion;
            RutaOrigen = rutaOrigen;
            RutaDestino = rutaDestino;
            TipoOperacion = tipoOperacion;
            FechaCreacion = DateTime.Now;
        }
    }
}
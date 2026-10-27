using System;

namespace ExploradorArchivos.Core.Modelos
{
    /// <summary>
    /// Representa de manera genérica un elemento físico del disco (puede ser un archivo o una carpeta).
    /// </summary>
    public class ElementoArchivo
    {
        public string Nombre { get; set; } = string.Empty;
        public string RutaAbsoluta { get; set; } = string.Empty;
        public long TamanoBytes { get; set; }
        public DateTime FechaModificacion { get; set; }
        public bool EsCarpeta { get; set; }

        // Propiedad formateada para mostrar el tamaño amigablemente en la interfaz
        public string TamanoFormateado
        {
            get
            {
                if (EsCarpeta) return "--";

                double kb = TamanoBytes / 1024.0;
                if (kb < 1024) return $"{kb:N1} KB";

                double mb = kb / 1024.0;
                if (mb < 1024) return $"{mb:N1} MB";

                double gb = mb / 1024.0;
                return $"{gb:N2} GB";
            }
        }
    }
}
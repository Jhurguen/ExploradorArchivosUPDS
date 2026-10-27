using System;
using System.IO;

namespace ExploradorArchivos.Core.Estructuras
{
    /// <summary>
    /// Gestiona la construcción y el recorrido del Árbol N-ario de directorios 
    /// utilizando las carpetas reales del sistema operativo.
    /// </summary>
    public class ArbolDirectorios
    {
        // Nodo raíz del árbol (por ejemplo, una unidad como "C:\" o una carpeta principal)
        public NodoDirectorio? Raiz { get; private set; }

        /// <summary>
        /// Construye el árbol de directorios a partir de una ruta raíz dada.
        /// </summary>
        public void ConstruirArbol(string rutaRaiz)
        {
            if (!Directory.Exists(rutaRaiz))
                return;

            var infoDirectorio = new DirectoryInfo(rutaRaiz);
            Raiz = new NodoDirectorio(infoDirectorio.FullName, infoDirectorio.Name);

            // Poblamos el primer nivel de subcarpetas
            PoblarSubCarpetas(Raiz);
        }

        /// <summary>
        /// Método para poblar las subcarpetas de un nodo específico de forma segura.
        /// </summary>
        public void PoblarSubCarpetas(NodoDirectorio nodoActual)
        {
            if (nodoActual == null) return;

            try
            {
                var directorioInfo = new DirectoryInfo(nodoActual.RutaAbsoluta);

                foreach (var subDirectorio in directorioInfo.GetDirectories())
                {
                    // Evitamos procesar carpetas del sistema ocultas o inaccesibles para prevenir bloqueos
                    if ((subDirectorio.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                        continue;

                    var nodoHijo = new NodoDirectorio(subDirectorio.FullName, subDirectorio.Name, nodoActual);
                    nodoActual.AgregarSubCarpeta(nodoHijo);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Captura excepciones de carpetas protegidas por Windows sin detener el programa
            }
            catch (Exception)
            {
                // Manejo general para errores de E/S
            }
        }
    }
}
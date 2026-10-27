using System.Collections.Generic;

namespace ExploradorArchivos.Core.Estructuras
{
    /// <summary>
    /// Representa un nodo individual dentro del Árbol N-ario de directorios.
    /// Cada nodo almacena información sobre una carpeta física y sus respectivas subcarpetas.
    /// </summary>
    public class NodoDirectorio
    {
        // Ruta absoluta en el disco duro (ej: "C:\CarpetaEjemplo")
        public string RutaAbsoluta { get; set; }

        // Nombre visible de la carpeta
        public string NombreCarpeta { get; set; }

        // Referencia al nodo padre (permite retroceder en la jerarquía del árbol)
        public NodoDirectorio NodoPadre { get; set; }

        // Lista de nodos hijos (característica fundamental del Árbol N-ario)
        public List<NodoDirectorio> SubCarpetas { get; set; }

        /// <summary>
        /// Constructor para inicializar un nuevo nodo de directorio.
        /// </summary>
        public NodoDirectorio(string rutaAbsoluta, string nombreCarpeta, NodoDirectorio nodoPadre = null)
        {
            RutaAbsoluta = rutaAbsoluta;
            NombreCarpeta = nombreCarpeta;
            NodoPadre = nodoPadre;
            SubCarpetas = new List<NodoDirectorio>();
        }

        /// <summary>
        /// Método para agregar una subcarpeta como nodo hijo al árbol.
        /// </summary>
        public void AgregarSubCarpeta(NodoDirectorio subCarpeta)
        {
            SubCarpetas.Add(subCarpeta);
        }
    }
}
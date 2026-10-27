using System;
using System.Collections.Generic;
using System.IO;
using ExploradorArchivos.Core.Modelos;

namespace ExploradorArchivos.Core.Servicios
{
    /// <summary>
    /// Servicio encargado de interactuar directamente con el disco duro utilizando System.IO
    /// para listar unidades, carpetas y archivos reales.
    /// </summary>
    public class GestorSistemaArchivos
    {
        /// <summary>
        /// Obtiene todas las unidades de almacenamiento disponibles en el equipo (ej: C:\, D:\).
        /// </summary>
        public List<ElementoArchivo> ObtenerUnidades()
        {
            var listaUnidades = new List<ElementoArchivo>();

            try
            {
                foreach (var unidad in DriveInfo.GetDrives())
                {
                    if (unidad.IsReady)
                    {
                        listaUnidades.Add(new ElementoArchivo
                        {
                            Nombre = unidad.Name,
                            RutaAbsoluta = unidad.RootDirectory.FullName,
                            EsCarpeta = true,
                            TamanoBytes = 0,
                            FechaModificacion = DateTime.Now
                        });
                    }
                }
            }
            catch (Exception)
            {
                // Manejo de excepciones en caso de error de lectura de unidades
            }

            return listaUnidades;
        }

        /// <summary>
        /// Obtiene los elementos (carpetas y archivos) contenidos en una ruta específica de manera segura.
        /// </summary>
        public List<ElementoArchivo> ObtenerElementosDeRuta(string rutaCarpeta)
        {
            var elementos = new List<ElementoArchivo>();

            if (string.IsNullOrEmpty(rutaCarpeta) || !Directory.Exists(rutaCarpeta))
                return elementos;

            try
            {
                var directorioInfo = new DirectoryInfo(rutaCarpeta);

                // 1. Listar Subcarpetas
                foreach (var subDir in directorioInfo.GetDirectories())
                {
                    if ((subDir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                        continue; // Omitir ocultas por limpieza visual

                    elementos.Add(new ElementoArchivo
                    {
                        Nombre = subDir.Name,
                        RutaAbsoluta = subDir.FullName,
                        EsCarpeta = true,
                        TamanoBytes = CalcularTamanoCarpeta(subDir.FullName),
                        FechaModificacion = subDir.LastWriteTime
                    });
                }

                // 2. Listar Archivos
                foreach (var archivo in directorioInfo.GetFiles())
                {
                    if ((archivo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                        continue;

                    elementos.Add(new ElementoArchivo
                    {
                        Nombre = archivo.Name,
                        RutaAbsoluta = archivo.FullName,
                        EsCarpeta = false,
                        TamanoBytes = archivo.Length,
                        FechaModificacion = archivo.LastWriteTime
                    });
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Acceso denegado a carpetas protegidas del sistema
            }
            catch (Exception)
            {
                // Manejo general de errores de E/S
            }

            return elementos;
        }

        /// <summary>
        /// Crea una nueva carpeta dentro de la ruta indicada y devuelve su ruta absoluta.
        /// </summary>
        public string CrearCarpeta(string rutaPadre, string nombre)
        {
            var rutaNueva = Path.Combine(rutaPadre, ValidarNombre(nombre));
            VerificarNoExiste(rutaNueva, nombre);

            Directory.CreateDirectory(rutaNueva);
            return rutaNueva;
        }

        /// <summary>
        /// Crea un nuevo archivo vacío dentro de la ruta indicada y devuelve su ruta absoluta.
        /// </summary>
        public string CrearArchivo(string rutaPadre, string nombre)
        {
            var rutaNueva = Path.Combine(rutaPadre, ValidarNombre(nombre));
            VerificarNoExiste(rutaNueva, nombre);

            File.Create(rutaNueva).Dispose();
            return rutaNueva;
        }

        /// <summary>
        /// Cambia el nombre de un archivo o carpeta y devuelve la nueva ruta absoluta.
        /// </summary>
        public string RenombrarElemento(string rutaOriginal, string nuevoNombre)
        {
            var directorioPadre = Path.GetDirectoryName(rutaOriginal)
                ?? throw new ArgumentException("La ruta del elemento no es válida.");
            var rutaNueva = Path.Combine(directorioPadre, ValidarNombre(nuevoNombre));

            if (string.Equals(rutaNueva, rutaOriginal, StringComparison.OrdinalIgnoreCase))
                return rutaOriginal;

            VerificarNoExiste(rutaNueva, nuevoNombre);

            if (Directory.Exists(rutaOriginal))
                Directory.Move(rutaOriginal, rutaNueva);
            else if (File.Exists(rutaOriginal))
                File.Move(rutaOriginal, rutaNueva);
            else
                throw new FileNotFoundException("El elemento seleccionado ya no existe.", rutaOriginal);

            return rutaNueva;
        }

        /// <summary>
        /// Elimina un archivo o una carpeta (con todo su contenido).
        /// </summary>
        public void EliminarElemento(string ruta)
        {
            if (Directory.Exists(ruta))
                Directory.Delete(ruta, true);
            else if (File.Exists(ruta))
                File.Delete(ruta);
            else
                throw new FileNotFoundException("El elemento seleccionado ya no existe.", ruta);
        }

        private static long CalcularTamanoCarpeta(string rutaCarpeta)
        {
            long tamano = 0;

            try
            {
                foreach (var archivo in new DirectoryInfo(rutaCarpeta).EnumerateFiles("*", SearchOption.AllDirectories))
                {
                    try
                    {
                        tamano += archivo.Length;
                    }
                    catch (IOException)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }

            return tamano;
        }

        private static string ValidarNombre(string nombre)
        {
            var nombreLimpio = nombre?.Trim() ?? string.Empty;

            if (nombreLimpio.Length == 0)
                throw new ArgumentException("El nombre no puede estar vacío.");

            if (nombreLimpio.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                throw new ArgumentException("El nombre contiene caracteres no válidos.");

            return nombreLimpio;
        }

        private static void VerificarNoExiste(string ruta, string nombre)
        {
            if (Directory.Exists(ruta) || File.Exists(ruta))
                throw new IOException($"Ya existe un elemento llamado '{nombre.Trim()}'.");
        }
    }
}
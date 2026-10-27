using System.Collections.Generic;

namespace ExploradorArchivos.Core.Estructuras
{
    /// <summary>
    /// Gestiona una cola de tareas asíncronas para operaciones de archivos (como copias masivas o movimientos)
    /// utilizando la estructura de datos Cola (Queue). El funcionamiento es FIFO (First In, First Out / 
    /// Primero en entrar, primero en salir).
    /// </summary>
    public class ColaTareas<T>
    {
        // Cola interna que almacena las tareas pendientes de procesar
        private Queue<T> ColaElementos;

        /// <summary>
        /// Constructor para inicializar la cola de tareas.
        /// </summary>
        public ColaTareas()
        {
            ColaElementos = new Queue<T>();
        }

        /// <summary>
        /// Agrega una nueva tarea al final de la cola (Enqueue).
        /// </summary>
        public void Encolar(T tarea)
        {
            ColaElementos.Enqueue(tarea);
        }

        /// <summary>
        /// Extrae y retorna la siguiente tarea al frente de la cola para ser procesada (Dequeue).
        /// </summary>
        public T Desencolar()
        {
            if (TieneTareas())
            {
                return ColaElementos.Dequeue();
            }
            return default(T);
        }

        /// <summary>
        /// Retorna la siguiente tarea al frente sin retirarla de la cola (Peek).
        /// </summary>
        public T VerSiguiente()
        {
            if (TieneTareas())
            {
                return ColaElementos.Peek();
            }
            return default(T);
        }

        /// <summary>
        /// Verifica si la cola contiene tareas pendientes.
        /// </summary>
        public bool TieneTareas()
        {
            return ColaElementos.Count > 0;
        }

        /// <summary>
        /// Retorna la cantidad actual de tareas en espera.
        /// </summary>
        public int ObtenerCantidadTareas()
        {
            return ColaElementos.Count;
        }

        /// <summary>
        /// Limpia todas las tareas pendientes de la cola.
        /// </summary>
        public void VaciarCola()
        {
            ColaElementos.Clear();
        }
    }
}
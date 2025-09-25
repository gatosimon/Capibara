using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer
{
    /// <summary>
    /// Objeto encargado de administrar el logeo de eventos
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Path de los logs
        /// </summary>
        static string PathLogs = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Logs\";

        /// <summary>
        /// Escribe un mensaje de evento en el archivo de log
        /// </summary>
        /// <param name="mensaje"></param>
        static public void Mensaje(string mensaje)
        {
            if (_LogHabilitado)
            {
                System.IO.StreamWriter archivo = new System.IO.StreamWriter(PathArchivoActual, true);
                archivo.WriteLine(System.DateTime.Now.ToString("HH:mm:ss") + "   " + mensaje);
                archivo.Close();
            }
        }

        /// <summary>
        /// Escribe un mensaje de error en el archivo de log
        /// </summary>
        /// <param name="err">error a logear</param>
        static public void Error(Exception err)
        {
            if (_LogHabilitado)
            {
                System.IO.StreamWriter archivo = new System.IO.StreamWriter(PathArchivoActual, true);
                archivo.WriteLine(new string('*', err.Message.Length + 11));
                archivo.WriteLine(System.DateTime.Now.ToString("HH:mm:ss") + "   " + err.Message);
                archivo.WriteLine(new string('*', err.Message.Length + 11));
                archivo.Close();
            }
        }

        /// <summary>
        /// Escribe una separacion en el archivo de log
        /// </summary>
        static public void Separar()
        {
            if (_LogHabilitado)
            {
                System.IO.StreamWriter archivo = new System.IO.StreamWriter(PathArchivoActual, true);
                archivo.WriteLine(" ");
                archivo.Close();
            }
        }

        /// <summary>
        /// Escribe un mensaje de actividad realizada en el archivo de log
        /// </summary>
        static public void Ok()
        {
            if (_LogHabilitado)
            {
                System.IO.StreamWriter archivo = new System.IO.StreamWriter(PathArchivoActual, true);
                archivo.WriteLine(System.DateTime.Now.ToString("HH:mm:ss") + "   >OK");
                archivo.Close();
            }
        }

        /// <summary>
        /// Establece si el logeo se encuentra habilitado o no
        /// </summary>
        static private bool _LogHabilitado = false;
        /// <summary>
        /// Establece si el logeo se encuentra habilitado o no
        /// </summary>
        static public bool LogHabilitado
        {
            get { return _LogHabilitado; }
            set { _LogHabilitado = value; }
        }

        /// <summary>
        /// Path del archivo de log actual
        /// </summary>
        static private string PathArchivoActual
        {
            get
            {
                return PathLogs + DateTime.Now.ToString("yyyy_MM_dd") + ".txt";
            }
        }
    }
}

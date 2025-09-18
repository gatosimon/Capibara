using System;
using System.Diagnostics;
using System.IO;

namespace Capibara
{
    public class TfsHelper
    {
        private readonly string _workspacePath;

        public TfsHelper(string workspacePath)
        {
            _workspacePath = workspacePath;
        }

        /// <summary>
        /// Agrega un archivo o carpeta a TFS si no está ya bajo control.
        /// - Si es carpeta nueva → la agrega completa (/recursive).
        /// - Si la carpeta ya está en TFS → revisa sus archivos internos y agrega los faltantes.
        /// - Si es archivo → lo agrega solo si no está en TFS.
        /// </summary>
        public void AddIfNotInTfs(string ruta)
        {
            if (File.Exists(ruta))
            {
                // Es archivo → procesar individual
                AddFileIfNotInTfs(ruta);
            }
            else if (Directory.Exists(ruta))
            {
                // Primero reviso si la carpeta ya está en TFS
                string salida = EjecutarTf($"info \"{ruta}\"");

                if (salida.IndexOf("No items match", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    salida.IndexOf("No se encontró ningún elemento", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // Carpeta nueva → agregarla completa
                    EjecutarTf($"add \"{ruta}\" /recursive");
                    Console.WriteLine("Carpeta agregada completa a TFS: " + ruta);
                }
                else
                {
                    // Carpeta ya está en TFS → revisar archivos internos
                    foreach (string archivo in Directory.GetFiles(ruta, "*.*", SearchOption.AllDirectories))
                    {
                        AddFileIfNotInTfs(archivo);
                    }
                }
            }
        }

        private void AddFileIfNotInTfs(string archivo)
        {
            string salida = EjecutarTf($"info \"{archivo}\"");

            if (salida.IndexOf("No items match", StringComparison.OrdinalIgnoreCase) >= 0 ||
                salida.IndexOf("No se encontró ningún elemento", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                EjecutarTf($"add \"{archivo}\"");
                Console.WriteLine("Archivo agregado a TFS: " + archivo);
            }
            else
            {
                Console.WriteLine("Archivo ya en TFS: " + archivo);
            }
        }

        private string EjecutarTf(string argumentos)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "tf.exe",
                    Arguments = argumentos,
                    WorkingDirectory = _workspacePath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (var proceso = Process.Start(psi))
                {
                    string salida = proceso.StandardOutput.ReadToEnd();
                    string error = proceso.StandardError.ReadToEnd();
                    proceso.WaitForExit();

                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine("⚠️ TF Error: " + error);
                    }

                    return salida;
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                return err.Message;
            }
        }
    }
}
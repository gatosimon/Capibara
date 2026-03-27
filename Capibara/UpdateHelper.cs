using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Capibara
{
    public static class UpdateHelper
    {
        private const string UpdaterFileName = "AutoUpdater.exe";
        private const string AppFolder = "Capibara";

        private static string GetUpdaterDir()
        {
            string dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AppFolder
            );
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        private static string FindEmbeddedResourceName(Assembly assembly)
        {
            string[] names = assembly.GetManifestResourceNames();

            LogToFile("Recursos embebidos encontrados en el assembly (" + names.Length + "):");
            foreach (string n in names)
                LogToFile("  -> " + n);

            foreach (string n in names)
            {
                if (n.EndsWith(UpdaterFileName, StringComparison.OrdinalIgnoreCase))
                {
                    LogToFile("Recurso seleccionado: " + n);
                    return n;
                }
            }

            LogToFile("WARN: No se encontro ningun recurso embebido que termine en " + UpdaterFileName);
            return null;
        }

        private static bool EnsureAutoUpdaterExists(out string updaterPath)
        {
            string updaterDir = GetUpdaterDir();
            updaterPath = Path.Combine(updaterDir, UpdaterFileName);

            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = FindEmbeddedResourceName(assembly);

            if (resourceName == null)
            {
                bool exists = File.Exists(updaterPath);
                LogToFile(exists
                    ? "Recurso no embebido pero AutoUpdater.exe existe en disco: " + updaterPath
                    : "Recurso no embebido y AutoUpdater.exe NO existe. No se puede actualizar.");
                return exists;
            }

            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    LogToFile("ERROR: GetManifestResourceStream devolvio null para: " + resourceName);
                    return File.Exists(updaterPath);
                }

                bool needsExtract = true;
                if (File.Exists(updaterPath))
                {
                    long diskSize = new FileInfo(updaterPath).Length;
                    long resourceSize = resourceStream.Length;
                    needsExtract = diskSize != resourceSize;
                    LogToFile("AutoUpdater.exe en disco: " + diskSize + " bytes | " +
                              "Recurso embebido: " + resourceSize + " bytes | " +
                              "Necesita extraccion: " + needsExtract);
                }
                else
                {
                    LogToFile("AutoUpdater.exe no existe. Se extrae del recurso embebido en: " + updaterPath);
                }

                if (needsExtract)
                {
                    resourceStream.Seek(0, SeekOrigin.Begin);
                    try
                    {
                        using (var fs = new FileStream(updaterPath, FileMode.Create, FileAccess.Write))
                        {
                            byte[] buffer = new byte[81920];
                            int read;
                            while ((read = resourceStream.Read(buffer, 0, buffer.Length)) > 0)
                                fs.Write(buffer, 0, read);
                        }
                        LogToFile("AutoUpdater.exe extraido correctamente en: " + updaterPath);
                    }
                    catch (Exception ex)
                    {
                        LogToFile("ERROR al extraer AutoUpdater.exe: " + ex.Message);
                        return false;
                    }
                }
            }

            return File.Exists(updaterPath);
        }

        public static void CheckForUpdates(string manifestUrl, bool silent = true)
        {
            try
            {
                string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                LogToFile("=== CheckForUpdates iniciado ===");
                LogToFile("AppDir: " + appDir);
                LogToFile("ManifestUrl: " + manifestUrl);

                string updaterPath;
                if (!EnsureAutoUpdaterExists(out updaterPath))
                {
                    LogToFile("No se pudo obtener AutoUpdater.exe. Se omite la verificacion.");
                    return;
                }

                string appName = Assembly.GetExecutingAssembly().GetName().Name;
                int pid = Process.GetCurrentProcess().Id;

                string args =
                    "--app-name \"" + appName + "\" " +
                    "--manifest-url \"" + manifestUrl + "\" " +
                    "--app-dir \"" + appDir + "\" " +
                    "--pid " + pid +
                    (silent ? " --silent" : "");

                LogToFile("Lanzando: " + updaterPath);
                LogToFile("Args: " + args);

                Process.Start(new ProcessStartInfo
                {
                    FileName = updaterPath,
                    Arguments = args,
                    // UseShellExecute = true es OBLIGATORIO para que Windows
                    // procese el manifiesto UAC y muestre el dialogo de elevacion.
                    // Con false el proceso hereda el token del padre sin elevar.
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                LogToFile("ERROR en CheckForUpdates: " + ex.Message);
            }
        }

        private static readonly object _logLock = new object();

        private static void LogToFile(string message)
        {
            try
            {
                string logPath = Path.Combine(GetUpdaterDir(), "updatehelper.log");
                string line = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + message;
                lock (_logLock)
                    File.AppendAllText(logPath, line + Environment.NewLine);
            }
            catch { }
        }
    }
}


//using System;
//using System.Diagnostics;
//using System.IO;
//using System.Reflection;

//// INSTRUCCIÓN: cambiar el namespace por el de la aplicación destino.
//namespace Capibara
//{
//    /// <summary>
//    /// Helper para lanzar AutoUpdater.exe desde cualquier aplicación .NET 4.5
//    /// (WPF o WinForms).
//    ///
//    /// AutoUpdater.exe está embebido como recurso en la app. Si no existe en el
//    /// directorio de la app, se extrae automáticamente antes de ejecutarlo.
//    /// Esto permite distribuir la app sin incluir AutoUpdater.exe por separado
//    /// en el instalador.
//    /// </summary>
//    public static class UpdateHelper
//    {
//        // Debe coincidir con el nombre del assembly de la app cliente
//        // y con el nombre del archivo embebido: {RootNamespace}.AutoUpdater.exe
//        private const string UpdaterFileName = "AutoUpdater.exe";
//        private const string EmbeddedResPrefix = ""; // se completa en runtime con el namespace raíz

//        /// <summary>
//        /// Verifica que AutoUpdater.exe exista en el directorio de la app.
//        /// Si no existe o está desactualizado respecto al recurso embebido,
//        /// lo extrae del recurso antes de continuar.
//        /// </summary>
//        private static bool EnsureAutoUpdaterExists(string appDir, out string updaterPath)
//        {
//            updaterPath = Path.Combine(appDir, UpdaterFileName);

//            // Obtener el nombre completo del recurso embebido
//            // Formato: {RootNamespace}.AutoUpdater.exe
//            Assembly assembly = Assembly.GetExecutingAssembly();
//            string rootNamespace = assembly.GetName().Name;
//            string resourceName = rootNamespace + "." + UpdaterFileName;

//            // Verificar que el recurso embebido existe en este assembly
//            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
//            {
//                if (resourceStream == null)
//                {
//                    // El recurso no está embebido: intentar usar el archivo si ya existe en disco
//                    return File.Exists(updaterPath);
//                }

//                // Comparar tamaños para detectar si el embebido es más nuevo
//                // (evita reescribir en cada ejecución si ya está actualizado)
//                bool needsExtract = true;
//                if (File.Exists(updaterPath))
//                {
//                    long diskSize = new FileInfo(updaterPath).Length;
//                    long resourceSize = resourceStream.Length;
//                    needsExtract = diskSize != resourceSize;
//                }

//                if (needsExtract)
//                {
//                    // Reposicionar el stream si ya fue leído para comparar tamaño
//                    resourceStream.Seek(0, SeekOrigin.Begin);

//                    using (var fs = new FileStream(updaterPath, FileMode.Create, FileAccess.Write))
//                    {
//                        byte[] buffer = new byte[81920]; // 80 KB chunks
//                        int read;
//                        while ((read = resourceStream.Read(buffer, 0, buffer.Length)) > 0)
//                            fs.Write(buffer, 0, read);
//                    }
//                }
//            }

//            return File.Exists(updaterPath);
//        }

//        /// <summary>
//        /// Extrae AutoUpdater.exe si es necesario y lo lanza para verificar
//        /// si hay una nueva versión disponible.
//        /// </summary>
//        /// <param name="manifestUrl">
//        ///   URL pública del archivo version.xml.
//        ///   Usar la URL /releases/latest/download/version.xml para apuntar
//        ///   siempre al último release sin cambiar el código en cada versión.
//        ///   Ejemplo: https://github.com/usuario/repo/releases/latest/download/version.xml
//        /// </param>
//        /// <param name="silent">
//        ///   true  = sin ventana si ya está actualizado (recomendado en startup).
//        ///   false = muestra mensaje "ya está actualizado" (para menú Buscar actualizaciones).
//        /// </param>
//        public static void CheckForUpdates(string manifestUrl, bool silent = true)
//        {
//            try
//            {
//                string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

//                string updaterPath;
//                if (!EnsureAutoUpdaterExists(appDir, out updaterPath))
//                    return; // No se pudo obtener el ejecutable, no interrumpir la app

//                string appName = Assembly.GetExecutingAssembly().GetName().Name;
//                int pid = Process.GetCurrentProcess().Id;

//                string args =
//                    "--app-name \"" + appName + "\" " +
//                    "--manifest-url \"" + manifestUrl + "\" " +
//                    "--app-dir \"" + appDir + "\" " +
//                    "--pid " + pid +
//                    (silent ? " --silent" : "");

//                Process.Start(new ProcessStartInfo
//                {
//                    FileName = updaterPath,
//                    Arguments = args,
//                    UseShellExecute = false
//                });
//            }
//            catch
//            {
//                // No interrumpir la app si el updater falla al lanzarse
//            }
//        }
//    }
//}
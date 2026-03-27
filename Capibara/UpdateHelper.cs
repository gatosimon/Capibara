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
                    UseShellExecute = true,
                    Verb = "runas" // Esto dispara el cartel de "Desea permitir que esta aplicación..."
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
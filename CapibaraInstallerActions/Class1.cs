using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace CapibaraInstallerActions
{
    [RunInstaller(true)]
    public class PostInstallAction : Installer
    {
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);

            try
            {
                // El instalador pasa "targetdir" como parámetro
                string targetPath = Context.Parameters["targetdir"];
                if (string.IsNullOrEmpty(targetPath))
                {
                    MessageBox.Show("No se pudo determinar la carpeta de instalación.");
                    return;
                }

                string exePath = Path.Combine(targetPath, "Capibara.exe");
                if (File.Exists(exePath))
                {
                    Process.Start(exePath);
                }
                else
                {
                    MessageBox.Show("No se encontró el ejecutable: " + exePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al ejecutar la aplicación: " + ex.Message);
            }
        }
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            string targetPath = Context.Parameters["targetdir"];
            if (!string.IsNullOrEmpty(targetPath))
            {
                // Aquí ya tienes la ruta de instalación correcta
                System.IO.File.WriteAllText(System.IO.Path.Combine(targetPath, "prueba.txt"), "Hola Mundo");
            }
            else
            {
                throw new Exception("No se pudo obtener la ruta de instalación.");
            }
        }
    }
}

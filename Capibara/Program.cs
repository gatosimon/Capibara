using System;
using System.Windows.Forms;

namespace Capibara
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Si CheckForUpdates devuelve false, se aplicó una actualización
            // y la versión nueva ya fue relanzada. No continuar con esta instancia.
            if (!UpdateHelper.CheckForUpdates(
                    "https://github.com/gatosimon/CapibaraUpdates/releases/latest/download/version.xml"))
                return;

            Application.Run(new FRMcapibara());
        }
    }
}

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
            UpdateHelper.CheckForUpdates("https://github.com/gatosimon/CapibaraUpdates/releases/download/v1.0.0.0/version.xml");
            Application.Run(new FRMcapibara());
        }
    }
}

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WMPLib;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;

namespace Capibara
{
    public static class Utilidades
    {
        public static DataGridViewRow BuscarFila(DataGridView grilla, string valor)
        {
            return grilla.Rows.Cast<DataGridViewRow>().FirstOrDefault(r => r.Cells[0].Value != null &&
                         r.Cells[0].Value.ToString() == valor);
        }

        /// <summary>
        /// Convierte un texto a formato "Titulo":
        /// - Separa palabras pegadas cuando hay mayúsculas en el medio.
        /// - Capitaliza la primera letra de cada palabra.
        /// </summary>
        public static string FormatearTitulo(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // 1) Insertar espacio antes de mayúsculas internas (ej: "holaMundo" → "hola Mundo")
            string separado = Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");

            // 2) Poner en minúsculas todo, para evitar cosas raras
            separado = separado.ToLowerInvariant();

            // 3) Usar TextInfo para capitalizar cada palabra
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            string resultado = ti.ToTitleCase(separado);

            return resultado;
        }

        private const string CAPIBARA = "Capibara";
        private const string CAPIBARAR = "Capibarar";
        private static WindowsMediaPlayer player;
        private static FRMcapibara formularioActual = null;


        public static bool HayDispositivoDeAudio()
        {
            var enumerator = new MMDeviceEnumerator();
            var dispositivo = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            return dispositivo != null && dispositivo.State == DeviceState.Active;
        }

        private static void ReproducirMusica(string nombreMP3, byte[] recurso, FRMcapibara form)
        {
            if (HayDispositivoDeAudio())
            {
                if (player == null)
                {
                    player = new WindowsMediaPlayer();
                }
                string pathMp3 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nombreMP3 + ".mp3");
                if (!File.Exists(pathMp3))
                {
                    File.WriteAllBytes(pathMp3, recurso);
                }
                if (formularioActual == null)
                {
                    formularioActual = form;
                }
                player.settings.volume = 10;
                player.URL = pathMp3;
                player.settings.autoStart = true;
                player.PlayStateChange += Player_PlayStateChange;
                player.controls.play();
            }
        }

        private static void Player_PlayStateChange(int NewState)
        {
            // 8 = MediaEnded
            if ((WMPPlayState)NewState == WMPPlayState.wmppsMediaEnded)
            {
                formularioActual.Invoke((Action)(() =>
                {
                    // 🔹 cerrar overlay
                    if (formularioActual.overlay != null && !formularioActual.overlay.IsDisposed)
                    {
                        formularioActual.overlay.Close();
                        formularioActual.overlay.Dispose();
                    }
                }));
                if (player != null)
                {
                    player.controls.stop();              // detiene el audio/video
                    Marshal.ReleaseComObject(player);    // libera la referencia COM
                    player = null;                       // limpia la referencia
                    GC.Collect();                        // fuerza GC (opcional)
                    GC.WaitForPendingFinalizers();       // espera finalizadores
                }
            }
        }

        public static void ReproducirIntro(FRMcapibara form)
        {
            ReproducirMusica(CAPIBARA, Properties.Resources.CapibaraCorto, form);
        }

        public static void ReproducirSplash(FRMcapibara form)
        {
            ReproducirMusica(CAPIBARAR, Properties.Resources.Capibarar, form);
        }
    }
}
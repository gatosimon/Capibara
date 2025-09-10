using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WMPLib;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

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
        static MMDeviceEnumerator enumerator;
        static AudioNotificationClient notificationClient;

        public static void IniciarDeteccionDispositivos(FRMcapibara form)
        {
            if (formularioActual == null)
            {
                formularioActual = form;
            }

            enumerator = new MMDeviceEnumerator();
            notificationClient = new AudioNotificationClient();

            // enganchar al evento
            notificationClient.DispositivoCambiado += (deviceId, state) =>
            {
                if (state == DeviceState.NotPresent || state == DeviceState.Unplugged || state == DeviceState.Disabled)
                {
                    // 🔹 el dispositivo ya no está disponible
                    //formularioActual.Invoke((Action)(() =>
                    //{
                    //    // cerrar overlay si está abierto
                    //    if (formularioActual.overlay != null && !formularioActual.overlay.IsDisposed)
                    //    {
                    //        formularioActual.overlay.Close();
                    //        formularioActual.overlay.Dispose();
                    //    }

                    //    // detener player si sigue activo
                    //    if (player != null)
                    //    {
                    //        player.controls.stop();              // detiene el audio/video
                    //        Marshal.ReleaseComObject(player);    // libera la referencia COM
                    //        player = null;                       // limpia la referencia
                    //        GC.Collect();                        // fuerza GC (opcional)
                    //        GC.WaitForPendingFinalizers();       // espera finalizadores
                    //    }
                    //}));
                    DisposePlayer();
                }
            };

            // registrar el callback
            enumerator.RegisterEndpointNotificationCallback(notificationClient);
        }

        public static void DesRegistrar()
        {
            if (enumerator != null && notificationClient != null)
            {
                enumerator.UnregisterEndpointNotificationCallback(notificationClient);
            }
        }

        public static bool HayDispositivoDeAudio()
        {
            try
            {
                enumerator = new MMDeviceEnumerator();
                var dispositivos = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
                return dispositivos != null && dispositivos.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        private static void ReproducirMusica(string nombreMP3, byte[] recurso)
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
                DisposePlayer();
            }
        }

        private static void DisposePlayer()
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

        public static void ReproducirIntro()
        {
            ReproducirMusica(CAPIBARA, Properties.Resources.CapibaraCorto);
        }

        public static void ReproducirSplash()
        {
            ReproducirMusica(CAPIBARAR, Properties.Resources.Capibarar);
        }
    }
    public class AudioNotificationClient : IMMNotificationClient
    {
        public event Action<string, DeviceState> DispositivoCambiado;

        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            DispositivoCambiado?.Invoke(deviceId, newState);
        }

        public void OnDeviceAdded(string pwstrDeviceId) { }
        public void OnDeviceRemoved(string deviceId) { }
        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId) { }
        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) { }
    }
}
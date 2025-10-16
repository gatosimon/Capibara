using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows.Forms;
using WMPLib;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System.Collections.Generic;

namespace Capibara
{
    public static class Utilidades
    {
        private const string CAPIBARA = "Capibara";
        private const string CAPIBARAR = "Capibarar";
        private static WindowsMediaPlayer player;
        private static FRMcapibara formularioActual = null;
        static MMDeviceEnumerator enumerator;
        static AudioNotificationClient notificationClient;
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

        //public static string LimpiarYPascalCase(string input)
        //{
        //    if (string.IsNullOrWhiteSpace(input) || string.IsNullOrEmpty(input.Trim()))
        //        return string.Empty;

        //    // Reemplazar cualquier caracter que no sea letra o número por un espacio
        //    string limpio = Regex.Replace(input, @"[^a-zA-Z0-9]+", " ");

        //    // Dividir en palabras por los espacios
        //    List<string> palabras = limpio.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        //    StringBuilder resultado = new StringBuilder();

        //    foreach (string palabra in palabras)
        //    {
        //        if (palabra.Length == 0) continue;

        //        StringBuilder palabraFormateada = new StringBuilder();

        //        int i = 0;

        //        // Primera letra siempre mayúscula
        //        if (palabras.IndexOf(palabra) == 0)
        //        {
        //            palabraFormateada.Append(char.ToUpper(palabra[0]));
        //            i++;
        //            while (char.IsUpper(palabra[i]))
        //            {
        //                palabraFormateada.Append(char.ToLower(palabra[i]));
        //                i++;
        //            }
        //        }
        //        else
        //        {
        //            palabraFormateada.Append(char.ToUpper(palabra[i]));
        //        }

        //        while (i < palabra.Length)
        //        {
        //            // Detectar secuencia de mayúsculas consecutivas
        //            int j = i;
        //            while (j < palabra.Length && char.IsUpper(palabra[j]))
        //                j++;

        //            int secuenciaLength = j - i;

        //            if (secuenciaLength > 1)
        //            {
        //                // Primera letra de la secuencia en mayúscula, el resto en minúscula
        //                palabraFormateada.Append(char.ToUpper(palabra[i]));
        //                palabraFormateada.Append(palabra.Substring(i + 1, secuenciaLength - 1).ToLower());
        //            }
        //            else if (secuenciaLength == 1)
        //            {
        //                // Una sola mayúscula interna: mantenerla
        //                palabraFormateada.Append(palabra[i]);
        //            }

        //            i += secuenciaLength;

        //            if (secuenciaLength == 0)
        //            {
        //                // Es minúscula, simplemente agregarla
        //                palabraFormateada.Append(palabra[i]);
        //                i++;
        //            }
        //        }

        //        resultado.Append(palabraFormateada);
        //    }

        //    return resultado.ToString();
        //}

        /// <summary>
        /// Formatea una cadena de caracteres con reglas configurables.
        /// </summary>
        /// <param name="input">La cadena de caracteres de entrada.</param>
        /// <param name="respetarMayusculasInternas">
        /// Si es true, mantiene mayúsculas que no sean consecutivas (ej: 'ProductoID' -> 'ProductoId').
        /// Si es false, convierte a minúsculas todos los caracteres excepto el primero de cada palabra (ej: 'ProductoID' -> 'Productoid').
        /// </param>
        /// <returns>La cadena formateada.</returns>
        public static string FormatearCadena(string input, bool respetarMayusculasInternas = true)
        {
            // 1. Manejar casos de entrada nula o vacía.
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            // 2. Dividir la cadena en palabras.
            string[] palabras = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder resultadoFinal = new StringBuilder();

            // 3. Procesar cada palabra.
            foreach (string palabra in palabras)
            {
                StringBuilder palabraLimpia = new StringBuilder();
                foreach (char c in palabra)
                {
                    if (char.IsLetterOrDigit(c))
                    {
                        palabraLimpia.Append(c);
                    }
                }

                if (palabraLimpia.Length > 0)
                {
                    // La primera letra de la palabra SIEMPRE va en mayúscula.
                    resultadoFinal.Append(char.ToUpper(palabraLimpia[0]));

                    for (int i = 1; i < palabraLimpia.Length; i++)
                    {
                        char caracterActual = palabraLimpia[i];

                        // 4. Lógica condicional basada en el nuevo parámetro booleano.
                        if (respetarMayusculasInternas)
                        {
                            // MODO: Respetar mayúsculas (PascalCase)
                            char caracterAnterior = resultadoFinal[resultadoFinal.Length - 1];
                            if (char.IsUpper(caracterAnterior))
                            {
                                resultadoFinal.Append(char.ToLower(caracterActual));
                            }
                            else
                            {
                                resultadoFinal.Append(caracterActual);
                            }
                        }
                        else
                        {
                            // MODO: Solo la primera mayúscula por palabra
                            resultadoFinal.Append(char.ToLower(caracterActual));
                        }
                    }
                }
            }

            return resultadoFinal.ToString();
        }

        public static bool EsCarpeta(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            return (attr & FileAttributes.Directory) == FileAttributes.Directory;
        }
        
        public static string ObtenerClaseYEntidad(string rutaArchivo)
        {
            if (!File.Exists(rutaArchivo))
                throw new FileNotFoundException("No se encontró el archivo especificado", rutaArchivo);

            string nombreClase = null;
            string nombrePropiedad = null;

            foreach (var linea in File.ReadLines(rutaArchivo))
            {
                string lineaTrim = linea.Trim();

                // Buscar clase
                if (nombreClase == null && (lineaTrim.StartsWith("public static class") || lineaTrim.StartsWith("public class")))
                {
                    var partes = lineaTrim.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    nombreClase = partes.Last(); // último token es el nombre de la clase
                    continue;
                }

                // Buscar propiedad
                if (nombrePropiedad == null && lineaTrim.StartsWith("public static"))
                {
                    var partes = lineaTrim.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    // ejemplo: public static apremiosEntities ApremiosEntidades
                    // el penúltimo token es el tipo, el último token es el nombre de la propiedad
                    nombrePropiedad = partes[partes.Length - 1];
                    continue;
                }
            }

            if (nombreClase != null && nombrePropiedad != null)
                return nombreClase + "." + nombrePropiedad;

            return null;
        }

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
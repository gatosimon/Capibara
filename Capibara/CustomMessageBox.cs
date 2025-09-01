using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WMPLib;
using System.Runtime.InteropServices;

namespace Capibara.Controles
{
    public partial class CustomMessageBox : Form
    {
        public const string ATENCION = "ATENCIÓN!!!";
        public const string ERROR = "ERROR!!!";
        public const string INFORMACION = "INFORMACIÓN";
        public const string GUATEFAK = "GUATEFAK?!?!";

        WindowsMediaPlayer player;
        // Constructor privado, solo usado por Show
        private CustomMessageBox(string text, string caption,
                                MessageBoxButtons buttons,
                                MessageBoxIcon icon,
                                MessageBoxDefaultButton defaultButton,
                                Font customFont)
        {
            InitializeComponent();

            // Configuración inicial
            this.Text = caption;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // Texto
            LBLmensage.Text = text;
            LBLmensage.Font = customFont ?? new Font("Verdana", 10);

            // Icono
            PCBicono.Image = GetIconBitmap(icon);
            this.Icon = GetIcon(icon);

            // Botones
            AddButtons(buttons, defaultButton);
        }

        private void AddButtons(MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        {
            void add(string text, DialogResult result, bool isDefault = false)
            {
                var btn = new Button();
                btn.Text = text;
                btn.DialogResult = result;
                btn.AutoSize = true;
                btn.Anchor = AnchorStyles.None;
                btn.Height = 35;
                btn.Margin = new Padding(5);
                FLPbotones.Controls.Add(btn);

                if (isDefault)
                    this.AcceptButton = btn;
            }

            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    add("OK", DialogResult.OK, defaultButton == MessageBoxDefaultButton.Button1);
                    break;
                case MessageBoxButtons.OKCancel:
                    add("Cancelar", DialogResult.Cancel, defaultButton == MessageBoxDefaultButton.Button2);
                    add("OK", DialogResult.OK, defaultButton == MessageBoxDefaultButton.Button1);
                    break;
                case MessageBoxButtons.YesNo:
                    add("No", DialogResult.No, defaultButton == MessageBoxDefaultButton.Button2);
                    add("Sí", DialogResult.Yes, defaultButton == MessageBoxDefaultButton.Button1);
                    break;
                case MessageBoxButtons.YesNoCancel:
                    add("Cancelar", DialogResult.Cancel, defaultButton == MessageBoxDefaultButton.Button3);
                    add("No", DialogResult.No, defaultButton == MessageBoxDefaultButton.Button2);
                    add("Sí", DialogResult.Yes, defaultButton == MessageBoxDefaultButton.Button1);
                    break;
                case MessageBoxButtons.RetryCancel:
                    add("Cancelar", DialogResult.Cancel, defaultButton == MessageBoxDefaultButton.Button2);
                    add("Reintentar", DialogResult.Retry, defaultButton == MessageBoxDefaultButton.Button1);
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    add("Ignorar", DialogResult.Ignore, defaultButton == MessageBoxDefaultButton.Button3);
                    add("Reintentar", DialogResult.Retry, defaultButton == MessageBoxDefaultButton.Button2);
                    add("Abortar", DialogResult.Abort, defaultButton == MessageBoxDefaultButton.Button1);
                    break;
            }
        }
        private static Icon GetIcon(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Error: return Capibara.Properties.Resources.Error;
                case MessageBoxIcon.Warning: return Capibara.Properties.Resources.Advertencia;
                case MessageBoxIcon.Information: return Capibara.Properties.Resources.Info;
                case MessageBoxIcon.Question: return Capibara.Properties.Resources.Pregunta;
                default: return Capibara.Properties.Resources.Capibara;
            }
        }

        private static Bitmap GetIconBitmap(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Error: return Capibara.Properties.Resources.CapiError;
                case MessageBoxIcon.Warning: return Capibara.Properties.Resources.CapiAdvertencia;
                case MessageBoxIcon.Information: return Capibara.Properties.Resources.CapiInfo;
                case MessageBoxIcon.Question: return Capibara.Properties.Resources.CapiPregunta;
                default: return Capibara.Properties.Resources.Capibara64x64;
            }
        }

        // 🔥 Sobrecargas de Show igual que MessageBox 🔥
        public static DialogResult Show(string text) =>
            Show(text, "", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, null);

        public static DialogResult Show(string text, string caption) =>
            Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, null);

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons) =>
            Show(text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, null);

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) =>
            Show(text, caption, buttons, icon, MessageBoxDefaultButton.Button1, null);

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) =>
            Show(text, caption, buttons, icon, defaultButton, null);

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, Font customFont)
        {
            using (var box = new CustomMessageBox(text, caption, buttons, icon, defaultButton, customFont))
            {
                return box.ShowDialog();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ReproducirMusica("Capibarar", Properties.Resources.Capibarar);
        }

        private void ReproducirMusica(string nombreMP3, byte[] recurso)
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
            player.settings.volume = 15;
            player.URL = pathMp3;
            player.settings.autoStart = true;
            player.PlayStateChange += Player_PlayStateChange;
            player.controls.play();
        }
        private void Player_PlayStateChange(int NewState)
        {
            // 8 = MediaEnded
            if ((WMPPlayState)NewState == WMPPlayState.wmppsMediaEnded)
            {
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
    }
}

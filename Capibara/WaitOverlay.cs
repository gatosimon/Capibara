using System;
using System.Drawing;
using System.Windows.Forms;

namespace Capibara.CustomControls
{
    public partial class WaitOverlay : Form
    {
        private PictureBox pic;
        private Timer timer;
        private Image[] frames;
        public int currentFrame = 0;

        public WaitOverlay(Form parent)
        {
            // Timer para cambiar la imagen
            timer = new Timer();
            timer.Interval = 200; // cada 200 ms (~5 FPS)
            timer.Tick += (s, e) => NextFrame();
            timer.Start();

            pic = new PictureBox
            {
                Image = Properties.Resources.Capibara00,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Width = 750,
                Height = 750
            };
            Controls.Add(pic);

            // Centrar
            CenterSpinner();
            Resize += (s, e) => CenterSpinner();

            // Frames de la animación (pueden venir de Resources o archivos)
            frames = new Image[]
            {
                Properties.Resources.Capibara04,
                Properties.Resources.Capibara00,
            };
            
            // Overlay transparente
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = false;
            BackColor = Color.White;
            Opacity = 0.4;
            Bounds = parent.Bounds;
            TopMost = true;

            //Asegurar que se "pegue" al formulario padre
            parent.LocationChanged += (s, e) => this.Bounds = parent.Bounds;
            parent.SizeChanged += (s, e) => this.Bounds = parent.Bounds;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            timer.Stop();
            base.OnFormClosed(e);
        }

        private void CenterSpinner()
        {
            if (pic != null)
                pic.Location = new Point(
                    (ClientSize.Width - pic.Width) / 2,
                    (ClientSize.Height - pic.Height) / 2
                );
        }

        private void NextFrame()
        {
            if (frames.Length == 0) return;

            currentFrame = (currentFrame + 1) % frames.Length;
            pic.Image = frames[currentFrame];
        }
    }

}
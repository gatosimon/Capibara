using System;
using System.Drawing;
using System.Windows.Forms;

namespace Capibara.CustomControls
{
    public class SplitButton : Button
    {
        private ContextMenuStrip _menu;
        private const int arrowHeight = 15; // Tamaño de la zona de flecha ▼

        public ContextMenuStrip Menu
        {
            get { return _menu; }
            set { _menu = value; }
        }

        public bool AlwaysDropDown { get; set; }

        public SplitButton()
        {
            this.AutoSize = false;
            this.AlwaysDropDown = false;

            // Evita que Windows dibuje por defecto el texto/imagen
            this.SetStyle(ControlStyles.UserPaint, true);
            this.UseMnemonic = true; // 🔹 Muy importante
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;

            // --- Dibujar fondo y borde con estados ---
            ButtonState state = ButtonState.Normal;
            if (!this.Enabled) state = ButtonState.Inactive;
            else if (this.Capture && MouseButtons == MouseButtons.Left) state = ButtonState.Pushed;

            ControlPaint.DrawButton(g, this.ClientRectangle, state);

            // Línea de separación
            int yLine = this.Height - arrowHeight;
            g.DrawLine(Pens.Gray, 4, yLine, this.Width - 4, yLine);

            int spacing = 15;
            int currentY = 10;

            // --- Imagen arriba ---
            if (this.Image != null)
            {
                int imgX = (this.Width - this.Image.Width) / 2;
                g.DrawImage(this.Image, imgX, currentY, this.Image.Width, this.Image.Height);
                currentY += this.Image.Height + spacing;
            }

            // --- Texto debajo de la imagen ---
            if (!string.IsNullOrEmpty(this.Text))
            {
                Rectangle textRect = new Rectangle(
                    0,
                    currentY,
                    this.Width,
                    yLine - currentY
                );

                TextRenderer.DrawText(
                    g,
                    this.Text,
                    this.Font,
                    textRect,
                    this.ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding
                );
            }

            // --- Flecha ▼ ---
            Point[] arrow = new Point[]
            {
                new Point(this.Width/2 - 5, this.Height - arrowHeight/2 - 2),
                new Point(this.Width/2 + 5, this.Height - arrowHeight/2 - 2),
                new Point(this.Width/2, this.Height - 5)
            };
            g.FillPolygon(Brushes.Black, arrow);

            // --- Opcional: Focus rectangle cuando el botón tiene foco ---
            if (this.Focused)
            {
                ControlPaint.DrawFocusRectangle(g, new Rectangle(3, 3, this.Width - 6, this.Height - arrowHeight - 6));
            }
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);

            if (_menu != null && mevent.Button == MouseButtons.Left)
            {
                bool clickEnFlecha = mevent.Y >= this.Height - arrowHeight;

                if (AlwaysDropDown || clickEnFlecha)
                {
                    _menu.Show(this, new Point(0, this.Height));
                }
                else
                {
                    this.OnClick(EventArgs.Empty);
                }
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            // Permite que se procesen las teclas con Alt
            if ((keyData & Keys.Alt) == Keys.Alt)
                return true;

            return base.IsInputKey(keyData);
        }

        protected override bool ProcessMnemonic(char charCode)
        {
            if (UseMnemonic && IsMnemonic(charCode, this.Text) && this.Enabled && this.Visible)
            {
                // 🔹 En vez de Click, abrimos el menú
                ShowContextMenu();
                return true;
            }
            if (UseMnemonic && charCode == '\u001b' && this.Enabled && this.Visible && this.Menu.Visible)
            {
                this.Menu.Hide();
            }
            return base.ProcessMnemonic(charCode);
        }

        private void ShowContextMenu()
        {
            if (this.Menu != null)
            {
                var menuLocation = new Point(0, this.Height);
                this.Menu.Show(this, menuLocation);
            }
        }
    }
}

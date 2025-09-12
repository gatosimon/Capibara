using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Capibara.CustomControls
{
    public class BoldComboBox : ComboBox
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<string> BoldItems { get; } = new List<string>();

        [DefaultValue(0)]
        public int BoldLevel { get; set; } = 0;

        [DefaultValue(true)]
        public bool CaseInsensitiveBoldItems { get; set; } = true;

        // Nuevo: color para los ítems destacados
        [DefaultValue(typeof(Color), "Blue")]
        public Color BoldColor { get; set; } = Color.Blue;

        // Nuevo: usar color distinto para ítems destacados
        [DefaultValue(true)]
        public bool UseBoldColor { get; set; } = true;

        public BoldComboBox()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DrawItem += BoldComboBox_DrawItem;
        }

        private void BoldComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            string text = this.Items[e.Index]?.ToString() ?? string.Empty;

            e.DrawBackground();

            bool isBold = false;

            // Caso 1: lista manual
            if (CaseInsensitiveBoldItems)
                isBold = BoldItems.Any(x => string.Equals(x, text, StringComparison.OrdinalIgnoreCase));
            else
                isBold = BoldItems.Contains(text);

            // Caso 2: nivel jerárquico
            if (!isBold && BoldLevel > 0)
            {
                int segmentos = text.Split('.').Length;
                if (segmentos <= BoldLevel) isBold = true;
            }

            // Fuente
            Font fontToUse = isBold ? new Font(e.Font, FontStyle.Bold) : e.Font;

            // Color
            Color colorToUse = (isBold && UseBoldColor) ? BoldColor : e.ForeColor;

            try
            {
                using (Brush b = new SolidBrush(colorToUse))
                {
                    e.Graphics.DrawString(text, fontToUse, b, e.Bounds.Left, e.Bounds.Top + 1);
                }

                e.DrawFocusRectangle();
            }
            finally
            {
                if (!object.ReferenceEquals(fontToUse, e.Font))
                    fontToUse.Dispose();
            }
        }

        public void AddBold(string item)
        {
            if (string.IsNullOrEmpty(item)) return;

            bool exists = CaseInsensitiveBoldItems
                ? BoldItems.Any(x => string.Equals(x, item, StringComparison.OrdinalIgnoreCase))
                : BoldItems.Contains(item);

            if (!exists) BoldItems.Add(item);
        }

        public void RemoveBold(string item)
        {
            if (string.IsNullOrEmpty(item)) return;

            string found = CaseInsensitiveBoldItems
                ? BoldItems.FirstOrDefault(x => string.Equals(x, item, StringComparison.OrdinalIgnoreCase))
                : BoldItems.FirstOrDefault(x => x == item);

            if (found != null) BoldItems.Remove(found);
        }
    }
}

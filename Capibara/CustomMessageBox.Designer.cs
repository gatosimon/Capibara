
using System.Windows.Forms;

namespace Capibara.CustomControls
{
    partial class CustomMessageBox
    {
        private System.ComponentModel.IContainer components = null;
        private Label LBLmensage;
        private PictureBox PCBicono;
        private FlowLayoutPanel FLPbotones;
        private TableLayoutPanel TLPprincipal;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.TLPprincipal = new System.Windows.Forms.TableLayoutPanel();
            this.PCBicono = new System.Windows.Forms.PictureBox();
            this.LBLmensage = new System.Windows.Forms.Label();
            this.FLPbotones = new System.Windows.Forms.FlowLayoutPanel();
            this.TLPprincipal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PCBicono)).BeginInit();
            this.SuspendLayout();
            // 
            // TLPprincipal
            // 
            this.TLPprincipal.AutoSize = true;
            this.TLPprincipal.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TLPprincipal.ColumnCount = 2;
            this.TLPprincipal.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLPprincipal.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLPprincipal.Controls.Add(this.PCBicono, 0, 0);
            this.TLPprincipal.Controls.Add(this.LBLmensage, 1, 0);
            this.TLPprincipal.Controls.Add(this.FLPbotones, 0, 1);
            this.TLPprincipal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLPprincipal.Location = new System.Drawing.Point(0, 0);
            this.TLPprincipal.Name = "TLPprincipal";
            this.TLPprincipal.RowCount = 2;
            this.TLPprincipal.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLPprincipal.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLPprincipal.Size = new System.Drawing.Size(500, 177);
            this.TLPprincipal.TabIndex = 0;
            // 
            // PCBicono
            // 
            this.PCBicono.Dock = System.Windows.Forms.DockStyle.Top;
            this.PCBicono.Location = new System.Drawing.Point(10, 10);
            this.PCBicono.Margin = new System.Windows.Forms.Padding(10);
            this.PCBicono.Name = "PCBicono";
            this.PCBicono.Size = new System.Drawing.Size(92, 92);
            this.PCBicono.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PCBicono.TabIndex = 0;
            this.PCBicono.TabStop = false;
            // 
            // LBLmensage
            // 
            this.LBLmensage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LBLmensage.AutoEllipsis = true;
            this.LBLmensage.AutoSize = true;
            this.LBLmensage.Location = new System.Drawing.Point(115, 10);
            this.LBLmensage.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.LBLmensage.MaximumSize = new System.Drawing.Size(370, 0);
            this.LBLmensage.Name = "LBLmensage";
            this.LBLmensage.Size = new System.Drawing.Size(370, 13);
            this.LBLmensage.TabIndex = 1;
            this.LBLmensage.Text = "Mensaje";
            this.LBLmensage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FLPbotones
            // 
            this.FLPbotones.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.FLPbotones.AutoSize = true;
            this.FLPbotones.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TLPprincipal.SetColumnSpan(this.FLPbotones, 2);
            this.FLPbotones.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.FLPbotones.Location = new System.Drawing.Point(245, 164);
            this.FLPbotones.Name = "FLPbotones";
            this.FLPbotones.Padding = new System.Windows.Forms.Padding(5);
            this.FLPbotones.Size = new System.Drawing.Size(10, 10);
            this.FLPbotones.TabIndex = 2;
            this.FLPbotones.WrapContents = false;
            // 
            // CustomMessageBox
            // 
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(500, 177);
            this.Controls.Add(this.TLPprincipal);
            this.MinimumSize = new System.Drawing.Size(300, 150);
            this.Name = "CustomMessageBox";
            this.Text = "CustomMessageBox";
            this.TLPprincipal.ResumeLayout(false);
            this.TLPprincipal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PCBicono)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    }
}
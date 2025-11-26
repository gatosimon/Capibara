using System.ComponentModel;
using System.Windows.Forms;

namespace Capibara
{
    partial class FRMconexiones
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblMotor;
        private ComboBox cmbMotor;
        private Label lblServidor;
        private TextBox txtServidor;
        private Button btnBuscarBase;
        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblContrasena;
        private TextBox txtContrasena;
        private Label lblBaseDatos;
        private TextBox txtBaseDatos;
        private ComboBox cmbBaseDatos;
        private Button btnGuardar;
        private Button btnProbar;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// true if managed resources should be disposed; otherwise, false.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblMotor = new System.Windows.Forms.Label();
            this.cmbMotor = new System.Windows.Forms.ComboBox();
            this.lblServidor = new System.Windows.Forms.Label();
            this.txtServidor = new System.Windows.Forms.TextBox();
            this.btnBuscarBase = new System.Windows.Forms.Button();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.lblContrasena = new System.Windows.Forms.Label();
            this.txtContrasena = new System.Windows.Forms.TextBox();
            this.lblBaseDatos = new System.Windows.Forms.Label();
            this.txtBaseDatos = new System.Windows.Forms.TextBox();
            this.cmbBaseDatos = new System.Windows.Forms.ComboBox();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnProbar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(12, 15);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(63, 16);
            this.lblNombre.TabIndex = 0;
            this.lblNombre.Text = "Nombre:";
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(120, 12);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(426, 23);
            this.txtNombre.TabIndex = 1;
            // 
            // lblMotor
            // 
            this.lblMotor.AutoSize = true;
            this.lblMotor.Location = new System.Drawing.Point(12, 45);
            this.lblMotor.Name = "lblMotor";
            this.lblMotor.Size = new System.Drawing.Size(52, 16);
            this.lblMotor.TabIndex = 2;
            this.lblMotor.Text = "Motor:";
            // 
            // cmbMotor
            // 
            this.cmbMotor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMotor.FormattingEnabled = true;
            this.cmbMotor.Location = new System.Drawing.Point(120, 42);
            this.cmbMotor.Name = "cmbMotor";
            this.cmbMotor.Size = new System.Drawing.Size(426, 24);
            this.cmbMotor.TabIndex = 3;
            this.cmbMotor.SelectedIndexChanged += new System.EventHandler(this.cmbMotor_SelectedIndexChanged);
            // 
            // lblServidor
            // 
            this.lblServidor.AutoSize = true;
            this.lblServidor.Location = new System.Drawing.Point(12, 75);
            this.lblServidor.Name = "lblServidor";
            this.lblServidor.Size = new System.Drawing.Size(68, 16);
            this.lblServidor.TabIndex = 4;
            this.lblServidor.Text = "Servidor:";
            // 
            // txtServidor
            // 
            this.txtServidor.Location = new System.Drawing.Point(120, 72);
            this.txtServidor.Name = "txtServidor";
            this.txtServidor.Size = new System.Drawing.Size(426, 23);
            this.txtServidor.TabIndex = 5;
            // 
            // btnBuscarBase
            // 
            this.btnBuscarBase.Enabled = false;
            this.btnBuscarBase.Location = new System.Drawing.Point(517, 71);
            this.btnBuscarBase.Name = "btnBuscarBase";
            this.btnBuscarBase.Size = new System.Drawing.Size(29, 22);
            this.btnBuscarBase.TabIndex = 6;
            this.btnBuscarBase.Text = "...";
            this.btnBuscarBase.UseVisualStyleBackColor = true;
            this.btnBuscarBase.Visible = false;
            this.btnBuscarBase.Click += new System.EventHandler(this.btnBuscarBase_Click);
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Location = new System.Drawing.Point(12, 105);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(62, 16);
            this.lblUsuario.TabIndex = 7;
            this.lblUsuario.Text = "Usuario:";
            // 
            // txtUsuario
            // 
            this.txtUsuario.Location = new System.Drawing.Point(120, 102);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(426, 23);
            this.txtUsuario.TabIndex = 8;
            // 
            // lblContrasena
            // 
            this.lblContrasena.AutoSize = true;
            this.lblContrasena.Location = new System.Drawing.Point(12, 135);
            this.lblContrasena.Name = "lblContrasena";
            this.lblContrasena.Size = new System.Drawing.Size(89, 16);
            this.lblContrasena.TabIndex = 9;
            this.lblContrasena.Text = "Contraseña:";
            // 
            // txtContrasena
            // 
            this.txtContrasena.Location = new System.Drawing.Point(120, 132);
            this.txtContrasena.Name = "txtContrasena";
            this.txtContrasena.PasswordChar = '●';
            this.txtContrasena.Size = new System.Drawing.Size(426, 23);
            this.txtContrasena.TabIndex = 10;
            this.txtContrasena.Leave += new System.EventHandler(this.txtContrasena_Leave);
            // 
            // lblBaseDatos
            // 
            this.lblBaseDatos.AutoSize = true;
            this.lblBaseDatos.Location = new System.Drawing.Point(12, 165);
            this.lblBaseDatos.Name = "lblBaseDatos";
            this.lblBaseDatos.Size = new System.Drawing.Size(108, 16);
            this.lblBaseDatos.TabIndex = 11;
            this.lblBaseDatos.Text = "Base de datos:";
            // 
            // txtBaseDatos
            // 
            this.txtBaseDatos.Location = new System.Drawing.Point(120, 162);
            this.txtBaseDatos.Name = "txtBaseDatos";
            this.txtBaseDatos.Size = new System.Drawing.Size(426, 23);
            this.txtBaseDatos.TabIndex = 12;
            // 
            // cmbBaseDatos
            // 
            this.cmbBaseDatos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBaseDatos.FormattingEnabled = true;
            this.cmbBaseDatos.Location = new System.Drawing.Point(120, 162);
            this.cmbBaseDatos.Name = "cmbBaseDatos";
            this.cmbBaseDatos.Size = new System.Drawing.Size(426, 24);
            this.cmbBaseDatos.TabIndex = 13;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(348, 205);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(198, 52);
            this.btnGuardar.TabIndex = 15;
            this.btnGuardar.Text = "Guardar Conexión";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnProbar
            // 
            this.btnProbar.Location = new System.Drawing.Point(120, 205);
            this.btnProbar.Name = "btnProbar";
            this.btnProbar.Size = new System.Drawing.Size(198, 52);
            this.btnProbar.TabIndex = 14;
            this.btnProbar.Text = "Probar Conexión";
            this.btnProbar.UseVisualStyleBackColor = true;
            this.btnProbar.Click += new System.EventHandler(this.btnProbar_Click);
            // 
            // FRMconexiones
            // 
            this.ClientSize = new System.Drawing.Size(570, 269);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnProbar);
            this.Controls.Add(this.cmbBaseDatos);
            this.Controls.Add(this.txtBaseDatos);
            this.Controls.Add(this.lblBaseDatos);
            this.Controls.Add(this.txtContrasena);
            this.Controls.Add(this.lblContrasena);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.btnBuscarBase);
            this.Controls.Add(this.txtServidor);
            this.Controls.Add(this.lblServidor);
            this.Controls.Add(this.cmbMotor);
            this.Controls.Add(this.lblMotor);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.lblNombre);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FRMconexiones";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración de Conexión";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
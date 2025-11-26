using Capibara.CustomControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Capibara
{
    public partial class FRMconexiones : Form
    {
        private Dictionary<TipoMotor, string> motores;

        public Conexion ConexionActual = null;

        public FRMconexiones()
        {
            InitializeComponent();
            InicializarMotores();
        }

        public FRMconexiones(Conexion conexion)
        {
            InitializeComponent();
            InicializarMotores();
            ConexionActual = conexion;
            InicializarDatosConexion();
        }

        private void InicializarMotores()
        {
            motores = Enum.GetValues(typeof(TipoMotor))
                .Cast<TipoMotor>()
                .ToDictionary(m => m, m => m.ToString().Replace("_", " "));

            var lista = motores
                .Select(x => new { Key = x.Key, Value = x.Value })
                .ToList();

            cmbMotor.DataSource = lista;
            cmbMotor.DisplayMember = "Value";
            cmbMotor.ValueMember = "Key";

            if (cmbMotor.Items.Count > 0)
                cmbMotor.SelectedIndex = 0;
        }

        private void InicializarDatosConexion()
        {
            if (ConexionActual != null)
            {
                txtNombre.Text = ConexionActual.Nombre;
                cmbMotor.SelectedValue = ConexionActual.Motor;
                txtServidor.Text = ConexionActual.Servidor;

                try
                {
                    cmbBaseDatos.SelectedItem = ConexionActual.BaseDatos;
                }
                catch
                {
                }

                txtBaseDatos.Text = ConexionActual.BaseDatos;
                txtUsuario.Text = ConexionActual.Usuario;
                txtContrasena.Text = ConexionActual.Contrasena;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (ConexionActual == null)
                ConexionActual = new Conexion();

            ConexionActual.Nombre = txtNombre.Text.Trim();
            ConexionActual.Motor = (TipoMotor)cmbMotor.SelectedValue;
            ConexionActual.Servidor = txtServidor.Text.Trim();
            //ConexionActual.BaseDatos = cmbBaseDatos.Visible ? (cmbBaseDatos.Text ?? string.Empty).Trim()  : (txtBaseDatos.Text ?? string.Empty).Trim();
            ConexionActual.BaseDatos = (cmbBaseDatos.Text ?? string.Empty).Trim();
            ConexionActual.Usuario = txtUsuario.Text.Trim();
            ConexionActual.Contrasena = txtContrasena.Text;

            var conexiones = ConexionesManager.Cargar();
            conexiones[ConexionActual.Nombre] = ConexionActual;
            ConexionesManager.Guardar(conexiones);

            // Si querés, acá podés asignar la conexión actual al formulario principal.
            // MainForm.ConexionActual = ConexionActual;

            CustomMessageBox.Show("Conexión guardada correctamente.", "Éxito",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private void cmbMotor_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbMotor.SelectedValue == null)
                    return;

                var motor = (TipoMotor)cmbMotor.SelectedValue;
                var seleccionado = motor.ToString();

                // DB2: combo de bases
                if (motor == TipoMotor.DB2)
                {
                    //cmbBaseDatos.Visible = true;
                    cmbBaseDatos.Items.Clear();
                    cmbBaseDatos.Items.AddRange(ConexionesManager.BasesDB2);
                    if (cmbBaseDatos.Items.Count > 0)
                        cmbBaseDatos.SelectedIndex = 0;

                    //txtBaseDatos.Visible = false;
                    //txtBaseDatos.Enabled = false;
                    //cmbBaseDatos.Enabled = true;
                }
                //else
                //{
                //    cmbBaseDatos.Visible = false;
                //    txtBaseDatos.Visible = true;
                //    txtBaseDatos.Enabled = true;
                //    cmbBaseDatos.Enabled = false;
                //}


                btnBuscarBase.Enabled = false;

                lblUsuario.Visible = true;
                txtUsuario.Visible = true;

                lblContrasena.Visible = true;
                txtContrasena.Visible = true;

                //lblBaseDatos.Visible = true;
                //cmbBaseDatos.Visible = (motor == TipoMotor.DB2);
                //txtBaseDatos.Visible = !cmbBaseDatos.Visible;
            }
            catch { }
        }

        private void txtContrasena_Leave(object sender, EventArgs e)
        {
            if (cmbMotor.SelectedValue == null)
                return;

            var motor = (TipoMotor)cmbMotor.SelectedValue;
            if (motor != TipoMotor.MS_SQL)
            {
                cmbBaseDatos.Items.Clear();
                cmbBaseDatos.Items.AddRange(ConexionesManager.BasesDB2);
                if (cmbBaseDatos.Items.Count > 0)
                    cmbBaseDatos.SelectedIndex = 0;
                return;
            }

            string servidor = txtServidor.Text.Trim();
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text;

            if (string.IsNullOrWhiteSpace(servidor) ||
                string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrWhiteSpace(contrasena))
                return;

            string stringConnection = string.Empty;
            if (servidor.EndsWith("WEB"))
            {
                stringConnection = $@"Driver={{ODBC Driver 17 for SQL Server}};Server=SQL{servidor.Replace("WEB", string.Empty)}\{servidor};Database=;Uid={usuario};Pwd={contrasena};TrustServerCertificate=yes;";
            }
            else
            {
                stringConnection = $@"Driver={{ODBC Driver 17 for SQL Server}};Server=SQL{servidor}\{servidor};Database=;Uid={usuario};Pwd={contrasena};TrustServerCertificate=yes;";
            }

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                using (var c = new OdbcConnection(stringConnection))
                {
                    c.Open();
                    cmbBaseDatos.Items.Clear();

                    using (OdbcCommand cmd = c.CreateCommand())
                    {
                        cmd.CommandText =
                            "SELECT UPPER(name) FROM sys.databases WHERE state = 0 ORDER BY name ASC";

                        using (OdbcDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cmbBaseDatos.Items.Add(reader.GetString(0));
                            }
                        }
                    }

                    cmbBaseDatos.Visible = true;
                    cmbBaseDatos.Enabled = true;
                    txtBaseDatos.Visible = false;
                    txtBaseDatos.Enabled = false;
                }
            }
            catch
            {
                CustomMessageBox.Show(
                    "Ocurrió un error al intentar obtener las bases de datos desde el servidor. Verifique el usuario y la contraseña.",
                    "ATENCIÓN!!!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnBuscarBase_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Seleccionar archivo";
                dlg.Filter = "Bases SQLite (*.db)|*.db|Todos los archivos (*.*)|*.*";

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    string ruta = dlg.FileName;
                    txtServidor.Text = ruta;
                }
            }
        }

        private void btnProbar_Click(object sender, EventArgs e)
        {
            if (cmbMotor.SelectedValue == null)
                return;

            string stringConnection = string.Empty;
            var motor = (TipoMotor)cmbMotor.SelectedValue;

            switch (motor)
            {
                case TipoMotor.MS_SQL:
                    stringConnection =
                        $@"Driver={{ODBC Driver 17 for SQL Server}};Server=SQL{txtServidor.Text}\{txtServidor.Text};Database={cmbBaseDatos.Text};Uid={txtUsuario.Text};Pwd={txtContrasena.Text};TrustServerCertificate=yes;";
                    break;

                case TipoMotor.DB2:
                    stringConnection =
                        $"Driver={{IBM DB2 ODBC DRIVER}};Database={cmbBaseDatos.Text};Hostname={txtServidor.Text};Port=50000; Protocol=TCPIP;Uid={txtUsuario.Text};Pwd={txtContrasena.Text};";
                    break;
            }

            if (string.IsNullOrWhiteSpace(stringConnection))
            {
                CustomMessageBox.Show("El string de conexión está vacío", "Atención!!!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                btnProbar.Enabled = false;

                using (var c = new OdbcConnection(stringConnection))
                {
                    c.Open();
                    CustomMessageBox.Show("Conexión exitosa.", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Conexión fallida: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                btnProbar.Enabled = true;
            }
        }
    }
}

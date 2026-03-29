using Capibara.CustomControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
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
                txtBaseDatos.Text = ConexionActual.BaseDatos;
                txtUsuario.Text = ConexionActual.Usuario;
                txtContrasena.Text = ConexionActual.Contrasena;
                TXTpuerto.Text = ConexionActual.Puerto;
                CHKesWeb.Checked = ConexionActual.EsWeb;

                try
                {
                    CargarBasesDeDatos();
                    cmbBaseDatos.SelectedItem = ConexionActual.BaseDatos;
                }
                catch
                {
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (ConexionActual == null)
                ConexionActual = new Conexion();

            ConexionActual.Nombre = txtNombre.Text.Trim();
            ConexionActual.Motor = (TipoMotor)cmbMotor.SelectedValue;
            ConexionActual.Servidor = txtServidor.Text.Trim();
            ConexionActual.BaseDatos = (cmbBaseDatos.Text ?? string.Empty).Trim();
            ConexionActual.Usuario = txtUsuario.Text.Trim();
            ConexionActual.Contrasena = txtContrasena.Text;
            ConexionActual.Puerto = TXTpuerto.Text;
            ConexionActual.EsWeb = CHKesWeb.Checked;

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
                    cmbBaseDatos.Items.Clear();
                    cmbBaseDatos.Items.AddRange(ConexionesManager.BasesDB2);
                    if (cmbBaseDatos.Items.Count > 0)
                        cmbBaseDatos.SelectedIndex = 0;
                }

                btnBuscarBase.Enabled = false;

                lblUsuario.Visible = true;
                txtUsuario.Visible = true;

                lblContrasena.Visible = true;
                txtContrasena.Visible = true;
            }
            catch { }
        }

        private void txtContrasena_Leave(object sender, EventArgs e)
        {
            CargarBasesDeDatos();
        }

        private void CargarBasesDeDatos()
        {
            if (cmbMotor.SelectedValue == null)
                return;

            var motor = (TipoMotor)cmbMotor.SelectedValue;
            if (motor == TipoMotor.DB2)
            {
                cmbBaseDatos.Items.Clear();
                cmbBaseDatos.Items.AddRange(ConexionesManager.BasesDB2);
                if (cmbBaseDatos.Items.Count > 0)
                    cmbBaseDatos.SelectedIndex = 0;
                return;
            }
            Conexion stringConnection = new Conexion();

            stringConnection.Servidor = txtServidor.Text.Trim();
            stringConnection.Usuario = txtUsuario.Text.Trim();
            stringConnection.Contrasena = txtContrasena.Text;
            stringConnection.Motor = motor;
            stringConnection.EsWeb = CHKesWeb.Checked;

            if (string.IsNullOrWhiteSpace(stringConnection.Servidor) ||
                string.IsNullOrWhiteSpace(stringConnection.Usuario) ||
                string.IsNullOrWhiteSpace(stringConnection.Contrasena))
                return;

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                List<string> basesDeDatos = ObtenerBasesDeDatos(stringConnection.StringConnection(false), motor);

                foreach (string baseDeDatos in basesDeDatos)
                {
                    cmbBaseDatos.Items.Add(baseDeDatos);
                }

                cmbBaseDatos.Visible = true;
                cmbBaseDatos.Enabled = true;
                txtBaseDatos.Visible = false;
                txtBaseDatos.Enabled = false;
            }
            catch (Exception err)
            {
                CustomMessageBox.Show(
                    "Ocurrió un error al intentar obtener las bases de datos desde el servidor. Verifique el usuario y la contraseña.",
                    "ATENCIÓN!!! " + err.Message,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        public List<string> ObtenerBasesDeDatos(string stringConnection, TipoMotor motor)
        {
            List<string> bases = new List<string>();
            string sql = string.Empty;

            switch (motor)
            {
                case TipoMotor.DB2:
                    sql = "SELECT SCHEMANAME FROM SYSCAT.SCHEMATA";
                    break;
                case TipoMotor.MS_SQL:
                    sql = "SELECT UPPER(name) FROM sys.databases WHERE state = 0 ORDER BY name ASC";
                    break;
                case TipoMotor.POSTGRES:
                    sql = "SELECT datname FROM pg_database WHERE datistemplate = false";
                    break;
                case TipoMotor.SQLITE:
                    sql = "PRAGMA database_list";
                    break;
            }

            using (var conn = new OdbcConnection(stringConnection))
            using (var cmd = new OdbcCommand(sql, conn))
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                    bases.Add(reader[0].ToString());
            }

            return bases;
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

            var motor = (TipoMotor)cmbMotor.SelectedValue;

            Conexion stringConnection = new Conexion();
            stringConnection.Servidor = txtServidor.Text;
            stringConnection.Usuario = txtUsuario.Text;
            stringConnection.BaseDatos = cmbBaseDatos.Text;
            stringConnection.Contrasena = txtContrasena.Text;
            stringConnection.EsWeb = CHKesWeb.Checked;

            switch (motor)
            {
                case TipoMotor.DB2:
                    stringConnection.Motor = TipoMotor.DB2;
                    break;
                case TipoMotor.MS_SQL:
                    stringConnection.Motor = TipoMotor.MS_SQL;
                    break;
                case TipoMotor.POSTGRES:
                    stringConnection.Motor = TipoMotor.POSTGRES;
                    break;
                case TipoMotor.SQLITE:
                    stringConnection.Motor = TipoMotor.SQLITE;
                    break;
                default:
                    break;
            }

            if (string.IsNullOrWhiteSpace(stringConnection.StringConnection()))
            {
                CustomMessageBox.Show("El string de conexión está vacío", "Atención!!!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                btnProbar.Enabled = false;

                using (var c = new OdbcConnection(stringConnection.StringConnection()))
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

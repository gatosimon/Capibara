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

        public FRMconexiones(Conexion conexion)
        {
            InitializeComponent();
            InicializarMotores();
            ConexionActual = conexion;
            InicializarDatosConexion();
        }
        // ------------------------------------------------------------
        // Inicializa el combo con los motores
        // ------------------------------------------------------------
        private void InicializarMotores()
        {
            motores = Enum.GetValues(typeof(TipoMotor))
                .Cast<TipoMotor>()
                .ToDictionary(m => m, m => m.ToString().Replace("_", " "));

            CMBmotor.DataSource = motores.ToList();
            CMBmotor.DisplayMember = "Value";
            CMBmotor.ValueMember = "Key";
            CMBmotor.SelectedIndex = 0;
        }
        private void InicializarDatosConexion()
        {
            if (ConexionActual != null)
            {
                TXTnombre.Text = ConexionActual.Nombre;
                CMBmotor.SelectedValue = ConexionActual.Motor;
                TXTservidor.Text = ConexionActual.Servidor;
                TXTusuario.Text = ConexionActual.Usuario;
                TXTcontraseña.Text = ConexionActual.Contrasena;
            }
        }

        private void FRMconexiones_Load(object sender, EventArgs e)
        {

        }

        private void CMBmotor_SelectedIndexChanged(object sender, EventArgs e)
        {
            var seleccionado = ((KeyValuePair<TipoMotor, string>)CMBmotor.SelectedItem).Key;

            // DB2: usa combo
            if (seleccionado == TipoMotor.DB2)
            {
                ConexionActual.BaseDatos = ConexionesManager.BasesDB2;
            }
        }

        private void Control_Leave(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(TXTnombre.Text) && !string.IsNullOrEmpty(TXTservidor.Text) && !string.IsNullOrEmpty(TXTusuario.Text) && !string.IsNullOrEmpty(TXTcontraseña.Text) && CMBmotor.SelectedIndex > -1)
                {
                    var motor = (TipoMotor)CMBmotor.SelectedValue;

                    if (motor != TipoMotor.MS_SQL)
                        return;

                    string stringConnection =
                        $@"Driver={{ODBC Driver 17 for SQL Server}};
                        Server=SQL{TXTservidor.Text.Trim()}\{TXTservidor.Text.Trim()};
                        Database=;
                        Uid={TXTusuario.Text.Trim()};
                        Pwd={TXTcontraseña.Text};
                        TrustServerCertificate=yes;";

                    try
                    {
                        using (var c = new OdbcConnection(stringConnection))
                        {
                            c.Open();
                            ConexionActual.BaseDatos = new string[] { };
                            List<string> basesDeDatos = new List<string>();

                            OdbcCommand cmd = c.CreateCommand();
                            cmd.CommandText = "SELECT UPPER(name) FROM sys.databases WHERE state = 0 ORDER BY name ASC";

                            var reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                basesDeDatos.Add(reader.GetString(0));
                            }
                            ConexionActual.BaseDatos = basesDeDatos.ToArray();
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Error al obtener bases de datos. Verifique usuario y contraseña.",
                            "ATENCIÓN!!!");
                    }
                }
            }
            catch { }
        }

        private void BTNguardar_Click(object sender, EventArgs e)
        {
            if (ConexionActual == null)
                ConexionActual = new Conexion();

            ConexionActual.Nombre = TXTnombre.Text.Trim();
            ConexionActual.Motor = (TipoMotor)CMBmotor.SelectedValue;
            ConexionActual.Servidor = TXTservidor.Text.Trim();
            ConexionActual.Usuario = TXTusuario.Text.Trim();
            ConexionActual.Contrasena = TXTcontraseña.Text;

            var conexiones = ConexionesManager.Cargar();
            conexiones[ConexionActual.Nombre] = ConexionActual;
            ConexionesManager.Guardar(conexiones);

            MessageBox.Show("Conexión guardada correctamente.",
                "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Close();
        }

        private void BTNeliminar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BTNcancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BTNprobar_Click(object sender, EventArgs e)
        {
            string stringConnection = string.Empty;
            TipoMotor motor = (TipoMotor)CMBmotor.SelectedValue;

            switch (motor)
            {
                case TipoMotor.MS_SQL:
                    stringConnection = $@"Driver={{ODBC Driver 17 for SQL Server}};Server=SQL{TXTservidor.Text}\{TXTservidor.Text};Database={ConexionActual.BaseDatos[0]};Uid={TXTusuario.Text};Pwd={TXTcontraseña.Text};TrustServerCertificate=yes;";
                    break;
                case TipoMotor.DB2:
                    stringConnection = $"Driver={{IBM DB2 ODBC DRIVER}};Database={ConexionActual.BaseDatos[0]};Hostname={TXTservidor.Text};Port=50000; Protocol=TCPIP;Uid={TXTusuario.Text};Pwd={TXTcontraseña.Text};";
                    break;
                default:
                    break;
            }

            if (string.IsNullOrWhiteSpace(stringConnection))
            {
                MessageBox.Show("El string de conexión está vacío", "Atención!!!");
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    using (var c = new OdbcConnection(stringConnection))
                    {
                        c.Open();
                        MessageBox.Show("Conexión exitosa.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Conexión fallida: " + ex.Message);
                }
            });
        }
    }
}

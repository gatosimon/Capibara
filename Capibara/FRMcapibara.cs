using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Capibara.Utilidades;
using System.Threading.Tasks;
using WMPLib;
using System.Threading;

namespace Capibara
{
    public partial class FRMcapibara : Form
    {
        Dictionary<Type, string> TIPOS = new Dictionary<Type, string>()
        {
            {typeof(bool),      "bool"},
            {typeof(byte),      "byte"},
            {typeof(Byte[]),    "byte[]"},
            {typeof(char),      "char"},
            {typeof(char[]),    ERROR},
            {typeof(DateTime),  "DateTime"},
            {typeof(DBNull),    ERROR},
            {typeof(decimal),   "double"},
            {typeof(double),    "double"},
            {typeof(Guid),      "Guid"},
            {typeof(int),       "int"},			// Int32
            {typeof(Int16),     "short"},			// Int16s
            {typeof(Int64),     "long"},			// Int64
            {typeof(Object),    ERROR},
            {typeof(SByte),     "double"},
            {typeof(Single),    "double"},
            {typeof(string),    "string"},
            {typeof(TimeSpan),  "TimeSpan"},
            {typeof(ushort),    "uint"},
            {typeof(uint),      "uint"},
            {typeof(ulong),     "uint"}
        };

        Dictionary<string, string> Mapeo = new Dictionary<string, string>
        {
            { "bool",       "OdbcType.Bit"},            //"OdbcType.Smallint"}, 	// DB2 no tiene BOOLEAN real en versiones antiguas
            { "byte",       "OdbcType.TinyInt"},        // SMALLINT usado como byte en DB2
            { "byte[]",     "OdbcType.VarBinary"},	    // BLOB, VARBINARY, BYTEA
            { "Byte[]",     "OdbcType.VarBinary"},	    // BLOB, VARBINARY, BYTEA
            { "char",       "OdbcType.Char"},           // CHAR(1)
            { "char[]",     "OdbcType.VarBinary"},	    // BLOB, VARBINARY, BYTEA
            { "DateTime",   "OdbcType.DateTime"},	    // TIMESTAMP (Date si sólo fecha)
            { "DBNull",     ERROR},
            { "decimal",    "OdbcType.Numeric" },       // DECIMAL(p,s), NUMERIC
            { "double",     "OdbcType.Double"},	        // DOUBLE
            { "float",      "OdbcType.Real" }, 	        // REAL
            { "Guid",       "OdbcType.Char"},           // DB2 no tiene UNIQUEIDENTIFIER → usar CHAR(36)
            { "int",        "OdbcType.Int"},            // INTEGER
            { "Int16",      "OdbcType.SmallInt" },      // SMALLINT	
            { "Int64",      "OdbcType.BigInt" },	    // BIGINT
            { "long",       "OdbcType.BigInt" },	    // BIGINT
            { "Object",     ERROR},
            { "SByte",      "OdbcType.Double"},	        // DOUBLE
            { "short",      "OdbcType.SmallInt" },      // SMALLINT	
            { "Single",     "OdbcType.Double"},	        // DOUBLE
            { "string",     "OdbcType.VarChar"},	    // VARCHAR, usar NVarChar si es Unicode	
            { "TimeSpan",   "OdbcType.Time"},	        // TIME
            { "uint",       "OdbcType.BigInt"},          // BIGINT
            { "ulong",       "OdbcType.BigInt"},          // BIGINT
            { "ushort",      "OdbcType.BigInt"}          // BIGINT
        };

        // Diccionario de mapeo de tipos .NET a tipos DB2
        Dictionary<Type, string> TIPOSDB2 = new Dictionary<Type, string>
        {
            { typeof(bool), "BOOLEAN" },        // System.Boolean (DB2 11+)
            { typeof(byte[]), "BLOB" },         // System.Byte[] (puede mapear BLOB, VARBINARY, etc.)
            { typeof(DateTime), "DATE" },       // System.DateTime (puede mapear DATE, TIME o TIMESTAMP)
            { typeof(decimal), "DECIMAL" },     // System.Decimal
            { typeof(double), "DOUBLE" },       // System.Double
            { typeof(float), "REAL" },          // System.Single
            { typeof(int), "INTEGER" },         // System.Int32
            { typeof(long), "BIGINT" },         // System.Int64
            { typeof(short), "SMALLINT" },      // System.Int16
            { typeof(string), "VARCHAR" },      // System.String (puede ser CHAR, VARCHAR, CLOB, LONGVAR)
            { typeof(TimeSpan), "TIME" },       // System.TimeSpan 
            { typeof(object), "ROWID" }         // System.Object (para ROWID u otros tipos especiales)
        };

        // Diccionario de mapeo de tipos .NET a tipos SQL Server
        Dictionary<Type, string> TIPOSSQL = new Dictionary<Type, string>
        {
            { typeof(bool), "BIT" },              // System.Boolean → BIT
            { typeof(byte), "TINYINT" },          // System.Byte → TINYINT
            { typeof(byte[]), "VARBINARY(MAX)" }, // System.Byte[] → BINARY / VARBINARY / IMAGE (desaconsejado)
            { typeof(char), "NCHAR(1)" },         // System.Char → NCHAR(1)
            { typeof(DateTime), "DATETIME" },     // System.DateTime → DATETIME (ó DATE, DATETIME2, SMALLDATETIME según precisión)
            { typeof(DateTimeOffset), "DATETIMEOFFSET" }, // Con zona horaria
            { typeof(decimal), "DECIMAL" },       // System.Decimal → DECIMAL(p,s) / NUMERIC
            { typeof(double), "FLOAT" },          // System.Double → FLOAT (8 bytes)
            { typeof(float), "REAL" },            // System.Single → REAL (4 bytes)
            { typeof(int), "INT" },               // System.Int32 → INT
            { typeof(long), "BIGINT" },           // System.Int64 → BIGINT
            { typeof(short), "SMALLINT" },        // System.Int16 → SMALLINT
            { typeof(string), "NVARCHAR(MAX)" },  // System.String → NVARCHAR / VARCHAR / CHAR / TEXT (deprecated)
            { typeof(TimeSpan), "TIME" },         // System.TimeSpan → TIME
            { typeof(Guid), "UNIQUEIDENTIFIER" }, // System.Guid → UNIQUEIDENTIFIER
            { typeof(object), "SQL_VARIANT" }     // System.Object → SQL_VARIANT
        };

        Dictionary<Type, string> PropiedadesTS = new Dictionary<Type, string>
        {
            {typeof(bool),      "Boolean;"},
            {typeof(byte),      "number = 0;"},
            {typeof(Byte[]),    "string = '';"},
            {typeof(char),      "string = '';"},
            {typeof(char[]),    "any;"},
            {typeof(DateTime),  "Date;"},
            {typeof(DBNull),    "null;"},
            {typeof(decimal),   "number = 0;"},
            {typeof(double),    "number = 0;"},
            {typeof(Guid),      "any;"},
            {typeof(int),       "number = 0;"},		// Int32
            {typeof(Int16),     "number = 0;"},		// Int16
            {typeof(Int64),     "number = 0;"},		// Int64
            {typeof(Object),    "any;"},
            {typeof(SByte),     "number = 0;"},
            {typeof(Single),    "number = 0;"},
            {typeof(string),    "string = '';"},
            {typeof(TimeSpan),  "Date;"},
            {typeof(ushort),    "number = 0;"},
            {typeof(uint),      "number = 0;"},
            {typeof(ulong),     "number = 0;"}
        };

        private Dictionary<string, string> CamposABM = new Dictionary<string, string>
        {
            { "FECHA ACTUAL", "System.DateTime.Now;"},
            { "FECHA POR DEFECTO", "new DateTime(1900, 1, 1);" },
            { "USUARIO MAGIC",  "Config.UsuarioMagic;" },
            { "CÓDIGO BAJA",  "codigoBaja;" },
            { "MOTIVO BAJA", "motivoBaja;" },
            { "CÓDIGO 0", "0;" },
            { "CADENA VACÍA", "string.Empty;" },
            { "HORA ACTUAL", "DateTime.Now.TimeOfDay;" },
            { "HORA POR DEFECTO", "TimeSpan.Zero;"}
        };

        private const string ERROR = "ERROR";
        private const string CONTROLLER = "Controller";
        private const string DTO = "Dto";
        private const string MODEL = "Model";
        private const string REPOSITORIES = "Repositories";
        private const string REPOSITORIES_INTERFACE = "RepositoriesInterface";
        private const string SERVICE = "Service";
        private const string SERVICE_INTERFACE = "ServiceInterface";

        List<string> tablasBase = new List<string>();
        List<string> camposTabla = new List<string>();
        string TABLA = string.Empty;
        string pathControllers { get { return this.TXTpathCapas.Text + @"\" + TABLA + @"\Controllers\"; } }
        string pathClaseController { get { return pathControllers + TABLA + CONTROLLER + ".cs"; } }
        string pathDto { get { return TXTpathCapas.Text + @"\" + TABLA + @"\" + DTO + @"\"; } }
        string pathClaseDto { get { return pathDto + TABLA + DTO + ".cs"; } }
        string pathModel { get { return TXTpathCapas.Text + @"\" + TABLA + @"\" + MODEL + @"\"; } }
        string pathClaseModel { get { return pathModel + TABLA + MODEL + ".cs"; } }
        string pathRepositories { get { return TXTpathCapas.Text + @"\" + TABLA + @"\" + REPOSITORIES + @"\"; } }
        string pathClaseRepositories { get { return pathRepositories + TABLA + REPOSITORIES + ".cs";}}
        string pathClaseRepositoriesInterface { get { return pathRepositories + TABLA + REPOSITORIES_INTERFACE + ".cs";}}
        string pathService { get { return TXTpathCapas.Text + @"\" + TABLA + @"\" + SERVICE + @"\";}}
        string pathClaseService { get { return pathService + TABLA + SERVICE + ".cs"; }}
        string pathClaseServiceInterface { get { return pathService + TABLA + SERVICE_INTERFACE + ".cs";}}

        private bool generarDesdeConsulta = false;

        private bool desplegarCombo = false;

        Configuracion configuracion;
        private WindowsMediaPlayer player;
        public FRMcapibara()
        {
            InitializeComponent();

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.ShowImageMargin = false;
            menu.Items.Add("DESDE TABLA", null, (s, ev) => GenerarDesdeTabla());
            menu.Items.Add("DESDE CONSULTA", null, (s, ev) => GenerarDesdeConsulta());

            // Asignar menú al SplitButton
            BTNgenerarDesdeTabla.Menu = menu;

            CargarConfiguracion();

            ListarNameSpaces();
            InicializarIndices();
        }

        private void CargarConfiguracion()
        {
            configuracion = Configuracion.Cargar();
            RDBsql.Checked = configuracion.SQL;
            TXTespacioDeNombres.Text = configuracion.UltimoNamespaceSeleccionado;
            TXTpathCapas.Text = configuracion.RutaPorDefectoResultados;
            OFDlistarDeSolucion.InitialDirectory = configuracion.PathSolucion != null && configuracion.PathSolucion.Length > 0 ? Directory.GetDirectoryRoot(configuracion.PathSolucion) : string.Empty;
            OFDlistarDeSolucion.FileName = configuracion.PathSolucion;

            foreach (string[] item in configuracion.camposBaja)
            {
                int indiceFila = DGVbaja.Rows.Add();
                DGVbaja.Rows[indiceFila].Cells[0].Value = item[0];
                ((DataGridViewComboBoxColumn)DGVbaja.Columns[1]).DataSource = new BindingSource(CamposABM, null);
                ((DataGridViewComboBoxColumn)DGVbaja.Columns[1]).DisplayMember = "Key";
                ((DataGridViewComboBoxColumn)DGVbaja.Columns[1]).ValueMember = "Value";
                DGVbaja.Rows[indiceFila].Cells[1].Value = CamposABM[item[1]];
            }

            foreach (string[] item in configuracion.camposModificacion)
            {
                int indiceFila = DGVmodificacion.Rows.Add();
                DGVmodificacion.Rows[indiceFila].Cells[0].Value = item[0];
                ((DataGridViewComboBoxColumn)DGVmodificacion.Columns[1]).DataSource = new BindingSource(CamposABM, null);
                ((DataGridViewComboBoxColumn)DGVmodificacion.Columns[1]).DisplayMember = "Key";
                ((DataGridViewComboBoxColumn)DGVmodificacion.Columns[1]).ValueMember = "Value";
                DGVmodificacion.Rows[indiceFila].Cells[1].Value = CamposABM[item[1]];
            }

            foreach (string[] item in configuracion.camposRecuperacion)
            {
                int indiceFila = DGVrecuperacion.Rows.Add();
                DGVrecuperacion.Rows[indiceFila].Cells[0].Value = item[0];
                ((DataGridViewComboBoxColumn)DGVrecuperacion.Columns[1]).DataSource = new BindingSource(CamposABM, null);
                ((DataGridViewComboBoxColumn)DGVrecuperacion.Columns[1]).DisplayMember = "Key";
                ((DataGridViewComboBoxColumn)DGVrecuperacion.Columns[1]).ValueMember = "Value";
                DGVrecuperacion.Rows[indiceFila].Cells[1].Value = CamposABM[item[1]];
            }
        }
        private void GuardarConfiguracion()
        {
            try
            {
                TXTclase.Text = Clase(CMBtablas.Items[CMBtablas.SelectedIndex].ToString(), generarDesdeConsulta ? TXTgenerarAPartirDeConsulta.Text : string.Empty);

                configuracion.SQL = RDBsql.Checked;
                configuracion.Servidor = CMBservidor.Items[CMBservidor.SelectedIndex].ToString();
                configuracion.Base = CMBbases.Items[CMBbases.SelectedIndex].ToString();
                configuracion.Tabla = CMBtablas.Items[CMBtablas.SelectedIndex].ToString();
                configuracion.Consulta = TXTgenerarAPartirDeConsulta.Text;
                configuracion.UltimoNamespaceSeleccionado = TXTespacioDeNombres.Text;
                configuracion.RutaPorDefectoResultados = TXTpathCapas.Text;
                configuracion.PathSolucion = OFDlistarDeSolucion.FileName;

                configuracion.camposBaja.Clear();
                foreach (DataGridViewRow item in DGVbaja.Rows)
                {
                    List<string> celdas = new List<string>();
                    foreach (DataGridViewCell celdaActual in item.Cells)
                    {
                        celdas.Add(celdaActual.FormattedValue.ToString());
                    }
                    configuracion.camposBaja.Add(celdas.ToArray());
                }

                configuracion.camposModificacion.Clear();
                foreach (DataGridViewRow item in DGVmodificacion.Rows)
                {
                    List<string> celdas = new List<string>();
                    foreach (DataGridViewCell celdaActual in item.Cells)
                    {
                        celdas.Add(celdaActual.FormattedValue.ToString());
                    }
                    configuracion.camposModificacion.Add(celdas.ToArray());
                }

                configuracion.camposRecuperacion.Clear();
                foreach (DataGridViewRow item in DGVrecuperacion.Rows)
                {
                    List<string> celdas = new List<string>();
                    foreach (DataGridViewCell celdaActual in item.Cells)
                    {
                        celdas.Add(celdaActual.FormattedValue.ToString());
                    }
                    configuracion.camposRecuperacion.Add(celdas.ToArray());
                }

                configuracion.Guardar();
            }
            catch (Exception)
            {
            }
        }

        private const int PANEL1_MIN = 510; // ancho/alto mínimo que querés para Panel1

        private void FRMgeneradorDeCapas_Load(object sender, EventArgs e)
        {
            //InicializarIndices();
        }

        private void InicializarIndices()
        {
            SPCclase.Panel1MinSize = PANEL1_MIN;

            int indice = CMBservidor.FindStringExact(configuracion.Servidor);
            if (indice > -1 && CMBservidor.Items.Count > 0)
            {
                CMBservidor.SelectedIndex = indice > -1 ? indice : 0;
                CMBservidor.Text = CMBservidor.Items[CMBservidor.SelectedIndex].ToString();
            }
            indice = CMBbases.FindStringExact(configuracion.Base);
            if (indice > -1 && CMBbases.Items.Count > 0)
            {
                CMBbases.SelectedIndex = indice > -1 ? indice : 0;
                CMBbases.Text = CMBbases.Items[CMBbases.SelectedIndex].ToString();
            }

            indice = CMBtablas.FindStringExact(configuracion.Tabla);
            if (indice > -1 && CMBtablas.Items.Count > 0)
            {
                CMBtablas.SelectedIndex = indice > -1 ? indice : 0;
                CMBtablas.Text = CMBtablas.Items[CMBtablas.SelectedIndex].ToString();
            }

            TXTgenerarAPartirDeConsulta.Text = configuracion.Consulta;

            LSVcampos.View = View.Details;
            LSVcampos.CheckBoxes = true;
            LSVcampos.Columns.Add("Nombre", 200);
            LSVcampos.Columns.Add("Tipo", 80);
            LSVcampos.Columns.Add("Longitud", 70);
            LSVcampos.Columns.Add("Escala", 60);
            LSVcampos.Columns.Add("Acepta Nulos", 100);

            EnsureFormMinimumSize();
            EnforceSplitBounds();
        }

        private string Clase(string tabla, string consulta = "")
        {
            if (consulta.Trim().Length > 0)
            {
                tabla = "CONSULTA";
            }
            string resultado = string.Empty;
            List<DataColumn> claves = new List<DataColumn>();
            List<DataColumn> camposConsulta = new List<DataColumn>();
            List<string> columnasError = new List<string>();

            bool consultaOk = true;

            if (RDBdb2.Checked)
            {
                try
                {
                    Ejecutar datos = EstablecerConexion();
                    datos.Consulta = consulta.Trim().Length > 0 ? consulta : "SELECT " + string.Join(", ", camposTabla) + " FROM " + tabla + " FETCH FIRST 1 ROW ONLY";
                    ComandoDB2 DB2 = new ComandoDB2(datos.Consulta, datos.ObtenerConexion());
                    DB2.Conexion = new System.Data.Odbc.OdbcConnection(datos.ObtenerConexion());

                    DataSet DS = DB2.ObtenerDataSet();

                    int i = 0;
                    foreach (DataColumn columna in DS.Tables[0].Columns)
                    {
                        // Si genero a partir de una Query
                        if (consulta.Trim().Length > 0)
                        {
                            camposConsulta.Add(columna);
                        }
                        else
                        {
                            if (LSVcampos.Items[i].Checked)
                            {
                                claves.Add(columna);
                            }
                            camposConsulta.Add(columna);

                            if (Tipo(columna) == ERROR)
                            {
                                foreach (ListViewItem item in LSVcampos.Items)
                                {
                                    if (item.SubItems[0].Text == columna.ColumnName)
                                    {
                                        columnasError.Add(item.SubItems[0].Text + "\r\n     TIPO: " + item.SubItems[1].Text.Trim() + " (" + columna.DataType.ToString() + ")");
                                        item.BackColor = System.Drawing.Color.Red;
                                        item.ForeColor = System.Drawing.Color.White;
                                        item.Font = new System.Drawing.Font(item.Font.FontFamily, item.Font.Size, System.Drawing.FontStyle.Bold);
                                        item.ListView.Refresh();
                                        break;
                                    }
                                }
                            }

                            i++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    resultado = ex.Message;
                    consultaOk = false;
                }
            }
            else
            {
                try
                {
                    string servidor = CMBservidor.Items[CMBservidor.SelectedIndex].ToString().ToUpper();
                    string connectionString = @"Data Source=SQL" + servidor + @"\" + servidor + "; Initial Catalog=" + CMBbases.Items[CMBbases.SelectedIndex].ToString() + ";Persist Security Info=True;User ID=usuario;Password=ci?r0ba;MultipleActiveResultSets=True";
                    tabla = CMBtablas.Items[CMBtablas.SelectedIndex].ToString();

                    string query = consulta.Trim().Length > 0 ? consulta : "SELECT TOP 1 " + string.Join(", ", camposTabla) + " FROM " + tabla;

                    DataSet DS = new DataSet();
                    using (SqlDataAdapter DA = new SqlDataAdapter(query, connectionString))
                    {
                        DA.Fill(DS);
                    }

                    int i = 0;
                    foreach (DataColumn columna in DS.Tables[0].Columns)
                    {
                        // Si genero a partir de una Query
                        if (consulta.Trim().Length > 0)
                        {
                            camposConsulta.Add(columna);
                        }
                        else
                        {
                            if (LSVcampos.Items[i].Checked)
                            {
                                claves.Add(columna);
                            }
                            camposConsulta.Add(columna);

                            if (Tipo(columna) == ERROR)
                            {
                                foreach (ListViewItem item in LSVcampos.Items)
                                {
                                    if (item.SubItems[0].Text == columna.ColumnName)
                                    {
                                        columnasError.Add(item.SubItems[0].Text + "\r\n     TIPO: " + item.SubItems[1].Text.Trim() + " (" + columna.DataType.ToString() + ")");
                                        item.BackColor = System.Drawing.Color.Red;
                                        item.ForeColor = System.Drawing.Color.White;
                                        item.Font = new System.Drawing.Font(item.Font.FontFamily, item.Font.Size, System.Drawing.FontStyle.Bold);
                                        item.ListView.Refresh();
                                        break;
                                    }
                                }
                            }

                            i++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    resultado = ex.Message;
                    consultaOk = false;
                }
            }

            if (consultaOk)
            {
                if (columnasError.Count > 0)
                {
                    string columnas = string.Join("\r\n", columnasError);
                    CustomMessageBox.Show("NO SE PUEDE PROCESAR LA SIGUIENTE TABLA DEBIDO A INCONSISTENCIAS CON LOS SIGUIENTES CAMPOS:\r\n\r\n" + columnas, CustomMessageBox.ATENCION, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {

                    if (CHKquitarEsquema.Checked)
                    {
                        string[] partes = tabla.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        tabla = partes[partes.Length - 1];
                    }
                    TABLA = tabla;

                    if (CHKcontrollers.Checked)
                    {
                        resultado = ArmarControllers(claves);
                        resultado += "\r\n";
                    }
                    else
                    {
                        ArmarControllers(claves);
                    }
                    if (CHKdto.Checked)
                    {
                        resultado += ArmarDto(camposConsulta);
                        resultado += "\r\n";
                    }
                    else
                    {
                        ArmarDto(camposConsulta);
                    }
                    if (CHKmodel.Checked)
                    {
                        resultado += ArmarModel(camposConsulta, claves);
                        resultado += "\r\n";
                    }
                    else
                    {
                        ArmarModel(camposConsulta, claves);
                    }
                    if (CHKrepositories.Checked)
                    {
                        resultado += ArmarRepositories(camposConsulta, claves);
                        resultado += "\r\n";
                        resultado += ArmarRepositoriesInterface(claves);
                        resultado += "\r\n";
                    }
                    else
                    {
                        ArmarRepositories(camposConsulta, claves);
                        ArmarRepositoriesInterface(claves);
                    }
                    if (CHKservice.Checked)
                    {
                        resultado += ArmarService(camposConsulta, claves);
                        resultado += "\r\n";
                        resultado += ArmarServiceInterface(claves);
                        resultado += "\r\n";
                    }
                    else
                    {
                        ArmarService(camposConsulta, claves);
                        ArmarServiceInterface(claves);
                    }
                    if (CHKtypeScript.Checked)
                    {
                        resultado += ArmarTypeScript(camposConsulta);
                        resultado += "\r\n";
                    }
                    else
                    {
                        ArmarTypeScript(camposConsulta);
                    }

                    if (System.IO.Directory.Exists(TXTpathCapas.Text))
                    {
                        Process.Start("explorer.exe", TXTpathCapas.Text);
                    }
                }
            }
            else
            {
                CustomMessageBox.Show("Ocurrió un error al intentar acceder a la" + (consulta.Trim().Length > 0 ? " consulta. " : " tabla. ") + resultado, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return resultado;
        }

        private string ArmarControllers(List<DataColumn> claves)
		{
            bool DB2 = RDBdb2.Checked;
            string origen = DB2 ? MODEL : DTO;
            string nombreDeClase = TABLA;
            string tipoClase = TABLA + origen;
			string nombreClasePrimeraMinuscula = nombreDeClase[0].ToString().ToLower() + nombreDeClase.Substring(1);
			string espacioDeNombres = TXTespacioDeNombres.Text;
            string camposFromUri = string.Join(", ", (from c in claves select "[FromUri] " + Tipo(c) + " " + c.ColumnName).ToList());
            string camposClave = string.Join(", ", (from c in claves select c.ColumnName).ToList());
            StringBuilder Controller = new StringBuilder();

            Controller.AppendLine("using System;");
            Controller.AppendLine("using System.Collections.Generic;");
            Controller.AppendLine("using SistemaMunicipalGeneral;");
            Controller.AppendLine("using SistemaMunicipalGeneral.Web.FiltrosDeAccion;");
            Controller.AppendLine("using System.Threading.Tasks;");
            Controller.AppendLine("using System.Web.Http;");
            Controller.AppendLine("using System.Web.Http.Cors;");
            if(espacioDeNombres.Trim().Length > 0) Controller.AppendLine("using " + espacioDeNombres + "." + origen + ";");
            if (espacioDeNombres.Trim().Length > 0) Controller.AppendLine("using " + espacioDeNombres + "." + SERVICE + ";");
            Controller.AppendLine();
            Controller.AppendLine("namespace " + espacioDeNombres + ".Controllers");
            Controller.AppendLine("{");
            Controller.AppendLine("\t[RoutePrefix(\"" + nombreDeClase.ToLower() + "\")]");
            Controller.AppendLine("\t[EnableCors(origins: \" * \", headers: \" * \", methods: \" * \")]");
            Controller.AppendLine();
            Controller.AppendLine("\tpublic class " + nombreDeClase + CONTROLLER + " : ApiController");
            Controller.AppendLine("\t{");
            Controller.AppendLine("\t\tprivate readonly " + nombreDeClase + SERVICE_INTERFACE + " _" + nombreClasePrimeraMinuscula + SERVICE + ";");
            Controller.AppendLine("\t\tpublic " + nombreDeClase + CONTROLLER + "(" + nombreDeClase + SERVICE_INTERFACE + " " + nombreClasePrimeraMinuscula + SERVICE + ")");
            Controller.AppendLine("\t\t{");
            Controller.AppendLine("\t\t\t_" + nombreClasePrimeraMinuscula + SERVICE + " = " + nombreClasePrimeraMinuscula + SERVICE + " ?? throw new ArgumentNullException(nameof(" + nombreClasePrimeraMinuscula + SERVICE + "));");
            Controller.AppendLine("\t\t}");
            Controller.AppendLine();
            // ALTA
            if (CHKalta.Checked)
            {
                Controller.AppendLine("\t\t[HttpPost, Route(\"nuevo\"), ControlarPermisos]");
                Controller.AppendLine("\t\tpublic async Task<Respuesta> alta" + nombreDeClase + "([FromBody] " + tipoClase + " nuevo" + origen + ")");
                Controller.AppendLine("\t\t{");
                Controller.AppendLine("\t\t\tRespuesta rta = new Respuesta();");
                Controller.AppendLine();
                Controller.AppendLine("\t\t\trta.Resultado = _" + nombreClasePrimeraMinuscula + SERVICE + ".alta" + nombreDeClase + "(nuevo" + origen + ");");
                Controller.AppendLine();
                Controller.AppendLine("\t\t\treturn rta;");
                Controller.AppendLine("\t\t}");
                Controller.AppendLine(); 
            }
            // BAJA
            if (CHKbaja.Checked)
            {
                Controller.AppendLine("\t\t[HttpGet, Route(\"baja\"), ControlarPermisos]");
                Controller.AppendLine("\t\tpublic async Task<Respuesta> baja" + nombreDeClase + "(" + camposFromUri + ", [FromUri] int codigoBaja, [FromUri] string motivoBaja)");
                Controller.AppendLine("\t\t{");
                Controller.AppendLine("\t\t\tRespuesta rta = new Respuesta();");
                Controller.AppendLine();
                Controller.AppendLine("\t\t\trta.Resultado = _" + nombreClasePrimeraMinuscula + SERVICE + ".baja" + nombreDeClase + "(" + camposClave + ", codigoBaja, motivoBaja);");
                Controller.AppendLine();
                Controller.AppendLine("\t\t\treturn rta;");
                Controller.AppendLine("\t\t}");
                Controller.AppendLine(); 
            }
            // MODIFICACION
            if (CHKmodificacion.Checked)
            {
                Controller.AppendLine("\t\t[HttpPut, Route(\"modificacion\"), ControlarPermisos]");
                Controller.AppendLine("\t\tpublic async Task<Respuesta> modificacion" + nombreDeClase + "([FromBody] " + tipoClase + " nuevo" + origen + ")");
                Controller.AppendLine("\t\t{");
                Controller.AppendLine("\t\t\tRespuesta rta = new Respuesta();");
                Controller.AppendLine();
                Controller.AppendLine("\t\t\trta.Resultado = _" + nombreClasePrimeraMinuscula + SERVICE + ".modificacion" + nombreDeClase + "(nuevo" + origen + ");");
                Controller.AppendLine();
                Controller.AppendLine("\t\t\treturn rta;");
                Controller.AppendLine("\t\t}");
                Controller.AppendLine(); 
            }
            // BUSCAR POR ID
            if (CHKobtenerPorId.Checked)
            {
                Controller.AppendLine("\t\t[HttpGet, Route(\"buscarid\"), ControlarPermisos]");
                Controller.AppendLine("\t\tpublic async Task<Respuesta> obtenerPorId(" + camposFromUri + ")");
                Controller.AppendLine("\t\t{");
                Controller.AppendLine("\t\t\tRespuesta rta = new Respuesta();");
                Controller.AppendLine("\t\t\t" + tipoClase + " solicitado = _" + nombreClasePrimeraMinuscula + SERVICE + ".obtenerPorId(" + camposClave + ");");
                Controller.AppendLine();
                Controller.AppendLine("\t\t\tif (solicitado != null)");
                Controller.AppendLine("\t\t\t{");
                Controller.AppendLine("\t\t\t\trta.Resultado = solicitado;");
                Controller.AppendLine("\t\t\t}");
                Controller.AppendLine("\t\t\telse");
                Controller.AppendLine("\t\t\t{");
                Controller.AppendLine("\t\t\t\trta.AgregarMensajeDeError(\"No se halló " + nombreDeClase + "\");");
                Controller.AppendLine("\t\t\t}");
                Controller.AppendLine();
                Controller.AppendLine("\t\t\treturn rta;");
                Controller.AppendLine("\t\t}");
                Controller.AppendLine(); 
            }
            // TODOS
            if (CHKtodos.Checked)
            {
                Controller.AppendLine("\t\t[HttpGet, Route(\"todos\"), ControlarPermisos]");
                Controller.AppendLine("\t\tpublic async Task<Respuesta> obtenerTodos()");
                Controller.AppendLine("\t\t{");
                Controller.AppendLine("\t\t\tRespuesta rta = new Respuesta();");
                Controller.AppendLine("\t\t\tList <" + tipoClase + "> " + nombreDeClase.ToLower() + " = _" + nombreClasePrimeraMinuscula + SERVICE + ".obtenerTodos();");
                Controller.AppendLine("\t\t\tif (" + nombreDeClase.ToLower() + " != null)");
                Controller.AppendLine("\t\t\t{");
                Controller.AppendLine("\t\t\t\trta.Resultado = " + nombreDeClase.ToLower() + ";");
                Controller.AppendLine("\t\t\t}");
                Controller.AppendLine("\t\t\telse");
                Controller.AppendLine("\t\t\t{");
                Controller.AppendLine("\t\t\t\trta.AgregarMensajeDeError(\" - No existe " + nombreDeClase + " que responda a la consulta indicada.\");");
                Controller.AppendLine("\t\t\t}");
                Controller.AppendLine();
                Controller.AppendLine("\t\t\treturn rta;");
                Controller.AppendLine("\t\t}");
                Controller.AppendLine(); 
            }
            // RECUPERAR
            if (CHKrecuperacion.Checked)
            {
                Controller.AppendLine("\t\t[HttpGet, Route(\"recuperar\"), ControlarPermisos]");
                Controller.AppendLine("\t\tpublic async Task<Respuesta> recuperar" + nombreDeClase + "(" + camposFromUri + ")");
                Controller.AppendLine("\t\t{");
                Controller.AppendLine("\t\t\tRespuesta rta = new Respuesta();");
                Controller.AppendLine();
                Controller.AppendLine("\t\t\trta.Resultado = _" + nombreClasePrimeraMinuscula + SERVICE + ".recuperar" + nombreDeClase + "(" + camposClave + ");");
                Controller.AppendLine();
                Controller.AppendLine("\t\t\treturn rta;");
                Controller.AppendLine("\t\t}");
            }
            Controller.AppendLine("\t}");
            Controller.AppendLine("}"); 

            if (CHKcontrollers.Checked)
            {
                try
                {
                    if (!Directory.Exists(pathControllers))
                    {
                        Directory.CreateDirectory(pathControllers);
                    }
                    if (File.Exists(pathClaseController))
                    {
                        File.Delete(pathClaseController);
                    }

                    StreamWriter clase = new StreamWriter(pathClaseController);
                    clase.Write(Controller.ToString());
                    clase.Flush();
                    clase.Close();
                }
                catch (Exception)
                {
                } 
            }

            return Controller.ToString();
		}

		private string ArmarDto(List<DataColumn> columnas)
		{
			string nombreDeClase = TABLA;
			string espacioDeNombres = TXTespacioDeNombres.Text;

			StringBuilder Dto = new StringBuilder();
			StringBuilder newDto = new StringBuilder();

			Dto.AppendLine("using System;");
			Dto.AppendLine("using System.Collections.Generic;");
			Dto.AppendLine("using System.Linq;");
			Dto.AppendLine("using System.Web;");
            Dto.AppendLine("using Newtonsoft.Json;");
            Dto.AppendLine("using System.ComponentModel.DataAnnotations;");
            Dto.AppendLine("using " + espacioDeNombres + "." + MODEL + ";");
            Dto.AppendLine("");
            Dto.AppendLine("namespace " + espacioDeNombres + "." + DTO);
            Dto.AppendLine("{");
			Dto.AppendLine("\tpublic class " + nombreDeClase + DTO);
			Dto.AppendLine("\t{");

			newDto.AppendLine("\t\tpublic " + nombreDeClase + DTO + " new" + nombreDeClase + DTO + "(" + nombreDeClase + (RDBsql.Checked ? string.Empty : MODEL) + " modelo)");
			newDto.AppendLine("\t\t{");

			int i = 0;
            foreach (DataColumn columna in columnas)
            {
				//Si es un array de byte en realidad es un booleano.
				if (columna.DataType.Name == "Byte[]")
				{
					Dto.AppendLine("\t\t[Required(ErrorMessage = \"- Ingrese " + columna.ColumnName + ".\")]");
					Dto.AppendLine("\t\tpublic bool " + columna.ColumnName + " { get;  set; }");
					newDto.AppendLine("\t\t\t" + columna.ColumnName + " = modelo." + columna.ColumnName + ";");
				}
				else
				{
					Dto.AppendLine("\t\t[Required(ErrorMessage = \"- Ingrese " + columna.ColumnName + ".\")]");
					Dto.AppendLine("\t\tpublic " + Tipo(columna) + " " + columna.ColumnName + " { get;  set; }");
					newDto.AppendLine("\t\t\t" + columna.ColumnName + " = modelo." + columna.ColumnName + ";");
				}

				i++;
				if (i < columnas.Count)
				{
					Dto.AppendLine();
                }
			}

			Dto.AppendLine();
            Dto.AppendLine(newDto.ToString());
			Dto.AppendLine("\t\t\treturn this;");
			Dto.AppendLine("\t\t}");
			Dto.AppendLine("\t}");
			Dto.AppendLine("}");

            if (CHKdto.Checked)
            {
                try
                {
                    if (!Directory.Exists(pathDto))
                    {
                        Directory.CreateDirectory(pathDto);
                    }
                    if (File.Exists(pathClaseDto))
                    {
                        File.Delete(pathClaseDto);
                    }

                    StreamWriter clase = new StreamWriter(pathClaseDto);
                    clase.Write(Dto.ToString());
                    clase.Flush();
                    clase.Close();
                }
                catch (Exception)
                {
                } 
            }

            return Dto.ToString();
		}

		private string ArmarModel(List<DataColumn> columnas, List<DataColumn> claves)
		{
			string nombreDeClase = TABLA;
			string espacioDeNombres = TXTespacioDeNombres.Text;

			StringBuilder Modelo = new StringBuilder();

			Modelo.AppendLine("using System;");
			Modelo.AppendLine("using System.Collections.Generic;");
			Modelo.AppendLine("using System.Linq;");
			Modelo.AppendLine("using System.Web;");
            Modelo.AppendLine("using System.ComponentModel.DataAnnotations;");
            Modelo.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
            Modelo.AppendLine("");
			Modelo.AppendLine("namespace " + espacioDeNombres + "." + MODEL);
			Modelo.AppendLine("{");
			Modelo.AppendLine("\tpublic class " + nombreDeClase + MODEL);
			Modelo.AppendLine("\t{");

			int i = 0;
			int j = 0;
            foreach (DataColumn columna in columnas)
            {
				if (claves.Count > j && claves[j].ColumnName == columna.ColumnName)
				{
					Modelo.AppendLine("\t\t[Key]");
					Modelo.AppendLine("\t\t[Column(Order = " + j.ToString() + ")]");
					j++;
				}
				//Si es un array de byte en realidad es un booleano.
				if (columna.DataType.Name == "Byte[]")
				{
					Modelo.AppendLine("\t\tpublic bool " + columna.ColumnName + " { get;  set; }");
				}
				else
				{
					Modelo.AppendLine("\t\tpublic " + Tipo(columna) + " " + columna.ColumnName + " { get;  set; }");
				}

				i++;
				if (i < columnas.Count)
				{
					Modelo.AppendLine();
                }
			}

			Modelo.AppendLine("\t}");
			Modelo.AppendLine("}");
			Modelo.AppendLine("\r\n");

            if (CHKmodel.Checked)
            {
                try
                {
                    if (!Directory.Exists(pathModel))
                    {
                        Directory.CreateDirectory(pathModel);
                    }
                    if (File.Exists(pathClaseModel))
                    {
                        File.Delete(pathClaseModel);
                    }

                    StreamWriter clase = new StreamWriter(pathClaseModel);
                    clase.Write(Modelo.ToString());
                    clase.Flush();
                    clase.Close();
                }
                catch (Exception)
                {
                } 
            }

            return Modelo.ToString();
		}

		private string ArmarRepositories(List<DataColumn> columnas, List<DataColumn> claves)
		{
            bool DB2 = RDBdb2.Checked;
            string origen = DB2 ? MODEL : string.Empty;
            string nombreDeClase = TABLA;
            string tipoClase = TABLA + origen;
			string nombreClasePrimeraMinuscula = nombreDeClase[0].ToString().ToLower() + nombreDeClase.Substring(1) + MODEL;
			string espacioDeNombres = TXTespacioDeNombres.Text;
			List<string> camposConsulta = (from c in columnas select c.ColumnName).ToList();
			string columnasClave = string.Join(", ", (from c in claves select Tipo(c) + " " + c.ColumnName).ToList());
			List<string[]> clavesConsulta = (from c in claves select new string[] { c.ColumnName, Tipo(c)}).ToList();
            string[] partes = espacioDeNombres.Trim().Length > 0 ? espacioDeNombres.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { };
            string espacio = partes.Length > 0 ? partes[partes.Length - 1] : string.Empty;

            StringBuilder Repositories = new StringBuilder();

            Repositories.AppendLine("using System;");
			Repositories.AppendLine("using System.Collections.Generic;");
            Repositories.AppendLine("using System.Data.Entity;");
			Repositories.AppendLine("using System.Data.Odbc;");
            Repositories.AppendLine("using System.Linq;");
			Repositories.AppendLine("using SistemaMunicipalGeneral.Controles;");
            if (espacioDeNombres.Trim().Length > 0) Repositories.AppendLine("using " + espacioDeNombres + "." + MODEL + ";");
			Repositories.AppendLine();
            Repositories.AppendLine("namespace " + espacioDeNombres + ".Repositories");
			Repositories.AppendLine("{");
			    Repositories.AppendLine("\tpublic class " + nombreDeClase + "Repositories : " + nombreDeClase + REPOSITORIES_INTERFACE);
			    Repositories.AppendLine("\t{");
                //ALTA
                if (CHKalta.Checked)
                {
                    if (DB2)
                    {
                        Repositories.AppendLine("\t\tpublic (string, bool) alta" + nombreDeClase + "(" + tipoClase + " " + nombreClasePrimeraMinuscula + ")");
                        Repositories.AppendLine("\t\t{");
                        Repositories.AppendLine("\t\t\ttry");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\tComandoDB2 SQLconsulta = new ComandoDB2(string.Empty, \"DB2_Tributos\");");
                        Repositories.AppendLine("\t\t\t\tSQLconsulta.Consulta = \"INSERT INTO " + nombreDeClase + " (" + string.Join(", ", (from c in columnas select c.ColumnName).ToList()) + ") VALUES (" + string.Join(",", Enumerable.Repeat("?", columnas.Count)) + ")\";");
                        Repositories.AppendLine();
                        foreach (DataColumn c in columnas)
                        {
                            Repositories.AppendLine("\t\t\t\tSQLconsulta.Agregar(\"@" + c.ColumnName + "\", " + Mapeo[Tipo(c)] + ", " + nombreClasePrimeraMinuscula + "." + c.ColumnName + ");");
                        }
                        Repositories.AppendLine();
                        if (CHKtryOrIf.Checked)
                        {
                            Repositories.AppendLine("\t\t\t\tSQLconsulta.Ejecutar(true);");
                            Repositories.AppendLine("\t\t\t\treturn (\"Alta correcta de " + nombreDeClase + "\", true);");
                        }
                        else
                        {
                            Repositories.AppendLine("\t\t\t\tif(SQLconsulta.EjecutarNonQuery(true) > -1)");
                            Repositories.AppendLine("\t\t\t\t{");
                            Repositories.AppendLine("\t\t\t\t\treturn (\"Alta correcta de " + nombreDeClase + "\", true);");
                            Repositories.AppendLine("\t\t\t\t}");
                            Repositories.AppendLine("\t\t\t\telse");
                            Repositories.AppendLine("\t\t\t\t{");
                            Repositories.AppendLine("\t\t\t\t\treturn (\"Ocurrió un error inesperado al intentar insertar " + nombreDeClase + "\", false);");
                            Repositories.AppendLine("\t\t\t\t}");
                        }
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\treturn (\"Ocurrió un error inesperado al intentar insertar " + nombreDeClase + ". \" + ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message, false);");
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t}");
                    }
                    else
                    {
                        Repositories.AppendLine("\t\tpublic (string, bool) alta" + nombreDeClase + "(" + tipoClase + " " + nombreClasePrimeraMinuscula + ")");
                        Repositories.AppendLine("\t\t{");
                        Repositories.AppendLine("\t\t\ttry");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades." + nombreDeClase + ".Attach(" + nombreClasePrimeraMinuscula + ");");
                        Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades.Entry(" + nombreClasePrimeraMinuscula + ").State = EntityState.Added;");
                        Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades.SaveChanges();");
                        Repositories.AppendLine();
                        Repositories.AppendLine("\t\t\t\treturn (\"Alta correcta de " + nombreDeClase + "\", true);");
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\treturn (ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message, false);");
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t}");
                    }
                    Repositories.AppendLine(); 
                }
                //BAJA
                if (CHKbaja.Checked)
                {
                    if (DB2)
                    {
                        //Repositories.AppendLine("\t\tpublic (string, bool) baja" + nombreDeClase + "(" + columnasClave + ", " + tipoClase + " " + nombreClasePrimeraMinuscula + ")");
                        Repositories.AppendLine("\t\tpublic (string, bool) baja" + nombreDeClase + "(" + tipoClase + " " + nombreClasePrimeraMinuscula + ")");
                        Repositories.AppendLine("\t\t{");
                        Repositories.AppendLine("\t\t\ttry");
                        Repositories.AppendLine("\t\t\t{");
                        List<DataColumn> columnasUpdate = (from c in columnas where !claves.Contains(c) select c).ToList();
                        Repositories.AppendLine("\t\t\t\tComandoDB2 SQLconsulta = new ComandoDB2(string.Empty, \"DB2_Tributos\");");
                        Repositories.AppendLine("\t\t\t\tSQLconsulta.Consulta = \"UPDATE " + TABLA + " SET " + string.Join(" AND ", (from c in columnasUpdate select c.ColumnName + " = ?").ToList()) + "\" +");
                        Repositories.AppendLine("\t\t\t\t\t\" WHERE " + string.Join(" AND ", (from c in claves select c.ColumnName + " = ?").ToList()) + "\";");
                        Repositories.AppendLine();
                        Repositories.AppendLine("\t\t\t\t// ***** UPDATE *****");

                        foreach (DataColumn c in columnasUpdate)
                        {
                            Repositories.AppendLine("\t\t\t\tSQLconsulta.Agregar(\"@" + c.ColumnName + "\", " + Mapeo[Tipo(c)] + ", " + nombreClasePrimeraMinuscula + "." + c.ColumnName + ");");
                        }
                        Repositories.AppendLine();
                        Repositories.AppendLine("\t\t\t\t// ***** WHERE *****");
                        foreach (DataColumn c in claves)
                        {
                            Repositories.AppendLine("\t\t\t\tSQLconsulta.Agregar(\"@" + c.ColumnName + "\", " + Mapeo[Tipo(c)] + ", " + nombreClasePrimeraMinuscula + "." + c.ColumnName + ");");
                        }

                        Repositories.AppendLine();

                        if (CHKtryOrIf.Checked)
                        {
                            Repositories.AppendLine("\t\t\t\tSQLconsulta.Ejecutar(true);");
                            Repositories.AppendLine("\t\t\t\treturn (\"Eliminación correcta de " + nombreDeClase + "\", true);");
                        }
                        else
                        {
                            Repositories.AppendLine("\t\t\t\tif(SQLconsulta.EjecutarNonQuery(true) > -1)");
                            Repositories.AppendLine("\t\t\t\t{");
                            Repositories.AppendLine("\t\t\t\t\treturn (\"Eliminación correcta de " + nombreDeClase + "\", true);");
                            Repositories.AppendLine("\t\t\t\t}");
                            Repositories.AppendLine("\t\t\t\telse");
                            Repositories.AppendLine("\t\t\t\t{");
                            Repositories.AppendLine("\t\t\t\t\treturn (\"Ocurrió un error inesperado al intentar eliminar " + nombreDeClase + "\", false);");
                            Repositories.AppendLine("\t\t\t\t}");
                        }
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\treturn (\"Ocurrió un error inesperado al intentar eliminar " + nombreDeClase + ". \" + ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message, false);");
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t}");
                    }
                    else
                    {
                        Repositories.AppendLine("\t\tpublic (string, bool) baja" + nombreDeClase + "(" + tipoClase + " " + nombreClasePrimeraMinuscula + ")");
                        Repositories.AppendLine("\t\t{");
                        Repositories.AppendLine("\t\t\ttry");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades." + nombreDeClase + ".Attach(" + nombreClasePrimeraMinuscula + ");");
                        Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades.Entry(" + nombreClasePrimeraMinuscula + ").State = EntityState.Modified;");
                        Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades.SaveChanges();");
                        Repositories.AppendLine();
                        Repositories.AppendLine("\t\t\t\treturn (\"Eliminación correcta de " + nombreDeClase + "\", true);");
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\treturn (ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message, false);");
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t}");
                    }
                    Repositories.AppendLine(); 
                }
                //MODIFICAR
                if (CHKmodificacion.Checked)
                {
                    if (DB2)
                    {
                        bool where = clavesConsulta.Count > 0 || generarDesdeConsulta;
                        if (where)
                        {
                            //Repositories.AppendLine("\t\tpublic (string, bool) modificacion" + nombreDeClase + "(" + columnasClave + ", " + tipoClase + " " + nombreClasePrimeraMinuscula + ")");
                            Repositories.AppendLine("\t\tpublic (string, bool) modificacion" + nombreDeClase + "(" + tipoClase + " " + nombreClasePrimeraMinuscula + ")");
                            Repositories.AppendLine("\t\t{");
                            Repositories.AppendLine("\t\t\ttry");
                            Repositories.AppendLine("\t\t\t{");
                            List<DataColumn> columnasUpdate = (from c in columnas where !claves.Contains(c) select c).ToList();
                            Repositories.AppendLine("\t\t\t\tComandoDB2 SQLconsulta = new ComandoDB2(string.Empty, \"DB2_Tributos\");");
                            Repositories.AppendLine("\t\t\t\tSQLconsulta.Consulta = \"UPDATE " + TABLA + " SET " + string.Join(" AND ", (from c in columnasUpdate select c.ColumnName + " = ?").ToList()) + "\" +");
                            Repositories.AppendLine("\t\t\t\t\t\" WHERE " + string.Join(" AND ", (from c in claves select c.ColumnName + " = ?").ToList()) + "\";");
                            Repositories.AppendLine();
                            Repositories.AppendLine("\t\t\t\t// ***** UPDATE *****");

                            foreach (DataColumn c in columnasUpdate)
                            {
                                Repositories.AppendLine("\t\t\t\tSQLconsulta.Agregar(\"@" + c.ColumnName + "\", " + Mapeo[Tipo(c)] + ", " + nombreClasePrimeraMinuscula + "." + c.ColumnName + ");");
                            }
                            Repositories.AppendLine();
                            Repositories.AppendLine("\t\t\t\t// ***** WHERE *****");
                            foreach (DataColumn c in claves)
                            {
                                Repositories.AppendLine("\t\t\t\tSQLconsulta.Agregar(\"@" + c.ColumnName + "\", " + Mapeo[Tipo(c)] + ", " + nombreClasePrimeraMinuscula + "." + c.ColumnName + ");");
                            }

                            if (CHKtryOrIf.Checked)
                            {
                                Repositories.AppendLine("\t\t\t\tSQLconsulta.Ejecutar(true);");
                                Repositories.AppendLine("\t\t\t\treturn (\"Modificación correcta de " + nombreDeClase + "\", true);");
                            }
                            else
                            {
                                Repositories.AppendLine("\t\t\t\tif(SQLconsulta.EjecutarNonQuery(true) > -1)");
                                Repositories.AppendLine("\t\t\t\t{");
                                Repositories.AppendLine("\t\t\t\t\treturn (\"Modificación correcta de " + nombreDeClase + "\", true);");
                                Repositories.AppendLine("\t\t\t\t}");
                                Repositories.AppendLine("\t\t\t\telse");
                                Repositories.AppendLine("\t\t\t\t{");
                                Repositories.AppendLine("\t\t\t\t\treturn (\"Ocurrió un error inesperado al intentar modificar " + nombreDeClase + "\", false);");
                                Repositories.AppendLine("\t\t\t\t}");
                            }
                            Repositories.AppendLine("\t\t\t}");
                            Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                            Repositories.AppendLine("\t\t\t{");
                            Repositories.AppendLine("\t\t\t\treturn (\"Ocurrió un error inesperado al intentar modificar " + nombreDeClase + ". \" + ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message, false);");
                            Repositories.AppendLine("\t\t\t}");
                            Repositories.AppendLine("\t\t}");
                        }
                    }
                    else
                    {
                        Repositories.AppendLine("\t\tpublic (string, bool) modificacion" + nombreDeClase + "(" + tipoClase + " " + nombreClasePrimeraMinuscula + ")");
                        Repositories.AppendLine("\t\t{");
                        Repositories.AppendLine("\t\t\ttry");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades." + nombreDeClase + ".Attach(" + nombreClasePrimeraMinuscula + ");");
                        Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades.Entry(" + nombreClasePrimeraMinuscula + ").State = EntityState.Modified;");
                        Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades.SaveChanges();");
                        Repositories.AppendLine();
                        Repositories.AppendLine("\t\t\t\treturn (\"Modificación correcta de " + nombreDeClase + "\", true);");
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\treturn (ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message, false);");
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t}");
                    }
                    Repositories.AppendLine(); 
                }
                //OBTENER POR ID
                if (CHKobtenerPorId.Checked)
                {
                    if (DB2)
                    {
                        Repositories.AppendLine("\t\tpublic " + tipoClase + " obtenerPorId(" + columnasClave + ")");
                        Repositories.AppendLine("\t\t{");
                        Repositories.AppendLine("\t\t\t" + tipoClase + " Resultado = new " + tipoClase + "();");
                        Repositories.AppendLine();
                        Repositories.AppendLine("\t\t\ttry");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\tComandoDB2 SQLconsulta = new ComandoDB2(string.Empty, \"DB2_Tributos\");");
                        Repositories.AppendLine();

                        var campoBaja = camposConsulta.Where(c => c.ToLower().Contains("baja") && c.ToLower().StartsWith("f")).FirstOrDefault();

                        bool where = campoBaja != null;
                        if (!where) where = clavesConsulta.Count > 0;
                        Repositories.AppendLine("\t\t\t\tSQLconsulta.Consulta = \"SELECT " + string.Join(", ", camposConsulta.ToArray()) + " FROM " + TABLA + "\" +");
                        Repositories.AppendLine("\t\t\t\t\t\"" + (where ? (" WHERE " + string.Join(" AND ", (from c in claves select c.ColumnName + " = ?").ToList()) + (campoBaja != null ? " AND " + campoBaja + " = ?" : string.Empty) + "\";") : string.Empty));
                        Repositories.AppendLine();

                        foreach (string[] clave in clavesConsulta)
                        {
                            Repositories.AppendLine("\t\t\t\tSQLconsulta.Agregar(\"@" + clave[0] + "\", " + Mapeo[clave[1]] + ", " + clave[0] + ");");
                        }
                        if (campoBaja != null)
                        {
                            Repositories.AppendLine("\t\t\t\tSQLconsulta.Agregar(\"@" + campoBaja + "\", OdbcType.DateTime, \"1900 - 01 - 01\");");
                        }
                        Repositories.AppendLine();
                        Repositories.AppendLine("\t\t\t\tif (SQLconsulta.HayRegistros())");
                        Repositories.AppendLine("\t\t\t\t{");
                        string instancia = char.ToLower(nombreDeClase[0]) + tipoClase.Substring(1);
                        Repositories.AppendLine("\t\t\t\t\t" + tipoClase + " " + instancia + " = new " + tipoClase + "();");
                        Repositories.AppendLine("\t\t\t\t\t" + tipoClase + " instancia = FuncionesGenerales.RellenarCampos(SQLconsulta, " + instancia + ") as " + tipoClase + ";");
                        Repositories.AppendLine("\t\t\t\t};");
                        Repositories.AppendLine();
                        Repositories.AppendLine("\t\t\t\tSQLconsulta.CerrarConexion();");
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\tthrow new Exception (\"Ocurrió un error inesperado al intentar recuperar " + nombreDeClase + ". \" + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message));");
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine();
                        Repositories.AppendLine("\t\t\treturn Resultado;");
                        Repositories.AppendLine("\t\t}");
                    }
                    else
                    {
                        Repositories.AppendLine("\t\tpublic " + tipoClase + " obtenerPorId(" + columnasClave + ")");
                        Repositories.AppendLine("\t\t{");
                        Repositories.AppendLine("\t\t\t" + tipoClase + " solicitado = null;");
                        Repositories.AppendLine("\t\t\ttry");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\tsolicitado = (from busqueda in BaseDeDatos" + espacio + "." + espacio + "Entidades." + nombreDeClase);
                        Repositories.AppendLine("\t\t\t\t\t\t\t  where " + string.Join(" && ", (from c in claves select "busqueda." + c.ColumnName + " == " + c.ColumnName).ToList()));
                        Repositories.AppendLine("\t\t\t\t\t\t\t  select busqueda).FirstOrDefault();");
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\tex.ToString();");
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t\treturn solicitado;");
                        Repositories.AppendLine("\t\t}");
                    }
                    Repositories.AppendLine(); 
                }
                //TODOS
                if (CHKtodos.Checked)
                {
                    if (DB2)
                    {
                        Repositories.AppendLine("\t\tpublic List<" + tipoClase + "> obtenerTodos()");
                        Repositories.AppendLine("\t\t{");
                        Repositories.AppendLine("\t\t\tList<" + tipoClase + "> todos = new List<" + tipoClase + ">();");
                        Repositories.AppendLine();
                        Repositories.AppendLine("\t\t\ttry");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\tComandoDB2 SQLconsulta = new ComandoDB2(\"\", \"DB2_Tributos\");");
                        Repositories.AppendLine();

                        Repositories.AppendLine("\t\t\t\tSQLconsulta.Consulta = \"SELECT " + string.Join(", ", camposConsulta.ToArray()) + " FROM " + TABLA + "\";");

                        Repositories.AppendLine();
                        Repositories.AppendLine("\t\t\t\twhile (SQLconsulta.HayRegistros())");
                        Repositories.AppendLine("\t\t\t\t{");
                        string instancia = char.ToLower(nombreDeClase[0]) + nombreDeClase.Substring(1);
                        Repositories.AppendLine("\t\t\t\t\t" + tipoClase + " " + instancia + " = new " + tipoClase + "();");
                        Repositories.AppendLine("\t\t\t\t\t" + tipoClase + " instancia = FuncionesGenerales.RellenarCampos(SQLconsulta, " + instancia + ") as " + tipoClase + ";");
                        Repositories.AppendLine("\t\t\t\t\ttodos.Add(instancia);");
                        Repositories.AppendLine("\t\t\t\t}");
                        Repositories.AppendLine("\t\t\t\tSQLconsulta.CerrarConexion();");
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\tthrow new Exception (ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);");
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine();
                        Repositories.AppendLine("\t\t\treturn todos;");
                        Repositories.AppendLine("\t\t}");
                    }
                    else
                    {
                        Repositories.AppendLine("\t\tpublic List<" + tipoClase + "> obtenerTodos()");
                        Repositories.AppendLine("\t\t{");
                        Repositories.AppendLine("\t\t\treturn (from busqueda in BaseDeDatos" + espacio + "." + espacio + "Entidades." + nombreDeClase + "");
                        Repositories.AppendLine("\t\t\t\t\tselect busqueda).ToList();");
                        Repositories.AppendLine("\t\t}");
                    }
                    Repositories.AppendLine(); 
                }
                // RECUPERAR
                if (CHKrecuperacion.Checked)
                {
                    if (DB2)
                    {
                        Repositories.AppendLine("\t\tpublic (string, bool) recuperar" + nombreDeClase + "(" + tipoClase + " " + nombreClasePrimeraMinuscula + ")");
                        Repositories.AppendLine("\t\t{");
                        Repositories.AppendLine("\t\t\ttry");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\tComandoDB2 SQLconsulta = new ComandoDB2(string.Empty, \"DB2_Tributos\");");
                        Repositories.AppendLine("\t\t\t\tSQLconsulta.Consulta = \"INSERT INTO " + TABLA + " (" + string.Join(", ", (from c in columnas select c.ColumnName).ToList()) + ") VALUES (" + string.Join(",", Enumerable.Repeat("?", columnas.Count)) + ")\";");
                        Repositories.AppendLine();
                        foreach (DataColumn c in columnas)
                        {
                            Repositories.AppendLine("\t\t\t\tSQLconsulta.Agregar(\"@" + c.ColumnName + "\", " + Mapeo[Tipo(c)] + ", " + nombreClasePrimeraMinuscula + "." + c.ColumnName + ");");
                        }
                        Repositories.AppendLine();
                        if (CHKtryOrIf.Checked)
                        {
                            Repositories.AppendLine("\t\t\t\tSQLconsulta.Ejecutar(true);");
                            Repositories.AppendLine("\t\t\t\treturn (\"Recuperación correcta de " + nombreDeClase + "\", true);");
                        }
                        else
                        {
                            Repositories.AppendLine("\t\t\t\tif(SQLconsulta.EjecutarNonQuery(true) > -1)");
                            Repositories.AppendLine("\t\t\t\t{");
                            Repositories.AppendLine("\t\t\t\t\treturn (\"Recuperación correcta de " + nombreDeClase + "\", true);");
                            Repositories.AppendLine("\t\t\t\t}");
                            Repositories.AppendLine("\t\t\t\telse");
                            Repositories.AppendLine("\t\t\t\t{");
                            Repositories.AppendLine("\t\t\t\t\treturn (\"Ocurrió un error inesperado al intentar recuperar " + nombreDeClase + "\", false);");
                            Repositories.AppendLine("\t\t\t\t}");
                        }
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\treturn (\"Ocurrió un error inesperado al intentar recuperar " + nombreDeClase + ". \" + ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message, false);");
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t}");
                    }
                    else
                    {
                        Repositories.AppendLine("\t\tpublic (string, bool) recuperar" + nombreDeClase + "(" + tipoClase + " " + nombreClasePrimeraMinuscula + ")");
                        Repositories.AppendLine("\t\t{");
                        Repositories.AppendLine("\t\t\ttry");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades." + nombreDeClase + ".Attach(" + nombreClasePrimeraMinuscula + ");");
                        Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades.Entry(" + nombreClasePrimeraMinuscula + ").State = EntityState.Modified;");
                        Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades.SaveChanges();");
                        Repositories.AppendLine();
                        Repositories.AppendLine("\t\t\t\treturn (\"Recuperación correcta de " + nombreDeClase + "\", true);");
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine("\t\t\t\treturn (ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message, false);");
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t}");
                    } 
                }
			    Repositories.AppendLine("\t}");
			Repositories.AppendLine("}");

            if (CHKrepositories.Checked)
            {
                try
                {
                    if (!Directory.Exists(pathRepositories))
                    {
                        Directory.CreateDirectory(pathRepositories);
                    }
                    if (File.Exists(pathClaseRepositories))
                    {
                        File.Delete(pathClaseRepositories);
                    }

                    StreamWriter clase = new StreamWriter(pathClaseRepositories);
                    clase.Write(Repositories.ToString());
                    clase.Flush();
                    clase.Close();
                }
                catch (Exception)
                {
                }

            }
            return Repositories.ToString();
		}

        private string ArmarRepositoriesInterface(List<DataColumn> claves)
		{
            bool DB2 = RDBdb2.Checked;
            string origen = DB2 ? MODEL : string.Empty;
            string nombreDeClase = TABLA;
            string tipoClase = TABLA + origen;
            string nombreClasePrimeraMinuscula = nombreDeClase[0].ToString().ToLower() + nombreDeClase.Substring(1);
			string espacioDeNombres = TXTespacioDeNombres.Text;
            string columnasClave = string.Join(", ", (from c in claves select Tipo(c) + " " + c.ColumnName).ToList());

            StringBuilder RepositoriesInterface = new StringBuilder();

			RepositoriesInterface.AppendLine("using SistemaMunicipalGeneral.Controles;");
			RepositoriesInterface.AppendLine("using System.Collections.Generic;");
			RepositoriesInterface.AppendLine("using System.Data.Odbc;");
            if (espacioDeNombres.Trim().Length > 0) RepositoriesInterface.AppendLine("using " + espacioDeNombres + "." + MODEL + ";");
			RepositoriesInterface.AppendLine();
			RepositoriesInterface.AppendLine("namespace " + espacioDeNombres + "." + REPOSITORIES);
			RepositoriesInterface.AppendLine("{");
			RepositoriesInterface.AppendLine("\tpublic interface " + nombreDeClase + REPOSITORIES_INTERFACE);
			RepositoriesInterface.AppendLine("\t{");
            if (CHKalta.Checked)
            {
                RepositoriesInterface.AppendLine("\t\t(string, bool) alta" + nombreDeClase + "(" + tipoClase + " " + nombreClasePrimeraMinuscula + MODEL + ");");
                RepositoriesInterface.AppendLine(); 
            }
            if (CHKbaja.Checked)
            {
                RepositoriesInterface.AppendLine("\t\t(string, bool) baja" + nombreDeClase + "(" + tipoClase + " " + nombreClasePrimeraMinuscula + MODEL + ");");
                RepositoriesInterface.AppendLine(); 
            }
            if (CHKmodificacion.Checked)
            {
                RepositoriesInterface.AppendLine("\t\t(string, bool) modificacion" + nombreDeClase + "(" + tipoClase + " " + nombreClasePrimeraMinuscula + MODEL + ");");
                RepositoriesInterface.AppendLine(); 
            }
            if (CHKobtenerPorId.Checked)
            {
                RepositoriesInterface.AppendLine("\t\t" + tipoClase + " obtenerPorId(" + columnasClave + ");");
                RepositoriesInterface.AppendLine(); 
            }
            if (CHKtodos.Checked)
            {
                RepositoriesInterface.AppendLine("\t\tList <" + tipoClase + "> obtenerTodos();");
                RepositoriesInterface.AppendLine(); 
            }
            if (CHKrecuperacion.Checked)
            {
                RepositoriesInterface.AppendLine("\t\t(string, bool) recuperar" + nombreDeClase + "(" + tipoClase + " " + nombreClasePrimeraMinuscula + MODEL + ");"); 
            }
			RepositoriesInterface.AppendLine("\t}");
			RepositoriesInterface.AppendLine("}");

            if (CHKrepositories.Checked)
            {
                try
                {
                    if (!Directory.Exists(pathRepositories))
                    {
                        Directory.CreateDirectory(pathRepositories);
                    }
                    if (File.Exists(pathClaseRepositoriesInterface))
                    {
                        File.Delete(pathClaseRepositoriesInterface);
                    }

                    StreamWriter clase = new StreamWriter(pathClaseRepositoriesInterface);
                    clase.Write(RepositoriesInterface.ToString());
                    clase.Flush();
                    clase.Close();
                }
                catch (Exception)
                {
                } 
            }

            return RepositoriesInterface.ToString();
		}

		private string ArmarService(List<DataColumn> columnas, List<DataColumn> claves)
		{
            bool DB2 = RDBdb2.Checked;
            string origen = DB2 ? MODEL : DTO;
            string nombreDeClase = TABLA;
            string tipoClase = TABLA + origen;
            string nombreClasePrimeraMinuscula = nombreDeClase[0].ToString().ToLower() + nombreDeClase.Substring(1) + origen;
			string espacioDeNombres = TXTespacioDeNombres.Text;
            string columnasClave = string.Join(", ", (from c in claves select c.ColumnName).ToList());
            string columnasClaveTipo = string.Join(", ", (from c in claves select Tipo(c) + " " + c.ColumnName).ToList());

            StringBuilder Service = new StringBuilder();

			Service.AppendLine("using System;");
			Service.AppendLine("using System.Collections.Generic;");
            Service.AppendLine("using SistemaMunicipalGeneral;");
			Service.AppendLine("using System.Linq;");
			Service.AppendLine("using System.Web;");
            if (espacioDeNombres.Trim().Length > 0) Service.AppendLine("using " + espacioDeNombres + "." + origen + ";");
            if (espacioDeNombres.Trim().Length > 0) Service.AppendLine("using " + espacioDeNombres + "." + REPOSITORIES + ";");
			Service.AppendLine();
            Service.AppendLine("namespace " + espacioDeNombres + "." + SERVICE);
			Service.AppendLine("{");
			    Service.AppendLine("\tpublic class " + nombreDeClase + SERVICE + " : " + nombreDeClase + SERVICE_INTERFACE);
			    Service.AppendLine("\t{");
			        Service.AppendLine("\t\tprivate readonly " + nombreDeClase + REPOSITORIES_INTERFACE + " _repositories;");
			        Service.AppendLine();

			        Service.AppendLine("\t\tpublic " + nombreDeClase + SERVICE + "(" + nombreDeClase + REPOSITORIES_INTERFACE + " repositories)");
			        Service.AppendLine("\t\t{");
			            Service.AppendLine("\t\t\t_repositories = repositories;");
			        Service.AppendLine("\t\t}");
                    Service.AppendLine();
                    //ALTA
                    if (CHKalta.Checked)
                    {
                        Service.AppendLine("\t\tpublic (string, bool) alta" + nombreDeClase + "(" + tipoClase + " " + nombreClasePrimeraMinuscula + ")");
                        Service.AppendLine("\t\t{");
                        Service.AppendLine("\t\t\t" + nombreDeClase + (DB2 ? origen : string.Empty) + " nuevo = new " + nombreDeClase + (DB2 ? origen : string.Empty) + "()");
                        Service.AppendLine("\t\t\t{");
                        int i = 0;
                        foreach (DataColumn columna in columnas)
                        {
                            Service.AppendLine("\t\t\t\t" + columna.ColumnName + " = " + nombreClasePrimeraMinuscula + "." + columna.ColumnName + (i < columnas.Count ? "," : string.Empty));
                            i++;
                        }
                        Service.AppendLine("\t\t\t};");
                        Service.AppendLine("\t\t\t(string, bool) respuesta = _repositories.alta" + nombreDeClase + "(nuevo);");
                        Service.AppendLine();
                        Service.AppendLine("\t\t\treturn respuesta;");
                        Service.AppendLine("\t\t}");
                        Service.AppendLine(); 
                    }
                    //BAJA
                    if (CHKbaja.Checked)
                    {
                        Service.AppendLine("\t\tpublic (string, bool) baja" + nombreDeClase + "(" + columnasClaveTipo + ", int codigoBaja, string motivoBaja)");
                        Service.AppendLine("\t\t{");
                        Service.AppendLine("\t\t\t" + nombreDeClase + (DB2 ? origen : string.Empty) + " solicitado = _repositories.obtenerPorId(" + columnasClave + ");");
                        Service.AppendLine("\t\t\tif (solicitado != null)");
                        Service.AppendLine("\t\t\t{");
                        if (DGVbaja.Rows.Count == 0)
                        {
                            Service.AppendLine("\t\t\t\tsolicitado.FechaBaja = System.DateTime.Now;");
                            Service.AppendLine("\t\t\t\tsolicitado.UsuarioBaja = Config.UsuarioMagic;");
                            Service.AppendLine("\t\t\t\tsolicitado.CodigoBaja = codigoBaja;");
                            Service.AppendLine("\t\t\t\tsolicitado.MotivoBaja = motivoBaja;");
                        }
                        else
                        {
                            foreach (DataGridViewRow item in DGVbaja.Rows)
                            {
                                Service.AppendLine("\t\t\t\tsolicitado." + item.Cells[0].FormattedValue + " = " + item.Cells[1].Value);
                            }
                        }
                        Service.AppendLine("\t\t\t}");
                        Service.AppendLine("\t\t\t(string, bool) respuesta = _repositories.baja" + nombreDeClase + "(solicitado);");
                        Service.AppendLine();
                        Service.AppendLine("\t\t\treturn respuesta;");
                        Service.AppendLine("\t\t}");
                        Service.AppendLine(); 
                    }
                    //MODIFICACION
                    if (CHKmodificacion.Checked)
                    {
                        Service.AppendLine("\t\tpublic (string, bool) modificacion" + nombreDeClase + "(" + tipoClase + " " + nombreClasePrimeraMinuscula + ")");
                        Service.AppendLine("\t\t{");
                        string columnasBusqueda = string.Join(", ", (from c in claves select nombreClasePrimeraMinuscula + "." + c.ColumnName).ToList());
                        Service.AppendLine("\t\t\t" + nombreDeClase + (DB2 ? origen : string.Empty) + " solicitado = _repositories.obtenerPorId(" + columnasBusqueda + ");");
                        Service.AppendLine("\t\t\tif (solicitado != null)");
                        Service.AppendLine("\t\t\t{");
                        if (DGVmodificacion.Rows.Count == 0)
                        {
                            Service.AppendLine("\t\t\t\tsolicitado.FechaModificacion = System.DateTime.Now;");
                            Service.AppendLine("\t\t\t\tsolicitado.UsuarioModificacion = Config.UsuarioMagic;");
                        }
                        else
                        {
                            foreach (DataGridViewRow item in DGVmodificacion.Rows)
                            {
                                Service.AppendLine("\t\t\t\tsolicitado." + item.Cells[0].FormattedValue + " = " + item.Cells[1].Value);
                            }
                        }
                        Service.AppendLine("\t\t\t}");
                        Service.AppendLine("\t\t\t(string, bool) respuesta = _repositories.modificacion" + nombreDeClase + "(solicitado);");
                        Service.AppendLine();
                        Service.AppendLine("\t\t\treturn respuesta;");
                        Service.AppendLine("\t\t}");
                        Service.AppendLine(); 
                    }
                    //OBTENER POR ID
                    if (CHKobtenerPorId.Checked)
                    {
                        Service.AppendLine("\t\tpublic " + tipoClase + " obtenerPorId(" + columnasClaveTipo + ")");
                        Service.AppendLine("\t\t{");
                        Service.AppendLine("\t\t\t" + nombreDeClase + (DB2 ? origen : string.Empty) + " solicitado = _repositories.obtenerPorId(" + columnasClave + ");");
                        Service.AppendLine("\t\t\tif (solicitado != null)");
                        Service.AppendLine("\t\t\t{");
                        if (DB2)
                        {
                            Service.AppendLine("\t\t\t\treturn solicitado;");
                        }
                        else
                        {
                            Service.AppendLine("\t\t\t\t" + nombreDeClase + origen + " solicitado" + origen + " = new " + nombreDeClase + origen + "();");
                            Service.AppendLine();
                            Service.AppendLine("\t\t\t\tsolicitado" + origen + ".new" + nombreDeClase + origen + "(solicitado);");
                            Service.AppendLine("\t\t\t\treturn solicitado" + origen + ";");
                        }
                        Service.AppendLine("\t\t\t}");
                        Service.AppendLine("\t\t\telse");
                        Service.AppendLine("\t\t\t{");
                        Service.AppendLine("\t\t\t\treturn null;");
                        Service.AppendLine("\t\t\t}");
                        Service.AppendLine("\t\t}");
                        Service.AppendLine(); 
                    }
                    //TODOS
                    if (CHKtodos.Checked)
                    {
                        Service.AppendLine("\t\tpublic List<" + tipoClase + "> obtenerTodos()");
                        Service.AppendLine("\t\t{");
                        Service.AppendLine("\t\t\tList<" + nombreDeClase + (DB2 ? origen : string.Empty) + "> listado = new List<" + nombreDeClase + (DB2 ? origen : string.Empty) + ">();");
                        Service.AppendLine();
                        Service.AppendLine("\t\t\tlistado = _repositories.obtenerTodos();");
                        Service.AppendLine("\t\t\tif (listado.Count() > 0)");
                        Service.AppendLine("\t\t\t{");
                        if (DB2)
                        {
                            Service.AppendLine("\t\t\t\treturn listado;");
                        }
                        else
                        {
                            Service.AppendLine("\t\t\t\tList<" + nombreDeClase + origen + "> " + nombreClasePrimeraMinuscula + " = new List<" + nombreDeClase + origen + ">();");
                            Service.AppendLine("\t\t\t\tforeach (" + nombreDeClase + " model in listado)");
                            Service.AppendLine("\t\t\t\t{");
                            Service.AppendLine("\t\t\t\t\t" + nombreDeClase + origen + " dto = new " + nombreDeClase + origen + "();");
                            Service.AppendLine("\t\t\t\t\t" + nombreClasePrimeraMinuscula + ".Add(dto.new" + nombreDeClase + origen + "(model));");
                            Service.AppendLine("\t\t\t\t}");
                            Service.AppendLine("\t\t\t\treturn " + nombreClasePrimeraMinuscula + ";");
                        }
                        Service.AppendLine("\t\t\t}");
                        Service.AppendLine("\t\t\treturn null;");
                        Service.AppendLine("\t\t}");
                        Service.AppendLine(); 
                    }
                    //RECUPERACION
                    if (CHKrecuperacion.Checked)
                    {
                        Service.AppendLine("\t\tpublic (string, bool) recuperar" + nombreDeClase + "(" + columnasClaveTipo + ")");
                        Service.AppendLine("\t\t{");
                        Service.AppendLine("\t\t\t" + nombreDeClase + (DB2 ? origen : string.Empty) + " solicitado = _repositories.obtenerPorId(" + columnasClave + ");");
                        Service.AppendLine("\t\t\tif (solicitado != null)");
                        Service.AppendLine("\t\t\t{");
                        if (DGVrecuperacion.Rows.Count == 0)
                        {
                            Service.AppendLine("\t\t\t\tsolicitado.FechaBaja = new DateTime(1900, 1, 1);");
                            Service.AppendLine("\t\t\t\tsolicitado.UsuarioBaja = string.Empty;");
                            Service.AppendLine("\t\t\t\tsolicitado.CodigoBaja = 0;");
                            Service.AppendLine("\t\t\t\tsolicitado.MotivoBaja = string.Empty;");
                        }
                        else
                        {
                            foreach (DataGridViewRow item in DGVrecuperacion.Rows)
                            {
                                Service.AppendLine("\t\t\t\tsolicitado." + item.Cells[0].FormattedValue + " = " + item.Cells[1].Value);
                            }
                        }
                        Service.AppendLine("\t\t\t}");
                        Service.AppendLine("\t\t\t(string, bool) respuesta = _repositories.recuperar" + nombreDeClase + "(solicitado);");
                        Service.AppendLine();
                        Service.AppendLine("\t\t\treturn respuesta;");
                        Service.AppendLine("\t\t}"); 
                    }
                Service.AppendLine("\t}");
			Service.AppendLine("}");

            if (CHKservice.Checked)
            {
                try
                {
                    if (!Directory.Exists(pathService))
                    {
                        Directory.CreateDirectory(pathService);
                    }
                    if (File.Exists(pathClaseService))
                    {
                        File.Delete(pathClaseService);
                    }

                    StreamWriter clase = new StreamWriter(pathClaseService);
                    clase.Write(Service.ToString());
                    clase.Flush();
                    clase.Close();
                }
                catch (Exception)
                {
                } 
            }

            return Service.ToString();
		}

		private string ArmarServiceInterface(List<DataColumn> claves)
		{
            bool DB2 = RDBdb2.Checked;
            string origen = DB2 ? MODEL : DTO;
			string nombreDeClase = TABLA;
            string tipoClase = TABLA + origen;
			string nombreClasePrimeraMinuscula = nombreDeClase[0].ToString().ToLower() + nombreDeClase.Substring(1) + origen;
			string espacioDeNombres = TXTespacioDeNombres.Text;
			string columnasClave = string.Join(", ", (from c in claves select Tipo(c) + " " + c.ColumnName).ToList());

			StringBuilder ServiceInterface = new StringBuilder();

            ServiceInterface.AppendLine("using System;");
            ServiceInterface.AppendLine("using System.Collections.Generic;");
            ServiceInterface.AppendLine("using System.Data.Odbc;");
			ServiceInterface.AppendLine("using SistemaMunicipalGeneral.Controles;");
            if (espacioDeNombres.Trim().Length > 0) ServiceInterface.AppendLine("using " + espacioDeNombres + "." + origen + ";");
			ServiceInterface.AppendLine();
            ServiceInterface.AppendLine("namespace " + espacioDeNombres + "." + SERVICE);
			ServiceInterface.AppendLine("{");
			    ServiceInterface.AppendLine("\tpublic interface " + nombreDeClase + SERVICE_INTERFACE);
			    ServiceInterface.AppendLine("\t{");
                    if (CHKalta.Checked)
                    {
                        ServiceInterface.AppendLine("\t\t(string, bool) alta" + nombreDeClase + "(" + tipoClase + " " + nombreClasePrimeraMinuscula + ");");
                        ServiceInterface.AppendLine(); 
                    }
                    if (CHKbaja.Checked)
                    {
                        ServiceInterface.AppendLine("\t\t(string, bool) baja" + nombreDeClase + "(" + columnasClave + ", int codigoBaja, string motivoBaja);");
                        ServiceInterface.AppendLine(); 
                    }
                    if (CHKmodificacion.Checked)
                    {
                        ServiceInterface.AppendLine("\t\t(string, bool) modificacion" + nombreDeClase + "(" + tipoClase + " " + nombreClasePrimeraMinuscula + ");");
                        ServiceInterface.AppendLine(); 
                    }
                    if (CHKobtenerPorId.Checked)
                    {
                        ServiceInterface.AppendLine("\t\t" + tipoClase + " obtenerPorId(" + columnasClave + ");");
                        ServiceInterface.AppendLine(); 
                    }
                    if (CHKtodos.Checked)
                    {
                        ServiceInterface.AppendLine("\t\tList<" + tipoClase + "> obtenerTodos();");
                        ServiceInterface.AppendLine(); 
                    }
                    if (CHKrecuperacion.Checked)
                    {
                        ServiceInterface.AppendLine("\t\t(string, bool) recuperar" + nombreDeClase + "(" + columnasClave + ");"); 
                    }
			    ServiceInterface.AppendLine("\t}");
			ServiceInterface.AppendLine("}");

            if (CHKservice.Checked)
            {
                try
                {
                    if (!Directory.Exists(pathService))
                    {
                        Directory.CreateDirectory(pathService);
                    }
                    if (File.Exists(pathClaseServiceInterface))
                    {
                        File.Delete(pathClaseServiceInterface);
                    }

                    StreamWriter clase = new StreamWriter(pathClaseServiceInterface);
                    clase.Write(ServiceInterface.ToString());
                    clase.Flush();
                    clase.Close();
                }
                catch (Exception)
                {
                } 
            }

            return ServiceInterface.ToString();
		}

        private string ArmarTypeScript(List<DataColumn> columnas)
        {
            string nombreDeClase = TABLA;
            StringBuilder typeSript = new StringBuilder();


            typeSript.AppendLine("export class " + nombreDeClase + "{");
                typeSript.AppendLine("\tconstructor(init?: Partial<" + nombreDeClase + ">) {");
                    typeSript.AppendLine("\t\tObject.assign(this, init);");
                typeSript.AppendLine("\t}");

                foreach (var columna in columnas)
                {
                    typeSript.AppendLine("\tpublic " + columna.ColumnName + ": " + PropiedadesTS[columna.DataType]);
                }
            typeSript.AppendLine("}");

            if (CHKtypeScript.Checked)
            {
                try
                {
                    string pathTypeScript = TXTpathCapas.Text + @"\" + TABLA + @"\TypeScript\";
                    string pathClaseTypeScript = pathTypeScript + TABLA + ".ts";
                    if (!Directory.Exists(pathTypeScript))
                    {
                        Directory.CreateDirectory(pathTypeScript);
                    }
                    if (File.Exists(pathClaseTypeScript))
                    {
                        File.Delete(pathClaseTypeScript);
                    }

                    StreamWriter clase = new StreamWriter(pathClaseTypeScript);
                    clase.Write(typeSript.ToString());
                    clase.Flush();
                    clase.Close();
                }
                catch (Exception)
                {
                }
            }
            return typeSript.ToString();
        }

		public string Tipo(DataColumn columna)
		{
			if (columna == null || columna.DataType == null)
				return string.Empty;

			Type tipo = columna.DataType;
            if (TIPOS.Keys.Contains(tipo))
            {
                return TIPOS[tipo];
            }
            else
            {
                return ERROR;
            }
        }

        private void GenerarDesdeTabla()
        {
            if (TXTpathCapas.Text.Trim().Length > 0)
            {
                generarDesdeConsulta = false;
                GuardarConfiguracion();
            }
            else
            {
                CustomMessageBox.Show("Seleccione una carpeta donde guardar las capas!", CustomMessageBox.ATENCION, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                DefinirDirectorioCapas();
            }
        }

        private void GenerarDesdeConsulta()
        {
            if (TXTpathCapas.Text.Trim().Length > 0)
            {
                generarDesdeConsulta = true;
                List<CheckBox> capasNecesarias = new List<CheckBox>();
                if (CHKalta.Checked)
                {
                    capasNecesarias.Add(CHKalta);
                }
                if (CHKbaja.Checked)
                {
                    capasNecesarias.Add(CHKbaja);
                }
                if (CHKmodificacion.Checked)
                {
                    capasNecesarias.Add(CHKmodificacion);
                }
                if (CHKobtenerPorId.Checked)
                {
                    capasNecesarias.Add(CHKobtenerPorId);
                }
                if (CHKrecuperacion.Checked)
                {
                    capasNecesarias.Add(CHKrecuperacion);
                }
                string mensaje = string.Empty;
                if (capasNecesarias.Count > 0)
                {
                    if (capasNecesarias.Count == 1)
                    {
                        mensaje = $"Al generar desde una consulta puede que solo necesite generar el Modelo y el Dto.\r\n" +
                            $"Conviene generar el Controlador, Servicio y Repositorio para la consulta en sí.\r\n" +
                            $"Generar en estos 3 últimos el método de:\r\n   • {TextoHelper.FormatearTitulo(capasNecesarias[0].Name.Replace("CHK", string.Empty))}\r\npuede generar inconsistencias.\r\n" +
                            $"¿Desea generarlo de todos modos?";
                    }
                    else
                    {
                        string metodos = string.Join("\r\n   • ", (from c in capasNecesarias select TextoHelper.FormatearTitulo(c.Name.Replace("CHK", string.Empty))).ToArray());
                        mensaje = $"Al generar desde una consulta puede que solo necesite generar el Modelo y el Dto.\r\n" +
                            $"Conviene generar el Controlador, Servicio y Repositorio para la consulta en sí.\r\n" +
                            $"Generar en estos 3 últimos los métodos de:\r\n   • {metodos}\r\npueden generar inconsistencias.\r\n" +
                            $"¿Desea generarlos de todos modos?";
                    }
                    if (CustomMessageBox.Show(mensaje, CustomMessageBox.ATENCION, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                    {
                        foreach (CheckBox item in capasNecesarias)
                        {
                            item.Checked = false;
                        }
                    }
                }

                CamposTabla("CONSULTA", TXTgenerarAPartirDeConsulta.Text);
                GuardarConfiguracion(); 
            }
            else
            {
                CustomMessageBox.Show("Seleccione una carpeta donde guardar las capas!", CustomMessageBox.ATENCION, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                DefinirDirectorioCapas();
            }
        }

        private void CMBservidor_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (RDBdb2.Checked)
                {
                    CMBbases.Items.Clear();
                    CMBbases.Items.AddRange(new object[] { "CONTABIL", "CONTAICD", "CONTAIMV", "CONTCBEL", "CONTIDS", "DOCUMENT", "GENERAL", "GIS", "HISTABM", "HISTORIC", "INFORMAT", "LICENCIA", "RRHH", "SISUS", "TRIBUTOS" });
                }
                else
                {
                    CMBbases.Items.Clear();

                    string servidor = CMBservidor.Items[CMBservidor.SelectedIndex].ToString().ToUpper();
                    string connectionString = @"Data Source=SQL" + servidor + @"\" + servidor + "; Initial Catalog=master;Persist Security Info=True;User ID=usuario;Password=ci?r0ba;MultipleActiveResultSets=True";

                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            string query = "SELECT UPPER(name) FROM sys.databases WHERE state = 0 ORDER BY name ASC"; // Solo bases "online"

                            using (SqlCommand command = new SqlCommand(query, connection))
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    CMBbases.Items.Add(reader.GetString(0));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                if (CMBbases.Items.Count > 0)
                {
                    CMBbases.SelectedIndex = 0;
                }
                CMBbases.Refresh();
            }
            catch (Exception)
            {
            }
        }

        private void CMBbases_SelectedIndexChanged(object sender, EventArgs e)
        {
			TablasBase();
		}

        private void CMBtablas_SelectedIndexChanged(object sender, EventArgs e)
        {
			CamposTabla();
        }

        private Ejecutar EstablecerConexion()
        {
            Ejecutar datos = new Ejecutar();
            try
            {
                datos.BaseDeDatos = CMBbases.Items[CMBbases.SelectedIndex].ToString();
                datos.Servidor = CMBservidor.Items[CMBservidor.SelectedIndex].ToString();
            }
            catch (Exception)
            {
            }            
            return datos;
        }

        private void TablasBase()
        {
            try
            {
                CMBtablas.Items.Clear();
                CMBtablas.Text = string.Empty;
                LBLtablaSeleccionada.Text = string.Empty;
                LSVcampos.Items.Clear();
                tablasBase = new List<string>();
                if (RDBdb2.Checked)
                {
                    try
                    {
                        Ejecutar datos = EstablecerConexion();
                        ComandoDB2 Db2 = new ComandoDB2("SELECT LTRIM(RTRIM(NAME)) AS Nombre, COLCOUNT Columnas FROM SYSIBM.SYSTABLES WHERE TYPE = 'T' AND CREATOR = 'DB2ADMIN' ORDER BY Nombre", datos.ObtenerConexion());

                        while (Db2.HayRegistros())
                        {
                            tablasBase.Add(Db2.CampoStr("Nombre"));
                            CMBtablas.Items.Add(Db2.CampoStr("Nombre"));
                        }
                        Db2.Cerrar();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    string servidor = CMBservidor.Items[CMBservidor.SelectedIndex].ToString().ToUpper();
                    string connectionString = @"Data Source=SQL" + servidor + @"\" + servidor + "; Initial Catalog=" + CMBbases.Items[CMBbases.SelectedIndex].ToString() + ";Persist Security Info=True;User ID=usuario;Password=ci?r0ba;MultipleActiveResultSets=True";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();

                            string query = @"SELECT TABLE_SCHEMA, TABLE_NAME 
                                 FROM INFORMATION_SCHEMA.TABLES 
                                 WHERE TABLE_TYPE = 'BASE TABLE'
                                 ORDER BY TABLE_SCHEMA, TABLE_NAME";

                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string schema = reader.GetString(0);
                                    string table = reader.GetString(1);
                                    tablasBase.Add($"{schema}.{table}");
                                    CMBtablas.Items.Add($"{schema}.{table}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                LSVcampos.Refresh();
                if (CMBtablas.Items.Count > 0)
                {
                    CMBtablas.SelectedIndex = 0;
                }
                CMBtablas.Refresh();
            }
            catch (Exception)
            {
            }
        }

        private void CamposTabla(string tabla = "", string consulta = "")
		{
            camposTabla = new List<string>();
            try
            {
                LBLtablaSeleccionada.Text = (tabla.Trim().Length > 0 ? tabla : CMBtablas.Items[CMBtablas.SelectedIndex].ToString()) + ":";
                LSVcampos.Items.Clear();
                string tablaSeleccionada = (tabla.Trim().Length > 0 ? tabla : CMBtablas.Items[CMBtablas.SelectedIndex].ToString());
                // BASE DE DATOS DB2
                if (RDBdb2.Checked)
                {
                    try
                    {
                        Ejecutar datos = EstablecerConexion();
                        ComandoDB2 Db2 = null;
                        // GENERO DESDE UNA CONSULTA
                        if (consulta.Trim().Length > 0)
                        {
                            Db2 = new ComandoDB2(consulta, datos.ObtenerConexion());
                            Db2.Conexion = new System.Data.Odbc.OdbcConnection(datos.ObtenerConexion());
                            Db2.HayRegistros();

                            using (var reader = Db2.ObtenerLector())
                            {
                                CargarListViewDesdeEsquema(reader, true);
                            }
                            Db2.Cerrar();
                        }
                        else // GENERO DESDE UNA TABLA
                        {
                            Db2 = new ComandoDB2("SELECT LTRIM(RTRIM(NAME)) AS Nombre, COLTYPE as Tipo, LENGTH as Longitud, SCALE as Escala, CASE WHEN NULLS = 'N' THEN 'NO' ELSE 'SÍ' END as AceptaNulos FROM SYSIBM.SYSCOLUMNS WHERE TBNAME = '" + tablaSeleccionada + "'", datos.ObtenerConexion());
                            Db2.Conexion = new System.Data.Odbc.OdbcConnection(datos.ObtenerConexion());

                            Db2.HayRegistros();
                            OdbcDataReader reader = reader = Db2.ObtenerLector();
                            do
                            {
                                CargarListViewDesdeReader(reader);
                            }
                            while (Db2.HayRegistros());
                            Db2.Cerrar();

                            if (LSVcampos.Items.Count > 0)
                            {
                                Db2 = new ComandoDB2("SELECT UPPER(COLNAMES) AS Clave FROM SYSCAT.INDEXES WHERE TABNAME = '" + tablaSeleccionada + "' AND UNIQUERULE IN ('U')", datos.ObtenerConexion());
                                List<string> claves = new List<string>();
                                while (Db2.HayRegistros())
                                {
                                    claves.Add(Db2.CampoStr("Clave"));
                                }
                                Db2.Cerrar();

                                int minCantidad = claves.Min(s => s.Count(c => c == '+'));

                                // Paso 2: filtrar los strings con esa cantidad mínima
                                List<string> clave = claves
                                    .Where(s => s.Count(c => c == '+') == minCantidad)
                                    .Select(s => s.Split('+'))
                                    .FirstOrDefault().ToList();

                                foreach (ListViewItem item in LSVcampos.Items)
                                {
                                    if (clave.Contains(item.SubItems[0].Text))
                                    {
                                        item.Checked = true;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else // BASE DE DATOS MS SQL
                {
                    try
                    {
                        string servidor = CMBservidor.Items[CMBservidor.SelectedIndex].ToString().ToUpper();
                        string connectionString = @"Data Source=SQL" + servidor + @"\" + servidor + "; Initial Catalog=" + CMBbases.Items[CMBbases.SelectedIndex].ToString() + ";Persist Security Info=True;User ID=usuario;Password=ci?r0ba;MultipleActiveResultSets=True";

                        string query = consulta.Trim().Length > 0 ? consulta : $@"SELECT c.name AS Nombre, ty.name AS Tipo, c.max_length AS Longitud, c.scale AS Escala, CASE WHEN c.is_nullable = 1 THEN 'SÍ' ELSE 'NO' END AS AceptaNulos "
                            + "FROM sys.columns c "
                            + "JOIN sys.types ty ON c.user_type_id = ty.user_type_id "
                            + "JOIN sys.tables t ON c.object_id = t.object_id "
                            + "WHERE t.name = @tabla "
                            + "ORDER BY c.column_id;";

                        using (SqlConnection conn = new SqlConnection(connectionString))
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            if (consulta.Trim().Length == 0)
                            {
                                string[] partes = tablaSeleccionada.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                                tablaSeleccionada = partes[partes.Length - 1];

                                cmd.Parameters.AddWithValue("@tabla", tablaSeleccionada);
                            }

                            try
                            {
                                conn.Open();
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    // GENERO DESDE UNA CONSULTA
                                    if (consulta.Trim().Length > 0)
                                    {
                                        CargarListViewDesdeEsquema(reader, false);
                                    }
                                    else // GENERO DESDE UNA TABLA
                                    {
                                        while (reader.Read())
                                        {
                                            CargarListViewDesdeReader(reader);
                                        } 
                                    }
                                }
                                conn.Close();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }

                        // Seteo las claves primarias si es que contiene
                        if (LSVcampos.Items.Count > 0)
                        {
                            string[] partes = tablaSeleccionada.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                            tablaSeleccionada = partes[partes.Length - 1];
                            query = "SELECT KU.COLUMN_NAME Nombre "
                                + "FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC "
                                + "INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KU ON TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME "
                                + "WHERE TC.TABLE_NAME = @tabla AND TC.CONSTRAINT_TYPE = 'PRIMARY KEY'";

                            using (SqlConnection conn = new SqlConnection(connectionString))
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@tabla", tablaSeleccionada);

                                try
                                {
                                    conn.Open();
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            var nombre = reader["Nombre"].ToString().ToUpper();

                                            foreach (ListViewItem item in LSVcampos.Items)
                                            {
                                                if (item.SubItems[0].Text.ToUpper() == nombre)
                                                {
                                                    item.Checked = true;
                                                }
                                            }
                                        }
                                    }
                                    conn.Close();
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                ComprobarTiposDeCampos(tablaSeleccionada);
                LSVcampos.Refresh();

            }
            catch (Exception)
            {
            }        
        }

        private void CargarListViewDesdeEsquema(IDataReader reader, bool esDB2)
        {
            DataTable schema = reader.GetSchemaTable();

            foreach (DataRow row in schema.Rows)
            {
                var nombre = row["ColumnName"].ToString().ToUpper();
                var dataType = (Type)row["DataType"];
                var longitud = row["NumericPrecision"] != DBNull.Value ? Convert.ToInt32(row["NumericPrecision"]) : 0;
                var escala = row["NumericScale"] != DBNull.Value ? Convert.ToInt32(row["NumericScale"]) : 0;
                var aceptaNulos = (bool)row["AllowDBNull"];

                var tipo = dataType.ToString().ToUpper();
                if (esDB2)
                {
                    tipo = (TIPOSDB2.ContainsKey(dataType) ? TIPOSDB2[dataType] : dataType.Name).ToUpper();
                }
                else
                {
                    tipo = (TIPOSSQL.ContainsKey(dataType) ? TIPOSSQL[dataType] : dataType.Name).ToUpper();
                }

                ListViewItem item = new ListViewItem(nombre);
                item.SubItems.Add(tipo);
                item.SubItems.Add(longitud.ToString());
                item.SubItems.Add(escala.ToString());
                item.SubItems.Add(aceptaNulos ? "SÍ" : "NO");
                LSVcampos.Items.Add(item);
            }
        }

        private void CargarListViewDesdeReader(IDataReader reader)
        {
            var nombre = reader["Nombre"].ToString().ToUpper();
            var tipo = reader["Tipo"].ToString().ToUpper();
            var longitud = reader["Longitud"].ToString();
            var escala = reader["Escala"].ToString();
            var aceptaNulos = reader["AceptaNulos"].ToString();

            camposTabla.Add(nombre);
            ListViewItem item = new ListViewItem(nombre);
            item.SubItems.Add(tipo);
            item.SubItems.Add(longitud);
            item.SubItems.Add(escala);
            item.SubItems.Add(aceptaNulos);
            LSVcampos.Items.Add(item);
        }

        private void ComprobarTiposDeCampos(string tabla)
        {
            List<string> columnasError = new List<string>();
            DataSet DS = null;

            if (RDBdb2.Checked)
            {
                try
                {
                    Ejecutar datos = EstablecerConexion();
                    datos.Consulta = "SELECT * FROM " + tabla + " FETCH FIRST 1 ROW ONLY";
                    ComandoDB2 DB2 = new ComandoDB2(datos.Consulta, datos.ObtenerConexion());
                    DB2.Conexion = new System.Data.Odbc.OdbcConnection(datos.ObtenerConexion());

                    DS = DB2.ObtenerDataSet();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                try
                {
                    string servidor = CMBservidor.Items[CMBservidor.SelectedIndex].ToString().ToUpper();
                    string connectionString = @"Data Source=SQL" + servidor + @"\" + servidor + "; Initial Catalog=" + CMBbases.Items[CMBbases.SelectedIndex].ToString() + ";Persist Security Info=True;User ID=usuario;Password=ci?r0ba;MultipleActiveResultSets=True";
                    tabla = CMBtablas.Items[CMBtablas.SelectedIndex].ToString();

                    string query = "SELECT TOP 1 * FROM " + tabla;

                    using (SqlDataAdapter DA = new SqlDataAdapter(query, connectionString))
                    {
                        DA.Fill(DS);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            if (DS != null)
            {
                int i = 0;
                foreach (DataColumn columna in DS.Tables[0].Columns)
                {
                    if (Tipo(columna) == ERROR)
                    {
                        foreach (ListViewItem item in LSVcampos.Items)
                        {
                            if (item.SubItems[0].Text == columna.ColumnName)
                            {
                                if (item.SubItems[1].Text.Trim() == "CHAR" && columna.DataType == typeof(Byte[]))
                                {
                                    break;
                                }
                                else
                                {
                                    columnasError.Add(item.SubItems[0].Text + "\r\n     TIPO: " + item.SubItems[1].Text.Trim() + " (" + columna.DataType.ToString() + ")");
                                    item.BackColor = System.Drawing.Color.Red;
                                    item.ForeColor = System.Drawing.Color.White;
                                    item.Font = new System.Drawing.Font(item.Font.FontFamily, item.Font.Size, System.Drawing.FontStyle.Bold);
                                    item.ListView.Refresh();
                                    break;
                                }
                            }
                        }
                    }

                    i++;
                }
            }

            if (columnasError.Count > 0)
            {
                string columnas = string.Join("\r\n", columnasError);
                CustomMessageBox.Show("NO SE PUEDE PROCESAR LA SIGUIENTE TABLA DEBIDO A INCONSISTENCIAS CON LOS SIGUIENTES CAMPOS:\r\n\r\n" + columnas, CustomMessageBox.ATENCION, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                BTNgenerarDesdeTabla.Enabled = false;
            }
            else
            {
                BTNgenerarDesdeTabla.Enabled = true;
            }
        }

        private void RDBsql_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //CHKquitarEsquema.Visible = RDBsql.Checked;
                CHKtryOrIf.Visible = !RDBsql.Checked;
                //CHKquitarEsquema.Refresh();
                if (RDBsql.Checked)
                {
                    CMBservidor.Items.Clear();
                    CMBservidor.Items.AddRange(new object[] { "DESARROLLO", "PRODUCCION" });
                    if (CMBservidor.Items.Count > 0)
                    {
                        CMBservidor.SelectedIndex = 0;
                    }
                    CMBservidor.Refresh();
                }
            }
            catch (Exception)
            {
            }
        }

        private void RDBdb2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                SPCbak2.Panel2Collapsed = !RDBdb2.Checked;
                if (RDBdb2.Checked)
                {
                    //CHKquitarEsquema.Visible = !RDBdb2.Checked;
                    CHKtryOrIf.Visible = RDBdb2.Checked;
                    //CHKquitarEsquema.Refresh();
                    CMBservidor.Items.Clear();
                    CMBservidor.Items.AddRange(new object[] { "133.123.120.120", "SERVER04", "SERVER01" });
                    if (CMBservidor.Items.Count > 0)
                    {
                        CMBservidor.SelectedIndex = 0;
                    }
                    CMBservidor.Refresh();
                }
            }
            catch (Exception)
            {
            }
        }

        private void BTNdirectorioCapas_Click(object sender, EventArgs e)
        {
            DefinirDirectorioCapas();
        }

        private void DefinirDirectorioCapas()
        {
            FBDdirectorioCapas.ShowDialog();
            TXTpathCapas.Text = FBDdirectorioCapas.SelectedPath;
        }

        private void CMBtablas_TextUpdate(object sender, EventArgs e)
        {
            string texto = CMBtablas.Text;

            // ✅ 1. Filtrar caracteres no válidos (solo letras, números y _ $ # @)
            if (!System.Text.RegularExpressions.Regex.IsMatch(texto, @"^[a-zA-Z0-9_@$#]*$"))
            {
                return; // ignorar si el texto tiene caracteres inválidos
            }

            // ✅ 2. Filtrar lista
            List<string> filtrados = tablasBase
                .Where(item => item.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            CMBtablas.BeginUpdate();
            CMBtablas.Items.Clear();

            if (filtrados.Count > 0)
            {
                CMBtablas.Items.AddRange(filtrados.ToArray());
                CMBtablas.DroppedDown = true;
                // ✅ 3. Restaurar texto 
                CMBtablas.Text = texto;
            }
            else
            {
                // Si no hay resultados, no explota: opcionalmente podrías cerrar el desplegable
                CMBtablas.SelectedIndex = -1; // evita el error
                CMBtablas.Text = texto;
                try
                {
                    CMBtablas.DroppedDown = false; 
                }
                catch (Exception err)
                {
                }
            }
            CMBtablas.SelectionStart = texto.Length;
            CMBtablas.SelectionLength = 0;

            CMBtablas.EndUpdate();
        }

        private void CMBtablas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                CMBtablas.BeginUpdate();
                CMBtablas.Items.Clear();
                CMBtablas.Items.AddRange(tablasBase.ToArray());
                CMBtablas.Text = string.Empty;
                CMBtablas.DroppedDown = true;
                CMBtablas.EndUpdate();
            }
        }

        private void SPCclase_ClientSizeChanged(object sender, EventArgs e)
        {
            if (SPCclase.SplitterDistance < 313)
            {
                SPCclase.SplitterDistance = 313;
                SPCclase.ResumeLayout();
                SPCclase.Refresh();
            }
        }

        private void EnforceSplitBounds()
        {
            try
            {
                if (SPCclase.Orientation == Orientation.Vertical)
                {
                    int min = SPCclase.Panel1MinSize;
                    int max = Math.Max(min, SPCclase.Width - SPCclase.Panel2MinSize - SPCclase.SplitterWidth);

                    if (SPCclase.SplitterDistance < min)
                        SPCclase.SplitterDistance = min;
                    else if (SPCclase.SplitterDistance > max)
                        SPCclase.SplitterDistance = max;
                }
                else // Horizontal
                {
                    int min = SPCclase.Panel1MinSize;
                    int max = Math.Max(min, SPCclase.Height - SPCclase.Panel2MinSize - SPCclase.SplitterWidth);

                    if (SPCclase.SplitterDistance < min)
                        SPCclase.SplitterDistance = min;
                    else if (SPCclase.SplitterDistance > max)
                        SPCclase.SplitterDistance = max;
                }
            }
            catch (Exception)
            {
            }
        }

        private void EnsureFormMinimumSize()
        {
            // Calcula un MinimumSize del Form para que nunca puedas redimensionarlo
            // por debajo de la suma de mínimos de los panels + splitter + márgenes del form.
            if (SPCclase.Orientation == Orientation.Vertical)
            {
                int extra = this.Width - SPCclase.Width; // bordes + margen entre form y split
                int requiredWidth = SPCclase.Panel1MinSize + SPCclase.Panel2MinSize + SPCclase.SplitterWidth + extra;
                this.MinimumSize = new System.Drawing.Size(requiredWidth, this.MinimumSize.Height);
            }
            else
            {
                int extra = this.Height - SPCclase.Height;
                int requiredHeight = SPCclase.Panel1MinSize + SPCclase.Panel2MinSize + SPCclase.SplitterWidth + extra;
                this.MinimumSize = new System.Drawing.Size(this.MinimumSize.Width, requiredHeight);
            }
        }

        private void FRMgeneradorDeCapas_Resize(object sender, EventArgs e)
        {
            EnforceSplitBounds();
        }

        private void SPCclase_SplitterMoved(object sender, SplitterEventArgs e)
        {
            EnforceSplitBounds();
        }

        private void BTNbuscarSolucion_Click(object sender, EventArgs e)
        {
            OFDlistarDeSolucion.ShowDialog();
            ListarNameSpaces();
            CMBnamespaces.DroppedDown = true;
        }

        private void ListarNameSpaces()
        {
            HashSet<string> namespaces = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 1) Namespaces reales
            List<string> nameSpacesDesdeSolucion = ObtenerNamespacesDesdeSolucion(OFDlistarDeSolucion.FileName);
            foreach (var ns in nameSpacesDesdeSolucion)
            {
                namespaces.Add(ns);
            }

            // 2) Carpetas declaradas en los proyectos
            var carpetas = ObtenerCarpetasDesdeProyectos(OFDlistarDeSolucion.FileName);
            foreach (var carpeta in carpetas)
            {
                namespaces.Add(carpeta.Replace("\\", ".").Replace("/", "."));
            }

            // 3) Mostrar en el combo
            CMBnamespaces.Items.Clear(); 
            foreach (string item in namespaces)
            {
                CMBnamespaces.Items.Add(item);
            }
            CMBnamespaces.DroppedDown = desplegarCombo;
        }

        private List<string> ObtenerCarpetasDesdeProyectos(string solucionPath)
        {
            var resultado = new List<string>();

            if (string.IsNullOrEmpty(solucionPath) || !File.Exists(solucionPath))
                return resultado;

            string directorioSolucion = Path.GetDirectoryName(solucionPath);

            // Buscar todos los proyectos .csproj en la solución
            foreach (var csproj in Directory.GetFiles(directorioSolucion, "*.csproj", SearchOption.AllDirectories))
            {
                XDocument doc = XDocument.Load(csproj);
                XNamespace ns = doc.Root.Name.Namespace;

                // Tomamos todos los archivos incluidos en el proyecto
                var includes = doc.Descendants(ns + "Compile")
                                  .Select(e => (string)e.Attribute("Include"))
                                  .Concat(doc.Descendants(ns + "Content")
                                             .Select(e => (string)e.Attribute("Include")))
                                  .Concat(doc.Descendants(ns + "EmbeddedResource")
                                             .Select(e => (string)e.Attribute("Include")))
                                  .Where(p => !string.IsNullOrWhiteSpace(p));

                foreach (var include in includes)
                {
                    string carpeta = Path.GetDirectoryName(include);
                    if (!string.IsNullOrEmpty(carpeta))
                        resultado.Add(carpeta);
                }
            }

            // Devolvemos solo carpetas únicas
            return resultado.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();
        }

        private List<string> ObtenerNamespacesDesdeSolucion(string slnPath)
        {
            var namespaces = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                var slnDir = Path.GetDirectoryName(slnPath);

                // 1. Leer el .sln y encontrar todos los .csproj
                var csprojPaths = File.ReadAllLines(slnPath)
                    .Where(line => line.Contains(".csproj"))
                    .Select(line =>
                    {
                        var parts = line.Split(',');
                        if (parts.Length >= 2)
                        {
                            var relativePath = parts[1].Trim().Trim('"');
                            return Path.Combine(slnDir, relativePath);
                        }
                        return null;
                    })
                    .Where(path => !string.IsNullOrEmpty(path) && File.Exists(path))
                    .ToList();

                // 2. Para cada proyecto, buscar todos los .cs
                foreach (var csproj in csprojPaths)
                {
                    var projDir = Path.GetDirectoryName(csproj);
                    var csFiles = Directory.GetFiles(projDir, "*.cs", SearchOption.AllDirectories);

                    foreach (var file in csFiles)
                    {
                        foreach (var ns in ExtraerNamespacesDesdeArchivo(file))
                        {
                            // Agrego el namespace completo
                            namespaces.Add(ns);

                            // Y también todos los "padres"
                            var parts = ns.Split('.');
                            for (int i = 1; i < parts.Length; i++)
                            {
                                var padre = string.Join(".", parts.Take(i));
                                namespaces.Add(padre);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return namespaces.OrderBy(n => n).ToList();
        }

        private IEnumerable<string> ExtraerNamespacesDesdeArchivo(string filePath)
        {
            var regex = new Regex(@"^\s*namespace\s+([A-Za-z0-9_.]+)", RegexOptions.Multiline);
            string contenido = File.ReadAllText(filePath);

            foreach (Match match in regex.Matches(contenido))
            {
                yield return match.Groups[1].Value;
            }
        }

        private void CMBnamespaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                TXTespacioDeNombres.Text = CMBnamespaces.Items[CMBnamespaces.SelectedIndex].ToString();
            }
            catch (Exception)
            {
            }
        }

        private void BNTobtenerEstructura_Click(object sender, EventArgs e)
        {
            CamposTabla("CONSULTA", TXTgenerarAPartirDeConsulta.Text);
        }

        private void BTNagregarCampo_Click(object sender, EventArgs e)
        {
            if (TBCcamposABM.SelectedTab == TBPbaja)
            {
                foreach (ListViewItem item in LSVcampos.SelectedItems)
                {
                    var fila = buscarFila(DGVbaja, item.Text);
                    // Buscar fila por valor exacto en la primer celda
                    if (fila == null)
                    {
                        int indiceFila = DGVbaja.Rows.Add();
                        DGVbaja.Rows[indiceFila].Cells[0].Value = item.Text;
                        ((DataGridViewComboBoxColumn)DGVbaja.Columns[1]).DataSource = new BindingSource(CamposABM, null);
                        ((DataGridViewComboBoxColumn)DGVbaja.Columns[1]).DisplayMember = "Key";
                        ((DataGridViewComboBoxColumn)DGVbaja.Columns[1]).ValueMember = "Value";
                    }
                }
            }
            if (TBCcamposABM.SelectedTab == TBPmodificacion)
            {
                foreach (ListViewItem item in LSVcampos.SelectedItems)
                {
                    var fila = buscarFila(DGVmodificacion, item.Text);
                    // Buscar fila por valor exacto en la primer celda
                    if (fila == null)
                    {
                        int indiceFila = DGVmodificacion.Rows.Add();
                        DGVmodificacion.Rows[indiceFila].Cells[0].Value = item.Text;
                        ((DataGridViewComboBoxColumn)DGVmodificacion.Columns[1]).DataSource = new BindingSource(CamposABM, null);
                        ((DataGridViewComboBoxColumn)DGVmodificacion.Columns[1]).DisplayMember = "Key";
                        ((DataGridViewComboBoxColumn)DGVmodificacion.Columns[1]).ValueMember = "Value";
                    }
                }
            }
            if (TBCcamposABM.SelectedTab == TBPrecuperacion)
            {
                foreach (ListViewItem item in LSVcampos.SelectedItems)
                {
                    var fila = buscarFila(DGVrecuperacion, item.Text);
                    // Buscar fila por valor exacto en la primer celda
                    if (fila == null)
                    {
                        int indiceFila = DGVrecuperacion.Rows.Add();
                        DGVrecuperacion.Rows[indiceFila].Cells[0].Value = item.Text;
                        ((DataGridViewComboBoxColumn)DGVrecuperacion.Columns[1]).DataSource = new BindingSource(CamposABM, null);
                        ((DataGridViewComboBoxColumn)DGVrecuperacion.Columns[1]).DisplayMember = "Key";
                        ((DataGridViewComboBoxColumn)DGVrecuperacion.Columns[1]).ValueMember = "Value";
                    }
                }
            }
        }

        private void BTNquitarCampo_Click(object sender, EventArgs e)
        {
            if (TBCcamposABM.SelectedTab == TBPbaja)
            {
                foreach (DataGridViewRow fila in DGVbaja.SelectedRows)
                {
                    DGVbaja.Rows.Remove(fila);
                }
                DGVbaja.Refresh();
            }
            if (TBCcamposABM.SelectedTab == TBPmodificacion)
            {
                foreach (DataGridViewRow fila in DGVmodificacion.SelectedRows)
                {
                    DGVmodificacion.Rows.Remove(fila);
                }
                DGVmodificacion.Refresh();
            }
            if (TBCcamposABM.SelectedTab == TBPrecuperacion)
            {
                foreach (DataGridViewRow fila in DGVrecuperacion.SelectedRows)
                {
                    DGVrecuperacion.Rows.Remove(fila);
                }
                DGVrecuperacion.Refresh();
            }
        }

        private DataGridViewRow buscarFila(DataGridView grilla, string valor)
        {
            return grilla.Rows.Cast<DataGridViewRow>().FirstOrDefault(r => r.Cells[0].Value != null &&
                         r.Cells[0].Value.ToString() == valor);
        }

        private void CHKbaja_CheckedChanged(object sender, EventArgs e)
        {
            if (CHKbaja.Checked)
            {
                // Si el CheckBox está marcado, agregar la TabPage al TabControl.
                if (!TBCcamposABM.TabPages.Contains(TBPbaja))
                {
                    TBCcamposABM.TabPages.Insert(0, TBPbaja);
                    TBCcamposABM.TabPages[0].Focus();
                }
            }
            else
            {
                // Si el CheckBox está desmarcado, quitar la TabPage del TabControl.
                if (TBCcamposABM.TabPages.Contains(TBPbaja))
                {
                    TBCcamposABM.TabPages.Remove(TBPbaja);
                }
            }
        }

        private void CHKmodificacion_CheckedChanged(object sender, EventArgs e)
        {
            if (CHKmodificacion.Checked)
            {
                // Si el CheckBox está marcado, agregar la TabPage al TabControl.
                if (!TBCcamposABM.TabPages.Contains(TBPmodificacion))
                {
                    int indice = 1;
                    if (TBCcamposABM.TabPages.Count == 0)
                    {
                        indice = 0;
                    }
                    else
                    {
                        if (TBCcamposABM.TabPages.Count == 1)
                        {
                            if (TBCcamposABM.TabPages[0] == TBPrecuperacion)
                            {
                                indice = 0;
                            }
                        }
                    }
                    TBCcamposABM.TabPages.Insert(indice, TBPmodificacion);
                    TBCcamposABM.TabPages[indice].Focus();
                }
            }
            else
            {
                // Si el CheckBox está desmarcado, quitar la TabPage del TabControl.
                if (TBCcamposABM.TabPages.Contains(TBPmodificacion))
                {
                    TBCcamposABM.TabPages.Remove(TBPmodificacion);
                }
            }
        }

        private void CHKrecuperacion_CheckedChanged(object sender, EventArgs e)
        {
            if (CHKrecuperacion.Checked)
            {
                // Si el CheckBox está marcado, agregar la TabPage al TabControl.
                if (!TBCcamposABM.TabPages.Contains(TBPrecuperacion))
                {
                    int indice = TBCcamposABM.TabPages.Count;
                    TBCcamposABM.TabPages.Insert(indice, TBPrecuperacion);
                    TBCcamposABM.TabPages[indice].Focus();
                }
            }
            else
            {
                // Si el CheckBox está desmarcado, quitar la TabPage del TabControl.
                if (TBCcamposABM.TabPages.Contains(TBPrecuperacion))
                {
                    TBCcamposABM.TabPages.Remove(TBPrecuperacion);
                }
            }
        }

        private void FRMcapibara_Validated(object sender, EventArgs e)
        {
            desplegarCombo = true;
        }
    }
}

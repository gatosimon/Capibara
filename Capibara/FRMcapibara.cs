using Capibara.CustomControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Capibara
{
    public partial class FRMcapibara : Form
    {
        private const int PANEL1_MIN = 510; // ancho/alto mínimo que querés para Panel1
        private const string FOLDER_CLOSE = "fclose";
        private const string FOLDER_OPEN = "fopen";
        private const string PROYECT = "proyect";
        private const string SOLUTION = "solution";
        private const string NUEVA_CARPETA = "Nueva Carpeta";
        private const string FILE = "file";
        private const string KEY = "key";
        public string PathCapas { get { return TXTpathCapas.Text; } }
        private string CarpetaDestino = string.Empty;
        private string OrigenDeDatoSql = string.Empty;
        private string pathProyecto = string.Empty;
        private string pathGlobalAsax = string.Empty;
        private bool generarDesdeConsulta = false;
        private bool desplegarCombo = false;
        private bool isActivated = false;
        private bool cargarCampos = false;
        Dictionary<string, Conexion> conexiones;

        Configuracion configuracion;

        Capas capas = null;

        public WaitOverlay overlay;

        public FRMcapibara()
        {
            InitializeComponent();
            capas = new Capas(this);
            configuracion = Configuracion.Cargar();
            desplegarCombo = false;
            overlay = new WaitOverlay(this);
            Utilidades.IniciarDeteccionDispositivos(this);
            Utilidades.ReproducirIntro();
            WaitCursor();
        }

        private void FRMcapibara_Shown(object sender, EventArgs e)
        {
            // Mostrar overlay(en UI thread)
            if (configuracion.MostrarOverlayEnInicio && overlay != null && !overlay.IsDisposed)
            {
                overlay.Show();
                Application.DoEvents();
            }

            // 🔹 Otro Task para la carga pesada
            Task.Run(() =>
            {
                // 🔹 Volvemos a la UI para actualizar controles
                this.Invoke((Action)(() =>
                {
                    // Cargar configuraciones y refrescar UI
                    ListarNameSpaces();
                    CargarConfiguracion();
                    InicializarIndices();

                    CursorDefault();
                    isActivated = true;
                }));
            });
        }

        private void FRMgeneradorDeCapas_Resize(object sender, EventArgs e)
        {
            ForzarSeparacionClase();
        }

        private void FRMcapibara_FormClosing(object sender, FormClosingEventArgs e)
        {
            Utilidades.DesRegistrar();
        }

        private void CargarConfiguracion()
        {
            try
            {
                //configuracion = Configuracion.Cargar();
                InicializarConexiones();
                if (configuracion.Conexion != null)
                {
                    ActualizarConeccionActual(configuracion.Conexion); 
                }
                else
                {
                }
                TXTespacioDeNombres.Text = configuracion.UltimoNamespaceSeleccionado;
                TXTnombreAmigable.Text = configuracion.NombreAmigable;
                CarpetaDestino = configuracion.CarpetaDestino;
                OrigenDeDatoSql = configuracion.OrigenDeDatosMsSQL;
                ActualizarLabelSeleccionTRV(Path.GetFileName(CarpetaDestino), OrigenDeDatoSql);
                TXTpathCapas.Text = configuracion.RutaPorDefectoResultados;
                OFDlistarDeSolucion.InitialDirectory = configuracion.PathSolucion != null && configuracion.PathSolucion.Length > 0 ? Path.GetDirectoryName(configuracion.PathSolucion) : string.Empty;
                OFDlistarDeSolucion.FileName = configuracion.PathSolucion;
                CHKmostrarOverlayEnIicio.Checked = configuracion.MostrarOverlayEnInicio;
                CHKinsertarEnProyecto.Checked = configuracion.InsertarEnProyecto;

                CargarCamposAbmDesdeConfiguracion(configuracion.camposAlta, DGValta);
                CargarCamposAbmDesdeConfiguracion(configuracion.camposBaja, DGVbaja);
                CargarCamposAbmDesdeConfiguracion(configuracion.camposModificacion, DGVmodificacion);
                CargarCamposAbmDesdeConfiguracion(configuracion.camposRecuperacion, DGVrecuperacion);

                CargarComboyTreeView();
            }
            catch { }
        }

        private void CargarCamposAbmDesdeConfiguracion(List<string[]> campos, DataGridView grilla)
        {
            foreach (string[] item in campos)
            {
                int indiceFila = grilla.Rows.Add();
                string campo = item[0];   // nombre del campo (ej: ALTAFECHA)
                string valor = item[1];   // valor a vincular (ej: FECHA ACTUAL)

                grilla.Rows[indiceFila].Cells[0].Value = campo;

                // 🔹 Crear comboCell específico
                var comboCell = new DataGridViewComboBoxCell();
                BindingSource bs;

                // Detectar qué diccionario usar según coincidencia
                if (Capas.CamposAbm.ContainsKey(valor))
                    bs = new BindingSource(Capas.CamposAbm, null);
                else if (Capas.CamposAbmFechas.ContainsKey(valor))
                    bs = new BindingSource(Capas.CamposAbmFechas, null);
                else if (Capas.CamposAbmHoras.ContainsKey(valor))
                    bs = new BindingSource(Capas.CamposAbmHoras, null);
                else
                    // fallback: lista general
                    bs = new BindingSource(Capas.CamposAbm, null);

                comboCell.DataSource = bs;
                comboCell.DisplayMember = "Key";
                comboCell.ValueMember = "Value";

                // Asignar el combo a la grilla
                grilla.Rows[indiceFila].Cells[1] = comboCell;

                // 🔹 Buscar si el valor existe en el diccionario elegido
                string valorReal = null;
                if (bs.Cast<KeyValuePair<string, string>>().Any(x => x.Key == valor))
                {
                    valorReal = bs.Cast<KeyValuePair<string, string>>()
                                  .First(x => x.Key == valor).Value;
                }

                // Si existe, lo asignamos; si no, fallback al primer valor
                if (!string.IsNullOrEmpty(valorReal))
                {
                    comboCell.Value = valorReal;
                }
                else
                {
                    var primero = bs.Cast<KeyValuePair<string, string>>().FirstOrDefault();
                    if (!primero.Equals(default(KeyValuePair<string, string>)))
                        comboCell.Value = primero.Value;
                }
            }
        }

        private void ActualizarLabelSeleccionTRV(string carpeta, string origenDatos)
        {
            if (conexionActual != null)
            {
                if (conexionActual.Motor == TipoMotor.DB2)
                {
                    LBLseleccionesTRV.Text = $"CARPETA SELECCIONADA:\n   {carpeta}\nORIGEN DE DATOS SQL:\n   {origenDatos}";
                }
                else
                {
                    LBLseleccionesTRV.Text = $"CARPETA SELECCIONADA:\n   {carpeta}";
                } 
            }
        }

        private void GuardarConfiguracion()
        {
            try
            {
                TXTclase.Text = Clase(CMBtablas.Items[CMBtablas.SelectedIndex].ToString(), generarDesdeConsulta ? TXTgenerarAPartirDeConsulta.Text : string.Empty);

                CapibararProyecto();

                configuracion.Conexion = conexionActual;
                configuracion.Tabla = CMBtablas.Items[CMBtablas.SelectedIndex].ToString();
                configuracion.Consulta = TXTgenerarAPartirDeConsulta.Text;
                configuracion.UltimoNamespaceSeleccionado = TXTespacioDeNombres.Text;
                configuracion.NombreAmigable = TXTnombreAmigable.Text;
                configuracion.CarpetaDestino = CarpetaDestino;
                configuracion.OrigenDeDatosMsSQL = OrigenDeDatoSql;
                configuracion.RutaPorDefectoResultados = TXTpathCapas.Text;
                configuracion.PathSolucion = OFDlistarDeSolucion.FileName;
                configuracion.MostrarOverlayEnInicio = CHKmostrarOverlayEnIicio.Checked;
                configuracion.InsertarEnProyecto = CHKinsertarEnProyecto.Checked;
                configuracion.camposAlta = GuardarCamposAbm(DGValta);
                configuracion.camposBaja = GuardarCamposAbm(DGVbaja);
                configuracion.camposModificacion = GuardarCamposAbm(DGVmodificacion);
                configuracion.camposRecuperacion = GuardarCamposAbm(DGVrecuperacion);

                configuracion.Guardar();
            }
            catch (Exception err)
            {
                CustomMessageBox.Show("Ocurrió un error inesperado", CustomMessageBox.ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<string[]> GuardarCamposAbm(DataGridView grilla)
        {
            return grilla.Rows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow) // 🔹 evita la fila vacía de edición
                .Select(r => r.Cells
                    .Cast<DataGridViewCell>()
                    .Select(c => c.FormattedValue?.ToString() ?? string.Empty) // 🔹 null safe
                    .ToArray())
                .ToList();
        }

        private void InicializarIndices()
        {
            SPCclase.Panel1MinSize = PANEL1_MIN;

            int indice = conexionActual != null ? CMBconexion.FindString(conexionActual.Nombre) : -1;
            if (indice > -1 && CMBconexion.Items.Count > 0)
            {
                CMBconexion.SelectedIndex = indice > -1 ? indice : 0;
                CMBconexion.Text = CMBconexion.Items[CMBconexion.SelectedIndex].ToString();
            }

            TXTgenerarAPartirDeConsulta.Text = configuracion.Consulta;

            LSVcampos.View = View.Details;
            LSVcampos.CheckBoxes = true;
            LSVcampos.Columns.Add("Nombre", 200);
            LSVcampos.Columns.Add("Tipo", 80);
            LSVcampos.Columns.Add("Longitud", 70);
            LSVcampos.Columns.Add("Escala", 60);
            LSVcampos.Columns.Add("Acepta Nulos", 100);
            
            // Fuerzo a que tome el valor de la tabla guardada en la configuración o sino la primera de la primer conexión
            indice = configuracion.Tabla.Trim().Length > 0 ? CMBtablas.FindStringExact(configuracion.Tabla) : 0;
            CMBtablas.SelectedIndex = -1;
            cargarCampos = true;
            if (indice > -1 && CMBtablas.Items.Count > 0)
            {
                CMBtablas.SelectedIndex = indice > -1 ? indice : 0;
                CMBtablas.Text = CMBtablas.Items[CMBtablas.SelectedIndex].ToString();
            }

            AsegurarTamañoMinimo();
            ForzarSeparacionClase();
        }

        private string Clase(string tabla, string consulta = "")
        {
            if (consulta.Trim().Length > 0)
            {
                tabla = "CONSULTA";
            }
            // Obtengo los campos en caso que no se haya recargado el ListView. Se perderán las posibles claves asignadas manualmente
            if (capas.camposTabla.Count == 0)
            {
                CamposTabla(tabla, consulta);
            }
            string resultado = string.Empty;
            List<DataColumn> claves = new List<DataColumn>();
            List<DataColumn> camposConsulta = new List<DataColumn>();
            List<string> columnasError = new List<string>();

            bool consultaOk = true;

            StringConnection stringConnection = DefinirStringConnection();
            if (conexionActual.Motor == TipoMotor.DB2)
            {
                try
                {
                    stringConnection.Consulta = consulta.Trim().Length > 0 ? consulta : "SELECT " + string.Join(", ", capas.camposTabla) + " FROM " + tabla + " FETCH FIRST 1 ROW ONLY";

                    DataLayer.DataBase DB2base = new DataLayer.DataBase(new OdbcConnection(stringConnection.Obtener()));
                    DB2base.OpenConnection();
                    DataSet DS = DB2base.DataSet(stringConnection.Consulta);
                    DB2base.CloseConnection();
                    camposConsulta = ObtenerCamposConsulta(consulta, claves, columnasError, DS);
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
                    tabla = CMBtablas.Items[CMBtablas.SelectedIndex].ToString();

                    string query = consulta.Trim().Length > 0 ? consulta : "SELECT TOP 1 " + string.Join(", ", capas.camposTabla) + " FROM " + tabla;
                    DataLayer.DataBase SQLbase = new DataLayer.DataBase(new OdbcConnection(stringConnection.Obtener()));
                    SQLbase.OpenConnection();
                    DataSet DS = SQLbase.DataSet(query);

                    camposConsulta = ObtenerCamposConsulta(consulta, claves, columnasError, DS);
                    SQLbase.CloseConnection();
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
                    // Saco el esquema para las tablas de SQL
                    string[] partes = tabla.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    tabla = partes[partes.Length - 1];
                    capas.TABLA = tabla;

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
                    resultado += ArmarGlobal();
                    resultado += "\r\n";

                    if (CHKclaseTypeScript.Checked)
                    {
                        resultado += ArmarClaseTypeScript(camposConsulta);
                        resultado += "\r\n";
                    }
                    else
                    {
                        ArmarClaseTypeScript(camposConsulta);
                    }

                    if (CHKgenerarAbm.Checked)
                    {
                        resultado += ArmarABM(camposConsulta);
                        resultado += "\r\n";
                    }
                    else
                    {
                        ArmarClaseTypeScript(camposConsulta);
                    }

                    Utilidades.ReproducirSplash();
                    if (!CHKinsertarEnProyecto.Checked && Directory.Exists(TXTpathCapas.Text))
                    {
                        Process.Start("explorer.exe", TXTpathCapas.Text);
                    }
                }
            }
            else
            {
                CustomMessageBox.Show("Ocurrió un error al intentar acceder a la" + (consulta.Trim().Length > 0 ? " consulta. " : " tabla. ") + resultado, CustomMessageBox.ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return resultado;
        }

        private List<DataColumn> ObtenerCamposConsulta(string consulta, List<DataColumn> claves, List<string> columnasError, DataSet DS)
        {
            List<DataColumn> camposConsulta = new List<DataColumn>();
            int i = 0;
            bool obtenerTodas = LSVcampos.CheckedItems.Count == 0;
            foreach (DataColumn columna in DS.Tables[0].Columns)
            {
                // Si genero a partir de una Query
                if (consulta.Trim().Length > 0)
                {
                    camposConsulta.Add(columna);
                }
                else
                {
                    if (LSVcampos.Items[i].ImageKey == KEY)
                    {
                        claves.Add(columna);
                    }

                    if (obtenerTodas)
                    {
                        camposConsulta.Add(columna);
                    }
                    else
                    {
                        ListViewItem item = LSVcampos.Items.Cast<ListViewItem>().FirstOrDefault(x => x.Text == columna.ColumnName && x.Checked);
                        if (item != null)
                        {
                            camposConsulta.Add(columna);
                        }
                    }

                    if (capas.Tipo(columna) == Capas.ERROR)
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
            return camposConsulta;
        }

        #region ARMADO DE CAPAS

        #region BACK

        private string ArmarControllers(List<DataColumn> claves)
        {
            bool DB2 = conexionActual.Motor == TipoMotor.DB2;
            string origen = DB2 ? Capas.MODEL : Capas.DTO;
            string nombreDeClase = capas.TABLA;
            string tipoClase = capas.TABLA + origen;
            string nombreClasePrimeraMinuscula = nombreDeClase[0].ToString().ToLower() + nombreDeClase.Substring(1);
            string espacioDeNombres = $"{TXTespacioDeNombres.Text.Trim()}.{Utilidades.FormatearCadena(TXTnombreAmigable.Text)}";
            string camposFromUri = string.Join(", ", (from c in claves select "[FromUri] " + capas.Tipo(c) + " " + c.ColumnName).ToList());
            string camposClave = string.Join(", ", (from c in claves select c.ColumnName).ToList());
            StringBuilder Controller = new StringBuilder();

            Controller.AppendLine("using System;");
            Controller.AppendLine("using System.Collections.Generic;");
            Controller.AppendLine("using SistemaMunicipalGeneral;");
            Controller.AppendLine("using SistemaMunicipalGeneral.Web.FiltrosDeAccion;");
            Controller.AppendLine("using System.Threading.Tasks;");
            Controller.AppendLine("using System.Web.Http;");
            Controller.AppendLine("using System.Web.Http.Cors;");
            Controller.AppendLine($"using { espacioDeNombres }.{ Capas.MODEL };");
            Controller.AppendLine($"using { espacioDeNombres }.{ Capas.DTO };");
            if (espacioDeNombres.Trim().Length > 0) Controller.AppendLine($"using { espacioDeNombres }.{ Capas.SERVICE };");
            Controller.AppendLine();
            Controller.AppendLine($"namespace { espacioDeNombres }.{ Capas.CONTROLLERS }");
            Controller.AppendLine("{");
            Controller.AppendLine($"\t[RoutePrefix(\"{ Utilidades.FormatearCadena(TXTnombreAmigable.Text).ToLower() }\")]");
            Controller.AppendLine("\t[EnableCors(origins: \" * \", headers: \" * \", methods: \" * \")]");
            Controller.AppendLine();
            Controller.AppendLine($"\tpublic class { nombreDeClase + Capas.CONTROLLER } : ApiController");
            Controller.AppendLine("\t{");
            Controller.AppendLine($"\t\tprivate readonly {nombreDeClase + Capas.SERVICE_INTERFACE } _{ nombreClasePrimeraMinuscula + Capas.SERVICE };");
            Controller.AppendLine($"\t\tpublic { nombreDeClase + Capas.CONTROLLER }({nombreDeClase + Capas.SERVICE_INTERFACE } { nombreClasePrimeraMinuscula + Capas.SERVICE })");
            Controller.AppendLine("\t\t{");
            Controller.AppendLine($"\t\t\t_{ nombreClasePrimeraMinuscula + Capas.SERVICE } = { nombreClasePrimeraMinuscula + Capas.SERVICE } ?? throw new ArgumentNullException(nameof({ nombreClasePrimeraMinuscula + Capas.SERVICE }));");
            Controller.AppendLine("\t\t}");
            Controller.AppendLine();

            #region ALTA
            if (CHKalta.Checked)
            {
                Controller.AppendLine("\t\t[HttpPost, Route(\"nuevo\"), ControlarPermisos]");
                Controller.AppendLine($"\t\tpublic async Task<Respuesta> alta{ nombreDeClase }([FromBody] { tipoClase } nuevo{ origen })");
                Controller.AppendLine("\t\t{");
                Controller.AppendLine("\t\t\tRespuesta rta = new Respuesta();");
                Controller.AppendLine();
                Controller.AppendLine($"\t\t\trta.Resultado = _{ nombreClasePrimeraMinuscula + Capas.SERVICE }.alta{ nombreDeClase }(nuevo{ origen });");
                Controller.AppendLine();
                Controller.AppendLine("\t\t\treturn rta;");
                Controller.AppendLine("\t\t}");
                Controller.AppendLine();
            }
            #endregion

            #region BAJA
            if (CHKbaja.Checked)
            {
                Controller.AppendLine("\t\t[HttpGet, Route(\"baja\"), ControlarPermisos]");
                Controller.AppendLine($"\t\tpublic async Task<Respuesta> baja{ nombreDeClase }({ camposFromUri }, [FromUri] int codigoBaja, [FromUri] string motivoBaja)");
                Controller.AppendLine("\t\t{");
                Controller.AppendLine("\t\t\tRespuesta rta = new Respuesta();");
                Controller.AppendLine();
                Controller.AppendLine($"\t\t\trta.Resultado = _{ nombreClasePrimeraMinuscula + Capas.SERVICE }.baja{ nombreDeClase }({ camposClave }, codigoBaja, motivoBaja);");
                Controller.AppendLine();
                Controller.AppendLine("\t\t\treturn rta;");
                Controller.AppendLine("\t\t}");
                Controller.AppendLine();
            }
            #endregion

            #region MODIFICACION
            if (CHKmodificacion.Checked)
            {
                Controller.AppendLine("\t\t[HttpPut, Route(\"modificacion\"), ControlarPermisos]");
                Controller.AppendLine($"\t\tpublic async Task<Respuesta> modificacion{ nombreDeClase }([FromBody] { tipoClase } nuevo{ origen })");
                Controller.AppendLine("\t\t{");
                Controller.AppendLine("\t\t\tRespuesta rta = new Respuesta();");
                Controller.AppendLine();
                Controller.AppendLine($"\t\t\trta.Resultado = _{ nombreClasePrimeraMinuscula + Capas.SERVICE }.modificacion{ nombreDeClase }(nuevo{ origen });");
                Controller.AppendLine();
                Controller.AppendLine("\t\t\treturn rta;");
                Controller.AppendLine("\t\t}");
                Controller.AppendLine();
            }
            #endregion

            #region BUSCAR POR ID
            if (CHKobtenerPorId.Checked)
            {
                Controller.AppendLine("\t\t[HttpGet, Route(\"buscarid\"), ControlarPermisos]");
                Controller.AppendLine($"\t\tpublic async Task<Respuesta> obtenerPorId({ camposFromUri })");
                Controller.AppendLine("\t\t{");
                Controller.AppendLine("\t\t\tRespuesta rta = new Respuesta();");
                Controller.AppendLine($"\t\t\t{ tipoClase } solicitado = _{ nombreClasePrimeraMinuscula + Capas.SERVICE }.obtenerPorId({ camposClave });");
                Controller.AppendLine();
                Controller.AppendLine("\t\t\tif (solicitado != null)");
                Controller.AppendLine("\t\t\t{");
                Controller.AppendLine("\t\t\t\trta.Resultado = solicitado;");
                Controller.AppendLine("\t\t\t}");
                Controller.AppendLine("\t\t\telse");
                Controller.AppendLine("\t\t\t{");
                Controller.AppendLine($"\t\t\t\trta.AgregarMensajeDeError($\"No se halló {{ { capas.NombreTabla } }}\");");
                Controller.AppendLine("\t\t\t}");
                Controller.AppendLine();
                Controller.AppendLine("\t\t\treturn rta;");
                Controller.AppendLine("\t\t}");
                Controller.AppendLine();
            }
            #endregion

            #region TODOS
            if (CHKtodos.Checked)
            {
                Controller.AppendLine("\t\t[HttpGet, Route(\"todos\"), ControlarPermisos]");
                Controller.AppendLine("\t\tpublic async Task<Respuesta> obtenerTodos()");
                Controller.AppendLine("\t\t{");
                Controller.AppendLine("\t\t\tRespuesta rta = new Respuesta();");
                Controller.AppendLine($"\t\t\tList <{ tipoClase }> { nombreDeClase.ToLower() } = _{ nombreClasePrimeraMinuscula + Capas.SERVICE }.obtenerTodos();");
                Controller.AppendLine($"\t\t\tif ({ nombreDeClase.ToLower() } != null)");
                Controller.AppendLine("\t\t\t{");
                Controller.AppendLine($"\t\t\t\trta.Resultado = { nombreDeClase.ToLower() };");
                Controller.AppendLine("\t\t\t}");
                Controller.AppendLine("\t\t\telse");
                Controller.AppendLine("\t\t\t{");
                Controller.AppendLine($"\t\t\t\trta.AgregarMensajeDeError($\" - No existe {{ { capas.NombreTabla } }} que responda a la consulta indicada.\");");
                Controller.AppendLine("\t\t\t}");
                Controller.AppendLine();
                Controller.AppendLine("\t\t\treturn rta;");
                Controller.AppendLine("\t\t}");
                Controller.AppendLine();
            }
            #endregion

            #region RECUPERAR
            if (CHKrecuperacion.Checked)
            {
                Controller.AppendLine("\t\t[HttpGet, Route(\"recuperar\"), ControlarPermisos]");
                Controller.AppendLine($"\t\tpublic async Task<Respuesta> recuperar{ nombreDeClase }({ camposFromUri })");
                Controller.AppendLine("\t\t{");
                Controller.AppendLine("\t\t\tRespuesta rta = new Respuesta();");
                Controller.AppendLine();
                Controller.AppendLine($"\t\t\trta.Resultado = _{ nombreClasePrimeraMinuscula + Capas.SERVICE }.recuperar{ nombreDeClase }({ camposClave });");
                Controller.AppendLine();
                Controller.AppendLine("\t\t\treturn rta;");
                Controller.AppendLine("\t\t}");
            }
            #endregion

            Controller.AppendLine("\t}");
            Controller.AppendLine("}");

            // Elimino cualquier versión anterior de la capa
            try
            {
                if (File.Exists(capas.pathClaseController))
                {
                    File.Delete(capas.pathClaseController);
                }
            }
            catch (Exception)
            {
            }

            if (CHKcontrollers.Checked)
            {
                try
                {
                    if (!Directory.Exists(capas.pathControllers))
                    {
                        Directory.CreateDirectory(capas.pathControllers);
                    }

                    StreamWriter clase = new StreamWriter(capas.pathClaseController);
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
            string nombreDeClase = capas.TABLA;
            string espacioDeNombres = $"{TXTespacioDeNombres.Text.Trim()}.{Utilidades.FormatearCadena(TXTnombreAmigable.Text)}";

            StringBuilder Dto = new StringBuilder();
            StringBuilder newDto = new StringBuilder();

            Dto.AppendLine("using System;");
            Dto.AppendLine("using System.Collections.Generic;");
            Dto.AppendLine("using System.Linq;");
            Dto.AppendLine("using System.Web;");
            Dto.AppendLine("using Newtonsoft.Json;");
            Dto.AppendLine("using System.ComponentModel.DataAnnotations;");
            Dto.AppendLine($"using { espacioDeNombres }.{ Capas.MODEL };");
            Dto.AppendLine();
            Dto.AppendLine($"namespace { espacioDeNombres }.{ Capas.DTO}");
            Dto.AppendLine("{");
            Dto.AppendLine($"\tpublic class { nombreDeClase + Capas.DTO}");
            Dto.AppendLine("\t{");

            newDto.AppendLine($"\t\tpublic { nombreDeClase + Capas.DTO } new{ nombreDeClase + Capas.DTO }({ nombreDeClase + (conexionActual.Motor == TipoMotor.DB2 ? string.Empty : Capas.MODEL) } modelo)");
            newDto.AppendLine("\t\t{");

            int i = 0;
            foreach (DataColumn columna in columnas)
            {
                //Si es un array de byte en realidad es un booleano.
                if (columna.DataType.Name == "Byte[]")
                {
                    Dto.AppendLine($"\t\t[Required(ErrorMessage = \"- Ingrese { columna.ColumnName }.\")]");
                    Dto.AppendLine($"\t\tpublic bool { columna.ColumnName } {{ get;  set; }}");
                    newDto.AppendLine($"\t\t\t{ columna.ColumnName } = modelo.{ columna.ColumnName };");
                }
                else
                {
                    Dto.AppendLine($"\t\t[Required(ErrorMessage = \"- Ingrese { columna.ColumnName }.\")]");
                    Dto.AppendLine($"\t\tpublic { capas.Tipo(columna) } { columna.ColumnName } {{ get;  set; }}");
                    newDto.AppendLine($"\t\t\t{ columna.ColumnName } = modelo.{ columna.ColumnName };");
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

            // Elimino cualquier version anterior de la capa
            try
            {
                if (File.Exists(capas.pathClaseDto))
                {
                    File.Delete(capas.pathClaseDto);
                }
            }
            catch (Exception)
            {
            }

            if (CHKdto.Checked)
            {
                try
                {
                    if (!Directory.Exists(capas.pathDto))
                    {
                        Directory.CreateDirectory(capas.pathDto);
                    }

                    StreamWriter clase = new StreamWriter(capas.pathClaseDto);
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
            string nombreDeClase = capas.TABLA;
            string espacioDeNombres = $"{TXTespacioDeNombres.Text.Trim()}.{Utilidades.FormatearCadena(TXTnombreAmigable.Text)}";

            StringBuilder Modelo = new StringBuilder();

            Modelo.AppendLine("using System;");
            Modelo.AppendLine("using System.Collections.Generic;");
            Modelo.AppendLine("using System.Linq;");
            Modelo.AppendLine("using System.Web;");
            Modelo.AppendLine("using System.ComponentModel.DataAnnotations;");
            Modelo.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
            Modelo.AppendLine();
            Modelo.AppendLine($"namespace { espacioDeNombres }.{ Capas.MODEL}");
            Modelo.AppendLine("{");
            Modelo.AppendLine($"\t[Table(\"{nombreDeClase}\")]");
            Modelo.AppendLine($"\tpublic class {nombreDeClase + Capas.MODEL}");
            Modelo.AppendLine("\t{");
            Modelo.AppendLine("\t\t/// <summary>");
            Modelo.AppendLine("\t\t/// NOMBRE INTERNO DE LA TABLA ASOCIADA AL MODELO");
            Modelo.AppendLine("\t\t/// </summary>");
            Modelo.AppendLine($"\t\tpublic static readonly string { Capas.MODELTABNAME } = \"{ nombreDeClase }\";");
            Modelo.AppendLine();

            int nroOrdenColumna = 0;
            int nroOrdenClave = 0;
            foreach (DataColumn columna in columnas)
            {
                if (claves.Count > nroOrdenClave && claves[nroOrdenClave].ColumnName == columna.ColumnName)
                {
                    Modelo.AppendLine("\t\t[Key]");
                    Modelo.AppendLine($"\t\t[Column(Order = { nroOrdenClave.ToString() })]");
                    nroOrdenClave++;
                }
                //Si es un array de byte en realidad es un booleano.
                if (columna.DataType.Name == "Byte[]")
                {
                    Modelo.AppendLine("\t\tpublic bool " + columna.ColumnName + " { get;  set; }");
                }
                else
                {
                    Modelo.AppendLine("\t\tpublic " + capas.Tipo(columna) + " " + columna.ColumnName + " { get;  set; }");
                }

                nroOrdenColumna++;
                if (nroOrdenColumna < columnas.Count)
                {
                    Modelo.AppendLine();
                }
            }

            Modelo.AppendLine("\t}");
            Modelo.AppendLine("}");
            Modelo.AppendLine("\r\n");

            // Elimino cualquier version anterior de la capa
            try
            {
                if (File.Exists(capas.pathClaseModel))
                {
                    File.Delete(capas.pathClaseModel);
                }
            }
            catch (Exception)
            {
            }

            if (CHKmodel.Checked)
            {
                try
                {
                    if (!Directory.Exists(capas.pathModel))
                    {
                        Directory.CreateDirectory(capas.pathModel);
                    }

                    StreamWriter clase = new StreamWriter(capas.pathClaseModel);
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
            bool DB2 = conexionActual.Motor == TipoMotor.DB2;
            string origen = DB2 ? Capas.MODEL : string.Empty;
            string nombreDeClase = capas.TABLA;
            string tipoClase = capas.TABLA + origen;
            string nombreClasePrimeraMinuscula = nombreDeClase[0].ToString().ToLower() + nombreDeClase.Substring(1) + Capas.MODEL;
            string espacioDeNombres = $"{TXTespacioDeNombres.Text.Trim()}.{Utilidades.FormatearCadena(TXTnombreAmigable.Text)}";
            List<string> camposConsulta = (from c in columnas select c.ColumnName).ToList();
            string columnasClave = string.Join(", ", (from c in claves select capas.Tipo(c) + " " + c.ColumnName).ToList());
            List<string[]> clavesConsulta = (from c in claves select new string[] { c.ColumnName, capas.Tipo(c) }).ToList();
            string[] partes = espacioDeNombres.Trim().Length > 0 ? espacioDeNombres.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { };
            string espacio = partes.Length > 0 ? partes[partes.Length - 1] : string.Empty;
            if (OrigenDeDatoSql.Trim().Length == 0 || !CHKinsertarEnProyecto.Checked) OrigenDeDatoSql = $"BaseDeDatos{ espacio }.{ espacio }Entidades";
            StringBuilder Repositories = new StringBuilder();

            Repositories.AppendLine("using System;");
            Repositories.AppendLine("using System.Collections.Generic;");
            Repositories.AppendLine("using System.Data.Entity;");
            Repositories.AppendLine("using System.Data.Odbc;");
            Repositories.AppendLine("using System.Linq;");
            if (!DB2) Repositories.AppendLine("using System.Text;");
            Repositories.AppendLine("using SistemaMunicipalGeneral.Controles;");
            if (espacioDeNombres.Trim().Length > 0) Repositories.AppendLine($"using { espacioDeNombres }.{ Capas.MODEL };");
            Repositories.AppendLine();
            Repositories.AppendLine($"namespace { espacioDeNombres }.{ Capas.REPOSITORIES }");
            Repositories.AppendLine("{");
            Repositories.AppendLine($"\tpublic class { nombreDeClase }Repositories : { nombreDeClase + Capas.REPOSITORIES_INTERFACE}");
            Repositories.AppendLine("\t{");

            #region ALTA
            if (CHKalta.Checked)
            {
                if (DB2)
                {
                    Repositories.AppendLine($"\t\tpublic (string, bool) alta{ nombreDeClase }({ tipoClase } { nombreClasePrimeraMinuscula })");
                    Repositories.AppendLine("\t\t{");
                    Repositories.AppendLine("\t\t\ttry");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine("\t\t\t\tComandoDB2 SQLconsulta = new ComandoDB2(string.Empty, \"DB2_Tributos\");");
                    Repositories.AppendLine($"\t\t\t\tSQLconsulta.Consulta = $@\"INSERT INTO {{ { capas.NombreTabla } }} ({ string.Join(", ", (from c in columnas select c.ColumnName).ToList()) }) ");
                    Repositories.AppendLine($"\t\t\t\t                          VALUES ({ string.Join(",", Enumerable.Repeat("?", columnas.Count)) })\";");
                    Repositories.AppendLine();

                    foreach (DataColumn c in columnas)
                    {
                        Repositories.AppendLine($"\t\t\t\tSQLconsulta.Agregar(\"@{ c.ColumnName }\", { capas.Mapeo[capas.Tipo(c)] }, { nombreClasePrimeraMinuscula }.{ c.ColumnName });");
                    }
                    Repositories.AppendLine();
                    if (CHKtryOrIf.Checked)
                    {
                        Repositories.AppendLine("\t\t\t\tSQLconsulta.Ejecutar(true);");
                        Repositories.AppendLine($"\t\t\t\treturn ($\"Alta correcta de {{ { capas.NombreTabla } }}\", true);");
                    }
                    else
                    {
                        Repositories.AppendLine("\t\t\t\tif(SQLconsulta.EjecutarNonQuery(true) > -1)");
                        Repositories.AppendLine("\t\t\t\t{");
                        Repositories.AppendLine($"\t\t\t\t\treturn ($\"Alta correcta de {{ { capas.NombreTabla } }}\", true);");
                        Repositories.AppendLine("\t\t\t\t}");
                        Repositories.AppendLine("\t\t\t\telse");
                        Repositories.AppendLine("\t\t\t\t{");
                        Repositories.AppendLine($"\t\t\t\t\treturn ($\"Ocurrió un error inesperado al intentar insertar {{ { capas.NombreTabla } }}\", false);");
                        Repositories.AppendLine("\t\t\t\t}");
                    }
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine($"\t\t\t\treturn ($\"Ocurrió un error inesperado al intentar insertar {{ { capas.NombreTabla } }}. {{ (ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message)}}\", false);");
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t}");
                }
                else
                {
                    Repositories.AppendLine($"\t\tpublic (string, bool) alta{ nombreDeClase }({ tipoClase } { nombreClasePrimeraMinuscula })");
                    Repositories.AppendLine("\t\t{");
                    Repositories.AppendLine("\t\t\ttry");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine($"\t\t\t\t{OrigenDeDatoSql}.{ nombreDeClase }.Attach({ nombreClasePrimeraMinuscula });");
                    Repositories.AppendLine($"\t\t\t\t{OrigenDeDatoSql}.Entry({ nombreClasePrimeraMinuscula }).State = EntityState.Added;");
                    Repositories.AppendLine($"\t\t\t\t{OrigenDeDatoSql}.SaveChanges();");
                    Repositories.AppendLine();
                    Repositories.AppendLine($"\t\t\t\treturn ($\"Alta correcta de {{ { capas.NombreTabla } }}\", true);");
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t\tcatch (System.Data.Entity.Validation.DbEntityValidationException ex)");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine("\t\t\t\tStringBuilder entityValidationException = new StringBuilder();");
                    Repositories.AppendLine($"\t\t\t\tentityValidationException.AppendLine($\"{{(ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message)}}\");");
                    Repositories.AppendLine("\t\t\t\tforeach (var eve in ex.EntityValidationErrors)");
                    Repositories.AppendLine("\t\t\t\t{");
                    Repositories.AppendLine($"\t\t\t\t\tentityValidationException.AppendLine($\"Entidad: {{eve.Entry.Entity.GetType().Name}}, Estado: {{eve.Entry.State}}\");");
                    Repositories.AppendLine();
                    Repositories.AppendLine("\t\t\t\t\tforeach (var ve in eve.ValidationErrors)");
                    Repositories.AppendLine("\t\t\t\t\t{");
                    Repositories.AppendLine($"\t\t\t\t\t\tentityValidationException.AppendLine($\" - Propiedad: {{ve.PropertyName}}, Error: {{ve.ErrorMessage}}\");");
                    Repositories.AppendLine("\t\t\t\t\t}");
                    Repositories.AppendLine("\t\t\t\t}");
                    Repositories.AppendLine("\t\t\t\treturn (entityValidationException.ToString(), false);");
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine("\t\t\t\treturn (ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message, false);");
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t}");
                }
                Repositories.AppendLine();
            }
            #endregion

            #region BAJA
            if (CHKbaja.Checked)
            {
                if (DB2)
                {
                    Repositories.AppendLine($"\t\tpublic (string, bool) baja{ nombreDeClase }({ tipoClase } { nombreClasePrimeraMinuscula })");
                    Repositories.AppendLine("\t\t{");
                    Repositories.AppendLine("\t\t\ttry");
                    Repositories.AppendLine("\t\t\t{");
                    List<DataColumn> columnasUpdate = (from c in columnas where !claves.Contains(c) select c).ToList();
                    Repositories.AppendLine("\t\t\t\tComandoDB2 SQLconsulta = new ComandoDB2(string.Empty, \"DB2_Tributos\");");
                    Repositories.AppendLine($"\t\t\t\tSQLconsulta.Consulta = $@\"UPDATE {{ { capas.NombreTabla } }} ");
                    Repositories.AppendLine($"\t\t\t\t                          SET { string.Join(", ", columnasUpdate.Select(c => c.ColumnName + " = ?")) }");
                    Repositories.AppendLine($"\t\t\t\t                          WHERE { string.Join(", ", claves.Select(c => c.ColumnName + " = ?")) }\";");
                    Repositories.AppendLine();
                    Repositories.AppendLine("\t\t\t\t#region UPDATE");

                    foreach (DataColumn c in columnasUpdate)
                    {
                        Repositories.AppendLine($"\t\t\t\tSQLconsulta.Agregar(\"@{ c.ColumnName }\", { capas.Mapeo[capas.Tipo(c)] }, { nombreClasePrimeraMinuscula }.{ c.ColumnName });");
                    }
                    Repositories.AppendLine("\t\t\t\t#endregion UPDATE");
                    Repositories.AppendLine();
                    Repositories.AppendLine("\t\t\t\t#region WHERE");
                    foreach (DataColumn c in claves)
                    {
                        Repositories.AppendLine($"\t\t\t\tSQLconsulta.Agregar(\"@{ c.ColumnName }\", { capas.Mapeo[capas.Tipo(c)] }, { nombreClasePrimeraMinuscula }.{ c.ColumnName });");
                    }
                    Repositories.AppendLine("\t\t\t\t#endregion UPDATE");

                    Repositories.AppendLine();

                    if (CHKtryOrIf.Checked)
                    {
                        Repositories.AppendLine("\t\t\t\tSQLconsulta.Ejecutar(true);");
                        Repositories.AppendLine($"\t\t\t\treturn ($\"Eliminación correcta de {{ { capas.NombreTabla } }}\", true);");
                    }
                    else
                    {
                        Repositories.AppendLine("\t\t\t\tif(SQLconsulta.EjecutarNonQuery(true) > -1)");
                        Repositories.AppendLine("\t\t\t\t{");
                        Repositories.AppendLine($"\t\t\t\t\treturn ($\"Eliminación correcta de {{ { capas.NombreTabla } }}\", true);");
                        Repositories.AppendLine("\t\t\t\t}");
                        Repositories.AppendLine("\t\t\t\telse");
                        Repositories.AppendLine("\t\t\t\t{");
                        Repositories.AppendLine($"\t\t\t\t\treturn ($\"Ocurrió un error inesperado al intentar eliminar {{ { capas.NombreTabla } }}\", false);");
                        Repositories.AppendLine("\t\t\t\t}");
                    }
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine($"\t\t\t\treturn ($\"Ocurrió un error inesperado al intentar eliminar {{ { capas.NombreTabla } }}. {{ (ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message) }}\", false);");
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t}");
                }
                else
                {
                    Repositories.AppendLine($"\t\tpublic (string, bool) baja{ nombreDeClase }({ tipoClase } { nombreClasePrimeraMinuscula })");
                    Repositories.AppendLine("\t\t{");
                    Repositories.AppendLine("\t\t\ttry");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine($"\t\t\t\t{OrigenDeDatoSql}.{ nombreDeClase }.Attach({ nombreClasePrimeraMinuscula });");
                    Repositories.AppendLine($"\t\t\t\t{OrigenDeDatoSql}.Entry({ nombreClasePrimeraMinuscula }).State = EntityState.Modified;");
                    Repositories.AppendLine($"\t\t\t\t{OrigenDeDatoSql}.SaveChanges();");
                    Repositories.AppendLine();
                    Repositories.AppendLine($"\t\t\t\treturn ($\"Eliminación correcta de {{ { capas.NombreTabla } }}\", true);");
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t\tcatch (System.Data.Entity.Validation.DbEntityValidationException ex)");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine("\t\t\t\tStringBuilder entityValidationException = new StringBuilder();");
                    Repositories.AppendLine($"\t\t\t\tentityValidationException.AppendLine($\"{{(ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message)}}\");");
                    Repositories.AppendLine("\t\t\t\tforeach (var eve in ex.EntityValidationErrors)");
                    Repositories.AppendLine("\t\t\t\t{");
                    Repositories.AppendLine($"\t\t\t\t\tentityValidationException.AppendLine($\"Entidad: {{eve.Entry.Entity.GetType().Name}}, Estado: {{eve.Entry.State}}\");");
                    Repositories.AppendLine();
                    Repositories.AppendLine("\t\t\t\t\tforeach (var ve in eve.ValidationErrors)");
                    Repositories.AppendLine("\t\t\t\t\t{");
                    Repositories.AppendLine($"\t\t\t\t\t\tentityValidationException.AppendLine($\" - Propiedad: {{ve.PropertyName}}, Error: {{ve.ErrorMessage}}\");");
                    Repositories.AppendLine("\t\t\t\t\t}");
                    Repositories.AppendLine("\t\t\t\t}");
                    Repositories.AppendLine("\t\t\t\treturn (entityValidationException.ToString(), false);");
                    Repositories.AppendLine("\t\t\t}");

                    Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine("\t\t\t\treturn (ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message, false);");
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t}");
                }
                Repositories.AppendLine();
            }
            #endregion

            #region MODIFICAR
            if (CHKmodificacion.Checked)
            {
                if (DB2)
                {
                    bool where = clavesConsulta.Count > 0 || generarDesdeConsulta;
                    if (where)
                    {
                        Repositories.AppendLine($"\t\tpublic (string, bool) modificacion{ nombreDeClase }({ tipoClase } { nombreClasePrimeraMinuscula })");
                        Repositories.AppendLine("\t\t{");
                        Repositories.AppendLine("\t\t\ttry");
                        Repositories.AppendLine("\t\t\t{");
                        List<DataColumn> columnasUpdate = (from c in columnas where !claves.Contains(c) select c).ToList();
                        Repositories.AppendLine("\t\t\t\tComandoDB2 SQLconsulta = new ComandoDB2(string.Empty, \"DB2_Tributos\");");
                        Repositories.AppendLine($"\t\t\t\tSQLconsulta.Consulta = $@\"UPDATE {{ { capas.NombreTabla } }} ");
                        Repositories.AppendLine($"\t\t\t\t                          SET { string.Join(", ", columnasUpdate.Select(c => c.ColumnName + " = ?")) }");
                        Repositories.AppendLine($"\t\t\t\t                          WHERE { string.Join(", ", claves.Select(c => c.ColumnName + " = ?")) }\";");
                        Repositories.AppendLine();
                        Repositories.AppendLine("\t\t\t\t#region UPDATE");

                        foreach (DataColumn c in columnasUpdate)
                        {
                            Repositories.AppendLine($"\t\t\t\tSQLconsulta.Agregar(\"@{ c.ColumnName }\", { capas.Mapeo[capas.Tipo(c)] }, { nombreClasePrimeraMinuscula }.{ c.ColumnName });");
                        }
                        Repositories.AppendLine("\t\t\t\t#endregion");

                        Repositories.AppendLine();
                        Repositories.AppendLine("\t\t\t\t#region WHERE");
                        foreach (DataColumn c in claves)
                        {
                            Repositories.AppendLine($"\t\t\t\tSQLconsulta.Agregar(\"@{ c.ColumnName }\", { capas.Mapeo[capas.Tipo(c)] }, { nombreClasePrimeraMinuscula }.{ c.ColumnName });");
                        }
                        Repositories.AppendLine("\t\t\t\t#endregion");
                        Repositories.AppendLine();
                        if (CHKtryOrIf.Checked)
                        {
                            Repositories.AppendLine("\t\t\t\tSQLconsulta.Ejecutar(true);");
                            Repositories.AppendLine($"\t\t\t\treturn ($\"Modificación correcta de {{ { capas.NombreTabla } }}\", true);");
                        }
                        else
                        {
                            Repositories.AppendLine("\t\t\t\tif(SQLconsulta.EjecutarNonQuery(true) > -1)");
                            Repositories.AppendLine("\t\t\t\t{");
                            Repositories.AppendLine($"\t\t\t\t\treturn ($\"Modificación correcta de {{ { capas.NombreTabla } }}\", true);");
                            Repositories.AppendLine("\t\t\t\t}");
                            Repositories.AppendLine("\t\t\t\telse");
                            Repositories.AppendLine("\t\t\t\t{");
                            Repositories.AppendLine($"\t\t\t\t\treturn ($\"Ocurrió un error inesperado al intentar modificar {{ { capas.NombreTabla } }}, false);");
                            Repositories.AppendLine("\t\t\t\t}");
                        }
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                        Repositories.AppendLine("\t\t\t{");
                        Repositories.AppendLine($"\t\t\t\treturn ($\"Ocurrió un error inesperado al intentar modificar {{ { capas.NombreTabla } }}. {{ (ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message) }}\", false);");
                        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t}");
                    }
                }
                else
                {
                    Repositories.AppendLine($"\t\tpublic (string, bool) modificacion{ nombreDeClase }({ tipoClase } { nombreClasePrimeraMinuscula })");
                    Repositories.AppendLine("\t\t{");
                    Repositories.AppendLine("\t\t\ttry");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine($"\t\t\t\t{OrigenDeDatoSql}.{ nombreDeClase }.Attach({ nombreClasePrimeraMinuscula });");
                    Repositories.AppendLine($"\t\t\t\t{OrigenDeDatoSql}.Entry({ nombreClasePrimeraMinuscula }).State = EntityState.Modified;");
                    Repositories.AppendLine($"\t\t\t\t{OrigenDeDatoSql}.SaveChanges();");
                    Repositories.AppendLine();
                    Repositories.AppendLine($"\t\t\t\treturn ($\"Modificación correcta de {{ { capas.NombreTabla } }}\", true);");
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t\tcatch (System.Data.Entity.Validation.DbEntityValidationException ex)");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine("\t\t\t\tStringBuilder entityValidationException = new StringBuilder();");
                    Repositories.AppendLine($"\t\t\t\tentityValidationException.AppendLine($\"{{(ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message)}}\");");
                    Repositories.AppendLine("\t\t\t\tforeach (var eve in ex.EntityValidationErrors)");
                    Repositories.AppendLine("\t\t\t\t{");
                    Repositories.AppendLine($"\t\t\t\t\tentityValidationException.AppendLine($\"Entidad: {{eve.Entry.Entity.GetType().Name}}, Estado: {{eve.Entry.State}}\");");
                    Repositories.AppendLine();
                    Repositories.AppendLine("\t\t\t\t\tforeach (var ve in eve.ValidationErrors)");
                    Repositories.AppendLine("\t\t\t\t\t{");
                    Repositories.AppendLine($"\t\t\t\t\t\tentityValidationException.AppendLine($\" - Propiedad: {{ve.PropertyName}}, Error: {{ve.ErrorMessage}}\");");
                    Repositories.AppendLine("\t\t\t\t\t}");
                    Repositories.AppendLine("\t\t\t\t}");
                    Repositories.AppendLine("\t\t\t\treturn (entityValidationException.ToString(), false);");
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine("\t\t\t\treturn (ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message, false);");
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t}");
                }
                Repositories.AppendLine();
            }
            #endregion

            #region OBTENER POR ID
            if (CHKobtenerPorId.Checked)
            {
                if (DB2)
                {
                    Repositories.AppendLine($"\t\tpublic { tipoClase } obtenerPorId({ columnasClave })");
                    Repositories.AppendLine("\t\t{");
                    Repositories.AppendLine($"\t\t\t{ tipoClase } Resultado = new { tipoClase }();");
                    Repositories.AppendLine();
                    Repositories.AppendLine("\t\t\ttry");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine("\t\t\t\tComandoDB2 SQLconsulta = new ComandoDB2(string.Empty, \"DB2_Tributos\");");
                    Repositories.AppendLine();

                    var campoBaja = camposConsulta.Where(c => c.ToLower().Contains("baja") && c.ToLower().StartsWith("f")).FirstOrDefault();

                    bool where = campoBaja != null;
                    if (!where) where = clavesConsulta.Count > 0;

                    Repositories.AppendLine($"\t\t\t\tSQLconsulta.Consulta = $@\"SELECT { string.Join(", ", camposConsulta.ToArray()) } FROM {{ { capas.NombreTabla } }} ");
                    Repositories.AppendLine($"\t\t\t\t                         { (where ? (" WHERE " + string.Join(" AND ", claves.Select(c => c.ColumnName + " = ?")) + (campoBaja != null ? " AND " + campoBaja + " = ?" : string.Empty) + "\";") : string.Empty)}");
                    Repositories.AppendLine();

                    foreach (string[] clave in clavesConsulta)
                    {
                        Repositories.AppendLine($"\t\t\t\tSQLconsulta.Agregar(\"@{ clave[0] }\", { capas.Mapeo[clave[1]] }, { clave[0] });");
                    }
                    if (campoBaja != null)
                    {
                        Repositories.AppendLine($"\t\t\t\tSQLconsulta.Agregar(\"@{ campoBaja }\", OdbcType.DateTime, new System.DateTime(1900, 1, 1));");
                    }
                    Repositories.AppendLine();
                    Repositories.AppendLine("\t\t\t\tif (SQLconsulta.HayRegistros())");
                    Repositories.AppendLine("\t\t\t\t{");
                    string instancia = char.ToLower(nombreDeClase[0]) + tipoClase.Substring(1);
                    Repositories.AppendLine($"\t\t\t\t\t{ tipoClase } { instancia } = new { tipoClase }();");
                    Repositories.AppendLine($"\t\t\t\t\tResultado = FuncionesGenerales.RellenarCampos(SQLconsulta, { instancia }) as { tipoClase };");
                    Repositories.AppendLine("\t\t\t\t};");
                    Repositories.AppendLine();
                    Repositories.AppendLine("\t\t\t\tSQLconsulta.CerrarConexion();");
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine($"\t\t\t\tthrow new Exception ($\"Ocurrió un error inesperado al intentar recuperar los datos por ID de la tabla {{ { capas.NombreTabla } }}. {{ (ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message)}}\");");
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine();
                    Repositories.AppendLine("\t\t\treturn Resultado;");
                    Repositories.AppendLine("\t\t}");
                }
                else
                {
                    Repositories.AppendLine($"\t\tpublic { tipoClase } obtenerPorId({ columnasClave })");
                    Repositories.AppendLine("\t\t{");
                    Repositories.AppendLine($"\t\t\t{ tipoClase } solicitado = null;");
                    Repositories.AppendLine("\t\t\ttry");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine($"\t\t\t\tsolicitado = (from busqueda in {OrigenDeDatoSql}.{ nombreDeClase }");
                    Repositories.AppendLine($"\t\t\t\t\t\t\t  where { string.Join(" && ", claves.Select(c => "busqueda." + c.ColumnName + " == " + c.ColumnName)) }");
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
            #endregion

            #region TODOS
            if (CHKtodos.Checked)
            {
                if (DB2)
                {
                    Repositories.AppendLine($"\t\tpublic List<{ tipoClase }> obtenerTodos()");
                    Repositories.AppendLine("\t\t{");
                    Repositories.AppendLine($"\t\t\tList<{ tipoClase }> todos = new List<{ tipoClase }>();");
                    Repositories.AppendLine();
                    Repositories.AppendLine("\t\t\ttry");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine("\t\t\t\tComandoDB2 SQLconsulta = new ComandoDB2(\"\", \"DB2_Tributos\");");
                    Repositories.AppendLine();
                    Repositories.AppendLine($"\t\t\t\tSQLconsulta.Consulta = $\"SELECT { string.Join(", ", camposConsulta.ToArray()) } FROM {{ { capas.NombreTabla } }}\";");
                    Repositories.AppendLine();
                    Repositories.AppendLine("\t\t\t\twhile (SQLconsulta.HayRegistros())");
                    Repositories.AppendLine("\t\t\t\t{");
                    string instancia = char.ToLower(nombreDeClase[0]) + nombreDeClase.Substring(1);
                    Repositories.AppendLine($"\t\t\t\t\t{ tipoClase } { instancia } = new { tipoClase }();");
                    Repositories.AppendLine($"\t\t\t\t\t{ tipoClase } instancia = FuncionesGenerales.RellenarCampos(SQLconsulta, { instancia }) as { tipoClase };");
                    Repositories.AppendLine("\t\t\t\t\ttodos.Add(instancia);");
                    Repositories.AppendLine("\t\t\t\t}");
                    Repositories.AppendLine("\t\t\t\tSQLconsulta.CerrarConexion();");
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine($"\t\t\t\tthrow new Exception ($\"Ocurrió un error inesperado al intentar recuperar los datos de la tabla {{ { capas.NombreTabla } }}. {{ (ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message) }}\");");
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine();
                    Repositories.AppendLine("\t\t\treturn todos;");
                    Repositories.AppendLine("\t\t}");
                }
                else
                {
                    Repositories.AppendLine($"\t\tpublic List<{ tipoClase }> obtenerTodos()");
                    Repositories.AppendLine("\t\t{");
                    Repositories.AppendLine($"\t\t\treturn (from busqueda in {OrigenDeDatoSql}.{ nombreDeClase }");
                    Repositories.AppendLine("\t\t\t\t\tselect busqueda).ToList();");
                    Repositories.AppendLine("\t\t}");
                }
                Repositories.AppendLine();
            }
            #endregion

            #region RECUPERAR
            if (CHKrecuperacion.Checked)
            {
                if (DB2)
                {
                    Repositories.AppendLine($"\t\tpublic (string, bool) recuperar{ nombreDeClase }({ tipoClase } { nombreClasePrimeraMinuscula })");
                    Repositories.AppendLine("\t\t{");
                    Repositories.AppendLine("\t\t\ttry");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine("\t\t\t\tComandoDB2 SQLconsulta = new ComandoDB2(string.Empty, \"DB2_Tributos\");");
                    Repositories.AppendLine($"\t\t\t\tSQLconsulta.Consulta = $@\"INSERT INTO {{ { capas.NombreTabla } }} ({ string.Join(", ", columnas.Select(c => c.ColumnName)) }) ");
                    Repositories.AppendLine($"\t\t\t\t                          VALUES ({ string.Join(",", Enumerable.Repeat("?", columnas.Count)) })\";");

                    Repositories.AppendLine();
                    foreach (DataColumn c in columnas)
                    {
                        Repositories.AppendLine($"\t\t\t\tSQLconsulta.Agregar(\"@{ c.ColumnName }\", { capas.Mapeo[capas.Tipo(c)] }, { nombreClasePrimeraMinuscula }.{ c.ColumnName });");
                    }
                    Repositories.AppendLine();
                    if (CHKtryOrIf.Checked)
                    {
                        Repositories.AppendLine("\t\t\t\tSQLconsulta.Ejecutar(true);");
                        Repositories.AppendLine($"\t\t\t\treturn ($\"Recuperación correcta de {{ { capas.NombreTabla } }}\", true);");
                    }
                    else
                    {
                        Repositories.AppendLine("\t\t\t\tif(SQLconsulta.EjecutarNonQuery(true) > -1)");
                        Repositories.AppendLine("\t\t\t\t{");
                        Repositories.AppendLine($"\t\t\t\t\treturn ($\"Recuperación correcta de {{ { capas.NombreTabla } }}\", true);");
                        Repositories.AppendLine("\t\t\t\t}");
                        Repositories.AppendLine("\t\t\t\telse");
                        Repositories.AppendLine("\t\t\t\t{");
                        Repositories.AppendLine($"\t\t\t\t\treturn ($\"Ocurrió un error inesperado al intentar recuperar {{ { capas.NombreTabla } }}\", false);");
                        Repositories.AppendLine("\t\t\t\t}");
                    }
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine($"\t\t\t\treturn ($\"Ocurrió un error inesperado al intentar recuperar {{ { capas.NombreTabla } }}. {{ (ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message) }}\", false);");
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t}");
                }
                else
                {
                    Repositories.AppendLine($"\t\tpublic (string, bool) recuperar{ nombreDeClase }({ tipoClase } { nombreClasePrimeraMinuscula })");
                    Repositories.AppendLine("\t\t{");
                    Repositories.AppendLine("\t\t\ttry");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine($"\t\t\t\t{OrigenDeDatoSql}.{ nombreDeClase }.Attach({ nombreClasePrimeraMinuscula });");
                    Repositories.AppendLine($"\t\t\t\t{OrigenDeDatoSql}.Entry({ nombreClasePrimeraMinuscula }).State = EntityState.Modified;");
                    Repositories.AppendLine($"\t\t\t\t{OrigenDeDatoSql}.SaveChanges();");
                    Repositories.AppendLine();
                    Repositories.AppendLine($"\t\t\t\treturn ($\"Recuperación correcta de {{ { capas.NombreTabla } }}\", true);");
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t\tcatch (System.Data.Entity.Validation.DbEntityValidationException ex)");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine("\t\t\t\tStringBuilder entityValidationException = new StringBuilder();");
                    Repositories.AppendLine($"\t\t\t\tentityValidationException.AppendLine($\"{{(ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message)}}\");");
                    Repositories.AppendLine("\t\t\t\tforeach (var eve in ex.EntityValidationErrors)");
                    Repositories.AppendLine("\t\t\t\t{");
                    Repositories.AppendLine($"\t\t\t\t\tentityValidationException.AppendLine($\"Entidad: {{eve.Entry.Entity.GetType().Name}}, Estado: {{eve.Entry.State}}\");");
                    Repositories.AppendLine();
                    Repositories.AppendLine("\t\t\t\t\tforeach (var ve in eve.ValidationErrors)");
                    Repositories.AppendLine("\t\t\t\t\t{");
                    Repositories.AppendLine($"\t\t\t\t\t\tentityValidationException.AppendLine($\" - Propiedad: {{ve.PropertyName}}, Error: {{ve.ErrorMessage}}\");");
                    Repositories.AppendLine("\t\t\t\t\t}");
                    Repositories.AppendLine("\t\t\t\t}");
                    Repositories.AppendLine("\t\t\t\treturn (entityValidationException.ToString(), false);");
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t\tcatch (Exception ex)");
                    Repositories.AppendLine("\t\t\t{");
                    Repositories.AppendLine("\t\t\t\treturn (ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message, false);");
                    Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t}");
                }
            }
            #endregion

            Repositories.AppendLine("\t}");
            Repositories.AppendLine("}");

            // Elimino cualquier version anterior de la capa
            try
            {
                if (File.Exists(capas.pathClaseRepositories))
                {
                    File.Delete(capas.pathClaseRepositories);
                }
            }
            catch (Exception)
            {
            }

            if (CHKrepositories.Checked)
            {
                try
                {
                    if (!Directory.Exists(capas.pathRepositories))
                    {
                        Directory.CreateDirectory(capas.pathRepositories);
                    }

                    StreamWriter clase = new StreamWriter(capas.pathClaseRepositories);
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
            bool DB2 = conexionActual.Motor == TipoMotor.DB2;
            string origen = DB2 ? Capas.MODEL : string.Empty;
            string nombreDeClase = capas.TABLA;
            string tipoClase = capas.TABLA + origen;
            string nombreClasePrimeraMinuscula = nombreDeClase[0].ToString().ToLower() + nombreDeClase.Substring(1);
            string espacioDeNombres = $"{TXTespacioDeNombres.Text.Trim()}.{Utilidades.FormatearCadena(TXTnombreAmigable.Text)}";
            string columnasClave = string.Join(", ", (from c in claves select capas.Tipo(c) + " " + c.ColumnName).ToList());

            StringBuilder RepositoriesInterface = new StringBuilder();

            RepositoriesInterface.AppendLine("using System;");
            RepositoriesInterface.AppendLine("using SistemaMunicipalGeneral.Controles;");
            RepositoriesInterface.AppendLine("using System.Collections.Generic;");
            RepositoriesInterface.AppendLine("using System.Data.Odbc;");
            if (espacioDeNombres.Trim().Length > 0) RepositoriesInterface.AppendLine($"using { espacioDeNombres }.{ Capas.MODEL };");
            RepositoriesInterface.AppendLine();
            RepositoriesInterface.AppendLine($"namespace { espacioDeNombres }.{ Capas.REPOSITORIES}");
            RepositoriesInterface.AppendLine("{");
            RepositoriesInterface.AppendLine($"\tpublic interface { nombreDeClase + Capas.REPOSITORIES_INTERFACE}");
            RepositoriesInterface.AppendLine("\t{");
            if (CHKalta.Checked)
            {
                RepositoriesInterface.AppendLine($"\t\t(string, bool) alta{ nombreDeClase }({ tipoClase } { nombreClasePrimeraMinuscula + Capas.MODEL });");
                RepositoriesInterface.AppendLine();
            }
            if (CHKbaja.Checked)
            {
                RepositoriesInterface.AppendLine($"\t\t(string, bool) baja{ nombreDeClase }({ tipoClase } { nombreClasePrimeraMinuscula + Capas.MODEL });");
                RepositoriesInterface.AppendLine();
            }
            if (CHKmodificacion.Checked)
            {
                RepositoriesInterface.AppendLine($"\t\t(string, bool) modificacion{ nombreDeClase }({ tipoClase } { nombreClasePrimeraMinuscula + Capas.MODEL });");
                RepositoriesInterface.AppendLine();
            }
            if (CHKobtenerPorId.Checked)
            {
                RepositoriesInterface.AppendLine($"\t\t{ tipoClase } obtenerPorId({ columnasClave });");
                RepositoriesInterface.AppendLine();
            }
            if (CHKtodos.Checked)
            {
                RepositoriesInterface.AppendLine($"\t\tList <{ tipoClase }> obtenerTodos();");
                RepositoriesInterface.AppendLine();
            }
            if (CHKrecuperacion.Checked)
            {
                RepositoriesInterface.AppendLine($"\t\t(string, bool) recuperar{ nombreDeClase }({ tipoClase } { nombreClasePrimeraMinuscula + Capas.MODEL });");
            }
            RepositoriesInterface.AppendLine("\t}");
            RepositoriesInterface.AppendLine("}");

            // Elimino cualquier version anterior de la capa
            try
            {
                if (File.Exists(capas.pathClaseRepositoriesInterface))
                {
                    File.Delete(capas.pathClaseRepositoriesInterface);
                }
            }
            catch (Exception)
            {
            }

            if (CHKrepositories.Checked)
            {
                try
                {
                    if (!Directory.Exists(capas.pathRepositories))
                    {
                        Directory.CreateDirectory(capas.pathRepositories);
                    }

                    StreamWriter clase = new StreamWriter(capas.pathClaseRepositoriesInterface);
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
            bool DB2 = conexionActual.Motor == TipoMotor.DB2;
            string origen = DB2 ? Capas.MODEL : Capas.DTO;
            string nombreDeClase = capas.TABLA;
            string tipoClase = capas.TABLA + origen;
            string nombreClasePrimeraMinuscula = nombreDeClase[0].ToString().ToLower() + nombreDeClase.Substring(1) + origen;
            string espacioDeNombres = $"{TXTespacioDeNombres.Text.Trim()}.{Utilidades.FormatearCadena(TXTnombreAmigable.Text)}";
            string columnasClave = string.Join(", ", claves.Select(c => c.ColumnName));
            string columnasClaveTipo = string.Join(", ", claves.Select(c => capas.Tipo(c) + " " + c.ColumnName));

            StringBuilder Service = new StringBuilder();

            Service.AppendLine("using System;");
            Service.AppendLine("using System.Collections.Generic;");
            Service.AppendLine("using SistemaMunicipalGeneral;");
            Service.AppendLine("using System.Linq;");
            Service.AppendLine("using System.Web;");
            if (espacioDeNombres.Trim().Length > 0) Service.AppendLine($"using { espacioDeNombres }.{ origen };");
            if (espacioDeNombres.Trim().Length > 0) Service.AppendLine($"using { espacioDeNombres }.{ Capas.REPOSITORIES };");
            Service.AppendLine();
            Service.AppendLine($"namespace { espacioDeNombres }.{ Capas.SERVICE}");
            Service.AppendLine("{");
            Service.AppendLine($"\tpublic class { nombreDeClase + Capas.SERVICE } : { nombreDeClase + Capas.SERVICE_INTERFACE}");
            Service.AppendLine("\t{");
            Service.AppendLine($"\t\tprivate readonly { nombreDeClase + Capas.REPOSITORIES_INTERFACE } _repositories;");
            Service.AppendLine();

            Service.AppendLine($"\t\tpublic { nombreDeClase + Capas.SERVICE }({ nombreDeClase + Capas.REPOSITORIES_INTERFACE } repositories)");
            Service.AppendLine("\t\t{");
            Service.AppendLine("\t\t\t_repositories = repositories;");
            Service.AppendLine("\t\t}");
            Service.AppendLine();

            #region ALTA
            if (CHKalta.Checked)
            {
                Service.AppendLine($"\t\tpublic (string, bool) alta{ nombreDeClase }({ tipoClase } { nombreClasePrimeraMinuscula })");
                Service.AppendLine("\t\t{");
                Service.AppendLine($"\t\t\t{ nombreDeClase + (DB2 ? origen : string.Empty) } nuevo = new { nombreDeClase + (DB2 ? origen : string.Empty) }()");
                Service.AppendLine("\t\t\t{");

                Dictionary<string, string> camposAlta = new Dictionary<string, string>();
                if (DGValta.Rows.Count > 0)
                {
                    foreach (DataGridViewRow item in DGValta.Rows)
                    {
                        camposAlta.Add(item.Cells[0].FormattedValue.ToString(), $"\t\t\t\t{ item.Cells[0].FormattedValue } = { item.Cells[1].Value.ToString().Replace(";", string.Empty) },");
                    }
                }

                if (DGVmodificacion.Rows.Count > 0)
                {
                    foreach (DataGridViewRow item in DGVmodificacion.Rows)
                    {
                        switch (item.Cells[1].FormattedValue.ToString())
                        {
                            case "FECHA ACTUAL":
                                camposAlta.Add(item.Cells[0].FormattedValue.ToString(), $"\t\t\t\t{ item.Cells[0].FormattedValue } = { Capas.CamposAbmFechas["FECHA POR DEFECTO"].Replace(";", string.Empty) },");
                                break;
                            case "USUARIO MAGIC":
                                camposAlta.Add(item.Cells[0].FormattedValue.ToString(), $"\t\t\t\t{ item.Cells[0].FormattedValue } = { Capas.CamposAbm["CADENA VACÍA"].Replace(";", string.Empty) },");
                                break;
                            case "HORA ACTUAL":
                                camposAlta.Add(item.Cells[0].FormattedValue.ToString(), $"\t\t\t\t{ item.Cells[0].FormattedValue } = { Capas.CamposAbmHoras["HORA POR DEFECTO"].Replace(";", string.Empty) },");
                                break;
                            default:
                                camposAlta.Add(item.Cells[0].FormattedValue.ToString(), $"\t\t\t\t{ item.Cells[0].FormattedValue } = { item.Cells[1].Value.ToString().Replace(";", string.Empty) },");
                                break;
                        }
                    }
                }

                if (DGVbaja.Rows.Count > 0)
                {
                    foreach (DataGridViewRow item in DGVbaja.Rows)
                    {
                        switch (item.Cells[1].FormattedValue.ToString())
                        {
                            case "FECHA ACTUAL":
                                camposAlta.Add(item.Cells[0].FormattedValue.ToString(), $"\t\t\t\t{ item.Cells[0].FormattedValue } = { Capas.CamposAbmFechas["FECHA POR DEFECTO"].Replace(";", string.Empty) },");
                                break;
                            case "USUARIO MAGIC":
                                camposAlta.Add(item.Cells[0].FormattedValue.ToString(), $"\t\t\t\t{ item.Cells[0].FormattedValue } = { Capas.CamposAbm["CADENA VACÍA"].Replace(";", string.Empty) },");
                                break;
                            case "HORA ACTUAL":
                                camposAlta.Add(item.Cells[0].FormattedValue.ToString(), $"\t\t\t\t{ item.Cells[0].FormattedValue } = { Capas.CamposAbmHoras["HORA POR DEFECTO"].Replace(";", string.Empty) },");
                                break;
                            default:
                                camposAlta.Add(item.Cells[0].FormattedValue.ToString(), $"\t\t\t\t{ item.Cells[0].FormattedValue } = { item.Cells[1].Value.ToString().Replace(";", string.Empty) },");
                                break;
                        }
                    }
                }

                int i = 0;
                foreach (DataColumn columna in columnas)
                {
                    if (camposAlta.Count > 0 && camposAlta.ContainsKey(columna.ColumnName))
                    {
                        Service.AppendLine(camposAlta[columna.ColumnName]);
                    }
                    else
                    {
                        Service.AppendLine($"\t\t\t\t{ columna.ColumnName } = { nombreClasePrimeraMinuscula }.{ columna.ColumnName + (i < columnas.Count ? "," : string.Empty)}");
                    }
                    i++;
                }
                Service.AppendLine("\t\t\t};");
                Service.AppendLine($"\t\t\t(string, bool) respuesta = _repositories.alta{ nombreDeClase }(nuevo);");
                Service.AppendLine();
                Service.AppendLine("\t\t\treturn respuesta;");
                Service.AppendLine("\t\t}");
                Service.AppendLine();
            }
            #endregion

            #region BAJA
            if (CHKbaja.Checked)
            {
                Service.AppendLine($"\t\tpublic (string, bool) baja{ nombreDeClase }({ columnasClaveTipo }, int codigoBaja, string motivoBaja)");
                Service.AppendLine("\t\t{");
                Service.AppendLine($"\t\t\t{ nombreDeClase + (DB2 ? origen : string.Empty) } solicitado = _repositories.obtenerPorId({ columnasClave });");
                Service.AppendLine("\t\t\tif (solicitado != null)");
                Service.AppendLine("\t\t\t{");

                Dictionary<string, string> camposBaja = new Dictionary<string, string>();
                if (DGVbaja.Rows.Count == 0)
                {
                    camposBaja.Add("FechaBaja", "\t\t\t\tsolicitado.FechaBaja = System.DateTime.Now;");
                    camposBaja.Add("UsuarioBaja", "\t\t\t\tsolicitado.UsuarioBaja = Config.UsuarioMagic;");
                    camposBaja.Add("CodigoBaja", "\t\t\t\tsolicitado.CodigoBaja = codigoBaja;");
                    camposBaja.Add("MotivoBaja", "\t\t\t\tsolicitado.MotivoBaja = motivoBaja;");
                }
                else
                {
                    foreach (DataGridViewRow item in DGVbaja.Rows)
                    {
                        camposBaja.Add(item.Cells[0].FormattedValue.ToString(), $"\t\t\t\tsolicitado.{ item.Cells[0].FormattedValue } = { item.Cells[1].Value}");
                    }
                }

                bool existeCoincidencia = false;
                foreach (DataColumn columna in columnas)
                {
                    if (camposBaja.ContainsKey(columna.ColumnName))
                    {
                        Service.AppendLine(camposBaja[columna.ColumnName]);
                        existeCoincidencia = true;
                    }
                }
                if (!existeCoincidencia)
                {
                    Service.AppendLine("\t\t\t\t// SIN COINCIDENCIAS PARA ASIGNAR");
                }

                Service.AppendLine("\t\t\t}");
                Service.AppendLine($"\t\t\t(string, bool) respuesta = _repositories.baja{ nombreDeClase }(solicitado);");
                Service.AppendLine();
                Service.AppendLine("\t\t\treturn respuesta;");
                Service.AppendLine("\t\t}");
                Service.AppendLine();
            }
            #endregion

            #region MODIFICACION
            if (CHKmodificacion.Checked)
            {
                Service.AppendLine($"\t\tpublic (string, bool) modificacion{ nombreDeClase }({ tipoClase } { nombreClasePrimeraMinuscula })");
                Service.AppendLine("\t\t{");
                string columnasBusqueda = string.Join(", ", claves.Select(c => nombreClasePrimeraMinuscula + "." + c.ColumnName));
                Service.AppendLine($"\t\t\t{ nombreDeClase + (DB2 ? origen : string.Empty) } solicitado = _repositories.obtenerPorId({ columnasBusqueda });");
                Service.AppendLine("\t\t\tif (solicitado != null)");
                Service.AppendLine("\t\t\t{");
                Dictionary<string, string> camposModificacion = new Dictionary<string, string>();

                if (DGVmodificacion.Rows.Count == 0)
                {
                    camposModificacion.Add("FechaModificacion", "\t\t\t\tsolicitado.FechaModificacion = System.DateTime.Now;");
                    camposModificacion.Add("UsuarioModificacion", "\t\t\t\tsolicitado.UsuarioModificacion = Config.UsuarioMagic;");
                }
                else
                {
                    foreach (DataGridViewRow item in DGVmodificacion.Rows)
                    {
                        camposModificacion.Add(item.Cells[0].FormattedValue.ToString(), $"\t\t\t\tsolicitado.{ item.Cells[0].FormattedValue } = { item.Cells[1].Value}");
                    }
                }

                foreach (DataColumn columna in columnas)
                {
                    if (camposModificacion.ContainsKey(columna.ColumnName))
                    {
                        Service.AppendLine(camposModificacion[columna.ColumnName]);
                    }
                    else
                    {
                        Service.AppendLine($"\t\t\t\tsolicitado.{ columna.ColumnName } = { nombreClasePrimeraMinuscula }.{ columna.ColumnName };");
                    }
                }

                Service.AppendLine("\t\t\t}");
                Service.AppendLine($"\t\t\t(string, bool) respuesta = _repositories.modificacion{ nombreDeClase }(solicitado);");
                Service.AppendLine();
                Service.AppendLine("\t\t\treturn respuesta;");
                Service.AppendLine("\t\t}");
                Service.AppendLine();
            }
            #endregion

            #region OBTENER POR ID
            if (CHKobtenerPorId.Checked)
            {
                Service.AppendLine($"\t\tpublic { tipoClase } obtenerPorId({ columnasClaveTipo })");
                Service.AppendLine("\t\t{");
                Service.AppendLine($"\t\t\t{ nombreDeClase + (DB2 ? origen : string.Empty) } solicitado = _repositories.obtenerPorId({ columnasClave });");
                Service.AppendLine("\t\t\tif (solicitado != null)");
                Service.AppendLine("\t\t\t{");
                if (DB2)
                {
                    Service.AppendLine("\t\t\t\treturn solicitado;");
                }
                else
                {
                    Service.AppendLine($"\t\t\t\t{ nombreDeClase + origen } solicitado{ origen } = new { nombreDeClase + origen }();");
                    Service.AppendLine();
                    Service.AppendLine($"\t\t\t\tsolicitado{ origen }.new{ nombreDeClase + origen }(solicitado);");
                    Service.AppendLine($"\t\t\t\treturn solicitado" + origen + ";");
                }
                Service.AppendLine("\t\t\t}");
                Service.AppendLine("\t\t\telse");
                Service.AppendLine("\t\t\t{");
                Service.AppendLine("\t\t\t\treturn null;");
                Service.AppendLine("\t\t\t}");
                Service.AppendLine("\t\t}");
                Service.AppendLine();
            }
            #endregion

            #region TODOS
            if (CHKtodos.Checked)
            {
                Service.AppendLine($"\t\tpublic List<{ tipoClase }> obtenerTodos()");
                Service.AppendLine("\t\t{");
                Service.AppendLine($"\t\t\tList<{ nombreDeClase + (DB2 ? origen : string.Empty) }> listado = new List<{ nombreDeClase + (DB2 ? origen : string.Empty) }>();");
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
                    Service.AppendLine($"\t\t\t\tList<{ nombreDeClase + origen }> { nombreClasePrimeraMinuscula } = new List<{ nombreDeClase + origen }>();");
                    Service.AppendLine($"\t\t\t\tforeach (" + nombreDeClase + " model in listado)");
                    Service.AppendLine("\t\t\t\t{");
                    Service.AppendLine($"\t\t\t\t\t{ nombreDeClase + origen } dto = new { nombreDeClase + origen }();");
                    Service.AppendLine($"\t\t\t\t\t{ nombreClasePrimeraMinuscula }.Add(dto.new{ nombreDeClase + origen }(model));");
                    Service.AppendLine("\t\t\t\t}");
                    Service.AppendLine($"\t\t\t\treturn { nombreClasePrimeraMinuscula };");
                }
                Service.AppendLine("\t\t\t}");
                Service.AppendLine("\t\t\treturn null;");
                Service.AppendLine("\t\t}");
                Service.AppendLine();
            }
            #endregion

            #region RECUPERACION
            if (CHKrecuperacion.Checked)
            {
                Service.AppendLine($"\t\tpublic (string, bool) recuperar{ nombreDeClase }({ columnasClaveTipo })");
                Service.AppendLine("\t\t{");
                Service.AppendLine($"\t\t\t{ nombreDeClase + (DB2 ? origen : string.Empty) } solicitado = _repositories.obtenerPorId({ columnasClave });");
                Service.AppendLine("\t\t\tif (solicitado != null)");
                Service.AppendLine("\t\t\t{");
                Dictionary<string, string> camposRecuperacion = new Dictionary<string, string>();

                if (DGVrecuperacion.Rows.Count == 0)
                {
                    camposRecuperacion.Add("FechaBaja", "\t\t\t\tsolicitado.FechaBaja = new DateTime(1900, 1, 1);");
                    camposRecuperacion.Add("UsuarioBaja", "\t\t\t\tsolicitado.UsuarioBaja = string.Empty;");
                    camposRecuperacion.Add("CodigoBaja", "\t\t\t\tsolicitado.CodigoBaja = 0;");
                    camposRecuperacion.Add("MotivoBaja", "\t\t\t\tsolicitado.MotivoBaja = string.Empty;");
                }
                else
                {
                    foreach (DataGridViewRow item in DGVrecuperacion.Rows)
                    {
                        camposRecuperacion.Add(item.Cells[0].FormattedValue.ToString(), $"\t\t\t\tsolicitado.{ item.Cells[0].FormattedValue } = { item.Cells[1].Value}");
                    }
                }

                bool existeCoincidencia = false;
                foreach (DataColumn columna in columnas)
                {
                    if (camposRecuperacion.ContainsKey(columna.ColumnName))
                    {
                        Service.AppendLine(camposRecuperacion[columna.ColumnName]);
                        existeCoincidencia = true;
                    }
                }
                if (!existeCoincidencia)
                {
                    Service.AppendLine("\t\t\t\t// SIN COINCIDENCIAS PARA ASIGNAR");
                }

                Service.AppendLine("\t\t\t}");
                Service.AppendLine($"\t\t\t(string, bool) respuesta = _repositories.recuperar{ nombreDeClase }(solicitado);");
                Service.AppendLine();
                Service.AppendLine("\t\t\treturn respuesta;");
                Service.AppendLine("\t\t}");
            }
            #endregion

            Service.AppendLine("\t}");
            Service.AppendLine("}");

            // Elimino cualquier version anterior de la capa
            try
            {
                if (File.Exists(capas.pathClaseService))
                {
                    File.Delete(capas.pathClaseService);
                }
            }
            catch (Exception)
            {
            }

            if (CHKservice.Checked)
            {
                try
                {
                    if (!Directory.Exists(capas.pathService))
                    {
                        Directory.CreateDirectory(capas.pathService);
                    }

                    StreamWriter clase = new StreamWriter(capas.pathClaseService);
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
            bool DB2 = conexionActual.Motor == TipoMotor.DB2;
            string origen = DB2 ? Capas.MODEL : Capas.DTO;
            string nombreDeClase = capas.TABLA;
            string tipoClase = capas.TABLA + origen;
            string nombreClasePrimeraMinuscula = nombreDeClase[0].ToString().ToLower() + nombreDeClase.Substring(1) + origen;
            string espacioDeNombres = $"{TXTespacioDeNombres.Text.Trim()}.{Utilidades.FormatearCadena(TXTnombreAmigable.Text)}";
            string columnasClave = string.Join(", ", (from c in claves select capas.Tipo(c) + " " + c.ColumnName).ToList());

            StringBuilder ServiceInterface = new StringBuilder();

            ServiceInterface.AppendLine("using System;");
            ServiceInterface.AppendLine("using System.Collections.Generic;");
            ServiceInterface.AppendLine("using System.Data.Odbc;");
            ServiceInterface.AppendLine("using SistemaMunicipalGeneral.Controles;");
            if (espacioDeNombres.Trim().Length > 0) ServiceInterface.AppendLine($"using { espacioDeNombres }.{ origen };");
            ServiceInterface.AppendLine();
            ServiceInterface.AppendLine($"namespace { espacioDeNombres }.{ Capas.SERVICE}");
            ServiceInterface.AppendLine("{");
            ServiceInterface.AppendLine($"\tpublic interface { nombreDeClase + Capas.SERVICE_INTERFACE}");
            ServiceInterface.AppendLine("\t{");
            if (CHKalta.Checked)
            {
                ServiceInterface.AppendLine($"\t\t(string, bool) alta{ nombreDeClase }({ tipoClase } { nombreClasePrimeraMinuscula });");
                ServiceInterface.AppendLine();
            }
            if (CHKbaja.Checked)
            {
                ServiceInterface.AppendLine($"\t\t(string, bool) baja{ nombreDeClase }({ columnasClave }, int codigoBaja, string motivoBaja);");
                ServiceInterface.AppendLine();
            }
            if (CHKmodificacion.Checked)
            {
                ServiceInterface.AppendLine($"\t\t(string, bool) modificacion{ nombreDeClase }({ tipoClase } { nombreClasePrimeraMinuscula });");
                ServiceInterface.AppendLine();
            }
            if (CHKobtenerPorId.Checked)
            {
                ServiceInterface.AppendLine($"\t\t{ tipoClase } obtenerPorId({ columnasClave });");
                ServiceInterface.AppendLine();
            }
            if (CHKtodos.Checked)
            {
                ServiceInterface.AppendLine($"\t\tList<{ tipoClase }> obtenerTodos();");
                ServiceInterface.AppendLine();
            }
            if (CHKrecuperacion.Checked)
            {
                ServiceInterface.AppendLine($"\t\t(string, bool) recuperar{ nombreDeClase }({ columnasClave });");
            }
            ServiceInterface.AppendLine("\t}");
            ServiceInterface.AppendLine("}");

            // Elimino cualquier version anterior de la capa
            try
            {
                if (File.Exists(capas.pathClaseServiceInterface))
                {
                    File.Delete(capas.pathClaseServiceInterface);
                }
            }
            catch (Exception)
            {
            }

            if (CHKservice.Checked)
            {
                try
                {
                    if (!Directory.Exists(capas.pathService))
                    {
                        Directory.CreateDirectory(capas.pathService);
                    }

                    StreamWriter clase = new StreamWriter(capas.pathClaseServiceInterface);
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

        private string ArmarGlobal()
        {
            StringBuilder Global = new StringBuilder();

            string espacioDeNombres = $"{TXTespacioDeNombres.Text.Trim()}.{Utilidades.FormatearCadena(TXTnombreAmigable.Text)}";

            Global.AppendLine("***** AGREGAR USINGS *****");
            Global.AppendLine();

            Global.AppendLine($"using { espacioDeNombres }.{ Capas.REPOSITORIES };");
            Global.AppendLine($"using { espacioDeNombres }.{ Capas.SERVICE };");

            Global.AppendLine();
            Global.AppendLine("***** AGREGAR REGISTROS *****");
            Global.AppendLine();

            Global.AppendLine($"\t\t\tbuilder.RegisterType<{ capas.TABLA + Capas.REPOSITORIES }>()");
            Global.AppendLine($"\t\t\t\t\t.As<{ capas.TABLA + Capas.REPOSITORIES_INTERFACE }>()");
            Global.AppendLine("\t\t\t\t\t.InstancePerRequest();");
            Global.AppendLine($"\t\t\tbuilder.RegisterType<{ capas.TABLA + Capas.SERVICE }>()");
            Global.AppendLine($"\t\t\t\t\t.As<{ capas.TABLA + Capas.SERVICE_INTERFACE }>()");
            Global.AppendLine($"\t\t\t\t\t.InstancePerRequest();");

            // Elimino cualquier version anterior de la capa
            try
            {
                if (File.Exists(capas.pathGlobal))
                {
                    File.Delete(capas.pathGlobal);
                }

                StreamWriter clase = new StreamWriter(capas.pathGlobal);
                clase.Write(Global.ToString());
                clase.Flush();
                clase.Close();
            }
            catch (Exception)
            {
            }

            return Global.ToString();
        }

        #endregion

        #region FRONT

        private string ArmarClaseTypeScript(List<DataColumn> columnas)
        {
            string nombreDeClase = capas.TABLA;
            StringBuilder TypeSript = new StringBuilder();

            TypeSript.AppendLine($"export class { nombreDeClase }{{");
            TypeSript.AppendLine($"\tconstructor(init?: Partial<{ nombreDeClase }>) {{");
            TypeSript.AppendLine("\t\tObject.assign(this, init);");
            TypeSript.AppendLine("\t}");

            foreach (var columna in columnas)
            {
                TypeSript.AppendLine($"\tpublic { columna.ColumnName }: { capas.PropiedadesTS[columna.DataType]}");
            }
            TypeSript.AppendLine("}");

            // Elimino cualquier version anterior de la capa
            try
            {
                if (File.Exists(capas.pathClaseTypeScript))
                {
                    File.Delete(capas.pathClaseTypeScript);
                }
            }
            catch (Exception)
            {
            }

            if (CHKclaseTypeScript.Checked)
            {
                try
                {
                    if (!Directory.Exists(capas.pathTypeScript))
                    {
                        Directory.CreateDirectory(capas.pathTypeScript);
                    }

                    StreamWriter clase = new StreamWriter(capas.pathClaseTypeScript);
                    clase.Write(TypeSript.ToString());
                    clase.Flush();
                    clase.Close();
                }
                catch (Exception err)
                {
                    CustomMessageBox.Show(err.Message);
                }
            }

            return TypeSript.ToString();
        }

        private string ArmarABM(List<DataColumn> columnas)
        {
            string nombreDeClase = capas.TABLA;
            StringBuilder TypeSript = new StringBuilder();

            TypeSript.AppendLine($"export class { nombreDeClase }{{");
            TypeSript.AppendLine($"\tconstructor(init?: Partial<{ nombreDeClase }>) {{");
            TypeSript.AppendLine("\t\tObject.assign(this, init);");
            TypeSript.AppendLine("\t}");

            foreach (var columna in columnas)
            {
                TypeSript.AppendLine($"\tpublic { columna.ColumnName }: { capas.PropiedadesTS[columna.DataType]}");
            }
            TypeSript.AppendLine("}");

            // Elimino cualquier version anterior de la capa
            if (CHKclaseTypeScript.Checked)
            {
                try
                {
                    if (File.Exists(capas.pathClaseTypeScript))
                    {
                        File.Delete(capas.pathClaseTypeScript);
                    }

                    StreamWriter clase = new StreamWriter(capas.pathClaseTypeScript);
                    clase.Write(TypeSript.ToString());
                    clase.Flush();
                    clase.Close();
                }
                catch (Exception)
                {
                }
            }

            if (CHKclaseTypeScript.Checked)
            {
                try
                {
                    string pathTypeScript = TXTpathCapas.Text + @"\" + capas.TABLA + @"\TypeScript\";
                    string pathClaseTypeScript = pathTypeScript + capas.TABLA + ".ts";
                    if (!Directory.Exists(pathTypeScript))
                    {
                        Directory.CreateDirectory(pathTypeScript);
                    }

                    StreamWriter clase = new StreamWriter(pathClaseTypeScript);
                    clase.Write(TypeSript.ToString());
                    clase.Flush();
                    clase.Close();
                }
                catch (Exception)
                {
                }
            }

            return TypeSript.ToString();
        }

        #endregion

        #endregion

        private void GenerarDesdeTabla()
        {
            WaitCursor();
            if (TXTpathCapas.Text.Trim().Length > 0)
            {
                if (TXTnombreAmigable.Text.Trim().Length == 0)
                {
                    CustomMessageBox.Show("Indique un 'nombre amigable' para la carpeta en el proyecto!", CustomMessageBox.ATENCION, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    if (ComprobarClaves())
                    {
                        generarDesdeConsulta = false;
                        GuardarConfiguracion();
                    }
                }
            }
            else
            {
                CustomMessageBox.Show("Seleccione una carpeta donde guardar las capas!", CustomMessageBox.ATENCION, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                DefinirDirectorioCapas();
            }
            CursorDefault();
        }

        private void GenerarDesdeConsulta()
        {
            WaitCursor();
            if (TXTpathCapas.Text.Trim().Length > 0)
            {
                if (TXTnombreAmigable.Text.Trim().Length == 0)
                {
                    CustomMessageBox.Show("Indique un 'nombre amigable' para la carpeta en el proyecto!", CustomMessageBox.ATENCION, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
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
                                $"Generar en estos 3 últimos el método de:\r\n   • {Utilidades.FormatearTitulo(capasNecesarias[0].Name.Replace("CHK", string.Empty))}\r\npuede generar inconsistencias.\r\n" +
                                $"¿Desea generarlo de todos modos?";
                        }
                        else
                        {
                            string metodos = string.Join("\r\n   • ", (from c in capasNecesarias select Utilidades.FormatearTitulo(c.Name.Replace("CHK", string.Empty))).ToArray());
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

                    if (CamposTabla("CONSULTA", TXTgenerarAPartirDeConsulta.Text))
                    {
                        GuardarConfiguracion();
                    }
                }
            }
            else
            {
                CustomMessageBox.Show("Seleccione una carpeta donde guardar las capas!", CustomMessageBox.ATENCION, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                DefinirDirectorioCapas();
            }

            CursorDefault();
        }

        private void CMBtablas_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                generarDesdeConsulta = false;
                CamposTabla();
            }
            catch (Exception err)
            {
                CustomMessageBox.Show($"ERROR EN EL CAMBIO DE TABLA: {err.Message}");
            }
        }

        private StringConnection DefinirStringConnection()
        {
            StringConnection stringConnection = new StringConnection();
            try
            {
                if (conexionActual != null)
                {
                    stringConnection.TipoConexion = conexionActual.Motor == TipoMotor.DB2 ? StringConnection.Motor.DB2 : StringConnection.Motor.SQL;
                    stringConnection.Servidor = conexionActual.Servidor.ToUpper();
                    stringConnection.BaseDeDatos = conexionActual.BaseDatos; 
                }
            }
            catch (Exception err)
            {
            }
            return stringConnection;
        }

        private void TablasBase()
        {
            try
            {
                CMBtablas.Items.Clear();
                CMBtablas.Text = string.Empty;
                LBLtablaSeleccionada.Text = string.Empty;
                LSVcampos.Items.Clear();
                capas.tablasBase = new List<string>();
                StringConnection stringConnection = DefinirStringConnection();
                if (conexionActual.Motor == TipoMotor.DB2)
                {
                    try
                    {
                        DataLayer.DataBase DB2base = new DataLayer.DataBase(new OdbcConnection(stringConnection.Obtener()));
                        DB2base.OpenConnection();
                        IDataReader consulta = DB2base.DataReader("SELECT LTRIM(RTRIM(TBNAME)) AS Nombre, COUNT(NAME) FROM SYSIBM.SYSCOLUMNS WHERE TBCREATOR = 'DB2ADMIN' GROUP BY LTRIM(RTRIM(TBNAME)) ORDER BY Nombre");

                        while (consulta.Read())
                        {
                            capas.tablasBase.Add(consulta.GetString(0));
                            CMBtablas.Items.Add(consulta.GetString(0));
                        }
                        DB2base.CloseConnection();
                    }
                    catch (Exception ex)
                    {
                        //throw ex;
                    }
                }
                else
                {
                    DataLayer.DataBase SQLbase = new DataLayer.DataBase(new OdbcConnection(stringConnection.Obtener()));
                    using (SQLbase.Connection)
                    {
                        try
                        {
                            SQLbase.Connection.Open();

                            string query = @"SELECT TABLE_SCHEMA, TABLE_NAME 
                                FROM INFORMATION_SCHEMA.TABLES 
                                WHERE TABLE_TYPE = 'BASE TABLE'
                                ORDER BY TABLE_SCHEMA, TABLE_NAME";

                            IDataReader reader = SQLbase.DataReader(query);

                            while (reader.Read())
                            {
                                string schema = reader.GetString(0);
                                string table = reader.GetString(1);
                                capas.tablasBase.Add($"{schema}.{table}");
                                CMBtablas.Items.Add($"{schema}.{table}");
                            }
                        }
                        catch (Exception ex)
                        {
                            //throw ex;
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

        private bool CamposTabla(string tabla = "", string consulta = "")
        {
            WaitCursor();
            bool camposOk = true;
            if (cargarCampos)
            {
                capas.camposTabla = new List<string>();
                try
                {
                    LBLtablaSeleccionada.Text = (tabla.Trim().Length > 0 ? tabla : CMBtablas.Items[CMBtablas.SelectedIndex].ToString()) + ":";
                    LSVcampos.Items.Clear();
                    string tablaSeleccionada = (tabla.Trim().Length > 0 ? tabla : CMBtablas.Items[CMBtablas.SelectedIndex].ToString());
                    StringConnection stringConnection = DefinirStringConnection();
                    // BASE DE DATOS DB2
                    if (conexionActual.Motor == TipoMotor.DB2)
                    {
                        try
                        {
                            DataLayer.DataBase DB2base = new DataLayer.DataBase(new OdbcConnection(stringConnection.Obtener()));
                            DB2base.OpenConnection();
                            // GENERO DESDE UNA CONSULTA
                            if (overlay.IsDisposed && generarDesdeConsulta && consulta.Trim().Length > 0)
                            {
                                if (!consulta.Trim().ToUpper().Contains("FETCH"))
                                {
                                    DialogResult resultado = CustomMessageBox.Show("La consulta de la que intenta obtener una estructura de tabla no posee la clausula FETCH y puede ser que tarde mucho en ejecutarse.\r\n   • Si desea continuar de todas maneras, presione Sí.\r\n   • Si desea agregar la clausula, presione No", CustomMessageBox.GUATEFAK, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                                    switch (resultado)
                                    {
                                        case DialogResult.Cancel:
                                            CursorDefault();
                                            camposOk = false;
                                            break;
                                        case DialogResult.No:
                                            consulta += " FETCH FIRST 1 ROW ONLY";
                                            TXTgenerarAPartirDeConsulta.Text = consulta;
                                            break;
                                    }
                                }
                                if (camposOk)
                                {
                                    IDataReader Db2 = DB2base.DataReader(consulta);
                                    Db2.Read();

                                    using (Db2)
                                    {
                                        CargarListViewDesdeEsquema(Db2, true);
                                    }
                                }
                            }
                            else // GENERO DESDE UNA TABLA
                            {
                                IDataReader Db2 = DB2base.DataReader("SELECT LTRIM(RTRIM(NAME)) AS Nombre, COLTYPE as Tipo, LENGTH as Longitud, SCALE as Escala, CASE WHEN NULLS = 'N' THEN 'NO' ELSE 'SÍ' END as AceptaNulos FROM SYSIBM.SYSCOLUMNS WHERE TBNAME = '" + tablaSeleccionada + "'");

                                Db2.Read();
                                do
                                {
                                    CargarListViewDesdeReader(Db2);
                                }
                                while (Db2.Read());

                                if (LSVcampos.Items.Count > 0)
                                {
                                    Db2 = DB2base.DataReader("SELECT UPPER(COLNAMES) AS Clave FROM SYSCAT.INDEXES WHERE TABNAME = '" + tablaSeleccionada + "' AND UNIQUERULE IN ('U')");
                                    List<string> claves = new List<string>();
                                    while (Db2.Read())
                                    {
                                        claves.Add(Db2.GetString(0));
                                    }

                                    if (claves.Count > 0)
                                    {
                                        int minCantidad = claves.Min(s => s.Count(c => c == '+'));

                                        // Paso 2: filtrar los strings con esa cantidad mínima
                                        List<string> clave = claves
                                            .Where(s => s.Count(c => c == '+') == minCantidad)
                                            .Select(s => s.Split('+'))
                                            .FirstOrDefault().ToList();
                                        // Eliminar los elementos vacíos
                                        clave.RemoveAll(s => string.IsNullOrWhiteSpace(s));

                                        foreach (ListViewItem item in LSVcampos.Items)
                                        {
                                            if (clave.Contains(item.SubItems[0].Text))
                                            {
                                                //item.Checked = true;
                                                item.ImageKey = KEY;
                                            }
                                        }
                                        LSVcampos.Refresh();
                                    }
                                }
                            }
                            DB2base.CloseConnection();
                        }
                        catch (Exception ex)
                        {
                            camposOk = false;
                            var error = ex.InnerException ?? ex;
                            if (consulta.Trim().Length > 0)
                            {
                                CustomMessageBox.Show("Ocurrió un error al intentar obtener la estructura de la consulta:\r\n" + error.Message, CustomMessageBox.ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                CustomMessageBox.Show("Ocurrió un error al intentar obtener la estructura de la tabla:\r\n" + tablaSeleccionada.ToUpper() + "\r\n" + error.Message, CustomMessageBox.ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else // BASE DE DATOS MS SQL
                    {
                        try
                        {
                            bool comienzaConSelectTop = Regex.IsMatch(consulta, @"^\s*SELECT\s+TOP\s", RegexOptions.IgnoreCase);

                            if (overlay.IsDisposed && generarDesdeConsulta && !comienzaConSelectTop)
                            {
                                DialogResult resultado = CustomMessageBox.Show("La consulta de la que intenta obtener una estructura de tabla no posee la clausula TOP al inicio y puede ser que tarde mucho en ejecutarse.\r\n   • Si desea continuar de todas maneras, presione Sí.\r\n   • Si desea agregar la clausula, presione No", CustomMessageBox.GUATEFAK, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                                switch (resultado)
                                {
                                    case DialogResult.Cancel:
                                        CursorDefault();
                                        camposOk = false;
                                        break;
                                    case DialogResult.No:
                                        consulta = Regex.Replace(
                                                        consulta,
                                                        @"^\s*SELECT\b",     // busca SELECT al inicio, permitiendo espacios previos
                                                        "SELECT TOP 1",
                                                        RegexOptions.IgnoreCase
                                                    );
                                        TXTgenerarAPartirDeConsulta.Text = consulta;
                                        break;
                                }
                            }

                            if (camposOk)
                            {
                                string query = (overlay.IsDisposed && generarDesdeConsulta && consulta.Trim().Length > 0) ? consulta : $@"SELECT c.name AS Nombre, ty.name AS Tipo, c.max_length AS Longitud, c.scale AS Escala, CASE WHEN c.is_nullable = 1 THEN 'SÍ' ELSE 'NO' END AS AceptaNulos "
                                                    + "FROM sys.columns c "
                                                    + "JOIN sys.types ty ON c.user_type_id = ty.user_type_id "
                                                    + "JOIN sys.tables t ON c.object_id = t.object_id "
                                                    + "WHERE t.name = @tabla "
                                                    + "ORDER BY c.column_id;";

                                DataLayer.DataBase SQLbase = new DataLayer.DataBase(new OdbcConnection(stringConnection.Obtener()));

                                if (consulta.Trim().Length == 0)
                                {
                                    string[] partes = tablaSeleccionada.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                                    tablaSeleccionada = partes[partes.Length - 1];
                                }
                                query = query.Replace("@tabla", $"'{tablaSeleccionada}'");

                                try
                                {
                                    SQLbase.OpenConnection();
                                    IDataReader reader = SQLbase.DataReader(query);
                                    using (reader)
                                    {
                                        // GENERO DESDE UNA CONSULTA
                                        if (overlay.IsDisposed && generarDesdeConsulta && consulta.Trim().Length > 0)
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
                                    SQLbase.CloseConnection();
                                }
                                catch (Exception ex)
                                {
                                    var error = ex.InnerException.Message ?? ex.Message;
                                    if (consulta.Trim().Length > 0)
                                    {
                                        CustomMessageBox.Show(CustomMessageBox.ERROR, "Ocurrió un error al intentar obtener la estructura de la consulta:\r\n" + error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        CustomMessageBox.Show(CustomMessageBox.ERROR, "Ocurrió un error al intentar obtener la estructura de la tabla:\r\n" + tablaSeleccionada.ToUpper() + "\r\n" + error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                        + $"WHERE TC.TABLE_NAME = '{tablaSeleccionada}' AND TC.CONSTRAINT_TYPE = 'PRIMARY KEY'";

                                    try
                                    {
                                        SQLbase.OpenConnection();
                                        IDataReader reader = SQLbase.DataReader(query);
                                        using (reader)
                                        {
                                            while (reader.Read())
                                            {
                                                var nombre = reader["Nombre"].ToString().ToUpper();

                                                foreach (ListViewItem item in LSVcampos.Items)
                                                {
                                                    if (item.SubItems[0].Text.ToUpper() == nombre)
                                                    {
                                                        //item.Checked = true;
                                                        item.ImageKey = KEY;
                                                    }
                                                }
                                            }
                                        }
                                        SQLbase.CloseConnection();
                                    }
                                    catch (Exception ex)
                                    {
                                        var error = ex.InnerException.Message ?? ex.Message;
                                        CustomMessageBox.Show(CustomMessageBox.ERROR, "Ocurrió un error al intentar setear las claves primarias:\r\n" + error, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            camposOk = false;
                            var error = ex.InnerException ?? ex;
                            if (consulta.Trim().Length > 0)
                            {
                                CustomMessageBox.Show("Ocurrió un error al intentar obtener la estructura de la consulta:\r\n" + error.Message, CustomMessageBox.ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                CustomMessageBox.Show("Ocurrió un error al intentar obtener la estructura de la tabla:\r\n" + tablaSeleccionada.ToUpper() + "\r\n" + error.Message, CustomMessageBox.ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    if (camposOk)
                    {
                        ComprobarTiposDeCampos(tablaSeleccionada);
                        LSVcampos.Refresh();
                    }
                }
                catch (Exception ex)
                {
                    camposOk = false;
                    var error = ex.InnerException ?? ex;
                    CustomMessageBox.Show("Ocurrió un error al intentar acceder a la base de datos:\r\n" + error.Message, CustomMessageBox.ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                LSVcampos.Refresh();
                LSVcampos.ResumeLayout();
            }
            CursorDefault();
            return camposOk;
        }

        private void CargarListViewDesdeEsquema(IDataReader reader, bool esDB2)
        {
            DataTable schema = reader.GetSchemaTable();

            foreach (DataRow row in schema.Rows)
            {
                var nombre = row["ColumnName"].ToString();
                var dataType = (Type)row["DataType"];
                var longitud = row["NumericPrecision"] != DBNull.Value ? Convert.ToInt32(row["NumericPrecision"]) : 0;
                var escala = row["NumericScale"] != DBNull.Value ? Convert.ToInt32(row["NumericScale"]) : 0;
                var aceptaNulos = (bool)row["AllowDBNull"];

                var tipo = dataType.ToString().ToUpper();
                if (esDB2)
                {
                    tipo = (capas.TIPOSDB2.ContainsKey(dataType) ? capas.TIPOSDB2[dataType] : dataType.Name).ToUpper();
                }
                else
                {
                    tipo = (capas.TIPOSSQL.ContainsKey(dataType) ? capas.TIPOSSQL[dataType] : dataType.Name).ToUpper();
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
            var nombre = reader["Nombre"].ToString();
            var tipo = reader["Tipo"].ToString().ToUpper();
            var longitud = reader["Longitud"].ToString();
            var escala = reader["Escala"].ToString();
            var aceptaNulos = reader["AceptaNulos"].ToString();

            capas.camposTabla.Add(nombre);
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

            StringConnection stringConnection = DefinirStringConnection();
            if (conexionActual.Motor == TipoMotor.DB2)
            {
                try
                {
                    stringConnection.Consulta = generarDesdeConsulta ? TXTgenerarAPartirDeConsulta.Text : "SELECT * FROM " + tabla + " FETCH FIRST 1 ROW ONLY";
                    DataLayer.DataBase DB2base = new DataLayer.DataBase(new OdbcConnection(stringConnection.Obtener()));
                    DB2base.OpenConnection();
                    DS = DB2base.DataSet(stringConnection.Consulta);
                    DB2base.CloseConnection();
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
                    tabla = CMBtablas.Items[CMBtablas.SelectedIndex].ToString();

                    string query = generarDesdeConsulta ? TXTgenerarAPartirDeConsulta.Text : "SELECT TOP 1 * FROM " + tabla;

                    DataLayer.DataBase SQLbase = new DataLayer.DataBase(new OdbcConnection(stringConnection.Obtener()));
                    SQLbase.OpenConnection();
                    DS = SQLbase.DataSet(query);
                    SQLbase.CloseConnection();
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
                    if (capas.Tipo(columna) == Capas.ERROR)
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

        private bool ComprobarClaves()
        {
            //bool resultado = LSVcampos.CheckedItems.Count > 0;
            bool resultado = LSVcampos.Items.Cast<ListViewItem>().Any(x => x.ImageKey == KEY);

            if (!resultado)
            {
                CustomMessageBox.Show("La tabla seleccionada debe contener una clave. \n\nAgréguela en su motor de base datos o seleccione al menos el checkbox de uno de sus campos en la lista.", CustomMessageBox.ATENCION, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }

            return resultado;
        }

        //private void RDBsql_CheckedChanged(object sender, EventArgs e)
        //{
        //    WaitCursor();
        //    try
        //    {
        //        CHKtryOrIf.Visible = !RDBsql.Checked;
        //        if (RDBsql.Checked)
        //        {
        //            CMBservidor.Items.Clear();
        //            //CMBservidor.Items.AddRange(new object[] { "133.123.108.29", "DESARROLLO", "PRODUCCION", "DESARROLLOWEB" }); LA primera es la de CABL
        //            CMBservidor.Items.AddRange(new object[] { "DESARROLLO", "PRODUCCION", "DESARROLLOWEB" });
        //            if (CMBservidor.Items.Count > 0)
        //            {
        //                CMBservidor.SelectedIndex = 0;
        //            }
        //            CMBservidor.Refresh();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    CursorDefault();
        //}

        //private void RDBdb2_CheckedChanged(object sender, EventArgs e)
        //{
        //    WaitCursor();
        //    try
        //    {
        //        if (RDBdb2.Checked)
        //        {
        //            CHKtryOrIf.Visible = RDBdb2.Checked;
        //            CMBservidor.Items.Clear();
        //            //CMBservidor.Items.AddRange(new object[] { "133.123.120.120", "133.123.108.29", "SERVER04", "SERVER01" }); el segundo es el de CABL
        //            CMBservidor.Items.AddRange(new object[] { "133.123.120.120", "SERVER04", "SERVER01" });

        //            if (CMBservidor.Items.Count > 0)
        //            {
        //                CMBservidor.SelectedIndex = 0;
        //            }
        //            CMBservidor.Refresh();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    CursorDefault();
        //}

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
            try
            {
                string texto = CMBtablas.Text;

                // ✅ 1. Filtrar caracteres no válidos (solo letras, números y _ $ # @)
                if (!System.Text.RegularExpressions.Regex.IsMatch(texto, @"^[a-zA-Z0-9_@$#]*$"))
                {
                    return; // ignorar si el texto tiene caracteres inválidos
                }

                // ✅ 2. Filtrar lista
                List<string> filtrados = capas.tablasBase
                    .Where(item => item.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                CMBtablas.BeginUpdate();
                CMBtablas.Items.Clear();

                if (desplegarCombo && filtrados.Count > 0)
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
            catch (Exception err)
            {
                CustomMessageBox.Show($"ERROR EN LA ACTUALIZACION DEL TEXTO DE TABLA: {err.Message}");
            }
        }

        private void CMBtablas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                try
                {
                    CMBtablas.BeginUpdate();
                    CMBtablas.Items.Clear();
                    CMBtablas.Items.AddRange(capas.tablasBase.ToArray());
                    CMBtablas.Text = string.Empty;
                    CMBtablas.DroppedDown = true;
                    CMBtablas.EndUpdate();
                }
                catch (Exception err)
                {
                    CustomMessageBox.Show($"ERROR EN EL KEYDOWN DE TABLA: {err.Message}");
                }
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

        private void ForzarSeparacionClase()
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

        private void ForzarSeparacionSeparador()
        {
            try
            {
                int min = SPCseparador.Panel1MinSize;
                int max = Math.Max(min, SPCseparador.Height - SPCseparador.Panel2MinSize - SPCseparador.SplitterWidth);

                if (SPCseparador.SplitterDistance < min)
                    SPCseparador.SplitterDistance = min;
                else if (SPCseparador.SplitterDistance > max)
                    SPCseparador.SplitterDistance = max;
            }
            catch (Exception)
            {
            }
        }

        private void AsegurarTamañoMinimo()
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

        private void SPCclase_SplitterMoved(object sender, SplitterEventArgs e)
        {
            ForzarSeparacionClase();
        }

        private void SPCseparador_SplitterMoved(object sender, SplitterEventArgs e)
        {
            ForzarSeparacionSeparador();
        }

        private void BTNbuscarSolucion_Click(object sender, EventArgs e)
        {
            OFDlistarDeSolucion.ShowDialog();
            CargarComboyTreeView();
        }

        private void CargarComboyTreeView()
        {
            if (OFDlistarDeSolucion.FileName.Length > 0)
            {
                try
                {
                    WaitCursor();

                    ListarNameSpaces();
                    desplegarCombo = isActivated && !CHKinsertarEnProyecto.Checked;
                    CargarSolucionPorCarpetas();
                    CMBnamespaces.DroppedDown = desplegarCombo;
                    desplegarCombo = true;
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show("Ocurrió un error al intentar acceder a la lista de carpetas de la solución seleccionada.", CustomMessageBox.ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                CursorDefault();
            }
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
                try
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
                        string decoded = Uri.UnescapeDataString(include); // 🔹 decodificar
                        string carpeta = Path.GetDirectoryName(decoded);
                        if (!string.IsNullOrEmpty(carpeta))
                            resultado.Add(carpeta);
                    }
                }
                catch (Exception err)
                {
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

        private void BTNobtenerEstructura_Click(object sender, EventArgs e)
        {
            CamposTabla("CONSULTA", TXTgenerarAPartirDeConsulta.Text);
        }

        private void BTNagregarCampo_Click(object sender, EventArgs e)
        {
            // Filtrar las tablas por tipos de datos
            // DATE y DATETIME juntos
            // TIME
            // EL RESTO
            CargarCamposAbm(TBPalta, DGValta);
            CargarCamposAbm(TBPbaja, DGVbaja);
            CargarCamposAbm(TBPmodificacion, DGVmodificacion);
            CargarCamposAbm(TBPrecuperacion, DGVrecuperacion, false);
        }

        private void CargarCamposAbm(TabPage tab, DataGridView grilla, bool ABM = true)
        {
            if (TBCcamposABM.SelectedTab == tab)
            {
                foreach (ListViewItem item in LSVcampos.SelectedItems)
                {
                    var fila = Utilidades.BuscarFila(grilla, item.Text);
                    if (fila == null)
                    {
                        int indiceFila = grilla.Rows.Add();
                        grilla.Rows[indiceFila].Cells[0].Value = item.Text;

                        // 🔹 Crear comboCell específico por fila
                        var comboCell = new DataGridViewComboBoxCell();

                        BindingSource bs;
                        switch (item.SubItems[1].Text.Trim().ToUpper())
                        {
                            case "DATE":
                            case "DATETIME":
                                bs = new BindingSource(Capas.CamposAbmFechas, null);
                                break;
                            case "TIME":
                                bs = new BindingSource(Capas.CamposAbmHoras, null);
                                break;
                            default:
                                bs = new BindingSource(Capas.CamposAbm, null);
                                break;
                        }

                        comboCell.DataSource = bs;
                        comboCell.DisplayMember = "Key";
                        comboCell.ValueMember = "Value";

                        // 🔹 Asignar el comboCell a la grilla
                        grilla.Rows[indiceFila].Cells[1] = comboCell;

                        // 🔹 Obtener valor predicho con tu método
                        PredecirValor(item, comboCell, ABM);

                        // 🔹 Validar que el valor exista en el DataSource
                        string valorPredicho = comboCell.Value?.ToString();
                        if (!string.IsNullOrEmpty(valorPredicho))
                        {
                            bool existe = bs.Cast<KeyValuePair<string, string>>()
                                            .Any(x => x.Value == valorPredicho);

                            if (!existe)
                            {
                                // fallback al primer valor
                                var primero = bs.Cast<KeyValuePair<string, string>>().FirstOrDefault();
                                if (!primero.Equals(default(KeyValuePair<string, string>)))
                                    comboCell.Value = primero.Value;
                            }
                        }
                        else
                        {
                            // si no hay valor, forzar el primero
                            var primero = bs.Cast<KeyValuePair<string, string>>().FirstOrDefault();
                            if (!primero.Equals(default(KeyValuePair<string, string>)))
                                comboCell.Value = primero.Value;
                        }
                    }
                }
            }
        }

        private void grilla_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void PredecirValor(ListViewItem item, DataGridViewCell celda, bool ABM)
        {
            var campo = item.Text.ToUpper();
            var tipo = item.SubItems[1].Text.ToUpper().Trim();
            celda.Value = Capas.CamposAbm[Capas.CADENA_VACIA_CLAVE];
            var tipoCampo = Capas.TipoCampoAbm.CADENA;
            if (tipo.StartsWith("DATE"))
            {
                tipoCampo = Capas.TipoCampoAbm.FECHA;
            }
            else if (tipo == "TIME")
            {
                tipoCampo = Capas.TipoCampoAbm.HORA;
            }

            if (tipoCampo == Capas.TipoCampoAbm.FECHA || campo.StartsWith("FECHA") || campo.StartsWith("FECH") || campo.StartsWith("FEC") || campo.StartsWith("FE") || campo.StartsWith("F"))
            {
                celda.Value = Capas.CamposAbmFechas[ABM ? Capas.FECHA_ACTUAL_CLAVE : Capas.FECHA_POR_DEFECTO_CLAVE];
            }
            if (campo.StartsWith("USUARIO") || campo.StartsWith("USU") || campo.StartsWith("USER") || campo.StartsWith("USR") || campo.StartsWith("US") || campo.StartsWith("U"))
            {
                celda.Value = Capas.CamposAbm[ABM ? Capas.USUARIO_MAGIC_CLAVE : Capas.CADENA_VACIA_CLAVE];
            }
            if (tipoCampo == Capas.TipoCampoAbm.HORA || campo.StartsWith("HORA") || campo.StartsWith("HOR") || campo.StartsWith("HO") || campo.StartsWith("H"))
            {
                celda.Value = Capas.CamposAbmHoras[ABM ? Capas.HORA_ACTUAL_CLAVE : Capas.HORA_POR_DEFECTO_CLAVE];
            }
            if (campo.StartsWith("CODIGO") || campo.StartsWith("CODIG") || campo.StartsWith("CODI") || campo.StartsWith("COD") || campo.StartsWith("CO") || campo.StartsWith("C"))
            {
                celda.Value = Capas.CamposAbm[ABM ? Capas.CODIGO_BAJA_CLAVE : Capas.CODIGO_0_CLAVE];
            }
            if (campo.StartsWith("MOTIVO") || campo.StartsWith("MOTIV") || campo.StartsWith("MOTI") || campo.StartsWith("MOT") || campo.StartsWith("MO") || campo.StartsWith("M"))
            {
                celda.Value = Capas.CamposAbm[ABM ? Capas.MOTIVO_BAJA_CLAVE : Capas.CADENA_VACIA_CLAVE];
            }
        }

        private void BTNquitarCampo_Click(object sender, EventArgs e)
        {
            QuitarCampo(TBPalta, DGValta);
            QuitarCampo(TBPbaja, DGVbaja);
            QuitarCampo(TBPmodificacion, DGVmodificacion);
            QuitarCampo(TBPrecuperacion, DGVrecuperacion);
        }

        private void QuitarCampo(TabPage tab, DataGridView grilla)
        {
            if (TBCcamposABM.SelectedTab == tab)
            {
                foreach (DataGridViewRow fila in grilla.SelectedRows)
                {
                    grilla.Rows.Remove(fila);
                }
                grilla.Refresh();
            }
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

        private void TSMdesdeConsulta_Click(object sender, EventArgs e)
        {
            GenerarDesdeConsulta();
        }

        private void TSMdesdeTabla_Click(object sender, EventArgs e)
        {
            GenerarDesdeTabla();
        }

        private void WaitCursor()
        {
            Cursor.Current = Cursors.WaitCursor;
            this.UseWaitCursor = true;
            Application.DoEvents();
        }

        private void CursorDefault()
        {
            Cursor.Current = Cursors.Default;
            this.UseWaitCursor = false;
            Application.DoEvents();
        }

        private void CHKinsertarEnProyecto_CheckedChanged(object sender, EventArgs e)
        {
            CargarSolucionPorCarpetas();
        }

        private void CargarSolucionPorCarpetas()
        {
            if (CHKinsertarEnProyecto.Checked)
            {
                if (OFDlistarDeSolucion.FileName.Length > 0)
                {
                    TRVsolucion.Show();
                    LBLseleccionesTRV.Show();
                    WaitCursor();
                    TRVsolucion.Nodes.Clear();
                    TRVsolucion.Refresh();
                    string pathSolucion = OFDlistarDeSolucion.FileName;

                    CargarEstructuraDeSolucion(pathSolucion, TRVsolucion);

                    CursorDefault();
                }
                else
                {
                    CustomMessageBox.Show("Seleccione una solución.", CustomMessageBox.ATENCION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                TRVsolucion.Hide();
                LBLseleccionesTRV.Hide();
            }
        }

        public void CargarEstructuraDeSolucion(string pathSolucion, TreeView trvSolucion)
        {
            if (!File.Exists(pathSolucion))
                throw new FileNotFoundException("No se encontró el archivo .sln", pathSolucion);

            trvSolucion.Nodes.Clear();
            TreeNode nodoRaiz = new TreeNode(Path.GetFileNameWithoutExtension(pathSolucion));
            nodoRaiz.ImageKey = nodoRaiz.SelectedImageKey = SOLUTION;
            nodoRaiz.Tag = Path.GetDirectoryName(pathSolucion);
            trvSolucion.Nodes.Add(nodoRaiz);

            // Leer proyectos del .sln
            var lineasDeProyecto = File.ReadAllLines(pathSolucion)
                .Where(l => l.StartsWith("Project("))
                .ToList();

            foreach (var linea in lineasDeProyecto)
            {
                var parts = linea.Split(',');
                if (parts.Length < 2) continue;

                string pathProyecto = parts[1].Trim().Trim('"'); // ruta relativa al .sln
                string pathCompletoProyecto = Path.Combine(Path.GetDirectoryName(pathSolucion), pathProyecto);

                if (File.Exists(pathCompletoProyecto))
                {
                    CargarEstructuraDeProyecto(pathCompletoProyecto, nodoRaiz);
                }
            }
            ObtenerPathsDeProyectoYGlobalAsax(pathSolucion, out string pathCsproj, out string pathGlobalAsaxCs);
            pathProyecto = pathCsproj;
            pathGlobalAsax = pathGlobalAsaxCs;
            nodoRaiz.Expand();
        }

        private void CargarEstructuraDeProyecto(string pathCsproj, TreeNode nodoPadre)
        {
            var doc = XDocument.Load(pathCsproj);
            string nombreDelProyecto = Path.GetFileNameWithoutExtension(pathCsproj);
            TreeNode nodoDelProyecto = nodoPadre.Nodes.Add(nombreDelProyecto);
            nodoDelProyecto.ImageKey = nodoDelProyecto.SelectedImageKey = PROYECT;

            string directorioDelProyecto = Path.GetDirectoryName(pathCsproj);
            nodoDelProyecto.Tag = directorioDelProyecto;

            // Extraer nodos que agregan archivos al proyecto
            var itemGroups = doc.Descendants()
                .Where(x => x.Name.LocalName == "Compile" ||
                            x.Name.LocalName == "Content" ||
                            x.Name.LocalName == "None")
                .Select(x => (string)x.Attribute("Include"))
                .Where(x => !string.IsNullOrEmpty(x)).OrderBy(x => x);

            foreach (var includePath in itemGroups)
            {
                string decodificado = Uri.UnescapeDataString(includePath); // 🔹 decodificar
                string pathCompleto = Path.Combine(directorioDelProyecto, decodificado);
                AgregarPathAlTreeView(nodoDelProyecto, decodificado, pathCompleto);
            }
            nodoDelProyecto.Expand();
        }

        private static void AgregarPathAlTreeView(TreeNode nodoDelProyecto, string pathRelativo, string pathCompleto)
        {
            var partes = pathRelativo.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            TreeNode nodoActual = nodoDelProyecto;

            foreach (var parte in partes)
            {
                var existe = nodoActual.Nodes.Cast<TreeNode>()
                    .FirstOrDefault(n => n.Text == parte);

                if (existe == null)
                {
                    existe = nodoActual.Nodes.Add(parte);
                    existe.ImageKey = existe.SelectedImageKey = pathCompleto.EndsWith(parte) ? FILE : FOLDER_CLOSE;
                    existe.Tag = pathCompleto.Substring(0, pathCompleto.IndexOf(parte) + parte.Length);
                }

                nodoActual = existe;
            }
        }

        private void CapibararProyecto()
        {
            if (CHKinsertarEnProyecto.Checked)
            {
                desplegarCombo = false;
                if (string.IsNullOrEmpty(CarpetaDestino))
                    return;

                string carpetaCapas = $@"{Path.Combine(CarpetaDestino, Utilidades.FormatearCadena(TXTnombreAmigable.Text))} ({capas.TABLA})";

                // Crear carpeta en el disco
                CopiarDirectorio(capas.pathCarpetaClase, carpetaCapas);

                // Agregar al proyecto
                AgregarArchivosACsproj(carpetaCapas);

                // Modificar Global.asax
                ActualizarGlobalAsax();

                // Refrescar Combo y TreeView
                CargarComboyTreeView();
            }
        }

        public void CopiarDirectorio(string directorioFuente, string directorioDestino, bool sobreescribir = true)
        {
            // Crear el destino si no existe
            if (!Directory.Exists(directorioDestino))
            {
                Directory.CreateDirectory(directorioDestino);
                //TfsHelper.AgregarAFuente(directorioDestino);
            }

            // Copiar todos los archivos
            foreach (string archivo in Directory.GetFiles(directorioFuente))
            {
                string nombreArchivo = Path.GetFileName(archivo);
                string archivoDestino = Path.Combine(directorioDestino, nombreArchivo);
                File.Copy(archivo, archivoDestino, sobreescribir);
                //TfsHelper.AgregarAFuente(archivoDestino);
            }

            // Copiar recursivamente los subdirectorios
            foreach (string subdirectorio in Directory.GetDirectories(directorioFuente))
            {
                string nombreDirectorio = Path.GetFileName(subdirectorio);
                string subdirectorioDestino = Path.Combine(directorioDestino, nombreDirectorio);
                CopiarDirectorio(subdirectorio, subdirectorioDestino, sobreescribir);
            }
        }

        public void ObtenerPathsDeProyectoYGlobalAsax(string pathSolucion, out string pathDeProyecto, out string pathGlobalAsaxCs)
        {
            pathDeProyecto = null;
            pathGlobalAsaxCs = null;

            if (!File.Exists(pathSolucion))
                throw new FileNotFoundException("No se encontró el archivo .sln", pathSolucion);

            string directorioSolucion = Path.GetDirectoryName(pathSolucion);

            // Buscar línea con ruta al .csproj dentro del .sln
            string csprojRelativo = File.ReadAllLines(pathSolucion)
                .Where(l => l.StartsWith("Project(") && l.Contains(".csproj"))
                .Select(l => l.Split(',')[1].Trim().Trim('"'))
                .FirstOrDefault();

            if (csprojRelativo == null)
                throw new Exception("No se encontró ningún proyecto .csproj en la solución.");

            pathDeProyecto = Path.Combine(directorioSolucion, csprojRelativo);

            if (!File.Exists(pathDeProyecto))
                throw new FileNotFoundException("No se encontró el archivo .csproj referenciado.", pathDeProyecto);

            // Leer el .csproj y buscar Global.asax.cs
            var doc = XDocument.Load(pathDeProyecto);

            var globalAsaxInclude = doc.Descendants()
                .Where(x => x.Name.LocalName == "Compile" || x.Name.LocalName == "Content" || x.Name.LocalName == "None")
                .Select(x => (string)x.Attribute("Include"))
                .FirstOrDefault(p => p != null && p.EndsWith("Global.asax.cs", StringComparison.OrdinalIgnoreCase));

            if (globalAsaxInclude != null)
            {
                pathGlobalAsaxCs = Path.Combine(Path.GetDirectoryName(pathDeProyecto), globalAsaxInclude);
            }
        }

        private void AgregarArchivosACsproj(string carpeta)
        {
            if (pathProyecto == null) return;

            XDocument doc = XDocument.Load(pathProyecto);
            XNamespace ns = doc.Root.Name.Namespace;

            // 🔹 Recorre todos los .cs en la carpeta y subcarpetas
            foreach (var archivo in Directory.GetFiles(carpeta, "*.cs", SearchOption.AllDirectories))
            {
                string relativePath = archivo.Replace(Path.GetDirectoryName(pathProyecto) + "\\", "");

                // Normalizo para que siempre coincida
                string normalizedPath = relativePath.Replace("(", "%28").Replace(")", "%29");

                if (!doc.Descendants(ns + "Compile").Any(e => (string)e.Attribute("Include") == normalizedPath))
                {
                    // Busco un ItemGroup existente o creo uno nuevo
                    var itemGroup = doc.Root.Elements(ns + "ItemGroup").FirstOrDefault();
                    if (itemGroup == null)
                    {
                        itemGroup = new XElement(ns + "ItemGroup");
                        doc.Root.Add(itemGroup);
                    }
                    itemGroup.Add(new XElement(ns + "Compile", new XAttribute("Include", normalizedPath)));
                }
            }

            doc.Save(pathProyecto);
        }

        private void ActualizarGlobalAsax()
        {
            if (!File.Exists(pathGlobalAsax))
                return;

            var lineas = File.ReadAllLines(pathGlobalAsax).ToList();
            // Agregar using
            int idx = lineas.FindIndex(l => l.Contains("namespace"));
            if (idx > 1)
            {
                idx--;
            }
            string espacioDeNombres = $"{TXTespacioDeNombres.Text.Trim()}.{Utilidades.FormatearCadena(TXTnombreAmigable.Text)}";
            string usingRepositories = $"using {espacioDeNombres}.Repositories;";
            string usingService = $"using {espacioDeNombres}.Service;";

            if (!lineas.Any(l => l.Contains(usingRepositories)))
            {
                lineas.Insert(idx, usingRepositories);
                idx++;
            }
            if (!lineas.Any(l => l.Contains(usingService)))
            {
                lineas.Insert(idx, usingService);
                idx++;
            }

            // Agregar registros de Autofac
            idx = lineas.FindIndex(l => l.Contains("var container = builder.Build();"));
            if (idx >= 0)
            {
                if (idx > 2)
                {
                    idx -= 2;
                }
                lineas.Insert(idx, string.Empty);
                if (!lineas.Any(l => l.Trim().Contains($"builder.RegisterType<{capas.TABLA}Repositories>()")))
                {
                    idx++;
                    lineas.Insert(idx, $"            builder.RegisterType<{capas.TABLA}Repositories>()");
                    idx++;
                    lineas.Insert(idx, $"                    .As<{capas.TABLA}RepositoriesInterface>()");
                    idx++;
                    lineas.Insert(idx, $"                    .InstancePerRequest();");
                }
                if (!lineas.Any(l => l.Trim().Contains($"builder.RegisterType<{capas.TABLA}Service>()")))
                {
                    idx++;
                    lineas.Insert(idx, $"            builder.RegisterType<{capas.TABLA}Service>()");
                    idx++;
                    lineas.Insert(idx, $"                    .As<{capas.TABLA}ServiceInterface>()");
                    idx++;
                    lineas.Insert(idx, $"                    .InstancePerRequest();");
                }
            }
            File.WriteAllLines(pathGlobalAsax, lineas);
        }

        // Manejar el cambio de iconos
        private void TRVsolucion_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.ImageKey == FOLDER_CLOSE)
            {
                e.Node.ImageKey = e.Node.SelectedImageKey = FOLDER_OPEN;
            }
        }

        private void TRVsolucion_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.ImageKey == FOLDER_OPEN)
            {
                e.Node.ImageKey = e.Node.SelectedImageKey = FOLDER_CLOSE;
            }
        }

        private void TRVsolucion_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                if (Directory.Exists(e.Node.Tag.ToString()))
                {
                    CarpetaDestino = e.Node.Tag.ToString();
                }

                ActualizarLabelSeleccionTRV(Path.GetFileName(CarpetaDestino), OrigenDeDatoSql);
            }
        }

        private void TRVsolucion_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TRVsolucion.SelectedNode = e.Node; // asegura que quede seleccionado
            }
        }

        private void TSMnuevaCarpeta_Click(object sender, EventArgs e)
        {
            if (TRVsolucion.SelectedNode == null) return;
            TRVsolucion.LabelEdit = true;
            TreeNode nuevoNodo = new TreeNode(NUEVA_CARPETA);
            nuevoNodo.ImageKey = nuevoNodo.SelectedImageKey = FOLDER_CLOSE;
            nuevoNodo.Tag = TRVsolucion.SelectedNode.Tag; // path padre
            TRVsolucion.SelectedNode.Nodes.Add(nuevoNodo);
            TRVsolucion.SelectedNode.Expand();

            // Permitir editar el texto del nodo
            TRVsolucion.SelectedNode.LastNode.BeginEdit();
        }

        private void TRVsolucion_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Label))
            {
                e.CancelEdit = true;
                return;
            }

            string parentPath = e.Node.Parent.Tag.ToString();
            string nuevaRuta = Path.Combine(parentPath, e.Label);

            if (!Directory.Exists(nuevaRuta))
                Directory.CreateDirectory(nuevaRuta);

            e.Node.Tag = nuevaRuta;
            TRVsolucion.LabelEdit = false;
        }

        private void TSMorigenDeDatosSQL_Click(object sender, EventArgs e)
        {
            try
            {
                if (TRVsolucion.SelectedNode == null) return;
                if (Directory.Exists(TRVsolucion.SelectedNode.Tag.ToString()))
                {
                    CarpetaDestino = TRVsolucion.SelectedNode.Tag.ToString();
                }
                OrigenDeDatoSql = Utilidades.ObtenerClaseYEntidad(TRVsolucion.SelectedNode.Tag.ToString());

                ActualizarLabelSeleccionTRV(Path.GetFileName(CarpetaDestino), OrigenDeDatoSql);
            }
            catch { }
        }

        static public Conexion conexionActual = null;

        private void BTNnueva_Click(object sender, EventArgs e)
        {
            FRMconexiones datosConexion = new FRMconexiones();
            datosConexion.ShowDialog();
            ConexionesManager.Guardar(conexiones);
            InicializarConexiones();
        }

        private void BTNeliminar_Click(object sender, EventArgs e)
        {
            try
            {
                string nombreConeccion = string.Copy(conexionActual.Nombre);
                conexiones.Remove(nombreConeccion);
                conexionActual = null;
                ConexionesManager.Guardar(conexiones);
                InicializarConexiones();
                CustomMessageBox.Show($"Se eliminó correctamente la conección: {nombreConeccion}");
            }
            catch (Exception err)
            {
                CustomMessageBox.Show($"Ocurrió un error al eliminar la conección: {err.Message}");
            }
        }

        private void BTNeditar_Click(object sender, EventArgs e)
        {
            string nombreConexionActual = string.Copy(conexionActual.Nombre);
            FRMconexiones datosConexion = new FRMconexiones(conexionActual);
            datosConexion.ShowDialog();
            if (!conexiones.ContainsKey(conexionActual.Nombre) && nombreConexionActual != conexionActual.Nombre)
            {
                conexiones.Add(conexionActual.Nombre, conexionActual);
            }
            else
            {
                conexiones[nombreConexionActual] = conexionActual;
            }
            ConexionesManager.Guardar(conexiones);
            InicializarConexiones();
        }

        private void InicializarConexiones()
        {
            conexiones = ConexionesManager.Cargar();

            CMBconexion.Items.Clear();
            CMBconexion.Items.AddRange(conexiones.Keys.ToArray());

            string nombreConexion = conexiones.First().Key;
            // Seleccionar conexión guardada
            if (conexionActual != null)
            {
                nombreConexion = conexionActual.Nombre;
            }
            var item = conexiones[nombreConexion];
            if (item != null)
            {
                // Fuerzo la seleccion de la conexion definida
                CMBconexion.SelectedIndex = 0;
                CMBconexion.SelectedIndex = -1;
                CMBconexion.SelectedItem = item;
                CMBconexion.Text = nombreConexion;
                CMBconexion.Refresh();
            }
        }

        private void CMBconexion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CMBconexion.SelectedItem != null)
            {
                ActualizarConeccionActual(conexiones[CMBconexion.SelectedItem.ToString()]);

                try
                {
                    desplegarCombo = true;
                    TablasBase();
                }
                catch (Exception err)
                {
                    CustomMessageBox.Show($"ERROR EN EL CAMBIO DE BASE: {err.Message}");
                }
            }
        }

        private void ActualizarConeccionActual(Conexion conexion)
        {
            try
            {
                conexionActual = configuracion.Conexion = conexion;
                LBLbaseDeDatos.Text = conexionActual != null ? conexionActual.BaseDatos : string.Empty;
            }
            catch { }
        }
    }
}

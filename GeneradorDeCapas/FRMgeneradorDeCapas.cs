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

namespace GeneradorDeCapas
{
    public partial class FRMgeneradorDeCapas : Form
    {
        Dictionary<Type, string> TIPOS = new Dictionary<Type, string>()
        {
            {typeof(bool), "bool"},
            {typeof(byte), "byte"},
            {typeof(char), "char"},
            {typeof(DateTime), "DateTime"},
            {typeof(decimal),"double"},
            {typeof(double),"double"},
            {typeof(Guid),"Guid"},
            {typeof(short),"int"},         // Int16
			{typeof(int),"int"},             // Int32
			{typeof(long),"int"},           // Int64
			{typeof(sbyte),"double"},
            {typeof(float),"double"},         // Single
			{typeof(string),"string"},
            {typeof(TimeSpan),"TimeSpan"},
            {typeof(ushort),"uint"},       // UInt16
			{typeof(uint),"uint"},           // UInt32
			{typeof(ulong),"uint" }         // UInt64
		};

        Dictionary<string, string> Mapeo = new Dictionary<string, string>
        {
            { "long",       "OdbcType.BigInt" },           // BIGINT
			{ "int",        "OdbcType.Int" },              // INTEGER
			{ "short",      "OdbcType.SmallInt" },         // SMALLINT
			{ "byte",       "OdbcType.TinyInt" },          // SMALLINT usado como byte en DB2
			{ "decimal",    "OdbcType.Numeric" },          // DECIMAL(p,s), NUMERIC
			{ "float",      "OdbcType.Real" },             // REAL
			{ "double",     "OdbcType.Double" },           // DOUBLE
			{ "bool",       "OdbcType.SmallInt" },         // DB2 no tiene BOOLEAN real en versiones antiguas
			{ "string",     "OdbcType.VarChar" },          // VARCHAR, usar NVarChar si es Unicode
			{ "char",       "OdbcType.Char" },             // CHAR(1)
			{ "DateTime",   "OdbcType.DateTime" },         // TIMESTAMP (Date si sólo fecha)
			{ "TimeSpan",   "OdbcType.Time" },             // TIME
			{ "Guid",       "OdbcType.Char" },             // DB2 no tiene UNIQUEIDENTIFIER → usar CHAR(36)
			{ "byte[]",     "OdbcType.VarBinary" }         // BLOB, VARBINARY, BYTEA
		};

        Dictionary<Type, string> Campo = new Dictionary<Type, string>
        {
            {typeof(bool), "CampoBool"},
            {typeof(byte), "Campo"},
            {typeof(char), "CampoStr"},
            {typeof(DateTime), "CampoDateTime"},
            {typeof(decimal),"CampoDouble"},
            {typeof(double),"CampoDouble"},
            {typeof(Guid),"Campo"},
            {typeof(short),"CampoInt"},         // Int16
			{typeof(int),"CampoInt"},             // Int32
			{typeof(long),"CampoInt"},           // Int64
			{typeof(sbyte),"CampoDouble"},
            {typeof(float),"CampoDouble"},         // Single
			{typeof(string),"CampoStr"},
            {typeof(TimeSpan),"Campo"},
            {typeof(ushort),"Campo"},       // UInt16
			{typeof(uint),"Campo"},           // UInt32
			{typeof(ulong),"Campo" }         // UInt64
		};

        private const string ERROR = "ERROR";

        List<string> tablasBase = new List<string>();

        public FRMgeneradorDeCapas()
		{
			InitializeComponent();
        }

        private const int PANEL1_MIN = 510; // ancho/alto mínimo que querés para Panel1

        private void FRMgeneradorDeCapas_Load(object sender, EventArgs e)
        {
            SPCclase.Panel1MinSize = PANEL1_MIN;

            CMBservidor.SelectedIndex = 0;
            CMBservidor.Text = CMBservidor.Items[CMBservidor.SelectedIndex].ToString();

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

        private string Clase(string tabla)
        {
            string resultado = string.Empty;
            List<DataColumn> claves = new List<DataColumn>();
            List<DataColumn> camposConsulta = new List<DataColumn>();
            List<string> columnasError = new List<string>();
            if (RDBdb2.Checked)
            {
                try
                {
                    Ejecutar datos = EstablecerConexion();
                    datos.Consulta = "SELECT * FROM " + tabla + " FETCH FIRST 1 ROW ONLY";
                    ComandoDB2 DB2 = new ComandoDB2(datos.Consulta, datos.ObtenerConexion());
                    DB2.Conexion = new System.Data.Odbc.OdbcConnection(datos.ObtenerConexion());

                    DataSet DS = DB2.ObtenerDataSet();

                    int i = 0;
                    foreach (DataColumn columna in DS.Tables[0].Columns)
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
                catch (Exception ex)
                {
                    resultado = ex.Message;
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

                    DataSet DS = new DataSet();
                    using (SqlDataAdapter DA = new SqlDataAdapter(query, connectionString))
                    {
                        DA.Fill(DS);
                    }

                    int i = 0;
                    foreach (DataColumn columna in DS.Tables[0].Columns)
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
                catch (Exception ex)
                {
                    resultado = ex.Message;
                }
            }

            if (columnasError.Count > 0)
            {
                string columnas = string.Join("\r\n", columnasError);
                MessageBox.Show("NO SE PUEDE PROCESAR LA SIGUIENTE TABLA DEBIDO A INCONSISTENCIAS CON LOS SIGUIENTES CAMPOS:\r\n\r\n" + columnas, "ATENCIÓN!!!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                if (CHKquitarEsquema.Checked)
                {
                    string[] partes = tabla.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    tabla = partes[partes.Length - 1];
                }
                if (CHKcontrollers.Checked)
                {
                    resultado = Controllers(tabla, claves);
                    resultado += "\r\n";
                }
                if (CHKdto.Checked)
                {
                    resultado += Dto(tabla, camposConsulta);
                    resultado += "\r\n";
                }
                if (CHKmodel.Checked)
                {
                    resultado += Model(tabla, camposConsulta, claves);
                    resultado += "\r\n";
                }
                if (CHKrepositories.Checked)
                {
                    resultado += Repositories(tabla, camposConsulta, claves, RDBdb2.Checked);
                    resultado += "\r\n";
                    resultado += RepositoriesInterface(tabla, claves);
                    resultado += "\r\n";
                }
                if (CHKservice.Checked)
                {
                    resultado += Service(tabla, camposConsulta, claves, RDBdb2.Checked);
                    resultado += "\r\n";
                    resultado += ServiceInterface(tabla, claves, RDBdb2.Checked);
                }

                if (System.IO.Directory.Exists(TXTpathCapas.Text))
                {
                    Process.Start("explorer.exe", TXTpathCapas.Text);
                } 
            }

            return resultado;
        }

        private string Controllers(string tabla, List<DataColumn> claves)
		{
			string nombreDeClase = tabla;
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
            Controller.AppendLine("using " + espacioDeNombres + "." + nombreDeClase + ".Dto;");
            Controller.AppendLine("using " + espacioDeNombres + "." + nombreDeClase + ".Service;");
            Controller.AppendLine();
            Controller.AppendLine("namespace " + espacioDeNombres + ".Controllers");
            Controller.AppendLine("{");
            Controller.AppendLine("\t[RoutePrefix(\"" + nombreDeClase.ToLower() + "\")]");
            Controller.AppendLine("\t[EnableCors(origins: \" * \", headers: \" * \", methods: \" * \")]");
            Controller.AppendLine();
            Controller.AppendLine("\tpublic class " + nombreDeClase + " : ApiController");
            Controller.AppendLine("\t{");
            Controller.AppendLine("\t\tprivate readonly " + nombreDeClase + "ServiceInterface _" + nombreClasePrimeraMinuscula + "Service;");
            Controller.AppendLine("\t\tpublic " + nombreDeClase + "Controller(" + nombreDeClase + "ServiceInterface " + nombreClasePrimeraMinuscula + "Service)");
            Controller.AppendLine("\t\t{");
            Controller.AppendLine("\t\t\t_" + nombreClasePrimeraMinuscula + "Service = " + nombreClasePrimeraMinuscula + "Service ?? throw new ArgumentNullException(nameof(" + nombreClasePrimeraMinuscula + "Service));");
            Controller.AppendLine("\t\t}");
            Controller.AppendLine();
            Controller.AppendLine("\t\t[HttpPost, Route(\"nuevo\"), ControlarPermisos]");
            Controller.AppendLine("\t\tpublic async Task<Respuesta> alta" + nombreDeClase + "([FromBody] " + nombreDeClase + "Dto nuevoDto)");
            Controller.AppendLine("\t\t{");
            Controller.AppendLine("\t\t\tRespuesta rta = new Respuesta();");
            Controller.AppendLine();
            Controller.AppendLine("\t\t\trta.Resultado = _" + nombreClasePrimeraMinuscula + "Service.alta" + nombreDeClase + "(nuevoDto);");
            Controller.AppendLine();
            Controller.AppendLine("\t\t\treturn rta;");
            Controller.AppendLine("\t\t}");
            Controller.AppendLine();
            Controller.AppendLine("\t\t[HttpGet, Route(\"baja\"), ControlarPermisos]");
            Controller.AppendLine("\t\tpublic async Task<Respuesta> baja" + nombreDeClase + "(" + camposFromUri + ", [FromUri] int codigoBaja, [FromUri] string motivoBaja)");
            Controller.AppendLine("\t\t{");
            Controller.AppendLine("\t\t\tRespuesta rta = new Respuesta();");
            Controller.AppendLine();
            Controller.AppendLine("\t\t\trta.Resultado = _" + nombreClasePrimeraMinuscula + "Service.baja" + nombreDeClase + "(" + camposClave + ", codigoBaja, motivoBaja);");
            Controller.AppendLine();
            Controller.AppendLine("\t\t\treturn rta;");
            Controller.AppendLine("\t\t}");
            Controller.AppendLine();
            Controller.AppendLine("\t\t[HttpGet, Route(\"recuperar\"), ControlarPermisos]");
            Controller.AppendLine("\t\tpublic async Task<Respuesta> recuperar" + nombreDeClase + "(" + camposFromUri + ")");
            Controller.AppendLine("\t\t{");
            Controller.AppendLine("\t\t\tRespuesta rta = new Respuesta();");
            Controller.AppendLine();
            Controller.AppendLine("\t\t\trta.Resultado = _" + nombreClasePrimeraMinuscula + "Service.recuperar" + nombreDeClase + "(" + camposClave + ");");
            Controller.AppendLine();
            Controller.AppendLine("\t\t\treturn rta;");
            Controller.AppendLine("\t\t}");
            Controller.AppendLine();
            Controller.AppendLine("\t\t[HttpPut, Route(\"modificacion\"), ControlarPermisos]");
            Controller.AppendLine("\t\tpublic async Task<Respuesta> modificacion" + nombreDeClase + "([FromBody] " + nombreDeClase + "Dto nuevoDto)");
            Controller.AppendLine("\t\t{");
            Controller.AppendLine("\t\t\tRespuesta rta = new Respuesta();");
            Controller.AppendLine();
            Controller.AppendLine("\t\t\trta.Resultado = _" + nombreClasePrimeraMinuscula + "Service.modificacion" + nombreDeClase + "(nuevoDto);");
            Controller.AppendLine();
            Controller.AppendLine("\t\t\treturn rta;");
            Controller.AppendLine("\t\t}");
            Controller.AppendLine();
            Controller.AppendLine("\t\t[HttpGet, Route(\"buscarid\"), ControlarPermisos]");
            Controller.AppendLine("\t\tpublic async Task<Respuesta> obtenerPorId(" + camposFromUri + ")");
            Controller.AppendLine("\t\t{");
            Controller.AppendLine("\t\t\tRespuesta rta = new Respuesta();");
            Controller.AppendLine("\t\t\t" + nombreDeClase + "Dto solicitado = _" + nombreClasePrimeraMinuscula + "Service.obtenerPorId(" + camposClave + ");");
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
            Controller.AppendLine("\t\t[HttpGet, Route(\"todos\")]");
            Controller.AppendLine("\t\tpublic async Task<Respuesta> obtenerTodos()");
            Controller.AppendLine("\t\t{");
            Controller.AppendLine("\t\t\tRespuesta rta = new Respuesta();");
            Controller.AppendLine("\t\t\tList <" + nombreDeClase + "Dto> " + nombreDeClase.ToLower() + " = _" + nombreClasePrimeraMinuscula +"Service.obtenerTodos();");
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
            Controller.AppendLine("\t}");
            Controller.AppendLine("}");

            try
            {
                string pathControllers = TXTpathCapas.Text + tabla +  @"\Controllers\";
                string pathClaseController = pathControllers + tabla + "Controller.cs";
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

            return Controller.ToString();
		}

		private string Dto(string tabla, List<DataColumn> columnas)
		{
			string nombreDeClase = tabla;
			string nombreClasePrimeraMinuscula = nombreDeClase[0].ToString().ToLower() + nombreDeClase.Substring(1);
			string espacioDeNombres = TXTespacioDeNombres.Text;

			StringBuilder Dto = new StringBuilder();
			StringBuilder newDto = new StringBuilder();

			Dto.AppendLine("using System;");
			Dto.AppendLine("using System.Collections.Generic;");
			Dto.AppendLine("using System.Linq;");
			Dto.AppendLine("using System.Web;");
			Dto.AppendLine("");
			Dto.AppendLine("namespace " + espacioDeNombres + ".Dto");
			Dto.AppendLine("{");
			Dto.AppendLine("\tpublic class " + nombreDeClase + "Dto");
			Dto.AppendLine("\t{");

			newDto.AppendLine("\t\tpublic " + nombreDeClase + "Dto new" + nombreDeClase + "Dto(" + nombreDeClase + " modelo)");
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

            try
            {
                string pathDto = TXTpathCapas.Text + tabla + @"\Dto\";
                string pathClaseDto = pathDto + tabla + "Dto.cs";
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

            return Dto.ToString();
		}

		private string Model(string tabla, List<DataColumn> columnas, List<DataColumn> claves)
		{
			string nombreDeClase = tabla;
			string nombreClasePrimeraMinuscula = nombreDeClase[0].ToString().ToLower() + nombreDeClase.Substring(1);
			string espacioDeNombres = TXTespacioDeNombres.Text;

			StringBuilder Modelo = new StringBuilder();

			Modelo.AppendLine("using System;");
			Modelo.AppendLine("using System.Collections.Generic;");
			Modelo.AppendLine("using System.Linq;");
			Modelo.AppendLine("using System.Web;");
			Modelo.AppendLine("");
			Modelo.AppendLine("namespace " + espacioDeNombres + ".Model");
			Modelo.AppendLine("{");
			Modelo.AppendLine("\tpublic class " + nombreDeClase + "Model");
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

            try
            {
                string pathModel = TXTpathCapas.Text + tabla + @"\Model\";
                string pathClaseModel = pathModel + tabla + "Model.cs";
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

            return Modelo.ToString();
		}

		private string Repositories(string tabla, List<DataColumn> columnas, List<DataColumn> claves, bool DB2)
		{
			string nombreDeClase = tabla;
			string nombreClasePrimeraMinuscula = nombreDeClase[0].ToString().ToLower() + nombreDeClase.Substring(1);
			string espacioDeNombres = TXTespacioDeNombres.Text;
			List<string> camposConsulta = (from c in columnas select c.ColumnName).ToList();
			string columnasClave = string.Join(", ", (from c in claves select Tipo(c) + " " + c.ColumnName).ToList());
			List<string[]> clavesConsulta = (from c in claves select new string[] { c.ColumnName, Tipo(c)}).ToList();
            string[] partes = espacioDeNombres.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            string espacio = partes[partes.Length - 1];

            StringBuilder Repositories = new StringBuilder();

            Repositories.AppendLine("using System;");
			Repositories.AppendLine("using System.Collections.Generic;");
            Repositories.AppendLine("using System.Data.Entity;");
			Repositories.AppendLine("using System.Data.Odbc;");
            Repositories.AppendLine("using System.Linq;");
			Repositories.AppendLine("using SistemaMunicipalGeneral.Controles;");
            Repositories.AppendLine("using " + espacioDeNombres + ".Model;");
			Repositories.AppendLine();
            Repositories.AppendLine("namespace " + espacioDeNombres + ".Repositories");
			Repositories.AppendLine("{");
			    Repositories.AppendLine("\tpublic class " + nombreDeClase + "Repositories : " + nombreDeClase + "RepositoriesInterface");
			    Repositories.AppendLine("\t{");
                //ALTA
                if (DB2)
                {
                    Repositories.AppendLine("\t\tpublic (string, bool) alta" + nombreDeClase + "(" + nombreDeClase + " " + nombreClasePrimeraMinuscula +"Model)");
                    Repositories.AppendLine("\t\t{");
                        Repositories.AppendLine("\t\t\ttry");
                        Repositories.AppendLine("\t\t\t{");
                            Repositories.AppendLine("\t\t\t\tComandoDB2 SQLconsulta = new ComandoDB2(string.Empty, \"DB2_Tributos\");");
                            Repositories.AppendLine("\t\t\t\tSQLconsulta.Consulta = \"INSERT INTO " + nombreDeClase + " (" + string.Join(", ", (from c in columnas select c.ColumnName).ToList()) + ") VALUES (" + string.Join(",", Enumerable.Repeat("?", columnas.Count)) + ")\";");
                            Repositories.AppendLine();
                            foreach (DataColumn c in columnas)
                            {
                                Repositories.AppendLine("\t\t\t\tSQLconsulta.Agregar(\"@" + c.ColumnName + "\", " + Mapeo[Tipo(c)] + ", " + nombreClasePrimeraMinuscula + "Model." + c.ColumnName + ");");
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
                            Repositories.AppendLine("\t\t\t\treturn (\"Ocurrió un error inesperado al intentar insertar \" + ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message, false);");
                        Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t}");
                }
                else 
                { 
                    Repositories.AppendLine("\t\tpublic (string, bool) alta" + nombreDeClase + "(" + nombreDeClase + " " + nombreClasePrimeraMinuscula +"Model)");
                    Repositories.AppendLine("\t\t{");
                        Repositories.AppendLine("\t\t\ttry");
                        Repositories.AppendLine("\t\t\t{");
                            Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades." + nombreDeClase + ".Attach(" + nombreClasePrimeraMinuscula + "Model);");
                            Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades.Entry(" + nombreClasePrimeraMinuscula + "Model).State = EntityState.Added;");
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
                //BAJA
                if (DB2)
                {
                    Repositories.AppendLine("\t\tpublic (string, bool) baja" + nombreDeClase + "(" + columnasClave + ", " + nombreDeClase + " " + nombreClasePrimeraMinuscula + ")");
                    Repositories.AppendLine("\t\t{");
            	        Repositories.AppendLine("\t\t\ttry");
            	        Repositories.AppendLine("\t\t\t{");            	             
                            List<DataColumn> columnasUpdate = (from c in columnas where !claves.Contains(c) select c).ToList();
                            Repositories.AppendLine("\t\t\t\tComandoDB2 SQLconsulta = new ComandoDB2(string.Empty, \"DB2_Tributos\");");
                            Repositories.AppendLine("\t\t\t\tSQLconsulta.Consulta = \"UPDATE " + tabla + " SET " + string.Join(" AND ", (from c in columnasUpdate select c.ColumnName + " = ?").ToList()) + "\" +");
                            Repositories.AppendLine("\t\t\t\t\t\" WHERE " + string.Join(" AND ", (from c in claves select c.ColumnName + " = ?").ToList()) + "\";");
                            Repositories.AppendLine();
                                    
                            foreach (DataColumn c in columnasUpdate)
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
                	        Repositories.AppendLine("\t\t\t\treturn (\"Ocurrió un error inesperado al intentar eliminar \" + ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message, false);");
            	        Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t}"); 
                }
                else
                {
	                Repositories.AppendLine("\t\tpublic (string, bool) baja" + nombreDeClase + "(" + nombreDeClase + " " + nombreClasePrimeraMinuscula + "Model)");
                        Repositories.AppendLine("\t\t{");
            	                Repositories.AppendLine("\t\t\ttry");
            	                Repositories.AppendLine("\t\t\t{");
                	                Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades." + nombreDeClase + ".Attach(" + nombreClasePrimeraMinuscula + "Model);");
                	                Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades.Entry(" + nombreClasePrimeraMinuscula + "Model).State = EntityState.Modified;");
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
                //MODIFICAR
                if (DB2)
                {
                    bool where = clavesConsulta.Count > 0;
                    if (where)
                    {
                        Repositories.AppendLine("\t\tpublic (string, bool) modificacion" + nombreDeClase + "(" + columnasClave + ", " + nombreDeClase + " " + nombreClasePrimeraMinuscula + ")");
                        Repositories.AppendLine("\t\t{");
            	            Repositories.AppendLine("\t\t\ttry");
            	            Repositories.AppendLine("\t\t\t{");            	             
                                List<DataColumn> columnasUpdate = (from c in columnas where !claves.Contains(c) select c).ToList();
                                Repositories.AppendLine("\t\t\t\tComandoDB2 SQLconsulta = new ComandoDB2(string.Empty, \"DB2_Tributos\");");
                                Repositories.AppendLine("\t\t\t\tSQLconsulta.Consulta = \"UPDATE " + tabla + " SET " + string.Join(" AND ", (from c in columnasUpdate select c.ColumnName + " = ?").ToList()) + "\" +");
                                Repositories.AppendLine("\t\t\t\t\t\" WHERE " + string.Join(" AND ", (from c in claves select c.ColumnName + " = ?").ToList()) + "\";");
                                Repositories.AppendLine();
                                    
                                foreach (DataColumn c in columnasUpdate)
                                {
                                    Repositories.AppendLine("\t\t\t\tSQLconsulta.Agregar(\"@" + c.ColumnName + "\", " + Mapeo[Tipo(c)] + ", " + nombreClasePrimeraMinuscula + "." + c.ColumnName + ");");
                                }
                                Repositories.AppendLine();
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
                	            Repositories.AppendLine("\t\t\t\treturn (\"Ocurrió un error inesperado al intentar modificar \" + ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message, false);");
            	            Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine("\t\t}"); 
                    }
                }
                else
                {
	                Repositories.AppendLine("\t\tpublic (string, bool) modificacion" + nombreDeClase + "(" + nombreDeClase + " " + nombreClasePrimeraMinuscula + "Model)");
                        Repositories.AppendLine("\t\t{");
            	                Repositories.AppendLine("\t\t\ttry");
            	                Repositories.AppendLine("\t\t\t{");
                	                Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades." + nombreDeClase + ".Attach(" + nombreClasePrimeraMinuscula + "Model);");
                	                Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades.Entry(" + nombreClasePrimeraMinuscula + "Model).State = EntityState.Modified;");
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
                //OBTENER POR ID
                if(DB2)
                {
                    Repositories.AppendLine("\t\tpublic " + nombreDeClase + " obtenerPorId(" + columnasClave + ")");
                    Repositories.AppendLine("\t\t{");
            	        Repositories.AppendLine("\t\t\t" + nombreDeClase + " Resultado = new " + nombreDeClase +"();");
		                Repositories.AppendLine();
                        Repositories.AppendLine("\t\t\ttry");
                        Repositories.AppendLine("\t\t\t{");            	             
            	            Repositories.AppendLine("\t\t\t\tComandoDB2 SQLconsulta = new ComandoDB2(string.Empty, \"DB2_Tributos\");");
		                    Repositories.AppendLine();

                		    var campoBaja = camposConsulta.Where(c => c.ToLower().Contains("baja") && c.ToLower().StartsWith("f")).FirstOrDefault();

			                bool where = campoBaja != null;
			                if (!where) where = clavesConsulta.Count > 0;
                            Repositories.AppendLine("\t\t\t\tSQLconsulta.Consulta = \"SELECT " + string.Join(", ", camposConsulta.ToArray()) + " FROM " + tabla + "\" +");
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
                            string instancia = char.ToLower(nombreDeClase[0]) + nombreDeClase.Substring(1);
			                    Repositories.AppendLine("\t\t\t\t\t" + nombreDeClase + " " + instancia + " = new " + nombreDeClase + "();");
			                    Repositories.AppendLine("\t\t\t\t\t" + nombreDeClase + " instancia = FuncionesGenerales.RellenarCampos(SQLconsulta, " + instancia + ") as " + nombreDeClase + ";");
            	            Repositories.AppendLine("\t\t\t\t};");
		                    Repositories.AppendLine();
            	            Repositories.AppendLine("\t\t\t\tSQLconsulta.CerrarConexion();");
            	        Repositories.AppendLine("\t\t\t}");
            	        Repositories.AppendLine("\t\t\tcatch (Exception ex)");
            	        Repositories.AppendLine("\t\t\t{");
                	        Repositories.AppendLine("\t\t\t\treturn (ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);");
            	        Repositories.AppendLine("\t\t\t}");
		                Repositories.AppendLine();
            	        Repositories.AppendLine("\t\t\treturn Resultado;");
                    Repositories.AppendLine("\t\t}");
                }
                else
                {
	                Repositories.AppendLine("\t\tpublic " + nombreDeClase + " obtenerPorId(" + columnasClave + ")");
                        Repositories.AppendLine("\t\t{");
            	                Repositories.AppendLine("\t\t\t" + nombreDeClase + " solicitado = null;");
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
                //TODOS
                if(DB2)
                {
                    Repositories.AppendLine("\t\tpublic List<" + nombreDeClase + "> obtenerTodos()");
			        Repositories.AppendLine("\t\t{");
			            Repositories.AppendLine("\t\t\tList<" + nombreDeClase + "> todos = new List<" + nombreDeClase + ">();");
			            Repositories.AppendLine();
                        Repositories.AppendLine("\t\t\ttry");
                        Repositories.AppendLine("\t\t\t{");            	             
			                Repositories.AppendLine("\t\t\t\tComandoDB2 SQLconsulta = new ComandoDB2(\"\", \"DB2_Tributos\");");
			                Repositories.AppendLine();

			                bool where = clavesConsulta.Count > 0;
                            Repositories.AppendLine("\t\t\t\tSQLconsulta.Consulta = \"SELECT " + string.Join(", ", camposConsulta.ToArray()) + " FROM " + tabla + "\" +");
                                Repositories.AppendLine("\t\t\t\t\t\"" + (where ? (" WHERE " + string.Join(" AND ", (from c in clavesConsulta select c[0] + " = ?").ToArray())) : string.Empty));
			                Repositories.AppendLine();

			                foreach (string[] clave in clavesConsulta)
			                {
				                Repositories.AppendLine("\t\t\t\tSQLconsulta.Agregar(\"@" + clave[0] + "\", " + Mapeo[clave[1]] + ", " + clave[0] + ");");
			                }

			                Repositories.AppendLine();
			                Repositories.AppendLine("\t\t\t\twhile (SQLconsulta.HayRegistros())");
			                Repositories.AppendLine("\t\t\t\t{");
			                string instancia = char.ToLower(nombreDeClase[0]) + nombreDeClase.Substring(1);
			                    Repositories.AppendLine("\t\t\t\t\t" + nombreDeClase + " " + instancia + " = new " + nombreDeClase + "();");
			                    Repositories.AppendLine("\t\t\t\t\t" + nombreDeClase + " instancia = FuncionesGenerales.RellenarCampos(SQLconsulta, " + instancia + ") as " + nombreDeClase + ";");
			                    Repositories.AppendLine("\t\t\t\t\ttodos.Add(instancia);");
			                Repositories.AppendLine("\t\t\t\t}");
			                Repositories.AppendLine("\t\t\t\tSQLconsulta.CerrarConexion();");
            	        Repositories.AppendLine("\t\t\t}");
            	        Repositories.AppendLine("\t\t\tcatch (Exception ex)");
            	        Repositories.AppendLine("\t\t\t{");
                	        Repositories.AppendLine("\t\t\t\treturn (ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);");
            	        Repositories.AppendLine("\t\t\t}");
                        Repositories.AppendLine();
			            Repositories.AppendLine("\t\t\treturn todos;");
			        Repositories.AppendLine("\t\t}");
                }
                else
                {
                    Repositories.AppendLine("\t\tpublic List<" + nombreDeClase + "> obtenerTodos()");
                    Repositories.AppendLine("\t\t{");
                        Repositories.AppendLine("\t\t\treturn (from busqueda in BaseDeDatos" + espacio + "." + espacio + "Entidades." + nombreDeClase + "");
                            Repositories.AppendLine("\t\t\t\t\tselect busqueda).ToList();");
                    Repositories.AppendLine("\t\t}");
                }
                Repositories.AppendLine();
                // RECUPERAR
                if(DB2)
                {
                    Repositories.AppendLine("\t\tpublic (string, bool) recuperar" + nombreDeClase + "(" + nombreDeClase + " " + nombreClasePrimeraMinuscula +"Model)");
                    Repositories.AppendLine("\t\t{");
                        Repositories.AppendLine("\t\t\ttry");
                        Repositories.AppendLine("\t\t\t{");
                            Repositories.AppendLine("\t\t\t\tComandoDB2 SQLconsulta = new ComandoDB2(string.Empty, \"DB2_Tributos\");");
                            Repositories.AppendLine("\t\t\t\tSQLconsulta.Consulta = \"INSERT INTO " + nombreDeClase + " (" + string.Join(", ", (from c in columnas select c.ColumnName).ToList()) + ") VALUES (" + string.Join(",", Enumerable.Repeat("?", columnas.Count)) + ")\";");
                            Repositories.AppendLine();
                            foreach (DataColumn c in columnas)
                            {
                                Repositories.AppendLine("\t\t\t\tSQLconsulta.Agregar(\"@" + c.ColumnName + "\", " + Mapeo[Tipo(c)] + ", " + nombreClasePrimeraMinuscula + "Model." + c.ColumnName + ");");
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
                            Repositories.AppendLine("\t\t\t\treturn (\"Ocurrió un error inesperado al intentar recuperar \" + ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message, false);");
                        Repositories.AppendLine("\t\t\t}");
                    Repositories.AppendLine("\t\t}");
                }
                else
                {
                    Repositories.AppendLine("\t\tpublic (string, bool) recuperar" + nombreDeClase + "(" + nombreDeClase + " " + nombreClasePrimeraMinuscula + "Model)");
                    Repositories.AppendLine("\t\t{");
            	            Repositories.AppendLine("\t\t\ttry");
            	            Repositories.AppendLine("\t\t\t{");
                	            Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades." + nombreDeClase + ".Attach(" + nombreClasePrimeraMinuscula +"Model);");
                	            Repositories.AppendLine("\t\t\t\tBaseDeDatos" + espacio + "." + espacio + "Entidades.Entry(" + nombreClasePrimeraMinuscula + "Model).State = EntityState.Modified;");
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
			    Repositories.AppendLine("\t}");
			Repositories.AppendLine("}");

            try
            {
                string pathRepositories = TXTpathCapas.Text + tabla + @"\Repositories\";
                string pathClaseRepositories = pathRepositories + tabla + "Repositories.cs";
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

            return Repositories.ToString();
		}

        private string RepositoriesInterface(string tabla, List<DataColumn> claves)
		{
			string nombreDeClase = tabla;
			string nombreClasePrimeraMinuscula = nombreDeClase[0].ToString().ToLower() + nombreDeClase.Substring(1);
			string espacioDeNombres = TXTespacioDeNombres.Text;
            string columnasClave = string.Join(", ", (from c in claves select Tipo(c) + " " + c.ColumnName).ToList());

            StringBuilder RepositoriesInterface = new StringBuilder();

			RepositoriesInterface.AppendLine("using SistemaMunicipalGeneral.Controles;");
			RepositoriesInterface.AppendLine("using System.Collections.Generic;");
			RepositoriesInterface.AppendLine("using System.Data.Odbc;");
			RepositoriesInterface.AppendLine("using " + espacioDeNombres + ".Model;");
			RepositoriesInterface.AppendLine();
			RepositoriesInterface.AppendLine("namespace " + espacioDeNombres + ".Repositories");
			RepositoriesInterface.AppendLine("{");
			RepositoriesInterface.AppendLine("\tpublic interface " + nombreDeClase + "RepositoriesInterface");
			RepositoriesInterface.AppendLine("\t{");
            RepositoriesInterface.AppendLine("\t\t(string, bool) alta" + nombreDeClase + "(" + nombreDeClase + " " + nombreClasePrimeraMinuscula + "Model);");
            RepositoriesInterface.AppendLine();
            RepositoriesInterface.AppendLine("\t\t(string, bool) baja" + nombreDeClase + "(" + nombreDeClase + " " + nombreClasePrimeraMinuscula + "Model);");
            RepositoriesInterface.AppendLine();
            RepositoriesInterface.AppendLine("\t\t(string, bool) modificacion" + nombreDeClase + "(" + nombreDeClase + " " + nombreClasePrimeraMinuscula + "Model);");
            RepositoriesInterface.AppendLine();
            RepositoriesInterface.AppendLine("\t\t" + nombreDeClase + " obtenerPorId(" + columnasClave + ");");
            RepositoriesInterface.AppendLine();
            RepositoriesInterface.AppendLine("\t\tList <" + nombreDeClase + "> obtenerTodos();");
            RepositoriesInterface.AppendLine();
            RepositoriesInterface.AppendLine("\t\t(string, bool) recuperar" + nombreDeClase + "(" + nombreDeClase + " " + nombreClasePrimeraMinuscula + "Model);");
			RepositoriesInterface.AppendLine("\t}");
			RepositoriesInterface.AppendLine("}");

            try
            {
                string pathRepositories = TXTpathCapas.Text + tabla + @"\Repositories\";
                string pathClaseRepositoriesInterface = pathRepositories + tabla + "RepositoriesInterface.cs";
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

            return RepositoriesInterface.ToString();
		}

		private string Service(string tabla, List<DataColumn> columnas, List<DataColumn> claves, bool DB2)
		{
            string origen = DB2 ? "Model" : "Dto";
            string nombreDeClase = tabla;
			string nombreClasePrimeraMinuscula = nombreDeClase[0].ToString().ToLower() + nombreDeClase.Substring(1);
			string espacioDeNombres = TXTespacioDeNombres.Text;
            string columnasClave = string.Join(", ", (from c in claves select c.ColumnName).ToList());
            string columnasClaveTipo = string.Join(", ", (from c in claves select Tipo(c) + " " + c.ColumnName).ToList());

            StringBuilder Service = new StringBuilder();

			Service.AppendLine("using System;");
			Service.AppendLine("using System.Collections.Generic;");
            Service.AppendLine("using SistemaMunicipalGeneral;");
			Service.AppendLine("using System.Linq;");
			Service.AppendLine("using System.Web;");
			Service.AppendLine("using " + espacioDeNombres + ".Model;");
			Service.AppendLine("using " + espacioDeNombres + ".Repositories;");
			Service.AppendLine();
            Service.AppendLine("namespace " + espacioDeNombres + ".Service");
			Service.AppendLine("{");
			    Service.AppendLine("\tpublic class " + nombreDeClase + "Service : " + nombreDeClase + "ServiceInterface");
			    Service.AppendLine("\t{");
			        Service.AppendLine("\t\tprivate readonly " + nombreDeClase + "RepositoriesInterface _repositories;");
			        Service.AppendLine();

			        Service.AppendLine("\t\tpublic " + nombreDeClase + "Service(" + nombreDeClase + "RepositoriesInterface repositories)");
			        Service.AppendLine("\t\t{");
			            Service.AppendLine("\t\t\t_repositories = repositories;");
			        Service.AppendLine("\t\t}");
                    Service.AppendLine();
                    //ALTA
                    Service.AppendLine("\t\tpublic (string, bool) alta" + nombreDeClase + "(" + nombreDeClase + origen + " " + nombreClasePrimeraMinuscula + origen + ")");
                    Service.AppendLine("\t\t{");
                        Service.AppendLine("\t\t\t" + nombreDeClase + (DB2 ? origen : string.Empty) + " nuevo = new " + nombreDeClase + (DB2 ? origen : string.Empty) + "()");
                        Service.AppendLine("\t\t\t{");
                        int i = 0;
                        foreach (DataColumn columna in columnas)
                        {
                            Service.AppendLine("\t\t\t\t" + columna.ColumnName + " = " + nombreClasePrimeraMinuscula + origen + "." + columna.ColumnName + (i < columnas.Count ? "," : string.Empty));
                            i++;
                        }
                        Service.AppendLine("\t\t\t};");
                    Service.AppendLine("\t\t\t(string, bool) respuesta = _repositories.alta" + nombreDeClase + "(nuevo);");
                    Service.AppendLine();
                    Service.AppendLine("\t\t\treturn respuesta;");
                    Service.AppendLine("\t\t}");
                    Service.AppendLine();
                    //BAJA
                    Service.AppendLine("\t\tpublic (string, bool) baja" + nombreDeClase + "(" + columnasClaveTipo + ", int codigoBaja, string motivoBaja)");
                    Service.AppendLine("\t\t{");
                        Service.AppendLine("\t\t\t" + nombreDeClase + (DB2 ? origen : string.Empty) + " solicitado = _repositories.obtenerPorId(" + columnasClave + ");");
                        Service.AppendLine("\t\t\tif (solicitado != null)");
                        Service.AppendLine("\t\t\t{");
                            Service.AppendLine("\t\t\t\tsolicitado.FechaBaja = System.DateTime.Now;");
                            Service.AppendLine("\t\t\t\tsolicitado.UsuarioBaja = Config.UsuarioMagic;");
                            Service.AppendLine("\t\t\t\tsolicitado.CodigoBaja = codigoBaja;");
                            Service.AppendLine("\t\t\t\tsolicitado.MotivoBaja = motivoBaja;");
                        Service.AppendLine("\t\t\t}");
                        Service.AppendLine("\t\t\t(string, bool) respuesta = _repositories.baja" + nombreDeClase + "(solicitado);");
                        Service.AppendLine();
                        Service.AppendLine("\t\t\treturn respuesta;");
                    Service.AppendLine("\t\t}");
                    Service.AppendLine();
                    //MODIFICACION
                    Service.AppendLine("\t\tpublic (string, bool) modificacion" + nombreDeClase + "(" + nombreDeClase + origen + " " + nombreClasePrimeraMinuscula + origen + ")");
                    Service.AppendLine("\t\t{");
                        string columnasBusqueda = string.Join(", ", (from c in claves select nombreClasePrimeraMinuscula + origen + "." + c.ColumnName).ToList());
                        Service.AppendLine("\t\t\t" + nombreDeClase + (DB2 ? origen : string.Empty) + " solicitado = _repositories.obtenerPorId(" + columnasBusqueda + ");");
                        Service.AppendLine("\t\t\tif (solicitado != null)");
                        Service.AppendLine("\t\t\t{");
                            Service.AppendLine("\t\t\t\tsolicitado.FechaModificacion = System.DateTime.Now;");
                            Service.AppendLine("\t\t\t\tsolicitado.UsuarioModificacion = Config.UsuarioMagic;");
                        Service.AppendLine("\t\t\t}");
                        Service.AppendLine("\t\t\t(string, bool) respuesta = _repositories.modificacion" + nombreDeClase + "(solicitado);");
                        Service.AppendLine();
                        Service.AppendLine("\t\t\treturn respuesta;");
                    Service.AppendLine("\t\t}");
                    Service.AppendLine();
                    //OBTENER POR ID
                    Service.AppendLine("\t\tpublic " + nombreDeClase + origen + " obtenerPorId(" + columnasClaveTipo + ")");
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
                            Service.AppendLine("\t\t\t\t" + nombreDeClase + "Dto solicitadoDto = new " + nombreDeClase + "Dto();");
                            Service.AppendLine();
                            Service.AppendLine("\t\t\t\tsolicitadoDto.new" + nombreDeClase + "Dto(solicitado);");
                            Service.AppendLine("\t\t\t\treturn solicitadoDto;");
                        }
                        Service.AppendLine("\t\t\t}");
                        Service.AppendLine("\t\t\telse");
                        Service.AppendLine("\t\t\t{");
                        Service.AppendLine("\t\t\t\treturn null;");
                        Service.AppendLine("\t\t\t}");
                    Service.AppendLine("\t\t}");
                    Service.AppendLine();
                    //TODOS
                    Service.AppendLine("\t\tpublic List<" + nombreDeClase + origen + "> obtenerTodos()");
                    Service.AppendLine("\t\t{");
                        Service.AppendLine("\t\t\tList<" + nombreDeClase + (DB2 ? origen : string.Empty) + "> listado = new List<" + nombreDeClase + (DB2 ? origen : string.Empty) + ">();");
                        Service.AppendLine();
                        Service.AppendLine("\t\t\tlistado = _repositories.obtenerTodos();");
                        Service.AppendLine("\t\t\tif (listado.Count() > 0)");
                        Service.AppendLine("\t\t\t{");
                            if (DB2)
                            {
                                Service.AppendLine("\t\t\treturn listado;");
                            }
                            else
                            {
                                Service.AppendLine("\t\t\t\tList<" + nombreDeClase + "Dto> " + nombreClasePrimeraMinuscula + "Dto = new List<" + nombreDeClase + "Dto>();");
                                Service.AppendLine("\t\t\t\tforeach (" + nombreDeClase + " model in listado)");
                                Service.AppendLine("\t\t\t\t{");
                                Service.AppendLine("\t\t\t\t\t" + nombreDeClase + "Dto dto = new " + nombreDeClase + "Dto();");
                                Service.AppendLine("\t\t\t\t\t" + nombreClasePrimeraMinuscula + "Dto.Add(dto.new" + nombreDeClase + "Dto(model));");
                                Service.AppendLine("\t\t\t\t}");
                                Service.AppendLine("\t\t\t\treturn " + nombreClasePrimeraMinuscula + "Dto;");
                            }
                        Service.AppendLine("\t\t\t}");
                        Service.AppendLine("\t\t\treturn null;");
                    Service.AppendLine("\t\t}");
                    Service.AppendLine();
                    //RECUPERACION
                    Service.AppendLine("\t\tpublic (string, bool) recuperar" + nombreDeClase + "(" + columnasClaveTipo + ")");
                    Service.AppendLine("\t\t{");
                        Service.AppendLine("\t\t\t" + nombreDeClase + (DB2 ? origen : string.Empty) + " solicitado = _repositories.obtenerPorId(" + columnasClave + ");");
                        Service.AppendLine("\t\t\tif (solicitado != null)");
                        Service.AppendLine("\t\t\t{");
                            Service.AppendLine("\t\t\t\tsolicitado.FechaBaja = new DateTime(1900, 1, 1);");
                            Service.AppendLine("\t\t\t\tsolicitado.UsuarioBaja = string.Empty;");
                            Service.AppendLine("\t\t\t\tsolicitado.CodigoBaja = 0;");
                            Service.AppendLine("\t\t\t\tsolicitado.MotivoBaja = string.Empty;");
                        Service.AppendLine("\t\t\t}");
                        Service.AppendLine("\t\t\t(string, bool) respuesta = _repositories.recupera" + nombreDeClase + "(solicitado);");
                        Service.AppendLine();
                        Service.AppendLine("\t\t\treturn respuesta;");
                    Service.AppendLine("\t\t}");
                Service.AppendLine("\t}");
			Service.AppendLine("}");

            try
            {
                string pathService = TXTpathCapas.Text + tabla + @"\Service\";
                string pathClaseService = pathService + tabla + "Service.cs";
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

            return Service.ToString();
		}

		private string ServiceInterface(string tabla, List<DataColumn> claves, bool DB2)
		{
            string origen = DB2 ? "Model" : "Dto";
			string nombreDeClase = tabla;
			string nombreClasePrimeraMinuscula = nombreDeClase[0].ToString().ToLower() + nombreDeClase.Substring(1);
			string espacioDeNombres = TXTespacioDeNombres.Text;
			string columnasClave = string.Join(", ", (from c in claves select Tipo(c) + " " + c.ColumnName).ToList());

			StringBuilder ServiceInterface = new StringBuilder();

            ServiceInterface.AppendLine("using System;");
            ServiceInterface.AppendLine("using System.Collections.Generic;");
            ServiceInterface.AppendLine("using System.Data.Odbc;");
			ServiceInterface.AppendLine("using SistemaMunicipalGeneral.Controles;");
			ServiceInterface.AppendLine("using " + espacioDeNombres + "." + origen + ";");
			ServiceInterface.AppendLine();
            ServiceInterface.AppendLine("namespace " + espacioDeNombres + ".Service");
			ServiceInterface.AppendLine("{");
			    ServiceInterface.AppendLine("\tpublic interface " + nombreDeClase + "ServiceInterface");
			    ServiceInterface.AppendLine("\t{");
			        ServiceInterface.AppendLine("\t\t(string, bool) alta" + nombreDeClase + "(" + nombreDeClase + origen + " " + nombreClasePrimeraMinuscula + origen + ");");
			        ServiceInterface.AppendLine();
                    ServiceInterface.AppendLine("\t\t(string, bool) baja" + nombreDeClase + "(" + columnasClave + ", int codigoBaja, string motivoBaja);");
                    ServiceInterface.AppendLine();
                    ServiceInterface.AppendLine("\t\t(string, bool) modificacion" + nombreDeClase + "(" + nombreDeClase + origen + " " + nombreClasePrimeraMinuscula + origen + ");");
			        ServiceInterface.AppendLine();
			        ServiceInterface.AppendLine("\t\t" + nombreDeClase + origen +" obtenerPorId(" + columnasClave + ");");
                    ServiceInterface.AppendLine();
                    ServiceInterface.AppendLine("\t\tList<" + nombreDeClase + "> obtenerTodos();");
			        ServiceInterface.AppendLine();
			        ServiceInterface.AppendLine("\t\t(string, bool) recuperar" + nombreDeClase + "(" + columnasClave + ");");
			    ServiceInterface.AppendLine("\t}");
			ServiceInterface.AppendLine("}");

            try
            {
                string pathService = TXTpathCapas.Text + tabla + @"\Service\";
                string pathClaseServiceInterface = pathService + tabla + "ServiceInterface.cs";
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

            return ServiceInterface.ToString();
		}

		public string Tipo(DataColumn column)
		{
			if (column == null || column.DataType == null)
				return string.Empty;

			Type tipo = column.DataType;
            if (TIPOS.Keys.Contains(tipo))
            {
                return TIPOS[tipo];
            }
            else
            {
                return ERROR;
            }
        }

        private void BTNgenerar_Click(object sender, EventArgs e)
        {
			TXTclase.Text = Clase(CMBtablas.Items[CMBtablas.SelectedIndex].ToString());
		}

        private void CMBservidor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RDBdb2.Checked)
            {
                CMBbases.Items.Clear();
                CMBbases.Items.AddRange(new object[] {"CONTABIL", "CONTAICD", "CONTAIMV", "CONTCBEL", "CONTIDS", "DOCUMENT", "GENERAL", "GIS", "HISTABM", "HISTORIC", "INFORMAT", "LICENCIA", "RRHH", "SISUS", "TRIBUTOS"});
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

                        string query = "SELECT name FROM sys.databases WHERE state = 0"; // Solo bases "online"

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
                }
            }
            if (CMBbases.Items.Count > 0)
            {
                CMBbases.SelectedIndex = 0;
            }
            CMBbases.Refresh();
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
            datos.BaseDeDatos = CMBbases.Items[CMBbases.SelectedIndex].ToString();
            datos.Servidor = CMBservidor.Items[CMBservidor.SelectedIndex].ToString();
            return datos;
        }

        private void TablasBase()
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

        private void CamposTabla()
		{
            LBLtablaSeleccionada.Text = CMBtablas.Items[CMBtablas.SelectedIndex].ToString() + ":";
            LSVcampos.Items.Clear();
            string tablaSeleccionada = CMBtablas.Items[CMBtablas.SelectedIndex].ToString();
            if (RDBdb2.Checked)
            {
                try
                {
                    Ejecutar datos = EstablecerConexion();
                    ComandoDB2 Db2 = new ComandoDB2("SELECT LTRIM(RTRIM(NAME)) AS Nombre, COLTYPE as Tipo, LENGTH as Longitud, SCALE as Escala, CASE WHEN NULLS = 'N' THEN 'NO' ELSE 'SI' END as AceptaNulos FROM SYSIBM.SYSCOLUMNS WHERE TBNAME = '" + tablaSeleccionada + "'", datos.ObtenerConexion());
                    Db2.Conexion = new System.Data.Odbc.OdbcConnection(datos.ObtenerConexion());

                    while (Db2.HayRegistros())
                    {
                        var nombre = Db2.CampoStr("Nombre").ToUpper();
                        var tipo = Db2.CampoStr("Tipo").ToUpper();
                        var longitud = Db2.CampoInt("Longitud").ToString();
                        var escala = Db2.CampoInt("Escala").ToString();
                        var aceptaNulos = Db2.CampoStr("AceptaNulos");

                        ListViewItem item = new ListViewItem(nombre);
                        item.SubItems.Add(tipo);
                        item.SubItems.Add(longitud);
                        item.SubItems.Add(escala);
                        item.SubItems.Add(aceptaNulos);
                        LSVcampos.Items.Add(item);
                    }
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
                catch (Exception ex)
                {
                }
            }
            else
            {
                try
                {
                    string servidor = CMBservidor.Items[CMBservidor.SelectedIndex].ToString().ToUpper();
                    string connectionString = @"Data Source=SQL" + servidor + @"\" + servidor + "; Initial Catalog=" + CMBbases.Items[CMBbases.SelectedIndex].ToString() + ";Persist Security Info=True;User ID=usuario;Password=ci?r0ba;MultipleActiveResultSets=True";

                    string query = $@"SELECT c.name AS Nombre, ty.name AS Tipo, c.max_length AS Longitud, c.scale AS Escala, CASE WHEN c.is_nullable = 1 THEN 'SI' ELSE 'NO' END AS AceptaNulos "
                        + "FROM sys.columns c "
                        + "JOIN sys.types ty ON c.user_type_id = ty.user_type_id "
                        + "JOIN sys.tables t ON c.object_id = t.object_id "
                        + "WHERE t.name = @tabla "
                        + "ORDER BY c.column_id;";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        string[] partes = tablaSeleccionada.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        tablaSeleccionada = partes[partes.Length - 1];

                        cmd.Parameters.AddWithValue("@tabla", tablaSeleccionada);

                        try
                        {
                            conn.Open();
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var nombre = reader["Nombre"].ToString().ToUpper();
                                    var tipo = reader["Tipo"].ToString().ToUpper();
                                    var longitud = reader["Longitud"].ToString();
                                    var escala = reader["Escala"].ToString();
                                    var aceptaNulos = reader["AceptaNulos"].ToString();

                                    ListViewItem item = new ListViewItem(nombre);
                                    item.SubItems.Add(tipo);
                                    item.SubItems.Add(longitud);
                                    item.SubItems.Add(escala);
                                    item.SubItems.Add(aceptaNulos);
                                    LSVcampos.Items.Add(item);
                                }
                            }
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
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
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            ComprobarTiposDeCampos(tablaSeleccionada);
            LSVcampos.Refresh();
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

            if (columnasError.Count > 0)
            {
                string columnas = string.Join("\r\n", columnasError);
                MessageBox.Show("NO SE PUEDE PROCESAR LA SIGUIENTE TABLA DEBIDO A INCONSISTENCIAS CON LOS SIGUIENTES CAMPOS:\r\n\r\n" + columnas, "ATENCIÓN!!!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                BTNgenerar.Enabled = false;
            }
            else
            {
                BTNgenerar.Enabled = true;
            }
        }

        private void RDBsql_CheckedChanged(object sender, EventArgs e)
        {
            CHKquitarEsquema.Visible = RDBsql.Checked;
            CHKquitarEsquema.Refresh();
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

        private void RDBdb2_CheckedChanged(object sender, EventArgs e)
        {
            if (RDBdb2.Checked)
            {
                CHKquitarEsquema.Visible = false;
                CHKquitarEsquema.Refresh();
                CMBservidor.Items.Clear();
                CMBservidor.Items.AddRange(new object[] {"133.123.120.120", "SERVER04", "SERVER01"});
                if (CMBservidor.Items.Count > 0)
                {
                    CMBservidor.SelectedIndex = 0;
                }
                CMBservidor.Refresh();
            }
        }

        private void BTNdirectorioCapas_Click(object sender, EventArgs e)
        {
            FBDdirectorioCapas.ShowDialog();
            TXTpathCapas.Text = FBDdirectorioCapas.SelectedPath;
        }

        private void CMBtablas_TextUpdate(object sender, EventArgs e)
        {
            string texto = CMBtablas.Text;
            List<string> filtrados = tablasBase
                .Where(item => item.ToUpper().Contains(texto.ToUpper()))
                .ToList();

            // Evitar que parpadee
            CMBtablas.BeginUpdate();
            CMBtablas.Items.Clear();
            CMBtablas.Items.AddRange(filtrados.ToArray());
            CMBtablas.DroppedDown = true;
            CMBtablas.SelectionStart = texto.Length;
            CMBtablas.SelectionLength = 0;
            CMBtablas.EndUpdate();
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
    }
}

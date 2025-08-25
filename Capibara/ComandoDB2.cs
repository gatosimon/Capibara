using System;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Reflection;
using System.Text;
using SistemaMunicipalGeneral.Modelos;

namespace Capibara
{
	/// <summary>
	///
	/// </summary>
	// Token: 0x0200008B RID: 139
	public class ComandoDB2
	{
		// Token: 0x170003AB RID: 939
		// (get) Token: 0x0600093C RID: 2364 RVA: 0x0001EA84 File Offset: 0x0001CC84
		// (set) Token: 0x0600093D RID: 2365 RVA: 0x0001EA9C File Offset: 0x0001CC9C
		public OdbcConnection Conexion
		{
			get
			{
				return this._conexionDB2;
			}
			set
			{
				this._conexionDB2 = value;
				this._comando.Connection = this._conexionDB2;
			}
		}

		/// <summary>
		/// Obtiene o establece la instrucción T-SQL que se va a ejecutar.
		/// </summary>
		// Token: 0x170003AC RID: 940
		// (get) Token: 0x0600093E RID: 2366 RVA: 0x0001EAB8 File Offset: 0x0001CCB8
		// (set) Token: 0x0600093F RID: 2367 RVA: 0x0001EAD5 File Offset: 0x0001CCD5
		public string Consulta
		{
			get
			{
				return this._comando.CommandText;
			}
			set
			{
				this._comando.CommandText = value;
			}
		}

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x06000940 RID: 2368 RVA: 0x0001EAFC File Offset: 0x0001CCFC
		public int CantidadCampos
		{
			get
			{
				bool flag = this._lector != null;
				int result;
				if (flag)
				{
					result = this._lector.FieldCount;
				}
				else
				{
					result = -1;
				}
				return result;
			}
		}

		/// <summary>
		/// Obtiene una lista con todos los parámetro que se utilizarán en la lista.
		/// </summary>
		// Token: 0x170003AE RID: 942
		// (get) Token: 0x06000941 RID: 2369 RVA: 0x0001EB2C File Offset: 0x0001CD2C
		public OdbcParameterCollection Parametros
		{
			get
			{
				return this._comando.Parameters;
			}
		}

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06000942 RID: 2370 RVA: 0x0001EB4C File Offset: 0x0001CD4C
		// (set) Token: 0x06000943 RID: 2371 RVA: 0x0001EB64 File Offset: 0x0001CD64
		public OdbcCommand Comando
		{
			get
			{
				return this._comando;
			}
			set
			{
				this._comando = value;
			}
		}

		/// <summary>
		/// Agrega un parámetro a la consulta
		/// </summary>
		/// <param name="Parametro"></param>
		// Token: 0x06000944 RID: 2372 RVA: 0x0001EB70 File Offset: 0x0001CD70
		public void Agregar(OdbcParameter Parametro)
		{
			this._parametro = Parametro;
			ComandoDB2.ChequearValorDelParametro(this._parametro);
			this._comando.Parameters.Add(this._parametro);
		}

		/// <summary>
		/// Agrega un parámetro a la consulta
		/// </summary>
		/// <param name="Nombre"></param>
		/// <param name="Tipo"></param>
		/// <param name="Valor"></param>
		// Token: 0x06000945 RID: 2373 RVA: 0x0001EBD4 File Offset: 0x0001CDD4
		public void Agregar(string Nombre, OdbcType Tipo, object Valor)
		{
			this.Agregar(new OdbcParameter(Nombre, Tipo)
			{
				Value = Valor
			});
		}

		/// <summary>
		/// Agrega un parámetro a la consulta
		/// </summary>
		/// <param name="Nombre"></param>
		/// <param name="Tipo"></param>
		/// <param name="Valor"></param>
		/// <param name="Tamaño"></param>
		// Token: 0x06000946 RID: 2374 RVA: 0x0001EBFC File Offset: 0x0001CDFC
		public void Agregar(string Nombre, OdbcType Tipo, object Valor, int Tamaño)
		{
			this.Agregar(new OdbcParameter(Nombre, Tipo, Tamaño)
			{
				Value = Valor
			});
		}

		/// <summary>
		/// Cierra la conexión y elimina la instancia de la clase.
		/// </summary>
		// Token: 0x06000947 RID: 2375 RVA: 0x0001EC24 File Offset: 0x0001CE24
		public void Cerrar()
		{
			this.CerrarConexion();
			this._comando.Dispose();
		}

		/// <summary>
		/// Cierra la conexión al servidor SQL.
		/// </summary>
		// Token: 0x06000948 RID: 2376 RVA: 0x0001EC3C File Offset: 0x0001CE3C
		public void CerrarConexion()
		{
			try
			{
				this._comando.Connection.Close();
				bool flag = this._lector != null;
				if (flag)
				{
					this._lector.Close();
					this._lector = null;
				}
			}
			catch (Exception ex)
			{				
			}
		}

		/// <summary>
		/// Reemplaza la comilla simple en comilla doble si el tipo de datos es Varchar, Char, NChar, Text y NVarchar.
		/// </summary>
		/// <param name="p"></param>
		// Token: 0x06000949 RID: 2377 RVA: 0x0001EC98 File Offset: 0x0001CE98
		private static void ChequearValorDelParametro(OdbcParameter p)
		{
			bool flag = (p.DbType == DbType.AnsiStringFixedLength || p.DbType == DbType.Currency || p.DbType == DbType.Int32 || p.DbType == DbType.UInt16 || p.DbType == DbType.Object) && p.Value != DBNull.Value;
			if (flag)
			{
				p.Value = p.Value.ToString().Replace('\u0093', '"').Replace('\u0094', '"').Replace('\u0096', '-');
			}
		}

		// Token: 0x0600094A RID: 2378 RVA: 0x0001ED26 File Offset: 0x0001CF26
		public ComandoDB2()
		{
			this._lector = null;
		}

		///// <summary>
		///// Crea una nueva instancia de la clase.
		///// </summary>
		///// <param name="Consulta"></param>
		//// Token: 0x0600094B RID: 2379 RVA: 0x0001ED38 File Offset: 0x0001CF38
		//public ComandoDB2(string Consulta)
		//{
		//	this._comando = new OdbcCommand(Consulta, this.ObtenerConexion("DB2Connection"));
		//	this._lector = null;
		//}

		/// <summary>
		/// Crea una nueva instancia de la clase.
		/// </summary>
		/// <param name="Consulta"></param>
		/// <param name="Conexion"></param>
		// Token: 0x0600094C RID: 2380 RVA: 0x0001ED86 File Offset: 0x0001CF86
		public ComandoDB2(string Consulta, string Conexion)
		{
			this._comando = new OdbcCommand(Consulta, this.ObtenerConexion(Conexion));
		}

		/// <summary>
		/// Crea una nueva instancia de la clase.
		/// </summary>
		/// <param name="Consulta"></param>
		/// <param name="TamañoDePagina"></param>
		/// <param name="IndiceActual"></param>
		/// <param name="EsPostBack"></param>
		// Token: 0x0600094D RID: 2381 RVA: 0x0001EDBE File Offset: 0x0001CFBE
		public ComandoDB2(string Consulta, int TamañoDePagina, int IndiceActual, bool EsPostBack)
		{
			this._vistaDeDatos = this.ObtenerVistaDeDatos(Consulta, TamañoDePagina, IndiceActual, EsPostBack);
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x0001EDDC File Offset: 0x0001CFDC
		public void CrearConexion(string nombreDeConexion = "")
		{
			bool flag = nombreDeConexion == "";
			if (flag)
			{
				this._conexionDB2 = this.ObtenerConexion("DB2Connection");
			}
			else
			{
				this._conexionDB2 = this.ObtenerConexion(nombreDeConexion);
			}
			bool flag2 = this._comando == null;
			if (flag2)
			{
				this._comando = new OdbcCommand();
			}
			this._comando.Connection = this._conexionDB2;
		}

		/// <summary>
		/// Crea una nueva transacción.
		/// </summary>
		/// <remarks>Recordar que los procedimientos y funciones como "Ejecutar" hay que pasarlos con el parámetro false para que no cierre la conexión a la base de datos.</remarks>
		// Token: 0x0600094F RID: 2383 RVA: 0x0001EE4C File Offset: 0x0001D04C
		public void ComenzarTransaccion()
		{
			bool flag = this._comando.Connection.State != ConnectionState.Open;
			if (flag)
			{
				this._comando.Connection.Open();
			}
			this._transaccion = this._comando.Connection.BeginTransaction();
			this._comando.Transaction = this._transaccion;
		}

		/// <summary>
		/// Confirma la transacción en la base de datos.
		/// </summary>
		// Token: 0x06000950 RID: 2384 RVA: 0x0001EEB0 File Offset: 0x0001D0B0
		public void ConfirmarTransaccion(bool cerrarConexion = true)
		{
			this._transaccion.Commit();
			if (cerrarConexion)
			{
				this.CerrarConexion();
			}
		}

		/// <summary>
		/// Deshace todos los cambios de la base de datos asociados a la transacción comenzada con el procedimiento "ComenzarTransaccion()".
		/// </summary>
		/// <param name="CerrarConexion">Determina si cierra o no la conexión a la base de datos después de ejecutar la acción.</param>
		// Token: 0x06000951 RID: 2385 RVA: 0x0001EED8 File Offset: 0x0001D0D8
		public void DeshacerTransaccion(bool cerrarConexion = true)
		{
			try
			{
				this._transaccion.Rollback();
			}
			catch (Exception ex)
			{
			}
			if (cerrarConexion)
			{
				this.CerrarConexion();
			}
		}

		/// <summary>
		/// Ejecuta la consulta.
		/// </summary>
		// Token: 0x06000952 RID: 2386 RVA: 0x0001EF1C File Offset: 0x0001D11C
		public int Ejecutar(bool cerrarConexion = true)
		{
			int resultado = -1;
			bool flag = this._comando.Connection.State != ConnectionState.Open;
			if (flag)
			{
				this._comando.Connection.Open();
			}
			bool flag2 = this._lector != null;
			if (flag2)
			{
				this._lector.Close();
				this._lector = null;
			}
			try
			{
				resultado = this._comando.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				this._comando.Connection.Close();
				throw ex;
			}
			if (cerrarConexion)
			{
				this._comando.Connection.Close();
			}
			return resultado;
		}

		/// <summary>
		/// Ejecuta un bloque de código SQL en forma de transacción.
		/// </summary>
		// Token: 0x06000953 RID: 2387 RVA: 0x0001EFC8 File Offset: 0x0001D1C8
		public void EjecutarTransaccion()
		{
			this._comando.Connection.Open();
			string MsgError = "";
			OdbcTransaction Transaccion = this._comando.Connection.BeginTransaction();
			this._comando.Transaction = Transaccion;
			try
			{
				this._comando.ExecuteNonQuery();
				Transaccion.Commit();
			}
			catch (Exception ex)
			{
				Transaccion.Rollback();
				MsgError = ex.Message;
			}
			finally
			{
				this._comando.Connection.Close();
			}
			bool flag = MsgError != "";
			if (flag)
			{
				Exception Excep = new Exception(MsgError);
				throw Excep;
			}
		}

		/// <summary>
		/// Intenta obtener el valor de un campo de un registro.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Para que este método devuelva un valor correcto, previamente se debe haber invocado el método HayRegistros() y haber obtenido el valor true.</remarks>
		// Token: 0x06000954 RID: 2388 RVA: 0x0001F084 File Offset: 0x0001D284
		public object Campo(int nroCampo)
		{
			bool flag = this._lector != null;
			object result;
			if (flag)
			{
				result = this._lector[nroCampo];
			}
			else
			{
				result = null;
			}
			return result;
		}

		/// <summary>
		/// Intenta obtener el valor de un campo de un registro.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Para que este método devuelva un valor correcto, previamente se debe haber invocado el método HayRegistros() y haber obtenido el valor true.</remarks>
		// Token: 0x06000955 RID: 2389 RVA: 0x0001F0B4 File Offset: 0x0001D2B4
		public object Campo(string nombreCampo)
		{
			bool flag = this._lector != null;
			object result;
			if (flag)
			{
				result = this._lector[nombreCampo];
			}
			else
			{
				result = null;
			}
			return result;
		}

		/// <summary>
		/// Retorna el valor de un campo de un registro convertido en string.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Para que este método devuelva un valor correcto, previamente se debe haber invocado el método HayRegistros() y haber obtenido el valor true.</remarks>
		// Token: 0x06000956 RID: 2390 RVA: 0x0001F0E4 File Offset: 0x0001D2E4
		public string CampoStr(string nombreCampo)
		{
			bool flag = this._lector != null;
			string result;
			if (flag)
			{
				result = this._lector[nombreCampo].ToString();
			}
			else
			{
				result = "";
			}
			return result;
		}

		/// <summary>
		/// Retorna el valor de un campo de un registro convertido en DateTime.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Para que este método devuelva un valor correcto, previamente se debe haber invocado el método HayRegistros() y haber obtenido el valor true.</remarks>
		// Token: 0x06000957 RID: 2391 RVA: 0x0001F11C File Offset: 0x0001D31C
		public DateTime CampoDT(string nombreCampo)
		{
			bool flag = this._lector != null && DBNull.Value != this._lector[nombreCampo];
			DateTime result;
			if (flag)
			{
				result = DateTime.Parse(this._lector[nombreCampo].ToString());
			}
			else
			{
				result = DateTime.MinValue;
			}
			return result;
		}

		/// <summary>
		/// Retorna el valor de un campo de un registro convertido en int.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Para que este método devuelva un valor correcto, previamente se debe haber invocado el método HayRegistros() y haber obtenido el valor true.</remarks>
		// Token: 0x06000958 RID: 2392 RVA: 0x0001F174 File Offset: 0x0001D374
		public int CampoInt(string nombreCampo)
		{
			bool flag = this._lector != null && DBNull.Value != this._lector[nombreCampo];
			int result;
			if (flag)
			{
				result = int.Parse(this._lector[nombreCampo].ToString());
			}
			else
			{
				result = 0;
			}
			return result;
		}

		/// <summary>
		/// Retorna el valor de un campo de un registro convertido en long.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Para que este método devuelva un valor correcto, previamente se debe haber invocado el método HayRegistros() y haber obtenido el valor true.</remarks>
		// Token: 0x06000959 RID: 2393 RVA: 0x0001F1C8 File Offset: 0x0001D3C8
		public long CampoLong(string nombreCampo)
		{
			bool flag = this._lector != null && DBNull.Value != this._lector[nombreCampo];
			long result;
			if (flag)
			{
				result = long.Parse(this._lector[nombreCampo].ToString());
			}
			else
			{
				result = 0L;
			}
			return result;
		}

		/// <summary>
		/// Retorna el valor de un campo de un registro convertido en bool.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Para que este método devuelva un valor correcto, previamente se debe haber invocado el método HayRegistros() y haber obtenido el valor true.</remarks>
		// Token: 0x0600095A RID: 2394 RVA: 0x0001F21C File Offset: 0x0001D41C
		public bool CampoBool(string nombreCampo)
		{
			bool flag = this._lector != null && DBNull.Value != this._lector[nombreCampo];
			return flag && bool.Parse(this._lector[nombreCampo].ToString());
		}

		/// <summary>
		/// Retorna el valor de un campo de un registro convertido en decimal.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Para que este método devuelva un valor correcto, previamente se debe haber invocado el método HayRegistros() y haber obtenido el valor true.</remarks>
		// Token: 0x0600095B RID: 2395 RVA: 0x0001F270 File Offset: 0x0001D470
		public decimal CampoDecimal(string nombreCampo)
		{
			bool flag = this._lector != null && DBNull.Value != this._lector[nombreCampo];
			decimal result;
			if (flag)
			{
				decimal resultado = Convert.ToDecimal(this._lector[nombreCampo].ToString());
				result = resultado;
			}
			else
			{
				result = 0m;
			}
			return result;
		}

		/// <summary>
		/// Retorna el valor de un campo de un registro convertido en double.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Para que este método devuelva un valor correcto, previamente se debe haber invocado el método HayRegistros() y haber obtenido el valor true.</remarks>
		// Token: 0x0600095C RID: 2396 RVA: 0x0001F2C8 File Offset: 0x0001D4C8
		public double CampoDouble(string nombreCampo)
		{
			bool flag = this._lector != null && DBNull.Value != this._lector[nombreCampo];
			double result;
			if (flag)
			{
				double resultado = Convert.ToDouble(this._lector[nombreCampo].ToString());
				result = resultado;
			}
			else
			{
				result = 0.0;
			}
			return result;
		}

		/// <summary>
		/// Retorna el valor de un campo de un registro convertido en string.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Para que este método devuelva un valor correcto, previamente se debe haber invocado el método HayRegistros() y haber obtenido el valor true.</remarks>
		// Token: 0x0600095D RID: 2397 RVA: 0x0001F324 File Offset: 0x0001D524
		public string CampoMemo(string nombreCampo)
		{
			bool flag = this._lector != null;
			string result;
			if (flag)
			{
				result = Encoding.Default.GetString((byte[])this._lector[nombreCampo]);
			}
			else
			{
				result = "";
			}
			return result;
		}

		/// <summary>
		/// Intenta obtener el valor de un campo de un registro.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Para que este método devuelva un valor correcto, previamente se debe haber invocado el método HayRegistros() y haber obtenido el valor true.</remarks>
		// Token: 0x0600095E RID: 2398 RVA: 0x0001F368 File Offset: 0x0001D568
		public string NombreCampo(int nroCampo)
		{
			bool flag = this._lector != null;
			string result;
			if (flag)
			{
				result = this._lector.GetName(nroCampo);
			}
			else
			{
				result = null;
			}
			return result;
		}

		/// <summary>
		/// Ejecuta la consulta y devuelve un número escalar.
		/// </summary>
		/// <returns></returns>
		// Token: 0x0600095F RID: 2399 RVA: 0x0001F398 File Offset: 0x0001D598
		public long EjecutarEscalar(bool cerrarConexion = true)
		{
			bool flag = this._comando.Connection.State == ConnectionState.Closed;
			if (flag)
			{
				this._comando.Connection.Open();
			}
			long Escalar;
			try
			{
				Escalar = Convert.ToInt64(this._comando.ExecuteScalar());
			}
			catch (Exception ex)
			{
				this._comando.Connection.Close();
				throw ex;
			}
			if (cerrarConexion)
			{
				this._comando.Connection.Close();
			}
			return Escalar;
		}

		/// <summary>
		/// Ejecuta una consulta y devuelve un objeto OdbcDataReader.
		/// </summary>
		/// <returns></returns>
		// Token: 0x06000960 RID: 2400 RVA: 0x0001F42C File Offset: 0x0001D62C
		public OdbcDataReader EjecutarLector()
		{
			bool flag = this._comando.Connection.State != ConnectionState.Open;
			if (flag)
			{
				this._comando.Connection.Open();
			}
			OdbcDataReader result;
			try
			{
				result = this._comando.ExecuteReader();
			}
			catch (Exception ex)
			{
				this._comando.Connection.Close();
				throw ex;
			}
			return result;
		}

		/// <summary>
		/// Consulta en el esquema de la base de datos si existe una tabla.
		/// </summary>
		/// <param name="nombreTabla"></param>
		/// <returns></returns>
		// Token: 0x06000961 RID: 2401 RVA: 0x0001F4A0 File Offset: 0x0001D6A0
		public static bool ExisteTabla(string nombreTabla, string baseDeDato)
		{
			bool Existe = false;
			bool flag = !string.IsNullOrEmpty(nombreTabla) && !string.IsNullOrEmpty(baseDeDato);
			if (flag)
			{
				ComandoDB2 DB2 = new ComandoDB2("SELECT 1 FROM SYSIBM.SYSTABLES WHERE NAME = '" + nombreTabla + "' AND TYPE = 'T'", baseDeDato);
				try
				{
					Existe = DB2.HayRegistros(true);
					DB2.Cerrar();
				}
				catch (Exception ex)
				{
					DB2.Cerrar();
					throw ex;
				}
			}
			return Existe;
		}

		/// <summary>
		/// Intenta leer un registro de la consulta. Si el registro se leyó correctamente el valor es true; en caso contrario, si no se pudo leer el registro, el valor es false. 
		/// </summary>
		/// <returns></returns>
		/// <remarks>Los valores de los campos se recuperan con el metodo Campo().</remarks>
		// Token: 0x06000962 RID: 2402 RVA: 0x0001F518 File Offset: 0x0001D718
		public bool HayRegistros(bool cerrarConexion = true)
		{
			bool flag = this._comando.Connection.State == ConnectionState.Closed;
			if (flag)
			{
				this._comando.Connection.Open();
				this._lector = this._comando.ExecuteReader();
			}
			bool Hay;
			try
			{
				bool flag2 = this._lector == null;
				if (flag2)
				{
					this._lector = this._comando.ExecuteReader();
				}
				Hay = this._lector.Read();
			}
			catch (Exception ex)
			{
				this.CerrarConexion();
				throw ex;
			}
			bool flag3 = !Hay && cerrarConexion;
			if (flag3)
			{
				this.CerrarConexion();
			}
			return Hay;
		}

		/// <summary>
		/// Obtiene un objeto que reprecenta una conexión a la base de datos de DB2.
		/// </summary>
		/// <param name="Nombre">Alias que tiene la conexión.</param>
		/// <returns></returns>
		// Token: 0x06000963 RID: 2403 RVA: 0x0001F5C8 File Offset: 0x0001D7C8
		public OdbcConnection ObtenerConexion(string nombreDeConexion = "DB2Connection")
		{
			return new OdbcConnection(nombreDeConexion);

			//return new OdbcConnection("Driver={IBM DB2 ODBC DRIVER};Database=TRIBUTOS;Hostname=133.123.120.120;Port=50000; Protocol=TCPIP;Uid=db2admin;Pwd=db2admin;");
			//string CadenaConexion = "";
			//bool flag = ConfigurationManager.ConnectionStrings[nombreDeConexion] == null;
			//if (flag)
			//{
			//	bool flag2 = Config.ObtenerClave(nombreDeConexion).Length > 0;
			//	if (flag2)
			//	{
			//		//bool cadenasDeConexionesEncriptadas = Config.CadenasDeConexionesEncriptadas;
			//		//if (cadenasDeConexionesEncriptadas)
			//		//{
			//		//	CadenaConexion = Rutinas.DesencriptarTexto(Config.ObtenerClave(nombreDeConexion));
			//		//}
			//		//else
			//		{
			//			CadenaConexion = Config.ObtenerClave(nombreDeConexion);
			//		}
			//	}
			//}
			//else
			//{
			//	CadenaConexion = ConfigurationManager.ConnectionStrings[nombreDeConexion].ConnectionString;
			//}

			//bool flag3 = CadenaConexion != "";
			//if (flag3)
			//{
			//	string ProcesosAControlar = Config.ObtenerClave("SIS_ConrolarProcesos_" + nombreDeConexion);
			//	bool flag4 = ProcesosAControlar.Length > 0;
			//	if (flag4)
			//	{
			//		int[] Procesos = Array.ConvertAll<string, int>(ProcesosAControlar.Split(new char[]
			//		{
			//			','
			//		}), new Converter<string, int>(int.Parse));
			//		bool flag5 = BaseDeDatos.ProcesosEnEjecucion(Procesos);
			//		if (flag5)
			//		{
			//			throw new Exception("- Proceso de emisión, base de dato en uso");
			//		}
			//	}
			//	try
			//	{
			//		return new OdbcConnection(CadenaConexion);
			//	}
			//	catch (Exception ex)
			//	{
			//		throw new Exception(string.Concat(new string[]
			//		{
			//			"ERROR: ",
			//			CadenaConexion,
			//			". ",
			//			ex.Message,
			//			". INNER: ",
			//			(ex.InnerException != null) ? ex.InnerException.Message : ""
			//		}));
			//	}
			//}
			//throw new Exception("- No existe la cadena de conexión con el nombre \"" + nombreDeConexion + "\" en el archivo de configuración.");
		}

		/// <summary>
		/// Obtiene los datos en un dataset;
		/// </summary>
		/// <returns></returns>
		// Token: 0x06000964 RID: 2404 RVA: 0x0001F740 File Offset: 0x0001D940
		public DataSet ObtenerDataSet()
		{
			DataSet DS = new DataSet();
			OdbcDataAdapter DA = new OdbcDataAdapter(this._comando);
			DA.Fill(DS);
			DA.Dispose();
			return DS;
		}

		/// <summary>
		/// Obtiene los datos en un dataset;
		/// </summary>
		/// <returns></returns>
		// Token: 0x06000964 RID: 2404 RVA: 0x0001F740 File Offset: 0x0001D940
		public DataSet ObtenerSchema()
		{
			DataSet DS = new DataSet();
			OdbcDataAdapter DA = new OdbcDataAdapter(this._comando);
			DA.FillSchema(DS, SchemaType.Source);
			DA.Dispose();
			return DS;
		}

		/// <summary>
		/// Devuelve el objeto OdbcDataReader que está siendo utilizado.
		/// </summary>
		/// <returns></returns>
		// Token: 0x06000965 RID: 2405 RVA: 0x0001F79C File Offset: 0x0001D99C
		public OdbcDataReader ObtenerLector()
		{
			return this._lector;
		}

		/// <summary>
		/// Devuelve una vista de datos.
		/// </summary>
		/// <param name="Consulta"></param>
		/// <param name="TamañoDePagina"></param>
		/// <param name="IndiceActual"></param>
		/// <param name="EsPostBack"></param>
		/// <returns></returns>
		// Token: 0x06000966 RID: 2406 RVA: 0x0001F7B4 File Offset: 0x0001D9B4
		public DataView ObtenerVistaDeDatos(string Consulta, int TamañoDePagina, int IndiceActual, bool EsPostBack)
		{
			DataView Obj;
			try
			{
				this._comando = new OdbcCommand();
				this._conexionDB2 = this.ObtenerConexion("DB2Connection");
				this._conexionDB2.Open();
				this._comando.Connection = this._conexionDB2;
				this._comando.CommandType = CommandType.Text;
				this._comando.CommandText = Consulta;
				DataSet miDS = new DataSet();
				OdbcDataAdapter miDataAdapter = new OdbcDataAdapter(this._comando);
				miDataAdapter.SelectCommand = this._comando;
				bool flag = !EsPostBack;
				if (flag)
				{
					miDataAdapter.Fill(miDS);
					this._cantidadRegistros = miDS.Tables[0].Rows.Count;
					miDS = new DataSet();
				}
				miDataAdapter.Fill(miDS, IndiceActual, TamañoDePagina, "0");
				miDataAdapter.SelectCommand.Connection.Close();
				Obj = miDS.Tables["0"].DefaultView;
			}
			catch (Exception ex)
			{
				Obj = null;
			}
			finally
			{
				this.Cerrar();
			}
			return Obj;
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x0001F8E0 File Offset: 0x0001DAE0
		public void LlenarDataSet(ref DataSet ds)
		{
			new OdbcDataAdapter
			{
				SelectCommand = this._comando
			}.Fill(ds);
		}

		/// <summary>
		/// Actualiza el lector con el resultado de la consulta actual.
		/// </summary>
		// Token: 0x06000968 RID: 2408 RVA: 0x0001F90C File Offset: 0x0001DB0C
		public void VolverAEjecutarConsulta()
		{
			bool flag = this._comando.Connection.State == ConnectionState.Closed;
			if (flag)
			{
				this._comando.Connection.Open();
			}
			bool flag2 = this._lector != null;
			if (flag2)
			{
				this._lector.Close();
			}
			this._lector = this._comando.ExecuteReader();
		}

		// Token: 0x0400039D RID: 925
		private OdbcConnection _conexionDB2;

		// Token: 0x0400039E RID: 926
		private OdbcCommand _comando;

		// Token: 0x0400039F RID: 927
		private OdbcParameter _parametro;

		// Token: 0x040003A0 RID: 928
		private DataView _vistaDeDatos;

		// Token: 0x040003A1 RID: 929
		private int _cantidadRegistros;

		// Token: 0x040003A2 RID: 930
		private OdbcDataReader _lector;

		// Token: 0x040003A3 RID: 931
		private OdbcTransaction _transaccion;
	}
}

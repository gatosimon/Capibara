using System;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Reflection;
using System.Text;

namespace Capibara
{
	public class ComandoDB2
	{
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
        public OdbcParameterCollection Parametros
        {
            get
            {
                return this._comando.Parameters;
            }
        }

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

        /// <summary>
        /// Crea una nueva instancia de la clase.
        /// </summary>
        /// <param name="Consulta"></param>
        /// <param name="Conexion"></param>
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
        public ComandoDB2(string Consulta, int TamañoDePagina, int IndiceActual, bool EsPostBack)
        {
            this._vistaDeDatos = this.ObtenerVistaDeDatos(Consulta, TamañoDePagina, IndiceActual, EsPostBack);
        }

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
        public OdbcConnection ObtenerConexion(string nombreDeConexion = "DB2Connection")
        {
            return new OdbcConnection(nombreDeConexion);
        }

        /// <summary>
        /// Obtiene los datos en un dataset;
        /// </summary>
        /// <returns></returns>
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

        private OdbcConnection _conexionDB2;

        private OdbcCommand _comando;

        private OdbcParameter _parametro;

        private DataView _vistaDeDatos;

        private int _cantidadRegistros;

        private OdbcDataReader _lector;

        private OdbcTransaction _transaccion;
    }
}

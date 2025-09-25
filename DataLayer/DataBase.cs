using System;
using System.Data;
using System.Collections.Generic;

namespace DataLayer
{
    /// <summary>
    /// Controlador de la conexion a una base de datos
    /// </summary>
    public class DataBase
    {
        private const string ANSI = "yyyyMMdd HH:mm:ss";

        List<IDataReader> cursoresAbiertos = new List<IDataReader>();



        /// <summary>
        /// Constructor
        /// </summary>
        public DataBase()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connection">conexion a la base de datos</param>
        public DataBase(IDbConnection connection)
        {
            Connection = connection;
        }

        /// <summary>
        /// Conexion a la base de datos
        /// </summary>
        internal IDbConnection _Connection;
        /// <summary>
        /// Conexion a la base de datos
        /// </summary>
        public IDbConnection Connection
        {
            get { return _Connection; }
            set { _Connection = value; }
        }

        /// <summary>
        /// DataAdapter para sincronizacion de DataSets
        /// </summary>
        internal IDbDataAdapter _DataAdapter;
        /// <summary>
        /// DataAdapter para sincronizacion de DataSets
        /// </summary>
        public IDbDataAdapter DataAdapter
        {
            get { return _DataAdapter; }
            set { _DataAdapter = value; }
        }

        /// <summary>
        /// Abre la conexion a la base de datos
        /// </summary>
        public void OpenConnection()
        {
            Connection.Open();
        }

        /// <summary>
        /// Cierra la conexion a la base de datos
        /// </summary>
        public void CloseConnection()
        {
            Connection.Close();
        }

        /// <summary>
        /// Transaccion actual en la que se ejecutan los comandos
        /// </summary>
        IDbTransaction _Transaction;
        /// <summary>
        /// Transaccion actual en la que se ejecutan los comandos
        /// </summary>
        public IDbTransaction Transaction
        {
            get { return _Transaction; }
            set { _Transaction = value; }
        }

        /// <summary>
        /// Devuelve si se encuentra iniciada una transaccion o no
        /// </summary>
        public bool InTransaction
        {
            get
            {
                if (_Transaction != null)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Inicia una transaccion
        /// </summary>
        public bool BeginTransaction()
        {
            if (!InTransaction)
            {
                try
                {
                    _Transaction = Connection.BeginTransaction();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Finaliza una transaccion
        /// </summary>
        public bool CommitTransaction()
        {
            if (InTransaction)
            {
                foreach (IDataReader cursor in cursoresAbiertos)
                {
                    try
                    {
                        cursor.Close();
                    }
                    catch { }
                }
                _Transaction.Commit();
                _Transaction = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Cancela una transaccion
        /// </summary>
        public bool RollbackTransaction()
        {
            if (InTransaction)
            {
                foreach (IDataReader cursor in cursoresAbiertos)
                {
                    try
                    {
                        cursor.Close();
                    }
                    catch { }
                }
                _Transaction.Rollback();
                _Transaction = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Crea un objeto Parameter para ejecutar con los metodos Query, DataReader, NonQuery y Scalar
        /// </summary>
        /// <returns></returns>
        public IDbDataParameter Parameter()
        {
            IDbCommand CommandInterno = Connection.CreateCommand();
            IDbDataParameter Resultado = CommandInterno.CreateParameter();
            return Resultado;
        }

        /// <summary>
        /// Crea un objeto Parameter para ejecutar con los metodos Query, DataReader, NonQuery y Scalar
        /// </summary>
        /// <param name="name">nombre del parametro</param>
        /// <returns></returns>
        public IDbDataParameter Parameter(string name)
        {
            IDbDataParameter NuevoParameter = Parameter();
            NuevoParameter.ParameterName = name.ToLower();
            return NuevoParameter;
        }

        /// <summary>
        /// Crea un objeto Parameter para ejecutar con los metodos Query, DataReader, NonQuery y Scalar
        /// </summary>
        /// <param name="name">nombre del parametro</param>
        /// <param name="type">tipo de dato del parametro</param>
        /// <returns></returns>
        public IDbDataParameter Parameter(string name, DbType type)
        {
            IDbDataParameter NuevoParameter = Parameter(name.ToLower());
            NuevoParameter.DbType = type;
            return NuevoParameter;
        }

        /// <summary>
        /// Crea un objeto Parameter para ejecutar con los metodos Query, DataReader, NonQuery y Scalar
        /// </summary>
        /// <param name="name">nombre del parametro</param>
        /// <param name="type">tipo de dato del parametro</param>
        /// <param name="value">valor del parametro</param>
        /// <returns></returns>
        public IDbDataParameter Parameter(string name, DbType type, object value)
        {
            IDbDataParameter NuevoParameter = Parameter(name.ToLower(), type);
            NuevoParameter.Value = value;
            return NuevoParameter;
        }

        /// <summary>
        /// Crea un objeto Parameter para ejecutar con los metodos Query, DataReader, NonQuery y Scalar
        /// </summary>
        /// <param name="name">nombre del parametro</param>
        /// <param name="type">tipo de dato del parametro</param>
        /// <param name="value">valor del parametro</param>
        /// <param name="size">longitud del parametro</param>
        /// <returns></returns>
        public IDbDataParameter Parameter(string name, DbType type, int size, object value)
        {
            IDbDataParameter NuevoParameter = Parameter(name.ToLower(), type, value);
            NuevoParameter.Size = size;
            return NuevoParameter;
        }

        /// <summary>
        /// Crea un objeto Parameter para ejecutar con los metodos Query, DataReader, NonQuery y Scalar
        /// </summary>
        /// <param name="name">nombre del parametro</param>
        /// <param name="value">valor del parametro</param>
        /// <param name="size">longitud del parametro</param>
        /// <returns></returns>
        public IDbDataParameter Parameter(string name, int size, object value)
        {
            IDbDataParameter NuevoParameter = Parameter(name.ToLower());
            NuevoParameter.Value = value;
            NuevoParameter.Size = size;
            return NuevoParameter;
        }

        /// <summary>
        /// Crea un objeto Parameter para ejecutar con los metodos Query, DataReader, NonQuery y Scalar
        /// </summary>
        /// <param name="name">nombre del parametro</param>
        /// <param name="value">valor del parametro</param>
        /// <returns></returns>
        public IDbDataParameter Parameter(string name, object value)
        {
            IDbDataParameter NuevoParameter = Parameter(name.ToLower());
            NuevoParameter.Value = value;
            return NuevoParameter;
        }

        /// <summary>
        /// Crea un objeto Command asociado a la conexion actual
        /// </summary>
        /// <param name="sql">sentencia del command</param>
        /// <param name="parameters">parametros de la sentencia</param>
        /// <returns></returns>
        internal IDbCommand Command(string sql, params IDbDataParameter[] parameters)
        {
            IDbCommand NuevoCommand = Connection.CreateCommand();
            NuevoCommand.CommandText = sql;
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i].ParameterName = parameters[i].ParameterName.ToLower();
                NuevoCommand.Parameters.Add(parameters[i]);
            }
            return NuevoCommand;
        }

        /// <summary>
        /// Ejecuta una sentencia y devuelve la cantidad de filas afectadas
        /// </summary>
        /// <param name="sql">sentencia a ejecutar</param>
        /// <param name="parameters">parametros de la sentencia</param>
        public int NonQuery(string sql, params IDbDataParameter[] parameters)
        {
            IDbCommand CommandInterno = Connection.CreateCommand();
            if (InTransaction)
            {
                CommandInterno.Transaction = Transaction;
            }
            CommandInterno.CommandText = sql;
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i].ParameterName = parameters[i].ParameterName.ToLower();
                CommandInterno.Parameters.Add(parameters[i]);
            }
            return CommandInterno.ExecuteNonQuery();
        }

        /// <summary>
        /// Ejecuta una sentencia escalar y devuelve su resultado
        /// </summary>
        /// <param name="sql">sentencia a ejecutar</param>
        /// <param name="parameters">parametros de la sentencia</param>
        public object Scalar(string sql, params IDbDataParameter[] parameters)
        {
            IDbCommand CommandInterno = Connection.CreateCommand();
            if (InTransaction)
            {
                CommandInterno.Transaction = Transaction;
            }
            CommandInterno.CommandText = sql;
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i].ParameterName = parameters[i].ParameterName.ToLower();
                CommandInterno.Parameters.Add(parameters[i]);
            }
            return CommandInterno.ExecuteScalar();
        }

        /// <summary>
        /// Ejecuta una sentencia query y devuelve un DataReader con los resultados
        /// </summary>
        /// <param name="sql">sentencia a ejecutar</param>
        /// <param name="parameters">parametros de la sentencia</param>
        public IDataReader DataReader(string sql, params IDbDataParameter[] parameters)
        {
            IDbCommand CommandInterno = Connection.CreateCommand();
            if (InTransaction)
            {
                CommandInterno.Transaction = Transaction;
            }
            CommandInterno.CommandText = sql;
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i].ParameterName = parameters[i].ParameterName.ToLower();
                CommandInterno.Parameters.Add(parameters[i]);
            }

            IDataReader cursor = CommandInterno.ExecuteReader();
            cursoresAbiertos.Add(cursor);
            return cursor;
        }

        /// <summary>
        /// Ejecuta una sentencia query y la utiliza para rellenar un DataTable
        /// </summary>
        /// <param name="sql">sentencia a ejecutar</param>
        /// <param name="parameters">parametros de la sentencia</param>
        public DataTable DataTable(string sql, params IDbDataParameter[] parameters)
        {
            DataTable Tabla = new DataTable();
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i].ParameterName = parameters[i].ParameterName.ToLower();
            }
            return Query(sql, Tabla, parameters);
        }

        /// <summary>
        /// Ejecuta una sentencia query y la utiliza para rellenar un DataTable
        /// </summary>
        /// <param name="sql">sentencia a ejecutar</param>
        /// <param name="dataTable">DataTable a llenar con los valores</param>
        /// <param name="parameters">parametros de la sentencia</param>
        public DataTable Query(string sql, DataTable dataTable, params IDbDataParameter[] parameters)
        {
            if (Connection.State == ConnectionState.Open)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    parameters[i].ParameterName = parameters[i].ParameterName.ToLower();
                }
                dataTable.Load(DataReader(sql, parameters));
                return dataTable;
            }
            else
            {
                throw new Exception("La conexión  a la base de datos no se encuentra abierta.");
            }
        }

        /// <summary>
        /// Actualiza la base de datos a partir de un DataSet, utilizando el DataAdapter
        /// </summary>
        /// <param name="dataSet">DataSet con los datos modificados</param>
        public void DataAdapterUpdate(DataSet dataSet)
        {
            if (DataAdapter != null)
            {
                DataAdapter.Update(dataSet);
            }
            else
            {
                throw new Exception("La propiedad DataAdapter no se encuentra inicializada.");
            }
        }

        /// <summary>
        /// Actualiza DataSet a partir de la base de datos, utilizando el DataAdapter
        /// </summary>
        /// <param name="dataSet">DataSet a actualizar</param>
        public void DataAdapterFill(DataSet dataSet)
        {
            if (DataAdapter != null)
            {
                DataAdapter.Fill(dataSet);
            }
            else
            {
                throw new Exception("La propiedad DataAdapter no se encuentra inicializada.");
            }
        }

        /// <summary>
        /// Carga un DataSet a partir de una consulta a la base de datos
        /// </summary>
        /// <param name="sql">sentencia a ejecutar</param>
        /// <param name="parameters">parametros de la sentencia</param>
        /// <returns></returns>
        public DataSet DataSet(string sql, params IDbDataParameter[] parameters)
        {
            DataSet ds = new DataSet();
            int i = 0;

            IDataReader reader = DataReader(sql, parameters);
            
            reader.Read();
            do
            {
                DataTable tabla = new DataTable("Tabla" + i);
                tabla.Load(reader);
                ds.Tables.Add(tabla);
                i++;
            }
            while (!reader.IsClosed && reader.NextResult()) ;

            return ds;
        }

        /// <summary>
        /// Devuelve el nombre de tabla de una clase marcada con TableAttributes
        /// </summary>
        /// <param name="objectType"> tipo de objeto del cual obtener el nombre de tabla</param>
        /// <returns></returns>
        static public string TableNameFromType(Type objectType)
        {
            string NombreTabla = "";
            object[] Atributos = objectType.GetCustomAttributes(true);

            foreach (object AtributoActual in Atributos)
            {
                if (AtributoActual.GetType() == typeof(TableAttributes))
                {
                    NombreTabla = (AtributoActual as TableAttributes).TableName;
                }
            }

            return NombreTabla;
        }

        /// <summary>
        /// Arma la sentencia insert con los valores de un objeto de una clase marcada con TableAttributes y con propiedades marcadas con FieldAttributes, y la ejecuta en la base de datos devolviendo el numero de filas afectadas
        /// </summary>
        /// <param name="obj">objeto con los valores del insert</param>
        /// <returns></returns>
        public int InsertFromObject(object obj)
        {
            Type TipoClase = obj.GetType();
            System.Reflection.PropertyInfo[] Propiedades = TipoClase.GetProperties();
            List<string> Campos = new List<string>();
            List<string> Valores = new List<string>();
            List<IDbDataParameter> Parametros = new List<IDbDataParameter>();
            string NombreTabla = TableNameFromType(TipoClase);
            IDbDataParameter ParametroAgregar;

            string CampoAgregar = "";
            foreach (System.Reflection.PropertyInfo PropiedadActual in Propiedades)
            {
                object[] Atributos = PropiedadActual.GetCustomAttributes(true);
                foreach (object AtributoActual in Atributos)
                {
                    if (AtributoActual.GetType() == typeof(FieldAttributes))
                    {
                        FieldAttributes atributoCampo = (AtributoActual as FieldAttributes);
                        if (atributoCampo.ColumnName.Length > 0)
                        {
                            CampoAgregar = atributoCampo.ColumnName;
                        }
                        else
                        {
                            CampoAgregar = PropiedadActual.Name;
                        }
                        Campos.Add(CampoAgregar);
                        Valores.Add("@" + CampoAgregar);
                        ParametroAgregar = ParameterFromFieldAttributes("@" + CampoAgregar.ToLower(), atributoCampo, PropiedadActual.GetValue(obj, null));
                        Parametros.Add(ParametroAgregar);
                    }
                }
            }

            return NonQuery(string.Format("INSERT INTO {0} ({1}) VALUES ({2})", NombreTabla, string.Join(", ", Campos.ToArray()), string.Join(", ", Valores.ToArray())), Parametros.ToArray());
        }

        /// <summary>
        /// Crea un objeto Parameter para ejecutar con los metodos Query, DataReader, NonQuery y Scalar, desde los atributos de un campo
        /// </summary>
        /// <param name="name">nombre del parametro</param>
        /// <param name="fieldAttributes">atributos del campo</param>
        /// <returns></returns>
        internal IDbDataParameter ParameterFromFieldAttributes(string name, FieldAttributes fieldAttributes)
        {
            IDbDataParameter ParametroDevolver;
            ParametroDevolver = Parameter(name.ToLower(), fieldAttributes.dbType);
            if (fieldAttributes.Precision > 0)
            {
                ParametroDevolver.Precision = fieldAttributes.Precision;
            }
            if (fieldAttributes.Size > 0)
            {
                ParametroDevolver.Size = fieldAttributes.Size;
            }
            if (fieldAttributes.Scale > 0)
            {
                ParametroDevolver.Scale = fieldAttributes.Scale;
            }
            return ParametroDevolver;
        }

        /// <summary>
        /// Crea un objeto Parameter para ejecutar con los metodos Query, DataReader, NonQuery y Scalar, desde los atributos de un campo
        /// </summary>
        /// <param name="name">nombre del parametro</param>
        /// <param name="fieldAttributes">atributos del campo</param>
        /// <param name="value">valor del parametro</param>
        /// <returns></returns>
        internal IDbDataParameter ParameterFromFieldAttributes(string name, FieldAttributes fieldAttributes, object value)
        {
            IDbDataParameter ParametroDevolver = ParameterFromFieldAttributes(name.ToLower(), fieldAttributes);
            ParametroDevolver.Value = value;
            return ParametroDevolver;
        }

        /// <summary>
        /// Crea un objeto Parameter para ejecutar con los metodos Query, DataReader, NonQuery y Scalar, desde una clase marcada con TableAttributes y con propiedades marcadas con FieldAttributes
        /// </summary>
        /// <param name="classType">clase con el campo del cual crear un parametro</param>
        /// <param name="fieldName">nombre del campo en la base de datos</param>
        /// <returns></returns>
        public IDbDataParameter ParameterFromField(Type classType, string fieldName)
        {
            IDbDataParameter ParametroDevolver;
            System.Reflection.PropertyInfo[] Propiedades = classType.GetProperties();

            string CampoAgregar = "";
            foreach (System.Reflection.PropertyInfo PropiedadActual in Propiedades)
            {
                object[] Atributos = PropiedadActual.GetCustomAttributes(true);
                foreach (object AtributoActual in Atributos)
                {
                    if (AtributoActual.GetType() == typeof(FieldAttributes))
                    {
                        FieldAttributes atributoCampo = (AtributoActual as FieldAttributes);
                        if (atributoCampo.ColumnName.Length > 0)
                        {
                            CampoAgregar = atributoCampo.ColumnName;
                        }
                        else
                        {
                            CampoAgregar = PropiedadActual.Name;
                        }
                        if (CampoAgregar == fieldName)
                        {
                            ParametroDevolver = ParameterFromFieldAttributes("@" + CampoAgregar.ToLower(), atributoCampo);
                            return ParametroDevolver;
                        }
                    }
                }
            }

            ParametroDevolver = Parameter("@" + fieldName.ToLower());
            return ParametroDevolver;
        }

        /// <summary>
        /// Crea un objeto Parameter para ejecutar con los metodos Query, DataReader, NonQuery y Scalar, desde una clase marcada con TableAttributes y con propiedades marcadas con FieldAttributes
        /// </summary>
        /// <param name="classType">clase con el campo del cual crear un parametro</param>
        /// <param name="fieldName">nombre del campo en la base de datos</param>
        /// <param name="value">valor del parametro</param>
        /// <returns></returns>
        public IDbDataParameter ParameterFromField(Type classType, string fieldName, object value)
        {
            IDbDataParameter ParametroDevolver;
            ParametroDevolver = ParameterFromField(classType, fieldName.ToLower());
            ParametroDevolver.Value = value;
            return ParametroDevolver;
        }

        /// <summary>
        /// Obtiene los campos de una clase marcada con TableAttributes y con propiedades marcadas con FieldAttributes
        /// </summary>
        /// <param name="classType">clase con el campo del cual crear un parametro</param>
        /// <returns></returns>
        static public FieldAttributes[] FieldsFromType(Type classType)
        {
            List<FieldAttributes> Campos = new List<FieldAttributes>();
            System.Reflection.PropertyInfo[] Propiedades = classType.GetProperties();

            foreach (System.Reflection.PropertyInfo PropiedadActual in Propiedades)
            {
                object[] Atributos = PropiedadActual.GetCustomAttributes(true);
                foreach (object AtributoActual in Atributos)
                {
                    if (AtributoActual.GetType() == typeof(FieldAttributes))
                    {
                        FieldAttributes atributoCampo = (AtributoActual as FieldAttributes);
                        if (atributoCampo.ColumnName.Length == 0)
                        {
                            atributoCampo.ColumnName = PropiedadActual.Name;
                        }
                        Campos.Add(atributoCampo);
                    }
                }
            }
            return Campos.ToArray();
        }

        /// <summary>
        /// Persiste un objeto de una clase marcada con TableAttributes y con propiedades marcadas con FieldAttributes
        /// </summary>
        /// <param name="obj">objeto con los valores del update</param>
        /// <returns>devuelve true si el objeto ya se encontraba presente en la base de datos, y false si se agrego como nuevo</returns>
        public bool SaveObject(object obj)
        {
            Type TipoClase = obj.GetType();
            System.Reflection.PropertyInfo[] Propiedades = TipoClase.GetProperties();
            List<string> CamposClave = new List<string>();
            List<IDbDataParameter> Parametros = new List<IDbDataParameter>();
            string NombreTabla = TableNameFromType(TipoClase);
            IDbDataParameter ParametroAgregar;

            string CampoAgregar = "";
            foreach (System.Reflection.PropertyInfo PropiedadActual in Propiedades)
            {
                object[] Atributos = PropiedadActual.GetCustomAttributes(true);
                foreach (object AtributoActual in Atributos)
                {
                    if (AtributoActual.GetType() == typeof(FieldAttributes))
                    {
                        FieldAttributes atributoCampo = (AtributoActual as FieldAttributes);
                        if (atributoCampo.Key)
                        {
                            if (atributoCampo.ColumnName.Length > 0)
                            {
                                CampoAgregar = atributoCampo.ColumnName;
                            }
                            else
                            {
                                CampoAgregar = PropiedadActual.Name;
                            }

                            CamposClave.Add(CampoAgregar + " = @" + CampoAgregar);

                            ParametroAgregar = ParameterFromFieldAttributes("@" + CampoAgregar.ToLower(), atributoCampo, PropiedadActual.GetValue(obj, null));
                            Parametros.Add(ParametroAgregar);
                        }
                    }
                }
            }

            if (((int)Scalar(string.Format("SELECT COUNT(*) FROM {0} WHERE {1}", NombreTabla, string.Join(" AND ", CamposClave.ToArray())), Parametros.ToArray())) > 0)
            {
                UpdateFromObject(obj);
                return true;
            }
            else
            {
                InsertFromObject(obj);
                return false;
            }
        }

        /// <summary>
        /// Actualiza los valores de un objeto marcado con FieldAttributes y TableAttributes, con los valores almacenados en la base de datos
        /// </summary>
        /// <param name="obj">objeto a actualizar</param>
        /// <returns>devuelve verdadero si se pudieron leer datos en la base de datos, y falso si no se pudo</returns>
        public bool UpdateObject(object obj)
        {
            Type objectType = obj.GetType();
            System.Reflection.PropertyInfo[] Propiedades = objectType.GetProperties();
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            List<IDbDataParameter> list3 = new List<IDbDataParameter>();
            string str = TableNameFromType(objectType);
            string columnName = "";
            foreach (System.Reflection.PropertyInfo info in Propiedades)
            {
                object[] Atributos = info.GetCustomAttributes(true);
                foreach (object obj2 in Atributos)
                {
                    if (obj2.GetType() == typeof(FieldAttributes))
                    {
                        FieldAttributes atributoCampo = (obj2 as FieldAttributes);
                        if (atributoCampo.ColumnName.Length > 0)
                        {
                            columnName = atributoCampo.ColumnName;
                        }
                        else
                        {
                            columnName = info.Name;
                        }
                        if (atributoCampo.Key)
                        {
                            list.Add(columnName + " = @" + columnName);
                            IDbDataParameter item = ParameterFromFieldAttributes("@" + columnName.ToLower(), atributoCampo, info.GetValue(obj, null));
                            if (atributoCampo.Precision > 0)
                            {
                                item.Precision = atributoCampo.Precision;
                            }
                            if (atributoCampo.Size > 0)
                            {
                                item.Size = atributoCampo.Size;
                            }
                            if (atributoCampo.Scale > 0)
                            {
                                item.Scale = atributoCampo.Scale;
                            }
                            list3.Add(item);
                        }
                        else
                        {
                            list2.Add(columnName.ToUpper());
                        }
                    }
                }
            }

            IDataReader reader = DataReader(string.Format("SELECT {0} FROM {1} WHERE {2}", string.Join(", ", list2.ToArray()), str, string.Join(" AND ", list.ToArray())), list3.ToArray());
            if (reader.Read())
            {
                foreach (System.Reflection.PropertyInfo info2 in Propiedades)
                {
                    object[] Atributos = info2.GetCustomAttributes(true);
                    foreach (object obj3 in Atributos)
                    {
                        if (obj3.GetType() == typeof(FieldAttributes))
                        {
                            FieldAttributes atributoCampo = (obj3 as FieldAttributes);
                            string name;
                            if (atributoCampo.ColumnName.Length > 0)
                            {
                                name = atributoCampo.ColumnName;
                            }
                            else
                            {
                                name = info2.Name;
                            }
                            try
                            {
                                object obj4 = reader.GetValue(list2.IndexOf(name.ToUpper()));
                                info2.SetValue(obj, obj4, null);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                reader.Close();
                return true;
            }
            reader.Close();
            return false;
        }

        /// <summary>
        /// Arma la sentencia update con los valores de un objeto de una clase marcada con TableAttributes y con propiedades marcadas con FieldAttributes, y la ejecuta en la base de datos devolviendo el numero de filas afectadas
        /// </summary>
        /// <param name="obj">objeto con los valores del update</param>
        /// <returns></returns>
        public int UpdateFromObject(object obj)
        {
            Type TipoClase = obj.GetType();
            System.Reflection.PropertyInfo[] Propiedades = TipoClase.GetProperties();
            List<string> CamposClave = new List<string>();
            List<string> CamposNoClave = new List<string>();
            List<IDbDataParameter> Parametros = new List<IDbDataParameter>();
            string NombreTabla = TableNameFromType(TipoClase);
            IDbDataParameter ParametroAgregar;

            string CampoAgregar = "";
            foreach (System.Reflection.PropertyInfo PropiedadActual in Propiedades)
            {
                object[] Atributos = PropiedadActual.GetCustomAttributes(true);
                foreach (object AtributoActual in Atributos)
                {
                    if (AtributoActual.GetType() == typeof(FieldAttributes))
                    {
                        FieldAttributes atributoCampo = (AtributoActual as FieldAttributes);
                        if (atributoCampo.ColumnName.Length > 0)
                        {
                            CampoAgregar = atributoCampo.ColumnName;
                        }
                        else
                        {
                            CampoAgregar = PropiedadActual.Name;
                        }
                        if (atributoCampo.Key)
                        {
                            CamposClave.Add(CampoAgregar + " = @" + CampoAgregar);
                        }
                        else
                        {
                            CamposNoClave.Add(CampoAgregar + " = @" + CampoAgregar);
                        }

                        ParametroAgregar = ParameterFromFieldAttributes("@" + CampoAgregar.ToLower(), atributoCampo, PropiedadActual.GetValue(obj, null));
                        Parametros.Add(ParametroAgregar);
                    }
                }
            }

            return NonQuery(string.Format("UPDATE {0} SET {1} WHERE {2}", NombreTabla, string.Join(", ", CamposNoClave.ToArray()), string.Join(" AND ", CamposClave.ToArray())), Parametros.ToArray());
        }

        /// <summary>
        /// Arma la sentencia delete con los valores de un objeto de una clase marcada con TableAttributes y con propiedades marcadas con FieldAttributes, la ejecuta en la base de datos y devuelve el numero de filas afectadas
        /// </summary>
        /// <param name="obj">objeto con los valores del delete</param>
        /// <returns></returns>
        public int DeleteFromObject(object obj)
        {
            Type TipoClase = obj.GetType();
            System.Reflection.PropertyInfo[] Propiedades = TipoClase.GetProperties();
            List<string> Campos = new List<string>();
            List<IDbDataParameter> Parametros = new List<IDbDataParameter>();
            string NombreTabla = TableNameFromType(TipoClase);
            IDbDataParameter ParametroAgregar;

            string CampoAgregar = "";
            foreach (System.Reflection.PropertyInfo PropiedadActual in Propiedades)
            {
                object[] Atributos = PropiedadActual.GetCustomAttributes(true);
                foreach (object AtributoActual in Atributos)
                {
                    if (AtributoActual.GetType() == typeof(FieldAttributes))
                    {
                        FieldAttributes atributoCampo = (AtributoActual as FieldAttributes);
                        if (atributoCampo.ColumnName.Length > 0)
                        {
                            CampoAgregar = atributoCampo.ColumnName;
                        }
                        else
                        {
                            CampoAgregar = PropiedadActual.Name;
                        }
                        if (atributoCampo.Key)
                        {
                            Campos.Add(CampoAgregar + " = @" + CampoAgregar);
                            ParametroAgregar = ParameterFromFieldAttributes("@" + CampoAgregar.ToLower(), atributoCampo, PropiedadActual.GetValue(obj, null));
                            Parametros.Add(ParametroAgregar);
                        }
                    }
                }
            }

            return NonQuery(string.Format("DELETE FROM {0} WHERE {1}", NombreTabla, string.Join(" AND ", Campos.ToArray())), Parametros.ToArray());
        }

        /// <summary>
        /// Actualiza los valores de un objeto marcado con FieldAttributes y TableAttributes, con los valores almacenados en la base de datos
        /// </summary>
        /// <param name="obj">objeto a actualizar</param>
        /// <returns>devuelve verdadero si se pudieron leer datos en la base de datos, y falso si no se pudo</returns>
        public bool ReadObject(object obj)
        {
            Type TipoClase = obj.GetType();
            System.Reflection.PropertyInfo[] Propiedades = TipoClase.GetProperties();
            List<string> CamposClave = new List<string>();
            List<string> CamposNoClave = new List<string>();
            List<IDbDataParameter> Parametros = new List<IDbDataParameter>();
            string NombreTabla = TableNameFromType(TipoClase);
            IDbDataParameter ParametroAgregar;

            string CampoAgregar = "";
            foreach (System.Reflection.PropertyInfo PropiedadActual in Propiedades)
            {
                object[] Atributos = PropiedadActual.GetCustomAttributes(true);
                foreach (object AtributoActual in Atributos)
                {
                    if (AtributoActual.GetType() == typeof(FieldAttributes))
                    {
                        FieldAttributes atributoCampo = (AtributoActual as FieldAttributes);
                        if (atributoCampo.ColumnName.Length > 0)
                        {
                            CampoAgregar = atributoCampo.ColumnName;
                        }
                        else
                        {
                            CampoAgregar = PropiedadActual.Name;
                        }
                        if (atributoCampo.Key)
                        {
                            CamposNoClave.Add(CampoAgregar.ToUpper());
                            CamposClave.Add(CampoAgregar + " = @" + CampoAgregar);
                            ParametroAgregar = Parameter("@" + CampoAgregar.ToLower(), atributoCampo.dbType, PropiedadActual.GetValue(obj, null));
                            if (atributoCampo.Precision > 0)
                            {
                                ParametroAgregar.Precision = atributoCampo.Precision;
                            }
                            if (atributoCampo.Size > 0)
                            {
                                ParametroAgregar.Size = atributoCampo.Size;
                            }
                            if (atributoCampo.Scale > 0)
                            {
                                ParametroAgregar.Scale = atributoCampo.Scale;
                            }
                            Parametros.Add(ParametroAgregar);
                        }
                        else
                        {
                            CamposNoClave.Add(CampoAgregar.ToUpper());
                        }
                    }
                }
            }

            IDataReader Resultado = DataReader(string.Format("SELECT {0} FROM {1} WHERE {2}", string.Join(", ", CamposNoClave.ToArray()), NombreTabla, string.Join(" AND ", CamposClave.ToArray())), Parametros.ToArray());

            if (Resultado.Read())
            {
                string NombreCampo;

                foreach (System.Reflection.PropertyInfo PropiedadActual in Propiedades)
                {
                    object[] Atributos = PropiedadActual.GetCustomAttributes(true);
                    foreach (object AtributoActual in Atributos)
                    {
                        if (AtributoActual.GetType() == typeof(FieldAttributes))
                        {
                            FieldAttributes atributoCampo = (AtributoActual as FieldAttributes);
                            if (atributoCampo.ColumnName.Length > 0)
                            {
                                NombreCampo = atributoCampo.ColumnName;
                            }
                            else
                            {
                                NombreCampo = PropiedadActual.Name;
                            }
                            try
                            {
                                object Valor = Resultado.GetValue(CamposNoClave.IndexOf(NombreCampo.ToUpper()));
                                PropiedadActual.SetValue(obj, Valor, null);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                Resultado.Close();
                return true;
            }
            else
            {
                Resultado.Close();
                return false;
            }
        }

        /// <summary>
        /// Actualiza los valores de un objeto marcado con FieldAttributes y TableAttributes, con los valores almacenados en una fila
        /// </summary>
        /// <param name="obj">objeto a actualizar</param>
        /// <param name="dataRow">fila con los valores</param>
        static public void DataRowToObject(DataRow dataRow, object obj)
        {
            Type TipoClase = obj.GetType();

            string NombreCampo;

            System.Reflection.PropertyInfo[] Propiedades = TipoClase.GetProperties();
            Type TipoCampo = typeof(FieldAttributes);

            foreach (System.Reflection.PropertyInfo PropiedadActual in Propiedades)
            {
                object[] Atributos = PropiedadActual.GetCustomAttributes(true);
                foreach (object AtributoActual in Atributos)
                {
                    if (AtributoActual.GetType() == TipoCampo)
                    {
                        FieldAttributes atributo = (AtributoActual as FieldAttributes);
                        if (atributo.ColumnName.Length > 0)
                        {
                            NombreCampo = atributo.ColumnName;
                        }
                        else
                        {
                            NombreCampo = PropiedadActual.Name;
                        }
                        try
                        {
                            object Valor = dataRow[NombreCampo];
                            switch (atributo.dbType)
                            {
                                case DbType.Boolean:
                                    PropiedadActual.SetValue(obj, Convert.ToBoolean(Valor), null);
                                    break;
                                case DbType.Byte:
                                    PropiedadActual.SetValue(obj, Convert.ToByte(Valor), null);
                                    break;
                                case DbType.Date:
                                case DbType.DateTime:
                                    PropiedadActual.SetValue(obj, DeterminarFecha(Valor.ToString()), null);
                                    break;
                                case DbType.Decimal:
                                    PropiedadActual.SetValue(obj, Convert.ToDecimal(Valor), null);
                                    break;
                                case DbType.Double:
                                    PropiedadActual.SetValue(obj, Convert.ToDouble(Valor), null);
                                    break;
                                case DbType.Int16:
                                    PropiedadActual.SetValue(obj, Convert.ToInt16(Valor), null);
                                    break;
                                case DbType.Int32:
                                    PropiedadActual.SetValue(obj, Convert.ToInt32(Valor), null);
                                    break;
                                case DbType.Int64:
                                    PropiedadActual.SetValue(obj, Convert.ToInt64(Valor), null);
                                    break;
                                case DbType.SByte:
                                    PropiedadActual.SetValue(obj, Convert.ToSByte(Valor), null);
                                    break;
                                case DbType.Single:
                                    PropiedadActual.SetValue(obj, Convert.ToSingle(Valor), null);
                                    break;
                                case DbType.String:
                                    PropiedadActual.SetValue(obj, Convert.ToString(Valor), null);
                                    break;
                                case DbType.Time:
                                    PropiedadActual.SetValue(obj, Convert.ToDateTime(Valor).TimeOfDay, null);
                                    break;
                                case DbType.UInt16:
                                    PropiedadActual.SetValue(obj, Convert.ToUInt16(Valor), null);
                                    break;
                                case DbType.UInt32:
                                    PropiedadActual.SetValue(obj, Convert.ToUInt32(Valor), null);
                                    break;
                                case DbType.UInt64:
                                    PropiedadActual.SetValue(obj, Convert.ToUInt64(Valor), null);
                                    break;
                                default:
                                    PropiedadActual.SetValue(obj, Valor, null);
                                    break;
                            }
                        }
                        catch (Exception err)
                        {
                            Log.Mensaje("*** Error al querer insertar el valor '" + dataRow[NombreCampo] + "' en la propiedad " + NombreCampo + " del tipo: " + atributo.dbType.ToString() + " ***");
                            Log.Error(err);
                        }
                    }
                }
            }
        }

        private static DateTime DeterminarFecha(string fecha)
        {
            DateTime resultado;
            if (fecha.Length == 17)
            {
                try
                {
                    resultado = DateTime.ParseExact(fecha, ANSI, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
                }
                catch
                {
                    resultado = Convert.ToDateTime(fecha);
                }
            }
            else
            {
                resultado = Convert.ToDateTime(fecha);
            }
            return resultado;
        }

        /// <summary>
        /// Rellena un datarow con los valores de un objeto de una clase marcada con TableAttributes y con propiedades marcadas con FieldAttributes
        /// </summary>
        /// <param name="obj">objeto con los valores</param>
        /// <param name="dataRow">fila a rellenar</param>
        /// <param name="formatoFechasANSI">indica si utiliza o no el formato de fechas ANSI</param>
        /// <returns></returns>
        static public void ObjectToDataRow(object obj, DataRow dataRow, bool formatoFechasANSI)
        {
            Type TipoClase = obj.GetType();
            System.Reflection.PropertyInfo[] Propiedades = TipoClase.GetProperties();

            string NombreCampo = "";
            foreach (System.Reflection.PropertyInfo PropiedadActual in Propiedades)
            {
                object[] Atributos = PropiedadActual.GetCustomAttributes(true);
                foreach (object AtributoActual in Atributos)
                {
                    if (AtributoActual.GetType() == typeof(FieldAttributes))
                    {
                        FieldAttributes atributoCampo = (AtributoActual as FieldAttributes);
                        if (atributoCampo.ColumnName.Length > 0)
                        {
                            NombreCampo = atributoCampo.ColumnName;
                        }
                        else
                        {
                            NombreCampo = PropiedadActual.Name;
                        }
                        try
                        {
                            if (formatoFechasANSI && PropiedadActual.PropertyType == typeof(DateTime))
                            {
                                try
                                {
                                    dataRow[NombreCampo] = Convert.ToDateTime(PropiedadActual.GetValue(obj, null)).ToString(ANSI);
                                }
                                catch
                                {
                                    dataRow[NombreCampo] = PropiedadActual.GetValue(obj, null);
                                }
                            }
                            else
                            {
                                dataRow[NombreCampo] = PropiedadActual.GetValue(obj, null);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Rellena un datarow con los valores de un objeto de una clase marcada con TableAttributes y con propiedades marcadas con FieldAttributes
        /// </summary>
        /// <param name="obj">objeto con los valores</param>
        /// <param name="dataRow">fila a rellenar</param>
        /// <returns></returns>
        static public void ObjectToDataRow(object obj, DataRow dataRow)
        {
            Type TipoClase = obj.GetType();
            System.Reflection.PropertyInfo[] Propiedades = TipoClase.GetProperties();

            string NombreCampo = "";
            foreach (System.Reflection.PropertyInfo PropiedadActual in Propiedades)
            {
                object[] Atributos = PropiedadActual.GetCustomAttributes(true);
                foreach (object AtributoActual in Atributos)
                {
                    if (AtributoActual.GetType() == typeof(FieldAttributes))
                    {
                        FieldAttributes atributoCampo = (AtributoActual as FieldAttributes);
                        if (atributoCampo.ColumnName.Length > 0)
                        {
                            NombreCampo = atributoCampo.ColumnName;
                        }
                        else
                        {
                            NombreCampo = PropiedadActual.Name;
                        }
                        try
                        {
                            dataRow[NombreCampo] = PropiedadActual.GetValue(obj, null);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
    }
}

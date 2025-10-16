using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Capibara
{
    public class Capas
    {
        #region DICCIONARIOS Y CONSTANTES

        public Dictionary<Type, string> TIPOS = new Dictionary<Type, string>()
        {
            {typeof(bool),      "bool"},
            {typeof(byte),      "byte"},
            {typeof(Byte[]),    "byte[]"},
            {typeof(char),      "char"},
            {typeof(char[]),    ERROR},
            {typeof(DateTime),  "DateTime"},
            {typeof(DBNull),    ERROR},
            {typeof(decimal),   "decimal"},
            {typeof(double),    "double"},
            {typeof(Guid),      "Guid"},
            {typeof(int),       "int"},			//                                                                      Int32
            {typeof(Int16),     "int" },        // "short"}, GONZA PIDIO ESTO SI REVIENTA OTRA VEZ ES ES SU CULPA		Int16s AHORA LO VOLVI A CAMBIAR, PERO ESTA VEZ A int PORQUE OOOOTRA VEZ LO PIDIO EL GONZA
            {typeof(Int64),     "long"},		//                                                                      Int64
            {typeof(Object),    ERROR},
            {typeof(SByte),     "double"},
            {typeof(Single),    "double"},
            {typeof(string),    "string"},
            {typeof(TimeSpan),  "TimeSpan"},
            {typeof(ushort),    "uint"},
            {typeof(uint),      "uint"},
            {typeof(ulong),     "uint"}
        };

        public Dictionary<string, string> Mapeo = new Dictionary<string, string>
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
            { "ulong",      "OdbcType.BigInt"},          // BIGINT
            { "ushort",     "OdbcType.BigInt"}          // BIGINT
        };

        // Diccionario de mapeo de tipos .NET a tipos DB2
        public Dictionary<Type, string> TIPOSDB2 = new Dictionary<Type, string>
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
        public Dictionary<Type, string> TIPOSSQL = new Dictionary<Type, string>
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

        public Dictionary<Type, string> PropiedadesTS = new Dictionary<Type, string>
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

        static public Dictionary<string, string> CamposAbm = new Dictionary<string, string>
        {
            { USUARIO_MAGIC_CLAVE,  USUARIO_MAGIC_VALOR },
            { CODIGO_BAJA_CLAVE,  CODIGO_BAJA_VALOR },
            { MOTIVO_BAJA_CLAVE, MOTIVO_BAJA_VALOR },
            { CODIGO_0_CLAVE, CODIGO_0_VALOR },
            { CADENA_VACIA_CLAVE, CADENA_VACIA_VALOR },
        };

        static public Dictionary<string, string> CamposAbmFechas = new Dictionary<string, string>
        {
            { FECHA_ACTUAL_CLAVE, FECHA_ACTUAL_VALOR},
            { FECHA_POR_DEFECTO_CLAVE, FECHA_POR_DEFECTO_VALOR },
        };

        static public Dictionary<string, string> CamposAbmHoras = new Dictionary<string, string>
        {
            { HORA_ACTUAL_CLAVE, HORA_ACTUAL_VALOR },
            { HORA_POR_DEFECTO_CLAVE, HORA_POR_DEFECTO_VALOR }
        };

        public const string ERROR = "ERROR";
        public const string CONTROLLER = "Controller";
        public const string CONTROLLERS = "Controllers";
        public const string DTO = "Dto";
        public const string MODEL = "Model";
        public const string REPOSITORIES = "Repositories";
        public const string REPOSITORIES_INTERFACE = "RepositoriesInterface";
        public const string SERVICE = "Service";
        public const string SERVICE_INTERFACE = "ServiceInterface";
        public const string TYPESCRIPT = "TypeScript";
        public const string MODELTABNAME = "MODELTABNAME";

        public const string USUARIO_MAGIC_CLAVE = "USUARIO MAGIC";
        public const string USUARIO_MAGIC_VALOR = "Config.UsuarioMagic;";
        public const string CODIGO_BAJA_CLAVE = "CÓDIGO BAJA";
        public const string CODIGO_BAJA_VALOR = "codigoBaja;";
        public const string MOTIVO_BAJA_CLAVE = "MOTIVO BAJA";
        public const string MOTIVO_BAJA_VALOR = "motivoBaja;";
        public const string CODIGO_0_CLAVE = "CÓDIGO 0";
        public const string CODIGO_0_VALOR = "0;";
        public const string CADENA_VACIA_CLAVE = "CADENA VACÍA";
        public const string CADENA_VACIA_VALOR = "string.Empty;";
        public const string FECHA_ACTUAL_CLAVE = "FECHA ACTUAL";
        public const string FECHA_ACTUAL_VALOR = "DateTime.Now;";
        public const string FECHA_POR_DEFECTO_CLAVE = "FECHA POR DEFECTO";
        public const string FECHA_POR_DEFECTO_VALOR = "new DateTime(1900, 1, 1);";
        public const string HORA_ACTUAL_CLAVE = "HORA ACTUAL";
        public const string HORA_ACTUAL_VALOR = "DateTime.Now.TimeOfDay;";
        public const string HORA_POR_DEFECTO_CLAVE = "HORA POR DEFECTO";
        public const string HORA_POR_DEFECTO_VALOR = "TimeSpan.Zero;";

        public enum TipoCampoAbm
        {
            FECHA,
            HORA,
            CADENA
        }

        #endregion

        #region PATHS

        public string pathControllers { get { return $@"{pathCarpetaClase}\Controllers\"; } }
        public string pathClaseController { get { return $@"{pathControllers}{TABLA}{Capas.CONTROLLER}.cs"; } }
        public string pathDto { get { return $@"{pathCarpetaClase}\{Capas.DTO}\"; } }
        public string pathClaseDto { get { return $@"{pathDto}{TABLA}{Capas.DTO}.cs"; } }
        public string pathModel { get { return $@"{pathCarpetaClase}\{Capas.MODEL}\"; } }
        public string pathClaseModel { get { return $@"{pathModel}{TABLA}{Capas.MODEL}.cs"; } }
        public string pathRepositories { get { return $@"{pathCarpetaClase}\{Capas.REPOSITORIES}\"; } }
        public string pathClaseRepositories { get { return $@"{pathRepositories}{TABLA}{Capas.REPOSITORIES}.cs"; } }
        public string pathClaseRepositoriesInterface { get { return $@"{pathRepositories}{TABLA}{Capas.REPOSITORIES_INTERFACE}.cs"; } }
        public string pathService { get { return $@"{pathCarpetaClase}\{Capas.SERVICE}\"; } }
        public string pathClaseService { get { return $@"{pathService}{TABLA}{Capas.SERVICE}.cs"; } }
        public string pathClaseServiceInterface { get { return $@"{pathService}{TABLA}{Capas.SERVICE_INTERFACE}.cs"; } }
        public string pathGlobal { get { return $@"{formulario.PathCapas}\AGREGAR AL GLOBAL.txt"; } }
        public string pathTypeScript { get { return $@"{pathCarpetaClase}\{Capas.TYPESCRIPT}\"; } }
        public string pathClaseTypeScript { get { return $@"{pathTypeScript}{TABLA}.ts"; } }
        public string pathCarpetaClase { get { return $@"{formulario.PathCapas}\{TABLA}\"; } }

        #endregion

        FRMcapibara formulario = null;

        public List<string> tablasBase = new List<string>();

        public List<string> camposTabla = new List<string>();
        
        public string TABLA = string.Empty;

        public string NombreTabla { get { return TABLA + MODEL + "." + MODELTABNAME; } }

        public Capas(FRMcapibara formulario)
        {
            this.formulario = formulario;
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
                return Capas.ERROR;
            }
        }
    }
}

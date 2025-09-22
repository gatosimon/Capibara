using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capibara
{
	public class StringConnection
	{
		public enum Motor
		{
			DB2,
			SQL
		}

		public string Servidor { get; set; }

		public string BaseDeDatos { get; set; }

		public string Consulta { get; set; }

		public Motor TipoConexion { get; set; }

		public string Obtener()
		{
			string stringConnection = string.Empty;
            switch (TipoConexion)
            {
                case Motor.DB2:
					stringConnection = $"Driver={{IBM DB2 ODBC DRIVER}};Database={BaseDeDatos};Hostname={Servidor};Port=50000; Protocol=TCPIP;Uid=db2admin;Pwd=db2admin;";
					break;
                case Motor.SQL:
					stringConnection = $@"Driver={{ODBC Driver 17 for SQL Server}};Server=SQL{Servidor}\{Servidor};Database={BaseDeDatos};Uid=usuario;Pwd=ci?r0ba;TrustServerCertificate=yes;";
					break;
            }
			return stringConnection;
		}
	}
}

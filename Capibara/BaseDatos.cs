using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capibara
{
	public class Ejecutar
	{
		//public string Conexion { get; set; }

		public string Servidor { get; set; }

		public string BaseDeDatos { get; set; }

		public string Consulta { get; set; }

		public string ObtenerConexion()
		{
			return "Driver={IBM DB2 ODBC DRIVER};Database=" + BaseDeDatos + ";Hostname=" + Servidor + ";Port=50000; Protocol=TCPIP;Uid=db2admin;Pwd=db2admin;";
		}
	}
}

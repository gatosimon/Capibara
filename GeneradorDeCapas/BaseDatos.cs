using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneradorDeCapas
{
    public class BaseDatos
    {
    }
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

	public class Tablas
	{
		public string Nombre { get; set; }

		public List<Campo> Campos { get; set; }
	}
	public class Columna
	{
		public string Nombre { get; set; }

		public bool EsClave { get; set; }

		public Type Tipo { get; set; }
	}
	public class Campo
	{
		public string Nombre { get; set; }

		public string Tipo { get; set; }

		public int Longitud { get; set; }

		public int Escala { get; set; }

		public bool AceptaNulos { get; set; }
	}
}

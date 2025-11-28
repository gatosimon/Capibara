using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Capibara
{
    public static class ConexionesManager
    {
        public static List<Conexion> ConexionesBasicas = null;

        public static string[] BasesDB2 = new string[] { "CONTABIL", "CONTAICD", "CONTAIMV", "CONTCBEL", "CONTIDS", "DOCUMENT", "GENERAL", "GIS", "HISTABM", "HISTORIC", "INFORMAT", "LICENCIA", "RRHH", "SISUS", "TRIBUTOS" };

        private static readonly string ArchivoXml = "conexiones.xml";

        public static Dictionary<string, Conexion> Cargar()
        {
            #region CONEXIONES BASICAS
            List<Conexion> ConexionesBase = new List<Conexion>();
            ConexionesBase.Add(new Conexion()
            {
                BaseDatos = "TRIBUTOS",
                Contrasena = "db2admin",
                Motor = TipoMotor.DB2,
                Nombre = "DB2 - DESARROLLO",
                Servidor = "133.123.120.120",
                Usuario = "db2admin"
            });
            ConexionesBase.Add(new Conexion()
            {
                BaseDatos = "TRIBUTOS",
                Contrasena = "db2admin",
                Motor = TipoMotor.DB2,
                Nombre = "DB2 - PRODUCCION",
                Servidor = "SERVER01",
                Usuario = "db2admin"
            });
            ConexionesBase.Add(new Conexion()
            {
                BaseDatos = "TRIBUTOS",
                Contrasena = "ci?r0ba",
                Motor = TipoMotor.MS_SQL,
                Nombre = "SQL - DESARROLLO",
                Servidor = "DESARROLLO",
                Usuario = "usuario"
            });
            ConexionesBase.Add(new Conexion()
            {
                BaseDatos = "TRIBUTOS",
                Contrasena = "ci?r0ba",
                Motor = TipoMotor.MS_SQL,
                Nombre = "SQL - PRODUCCION",
                Servidor = "PRODUCCION",
                Usuario = "usuario"
            });
            ConexionesBase.Add(new Conexion()
            {
                BaseDatos = "rafaelagovar",
                Contrasena = "ci?r0ba",
                Motor = TipoMotor.MS_SQL,
                Nombre = "SQL - DESARROLLOWEB",
                Servidor = "DESARROLLOWEB",
                Usuario = "usuario"
            }); 
            #endregion

            Dictionary<string, Conexion> resultado = new Dictionary<string, Conexion>();
            if (File.Exists(ArchivoXml))
            {
                try
                {
                    using (var fs = new FileStream(ArchivoXml, FileMode.Open))
                    {
                        var serializer = new XmlSerializer(typeof(List<Conexion>));
                        var lista = (List<Conexion>)serializer.Deserialize(fs);
                        foreach (var c in lista)
                            resultado[c.Nombre] = c;
                    }
                }
                catch
                {
                }
            }

            foreach (Conexion conexion in ConexionesBase)
            {
                if (!resultado.ContainsKey(conexion.Nombre))
                {
                    resultado.Add(conexion.Nombre, conexion);
                }
            }

            return resultado;
        }

        public static void Guardar(Dictionary<string, Conexion> conexiones)
        {
            try
            {
                var lista = new List<Conexion>(conexiones.Values);
                using (var fs = new FileStream(ArchivoXml, FileMode.Create))
                {
                    var serializer = new XmlSerializer(typeof(List<Conexion>));
                    serializer.Serialize(fs, lista);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error guardando conexiones: " + ex.Message);
            }
        }
    }
}

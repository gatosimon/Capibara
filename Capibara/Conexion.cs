using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Capibara
{

    [Serializable]
    public class Conexion
    {
        public string Nombre { get; set; }
        public TipoMotor Motor { get; set; }
        public string Servidor { get; set; }
        public string BaseDatos { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
        public string Puerto { get; set; }
        public bool EsWeb { get; set; }

        public override string ToString()
        {
            return $"Nombre={Nombre}; Motor={Motor}; Servidor={Servidor}; Puerto={Puerto}; BaseDatos={BaseDatos}; Usuario={Usuario}; Contraseña={Contrasena}; EsWeb={EsWeb}";
        }

        public string StringConnection(bool baseDeDatos = true)
        {
            string stringConnection = string.Empty;
            string driver = ObtenerNombreDriver(Motor);
            switch (Motor)
            {
                case TipoMotor.MS_SQL:
                    if (EsWeb)
                    {
                        stringConnection = $@"Driver={{{driver}}};Server={Servidor};{(baseDeDatos ? $"Database={BaseDatos};" : string.Empty)}Uid={Usuario};Pwd={Contrasena};TrustServerCertificate=yes;";
                    }
                    else if (Servidor.EndsWith("WEB"))
                    {
                        stringConnection = $@"Driver={{{driver}}};Server=SQL{Servidor.Replace("WEB", string.Empty)}\{Servidor};Database=;Uid={Usuario};Pwd={Contrasena};TrustServerCertificate=yes;";
                    }
                    else
                    {
                        stringConnection = $@"Driver={{{driver}}};Server=SQL{Servidor}\{Servidor};{(baseDeDatos ? $"Database={BaseDatos};" : string.Empty)}Uid={Usuario};Pwd={Contrasena};TrustServerCertificate=yes;";
                    }
                    break;
                case TipoMotor.DB2:
                    stringConnection = $"Driver={{{driver}}};{(baseDeDatos ? $"Database={BaseDatos};" : string.Empty)}Hostname={Servidor};{(Puerto.Trim().Length > 0 ? $"Port={Puerto};" : string.Empty)}Protocol=TCPIP;Uid={Usuario};Pwd={Contrasena};";
                    break;
                case TipoMotor.POSTGRES:
                    stringConnection = $"Driver={{{driver}}};Server={Servidor};{(Puerto.Trim().Length > 0 ? $"Port={Puerto};" : string.Empty)}Database={(baseDeDatos ? BaseDatos : "postgres")};Uid={Usuario};Pwd={Contrasena};";
                    break;
                case TipoMotor.SQLITE:
                    stringConnection = $"Driver={{{driver}}};Database={Servidor};"; //"Data Source={conexionActual.Servidor};Version=3;";
                    break;
                default:
                    break;
            }
            return stringConnection;
        }

        public static string ObtenerNombreDriver(TipoMotor motor)
        {
            string palabraClave = string.Empty;
            switch (motor)
            {
                case TipoMotor.MS_SQL:
                    palabraClave = "SQL Server";
                    break;
                case TipoMotor.DB2:
                    palabraClave = "DB2";
                    break;
                case TipoMotor.POSTGRES:
                    palabraClave = "PostgreSQL";
                    break;
                case TipoMotor.SQLITE:
                    palabraClave = "SQLite";
                    break;
                default:
                    palabraClave = string.Empty;
                    break;
            }
            List<string> drivers = new List<string>();

            // Los drivers de ODBC se encuentran en esta ruta del registro
            string registroPath = @"SOFTWARE\ODBC\ODBCINST.INI\ODBC Drivers";

            // Forzamos la apertura de la vista de 32 bits (RegistryView.Registry32)
            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                using (var rk = baseKey.OpenSubKey(registroPath))
                {
                    if (rk != null)
                    {
                        foreach (string name in rk.GetValueNames())
                        {
                            drivers.Add(name);
                        }
                    }
                }
            }
            // Buscamos el que coincida con tu base de datos (ej: "PostgreSQL" o "IBM DB2")
            string driver = drivers.FirstOrDefault(d => d.IndexOf(palabraClave, StringComparison.OrdinalIgnoreCase) >= 0).Trim();
            if (driver.Length == 0)
            {
                CustomControls.CustomMessageBox.Show($"No se encontró un driver ODBC x86 para {driver}");
            }
            return driver;
        }
    }

    public enum TipoMotor
    {
        DB2,
        MS_SQL,
        POSTGRES,
        SQLITE
    }
}

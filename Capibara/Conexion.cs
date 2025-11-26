using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override string ToString()
        {
            return $"Nombre={Nombre}; Motor={Motor}; Servidor={Servidor}; BaseDatos={BaseDatos}; Usuario={Usuario}; Contraseña={Contrasena}";
        }
    }

    public enum TipoMotor
    {
        DB2,
        MS_SQL
    }
}

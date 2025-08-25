using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capibara
{
    public class ABM
    {
        public string Fecha_Actual { get { return "System.DateTime.Now;"; } }
        public string Fecha_Por_Defecto { get { return "new DateTime(1900, 1, 1);"; } }
        public string Usuario_Magic { get { return "Config.UsuarioMagic;"; } }
        public string Codigo_Baja { get { return "codigoBaja;"; } }
        public string Motivo_Baja { get { return "motivoBaja;"; } }
        public string Codigo_0 { get { return "0;"; } }
        public string Cadena_Vacia { get { return "string.Empty;"; } }
        public string Hora_Actual { get { return "DateTime.Now.TimeOfDay;"; } }
        public string Hora_Por_Defecto { get { return "TimeSpan.Zero;"; } }
    }
}

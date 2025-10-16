using System;
using System.Configuration;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace Capibara
{
    // Token: 0x02000011 RID: 17
    public class Config : IRequiresSessionState
    {
        /// <summary>
        /// Determina si las cadenas de conexiones utilizadas por ComandoSQL están encriptadas.
        /// </summary>
        /// <remarks>Clave de web config: SIS_CadenasDeConexionesEncriptadas</remarks>
        // Token: 0x17000017 RID: 23
        // (get) Token: 0x06000063 RID: 99 RVA: 0x00004228 File Offset: 0x00002428
        public static bool CadenasDeConexionesEncriptadas
        {
            get
            {
                bool Valor = false;
                bool flag = Config.ObtenerClave("SIS_CadenasDeConexionesEncriptadas").Length > 0;
                if (flag)
                {
                    Valor = bool.Parse(Config.ObtenerClave("SIS_CadenasDeConexionesEncriptadas"));
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene la clave secreta para validar el captcha, se guarda en el web config en la clave "Captcha".
        /// </summary>
        /// <remarks>Clave de web config: Captcha</remarks>
        // Token: 0x17000018 RID: 24
        // (get) Token: 0x06000064 RID: 100 RVA: 0x00004268 File Offset: 0x00002468
        public static string ClaveCaptcha
        {
            get
            {
                string Valor = "";
                bool flag = ConfigurationManager.AppSettings["Captcha"] != null;
                if (flag)
                {
                    Valor = ConfigurationManager.AppSettings["Captcha"].ToString();
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene del web.config la clave que se utiliza para encriptar el token.
        /// </summary>
        /// <remarks>Clave de web config: SIS_ClaveToken. En caso que no exista, la clave es "secret"</remarks>
        // Token: 0x17000019 RID: 25
        // (get) Token: 0x06000065 RID: 101 RVA: 0x000042B0 File Offset: 0x000024B0
        public static string ClaveToken
        {
            get
            {
                string Valor = "secret";
                bool flag = ConfigurationManager.AppSettings["SIS_ClaveToken"] != null;
                if (flag)
                {
                    Valor = ConfigurationManager.AppSettings["SIS_ClaveToken"].ToString();
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene el número de comuna, por defecto es 1
        /// </summary>
        /// <remarks>Clave web.config: SIS_Comuna</remarks>
        // Token: 0x1700001A RID: 26
        // (get) Token: 0x06000066 RID: 102 RVA: 0x000042F8 File Offset: 0x000024F8
        public static int Comuna
        {
            get
            {
                int Valor = 1;
                bool flag = ConfigurationManager.AppSettings["SIS_Comuna"] != null;
                if (flag)
                {
                    Valor = int.Parse(ConfigurationManager.AppSettings["SIS_Comuna"].ToString());
                }
                return Valor;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>Clave de web config: SIS_CorreoElectronicoAdministrador</remarks>
        // Token: 0x1700001B RID: 27
        // (get) Token: 0x06000067 RID: 103 RVA: 0x00004340 File Offset: 0x00002540
        public static string CorreoElectronicoAdministrador
        {
            get
            {
                return Config.ObtenerClave("SIS_CorreoElectronicoAdministrador");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>Clave de web config: SIS_CorreoElectronico</remarks>
        // Token: 0x1700001C RID: 28
        // (get) Token: 0x06000068 RID: 104 RVA: 0x0000435C File Offset: 0x0000255C
        public static string CorreoElectronicoUsuario
        {
            get
            {
                return Config.ObtenerClave("SIS_CorreoElectronico");
            }
        }

        /// <summary>
        /// Obtiene un string que representa las credenciales encriptadas para iniciar sesión en el servidor de correo.
        /// </summary>
        /// <remarks>Clave de web config: SIS_CorreoElectronicoCredenciales</remarks>
        // Token: 0x1700001D RID: 29
        // (get) Token: 0x06000069 RID: 105 RVA: 0x00004378 File Offset: 0x00002578
        public static string CorreoElectronicoCredenciales
        {
            get
            {
                string Valor = "";
                bool flag = ConfigurationManager.AppSettings["SIS_CorreoElectronicoCredenciales"] != null;
                if (flag)
                {
                    Valor = ConfigurationManager.AppSettings["SIS_CorreoElectronicoCredenciales"].ToString();
                }
                return Valor;
            }
        }

        /// <summary>
        /// Determina si en la respuesta muestra o no un debug.
        /// </summary>
        /// <remarks>Clave de web config: SIS_Debug</remarks>
        // Token: 0x1700001E RID: 30
        // (get) Token: 0x0600006A RID: 106 RVA: 0x000043C0 File Offset: 0x000025C0
        public static bool Debug
        {
            get
            {
                bool Valor = false;
                bool flag = ConfigurationManager.AppSettings["SIS_Debug"] != null;
                if (flag)
                {
                    Valor = bool.Parse(ConfigurationManager.AppSettings["SIS_Debug"].ToString());
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene el directorio donde se guardan los archivos.
        /// </summary>
        /// <remarks>Clave de web config: SIS_DirectorioDeArchivos. La URL debe ser absoluta. El valor por defecto es "/Archivos".</remarks>
        // Token: 0x1700001F RID: 31
        // (get) Token: 0x0600006B RID: 107 RVA: 0x00004408 File Offset: 0x00002608
        public static string DirectorioDeArchivos
        {
            get
            {
                string Valor = "/Archivos";
                bool flag = ConfigurationManager.AppSettings["SIS_DirectorioDeArchivos"] != null;
                if (flag)
                {
                    Valor = ConfigurationManager.AppSettings["SIS_DirectorioDeArchivos"].ToString();
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene el directorio donde se guardan los archivos temporales.
        /// </summary>
        /// <remarks>Clave de web config: SIS_DirectorioDeArchivosTemporales. La URL debe ser absoluta. El valor por defecto es "/Archivos/Tmp".</remarks>
        // Token: 0x17000020 RID: 32
        // (get) Token: 0x0600006C RID: 108 RVA: 0x00004450 File Offset: 0x00002650
        public static string DirectorioDeArchivosTemporales
        {
            get
            {
                string Valor = "/Archivos/Tmp";
                bool flag = ConfigurationManager.AppSettings["SIS_DirectorioDeArchivosTemporales"] != null;
                if (flag)
                {
                    Valor = ConfigurationManager.AppSettings["SIS_DirectorioDeArchivosTemporales"].ToString();
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene el dominio del sitio.
        /// </summary>
        // Token: 0x17000021 RID: 33
        // (get) Token: 0x0600006D RID: 109 RVA: 0x00004498 File Offset: 0x00002698
        public static string Host
        {
            get
            {
                return HttpContext.Current.Request.Url.Host.ToString();
            }
        }

        /// <summary>
        /// Obtiene el número que identifica el sistema.
        /// </summary>
        /// <remarks>Clave de web config: SIS_IdSistema</remarks>
        // Token: 0x17000022 RID: 34
        // (get) Token: 0x0600006E RID: 110 RVA: 0x000044C4 File Offset: 0x000026C4
        public static int IdSistema
        {
            get
            {
                int Valor = 0;
                bool flag = ConfigurationManager.AppSettings["SIS_IdSistema"] != null;
                if (flag)
                {
                    int.TryParse(ConfigurationManager.AppSettings["SIS_IdSistema"].ToString(), out Valor);
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene el número que identifica el usuario que inició sesión.
        /// </summary>
        // Token: 0x17000023 RID: 35
        // (get) Token: 0x0600006F RID: 111 RVA: 0x00004510 File Offset: 0x00002710
        public static int IdUsuario
        {
            get
            {
                int Valor = 0;
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene la ruta de la imagen del usuario que inició sesión.
        /// </summary>
        // Token: 0x17000024 RID: 36
        // (get) Token: 0x06000070 RID: 112 RVA: 0x0000453C File Offset: 0x0000273C
        public static string ImagenUsuario
        {
            get
            {
                bool flag = HttpContext.Current.Session["SIS_ImagenUsuario"] != null;
                string Valor;
                Valor = HttpContext.Current.Session["SIS_ImagenUsuario"].ToString();
                //if (flag)
                //{
                //	Valor = HttpContext.Current.Session["SIS_ImagenUsuario"].ToString();
                //}
                //else
                //{
                //	Valor = Rutinas.EncriptarTexto("Imagenes/Generico.jpg");
                //}
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene las extencionesde imágenes permitidas separadas por coma y sin punto.
        /// </summary>
        /// <remarks>Clave de web config: SIS_ImagenesPermitidas</remarks>
        // Token: 0x17000025 RID: 37
        // (get) Token: 0x06000071 RID: 113 RVA: 0x0000459C File Offset: 0x0000279C
        public static string ImagenesPermitidas
        {
            get
            {
                string Valor = "jpg,png,gif";
                bool flag = ConfigurationManager.AppSettings["SIS_ImagenesPermitidas"] != null;
                if (flag)
                {
                    Valor = ConfigurationManager.AppSettings["SIS_ImagenesPermitidas"].ToString();
                }
                return Valor;
            }
        }

        /// <summary>
        /// Determina si debe o no mostrar los errores del servidor si importar que se haya puento "debug=true" en el web.config.
        /// </summary>
        /// <remarks>Clave de web config: SIS_MostrarErroresDetallados</remarks>
        // Token: 0x17000026 RID: 38
        // (get) Token: 0x06000072 RID: 114 RVA: 0x000045E4 File Offset: 0x000027E4
        public static bool MostrarErroresDetallados
        {
            get
            {
                bool Valor = false;
                bool flag = ConfigurationManager.AppSettings["SIS_MostrarErroresDetallados"] != null;
                if (flag)
                {
                    bool.TryParse(ConfigurationManager.AppSettings["SIS_MostrarErroresDetallados"].ToString(), out Valor);
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene el nombre del sistema.
        /// </summary>
        /// <remarks>Clave de web config: SIS_NombreDelSistema</remarks>
        // Token: 0x17000027 RID: 39
        // (get) Token: 0x06000073 RID: 115 RVA: 0x00004630 File Offset: 0x00002830
        public static string NombreDelSistema
        {
            get
            {
                string Valor = "";
                bool flag = ConfigurationManager.AppSettings["SIS_NombreDelSistema"] != null;
                if (flag)
                {
                    Valor = ConfigurationManager.AppSettings["SIS_IdSistema"].ToString();
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene el nombre del usuario que inició sesión.
        /// </summary>
        // Token: 0x17000028 RID: 40
        // (get) Token: 0x06000074 RID: 116 RVA: 0x00004678 File Offset: 0x00002878
        public static string NombreUsuario
        {
            get
            {
                string Valor = "";
                bool flag = HttpContext.Current.Session["SIS_NombreUsuario"] != null;
                if (flag)
                {
                    Valor = HttpContext.Current.Session["SIS_NombreUsuario"].ToString();
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene el puerto para enviar correo. Si no se encuentra usa el puerto 25.
        /// </summary>
        /// <remarks>Clave de web config: SIS_PuertoParaCorreo</remarks>
        // Token: 0x17000029 RID: 41
        // (get) Token: 0x06000075 RID: 117 RVA: 0x000046C8 File Offset: 0x000028C8
        public static int PuertoParaCorreo
        {
            get
            {
                int Valor = 587;
                bool flag = ConfigurationManager.AppSettings["SIS_PuertoParaCorreo"] != null;
                if (flag)
                {
                    int.TryParse(ConfigurationManager.AppSettings["SIS_PuertoParaCorreo"].ToString(), out Valor);
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene el número de registros que se muestran en cada página de los listados.
        /// </summary>
        /// <remarks>Clave de web.config: SIS_RegistrosPorPaginas</remarks>
        // Token: 0x1700002A RID: 42
        // (get) Token: 0x06000076 RID: 118 RVA: 0x00004718 File Offset: 0x00002918
        public static int RegistrosPorPaginas
        {
            get
            {
                int Valor = 20;
                bool flag = HttpContext.Current.Session["SIS_RegistrosPorPaginas"] != null;
                if (flag)
                {
                    Valor = int.Parse(HttpContext.Current.Session["SIS_RegistrosPorPaginas"].ToString());
                }
                return Valor;
            }
        }

        /// <summary>
        /// Determina si debe o no controlar la seguridad. Por defecto está habilitada.
        /// </summary>
        /// <remarks>Clave de web.config: SIS_SeguridadHabilitada</remarks>
        // Token: 0x1700002B RID: 43
        // (get) Token: 0x06000077 RID: 119 RVA: 0x0000476C File Offset: 0x0000296C
        public static bool SeguridadHabilitada
        {
            get
            {
                bool Valor = true;
                bool flag = ConfigurationManager.AppSettings["SIS_SeguridadHabilitada"] != null;
                if (flag)
                {
                    Valor = bool.Parse(ConfigurationManager.AppSettings["SIS_SeguridadHabilitada"].ToString());
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene la configuración de cual es el separador decimal de un número. Si no lo encuentra devuelve la configuración del sistema.
        /// </summary>
        /// <remarks>Clave de web.config: SIS_SeparadorDeDecimales</remarks>
        // Token: 0x1700002C RID: 44
        // (get) Token: 0x06000078 RID: 120 RVA: 0x000047B4 File Offset: 0x000029B4
        public static string SeparadorDeDecimales
        {
            get
            {
                string Valor = Config.ObtenerClave("SIS_SeparadorDeDecimales");
                bool flag = Valor.Length == 0;
                if (flag)
                {
                    Valor = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene la configuración de cual es el separador de miles de un número. Si no lo encuentra devuelve la configuración del sistema.
        /// </summary>
        /// <remarks>Clave de web.config: SIS_SeparadorDeMiles</remarks>
        // Token: 0x1700002D RID: 45
        // (get) Token: 0x06000079 RID: 121 RVA: 0x000047F8 File Offset: 0x000029F8
        public static string SeparadorDeMiles
        {
            get
            {
                string Valor = Config.ObtenerClave("SIS_SeparadorDeMiles");
                bool flag = Valor.Length == 0;
                if (flag)
                {
                    Valor = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator;
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene el nombre del servidor que sirve para enviar correos. Si no encuentra la clave usa localhost.
        /// </summary>
        /// <remarks>Clave de web config: SIS_ServidorDeCorreo</remarks>
        // Token: 0x1700002E RID: 46
        // (get) Token: 0x0600007A RID: 122 RVA: 0x0000483C File Offset: 0x00002A3C
        public static string ServidorDeCorreo
        {
            get
            {
                string Valor = "localhost";
                bool flag = ConfigurationManager.AppSettings["SIS_ServidorDeCorreo"] != null;
                if (flag)
                {
                    Valor = ConfigurationManager.AppSettings["SIS_ServidorDeCorreo"].ToString();
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene la segunda parte de la url para llamar a un servicio. Éste se concatena con UrlServicios.
        /// </summary>
        /// <remarks>Clave de web.config: SIS_UrlAdicional</remarks>
        // Token: 0x17000030 RID: 48
        // (get) Token: 0x0600007D RID: 125 RVA: 0x0000493C File Offset: 0x00002B3C
        public static string UrlAdicional
        {
            get
            {
                string Valor = "";
                bool flag = ConfigurationManager.AppSettings["SIS_UrlAdicional"] != null;
                if (flag)
                {
                    Valor = ConfigurationManager.AppSettings["SIS_UrlAdicional"].ToString();
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene la URL donde está guardada la fuente de codigo de barras "3 of 9 barcode".
        /// </summary>
        // Token: 0x17000031 RID: 49
        // (get) Token: 0x0600007E RID: 126 RVA: 0x00004984 File Offset: 0x00002B84
        public static string UrlCodigoDeBarrasBarcode
        {
            get
            {
                string Valor = "/Fonts/3of9.ttf";
                bool flag = ConfigurationManager.AppSettings["SIS_UrlCodigoDeBarrasBarcode"] != null;
                if (flag)
                {
                    Valor = ConfigurationManager.AppSettings["SIS_UrlCodigoDeBarrasBarcode"].ToString();
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene la URL donde está guardada la fuente de codigo de barras "Bcode 128".
        /// </summary>
        // Token: 0x17000032 RID: 50
        // (get) Token: 0x0600007F RID: 127 RVA: 0x000049CC File Offset: 0x00002BCC
        public static string UrlCodigoDeBarrasEAN
        {
            get
            {
                string Valor = "/Fonts/Bcode128.ttf";
                bool flag = ConfigurationManager.AppSettings["SIS_UrlCodigoDeBarrasEAN"] != null;
                if (flag)
                {
                    Valor = ConfigurationManager.AppSettings["SIS_UrlCodigoDeBarrasEAN"].ToString();
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene la URL donde está guardada la fuente de codigo de barras "3of9".
        /// </summary>
        // Token: 0x17000033 RID: 51
        // (get) Token: 0x06000080 RID: 128 RVA: 0x00004A14 File Offset: 0x00002C14
        public static string UrlCodigoDeBarras3of9
        {
            get
            {
                string Valor = "/Fonts/3of9.ttf";
                bool flag = ConfigurationManager.AppSettings["SIS_UrlCodigoDeBarras3of9"] != null;
                if (flag)
                {
                    Valor = ConfigurationManager.AppSettings["SIS_UrlCodigoDeBarras3of9"].ToString();
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene la URL en donde está ubicado el inicio de sesión.
        /// </summary>
        // Token: 0x17000034 RID: 52
        // (get) Token: 0x06000081 RID: 129 RVA: 0x00004A5C File Offset: 0x00002C5C
        public static string UrlInicioDeSesion
        {
            get
            {
                string Valor = "";
                bool flag = ConfigurationManager.AppSettings["SIS_UrlInicioDeSesion"] != null;
                if (flag)
                {
                    Valor = ConfigurationManager.AppSettings["SIS_UrlInicioDeSesion"].ToString();
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene la URL en donde se encuentra el servidor de Web Services.
        /// </summary>
        /// <remarks>Clave de web.config: SIS_UrlServicios</remarks>
        // Token: 0x17000035 RID: 53
        // (get) Token: 0x06000082 RID: 130 RVA: 0x00004AA4 File Offset: 0x00002CA4
        public static string UrlServicios
        {
            get
            {
                string Valor = "";
                bool flag = ConfigurationManager.AppSettings["SIS_UrlServicios"] != null;
                if (flag)
                {
                    Valor = ConfigurationManager.AppSettings["SIS_UrlServicios"].ToString();
                }
                return Valor;
            }
        }

        /// <summary>
        /// Obtiene el nombre de usuario de magic del usuario que inició sesión.
        /// </summary>
        // Token: 0x17000036 RID: 54
        // (get) Token: 0x06000083 RID: 131 RVA: 0x00004AEC File Offset: 0x00002CEC
        public static string UsuarioMagic
        {
            get
            {
                string Valor = "";
                return Valor;
            }
        }

        /// <summary>
        /// Guarda un objeto en una variable de sesión.
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="valor"></param>
        /// <remarks>Si el nombre de la variable existe, reemplaza el valor existente.</remarks>
        // Token: 0x06000084 RID: 132 RVA: 0x00004B1C File Offset: 0x00002D1C
        [Obsolete("No se usa más", true)]
        public static void GuardarEnSesion(string nombre, object valor)
        {
            bool flag = HttpContext.Current.Session[nombre] == null;
            if (flag)
            {
                HttpContext.Current.Session.Add(nombre, valor);
            }
            else
            {
                HttpContext.Current.Session[nombre] = valor;
            }
        }

        /// <summary>
        /// Busca una clave en el appSettings del web.config.
        /// </summary>
        /// <param name="nombre"></param>
        /// <returns>Devuelve una cadena vacía si no existe.</returns>
        // Token: 0x06000085 RID: 133 RVA: 0x00004B6C File Offset: 0x00002D6C
        public static string ObtenerClave(string clave)
        {
            bool flag = ConfigurationManager.AppSettings[clave] != null;
            string result;
            if (flag)
            {
                result = ConfigurationManager.AppSettings[clave];
            }
            else
            {
                result = "";
            }
            return result;
        }

        /// <summary>
        /// Busca un valor en las varibles de sesión ya convertido al tipo específico.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nombre"></param>
        /// <returns></returns>
        // Token: 0x06000086 RID: 134 RVA: 0x00004BA4 File Offset: 0x00002DA4
        [Obsolete("No se usa más", true)]
        public static T ObtenerDeSesion<T>(string nombre)
        {
            T Valor = default(T);
            bool flag = HttpContext.Current.Session[nombre] != null;
            if (flag)
            {
                Valor = (T)((object)HttpContext.Current.Session[nombre]);
            }
            return Valor;
        }
    }
}

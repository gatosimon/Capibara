using System;
using System.IO;
using System.Xml.Serialization;

[Serializable]
public class Configuracion
{
    public string PathSolucion { get; set; }
    public bool SQL { get; set; }
    public string Servidor { get; set; }
    public string Base { get; set; }
    public string Tabla { get; set; }
    public string Consulta { get; set; }
    public string UltimoNamespaceSeleccionado { get; set; }
    public string RutaPorDefectoResultados { get; set; }
    public Configuracion()
    {
        PathSolucion = string.Empty;
        SQL = false;
        Servidor = string.Empty;
        Base = string.Empty;
        Tabla = string.Empty;
        Consulta = string.Empty;
        UltimoNamespaceSeleccionado = string.Empty;
        RutaPorDefectoResultados = string.Empty;
    }

    public static string ArchivoConfigPorDefecto
    {
        get
        {
            string carpeta = AppDomain.CurrentDomain.BaseDirectory + @"\Configuracion\";
            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }
            return Path.Combine(carpeta, "configuracion.xml");
        }
    }

    public void Guardar()
    {
        string rutaArchivo = ArchivoConfigPorDefecto;

        using (var writer = new StreamWriter(rutaArchivo))
        {
            var serializer = new XmlSerializer(typeof(Configuracion));
            serializer.Serialize(writer, this);
        }
    }

    public static Configuracion Cargar()
    {
        string rutaArchivo = ArchivoConfigPorDefecto;

        if (!File.Exists(rutaArchivo))
            return new Configuracion(); // Devuelve configuración vacía si no existe

        using (var reader = new StreamReader(rutaArchivo))
        {
            var serializer = new XmlSerializer(typeof(Configuracion));
            return (Configuracion)serializer.Deserialize(reader);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

[Serializable]
public class Configuracion
{
    public string PathSolucion { get; set; } = string.Empty;
    public bool SQL { get; set; } = false;
    public string Servidor { get; set; } = string.Empty;
    public string Base { get; set; } = string.Empty;
    public string Tabla { get; set; } = string.Empty;
    public string Consulta { get; set; } = string.Empty;
    public List<string[]> camposBaja { get; set; } = new List<string[]>();
    public List<string[]> camposModificacion { get; set; } = new List<string[]>();
    public List<string[]> camposRecuperacion { get; set; } = new List<string[]>();
    public string UltimoNamespaceSeleccionado { get; set; } = string.Empty;
    public string RutaPorDefectoResultados { get; set; } = @"C:\Temp\";
    public bool MostrarOverlayEnInicio { get; set; } = true;
    public Configuracion()
    {
        //PathSolucion = string.Empty;
        //SQL = false;
        //Servidor = string.Empty;
        //Base = string.Empty;
        //Tabla = string.Empty;
        //Consulta = string.Empty;
        //camposBaja = new List<string[]>();
        //camposModificacion = new List<string[]>();
        //camposRecuperacion = new List<string[]>();
        //UltimoNamespaceSeleccionado = string.Empty;
        //RutaPorDefectoResultados = @"C:\Temp\";
        //MostrarOverlayEnInicio = true;
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

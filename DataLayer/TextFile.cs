namespace DataLayer
{
    /// <summary>
    /// Funciones y utilidades para el manejo de archivos de texto planos
    /// </summary>
    public static class TextFile
    {
        /// <summary>
        /// Exportar un DataTable a un archivo de texto
        /// </summary>
        /// <param name="file">archivo de texto a generar</param>
        /// <param name="dataTable">tabla a exportar</param>
        /// <param name="fieldSeparator">caracter que dividira los campos</param>
        /// <param name="addHeader">agregar cabecera con nombres de columnas</param>
        static public void ExportTxt(string file, System.Data.DataTable dataTable, string fieldSeparator, bool addHeader)
        {
            System.Text.StringBuilder Linea = new System.Text.StringBuilder();
            System.IO.StreamWriter MiStreamWriter = new System.IO.StreamWriter(file, false);
            //primero agrega nombre de Campos
            if (addHeader)
            {
                foreach (System.Data.DataColumn ColumnaActual in dataTable.Columns)
                {
                    Linea.Append(ColumnaActual.ColumnName);
                    Linea.Append(fieldSeparator);
                }
                Linea.Remove(Linea.Length - fieldSeparator.Length, fieldSeparator.Length);
                MiStreamWriter.WriteLine(Linea.ToString());
            }
            //despues agrega registros
            foreach (System.Data.DataRow FilaActual in dataTable.Rows)
            {
                Linea = new System.Text.StringBuilder();
                foreach (object ValorActual in FilaActual.ItemArray)
                {
                    Linea.Append(ValorActual.ToString().Replace("\r\n", ""));
                    Linea.Append(fieldSeparator);
                }
                Linea.Remove(Linea.Length - fieldSeparator.Length, fieldSeparator.Length);
                MiStreamWriter.WriteLine(Linea.ToString());
                Linea = new System.Text.StringBuilder();
            }
            MiStreamWriter.Close();
        }

        /// <summary>
        /// Importar un archivo de texto a un DataTable
        /// </summary>
        /// <param name="file">archivo de texto a importar</param>
        /// <param name="filedSeparator">caracter que separa campos en el archivo</param>
        /// <param name="headers">cabeceras de las columnas (se asume que el archivo no contiene una linea de cabeceras)</param>
        /// <returns>devuelve un DataTable con el contenido del archivo</returns>
        static public System.Data.DataTable ImportTxt(string file, string filedSeparator, params string[] headers)
        {
            return ImportTxt(System.Text.Encoding.Default, file, filedSeparator, headers);
        }

        /// <summary>
        /// Importar un archivo de texto a un DataTable
        /// </summary>
        /// <param name="file">archivo de texto a importar</param>
        /// <param name="filedSeparator">caracter que separa campos en el archivo</param>
        /// <param name="headers">cabeceras de las columnas (se asume que el archivo no contiene una linea de cabeceras)</param>
        /// <param name="encoding">encoding del archivo a importar</param>
        /// <returns>devuelve un DataTable con el contenido del archivo</returns>
        static public System.Data.DataTable ImportTxt(System.Text.Encoding encoding, string file, string filedSeparator, params string[] headers)
        {
            //rellena un DataTable con el archivo de texto
            System.Data.DataTable Tabla = ImportTxt(encoding, file, filedSeparator, false);
            //completa las cabeceras de columnas con los valores pasados
            int Maximo;
            if (headers.Length > Tabla.Columns.Count)
            {
                Maximo = Tabla.Columns.Count;
            }
            else
            {
                Maximo = headers.Length;
            }
            for (int i = 0; i < Maximo; i++)
            {
                Tabla.Columns[i].ColumnName = headers[i];
            }
            //devuelve el resultado
            return Tabla;
        }

        /// <summary>
        /// Importar un archivo de texto a un DataTable
        /// </summary>
        /// <param name="file">archivo de texto a importar</param>
        /// <param name="fieldSeparator">caracter que separa campos en el archivo</param>
        /// <param name="hasHeaders">si el archivo posee una cabecera con los nombres de columnas</param>
        /// <returns>devuelve un DataTable con el contenido del archivo</returns>
        static public System.Data.DataTable ImportTxt(string file, string fieldSeparator, bool hasHeaders)
        {
            return ImportTxt(System.Text.Encoding.Default, file, fieldSeparator, hasHeaders);
        }

        /// <summary>
        /// Importar un archivo de texto a un DataTable
        /// </summary>
        /// <param name="file">archivo de texto a importar</param>
        /// <param name="fieldSeparator">caracter que separa campos en el archivo</param>
        /// <param name="hasHeaders">si el archivo posee una cabecera con los nombres de columnas</param>
        /// <returns>devuelve un DataTable con el contenido del archivo</returns>
        /// <param name="encoding">encoding del archivo a importar</param>
        static public System.Data.DataTable ImportTxt(System.Text.Encoding encoding, string file, string fieldSeparator, bool hasHeaders)
        {
            //cuento cuantas Columnas tiene apartir de la cantidad de divisores | en la
            //primer linea
            System.IO.StreamReader MiStreamReader = new System.IO.StreamReader(file, encoding);
            string Linea = "";

            Linea = MiStreamReader.ReadLine();
            int Columnas = 1;
            for (int cact = 0; cact < Linea.Length; cact++)
            {
                if (Linea.Substring(cact, 1) == fieldSeparator)
                {
                    Columnas++;
                }
            }
            MiStreamReader.Close();
            //armo objetos de matriz y tablas con la cantidad de columnas obtenidas
            MiStreamReader = new System.IO.StreamReader(file);
            System.Data.DataTable Tabla = new System.Data.DataTable();
            for (int col = 0; col < Columnas; col++)
            {
                Tabla.Columns.Add("" + col);
            }
            //completa nombres de columnas
            if (hasHeaders)
            {
                Linea = MiStreamReader.ReadLine();
                int ColumnaN = 0;
                for (int cactn = 0; cactn < Linea.Length; cactn++)
                {
                    if (Linea.Substring(cactn, 1) == fieldSeparator)
                    {
                        ColumnaN++;
                    }
                    else
                    {
                        Tabla.Columns[ColumnaN].ColumnName += Linea.Substring(cactn, 1);
                    }
                }
                for (int col = 0; col < Columnas; col++)
                {
                    Tabla.Columns[col].ColumnName = Tabla.Columns[col].ColumnName.Substring(col.ToString().Length, (Tabla.Columns[col].ColumnName.Length - col.ToString().Length));
                }
            }
            //relleno el datatable
            Linea = MiStreamReader.ReadLine();
            string[] Fila;
            while (Linea != null)
            {
                Fila = new string[Linea.Split(fieldSeparator.ToCharArray()).Length];
                Fila = Linea.Split(fieldSeparator.ToCharArray());
                Tabla.Rows.Add(Fila);
                Linea = MiStreamReader.ReadLine();
            }
            MiStreamReader.Close();
            return Tabla;
        }

        /// <summary>
        /// Columna de posiciones fijas en un archivo de texto
        /// </summary>
        public class FixedColumn
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="start">posicion del primer caracter</param>
            /// <param name="length">longitud en caracteres</param>
            public FixedColumn(int start, int length)
            {
                Start = start;
                Length = length;
            }

            /// <summary>
            /// Posicion del primer caracter
            /// </summary>
            private int _Start = 0;
            /// <summary>
            /// Posicion del primer caracter
            /// </summary>
            public int Start
            {
                get { return _Start; }
                set { _Start = value; }
            }

            /// <summary>
            /// Longitud en caracteres
            /// </summary>
            private int _Length = 0;
            /// <summary>
            /// Longitud en caracteres
            /// </summary>
            public int Length
            {
                get { return _Length; }
                set { _Length = value; }
            }
        }

        /// <summary>
        /// Columna de posiciones fijas en un archivo de texto
        /// </summary>
        public class NamedFixedColumn : FixedColumn
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="columnName">nombre de la columna</param>
            /// <param name="start">posicion del primer caracter</param>
            /// <param name="length">longitud en caracteres</param>
            public NamedFixedColumn(string columnName, int start, int length)
                : base(start, length)
            {
                ColumnName = columnName;
            }

            /// <summary>
            /// Nombre de la columna
            /// </summary>
            private string _ColumnName = "";
            /// <summary>
            /// Nombre de la columna
            /// </summary>
            public string ColumnName
            {
                get { return _ColumnName; }
                set { _ColumnName = value; }
            }
        }

        /// <summary>
        /// Importar un archivo de texto a un DataTable
        /// </summary>
        /// <param name="file">archivo de texto a importar</param>
        /// <param name="namedFixedColumns">columnas de posicion fija a importar, con nombre de columna especificado (se asume que el archivo no contiene una linea de cabeceras)</param>
        /// <returns>devuelve un DataTable con el contenido del archivo</returns>
        static public System.Data.DataTable ImportTxt(string file, params NamedFixedColumn[] namedFixedColumns)
        {
            return ImportTxt(System.Text.Encoding.Default, file, namedFixedColumns);
        }

        /// <summary>
        /// Importar un archivo de texto a un DataTable
        /// </summary>
        /// <param name="file">archivo de texto a importar</param>
        /// <param name="namedFixedColumns">columnas de posicion fija a importar, con nombre de columna especificado (se asume que el archivo no contiene una linea de cabeceras)</param>
        /// <returns>devuelve un DataTable con el contenido del archivo</returns>
        /// <param name="encoding">encoding del archivo a importar</param>
        static public System.Data.DataTable ImportTxt(System.Text.Encoding encoding, string file, params NamedFixedColumn[] namedFixedColumns)
        {
            //rellena un DataTable con el archivo de texto
            System.Data.DataTable Tabla = ImportTxt(encoding, file, false, namedFixedColumns);
            //completa las cabeceras de columnas con los valores pasados
            for (int i = 0; i < namedFixedColumns.Length; i++)
            {
                Tabla.Columns[i].ColumnName = namedFixedColumns[i].ColumnName;
            }
            //devuelve el resultado
            return Tabla;
        }

        /// <summary>
        /// Importar un archivo de texto a un DataTable
        /// </summary>
        /// <param name="file">archivo de texto a importar</param>
        /// <param name="hasHeaders">si el archivo posee una cabecera con los nombres de columnas</param>
        /// <param name="fixedColumns">columnas de posicion fija a importar</param>
        /// <returns>devuelve un DataTable con el contenido del archivo</returns>
        static public System.Data.DataTable ImportTxt(string file, bool hasHeaders, params FixedColumn[] fixedColumns)
        {
            return ImportTxt(System.Text.Encoding.Default, file, hasHeaders, fixedColumns);
        }

        /// <summary>
        /// Importar un archivo de texto a un DataTable
        /// </summary>
        /// <param name="file">archivo de texto a importar</param>
        /// <param name="hasHeaders">si el archivo posee una cabecera con los nombres de columnas</param>
        /// <param name="fixedColumns">columnas de posicion fija a importar</param>
        /// <param name="encoding">encoding del archivo a importar</param>
        /// <returns>devuelve un DataTable con el contenido del archivo</returns>
        static public System.Data.DataTable ImportTxt(System.Text.Encoding encoding, string file, bool hasHeaders, params FixedColumn[] fixedColumns)
        {
            //cuento cuantas Columnas tiene apartir de la cantidad de divisores | en la
            //primer linea
            System.IO.StreamReader MiStreamReader = new System.IO.StreamReader(file, encoding);
            string Linea = "";

            //armo objetos de matriz y tablas con la cantidad de columnas obtenidas
            System.Data.DataTable Tabla = new System.Data.DataTable();
            for (int col = 0; col < fixedColumns.Length; col++)
            {
                Tabla.Columns.Add("" + col);
            }
            //completa nombres de columnas
            if (hasHeaders)
            {
                Linea = MiStreamReader.ReadLine();
                for (int cactn = 0; cactn < fixedColumns.Length; cactn++)
                {
                    Tabla.Columns[cactn].ColumnName += Linea.Substring(fixedColumns[cactn].Start, fixedColumns[cactn].Length);
                }
            }
            //relleno el datatable
            Linea = MiStreamReader.ReadLine();
            string[] Fila = new string[fixedColumns.Length];
            while (Linea != null)
            {
                for (int i = 0; i < fixedColumns.Length; i++)
                {
                    Fila[i] = Linea.Substring(fixedColumns[i].Start, fixedColumns[i].Length);
                }
                Tabla.Rows.Add(Fila);
                Linea = MiStreamReader.ReadLine();
            }
            MiStreamReader.Close();
            return Tabla;
        }
    }
}

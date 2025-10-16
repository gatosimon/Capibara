namespace DataLayer
{
    /// <summary>
    /// Atributos de la tabla donde se guardan los objetos
    /// </summary>
    public class TableAttributes : System.Attribute
    {
        /// <summary>
        /// Nombre de la tabla
        /// </summary>
        internal string _TableName = "";
        /// <summary>
        /// Nombre de la tabla
        /// </summary>
        public string TableName
        {
            get { return _TableName; }
            set { _TableName = value; }
        }
    }
}

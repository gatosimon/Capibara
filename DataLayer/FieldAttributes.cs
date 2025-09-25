using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DataLayer
{
    /// <summary>
    /// Atributos del campo de la tabla donde se guarda la propiedad
    /// </summary>
    public class FieldAttributes : System.Attribute
    {
        /// <summary>
        /// Nombre del campo
        /// </summary>
        internal string _ColumnName = "";
        /// <summary>
        /// Nombre del campo
        /// </summary>
        public string ColumnName
        {
            get { return _ColumnName; }
            set { _ColumnName = value; }
        }

        /// <summary>
        /// Tipo de dato (obligatorio)
        /// </summary>
        private DbType _dbType = DbType.String;
        /// <summary>
        /// Tipo de dato (obligatorio)
        /// </summary>
        public DbType dbType
        {
            get { return _dbType; }
            set { _dbType = value; }
        }

        /// <summary>
        /// Es campo clave
        /// </summary>
        private bool _Key = false;
        /// <summary>
        /// Es campo clave
        /// </summary>
        public bool Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        /// <summary>
        /// Longitud
        /// </summary>
        private int _Size = 0;
        /// <summary>
        /// Longitud
        /// </summary>
        public int Size
        {
            get { return _Size; }
            set { _Size = value; }
        }

        /// <summary>
        /// Escala
        /// </summary>
        private byte _Scale = 0;
        /// <summary>
        /// Escala
        /// </summary>
        public byte Scale
        {
            get { return _Scale; }
            set { _Scale = value; }
        }

        /// <summary>
        /// Precision
        /// </summary>
        private byte _Precision = 0;
        /// <summary>
        /// Precision
        /// </summary>
        public byte Precision
        {
            get { return _Precision; }
            set { _Precision = value; }
        }
    }
}

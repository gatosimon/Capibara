using System;
using System.Collections.Generic;

namespace DataLayer
{
    public class Conversor
    {
        static Dictionary<string, Type> SQL2DOTNET = new Dictionary<string, Type>();
        static Dictionary<Type, string> DOTNET2SQL = new Dictionary<Type, string>();

        static public Type TipoDatoDotNet(string tipoDatoSQL)
        {
            if (SQL2DOTNET.Keys.Count == 0)
            {
                SQL2DOTNET.Add("bit", typeof(System.Boolean));
                SQL2DOTNET.Add("tinyint", typeof(System.Byte));
                SQL2DOTNET.Add("smallint", typeof(System.Int16));
                SQL2DOTNET.Add("int", typeof(System.Int32));
                SQL2DOTNET.Add("smallmoney", typeof(System.Double));
                SQL2DOTNET.Add("real", typeof(System.Double));
                SQL2DOTNET.Add("bigint", typeof(System.Int64));
                SQL2DOTNET.Add("money", typeof(System.Double));
                SQL2DOTNET.Add("float", typeof(System.Double));
                SQL2DOTNET.Add("decimal", typeof(System.Double));
                SQL2DOTNET.Add("numeric", typeof(System.Double));
                SQL2DOTNET.Add("date", typeof(System.DateTime));
                SQL2DOTNET.Add("datetime2", typeof(System.DateTime));
                SQL2DOTNET.Add("datetime", typeof(System.DateTime));
                SQL2DOTNET.Add("char", typeof(System.String));
                SQL2DOTNET.Add("varchar", typeof(System.String));
                SQL2DOTNET.Add("text", typeof(System.String));
                SQL2DOTNET.Add("nchar", typeof(System.String));
                SQL2DOTNET.Add("nvarchar", typeof(System.String));
                SQL2DOTNET.Add("ntext", typeof(System.String));
            }
            return SQL2DOTNET[tipoDatoSQL];
        }

        static public string TipoDatoSQL(Type tipoDatoDotNet)
        {
            if (DOTNET2SQL.Keys.Count == 0)
            {
                DOTNET2SQL.Add(typeof(System.Boolean), "bit");
                DOTNET2SQL.Add(typeof(System.Byte), "tinyint");
                DOTNET2SQL.Add(typeof(System.Int16), "smallint");
                DOTNET2SQL.Add(typeof(System.Int32), "int");
                DOTNET2SQL.Add(typeof(System.Double), "smallmoney");
                DOTNET2SQL.Add(typeof(System.Double), "real");
                DOTNET2SQL.Add(typeof(System.Int64), "bigint");
                DOTNET2SQL.Add(typeof(System.Double), "money");
                DOTNET2SQL.Add(typeof(System.Double), "float");
                DOTNET2SQL.Add(typeof(System.Double), "decimal");
                DOTNET2SQL.Add(typeof(System.Double), "numeric");
                DOTNET2SQL.Add(typeof(System.DateTime), "date");
                DOTNET2SQL.Add(typeof(System.DateTime), "datetime2");
                DOTNET2SQL.Add(typeof(System.DateTime), "datetime");
                DOTNET2SQL.Add(typeof(System.String), "char");
                DOTNET2SQL.Add(typeof(System.String), "varchar");
                DOTNET2SQL.Add(typeof(System.String), "text");
                DOTNET2SQL.Add(typeof(System.String), "nchar");
                DOTNET2SQL.Add(typeof(System.String), "nvarchar");
                DOTNET2SQL.Add(typeof(System.String), "ntext");
            }
            return DOTNET2SQL[tipoDatoDotNet];
        }
    }
}

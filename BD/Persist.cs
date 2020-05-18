
using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
//using FirebirdSql.Data.FirebirdClient;

namespace FWSCRM.BD
{
    public abstract class PersistClass
    {
        #region Propriedades da Classe

        // Fields
        private static string _delete = "DELETE FROM {0} WHERE {1}";
        private static string _insert = "SET DATEFORMAT dmy INSERT INTO {0} ({1}) VALUES({2})";
        private static string _insertIdentity = "SET DATEFORMAT dmy INSERT INTO {0} ({1}) VALUES({2}) SELECT ISNULL(@@IDENTITY,0) ";
        private static string _select = "SELECT * FROM {0} WHERE {1}";
        private static string _selectAll = "SELECT * FROM {0}";
        private static string _selectDateFormat = " SET DATEFORMAT dmy ";
        private static string _selectMax = "SELECT coalesce( max( {1} )+1, 1 ) FROM {0} ";
        private static string _selectWhere = "SELECT * FROM {0} {1}";
        private static string _tagParam = "@";
        private static string _update = "SET DATEFORMAT dmy UPDATE {0} SET {1} WHERE {2}";
        private DbCommand cmd = null;
        private DbConnection con = null;
        private ConnectionStringSettings conSettings = null;
        private DbDataAdapter da = null;
        private DataSet ds = null;
        private DbDataReader dr = null;
        private DataTable dt = null;
        private DbProviderFactory factory = null;
        private DbTransaction trans = null;

        public string ConexaoPrincipal { get; set; }

        public string ConexaoDiretaExterna { get; set; }

        public string ProviderexternoDireto { get; set; }

        public bool LogFWSCRMAtivdado { get; set; }
        #endregion

        #region Configurações

        // Methods
        public PersistClass()
        {
            this.InicializeColumns(base.GetType());
            this.InicializeVariaveisPrivate();
        }

        public DbTransaction ObterTransacao()
        {
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.con.Open();
            //this.trans = this.con.BeginTransaction();
            return this.con.BeginTransaction();
        }

        public void FecharTransacao()
        {
            if ((!this.con.Equals(null)) && (this.con.State == ConnectionState.Open))
            {
                this.con.Close();
            }
        }
        private void LogAtivo()
        {
            try
            {
                LogFWSCRMAtivdado = ConfigurationManager.AppSettings["LogFWSCRM"].ToUpper() == "S";
            }
            catch
            {
                LogFWSCRMAtivdado = false;
            }
        }

        private void SetaLog()
        {
            Util.LOG.Instance.LogAtivo = LogFWSCRMAtivdado;
        }

        public virtual string getAppSettings()
        {
            //return "StringConexaoPadrao";
            return ConfigurationManager.AppSettings.GetKey(0);
        }

        protected string getAppSettingsDataBase()
        {
            return "DataBase";
        }

        private string getProviderName()
        {
            if (!String.IsNullOrEmpty(ProviderexternoDireto))
            {
                return ProviderexternoDireto;
            }
            else
            {
                string key = this.getAppSettings();
                this.conSettings = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings[key]];
                return this.conSettings.ProviderName;
            }
        }

        protected string getConnectionString()
        {
            if (!String.IsNullOrEmpty(ConexaoDiretaExterna))
            {
                return ConexaoDiretaExterna;
            }
            else
            {
                string key = this.getAppSettings();
                this.conSettings = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings[key]];
                this.ConexaoPrincipal = this.conSettings.ConnectionString;

                return this.conSettings.ConnectionString;
            }
        }

        protected DataBaseType getDataBase()
        {
            string key = this.getAppSettingsDataBase();
            string l_Chave = string.Empty;

            try
            {
                l_Chave = ConfigurationManager.AppSettings[key].ToUpper();
            }
            catch
            {
                l_Chave = null;
            }

            //return Util.Geral.Instance.DescricaoEnum(l_Chave);

            if (l_Chave != null)
            {
                if (l_Chave == "MYSQL")
                {
                    return DataBaseType.MYSQL;
                }
                if (l_Chave.ToUpper() == "FIREBIRD")
                {
                    return DataBaseType.FIREBIRD;
                }
                if (l_Chave.ToUpper() == "ACCESS")
                {
                    return DataBaseType.ACCESS;
                }
            }
            return DataBaseType.MSSQL;
        }

        protected void InicializeVariaveisPrivate()
        {
            LogAtivo();

            #region MSSQL

            if (this.getDataBase() == DataBaseType.MSSQL)
            {
                _selectDateFormat = " SET DATEFORMAT dmy ";
                _insertIdentity = "SET DATEFORMAT dmy INSERT INTO {0} ({1}) VALUES({2}) SELECT ISNULL(@@IDENTITY,0) ";
                _insert = "SET DATEFORMAT dmy INSERT INTO {0} ({1}) VALUES({2})";
                _update = "SET DATEFORMAT dmy UPDATE {0} SET {1} WHERE {2}";
                _delete = "DELETE FROM {0} WHERE {1}";
                _selectAll = _selectDateFormat + " SELECT * FROM {0}";
                _select = _selectDateFormat + "SELECT * FROM {0} WHERE {1}";
                _selectWhere = _selectDateFormat + "SELECT * FROM {0} {1}";
                _selectMax = "SELECT isnull( max( {1} ), 0 )+1 FROM {0} ";
                _tagParam = "@";
            }

            #endregion

            #region MYSQL

            if (this.getDataBase() == DataBaseType.MYSQL)
            {
                _selectDateFormat = "";
                _insertIdentity = " INSERT INTO {0} ({1}) VALUES({2})";
                _insert = "INSERT INTO {0} ({1}) VALUES({2})";
                _update = "UPDATE {0} SET {1} WHERE {2}";
                _delete = "DELETE FROM {0} WHERE {1}";
                _selectAll = "SELECT * FROM {0}";
                _select = "SELECT * FROM {0} WHERE {1}";
                _selectWhere = "SELECT * FROM {0} {1}";
                _selectMax = "SELECT coalesce( max( {1} )+1, 1 ) FROM {0} ";
                _tagParam = "?";
            }

            #endregion

            #region FireBird

            if (this.getDataBase() == DataBaseType.FIREBIRD)
            {
                //SELECT GEN_ID ( <NomeGenerator>, 1) FROM RDB$DATABASE 
                _selectDateFormat = "";
                //_insertIdentity = " INSERT INTO {0} ({1}) VALUES({2})";
                _insertIdentity = "SET DATEFORMAT dmy INSERT INTO {0} ({1}) VALUES({2}) SELECT GEN_ID({2}, 1) FROM RDB$DATABASE";
                _insert = "INSERT INTO {0} ({1}) VALUES({2})";
                _update = "UPDATE {0} SET {1} WHERE {2}";
                _delete = "DELETE FROM {0} WHERE {1}";
                _selectAll = "SELECT * FROM {0}";
                _select = "SELECT * FROM {0} WHERE {1}";
                _selectWhere = "SELECT * FROM {0} {1}";
                _selectMax = "SELECT coalesce( max( {1} )+1, 1 ) FROM {0} ";
                _tagParam = "?";
            }

            #endregion
        }

        private string TrataErro(string erro)
        {
            if (erro.ToUpper().Contains("UNIQUE") || erro.ToUpper().Contains("DUPLICATE"))
            {
                if (erro.ToUpper().Contains("CANNOT INSERT DUPLICATE KEY") || erro.ToUpper().Contains("DUPLICATE"))
                {
                    return "J\x00e1 existe registro com estas caracter\x00edsticas.";
                }
                return erro;
            }
            if (erro.ToUpper().Contains("COLUMN REFERENCE") || erro.ToUpper().Contains("FOREIGN KEY CONSTRAINT FAILS"))
            {
                return "Existem registros dependentes.";
            }
            if (erro.ToUpper().Contains("DATA WOULD BE TRUNCATED"))
            {
                return "Um campo ultrapassou o limite de catacteres permitido.";
            }
            return erro;
        }

        public enum DataBaseType
        {
            [Description("MSSQL")]
            MSSQL,
            [Description("MYSQL")]
            MYSQL,
            [Description("FireBird")]
            FIREBIRD,
            [Description("Access")]
            ACCESS
        }

        public bool AbreConexao()
        {
            SetaLog();

            bool l_ConexcaoAberta = false;

            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();

            try
            {
                this.con.Open();
                l_ConexcaoAberta = true;
            }
            catch (DbException dbE)
            {
                Util.LOG.Instance.Ocorrencia = "Erro ao abrir conexão. Método 'AbreConexao()' , FWSCRM ";
                Util.LOG.Instance.RegistraLog(dbE);
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }

            return l_ConexcaoAberta;
        }

        #endregion

        #region Rotinas específicas SqlServer

        public DataTable getTabelas()
        {
            string sql = string.Empty;
            DataBaseType l_DataBase = getDataBase();
            if (l_DataBase == DataBaseType.MSSQL)
            {
                sql = "select ROW_NUMBER () OVER (ORDER BY id) as ID, name from sysobjects where type = 'U' and name != 'dtproperties' and name != 'sysdiagrams' order by name ";
            }
            return getDataTableSql(sql);
        }

        public DataTable getColunas(string coluna)
        {
            string sql = string.Empty;
            DataBaseType l_DataBase = getDataBase();
            if (l_DataBase == DataBaseType.MSSQL)
            {
                sql = " select syscolumns.name, syscolumns.length,  systypes.name as tipo, isnull((select  '1' from sysindexes, sysindexkeys " +
                      " where sysindexes.id = object_id('" + coluna + "') and   " +
                      "	  	  sysindexes.status & 2048 <> 0   and			" +
                      "	  	  sysindexes.id = sysindexkeys.id  and			" +
                      "	  	  sysindexes.indid = sysindexkeys.indid and		" +
                      "	  	  sysindexes.id = syscolumns.id and				" +
                      "	  	  sysindexkeys.colid = syscolumns.colid),0)PrimaryKey,		 " +
                      "	  	  syscolumns.isnullable, syscolumns.colstat[Identity]    " +
                      "	  	  from syscolumns, systypes                              " +
                      "	  	  where syscolumns.usertype >= 0 and                      " +
                      "	  	  syscolumns.id = object_id('" + coluna + "') and        " +
                      "	  	  syscolumns.xtype = systypes.xtype                      " +
                      "       and systypes.name <> 'sysname'" +
                      "	  	  order by  primarykey desc , syscolumns.colid                             ";
            }
            return getDataTableSql(sql);
        }

        public string criaBanco(string sLocalBanco, string sBanco)
        {
            string sql = string.Empty;

            DataBaseType l_DataBase = getDataBase();
            if (l_DataBase == DataBaseType.MSSQL)
            {
                if (sLocalBanco == string.Empty)
                {
                    //sLocalBanco = @"C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\";
                    sLocalBanco = @"D:\Arquivos de programas\Microsoft SQL Server\MSSQL.1\MSSQL\Data\";
                    //sLocalLog = @"C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\";
                    sLocalBanco = @"D:\Arquivos de programas\Microsoft SQL Server\MSSQL.1\MSSQL\Data\";
                }
                /* 
                if (sLocalLog == string.Empty)
                    //sLocalLog = @"C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\";
                    sLocalBanco = @"D:\Arquivos de programas\Microsoft SQL Server\MSSQL.1\MSSQL\Data\";
                 */
                sql = @"
                CREATE DATABASE [" + sBanco + @"] ON  PRIMARY 
                ( NAME = N'" + sBanco + @"', FILENAME = N'" + sLocalBanco + sBanco + @".mdf' , SIZE = 3072KB , FILEGROWTH = 1024KB )
                 LOG ON 
                ( NAME = N'" + sBanco + @"_log', FILENAME = N'" + sLocalBanco + sBanco + @"_log.ldf' , SIZE = 1024KB , FILEGROWTH = 10%)
                
                EXEC dbo.sp_dbcmptlevel @dbname=N'" + sBanco + @"', @new_cmptlevel=90
                ALTER DATABASE [" + sBanco + @"] SET ANSI_NULL_DEFAULT OFF 
                ALTER DATABASE [" + sBanco + @"] SET ANSI_NULLS OFF 
                ALTER DATABASE [" + sBanco + @"] SET ANSI_PADDING OFF 
                ALTER DATABASE [" + sBanco + @"] SET ANSI_WARNINGS OFF 
                ALTER DATABASE [" + sBanco + @"] SET ARITHABORT OFF 
                ALTER DATABASE [" + sBanco + @"] SET AUTO_CLOSE OFF 
                ALTER DATABASE [" + sBanco + @"] SET AUTO_CREATE_STATISTICS ON 
                ALTER DATABASE [" + sBanco + @"] SET AUTO_SHRINK OFF 
                ALTER DATABASE [" + sBanco + @"] SET AUTO_UPDATE_STATISTICS ON 
                ALTER DATABASE [" + sBanco + @"] SET CURSOR_CLOSE_ON_COMMIT OFF 
                ALTER DATABASE [" + sBanco + @"] SET CURSOR_DEFAULT  GLOBAL 
                ALTER DATABASE [" + sBanco + @"] SET CONCAT_NULL_YIELDS_NULL OFF 
                ALTER DATABASE [" + sBanco + @"] SET NUMERIC_ROUNDABORT OFF 
                ALTER DATABASE [" + sBanco + @"] SET QUOTED_IDENTIFIER OFF
                ALTER DATABASE [" + sBanco + @"] SET RECURSIVE_TRIGGERS OFF 
                ALTER DATABASE [" + sBanco + @"] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
                ALTER DATABASE [" + sBanco + @"] SET DATE_CORRELATION_OPTIMIZATION OFF 
                ALTER DATABASE [" + sBanco + @"] SET PARAMETERIZATION SIMPLE 
                ALTER DATABASE [" + sBanco + @"] SET  READ_WRITE 
                ALTER DATABASE [" + sBanco + @"] SET RECOVERY SIMPLE 
                ALTER DATABASE [" + sBanco + @"] SET  MULTI_USER 
                ALTER DATABASE [" + sBanco + @"] SET PAGE_VERIFY CHECKSUM  ";
                /* 
                USE [" + sBanco + @"]
                IF NOT EXISTS (SELECT name FROM sys.filegroups WHERE is_default=1 AND name = N'PRIMARY') ALTER DATABASE [" + sBanco + @"] MODIFY FILEGROUP [PRIMARY] DEFAULT";
                */
            }
            return sql;
        }

        public string GetTipo(string type)
        {
            /*
                Int32,		String,		DateTime,
                Boolean,    Byte,       Char,
                Decimal,    Double,     Single,
                Int64
            */
            string sql = string.Empty;
            DataBaseType l_DataBase = getDataBase();
            if (l_DataBase == DataBaseType.MSSQL)
            {
                switch (type)
                {
                    case "int":
                    case "smallint":
                        sql = "Int32";
                        break;
                    case "bigint":
                        sql = "Int64";
                        break;
                    case "varchar":
                    case "char":
                    case "nchar":
                        sql = "String";
                        break;
                    case "date":
                    case "datetime":
                        sql = "DateTime";
                        break;
                    case "numeric":
                    case "decimal":
                        sql = "Decimal";

                        break;
                }
            }
            return sql;
        }

        #endregion

        #region Tratamento de colunas

        public string getColumnDefaultValue(string a_Campo)
        {
            //Obtem os atributos referente ao campo passado.
            AttributeCollection l_AttributeCollection = TypeDescriptor.GetProperties(this)[a_Campo].Attributes;

            //Obtem o estrutura referente ao valor padrão
            DefaultValueAttribute l_DefaultValueAttribute = (DefaultValueAttribute)l_AttributeCollection[typeof(DefaultValueAttribute)];

            //Retorna valor padrão
            string l_ValorPadrao = string.Empty;

            if (l_DefaultValueAttribute == null)
            {
                return l_ValorPadrao;
            }
            else
            {
                return l_DefaultValueAttribute.Value.ToString();
            }

        }
        public string getColumnName(string field)
        {
            string column = string.Empty;
            foreach (PropertyInfo prop in base.GetType().GetProperties())
            {
                if (prop.Name == field)
                {
                    object[] cols = null;
                    cols = prop.GetCustomAttributes(true);

                    if (cols.Length == 1)
                    {
                        if (cols[0] is ColumnAttribute)
                        {
                            ColumnAttribute ca = cols[0] as ColumnAttribute;
                            column = ca.ColumnName;
                        }
                        else if (cols[0] is KeyAttribute)
                        {
                            KeyAttribute ca = cols[0] as KeyAttribute;
                            column = ca.KeyName;
                        }
                    }
                }
            }
            return column;
        }

        private string getColumns(Type type)
        {
            string columns = string.Empty;
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object[] cols = null;
                cols = prop.GetCustomAttributes(true);
                //if (cols.Length == 1)
                foreach (object col in cols)
                {
                    //if (cols[0] is ColumnAttribute)
                    if (col is ColumnAttribute)
                    {
                        //if (!(cols[0] as ColumnAttribute).Temp)
                        if (!(col as ColumnAttribute).Temp)
                        {
                            if (!this.ValueIsNull(prop))
                            {
                                //ColumnAttribute ca = cols[0] as ColumnAttribute;
                                ColumnAttribute ca = col as ColumnAttribute;
                                if (!ca.Inc)
                                {
                                    columns = columns + ca.ColumnName + ", ";
                                }
                            }
                        }
                    }
                    //else if (cols[0] is KeyAttribute)
                    else if (col is KeyAttribute)
                    {
                        //KeyAttribute ca = cols[0] as KeyAttribute;
                        KeyAttribute ca = col as KeyAttribute;
                        if (!ca.Inc)
                        {
                            columns = columns + ca.KeyName + ", ";
                        }
                    }
                }
            }
            return columns.Remove(columns.Length - 2);
        }

        /// <summary>
        /// Obetem as colunas e seta seus valores
        /// </summary>
        /// <param name="type">Tipo(Calsse) que tem as propriedades(Campos) </param>
        /// <param name="InsertUpdade">True, traz e preenche apenas as colunas pertencente a tabela para serem atualizadas no banco</param>
        /// <returns>Classe com as colunas preenchidas</returns>
        private string getColumns(Type type, bool InsertUpdade)
        {
            string columns = string.Empty;
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object[] cols = null;
                cols = prop.GetCustomAttributes(true);
                if (cols.Length == 1)
                {
                    if (cols[0] is ColumnAttribute)
                    {
                        if (InsertUpdade)
                        {
                            if (!this.ValueIsNull(prop))
                            {
                                ColumnAttribute ca = cols[0] as ColumnAttribute;
                                if ((!ca.Inc) && (!ca.Temp))
                                {
                                    columns = columns + ca.ColumnName + ", ";
                                }
                            }
                        }
                    }
                    else if (cols[0] is KeyAttribute)
                    {
                        KeyAttribute ca = cols[0] as KeyAttribute;
                        if (!ca.Inc)
                        {
                            columns = columns + ca.KeyName + ", ";
                        }
                    }
                }
            }
            return columns.Remove(columns.Length - 2);
        }

        public string[] GetColumns()
        {
            return this.getColumns(base.GetType()).Split(new char[] { char.Parse(",") });
        }

        private string getColumnsParamInsert(Type type)
        {
            string columns = string.Empty;
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object[] cols = null;
                cols = prop.GetCustomAttributes(true);
                //if (cols.Length == 1)
                foreach (object col in cols)
                {
                    //if (cols[0] is ColumnAttribute)
                    if (col is ColumnAttribute)
                    {
                        //ColumnAttribute ca = cols[0] as ColumnAttribute;
                        ColumnAttribute ca = col as ColumnAttribute;
                        if ((!ca.Inc) && (!ca.Temp))
                        {
                            if (!this.ValueIsNull(prop))
                            {
                                columns = columns + _tagParam + ca.ColumnName + ", ";
                            }
                        }
                    }
                    //else if (cols[0] is KeyAttribute)
                    else if (col is KeyAttribute)
                    {
                        //KeyAttribute ca = cols[0] as KeyAttribute;
                        KeyAttribute ca = col as KeyAttribute;
                        if (!ca.Inc)
                        {
                            columns = columns + _tagParam + ca.KeyName + ", ";
                        }
                    }
                }
            }
            return columns.Remove(columns.Length - 2);
        }

        private string getColumnsParamSequencial(Type type)
        {
            string key = string.Empty;
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object[] cols = prop.GetCustomAttributes(true);
                //if ((cols.Length == 1) && (cols[0] is KeyAttribute))
                foreach (object col in cols)
                {

                    if (col is ColumnAttribute)
                    {
                        ColumnAttribute ca = col as ColumnAttribute;
                        if ((!ca.Temp) && (ca.Inc))
                        {
                            if (!this.ValueIsNull(prop))
                            {
                                key = ca.ColumnName + " = " + _tagParam + ca.ColumnName + " AND "; 
                            }
                        }
                    }
                }
            }
            return key.Remove(key.Length - 4);
        }

        private string getColumnsParamUpdateSequencial(Type type)
        {
            string columns = string.Empty;
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object[] cols = prop.GetCustomAttributes(true);
                foreach (object l_TipoColuna in cols)
                {
                    string l_Aux;
                    if (l_TipoColuna is ColumnAttribute)
                    {
                        ColumnAttribute ca = l_TipoColuna as ColumnAttribute;
                        if ((!ca.Temp) && (!ca.Inc) )
                        {
                            l_Aux = columns;
                            columns = l_Aux + ca.ColumnName + " = " + _tagParam + ca.ColumnName + ", ";
                        }
                    }
                    else if (l_TipoColuna is KeyAttribute)
                    {
                        KeyAttribute ca = l_TipoColuna as KeyAttribute;
                        if (!ca.Inc)
                        {
                            l_Aux = columns;
                            columns = l_Aux + ca.KeyName + " = " + _tagParam + ca.KeyName + ", ";
                        }
                    }
                }
            }
            return columns.Remove(columns.Length - 2);
        }

        private string getColumnsParamKey(Type type)
        {
            string key = string.Empty;
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object[] cols = prop.GetCustomAttributes(true);
                //if ((cols.Length == 1) && (cols[0] is KeyAttribute))
                foreach (object col in cols)
                {
                    if (col is KeyAttribute)
                    {
                        //KeyAttribute ca = cols[0] as KeyAttribute;
                        KeyAttribute ca = col as KeyAttribute;
                        string l_Aux = key;
                        key = l_Aux + ca.KeyName + " = " + _tagParam + "_KEY_" + ca.KeyName + " AND ";
                    }
                }
            }
            return key.Remove(key.Length - 4);
        }

        private string getColumnsParamUpdate(Type type)
        {
            string columns = string.Empty;
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object[] cols = prop.GetCustomAttributes(true);
                foreach (object l_TipoColuna in cols)
                {
                    //if (cols.Length == 1)
                    //{
                    string l_Aux;
                    //if (cols[0] is ColumnAttribute)
                    if (l_TipoColuna is ColumnAttribute)
                    {
                        ColumnAttribute ca = l_TipoColuna as ColumnAttribute;//cols[0] as ColumnAttribute;
                        if ((!ca.Inc) && (!ca.Temp))
                        {
                            l_Aux = columns;
                            columns = l_Aux + ca.ColumnName + " = " + _tagParam + ca.ColumnName + ", ";
                        }
                    }
                    //else if (cols[3] is KeyAttribute)
                    else if (l_TipoColuna is KeyAttribute)
                    {
                        KeyAttribute ca = l_TipoColuna as KeyAttribute; //cols[0] as KeyAttribute;
                        if (!ca.Inc)
                        {
                            l_Aux = columns;
                            columns = l_Aux + ca.KeyName + " = " + _tagParam + ca.KeyName + ", ";
                        }
                    }
                    //}
                }
            }
            return columns.Remove(columns.Length - 2);
        }

        private string getFieldsValuesIsNotNull(Type type)
        {
            string columns = string.Empty;
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object[] cols = prop.GetCustomAttributes(true);
                //if (cols.Length == 1)
                foreach (object col in cols)
                {
                    string l_Aux;
                    //if (cols[0] is ColumnAttribute)
                    if (col is ColumnAttribute)
                    {
                        //ColumnAttribute ca = cols[0] as ColumnAttribute;
                        ColumnAttribute ca = col as ColumnAttribute;
                        if (!this.ValueIsNull(prop))
                        {
                            if (((ca.CType == ColumnType.Single) || (ca.CType == ColumnType.Double)) || (ca.CType == ColumnType.Decimal))
                            {
                                l_Aux = columns;
                                columns = l_Aux + ca.ColumnName + " = '" + prop.GetValue(this, null).ToString().Replace(",", ".") + "', ";
                            }
                            else if ((ca.CType == ColumnType.DateTime) && (this.getDataBase() == DataBaseType.MYSQL))
                            {
                                l_Aux = columns;
                                columns = l_Aux + ca.ColumnName + " = '" + Convert.ToDateTime(prop.GetValue(this, null)).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                            }
                            else
                            {
                                l_Aux = columns;
                                columns = l_Aux + ca.ColumnName + " = '" + Seguranca.SeguracaSql(prop.GetValue(this, null).ToString()) + "', ";
                            }
                        }
                        else
                        {
                            columns = columns + ca.ColumnName + " = null , ";
                        }
                    }
                    //else if (cols[0] is KeyAttribute)
                    else if (col is KeyAttribute)
                    {
                        //KeyAttribute ca = cols[0] as KeyAttribute;
                        KeyAttribute ca = col as KeyAttribute;
                        if (!ca.Inc)
                        {
                            if (prop.PropertyType.FullName == "System.Int32[]")
                            {
                                l_Aux = columns;
                                columns = l_Aux + ca.KeyName + " = '" + Seguranca.SeguracaSql(((int[])prop.GetValue(this, null))[1].ToString()) + "', ";
                            }
                            else if ((ca.CType == ColumnType.DateTime) && (this.getDataBase() == DataBaseType.MYSQL))
                            {
                                l_Aux = columns;
                                columns = l_Aux + ca.KeyName + " = '" + Convert.ToDateTime(prop.GetValue(this, null)).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                            }
                            else
                            {
                                l_Aux = columns;
                                columns = l_Aux + ca.KeyName + " = '" + Seguranca.SeguracaSql(prop.GetValue(this, null).ToString()) + "', ";
                            }
                        }
                    }
                }
            }
            return columns.Remove(columns.Length - 2);
        }

        private string getFieldsValuesIsNotNullSelect(Type type)
        {
            string columns = string.Empty;
            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (!this.ValueIsNull(prop))
                {
                    object[] cols = prop.GetCustomAttributes(true);
                    if (cols.Length == 1)
                    {
                        object l_Aux;
                        if (cols[0] is KeyAttribute)
                        {
                            KeyAttribute ca = cols[0] as KeyAttribute;
                            l_Aux = columns;
                            columns = string.Concat(new object[] { l_Aux, ca.KeyName, " = '", prop.GetValue(this, null), "' AND " });
                        }
                        else
                        {
                            ColumnAttribute ca = cols[0] as ColumnAttribute;
                            l_Aux = columns;
                            columns = string.Concat(new object[] { l_Aux, ca.ColumnName, " = '", prop.GetValue(this, null), "' AND " });
                        }
                    }
                }
            }
            return columns.Remove(columns.Length - 4);
        }

        public string[] getKeys()
        {
            Type type = base.GetType();
            ArrayList keyList = new ArrayList();
            foreach (PropertyInfo p in type.GetProperties())
            {
                object[] field = p.GetCustomAttributes(typeof(KeyAttribute), true);
                if (field.Length >= 1)
                {
                    KeyAttribute c = field[0] as KeyAttribute;
                    keyList.Add(c.KeyName);
                }
            }
            string[] key = new string[keyList.Count];
            for (int i = 0; i < keyList.Count; i++)
            {
                key[i] = (string)keyList[i];
            }
            return key;
        }

        private string getNameColumns(Type type)
        {
            string columns = string.Empty;
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object[] cols = null;
                cols = prop.GetCustomAttributes(true);
                //if (cols.Length == 1)
                foreach (object col in cols)
                {
                    //if (cols[0] is ColumnAttribute)
                    if (col is ColumnAttribute)
                    {
                        //ColumnAttribute ca = cols[0] as ColumnAttribute;

                        ColumnAttribute ca = col as ColumnAttribute;
                        columns = columns + ca.ColumnName + ",";
                    }
                    else if (cols[0] is KeyAttribute)
                    {
                        KeyAttribute ca = cols[0] as KeyAttribute;
                        columns = columns + ca.KeyName + ",";
                    }
                }
            }
            return columns.Remove(columns.Length - 1);
        }

        public string[] GetNameColumns()
        {
            return this.getNameColumns(base.GetType()).Split(new char[] { char.Parse(",") });
        }

        private string getTableName(Type type)
        {
            return ((TableAttribute)type.GetCustomAttributes(typeof(TableAttribute), true)[0]).TableName;
        }

        public string GetTableName()
        {
            return this.getTableName(base.GetType());
        }

        private DbType getTypeAttribute(ColumnType coltype)
        {
            switch (coltype)
            {
                case ColumnType.Int32:
                    return DbType.Int32;

                case ColumnType.DateTime:
                    return DbType.DateTime;

                case ColumnType.Boolean:
                    return DbType.Boolean;

                case ColumnType.Byte:
                    return DbType.Byte;

                case ColumnType.Decimal:
                    return DbType.Decimal;

                case ColumnType.Double:
                    return DbType.Double;

                case ColumnType.Single:
                    return DbType.Single;

                case ColumnType.Int64:
                    return DbType.Int64;

                case ColumnType.TimeSpan:
                    return DbType.Time;

                case ColumnType.Image:
                    return DbType.Binary;
            }
            return DbType.String;
        }

        private string getValues()
        {
            string values = string.Empty;
            foreach (PropertyInfo field in base.GetType().GetProperties())
            {
                object[] cols = field.GetCustomAttributes(true);
                //if (cols.Length == 1)
                foreach (object col in cols)
                {
                    //if (cols[0] is KeyAttribute)
                    if (col is KeyAttribute)
                    {
                        //KeyAttribute ca = cols[0] as KeyAttribute;
                        KeyAttribute ca = col as KeyAttribute;
                        if (!ca.Inc)
                        {
                            if (field.PropertyType.FullName == "System.Int32[]")
                            {
                                values = values + "'" + Seguranca.SeguracaSql(((int[])field.GetValue(this, null))[0].ToString()) + "', ";
                            }
                            else
                            {
                                values = values + "'" + Seguranca.SeguracaSql(field.GetValue(this, null).ToString()) + "', ";
                            }
                        }
                    }
                    else
                    {
                        values = values + "'" + Seguranca.SeguracaSql(field.GetValue(this, null).ToString()) + "', ";
                    }
                }
            }
            return values.Remove(values.Length - 2);
        }

        private string getValuesIsNotNull()
        {
            string values = string.Empty;
            foreach (PropertyInfo field in base.GetType().GetProperties())
            {
                object[] cols = field.GetCustomAttributes(true);
                //if (cols.Length == 1)
                foreach (object col in cols)
                {
                    //if (cols[0] is KeyAttribute)
                    if (col is KeyAttribute)
                    {
                        //KeyAttribute ca = cols[0] as KeyAttribute;
                        KeyAttribute ca = col as KeyAttribute;
                        if (!ca.Inc)
                        {
                            if (field.PropertyType.FullName == "System.Int32[]")
                            {
                                values = values + "'" + Seguranca.SeguracaSql(((int[])field.GetValue(this, null))[0].ToString()) + "', ";
                            }
                            else if ((ca.CType == ColumnType.DateTime) && (this.getDataBase() == DataBaseType.MYSQL))
                            {
                                values = values + "'" + Convert.ToDateTime(field.GetValue(this, null)).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                            }
                            else
                            {
                                values = values + "'" + Seguranca.SeguracaSql(field.GetValue(this, null).ToString()) + "', ";
                            }
                        }
                    }
                    else
                    {
                        //ColumnAttribute ca = cols[0] as ColumnAttribute;
                        ColumnAttribute ca = col as ColumnAttribute;
                        if ((field.GetValue(this, null) != null) && !this.ValueIsNull(field))
                        {
                            if (((ca.CType == ColumnType.Single) || (ca.CType == ColumnType.Double)) || (ca.CType == ColumnType.Decimal))
                            {
                                values = values + "'" + field.GetValue(this, null).ToString().Replace(",", ".") + "', ";
                            }
                            else if ((ca.CType == ColumnType.DateTime) && (this.getDataBase() == DataBaseType.MYSQL))
                            {
                                values = values + "'" + Convert.ToDateTime(field.GetValue(this, null)).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                            }
                            else
                            {
                                values = values + "'" + Seguranca.SeguracaSql(field.GetValue(this, null).ToString()) + "', ";
                            }
                        }
                    }
                }
            }
            return values.Remove(values.Length - 2);
        }

        private void InicializeColumns(Type type)
        {
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object[] cols = null;
                cols = prop.GetCustomAttributes(true);
                if (prop.PropertyType.FullName == "System.Decimal")
                {
                    // prop.SetValue(this, -79228162514264337593543950335M, null);
                    prop.SetValue(this, 0M, null);
                }
                else if (prop.PropertyType.FullName == "System.Double")
                {
                    // prop.SetValue(this, -1.7976931348623157E+308, null);
                    prop.SetValue(this, 0, null);
                }
                else if (prop.PropertyType.FullName == "System.Int32")
                {
                    // prop.SetValue(this, -2147483648, null);
                    prop.SetValue(this, 0, null);
                }
                else if (prop.PropertyType.FullName == "System.Date")
                {
                    prop.SetValue(this, DateTime.MinValue, null);
                }
                else if (prop.PropertyType.FullName == "System.Byte")
                {
                    prop.SetValue(this, (byte)0, null);
                }
                else
                {
                    //prop.SetValue(this, null, null);
                }
            }
        }

        private void setParameterValueInsert(Type type, ref DbCommand cmd)
        {
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object[] cols = prop.GetCustomAttributes(true);
                //if (cols.Length == 1)
                foreach (object col in cols)
                {
                    DbParameter param = null;
                    //if (cols[0] is ColumnAttribute)
                    if (col is ColumnAttribute)
                    {
                        if (!this.ValueIsNull(prop))
                        {
                            //ColumnAttribute ca = cols[0] as ColumnAttribute;
                            ColumnAttribute ca = col as ColumnAttribute;
                            if ((!ca.Inc) && (!ca.Temp))
                            {
                                param = cmd.CreateParameter();
                                param.ParameterName = _tagParam + ca.ColumnName;
                                param.DbType = this.getTypeAttribute(ca.CType);
                                if ((prop.GetValue(this, null) == DBNull.Value) || this.ValueIsNull(prop))
                                {
                                    param.Value = DBNull.Value;
                                }
                                else
                                {
                                    param.Value = prop.GetValue(this, null);
                                }
                                cmd.Parameters.Add(param);
                            }
                        }
                    }
                    //else if (cols[0] is KeyAttribute)
                    else if (col is KeyAttribute)
                    {
                        //KeyAttribute ca = cols[0] as KeyAttribute;
                        KeyAttribute ca = col as KeyAttribute;
                        if (!ca.Inc)
                        {
                            param = cmd.CreateParameter();
                            param.ParameterName = _tagParam + ca.KeyName;
                            param.DbType = this.getTypeAttribute(ca.CType);
                            if (prop.PropertyType.FullName == "System.Int32[]")
                            {
                                param.Value = ((int[])prop.GetValue(this, null))[0];
                            }
                            else
                            {
                                param.Value = prop.GetValue(this, null);
                            }
                            cmd.Parameters.Add(param);

                        }
                    }
                }
            }

        }

        private void setParameterValueKey(Type type, ref DbCommand cmd)
        {
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object[] cols = prop.GetCustomAttributes(true);
                //if ((cols.Length == 1) && (cols[0] is KeyAttribute))
                foreach (object col in cols)
                {
                    if (col is KeyAttribute)
                    {
                        KeyAttribute ca = col as KeyAttribute;//cols[0] as KeyAttribute;
                        DbParameter param = cmd.CreateParameter();
                        param.ParameterName = _tagParam + "_KEY_" + ca.KeyName;
                        param.DbType = this.getTypeAttribute(ca.CType);
                        if (prop.PropertyType.FullName == "System.Int32[]")
                        {
                            param.Value = ((int[])prop.GetValue(this, null))[0];
                        }
                        else if (prop.PropertyType.FullName == "System.DateTime[]")
                        {
                            param.Value = ((DateTime[])prop.GetValue(this, null))[0];
                        }
                        else
                        {
                            param.Value = prop.GetValue(this, null);
                        }
                        cmd.Parameters.Add(param);
                    }
                }
            }
        }

        private void setParameterValueUpdate(Type type, ref DbCommand cmd)
        {
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object[] cols = prop.GetCustomAttributes(true);
                //if (cols.Length == 1)
                foreach (object col in cols)
                {
                    DbParameter param = null;
                    //if (cols[0] is ColumnAttribute)
                    if (col is ColumnAttribute)
                    {
                        //ColumnAttribute ca = cols[0] as ColumnAttribute;
                        ColumnAttribute ca = col as ColumnAttribute;
                        if ((!ca.Inc) && (!ca.Temp))
                        {
                            param = cmd.CreateParameter();
                            param.ParameterName = _tagParam + ca.ColumnName;
                            param.DbType = this.getTypeAttribute(ca.CType);
                            if ((prop.GetValue(this, null) == DBNull.Value) || this.ValueIsNull(prop))
                            {
                                param.Value = DBNull.Value;
                            }
                            else
                            {
                                param.Value = prop.GetValue(this, null);
                            }
                            param.Value = TrataValor(prop, param);
                            cmd.Parameters.Add(param);
                        }
                    }
                    //else if (cols[0] is KeyAttribute)
                    else if (col is KeyAttribute)
                    {
                        //KeyAttribute ca = cols[0] as KeyAttribute;
                        KeyAttribute ca = col as KeyAttribute;
                        if (!ca.Inc)
                        {
                            param = cmd.CreateParameter();
                            param.ParameterName = _tagParam + ca.KeyName;
                            param.DbType = this.getTypeAttribute(ca.CType);
                            if (prop.PropertyType.FullName == "System.Int32[]")
                            {
                                param.Value = ((int[])prop.GetValue(this, null))[1];
                            }
                            else
                            {
                                param.Value = prop.GetValue(this, null);
                            }
                            cmd.Parameters.Add(param);
                        }
                    }
                }
            }

        }

        private void setParameterValueKeySequencial(Type type, ref DbCommand cmd)
        {
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object[] cols = prop.GetCustomAttributes(true);
                
                foreach (object col in cols)
                {
                    if (col is ColumnAttribute)
                    {
                        //ColumnAttribute ca = cols[0] as ColumnAttribute;
                        ColumnAttribute ca = col as ColumnAttribute;
                        if ((!ca.Temp) && (ca.Inc))
                        {
                            DbParameter param = cmd.CreateParameter();
                            param.ParameterName = _tagParam + ca.ColumnName;
                            param.DbType = this.getTypeAttribute(ca.CType);
                            if (prop.PropertyType.FullName == "System.Int32[]")
                            {
                                param.Value = ((int[])prop.GetValue(this, null))[0];
                            }
                            else if (prop.PropertyType.FullName == "System.DateTime[]")
                            {
                                param.Value = ((DateTime[])prop.GetValue(this, null))[0];
                            }
                            else
                            {
                                param.Value = prop.GetValue(this, null);
                            }
                            cmd.Parameters.Add(param);
                        }
                    }
                }
            }
        }

        private void setParameterValueUpdateSequencial(Type type, ref DbCommand cmd)
        {
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object[] cols = prop.GetCustomAttributes(true);
                //if (cols.Length == 1)
                foreach (object col in cols)
                {
                    DbParameter param = null;
                    //if (cols[0] is ColumnAttribute)
                    if (col is ColumnAttribute)
                    {
                        //ColumnAttribute ca = cols[0] as ColumnAttribute;
                        ColumnAttribute ca = col as ColumnAttribute;
                        if ((!ca.Temp) && (!ca.Inc))
                        {
                            param = cmd.CreateParameter();
                            param.ParameterName = _tagParam + ca.ColumnName;
                            param.DbType = this.getTypeAttribute(ca.CType);
                            if ((prop.GetValue(this, null) == DBNull.Value) || this.ValueIsNull(prop))
                            {
                                param.Value = DBNull.Value;
                            }
                            else
                            {
                                param.Value = prop.GetValue(this, null);
                            }
                            param.Value = TrataValor(prop, param);
                            cmd.Parameters.Add(param);
                        }
                    }
                    //else if (cols[0] is KeyAttribute)
                    else if (col is KeyAttribute)
                    {
                        //KeyAttribute ca = cols[0] as KeyAttribute;
                        KeyAttribute ca = col as KeyAttribute;
                        if (!ca.Inc)
                        {
                            param = cmd.CreateParameter();
                            param.ParameterName = _tagParam + ca.KeyName;
                            param.DbType = this.getTypeAttribute(ca.CType);
                            if (prop.PropertyType.FullName == "System.Int32[]")
                            {
                                param.Value = ((int[])prop.GetValue(this, null))[1];
                            }
                            else
                            {
                                param.Value = prop.GetValue(this, null);
                            }
                            cmd.Parameters.Add(param);
                        }
                    }
                }
            }

        }

        private object TrataValor(PropertyInfo a_Prop, DbParameter a_Param)
        {
            object l_Valor = a_Param.Value;
            if (a_Param.DbType == DbType.Decimal)
            {
                if ((Decimal)a_Param.Value == Decimal.MinValue)
                {
                    a_Param.Value = decimal.Zero;
                }
            }
            return l_Valor;
        }

        public DbParameter ObterParametro(String NomeParametro, DbType TipoParametro, object Valor)
        {
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());

            DbParameter Parametro;
            Parametro = this.factory.CreateParameter();
            Parametro.ParameterName = NomeParametro;
            Parametro.DbType = TipoParametro;
            Parametro.Value = Valor;

            return Parametro;
        }

        public List<DbParameter> ObterParametros(Type type)
        {
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());

            List<DbParameter> Parametros = new List<DbParameter>();
            DbParameter Parametro;

            foreach (PropertyInfo prop in type.GetProperties())
            {
                object[] cols = prop.GetCustomAttributes(true);
                //if (cols.Length == 1)
                foreach (object col in cols)
                {
                    //if (cols[0] is ColumnAttribute)
                    if (col is ColumnAttribute)
                    {
                        //ColumnAttribute ca = cols[0] as ColumnAttribute;
                        ColumnAttribute ca = col as ColumnAttribute;
                        if (!ca.Temp)
                        {
                            Parametro = this.factory.CreateParameter();
                            Parametro.ParameterName = _tagParam + ca.ColumnName;
                            Parametro.DbType = this.getTypeAttribute(ca.CType);

                            if ((prop.GetValue(this, null) == DBNull.Value) || this.ValueIsNull(prop))
                            {
                                Parametro.Value = DBNull.Value;
                            }
                            else
                            {
                                Parametro.Value = prop.GetValue(this, null);
                            }

                            Parametros.Add(Parametro);
                        }
                    }
                }
            }
            return Parametros;
        }

        private bool ValueIsNull(PropertyInfo prop)
        {
            return ((prop.GetValue(this, null) == null) || (((prop.PropertyType.FullName == "System.DateTime") && (((prop.GetValue(this, null).ToString() == "01/01/0001 00:00:00") || (prop.GetValue(this, null).ToString() == "1/1/0001 00:00:00")) || (prop.GetValue(this, null).ToString() == DateTime.MinValue.ToString()))) || (((prop.PropertyType.FullName == "System.Int32") && (prop.GetValue(this, null).ToString() == 2147483648.ToString())) || (((prop.PropertyType.FullName == "System.Double") && (prop.GetValue(this, null).ToString() == 1.7976931348623157E+308.ToString())) || ((prop.PropertyType.FullName == "System.Decimal") && (prop.GetValue(this, null).ToString() == 79228162514264337593543950335M.ToString()))))));
        }

        #endregion

        #region Tratamento de procedures

        public object ExecStoredProc(string sql, DbParameter[] a_Parametros, DbTransaction a_Transacao) //, DbConnection a_conexao)
        {
            object l_Result;

            if (this.factory == null)
            {
                this.factory = DbProviderFactories.GetFactory(this.getProviderName());
                this.con = this.factory.CreateConnection();
            }
            if (string.IsNullOrEmpty(this.con.ConnectionString))
            {
                this.con.ConnectionString = this.getConnectionString();
            }
            this.cmd = this.con.CreateCommand();

            this.cmd.CommandText = sql;
            this.cmd.CommandType = CommandType.StoredProcedure;
            this.cmd.Connection = this.con;

            this.cmd.Parameters.AddRange(a_Parametros);
            this.cmd.Transaction = a_Transacao;

            try
            {
                if (this.con.State != ConnectionState.Open)
                {
                    this.con.Open();
                }

                l_Result = this.cmd.ExecuteNonQuery();// .ExecuteScalar();
                // l_Result = this.cmd.ExecuteScalar();
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                //this.con.Close();
            }
            return l_Result;
        }

        public object ExecStoredProc(string sql, DbParameter[] a_Parametros)
        {
            object l_Result;

            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();

            this.cmd = this.con.CreateCommand();

            this.cmd.CommandText = sql;
            this.cmd.CommandType = CommandType.StoredProcedure;
            this.cmd.Connection = this.con;

            this.cmd.Parameters.AddRange(a_Parametros);


            try
            {
                this.con.Open();

                l_Result = this.cmd.ExecuteScalar();
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                this.con.Close();
            }
            return l_Result;
        }

        public object ExecStoredProc(string sql)
        {
            object l_Result;
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = sql;
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = this.con;
            try
            {
                this.con.Open();
                l_Result = this.cmd.ExecuteScalar();
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                this.con.Close();
            }
            return l_Result;
        }

        public object ExecStoredProc(string sql, int timout)
        {
            object l_Result;
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = sql;
            this.cmd.CommandType = CommandType.Text;
            this.cmd.CommandTimeout = timout;
            this.cmd.Connection = this.con;
            try
            {
                this.con.Open();
                l_Result = this.cmd.ExecuteScalar();
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                this.con.Close();
            }
            return l_Result;
        }


        public List<T> ExecReaderStoredProc<T>(string sql, DbParameter[] a_Parametros, DbTransaction a_Transacao) //, DbConnection a_conexao)
        {
            List<T> l_Result;
            
            ////Obtem o tipo de objeto que esta sendo utilizado
            //Type objectType = typeof(T);

            //// Cria uma instancia de T
            //T objectInstance = Activator.CreateInstance<T>();

            if (this.factory == null)
            {
                this.factory = DbProviderFactories.GetFactory(this.getProviderName());
                this.con = this.factory.CreateConnection();
            }
            if (string.IsNullOrEmpty(this.con.ConnectionString))
            {
                this.con.ConnectionString = this.getConnectionString();
            }
            this.cmd = this.con.CreateCommand();

            this.cmd.CommandText = sql;
            this.cmd.CommandType = CommandType.StoredProcedure;
            this.cmd.Connection = this.con;

            if (a_Parametros != null)
            {
                this.cmd.Parameters.AddRange(a_Parametros);
            }

            if (a_Transacao != null)
            {
                this.cmd.Transaction = a_Transacao;
            }
            
            try
            {
                if (this.con.State != ConnectionState.Open)
                {
                    this.con.Open();
                }
                
                dr = this.cmd.ExecuteReader();
                l_Result = DataReaderMapToList<T>(dr);
                // l_Result = DataReaderMapToList<T>(this.cmd.ExecuteReader());
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao executar Stored Procedure: " + dbE.Message);
            }
            finally
            {
                //this.con.Close();
            }
            return l_Result;
        }

        public List<T> ExecReaderStoredProc<T>(string sql, DbParameter[] a_Parametros) //, DbConnection a_conexao)
        {
            List<T> l_Result;

            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = sql;
            this.cmd.CommandType = CommandType.StoredProcedure;
            this.cmd.Connection = this.con;

            if (a_Parametros != null)
            {
                this.cmd.Parameters.AddRange(a_Parametros);
            }

            try
            {
                this.con.Open();
                dr = this.cmd.ExecuteReader();
                // l_Result = DataReaderMapToList<T>(dr);
                l_Result = getDataReaderList<T>(dr);
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao executar Stored Procedure: " + dbE.Message);
            }
            finally
            {
                //this.con.Close();
            }
            return l_Result;
        }


        public List<T> ExecMultReaderStoredProc<T>(string sql, DbParameter[] a_Parametros, DbTransaction a_Transacao) //, DbConnection a_conexao)
        {
            List<T> l_Result;

            if (this.factory == null)
            {
                this.factory = DbProviderFactories.GetFactory(this.getProviderName());
                this.con = this.factory.CreateConnection();
            }
            if (string.IsNullOrEmpty(this.con.ConnectionString))
            {
                this.con.ConnectionString = this.getConnectionString();
            }
            this.cmd = this.con.CreateCommand();

            this.cmd.CommandText = sql;
            this.cmd.CommandType = CommandType.StoredProcedure;
            this.cmd.Connection = this.con;

            if (a_Parametros != null)
            {
                this.cmd.Parameters.AddRange(a_Parametros);
            }

            if (a_Transacao != null)
            {
                this.cmd.Transaction = a_Transacao;
            }

            try
            {
                if (this.con.State != ConnectionState.Open)
                {
                    this.con.Open();
                }

                dr = this.cmd.ExecuteReader();
                l_Result = DataReaderMapToList<T>(dr);
                // l_Result = DataReaderMapToList<T>(this.cmd.ExecuteReader());
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao executar Stored Procedure: " + dbE.Message);
            }
            finally
            {
                //this.con.Close();
            }
            return l_Result;
        }

    #endregion

    #region Execução geral

    public int Execute(string sql, DbTransaction a_Transacao)
        {
            int l_Retorno = 0;
            if (a_Transacao == null)
            {
                this.factory = DbProviderFactories.GetFactory(this.getProviderName());
                this.con = this.factory.CreateConnection();
                this.con.ConnectionString = this.getConnectionString();
            }
            else
            {
                this.cmd = this.con.CreateCommand();
                this.cmd.Transaction = a_Transacao;
            }

            this.cmd.CommandText = sql;
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = this.con;
            try
            {
                if (this.con.State == ConnectionState.Closed)
                {
                    this.con.Open();
                }
                l_Retorno = this.cmd.ExecuteNonQuery();
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                if (a_Transacao == null)
                {
                    this.con.Close();
                }
            }
            return l_Retorno;
        }

        public bool Execute(string sql)
        {
            bool l_Result;
            if (this.trans == null)
            {
                this.factory = DbProviderFactories.GetFactory(this.getProviderName());
                this.con = this.factory.CreateConnection();
                this.con.ConnectionString = this.getConnectionString();
            }
            else
            {
                this.cmd.Transaction = this.trans;
            }
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = sql;
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = this.con;
            try
            {
                this.con.Open();
                int l_Retorno = this.cmd.ExecuteNonQuery();
                if (l_Retorno >= 1)
                {
                    return true;
                }
                l_Result = false;
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                if (this.trans == null)
                {
                    this.con.Close();
                }
            }
            return l_Result;
        }

        public bool Execute(string sql, DbConnection con, DbTransaction trans)
        {
            bool l_Result;
            this.cmd = con.CreateCommand();
            this.cmd.CommandText = _selectDateFormat + sql;
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = con;
            this.cmd.Transaction = trans;
            try
            {
                int l_Retorno = this.cmd.ExecuteNonQuery();
                if (l_Retorno >= 1)
                {
                    return true;
                }
                l_Result = false;
            }
            catch (DbException dbE)
            {
                throw new ArgumentException(this.TrataErro(dbE.Message));
            }
            return l_Result;
        }

        #endregion

        #region Delete, Insert e Update

        public virtual bool Delete()
        {
            bool l_Result;
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = string.Format(_delete, this.getTableName(base.GetType()), this.getColumnsParamKey(base.GetType()));
            this.setParameterValueKey(base.GetType(), ref this.cmd);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = this.con;
            try
            {
                this.con.Open();
                int l_Retorno = this.cmd.ExecuteNonQuery();
                if (l_Retorno >= 1)
                {
                    return true;
                }
                l_Result = false;
            }
            catch (DbException dbE)
            {
                throw new ArgumentException((dbE.Message == this.TrataErro(dbE.Message)) ? ("Problema ao excluir dados, contacte o suporte. Mensagem:" + dbE.Message) : this.TrataErro(dbE.Message));
            }
            finally
            {
                if (this.trans == null)
                {
                    this.con.Close();
                }
            }
            return l_Result;
        }

        public virtual bool Delete(DbConnection con, DbTransaction trans)
        {
            bool l_Result;
            this.cmd = con.CreateCommand();
            this.cmd.CommandText = string.Format(_delete, this.getTableName(base.GetType()), this.getColumnsParamKey(base.GetType()));
            this.setParameterValueKey(base.GetType(), ref this.cmd);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = con;
            this.cmd.Transaction = trans;
            try
            {
                int l_Retorno = this.cmd.ExecuteNonQuery();
                if (l_Retorno >= 1)
                {
                    return true;
                }
                l_Result = false;
            }
            catch (DbException dbE)
            {
                throw new ArgumentException((dbE.Message == this.TrataErro(dbE.Message)) ? ("Problema ao excluir dados, contacte o suporte. Mensagem:" + dbE.Message) : this.TrataErro(dbE.Message));
            }
            return l_Result;
        }

        public virtual bool Insert()
        {
            bool l_Result;
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = string.Format(_insert, this.getTableName(base.GetType()), this.getColumns(base.GetType()), this.getColumnsParamInsert(base.GetType()));
            this.setParameterValueInsert(base.GetType(), ref this.cmd);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = this.con;
            try
            {
                this.con.Open();
                int l_Retorno = this.cmd.ExecuteNonQuery();
                if (l_Retorno >= 1)
                {
                    return true;
                }
                l_Result = false;
            }
            catch (DbException dbE)
            {
                throw new ArgumentException((dbE.Message == this.TrataErro(dbE.Message)) ? ("Problema ao inserir dados, contacte o suporte. Mensagem:" + dbE.Message) : this.TrataErro(dbE.Message));
            }
            finally
            {
                this.cmd.Dispose();
                this.con.Close();
            }
            return l_Result;
        }

        public virtual bool Insert(DbConnection con, DbTransaction trans)
        {
            bool l_Result;
            this.cmd = con.CreateCommand();
            this.cmd.CommandText = string.Format(_insert, this.getTableName(base.GetType()), this.getColumns(base.GetType()), this.getColumnsParamInsert(base.GetType()));
            this.setParameterValueInsert(base.GetType(), ref this.cmd);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = con;
            this.cmd.Transaction = trans;
            try
            {
                int l_Retorno = this.cmd.ExecuteNonQuery();
                if (l_Retorno >= 1)
                {
                    return true;
                }
                l_Result = false;
            }
            catch (DbException dbE)
            {
                throw new ArgumentException((dbE.Message == this.TrataErro(dbE.Message)) ? ("Problema ao inserir dados, contacte o suporte. Mensagem:" + dbE.Message) : this.TrataErro(dbE.Message));
            }
            return l_Result;
        }

        public virtual int InsertIdentity()
        {
            if (this.trans == null)
            {
                this.factory = DbProviderFactories.GetFactory(this.getProviderName());
                this.con = this.factory.CreateConnection();
                this.con.ConnectionString = this.getConnectionString();
            }
            this.cmd = this.con.CreateCommand();
            if (this.trans != null)
            {
                this.cmd.Transaction = this.trans;
            }
            this.cmd.CommandText = string.Format(_insertIdentity, this.getTableName(base.GetType()), this.getColumns(base.GetType()), this.getColumnsParamInsert(base.GetType()));
            this.setParameterValueInsert(base.GetType(), ref this.cmd);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = this.con;
            int valor = 0;
            try
            {
                if (this.trans == null)
                {
                    this.con.Open();
                }
                valor = Convert.ToInt32(this.cmd.ExecuteScalar().ToString());
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                if (this.trans == null)
                {
                    this.con.Close();
                }
            }
            return valor;
        }

        public virtual int InsertIdentity(DbConnection con, DbTransaction trans)
        {
            this.cmd = con.CreateCommand();
            this.cmd.Transaction = trans;
            this.cmd.CommandText = string.Format(_insertIdentity, this.getTableName(base.GetType()), this.getColumns(base.GetType()), this.getColumnsParamInsert(base.GetType()));
            this.setParameterValueInsert(base.GetType(), ref this.cmd);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = con;
            int valor = 0;
            try
            {
                valor = Convert.ToInt32(this.cmd.ExecuteScalar().ToString());
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            return valor;
        }

        public virtual int InsertIdentity(DbTransaction trans)
        {
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.Transaction = trans;
            this.cmd.CommandText = string.Format(_insertIdentity, this.getTableName(base.GetType()), this.getColumns(base.GetType()), this.getColumnsParamInsert(base.GetType()));
            this.setParameterValueInsert(base.GetType(), ref this.cmd);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = con;
            int valor = 0;
            try
            {
                valor = Convert.ToInt32(this.cmd.ExecuteScalar().ToString());
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            return valor;
        }

        public virtual bool Update()
        {
            bool l_Result;
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = string.Format(_update, this.getTableName(base.GetType()), this.getColumnsParamUpdate(base.GetType()), this.getColumnsParamKey(base.GetType()));
            this.setParameterValueUpdate(base.GetType(), ref this.cmd);
            this.setParameterValueKey(base.GetType(), ref this.cmd);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = this.con;
            try
            {
                this.con.Open();
                int l_Retorno = this.cmd.ExecuteNonQuery();
                if (l_Retorno >= 1)
                {
                    return true;
                }
                l_Result = false;
            }
            catch (DbException dbE)
            {
                throw new ArgumentException((dbE.Message == this.TrataErro(dbE.Message)) ? ("Problema ao atualizar os dados, contacte o suporte. Mensagem:" + dbE.Message) : this.TrataErro(dbE.Message));
            }
            finally
            {
                if (this.trans == null)
                {
                    this.con.Close();
                }
            }
            return l_Result;
        }

        public virtual bool UpdateSequencial()
        {
            bool l_Result;
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = string.Format(_update, this.getTableName(base.GetType()), this.getColumnsParamUpdateSequencial(base.GetType()), this.getColumnsParamSequencial(base.GetType()));
            this.setParameterValueUpdateSequencial(base.GetType(), ref this.cmd);
            this.setParameterValueKeySequencial(base.GetType(), ref this.cmd);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = this.con;
            try
            {
                this.con.Open();
                int l_Retorno = this.cmd.ExecuteNonQuery();
                if (l_Retorno >= 1)
                {
                    return true;
                }
                l_Result = false;
            }
            catch (DbException dbE)
            {
                throw new ArgumentException((dbE.Message == this.TrataErro(dbE.Message)) ? ("Problema ao atualizar os dados, contacte o suporte. Mensagem:" + dbE.Message) : this.TrataErro(dbE.Message));
            }
            finally
            {
                if (this.trans == null)
                {
                    this.con.Close();
                }
            }
            return l_Result;
        }

        public virtual bool Update(DbConnection con, DbTransaction trans)
        {
            bool l_Result;
            this.cmd = con.CreateCommand();
            this.cmd.CommandText = string.Format(_update, this.getTableName(base.GetType()), this.getColumnsParamUpdate(base.GetType()), this.getColumnsParamKey(base.GetType()));
            this.setParameterValueUpdate(base.GetType(), ref this.cmd);
            this.setParameterValueKey(base.GetType(), ref this.cmd);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = con;
            this.cmd.Transaction = trans;
            try
            {
                int l_Retorno = this.cmd.ExecuteNonQuery();
                if (l_Retorno >= 1)
                {
                    return true;
                }
                l_Result = false;
            }
            catch (DbException dbE)
            {
                throw new ArgumentException((dbE.Message == this.TrataErro(dbE.Message)) ? ("Problema ao atualizar os dados, contacte o suporte. Mensagem:" + dbE.Message) : this.TrataErro(dbE.Message));
            }
            return l_Result;
        }

        public virtual bool Update(DbTransaction trans)
        {
            bool l_Result;
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd.CommandText = string.Format(_update, this.getTableName(base.GetType()), this.getColumnsParamUpdate(base.GetType()), this.getColumnsParamKey(base.GetType()));
            this.setParameterValueUpdate(base.GetType(), ref this.cmd);
            this.setParameterValueKey(base.GetType(), ref this.cmd);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = con;
            this.cmd.Transaction = trans;
            try
            {
                int l_Retorno = this.cmd.ExecuteNonQuery();
                if (l_Retorno >= 1)
                {
                    return true;
                }
                l_Result = false;
            }
            catch (DbException dbE)
            {
                throw new ArgumentException((dbE.Message == this.TrataErro(dbE.Message)) ? ("Problema ao atualizar os dados, contacte o suporte. Mensagem:" + dbE.Message) : this.TrataErro(dbE.Message));
            }
            return l_Result;
        }

        #endregion

        #region Retorno de dados

        #region DataReader X List

        /// <summary>
        /// Retorna a quantidade de registro na tabela informada na query.
        /// </summary>
        /// <param name="sql">Query com a TABELA para retornar a quantidade de registro - select count(*) as Qtd from tabela where condição</param>
        /// <returns>Quantidade de registro</returns>
        public int QuantidadeRegistro(string sql)
        {
            int l_QtdRegistro = 0;
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = sql;
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = this.con;

            try
            {
                this.con.Open();
                dr = this.cmd.ExecuteReader();
                while (dr.Read())
                {
                    l_QtdRegistro = (int)dr["Qtd"];
                }
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            return l_QtdRegistro;
        }

        public object getDataReaderSql(string sql)
        {
            DbDataReader l_DataReader;
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = sql;
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = this.con;

            try
            {
                this.con.Open();
                l_DataReader = this.cmd.ExecuteReader();
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            return l_DataReader;
        }

        public T GetAllDataReaderList<T>()
        {
            //Obtem o tipo de objeto que esta sendo utilizado
            Type objectType = typeof(T);

            // Cria uma instancia de T
            T objectInstance = Activator.CreateInstance<T>();

            try
            {
                InicializarFactory();
                cmd.CommandText = string.Format(_selectAll, getTableName(this.GetType()), this.getKeys()[0]);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    // Percorre os campos do DataReader
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        // Pega o nome do Campo do DataReader
                        var fieldName = dr.GetName(i);

                        // Procura propriedade com mesmo nome do Campo
                        //var property = typeof(T).GetProperty(fieldName);
                        var property = this.GetType().GetProperty(fieldName);

                        // Verifica se existe a propriedade, se ela não é somente-leitura e se o tipo da propriedade é o mesmo tipo do valor do banco de dados
                        if (property == null || !property.CanWrite || property.PropertyType != dr.GetFieldType(i)) continue;

                        // Define o valor da propriedade na instancia de T          
                        objectType.GetProperty(property.Name).SetValue(objectInstance, dr[property.Name], null);
                    }
                }
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                con.Close();
            }

            return objectInstance;
        }

        public List<T> getDataReaderList<T>(DbDataReader dr)
        {
            // Cria uma instancia de T            
            Type objectType = typeof(T);

            //Cria instancias do objeto
            T objectInstance = Activator.CreateInstance<T>();

            List<T> l_Lista = new List<T>();

            try
            {
                
                while (dr.Read())
                {
                    // Percorre os campos do DataReader
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        // Pega o nome do Campo do DataReader
                        var fieldName = dr.GetName(i);

                        // Procura propriedade com mesmo nome do Campo
                        //var property = typeof(T).GetProperty(fieldName);
                        var property = objectType.GetProperty(fieldName);

                        // Verifica se existe a propriedade, se ela não é somente-leitura e se o tipo da propriedade é o mesmo tipo do valor do banco de dados
                        if (property == null || !property.CanWrite || property.PropertyType != dr.GetFieldType(i)) continue;

                        // Define o valor da propriedade na instancia de T          
                        objectType.GetProperty(property.Name).SetValue(objectInstance, dr[property.Name], null);
                    }
                    l_Lista.Add(objectInstance);
                    objectInstance = Activator.CreateInstance<T>();
                }
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                con.Close();
            }
            return l_Lista;
        }

        public List<T> getDataReaderList<T>()
        {
            // Cria uma instancia de T            
            Type objectType = typeof(T);

            //Cria instancias do objeto
            T objectInstance = Activator.CreateInstance<T>();

            List<T> l_Lista = new List<T>();

            try
            {
                InicializarFactory();
                cmd.CommandText = string.Format(_selectAll, getTableName(this.GetType()), this.getKeys()[0]);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    // Percorre os campos do DataReader
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        // Pega o nome do Campo do DataReader
                        var fieldName = dr.GetName(i);

                        // Procura propriedade com mesmo nome do Campo
                        //var property = typeof(T).GetProperty(fieldName);
                        var property = this.GetType().GetProperty(fieldName);

                        // Verifica se existe a propriedade, se ela não é somente-leitura e se o tipo da propriedade é o mesmo tipo do valor do banco de dados
                        if (property == null || !property.CanWrite || property.PropertyType != dr.GetFieldType(i)) continue;

                        // Define o valor da propriedade na instancia de T          
                        objectType.GetProperty(property.Name).SetValue(objectInstance, dr[property.Name], null);
                    }
                    l_Lista.Add(objectInstance);
                    objectInstance = Activator.CreateInstance<T>();
                }
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                con.Close();
            }
            return l_Lista;
        }

        public static List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!object.Equals(dr[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[prop.Name], null);
                    }
                }
                list.Add(obj);
            }
            return list;
        }

        /// <summary>
        /// Em Desenvolvimento
        /// </summary>
        /// <typeparam name="T">Classe Genérica</typeparam>
        /// <param name="dr">Data Reader com multiplos selects de retorno</param>
        /// <returns>Lista dos múltiplos selects</returns>
        public static List<T> MultDataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list;
            Type subClasses = typeof(T);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();

            IEnumerable<Type> subclasses = types.Where(t => t.IsSubclassOf(subClasses));

            foreach (Type type in subclasses)
            {
                Console.WriteLine(type.Name);
            }


            do
            {
                list = new List<T>();
                T obj = default(T);
                while (dr.Read())
                {
                    obj = Activator.CreateInstance<T>();
                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        if (!object.Equals(dr[prop.Name], DBNull.Value))
                        {
                            prop.SetValue(obj, dr[prop.Name], null);
                        }
                    }
                    list.Add(obj);
                }
            } while (dr.NextResult());

            return list;
        }

        public List<T> getDataReaderList<T>(String a_Sql)
        {
            // Cria uma instancia de T            
            Type objectType = typeof(T);
            //Cria instancias do objeto
            T objectInstance = Activator.CreateInstance<T>();

            List<T> l_Lista = new List<T>();
            try
            {
                InicializarFactory();
                cmd.CommandText = a_Sql;//string.Format(_selectWhere, this.getTableName(base.GetType()), a_Where);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    // Percorre os campos do DataReader
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        // Pega o nome do Campo do DataReader
                        var fieldName = dr.GetName(i);

                        // Procura propriedade com mesmo nome do Campo
                        var property = typeof(T).GetProperty(fieldName);
                        //var property = objectInstance.GetType().GetProperty(fieldName);
                        //var property = this.GetType().GetProperty(fieldName);

                        // Verifica se existe a propriedade, se ela não é somente-leitura e se o tipo da propriedade é o mesmo tipo do valor do banco de dados
                        if (property == null || !property.CanWrite || property.PropertyType != dr.GetFieldType(i)) continue;

                        if (dr.GetValue(i) != DBNull.Value)
                        {
                            // Define o valor da propriedade na instancia de T      
                            objectType.GetProperty(property.Name).SetValue(objectInstance, dr[property.Name], null);
                        }
                    }
                    l_Lista.Add(objectInstance);
                    objectInstance = Activator.CreateInstance<T>();
                }
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                con.Close();
            }

            return l_Lista;
        }

        public List<T> getDataReaderList<T>(String a_Sql, CommandType a_Tipo)
        {
            // Cria uma instancia de T            
            Type objectType = typeof(T);
            //Cria instancias do objeto
            T objectInstance = Activator.CreateInstance<T>();

            List<T> l_Lista = new List<T>();
            try
            {
                InicializarFactory();
                cmd.CommandText = a_Sql;
                cmd.CommandType = a_Tipo;
                con.Open();
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    // Percorre os campos do DataReader
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        // Pega o nome do Campo do DataReader
                        var fieldName = dr.GetName(i);

                        // Procura propriedade com mesmo nome do Campo
                        var property = typeof(T).GetProperty(fieldName);
                        //var property = objectInstance.GetType().GetProperty(fieldName);
                        //var property = this.GetType().GetProperty(fieldName);

                        // Verifica se existe a propriedade, se ela não é somente-leitura e se o tipo da propriedade é o mesmo tipo do valor do banco de dados
                        if (property == null || !property.CanWrite || property.PropertyType != dr.GetFieldType(i)) continue;

                        if (dr.GetValue(i) != DBNull.Value)
                        {
                            // Define o valor da propriedade na instancia de T      
                            objectType.GetProperty(property.Name).SetValue(objectInstance, dr[property.Name], null);
                        }
                    }
                    l_Lista.Add(objectInstance);
                    objectInstance = Activator.CreateInstance<T>();
                }
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                con.Close();
            }

            return l_Lista;
        }

        public ArrayList getListDataReader(String a_Sql, String a_TipoComando)
        {
            // Create an ArrayList to hold the results
            ArrayList rowList = new ArrayList();

            try
            {
                cmd.CommandText = a_Sql;
                if (a_TipoComando == "P")
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                }
                else if (a_TipoComando == "T")
                {
                    cmd.CommandType = CommandType.Text;
                }
                con.Open();
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                }
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                con.Close();
            }
            return rowList;

        }
        #endregion

        #region DataSet X DataTable

        //public DbDataReader getDataReaderSql(string sql)
        //{
        //    DbDataReader l_DataReader;
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = sql;
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = this.con;

        //    try
        //    {
        //        this.con.Open();
        //        l_DataReader = this.cmd.ExecuteReader();
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    return l_DataReader;
        //}

        public DataSet getDataSetFieldIsNotNull()
        {
            DataSet l_DataSet;
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = string.Format(_select, this.getTableName(base.GetType()), this.getFieldsValuesIsNotNullSelect(base.GetType()));
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = this.con;
            try
            {
                this.ds = new DataSet();
                this.da = this.factory.CreateDataAdapter();
                this.da.SelectCommand = this.cmd;
                this.da.Fill(this.ds);
                l_DataSet = this.ds;
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                this.con.Close();
            }
            return l_DataSet;
        }

        protected DataSet getDataSetSql(string select)
        {
            DataSet l_DataSet;
            this.factory = null;
            this.con = null;
            try
            {
                this.factory = DbProviderFactories.GetFactory(this.getProviderName());
                this.con = this.factory.CreateConnection();
                this.con.ConnectionString = this.getConnectionString();
                this.cmd = this.con.CreateCommand();
                this.cmd.CommandText = _selectDateFormat + select;
                this.cmd.CommandType = CommandType.Text;
                this.cmd.Connection = this.con;

                this.ds = new DataSet();
                this.da = this.factory.CreateDataAdapter();
                this.da.SelectCommand = this.cmd;
                this.da.Fill(this.ds);
                l_DataSet = this.ds;
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                this.da.Dispose();
                this.ds.Dispose();
                this.con.Close();
                this.con.Dispose();
            }
            return l_DataSet;
        }

        protected DataSet getDataSetProcedure(string select)
        {
            DataSet l_DataSet;
            this.factory = null;
            this.con = null;
            try
            {
                this.factory = DbProviderFactories.GetFactory(this.getProviderName());
                this.con = this.factory.CreateConnection();
                this.con.ConnectionString = this.getConnectionString();
                this.cmd = this.con.CreateCommand();
                this.cmd.CommandText = select;
                this.cmd.CommandType = CommandType.Text;
                this.cmd.Connection = this.con;

                this.ds = new DataSet();
                this.da = this.factory.CreateDataAdapter();
                this.da.SelectCommand = this.cmd;
                this.da.Fill(this.ds);
                l_DataSet = this.ds;
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                this.da.Dispose();
                this.ds.Dispose();
                this.con.Close();
                this.con.Dispose();
            }
            return l_DataSet;
        }

        public virtual DataSet getDataSetWhere(string where)
        {
            DataSet l_DataSet;
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = string.Format(_selectWhere, this.getTableName(base.GetType()), where);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = this.con;
            try
            {
                this.ds = new DataSet();
                this.da = this.factory.CreateDataAdapter();
                this.da.SelectCommand = this.cmd;
                this.da.Fill(this.ds);
                l_DataSet = this.ds;
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                this.con.Close();
            }
            return l_DataSet;
        }

        public DataTable getDataTableAllData()
        {
            DataTable l_DataTable;
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = string.Format(_selectAll, this.getTableName(base.GetType()));
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = this.con;
            try
            {
                this.dt = new DataTable();
                this.da = this.factory.CreateDataAdapter();
                this.da.SelectCommand = this.cmd;
                this.dt.BeginLoadData();
                this.da.Fill(this.dt);
                this.dt.EndLoadData();
                l_DataTable = this.dt;
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                this.dt.Dispose();
                this.con.Close();
            }
            return l_DataTable;
        }

        public DataTable getDataTableSql(string select)
        {
            DataTable l_DataTable;
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            //this.factory = DbProviderFactories.GetFactory("FirebirdSql.Data.FirebirdClient");//this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = _selectDateFormat + select;
            this.cmd.CommandType = CommandType.Text;
            this.cmd.CommandTimeout = 0;
            this.cmd.Connection = this.con;
            try
            {
                this.dt = new DataTable();
                this.da = this.factory.CreateDataAdapter();
                this.da.SelectCommand = this.cmd;
                this.dt.BeginLoadData();
                this.da.Fill(this.dt);
                this.dt.EndLoadData();
                l_DataTable = this.dt;
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                this.da.Dispose();
                this.dt.Dispose();
                this.con.Close();
                this.con.Dispose();
            }
            return l_DataTable;
        }

        public virtual DataTable getDataTableWhere(string where)
        {
            DataTable l_DataTable;
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = string.Format(_selectWhere, this.getTableName(base.GetType()), where);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.CommandTimeout = 0;
            this.cmd.Connection = this.con;
            try
            {
                this.dt = new DataTable();
                this.da = this.factory.CreateDataAdapter();
                this.da.SelectCommand = this.cmd;
                this.dt.BeginLoadData();
                this.da.Fill(this.dt);
                this.dt.EndLoadData();
                l_DataTable = this.dt;
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                this.con.Close();
            }
            return l_DataTable;
        }

        #endregion

        #region Métodos gerais para manipulação da classe herdeira

        public void getSelectDataKey()
        {
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = string.Format(_select, this.getTableName(base.GetType()), this.getColumnsParamKey(base.GetType()));
            this.setParameterValueKey(base.GetType(), ref this.cmd);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.CommandTimeout = 0;
            this.cmd.Connection = this.con;
            DbDataReader dr = null;
            try
            {
                object[] cols;
                this.con.Open();
                dr = this.cmd.ExecuteReader();
                if (dr.Read())
                {
                    foreach (PropertyInfo prop in base.GetType().GetProperties())
                    {
                        cols = prop.GetCustomAttributes(typeof(ColumnAttribute), true);
                        if (cols.Length == 1)
                        {
                            ColumnAttribute ca = cols[0] as ColumnAttribute;
                            if (!ca.Temp)
                            {
                                if (dr[ca.ColumnName].GetType() != DBNull.Value.GetType())
                                {
                                    prop.SetValue(this, dr[ca.ColumnName], null);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (PropertyInfo prop in base.GetType().GetProperties())
                    {
                        cols = prop.GetCustomAttributes(true);
                        if (cols.Length > 0)
                        {
                            if (cols[0] is KeyAttribute)
                            {
                                prop.SetValue(this, null, null);
                            }
                            else if (cols[0] is ColumnAttribute)
                            {
                                prop.SetValue(this, null, null);
                            }
                        }
                    }
                }
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                dr.Close();
                dr.Dispose();
                this.con.Close();
                this.con.Dispose();
            }
        }

        public void getSelectDataKey_DataTable()
        {
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = string.Format(_select, this.getTableName(base.GetType()), this.getColumnsParamKey(base.GetType()));
            this.setParameterValueKey(base.GetType(), ref this.cmd);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.CommandTimeout = 0;
            this.cmd.Connection = this.con;
            try
            {
                object[] cols;
                this.dt = new DataTable();
                this.da = this.factory.CreateDataAdapter();
                this.da.SelectCommand = this.cmd;
                this.dt.BeginLoadData();
                this.da.Fill(this.dt);
                this.dt.EndLoadData();
                this.da.Dispose();
                if (this.dt.Rows.Count >= 1)
                {
                    DataRow dr = this.dt.Rows[0];
                    foreach (PropertyInfo prop in base.GetType().GetProperties())
                    {
                        cols = prop.GetCustomAttributes(typeof(ColumnAttribute), true);
                        if (cols.Length > 0)
                        {
                            if (cols.Length == 1)
                            {
                                ColumnAttribute ca = cols[0] as ColumnAttribute;
                                if (dr[ca.ColumnName].GetType() != DBNull.Value.GetType())
                                {
                                    prop.SetValue(this, dr[ca.ColumnName], null);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (PropertyInfo prop in base.GetType().GetProperties())
                    {
                        cols = prop.GetCustomAttributes(true);
                        if (cols.Length > 0)
                        {
                            if (cols[0] is KeyAttribute)
                            {
                                prop.SetValue(this, null, null);
                            }
                            else if (cols[0] is ColumnAttribute)
                            {
                                prop.SetValue(this, null, null);
                            }
                        }
                    }
                }
                this.dt.Dispose();
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                this.con.Close();
                this.con.Dispose();
            }
        }

        public void getSelectDataWhere(string where)
        {
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = string.Format(_selectWhere, this.getTableName(base.GetType()), where);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.CommandTimeout = 0;
            this.cmd.Connection = this.con;
            DbDataReader dr = null;
            try
            {
                object[] cols;
                this.con.Open();
                dr = this.cmd.ExecuteReader();
                if (dr.Read())
                {
                    foreach (PropertyInfo prop in base.GetType().GetProperties())
                    {
                        cols = prop.GetCustomAttributes(true);
                        if (cols.Length == 1)
                        {
                            if (cols[0] is KeyAttribute)
                            {
                                KeyAttribute ca = cols[0] as KeyAttribute;
                                if (dr[ca.KeyName].GetType() != DBNull.Value.GetType())
                                {
                                    prop.SetValue(this, dr[ca.KeyName], null);
                                }
                            }
                            else
                            {
                                ColumnAttribute ca = cols[0] as ColumnAttribute;
                                if (dr[ca.ColumnName].GetType() != DBNull.Value.GetType())
                                {
                                    prop.SetValue(this, dr[ca.ColumnName], null);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (PropertyInfo prop in base.GetType().GetProperties())
                    {
                        cols = prop.GetCustomAttributes(true);
                        if (cols.Length > 0)
                        {
                            if (cols[0] is KeyAttribute)
                            {
                                prop.SetValue(this, null, null);
                            }
                            else if (cols[0] is ColumnAttribute)
                            {
                                prop.SetValue(this, null, null);
                            }
                        }
                    }
                }
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                dr.Close();
                dr.Dispose();
                this.con.Close();
                this.con.Dispose();
            }
        }

        public void getSelectDataWhere_Table(string where)
        {
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = string.Format(_selectWhere, this.getTableName(base.GetType()), where);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = this.con;
            try
            {
                object[] cols;
                this.dt = new DataTable();
                this.da = this.factory.CreateDataAdapter();
                this.da.SelectCommand = this.cmd;
                this.dt.BeginLoadData();
                this.da.Fill(this.dt);
                this.dt.EndLoadData();
                if (this.dt.Rows.Count >= 1)
                {
                    DataRow dr = this.dt.Rows[0];
                    foreach (PropertyInfo prop in base.GetType().GetProperties())
                    {
                        cols = prop.GetCustomAttributes(true);
                        if (cols.Length == 1)
                        {
                            if (cols[0] is KeyAttribute)
                            {
                                KeyAttribute ca = cols[0] as KeyAttribute;
                                if (dr[ca.KeyName].GetType() != DBNull.Value.GetType())
                                {
                                    prop.SetValue(this, dr[ca.KeyName], null);
                                }
                            }
                            else
                            {
                                ColumnAttribute ca = cols[0] as ColumnAttribute;
                                if (dr[ca.ColumnName].GetType() != DBNull.Value.GetType())
                                {
                                    prop.SetValue(this, dr[ca.ColumnName], null);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (PropertyInfo prop in base.GetType().GetProperties())
                    {
                        cols = prop.GetCustomAttributes(true);
                        if (cols[0] is KeyAttribute)
                        {
                            prop.SetValue(this, null, null);
                        }
                        else if (cols[0] is ColumnAttribute)
                        {
                            prop.SetValue(this, null, null);
                        }
                    }
                }
                this.dt.Dispose();
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                this.con.Close();
            }
        }

        public virtual int getSelectMax()
        {
            int l_Result;
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = string.Format(_selectMax, this.getTableName(base.GetType()), this.getKeys()[0]);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.CommandTimeout = 0;
            this.cmd.Connection = this.con;
            try
            {
                this.con.Open();
                l_Result = Convert.ToInt32(this.cmd.ExecuteScalar().ToString());
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                this.con.Close();
            }
            return l_Result;
        }

        public virtual int getSelectMax(string Key)
        {
            int l_Result;
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = string.Format(_selectMax, this.getTableName(base.GetType()), Key);
            this.cmd.CommandType = CommandType.Text;
            this.cmd.CommandTimeout = 0;
            this.cmd.Connection = this.con;
            try
            {
                this.con.Open();
                l_Result = Convert.ToInt32(this.cmd.ExecuteScalar().ToString());
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                this.con.Close();
            }
            return l_Result;
        }

        public object getValueScalar(string sql)
        {
            object l_Result;
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            this.con = this.factory.CreateConnection();
            this.con.ConnectionString = this.getConnectionString();
            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = sql;
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = this.con;
            try
            {
                this.con.Open();
                l_Result = this.cmd.ExecuteScalar();
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            finally
            {
                this.con.Close();
            }
            return l_Result;
        }

        public object getValueScalar(string sql, DbConnection con, DbTransaction trans)
        {
            object l_Result;
            this.cmd = con.CreateCommand();
            this.cmd.CommandText = sql;
            this.cmd.CommandType = CommandType.Text;
            this.cmd.Connection = con;
            this.cmd.Transaction = trans;
            try
            {
                l_Result = this.cmd.ExecuteScalar();
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
            return l_Result;
        }

        #endregion

        #endregion

        #region Inicializa Factory, Conexão e Comando...

        /// <summary>
        /// Inicializa o factory, conexão e comando
        /// </summary>
        private void InicializarFactory()
        {
            this.factory = DbProviderFactories.GetFactory(this.getProviderName());
            con = factory.CreateConnection();
            con.ConnectionString = getConnectionString(); //getConnectionStringsFromConfigFile("BancoAux");
            cmd = con.CreateCommand();
            cmd.Connection = con;
        }

        #endregion
        //Form da persist desarrumada        
        #region Old

        //#region Propriedades da Classe

        //// Fields
        //private static string _delete = "DELETE FROM {0} WHERE {1}";
        //private static string _insert = "SET DATEFORMAT dmy INSERT INTO {0} ({1}) VALUES({2})";
        //private static string _insertIdentity = "SET DATEFORMAT dmy INSERT INTO {0} ({1}) VALUES({2}) SELECT ISNULL(@@IDENTITY,0) ";
        //private static string _select = "SELECT * FROM {0} WHERE {1}";
        //private static string _selectAll = "SELECT * FROM {0}";
        //private static string _selectDateFormat = " SET DATEFORMAT dmy ";
        //private static string _selectMax = "SELECT coalesce( max( {1} )+1, 1 ) FROM {0} ";
        //private static string _selectWhere = "SELECT * FROM {0} {1}";
        //private static string _tagParam = "@";
        //private static string _update = "SET DATEFORMAT dmy UPDATE {0} SET {1} WHERE {2}";
        //private DbCommand cmd = null;
        //private DbConnection con = null;
        //private ConnectionStringSettings conSettings = null;
        //private DbDataAdapter da = null;
        //private DataSet ds = null;
        //private DataTable dt = null;
        //private DbProviderFactory factory = null;
        //private DbTransaction trans = null;

        //public string ConexaoDiretaExterna { get; set; }
        //public string ProviderexternoDireto { get; set; }

        //#endregion

        //// Methods
        //public PersistClass()
        //{
        //    this.InicializeColumns(base.GetType());
        //    this.InicializeVariaveisPrivate();
        //}

        //public DbTransaction ObterTransacao()
        //{
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.con.Open();
        //    //this.trans = this.con.BeginTransaction();
        //    return this.con.BeginTransaction();
        //}

        //public virtual bool Delete()
        //{
        //    bool l_Result;
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = string.Format(_delete, this.getTableName(base.GetType()), this.getColumnsParamKey(base.GetType()));
        //    this.setParameterValueKey(base.GetType(), ref this.cmd);
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = this.con;
        //    try
        //    {
        //        this.con.Open();
        //        if (this.cmd.ExecuteNonQuery() >= 1)
        //        {
        //            return true;
        //        }
        //        l_Result = false;
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException((dbE.Message == this.TrataErro(dbE.Message)) ? ("Problema ao excluir dados, contacte o suporte. Mensagem:" + dbE.Message) : this.TrataErro(dbE.Message));
        //    }
        //    finally
        //    {
        //        if (this.trans == null)
        //        {
        //            this.con.Close();
        //        }
        //    }
        //    return l_Result;
        //}

        //public virtual bool Delete(DbConnection con, DbTransaction trans)
        //{
        //    bool l_Result;
        //    this.cmd = con.CreateCommand();
        //    this.cmd.CommandText = string.Format(_delete, this.getTableName(base.GetType()), this.getColumnsParamKey(base.GetType()));
        //    this.setParameterValueKey(base.GetType(), ref this.cmd);
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = con;
        //    this.cmd.Transaction = trans;
        //    try
        //    {
        //        if (this.cmd.ExecuteNonQuery() >= 1)
        //        {
        //            return true;
        //        }
        //        l_Result = false;
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException((dbE.Message == this.TrataErro(dbE.Message)) ? ("Problema ao excluir dados, contacte o suporte. Mensagem:" + dbE.Message) : this.TrataErro(dbE.Message));
        //    }
        //    return l_Result;
        //}

        //public object ExecStoredProc(string sql, DbParameter[] a_Parametros, DbTransaction a_Transacao) //, DbConnection a_conexao)
        //{
        //    object l_Result;

        //    if (this.factory == null)
        //    {
        //        this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //        this.con = this.factory.CreateConnection();
        //    }
        //    if (string.IsNullOrEmpty(this.con.ConnectionString))
        //    {
        //        this.con.ConnectionString = this.getConnectionString();
        //    }
        //    this.cmd = this.con.CreateCommand();

        //    this.cmd.CommandText = sql;
        //    this.cmd.CommandType = CommandType.StoredProcedure;
        //    this.cmd.Connection = this.con;

        //    this.cmd.Parameters.AddRange(a_Parametros);
        //    this.cmd.Transaction = a_Transacao;

        //    try
        //    {

        //        if (this.con.State != ConnectionState.Open)
        //        {
        //            this.con.Open();
        //        }

        //        //l_Result = this.cmd.ExecuteNonQuery();// .ExecuteScalar();
        //        l_Result = this.cmd.ExecuteScalar();
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        //this.con.Close();
        //    }
        //    return l_Result;
        //}

        //public object ExecStoredProc(string sql, DbParameter[] a_Parametros)
        //{
        //    object l_Result;

        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();

        //    this.cmd = this.con.CreateCommand();

        //    this.cmd.CommandText = sql;
        //    this.cmd.CommandType = CommandType.StoredProcedure;
        //    this.cmd.Connection = this.con;

        //    this.cmd.Parameters.AddRange(a_Parametros);


        //    try
        //    {
        //        this.con.Open();

        //        l_Result = this.cmd.ExecuteScalar();
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        this.con.Close();
        //    }
        //    return l_Result;
        //}

        //public object ExecStoredProc(string sql)
        //{
        //    object l_Result;
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = " set dateformat dmy " + sql;
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = this.con;
        //    try
        //    {
        //        this.con.Open();
        //        l_Result = this.cmd.ExecuteScalar();
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        this.con.Close();
        //    }
        //    return l_Result;
        //}

        //public object ExecStoredProc(string sql, int timout)
        //{
        //    object l_Result;
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = sql;
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.CommandTimeout = timout;
        //    this.cmd.Connection = this.con;
        //    try
        //    {
        //        this.con.Open();
        //        l_Result = this.cmd.ExecuteScalar();
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        this.con.Close();
        //    }
        //    return l_Result;
        //}

        //public bool Execute(string sql)
        //{
        //    bool l_Result;
        //    if (this.trans == null)
        //    {
        //        this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //        this.con = this.factory.CreateConnection();
        //        this.con.ConnectionString = this.getConnectionString();
        //    }
        //    else
        //    {
        //        this.cmd.Transaction = this.trans;
        //    }
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = sql;
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = this.con;
        //    try
        //    {
        //        this.con.Open();
        //        if (this.cmd.ExecuteNonQuery() >= 1)
        //        {
        //            return true;
        //        }
        //        l_Result = false;
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        if (this.trans == null)
        //        {
        //            this.con.Close();
        //        }
        //    }
        //    return l_Result;
        //}

        //public bool Execute(string sql, DbConnection con, DbTransaction trans)
        //{
        //    bool l_Result;
        //    this.cmd = con.CreateCommand();
        //    this.cmd.CommandText = _selectDateFormat + sql;
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = con;
        //    this.cmd.Transaction = trans;
        //    try
        //    {
        //        if (this.cmd.ExecuteNonQuery() >= 1)
        //        {
        //            return true;
        //        }
        //        l_Result = false;
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException(this.TrataErro(dbE.Message));
        //    }
        //    return l_Result;
        //}

        //public virtual string getAppSettings()
        //{
        //    //return "StringConexaoPadrao";
        //    return ConfigurationManager.AppSettings.GetKey(0);
        //}

        //protected string getAppSettingsDataBase()
        //{
        //    return "DataBase";
        //}

        //public string getColumnName(string field)
        //{
        //    string column = string.Empty;
        //    foreach (PropertyInfo prop in base.GetType().GetProperties())
        //    {
        //        if (prop.Name == field)
        //        {
        //            object[] cols = null;
        //            cols = prop.GetCustomAttributes(true);
        //            if (cols.Length == 1)
        //            {
        //                if (cols[0] is ColumnAttribute)
        //                {
        //                    ColumnAttribute ca = cols[0] as ColumnAttribute;
        //                    column = ca.ColumnName;
        //                }
        //                else if (cols[0] is KeyAttribute)
        //                {
        //                    KeyAttribute ca = cols[0] as KeyAttribute;
        //                    column = ca.KeyName;
        //                }
        //            }
        //        }
        //    }
        //    return column;
        //}

        //private string getColumns(Type type)
        //{
        //    string columns = string.Empty;
        //    foreach (PropertyInfo prop in type.GetProperties())
        //    {
        //        object[] cols = null;
        //        cols = prop.GetCustomAttributes(true);
        //        if (cols.Length == 1)
        //        {
        //            if (cols[0] is ColumnAttribute)
        //            {
        //                if (!this.ValueIsNull(prop))
        //                {
        //                    ColumnAttribute ca = cols[0] as ColumnAttribute;
        //                    if (!ca.Inc)
        //                    {
        //                        columns = columns + ca.ColumnName + ", ";
        //                    }
        //                    //columns = columns + ca.ColumnName + ", ";
        //                }
        //            }
        //            else if (cols[0] is KeyAttribute)
        //            {
        //                KeyAttribute ca = cols[0] as KeyAttribute;
        //                if (!ca.Inc)
        //                {
        //                    columns = columns + ca.KeyName + ", ";
        //                }
        //            }
        //        }
        //    }
        //    return columns.Remove(columns.Length - 2);
        //}

        //public string[] GetColumns()
        //{
        //    return this.getColumns(base.GetType()).Split(new char[] { char.Parse(",") });
        //}

        //private string getColumnsParamInsert(Type type)
        //{
        //    string columns = string.Empty;
        //    foreach (PropertyInfo prop in type.GetProperties())
        //    {
        //        object[] cols = null;
        //        cols = prop.GetCustomAttributes(true);
        //        if (cols.Length == 1)
        //        {
        //            if (cols[0] is ColumnAttribute)
        //            {
        //                ColumnAttribute ca = cols[0] as ColumnAttribute;
        //                if (!ca.Inc)
        //                {
        //                    if (!this.ValueIsNull(prop))
        //                    {
        //                        columns = columns + _tagParam + ca.ColumnName + ", ";
        //                    }
        //                }
        //            }
        //            else if (cols[0] is KeyAttribute)
        //            {
        //                KeyAttribute ca = cols[0] as KeyAttribute;
        //                if (!ca.Inc)
        //                {
        //                    columns = columns + _tagParam + ca.KeyName + ", ";
        //                }
        //            }
        //        }
        //    }
        //    return columns.Remove(columns.Length - 2);
        //}

        //private string getColumnsParamKey(Type type)
        //{
        //    string key = string.Empty;
        //    foreach (PropertyInfo prop in type.GetProperties())
        //    {
        //        object[] cols = prop.GetCustomAttributes(true);
        //        if ((cols.Length == 1) && (cols[0] is KeyAttribute))
        //        {
        //            KeyAttribute ca = cols[0] as KeyAttribute;
        //            string l_Aux = key;
        //            key = l_Aux + ca.KeyName + " = " + _tagParam + "_KEY_" + ca.KeyName + " AND ";
        //        }
        //    }
        //    return key.Remove(key.Length - 4);
        //}

        //private string getColumnsParamUpdate(Type type)
        //{
        //    string columns = string.Empty;
        //    foreach (PropertyInfo prop in type.GetProperties())
        //    {
        //        object[] cols = prop.GetCustomAttributes(true);
        //        if (cols.Length == 1)
        //        {
        //            string l_Aux;
        //            if (cols[0] is ColumnAttribute)
        //            {
        //                ColumnAttribute ca = cols[0] as ColumnAttribute;
        //                if (!ca.Inc)
        //                {
        //                    l_Aux = columns;
        //                    columns = l_Aux + ca.ColumnName + " = " + _tagParam + ca.ColumnName + ", ";
        //                }
        //            }
        //            else if (cols[0] is KeyAttribute)
        //            {
        //                KeyAttribute ca = cols[0] as KeyAttribute;
        //                if (!ca.Inc)
        //                {
        //                    l_Aux = columns;
        //                    columns = l_Aux + ca.KeyName + " = " + _tagParam + ca.KeyName + ", ";
        //                }
        //            }
        //        }
        //    }
        //    return columns.Remove(columns.Length - 2);
        //}

        //private string getProviderName()
        //{
        //    if (!String.IsNullOrEmpty(ProviderexternoDireto))
        //    {
        //        return ProviderexternoDireto;
        //    }
        //    else
        //    {
        //        string key = this.getAppSettings();
        //        this.conSettings = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings[key]];
        //        return this.conSettings.ProviderName;
        //    }
        //}

        //protected string getConnectionString()
        //{
        //    if (!String.IsNullOrEmpty(ConexaoDiretaExterna))
        //    {
        //        return ConexaoDiretaExterna;
        //    }
        //    else
        //    {
        //        string key = this.getAppSettings();
        //        this.conSettings = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings[key]];
        //        return this.conSettings.ConnectionString;
        //    }
        //}

        //protected DataBaseType getDataBase()
        //{
        //    string key = this.getAppSettingsDataBase();
        //    string l_Chave = string.Empty;

        //    try
        //    {
        //        l_Chave = ConfigurationManager.AppSettings[key].ToUpper();
        //    }
        //    catch
        //    {
        //        l_Chave = null;
        //    }

        //    if ((l_Chave != null) && (l_Chave == "MYSQL"))
        //    {
        //        return DataBaseType.MYSQL;
        //    }
        //    return DataBaseType.MSSQL;
        //}

        //public DbDataReader getDataReaderSql(string sql)
        //{
        //    DbDataReader l_DataReader;
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = sql;
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = this.con;
        //    try
        //    {
        //        this.con.Open();
        //        l_DataReader = this.cmd.ExecuteReader();
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    return l_DataReader;
        //}

        //public DataSet getDataSetFieldIsNotNull()
        //{
        //    DataSet l_DataSet;
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = string.Format(_select, this.getTableName(base.GetType()), this.getFieldsValuesIsNotNullSelect(base.GetType()));
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = this.con;
        //    try
        //    {
        //        this.ds = new DataSet();
        //        this.da = this.factory.CreateDataAdapter();
        //        this.da.SelectCommand = this.cmd;
        //        this.da.Fill(this.ds);
        //        l_DataSet = this.ds;
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        this.con.Close();
        //    }
        //    return l_DataSet;
        //}

        //protected DataSet getDataSetSql(string select)
        //{
        //    DataSet l_DataSet;
        //    this.factory = null;
        //    this.con = null;
        //    try
        //    {
        //        this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //        this.con = this.factory.CreateConnection();
        //        this.con.ConnectionString = this.getConnectionString();
        //        this.cmd = this.con.CreateCommand();
        //        this.cmd.CommandText = _selectDateFormat + select;
        //        this.cmd.CommandType = CommandType.Text;
        //        this.cmd.Connection = this.con;

        //        this.ds = new DataSet();
        //        this.da = this.factory.CreateDataAdapter();
        //        this.da.SelectCommand = this.cmd;
        //        this.da.Fill(this.ds);
        //        l_DataSet = this.ds;
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        this.da.Dispose();
        //        this.ds.Dispose();
        //        this.con.Close();
        //        this.con.Dispose();
        //    }
        //    return l_DataSet;
        //}

        //public virtual DataSet getDataSetWhere(string where)
        //{
        //    DataSet l_DataSet;
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = string.Format(_selectWhere, this.getTableName(base.GetType()), where);
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = this.con;
        //    try
        //    {
        //        this.ds = new DataSet();
        //        this.da = this.factory.CreateDataAdapter();
        //        this.da.SelectCommand = this.cmd;
        //        this.da.Fill(this.ds);
        //        l_DataSet = this.ds;
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        this.con.Close();
        //    }
        //    return l_DataSet;
        //}

        //public DataTable getDataTableAllData()
        //{
        //    DataTable l_DataTable;
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = string.Format(_selectAll, this.getTableName(base.GetType()));
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = this.con;
        //    try
        //    {
        //        this.dt = new DataTable();
        //        this.da = this.factory.CreateDataAdapter();
        //        this.da.SelectCommand = this.cmd;
        //        this.dt.BeginLoadData();
        //        this.da.Fill(this.dt);
        //        this.dt.EndLoadData();
        //        l_DataTable = this.dt;
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        this.dt.Dispose();
        //        this.con.Close();
        //    }
        //    return l_DataTable;
        //}

        //protected DataTable getDataTableSql(string select)
        //{
        //    DataTable l_DataTable;
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = _selectDateFormat + select;
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.CommandTimeout = 0;
        //    this.cmd.Connection = this.con;
        //    try
        //    {
        //        this.dt = new DataTable();
        //        this.da = this.factory.CreateDataAdapter();
        //        this.da.SelectCommand = this.cmd;
        //        this.dt.BeginLoadData();
        //        this.da.Fill(this.dt);
        //        this.dt.EndLoadData();
        //        l_DataTable = this.dt;
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        this.da.Dispose();
        //        this.dt.Dispose();
        //        this.con.Close();
        //        this.con.Dispose();
        //    }
        //    return l_DataTable;
        //}

        //public virtual DataTable getDataTableWhere(string where)
        //{
        //    DataTable l_DataTable;
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = string.Format(_selectWhere, this.getTableName(base.GetType()), where);
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.CommandTimeout = 0;
        //    this.cmd.Connection = this.con;
        //    try
        //    {
        //        this.dt = new DataTable();
        //        this.da = this.factory.CreateDataAdapter();
        //        this.da.SelectCommand = this.cmd;
        //        this.dt.BeginLoadData();
        //        this.da.Fill(this.dt);
        //        this.dt.EndLoadData();
        //        l_DataTable = this.dt;
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        this.con.Close();
        //    }
        //    return l_DataTable;
        //}

        //private string getFieldsValuesIsNotNull(Type type)
        //{
        //    string columns = string.Empty;
        //    foreach (PropertyInfo prop in type.GetProperties())
        //    {
        //        object[] cols = prop.GetCustomAttributes(true);
        //        if (cols.Length == 1)
        //        {
        //            string l_Aux;
        //            if (cols[0] is ColumnAttribute)
        //            {
        //                ColumnAttribute ca = cols[0] as ColumnAttribute;
        //                if (!this.ValueIsNull(prop))
        //                {
        //                    if (((ca.CType == ColumnType.Single) || (ca.CType == ColumnType.Double)) || (ca.CType == ColumnType.Decimal))
        //                    {
        //                        l_Aux = columns;
        //                        columns = l_Aux + ca.ColumnName + " = '" + prop.GetValue(this, null).ToString().Replace(",", ".") + "', ";
        //                    }
        //                    else if ((ca.CType == ColumnType.DateTime) && (this.getDataBase() == DataBaseType.MYSQL))
        //                    {
        //                        l_Aux = columns;
        //                        columns = l_Aux + ca.ColumnName + " = '" + Convert.ToDateTime(prop.GetValue(this, null)).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
        //                    }
        //                    else
        //                    {
        //                        l_Aux = columns;
        //                        columns = l_Aux + ca.ColumnName + " = '" + Seguranca.SeguracaSql(prop.GetValue(this, null).ToString()) + "', ";
        //                    }
        //                }
        //                else
        //                {
        //                    columns = columns + ca.ColumnName + " = null , ";
        //                }
        //            }
        //            else if (cols[0] is KeyAttribute)
        //            {
        //                KeyAttribute ca = cols[0] as KeyAttribute;
        //                if (!ca.Inc)
        //                {
        //                    if (prop.PropertyType.FullName == "System.Int32[]")
        //                    {
        //                        l_Aux = columns;
        //                        columns = l_Aux + ca.KeyName + " = '" + Seguranca.SeguracaSql(((int[])prop.GetValue(this, null))[1].ToString()) + "', ";
        //                    }
        //                    else if ((ca.CType == ColumnType.DateTime) && (this.getDataBase() == DataBaseType.MYSQL))
        //                    {
        //                        l_Aux = columns;
        //                        columns = l_Aux + ca.KeyName + " = '" + Convert.ToDateTime(prop.GetValue(this, null)).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
        //                    }
        //                    else
        //                    {
        //                        l_Aux = columns;
        //                        columns = l_Aux + ca.KeyName + " = '" + Seguranca.SeguracaSql(prop.GetValue(this, null).ToString()) + "', ";
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return columns.Remove(columns.Length - 2);
        //}

        //private string getFieldsValuesIsNotNullSelect(Type type)
        //{
        //    string columns = string.Empty;
        //    foreach (PropertyInfo prop in type.GetProperties())
        //    {
        //        if (!this.ValueIsNull(prop))
        //        {
        //            object[] cols = prop.GetCustomAttributes(true);
        //            if (cols.Length == 1)
        //            {
        //                object l_Aux;
        //                if (cols[0] is KeyAttribute)
        //                {
        //                    KeyAttribute ca = cols[0] as KeyAttribute;
        //                    l_Aux = columns;
        //                    columns = string.Concat(new object[] { l_Aux, ca.KeyName, " = '", prop.GetValue(this, null), "' AND " });
        //                }
        //                else
        //                {
        //                    ColumnAttribute ca = cols[0] as ColumnAttribute;
        //                    l_Aux = columns;
        //                    columns = string.Concat(new object[] { l_Aux, ca.ColumnName, " = '", prop.GetValue(this, null), "' AND " });
        //                }
        //            }
        //        }
        //    }
        //    return columns.Remove(columns.Length - 4);
        //}

        //public string[] getKeys()
        //{
        //    Type type = base.GetType();
        //    ArrayList keyList = new ArrayList();
        //    foreach (PropertyInfo p in type.GetProperties())
        //    {
        //        object[] field = p.GetCustomAttributes(typeof(KeyAttribute), true);
        //        if (field.Length >= 1)
        //        {
        //            KeyAttribute c = field[0] as KeyAttribute;
        //            keyList.Add(c.KeyName);
        //        }
        //    }
        //    string[] key = new string[keyList.Count];
        //    for (int i = 0; i < keyList.Count; i++)
        //    {
        //        key[i] = (string)keyList[i];
        //    }
        //    return key;
        //}

        //private string getNameColumns(Type type)
        //{
        //    string columns = string.Empty;
        //    foreach (PropertyInfo prop in type.GetProperties())
        //    {
        //        object[] cols = null;
        //        cols = prop.GetCustomAttributes(true);
        //        if (cols.Length == 1)
        //        {
        //            if (cols[0] is ColumnAttribute)
        //            {
        //                ColumnAttribute ca = cols[0] as ColumnAttribute;
        //                columns = columns + ca.ColumnName + ",";
        //            }
        //            else if (cols[0] is KeyAttribute)
        //            {
        //                KeyAttribute ca = cols[0] as KeyAttribute;
        //                columns = columns + ca.KeyName + ",";
        //            }
        //        }
        //    }
        //    return columns.Remove(columns.Length - 1);
        //}

        //public string[] GetNameColumns()
        //{
        //    return this.getNameColumns(base.GetType()).Split(new char[] { char.Parse(",") });
        //}

        //public void getSelectDataKey()
        //{
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = string.Format(_select, this.getTableName(base.GetType()), this.getColumnsParamKey(base.GetType()));
        //    this.setParameterValueKey(base.GetType(), ref this.cmd);
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.CommandTimeout = 0;
        //    this.cmd.Connection = this.con;
        //    DbDataReader dr = null;
        //    try
        //    {
        //        object[] cols;
        //        this.con.Open();
        //        dr = this.cmd.ExecuteReader();
        //        if (dr.Read())
        //        {
        //            foreach (PropertyInfo prop in base.GetType().GetProperties())
        //            {
        //                cols = prop.GetCustomAttributes(typeof(ColumnAttribute), true);
        //                if (cols.Length == 1)
        //                {
        //                    ColumnAttribute ca = cols[0] as ColumnAttribute;
        //                    if (dr[ca.ColumnName].GetType() != DBNull.Value.GetType())
        //                    {
        //                        prop.SetValue(this, dr[ca.ColumnName], null);
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (PropertyInfo prop in base.GetType().GetProperties())
        //            {
        //                cols = prop.GetCustomAttributes(true);
        //                if (cols.Length > 0)
        //                {
        //                    if (cols[0] is KeyAttribute)
        //                    {
        //                        prop.SetValue(this, null, null);
        //                    }
        //                    else if (cols[0] is ColumnAttribute)
        //                    {
        //                        prop.SetValue(this, null, null);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        dr.Close();
        //        dr.Dispose();
        //        this.con.Close();
        //        this.con.Dispose();
        //    }
        //}

        //public void getSelectDataKey_DataTable()
        //{
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = string.Format(_select, this.getTableName(base.GetType()), this.getColumnsParamKey(base.GetType()));
        //    this.setParameterValueKey(base.GetType(), ref this.cmd);
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.CommandTimeout = 0;
        //    this.cmd.Connection = this.con;
        //    try
        //    {
        //        object[] cols;
        //        this.dt = new DataTable();
        //        this.da = this.factory.CreateDataAdapter();
        //        this.da.SelectCommand = this.cmd;
        //        this.dt.BeginLoadData();
        //        this.da.Fill(this.dt);
        //        this.dt.EndLoadData();
        //        this.da.Dispose();
        //        if (this.dt.Rows.Count >= 1)
        //        {
        //            DataRow dr = this.dt.Rows[0];
        //            foreach (PropertyInfo prop in base.GetType().GetProperties())
        //            {
        //                cols = prop.GetCustomAttributes(typeof(ColumnAttribute), true);
        //                if (cols.Length > 0)
        //                {
        //                    if (cols.Length == 1)
        //                    {
        //                        ColumnAttribute ca = cols[0] as ColumnAttribute;
        //                        if (dr[ca.ColumnName].GetType() != DBNull.Value.GetType())
        //                        {
        //                            prop.SetValue(this, dr[ca.ColumnName], null);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (PropertyInfo prop in base.GetType().GetProperties())
        //            {
        //                cols = prop.GetCustomAttributes(true);
        //                if (cols.Length > 0)
        //                {
        //                    if (cols[0] is KeyAttribute)
        //                    {
        //                        prop.SetValue(this, null, null);
        //                    }
        //                    else if (cols[0] is ColumnAttribute)
        //                    {
        //                        prop.SetValue(this, null, null);
        //                    }
        //                }
        //            }
        //        }
        //        this.dt.Dispose();
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        this.con.Close();
        //        this.con.Dispose();
        //    }
        //}

        //public void getSelectDataWhere(string where)
        //{
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = string.Format(_selectWhere, this.getTableName(base.GetType()), where);
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.CommandTimeout = 0;
        //    this.cmd.Connection = this.con;
        //    DbDataReader dr = null;
        //    try
        //    {
        //        object[] cols;
        //        this.con.Open();
        //        dr = this.cmd.ExecuteReader();
        //        if (dr.Read())
        //        {
        //            foreach (PropertyInfo prop in base.GetType().GetProperties())
        //            {
        //                cols = prop.GetCustomAttributes(true);
        //                if (cols.Length == 1)
        //                {
        //                    if (cols[0] is KeyAttribute)
        //                    {
        //                        KeyAttribute ca = cols[0] as KeyAttribute;
        //                        if (dr[ca.KeyName].GetType() != DBNull.Value.GetType())
        //                        {
        //                            prop.SetValue(this, dr[ca.KeyName], null);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        ColumnAttribute ca = cols[0] as ColumnAttribute;
        //                        if (dr[ca.ColumnName].GetType() != DBNull.Value.GetType())
        //                        {
        //                            prop.SetValue(this, dr[ca.ColumnName], null);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (PropertyInfo prop in base.GetType().GetProperties())
        //            {
        //                cols = prop.GetCustomAttributes(true);
        //                if (cols.Length > 0)
        //                {
        //                    if (cols[0] is KeyAttribute)
        //                    {
        //                        prop.SetValue(this, null, null);
        //                    }
        //                    else if (cols[0] is ColumnAttribute)
        //                    {
        //                        prop.SetValue(this, null, null);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        dr.Close();
        //        dr.Dispose();
        //        this.con.Close();
        //        this.con.Dispose();
        //    }
        //}

        //public void getSelectDataWhere_Table(string where)
        //{
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = string.Format(_selectWhere, this.getTableName(base.GetType()), where);
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = this.con;
        //    try
        //    {
        //        object[] cols;
        //        this.dt = new DataTable();
        //        this.da = this.factory.CreateDataAdapter();
        //        this.da.SelectCommand = this.cmd;
        //        this.dt.BeginLoadData();
        //        this.da.Fill(this.dt);
        //        this.dt.EndLoadData();
        //        if (this.dt.Rows.Count >= 1)
        //        {
        //            DataRow dr = this.dt.Rows[0];
        //            foreach (PropertyInfo prop in base.GetType().GetProperties())
        //            {
        //                cols = prop.GetCustomAttributes(true);
        //                if (cols.Length == 1)
        //                {
        //                    if (cols[0] is KeyAttribute)
        //                    {
        //                        KeyAttribute ca = cols[0] as KeyAttribute;
        //                        if (dr[ca.KeyName].GetType() != DBNull.Value.GetType())
        //                        {
        //                            prop.SetValue(this, dr[ca.KeyName], null);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        ColumnAttribute ca = cols[0] as ColumnAttribute;
        //                        if (dr[ca.ColumnName].GetType() != DBNull.Value.GetType())
        //                        {
        //                            prop.SetValue(this, dr[ca.ColumnName], null);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (PropertyInfo prop in base.GetType().GetProperties())
        //            {
        //                cols = prop.GetCustomAttributes(true);
        //                if (cols[0] is KeyAttribute)
        //                {
        //                    prop.SetValue(this, null, null);
        //                }
        //                else if (cols[0] is ColumnAttribute)
        //                {
        //                    prop.SetValue(this, null, null);
        //                }
        //            }
        //        }
        //        this.dt.Dispose();
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        this.con.Close();
        //    }
        //}

        //public virtual int getSelectMax()
        //{
        //    int l_Result;
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = string.Format(_selectMax, this.getTableName(base.GetType()), this.getKeys()[0]);
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.CommandTimeout = 0;
        //    this.cmd.Connection = this.con;
        //    try
        //    {
        //        this.con.Open();
        //        l_Result = Convert.ToInt32(this.cmd.ExecuteScalar().ToString());
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        this.con.Close();
        //    }
        //    return l_Result;
        //}

        //public virtual int getSelectMax(string Key)
        //{
        //    int l_Result;
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = string.Format(_selectMax, this.getTableName(base.GetType()), Key);
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.CommandTimeout = 0;
        //    this.cmd.Connection = this.con;
        //    try
        //    {
        //        this.con.Open();
        //        l_Result = Convert.ToInt32(this.cmd.ExecuteScalar().ToString());
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        this.con.Close();
        //    }
        //    return l_Result;
        //}

        //private string getTableName(Type type)
        //{
        //    return ((TableAttribute)type.GetCustomAttributes(typeof(TableAttribute), true)[0]).TableName;
        //}

        //public string GetTableName()
        //{
        //    return this.getTableName(base.GetType());
        //}

        //private DbType getTypeAttribute(ColumnType coltype)
        //{
        //    switch (coltype)
        //    {
        //        case ColumnType.Int32:
        //            return DbType.Int32;

        //        case ColumnType.DateTime:
        //            return DbType.DateTime;

        //        case ColumnType.Boolean:
        //            return DbType.Boolean;

        //        case ColumnType.Byte:
        //            return DbType.Byte;

        //        case ColumnType.Decimal:
        //            return DbType.Decimal;

        //        case ColumnType.Double:
        //            return DbType.Double;

        //        case ColumnType.Single:
        //            return DbType.Single;

        //        case ColumnType.Int64:
        //            return DbType.Int64;

        //        case ColumnType.TimeSpan:
        //            return DbType.Time;

        //        case ColumnType.Image:
        //            return DbType.Binary;
        //    }
        //    return DbType.String;
        //}

        //private string getValues()
        //{
        //    string values = string.Empty;
        //    foreach (PropertyInfo field in base.GetType().GetProperties())
        //    {
        //        object[] cols = field.GetCustomAttributes(true);
        //        if (cols.Length == 1)
        //        {
        //            if (cols[0] is KeyAttribute)
        //            {
        //                KeyAttribute ca = cols[0] as KeyAttribute;
        //                if (!ca.Inc)
        //                {
        //                    if (field.PropertyType.FullName == "System.Int32[]")
        //                    {
        //                        values = values + "'" + Seguranca.SeguracaSql(((int[])field.GetValue(this, null))[0].ToString()) + "', ";
        //                    }
        //                    else
        //                    {
        //                        values = values + "'" + Seguranca.SeguracaSql(field.GetValue(this, null).ToString()) + "', ";
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                values = values + "'" + Seguranca.SeguracaSql(field.GetValue(this, null).ToString()) + "', ";
        //            }
        //        }
        //    }
        //    return values.Remove(values.Length - 2);
        //}

        //public object getValueScalar(string sql)
        //{
        //    object l_Result;
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = sql;
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = this.con;
        //    try
        //    {
        //        this.con.Open();
        //        l_Result = this.cmd.ExecuteScalar();
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        this.con.Close();
        //    }
        //    return l_Result;
        //}

        //public object getValueScalar(string sql, DbConnection con, DbTransaction trans)
        //{
        //    object l_Result;
        //    this.cmd = con.CreateCommand();
        //    this.cmd.CommandText = sql;
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = con;
        //    this.cmd.Transaction = trans;
        //    try
        //    {
        //        l_Result = this.cmd.ExecuteScalar();
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    return l_Result;
        //}

        //private string getValuesIsNotNull()
        //{
        //    string values = string.Empty;
        //    foreach (PropertyInfo field in base.GetType().GetProperties())
        //    {
        //        object[] cols = field.GetCustomAttributes(true);
        //        if (cols.Length == 1)
        //        {
        //            if (cols[0] is KeyAttribute)
        //            {
        //                KeyAttribute ca = cols[0] as KeyAttribute;
        //                if (!ca.Inc)
        //                {
        //                    if (field.PropertyType.FullName == "System.Int32[]")
        //                    {
        //                        values = values + "'" + Seguranca.SeguracaSql(((int[])field.GetValue(this, null))[0].ToString()) + "', ";
        //                    }
        //                    else if ((ca.CType == ColumnType.DateTime) && (this.getDataBase() == DataBaseType.MYSQL))
        //                    {
        //                        values = values + "'" + Convert.ToDateTime(field.GetValue(this, null)).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
        //                    }
        //                    else
        //                    {
        //                        values = values + "'" + Seguranca.SeguracaSql(field.GetValue(this, null).ToString()) + "', ";
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                ColumnAttribute ca = cols[0] as ColumnAttribute;
        //                if ((field.GetValue(this, null) != null) && !this.ValueIsNull(field))
        //                {
        //                    if (((ca.CType == ColumnType.Single) || (ca.CType == ColumnType.Double)) || (ca.CType == ColumnType.Decimal))
        //                    {
        //                        values = values + "'" + field.GetValue(this, null).ToString().Replace(",", ".") + "', ";
        //                    }
        //                    else if ((ca.CType == ColumnType.DateTime) && (this.getDataBase() == DataBaseType.MYSQL))
        //                    {
        //                        values = values + "'" + Convert.ToDateTime(field.GetValue(this, null)).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
        //                    }
        //                    else
        //                    {
        //                        values = values + "'" + Seguranca.SeguracaSql(field.GetValue(this, null).ToString()) + "', ";
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return values.Remove(values.Length - 2);
        //}

        //private void InicializeColumns(Type type)
        //{
        //    foreach (PropertyInfo prop in type.GetProperties())
        //    {
        //        object[] cols = null;
        //        cols = prop.GetCustomAttributes(true);
        //        if (prop.PropertyType.FullName == "System.Decimal")
        //        {
        //            prop.SetValue(this, -79228162514264337593543950335M, null);
        //        }
        //        else if (prop.PropertyType.FullName == "System.Double")
        //        {
        //            prop.SetValue(this, -1.7976931348623157E+308, null);
        //        }
        //        else if (prop.PropertyType.FullName == "System.Int32")
        //        {
        //            prop.SetValue(this, -2147483648, null);
        //        }
        //        else if (prop.PropertyType.FullName == "System.Date")
        //        {
        //            prop.SetValue(this, DateTime.MinValue, null);
        //        }
        //        else if (prop.PropertyType.FullName == "System.Byte")
        //        {
        //            prop.SetValue(this, (byte)0, null);
        //        }
        //        else
        //        {
        //            //prop.SetValue(this, null, null);
        //        }
        //    }
        //}

        //protected void InicializeVariaveisPrivate()
        //{
        //    if (this.getDataBase() == DataBaseType.MSSQL)
        //    {
        //        _selectDateFormat = " SET DATEFORMAT dmy ";
        //        _insertIdentity = "SET DATEFORMAT dmy INSERT INTO {0} ({1}) VALUES({2}) SELECT ISNULL(@@IDENTITY,0) ";
        //        _insert = "SET DATEFORMAT dmy INSERT INTO {0} ({1}) VALUES({2})";
        //        _update = "SET DATEFORMAT dmy UPDATE {0} SET {1} WHERE {2}";
        //        _delete = "DELETE FROM {0} WHERE {1}";
        //        _selectAll = _selectDateFormat + " SELECT * FROM {0}";
        //        _select = _selectDateFormat + "SELECT * FROM {0} WHERE {1}";
        //        _selectWhere = _selectDateFormat + "SELECT * FROM {0} {1}";
        //        _selectMax = "SELECT isnull( max( {1} ), 0 )+1 FROM {0} ";
        //        _tagParam = "@";
        //    }
        //    else if (this.getDataBase() == DataBaseType.MYSQL)
        //    {
        //        _selectDateFormat = "";
        //        _insertIdentity = " INSERT INTO {0} ({1}) VALUES({2})";
        //        _insert = "INSERT INTO {0} ({1}) VALUES({2})";
        //        _update = "UPDATE {0} SET {1} WHERE {2}";
        //        _delete = "DELETE FROM {0} WHERE {1}";
        //        _selectAll = "SELECT * FROM {0}";
        //        _select = "SELECT * FROM {0} WHERE {1}";
        //        _selectWhere = "SELECT * FROM {0} {1}";
        //        _selectMax = "SELECT coalesce( max( {1} )+1, 1 ) FROM {0} ";
        //        _tagParam = "?";
        //    }
        //}

        //public virtual bool Insert()
        //{
        //    bool l_Result;
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = string.Format(_insert, this.getTableName(base.GetType()), this.getColumns(base.GetType()), this.getColumnsParamInsert(base.GetType()));
        //    this.setParameterValueInsert(base.GetType(), ref this.cmd);
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = this.con;
        //    try
        //    {
        //        this.con.Open();
        //        if (this.cmd.ExecuteNonQuery() >= 1)
        //        {
        //            return true;
        //        }
        //        l_Result = false;
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException((dbE.Message == this.TrataErro(dbE.Message)) ? ("Problema ao inserir dados, contacte o suporte. Mensagem:" + dbE.Message) : this.TrataErro(dbE.Message));
        //    }
        //    finally
        //    {
        //        this.cmd.Dispose();
        //        this.con.Close();
        //    }
        //    return l_Result;
        //}

        //public virtual bool Insert(DbConnection con, DbTransaction trans)
        //{
        //    bool l_Result;
        //    this.cmd = con.CreateCommand();
        //    this.cmd.CommandText = string.Format(_insert, this.getTableName(base.GetType()), this.getColumns(base.GetType()), this.getColumnsParamInsert(base.GetType()));
        //    this.setParameterValueInsert(base.GetType(), ref this.cmd);
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = con;
        //    this.cmd.Transaction = trans;
        //    try
        //    {
        //        if (this.cmd.ExecuteNonQuery() >= 1)
        //        {
        //            return true;
        //        }
        //        l_Result = false;
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException((dbE.Message == this.TrataErro(dbE.Message)) ? ("Problema ao inserir dados, contacte o suporte. Mensagem:" + dbE.Message) : this.TrataErro(dbE.Message));
        //    }
        //    return l_Result;
        //}

        //public virtual int InsertIdentity()
        //{
        //    if (this.trans == null)
        //    {
        //        this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //        this.con = this.factory.CreateConnection();
        //        this.con.ConnectionString = this.getConnectionString();
        //    }
        //    this.cmd = this.con.CreateCommand();
        //    if (this.trans != null)
        //    {
        //        this.cmd.Transaction = this.trans;
        //    }
        //    this.cmd.CommandText = string.Format(_insertIdentity, this.getTableName(base.GetType()), this.getColumns(base.GetType()), this.getColumnsParamInsert(base.GetType()));
        //    this.setParameterValueInsert(base.GetType(), ref this.cmd);
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = this.con;
        //    int valor = 0;
        //    try
        //    {
        //        if (this.trans == null)
        //        {
        //            this.con.Open();
        //        }
        //        valor = Convert.ToInt32(this.cmd.ExecuteScalar().ToString());
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    finally
        //    {
        //        if (this.trans == null)
        //        {
        //            this.con.Close();
        //        }
        //    }
        //    return valor;
        //}

        //public virtual int InsertIdentity(DbConnection con, DbTransaction trans)
        //{
        //    this.cmd = con.CreateCommand();
        //    this.cmd.Transaction = trans;
        //    this.cmd.CommandText = string.Format(_insertIdentity, this.getTableName(base.GetType()), this.getColumns(base.GetType()), this.getColumnsParamInsert(base.GetType()));
        //    this.setParameterValueInsert(base.GetType(), ref this.cmd);
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = con;
        //    int valor = 0;
        //    try
        //    {
        //        valor = Convert.ToInt32(this.cmd.ExecuteScalar().ToString());
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    return valor;
        //}

        //public virtual int InsertIdentity(DbTransaction trans)
        //{
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.Transaction = trans;
        //    this.cmd.CommandText = string.Format(_insertIdentity, this.getTableName(base.GetType()), this.getColumns(base.GetType()), this.getColumnsParamInsert(base.GetType()));
        //    this.setParameterValueInsert(base.GetType(), ref this.cmd);
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = con;
        //    int valor = 0;
        //    try
        //    {
        //        valor = Convert.ToInt32(this.cmd.ExecuteScalar().ToString());
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
        //    }
        //    return valor;
        //}

        //private void setParameterValueInsert(Type type, ref DbCommand cmd)
        //{
        //    foreach (PropertyInfo prop in type.GetProperties())
        //    {
        //        object[] cols = prop.GetCustomAttributes(true);
        //        if (cols.Length == 1)
        //        {
        //            DbParameter param;
        //            if (cols[0] is ColumnAttribute)
        //            {
        //                if (!this.ValueIsNull(prop))
        //                {
        //                    ColumnAttribute ca = cols[0] as ColumnAttribute;
        //                    if (!ca.Inc)
        //                    {
        //                        param = cmd.CreateParameter();
        //                        param.ParameterName = _tagParam + ca.ColumnName;
        //                        param.DbType = this.getTypeAttribute(ca.CType);
        //                        if ((prop.GetValue(this, null) == DBNull.Value) || this.ValueIsNull(prop))
        //                        {
        //                            param.Value = DBNull.Value;
        //                        }
        //                        else
        //                        {
        //                            param.Value = prop.GetValue(this, null);
        //                        }
        //                        cmd.Parameters.Add(param);
        //                    }
        //                }
        //            }
        //            else if (cols[0] is KeyAttribute)
        //            {
        //                KeyAttribute ca = cols[0] as KeyAttribute;
        //                if (!ca.Inc)
        //                {
        //                    param = cmd.CreateParameter();
        //                    param.ParameterName = _tagParam + ca.KeyName;
        //                    param.DbType = this.getTypeAttribute(ca.CType);
        //                    if (prop.PropertyType.FullName == "System.Int32[]")
        //                    {
        //                        param.Value = ((int[])prop.GetValue(this, null))[0];
        //                    }
        //                    else
        //                    {
        //                        param.Value = prop.GetValue(this, null);
        //                    }
        //                    cmd.Parameters.Add(param);
        //                }
        //            }
        //        }
        //    }
        //}

        //private void setParameterValueKey(Type type, ref DbCommand cmd)
        //{
        //    foreach (PropertyInfo prop in type.GetProperties())
        //    {
        //        object[] cols = prop.GetCustomAttributes(true);
        //        if ((cols.Length == 1) && (cols[0] is KeyAttribute))
        //        {
        //            KeyAttribute ca = cols[0] as KeyAttribute;
        //            DbParameter param = cmd.CreateParameter();
        //            param.ParameterName = _tagParam + "_KEY_" + ca.KeyName;
        //            param.DbType = this.getTypeAttribute(ca.CType);
        //            if (prop.PropertyType.FullName == "System.Int32[]")
        //            {
        //                param.Value = ((int[])prop.GetValue(this, null))[0];
        //            }
        //            else if (prop.PropertyType.FullName == "System.DateTime[]")
        //            {
        //                param.Value = ((DateTime[])prop.GetValue(this, null))[0];
        //            }
        //            else
        //            {
        //                param.Value = prop.GetValue(this, null);
        //            }
        //            cmd.Parameters.Add(param);
        //        }
        //    }
        //}

        //private void setParameterValueUpdate(Type type, ref DbCommand cmd)
        //{
        //    foreach (PropertyInfo prop in type.GetProperties())
        //    {
        //        object[] cols = prop.GetCustomAttributes(true);
        //        if (cols.Length == 1)
        //        {
        //            DbParameter param;
        //            if (cols[0] is ColumnAttribute)
        //            {
        //                ColumnAttribute ca = cols[0] as ColumnAttribute;
        //                if (!ca.Inc)
        //                {
        //                    param = cmd.CreateParameter();
        //                    param.ParameterName = _tagParam + ca.ColumnName;
        //                    param.DbType = this.getTypeAttribute(ca.CType);
        //                    if ((prop.GetValue(this, null) == DBNull.Value) || this.ValueIsNull(prop))
        //                    {
        //                        param.Value = DBNull.Value;
        //                    }
        //                    else
        //                    {
        //                        param.Value = prop.GetValue(this, null);
        //                    }
        //                    cmd.Parameters.Add(param);
        //                }
        //            }
        //            else if (cols[0] is KeyAttribute)
        //            {
        //                KeyAttribute ca = cols[0] as KeyAttribute;
        //                if (!ca.Inc)
        //                {
        //                    param = cmd.CreateParameter();
        //                    param.ParameterName = _tagParam + ca.KeyName;
        //                    param.DbType = this.getTypeAttribute(ca.CType);
        //                    if (prop.PropertyType.FullName == "System.Int32[]")
        //                    {
        //                        param.Value = ((int[])prop.GetValue(this, null))[1];
        //                    }
        //                    else
        //                    {
        //                        param.Value = prop.GetValue(this, null);
        //                    }
        //                    cmd.Parameters.Add(param);
        //                }
        //            }
        //        }
        //    }
        //}

        //public List<DbParameter> ObterParametros(Type type)
        //{
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());

        //    List<DbParameter> Parametros = new List<DbParameter>();
        //    DbParameter Parametro;

        //    foreach (PropertyInfo prop in type.GetProperties())
        //    {
        //        object[] cols = prop.GetCustomAttributes(true);
        //        if (cols.Length == 1)
        //        {
        //            if (cols[0] is ColumnAttribute)
        //            {
        //                ColumnAttribute ca = cols[0] as ColumnAttribute;
        //                Parametro = this.factory.CreateParameter();
        //                Parametro.ParameterName = _tagParam + ca.ColumnName;
        //                Parametro.DbType = this.getTypeAttribute(ca.CType);

        //                if ((prop.GetValue(this, null) == DBNull.Value) || this.ValueIsNull(prop))
        //                {
        //                    Parametro.Value = DBNull.Value;
        //                }
        //                else
        //                {
        //                    Parametro.Value = prop.GetValue(this, null);
        //                }
        //                Parametros.Add(Parametro);
        //            }
        //        }
        //    }
        //    return Parametros;
        //}

        //private string TrataErro(string erro)
        //{
        //    if (erro.ToUpper().Contains("UNIQUE") || erro.ToUpper().Contains("DUPLICATE"))
        //    {
        //        if (erro.ToUpper().Contains("CANNOT INSERT DUPLICATE KEY") || erro.ToUpper().Contains("DUPLICATE"))
        //        {
        //            return "J\x00e1 existe registro com estas caracter\x00edsticas.";
        //        }
        //        return erro;
        //    }
        //    if (erro.ToUpper().Contains("COLUMN REFERENCE") || erro.ToUpper().Contains("FOREIGN KEY CONSTRAINT FAILS"))
        //    {
        //        return "Existem registros dependentes.";
        //    }
        //    if (erro.ToUpper().Contains("DATA WOULD BE TRUNCATED"))
        //    {
        //        return "Um campo ultrapassou o limite de catacteres permitido.";
        //    }
        //    return erro;
        //}

        //public virtual bool Update()
        //{
        //    bool l_Result;
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd = this.con.CreateCommand();
        //    this.cmd.CommandText = string.Format(_update, this.getTableName(base.GetType()), this.getColumnsParamUpdate(base.GetType()), this.getColumnsParamKey(base.GetType()));
        //    this.setParameterValueUpdate(base.GetType(), ref this.cmd);
        //    this.setParameterValueKey(base.GetType(), ref this.cmd);
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = this.con;
        //    try
        //    {
        //        this.con.Open();
        //        if (this.cmd.ExecuteNonQuery() >= 1)
        //        {
        //            return true;
        //        }
        //        l_Result = false;
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException((dbE.Message == this.TrataErro(dbE.Message)) ? ("Problema ao atualizar os dados, contacte o suporte. Mensagem:" + dbE.Message) : this.TrataErro(dbE.Message));
        //    }
        //    finally
        //    {
        //        if (this.trans == null)
        //        {
        //            this.con.Close();
        //        }
        //    }
        //    return l_Result;
        //}

        //public virtual bool Update(DbConnection con, DbTransaction trans)
        //{
        //    bool l_Result;
        //    this.cmd = con.CreateCommand();
        //    this.cmd.CommandText = string.Format(_update, this.getTableName(base.GetType()), this.getColumnsParamUpdate(base.GetType()), this.getColumnsParamKey(base.GetType()));
        //    this.setParameterValueUpdate(base.GetType(), ref this.cmd);
        //    this.setParameterValueKey(base.GetType(), ref this.cmd);
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = con;
        //    this.cmd.Transaction = trans;
        //    try
        //    {
        //        if (this.cmd.ExecuteNonQuery() >= 1)
        //        {
        //            return true;
        //        }
        //        l_Result = false;
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException((dbE.Message == this.TrataErro(dbE.Message)) ? ("Problema ao atualizar os dados, contacte o suporte. Mensagem:" + dbE.Message) : this.TrataErro(dbE.Message));
        //    }
        //    return l_Result;
        //}

        //public virtual bool Update( DbTransaction trans)
        //{
        //    bool l_Result;
        //    this.factory = DbProviderFactories.GetFactory(this.getProviderName());
        //    this.con = this.factory.CreateConnection();
        //    this.con.ConnectionString = this.getConnectionString();
        //    this.cmd.CommandText = string.Format(_update, this.getTableName(base.GetType()), this.getColumnsParamUpdate(base.GetType()), this.getColumnsParamKey(base.GetType()));
        //    this.setParameterValueUpdate(base.GetType(), ref this.cmd);
        //    this.setParameterValueKey(base.GetType(), ref this.cmd);
        //    this.cmd.CommandType = CommandType.Text;
        //    this.cmd.Connection = con;
        //    this.cmd.Transaction = trans;
        //    try
        //    {
        //        if (this.cmd.ExecuteNonQuery() >= 1)
        //        {
        //            return true;
        //        }
        //        l_Result = false;
        //    }
        //    catch (DbException dbE)
        //    {
        //        throw new ArgumentException((dbE.Message == this.TrataErro(dbE.Message)) ? ("Problema ao atualizar os dados, contacte o suporte. Mensagem:" + dbE.Message) : this.TrataErro(dbE.Message));
        //    }
        //    return l_Result;
        //}

        //private bool ValueIsNull(PropertyInfo prop)
        //{
        //    return ((prop.GetValue(this, null) == null) || (((prop.PropertyType.FullName == "System.DateTime") && (((prop.GetValue(this, null).ToString() == "01/01/0001 00:00:00") || (prop.GetValue(this, null).ToString() == "1/1/0001 00:00:00")) || (prop.GetValue(this, null).ToString() == DateTime.MinValue.ToString()))) || (((prop.PropertyType.FullName == "System.Int32") && (prop.GetValue(this, null).ToString() == 2147483648.ToString())) || (((prop.PropertyType.FullName == "System.Double") && (prop.GetValue(this, null).ToString() == 1.7976931348623157E+308.ToString())) || ((prop.PropertyType.FullName == "System.Decimal") && (prop.GetValue(this, null).ToString() == 79228162514264337593543950335M.ToString()))))));
        //}

        //// Nested Types
        //public enum DataBaseType
        //{
        //    MSSQL,
        //    MYSQL
        //}

        #endregion

    }
}
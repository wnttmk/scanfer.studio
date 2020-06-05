using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace CGDemo.MySql
{
    public class DbObject : IDbObject
    {
        // Fields
        //private string cmcfgfile;
        //private INIFile cfgfile;
        //private bool isdbosp;
        private string _dbconnectStr;
        private MySqlConnection connect;

        // Methods
        public DbObject()
        {
            //this.cmcfgfile = AppDomain.CurrentDomain.BaseDirectory + @"\cmcfg.ini";
            this.connect = new MySqlConnection();
            this.IsDboSp();
        }

        public DbObject(string DbConnectStr)
        {
            //this.cmcfgfile = AppDomain.CurrentDomain.BaseDirectory + @"\cmcfg.ini";
            this.connect = new MySqlConnection();
            this._dbconnectStr = DbConnectStr;
            this.connect.ConnectionString = DbConnectStr;
        }

        public DbObject(bool SSPI, string Ip, string User, string Pass)
        {
            //this.cmcfgfile = AppDomain.CurrentDomain.BaseDirectory + @"\cmcfg.ini";
         
            this.connect = new MySqlConnection();
            if (SSPI)
            {
                this._dbconnectStr = $"server={Ip};user id={User}; password={Pass}; database=mysql; pooling=false";
            }
            else
            {
                this._dbconnectStr = $"server={Ip};user id={User}; password={Pass}; database=mysql; pooling=false";
            }
            this.connect.ConnectionString = this._dbconnectStr;
        }

        private MySqlCommand BuildQueryCommand(MySqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            MySqlCommand command = new MySqlCommand(storedProcName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            foreach (MySqlParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    if (((parameter.Direction == ParameterDirection.InputOutput) || (parameter.Direction == ParameterDirection.Input)) && (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }
            return command;
        }

        private int CompareStrByOrder(string x, string y)
        {
            if (x == "")
            {
                if (y == "")
                {
                    return 0;
                }
                return -1;
            }
            if (y == "")
            {
                return 1;
            }
            return x.CompareTo(y);
        }

        public DataTable CreateColumnTable()
        {
            DataTable table = new DataTable();
            DataColumn column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "colorder"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "ColumnName"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "TypeName"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "Length"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "Preci"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "Scale"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "IsIdentity"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "isPK"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "cisNull"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "defaultVal"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "deText"
            };
            table.Columns.Add(column);
            return table;
        }

        public bool DeleteTable(string DbName, string TableName)
        {
            try
            {
                MySqlCommand command = this.OpenDB(DbName);
                command.CommandText = "DROP TABLE " + TableName;
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public MySqlDataReader ExecuteReader(string DbName, string strSQL)
        {
            MySqlDataReader reader2;
            try
            {
                this.OpenDB(DbName);
                reader2 = new MySqlCommand(strSQL, this.connect).ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (MySqlException exception)
            {
                throw exception;
            }
            return reader2;
        }

        public int ExecuteSql(string DbName, string SQLString)
        {
            MySqlCommand command = this.OpenDB(DbName);
            command.CommandText = SQLString;
            return command.ExecuteNonQuery();
        }

        public List<ColumnInfo> GetColumnInfoList(string DbName, string TableName)
        {
            List<ColumnInfo> list2;
            try
            {
                string strSQL = "SHOW COLUMNS FROM " + TableName;
                List<ColumnInfo> list = new List<ColumnInfo>();
                MySqlDataReader reader = this.ExecuteReader(DbName, strSQL);
                for (int i = 1; reader.Read(); i++)
                {
                    string str4;
                    ColumnInfo item = new ColumnInfo
                    {
                        ColumnOrder = i.ToString()
                    };
                    if (!object.Equals(reader["Field"], null) && !object.Equals(reader["Field"], DBNull.Value))
                    {
                        switch (reader["Field"].GetType().Name)
                        {
                            case "Byte[]":
                                item.ColumnName = Encoding.Default.GetString((byte[])reader["Field"]);
                                goto Label_00D7;

                            case "":
                                goto Label_00D7;
                        }
                        item.ColumnName = reader["Field"].ToString();
                    }
                Label_00D7:
                    if (!object.Equals(reader["Type"], null) && !object.Equals(reader["Type"], DBNull.Value))
                    {
                        switch (reader["Type"].GetType().Name)
                        {
                            case "Byte[]":
                                item.TypeName = Encoding.Default.GetString((byte[])reader["Type"]);
                                goto Label_0178;

                            case "":
                                goto Label_0178;
                        }
                        item.TypeName = reader["Type"].ToString();
                    }
                Label_0178:
                    str4 = item.TypeName;
                    string length = "";
                    string preci = "";
                    string scale = "";
                    this.TypeNameProcess(item.TypeName, out str4, out length, out preci, out scale);
                    item.TypeName = str4;
                    item.Length = length;
                    item.Precision = preci;
                    item.Scale = scale;
                    if (!object.Equals(reader["Key"], null) && !object.Equals(reader["Key"], DBNull.Value))
                    {
                        string str8 = "";
                        switch (reader["Key"].GetType().Name)
                        {
                            case "Byte[]":
                                str8 = Encoding.Default.GetString((byte[])reader["Key"]);
                                break;

                            case "":
                                break;

                            default:
                                str8 = reader["Key"].ToString();
                                break;
                        }
                        item.IsPrimaryKey = str8.Trim() == "PRI";
                    }
                    if (!object.Equals(reader["Null"], null) && !object.Equals(reader["Null"], DBNull.Value))
                    {
                        string str10 = "";
                        switch (reader["Null"].GetType().Name)
                        {
                            case "Byte[]":
                                str10 = Encoding.Default.GetString((byte[])reader["Null"]);
                                break;

                            case "":
                                break;

                            default:
                                str10 = reader["Null"].ToString();
                                break;
                        }
                        item.Nullable = str10.Trim() == "YES";
                    }
                    if (!object.Equals(reader["Default"], null) && !object.Equals(reader["Default"], DBNull.Value))
                    {
                        switch (reader["Default"].GetType().Name)
                        {
                            case "Byte[]":
                                item.DefaultVal = Encoding.Default.GetString((byte[])reader["Default"]);
                                goto Label_03EA;

                            case "":
                                goto Label_03EA;
                        }
                        item.DefaultVal = reader["Default"].ToString();
                    }
                Label_03EA:
                    item.IsIdentity = false;
                    if (!object.Equals(reader["Extra"], null) && !object.Equals(reader["Extra"], DBNull.Value))
                    {
                        switch (reader["Extra"].GetType().Name)
                        {
                            case "Byte[]":
                                item.Description = Encoding.Default.GetString((byte[])reader["Extra"]);
                                break;

                            case "":
                                break;

                            default:
                                item.Description = reader["Extra"].ToString();
                                break;
                        }
                        if (item.Description.Trim() == "auto_increment")
                        {
                            item.IsIdentity = true;
                        }
                    }
                    list.Add(item);
                }
                reader.Close();
                list2 = list;
            }
            catch (Exception exception)
            {
                throw new Exception("获取列数据失败" + exception.Message);
            }
            return list2;
        }

        public DataTable GetColumnInfoListSP(string DbName, string TableName) =>
            null;

        public List<ColumnInfo> GetColumnList(string DbName, string TableName) =>
            this.GetColumnInfoList(DbName, TableName);

        public List<ColumnInfo> GetColumnListSP(string DbName, string TableName) =>
            this.GetColumnInfoList(DbName, TableName);

        public List<string> GetDBList()
        {
            List<string> list = new List<string>();
            string strSQL = "SHOW DATABASES";
            MySqlDataReader reader = this.ExecuteReader("mysql", strSQL);
            while (reader.Read())
            {
                list.Add(reader.GetString(0));
            }
            reader.Close();
            list.Sort(new Comparison<string>(this.CompareStrByOrder));
            return list;
        }

        public List<ColumnInfo> GetFKeyList(string DbName, string TableName)
        {
            List<ColumnInfo> columnInfoList = this.GetColumnInfoList(DbName, TableName);
            List<ColumnInfo> list2 = new List<ColumnInfo>();
            foreach (ColumnInfo info in columnInfoList)
            {
                if (info.IsForeignKey)
                {
                    list2.Add(info);
                }
            }
            return list2;
        }

        public List<ColumnInfo> GetKeyList(string DbName, string TableName)
        {
            List<ColumnInfo> columnInfoList = this.GetColumnInfoList(DbName, TableName);
            List<ColumnInfo> list2 = new List<ColumnInfo>();
            foreach (ColumnInfo info in columnInfoList)
            {
                if (info.IsPrimaryKey || info.IsIdentity)
                {
                    list2.Add(info);
                }
            }
            return list2;
        }

        public DataTable GetKeyName(string DbName, string TableName)
        {
            DataTable table = this.CreateColumnTable();
            foreach (DataRow row in CodeCommon.GetColumnInfoDt(this.GetColumnInfoList(DbName, TableName)).Select(" isPK='√' or IsIdentity='√' "))
            {
                DataRow row2 = table.NewRow();
                row2["colorder"] = row["colorder"];
                row2["ColumnName"] = row["ColumnName"];
                row2["TypeName"] = row["TypeName"];
                row2["Length"] = row["Length"];
                row2["Preci"] = row["Preci"];
                row2["Scale"] = row["Scale"];
                row2["IsIdentity"] = row["IsIdentity"];
                row2["isPK"] = row["isPK"];
                row2["cisNull"] = row["cisNull"];
                row2["defaultVal"] = row["defaultVal"];
                row2["deText"] = row["deText"];
                table.Rows.Add(row2);
            }
            return table;
        }

        public string GetObjectInfo(string DbName, string objName) =>
            "";

        public List<TableInfo> GetProcInfo(string DbName) =>
            null;

        public List<string> GetProcs(string DbName)
        {
            string sQLString = "show procedure status where db='" + DbName + "'";
            DataTable table = this.Query(DbName, sQLString).Tables[0];
            table.Columns[0].ColumnName = "name";
            List<string> list = new List<string>();
            if (table != null)
            {
                foreach (DataRow row in table.Select("", "name ASC"))
                {
                    list.Add(row["name"].ToString());
                }
            }
            return list;
        }

        public object GetSingle(string DbName, string SQLString)
        {
            try
            {
                MySqlCommand command = this.OpenDB(DbName);
                command.CommandText = SQLString;
                object objA = command.ExecuteScalar();
                if (object.Equals(objA, null) || object.Equals(objA, DBNull.Value))
                {
                    return null;
                }
                return objA;
            }
            catch
            {
                return null;
            }
        }

        public DataTable GetTabData(string DbName, string TableName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("select * from " + TableName);
            return this.Query(DbName, builder.ToString()).Tables[0];
        }

        public DataTable GetTabDataBySQL(string DbName, string strSQL) =>
            this.Query(DbName, strSQL).Tables[0];

        public List<string> GetTables(string DbName)
        {
            string strSQL = "SHOW TABLES";
            List<string> list = new List<string>();
            MySqlDataReader reader = this.ExecuteReader(DbName, strSQL);
            while (reader.Read())
            {
                list.Add(reader.GetString(0));
            }
            reader.Close();
            list.Sort(new Comparison<string>(this.CompareStrByOrder));
            return list;
        }

        public string GetTableScript(string DbName, string TableName)
        {
            string str = "";
            string strSQL = "SHOW CREATE TABLE " + TableName;
            MySqlDataReader reader = this.ExecuteReader(DbName, strSQL);
            while (reader.Read())
            {
                str = reader.GetString(1);
            }
            reader.Close();
            return str;
        }

        public DataTable GetTablesExProperty(string DbName) =>
            null;

        public List<TableInfo> GetTablesInfo(string DbName)
        {
            List<TableInfo> list = new List<TableInfo>();
            string strSQL = "SHOW TABLE STATUS";
            MySqlDataReader reader = this.ExecuteReader(DbName, strSQL);
            while (reader.Read())
            {
                TableInfo item = new TableInfo
                {
                    TabName = reader.GetString("Name")
                };
                try
                {
                    if (reader["Create_time"] != null)
                    {
                        item.TabDate = reader.GetString("Create_time");
                    }
                }
                catch
                {
                }
                item.TabType = "U";
                item.TabUser = "dbo";
                list.Add(item);
            }
            reader.Close();
            return list;
        }

        public DataTable GetTablesSP(string DbName)
        {
            try
            {
                MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("@table_name", MySqlDbType.VarChar, 0x180), new MySqlParameter("@table_owner", MySqlDbType.VarChar, 0x180), new MySqlParameter("@table_qualifier", MySqlDbType.VarChar, 0x180), new MySqlParameter("@table_type", MySqlDbType.VarChar, 100) };
                parameters[0].Value = null;
                parameters[1].Value = null;
                parameters[2].Value = null;
                parameters[3].Value = "'TABLE'";
                DataSet set = this.RunProcedure(DbName, "sp_tables", parameters, "ds");
                if (set.Tables.Count > 0)
                {
                    DataTable table = set.Tables[0];
                    table.Columns["TABLE_QUALIFIER"].ColumnName = "db";
                    table.Columns["TABLE_OWNER"].ColumnName = "cuser";
                    table.Columns["TABLE_NAME"].ColumnName = "name";
                    table.Columns["TABLE_TYPE"].ColumnName = "type";
                    table.Columns["REMARKS"].ColumnName = "remarks";
                    return table;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public List<string> GetTableViews(string DbName)
        {
            List<string> tables = this.GetTables(DbName);
            DataTable vIEWsSP = this.GetVIEWsSP(DbName);
            if (vIEWsSP != null)
            {
                foreach (DataRow row in vIEWsSP.Rows)
                {
                    string item = row["name"].ToString();
                    tables.Add(item);
                }
            }
            return tables;
        }

        public DataTable GetTabViews(string DbName)
        {
            string sQLString = "SHOW TABLES";
            DataTable table = this.Query(DbName, sQLString).Tables[0];
            table.Columns[0].ColumnName = "name";
            return table;
        }

        public List<TableInfo> GetTabViewsInfo(string DbName)
        {
            List<TableInfo> list = new List<TableInfo>();
            string strSQL = "SHOW TABLE STATUS";
            MySqlDataReader reader = this.ExecuteReader(DbName, strSQL);
            while (reader.Read())
            {
                TableInfo item = new TableInfo
                {
                    TabName = reader.GetString("Name")
                };
                try
                {
                    if (reader["Create_time"] != null)
                    {
                        item.TabDate = reader.GetString("Create_time");
                    }
                }
                catch
                {
                }
                item.TabType = "U";
                item.TabUser = "dbo";
                list.Add(item);
            }
            reader.Close();
            return list;
        }

        public DataTable GetTabViewsSP(string DbName) =>
            null;

        public string GetVersion()
        {
            try
            {
                string sQLString = "execute master..sp_msgetversion ";
                return this.Query("master", sQLString).Tables[0].Rows[0][0].ToString();
            }
            catch
            {
                return "";
            }
        }

        public DataTable GetVIEWs(string DbName) =>
            this.GetVIEWsSP(DbName);

        public List<TableInfo> GetVIEWsInfo(string DbName) =>
            null;

        public DataTable GetVIEWsSP(string DbName)
        {
            try
            {
                MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("@table_name", MySqlDbType.VarChar, 0x180), new MySqlParameter("@table_owner", MySqlDbType.VarChar, 0x180), new MySqlParameter("@table_qualifier", MySqlDbType.VarChar, 0x180), new MySqlParameter("@table_type", MySqlDbType.VarChar, 100) };
                parameters[0].Value = null;
                parameters[1].Value = null;
                parameters[2].Value = null;
                parameters[3].Value = "'VIEW'";
                DataSet set = this.RunProcedure(DbName, "sp_tables", parameters, "ds");
                if (set.Tables.Count > 0)
                {
                    DataTable table = set.Tables[0];
                    table.Columns["TABLE_QUALIFIER"].ColumnName = "db";
                    table.Columns["TABLE_OWNER"].ColumnName = "cuser";
                    table.Columns["TABLE_NAME"].ColumnName = "name";
                    table.Columns["TABLE_TYPE"].ColumnName = "type";
                    table.Columns["REMARKS"].ColumnName = "remarks";
                    return table;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private bool IsDboSp()
        {
            //if (File.Exists(this.cmcfgfile))
            //{
            //    this.cfgfile = new INIFile(this.cmcfgfile);
            //    if (this.cfgfile.IniReadValue("dbo", "dbosp").Trim() == "1")
            //    {
            //        this.isdbosp = true;
            //    }
            //}
            //return this.isdbosp;
            //this.isdbosp = false;
            return false;
        }

        private MySqlCommand OpenDB(string DbName)
        {
            try
            {
                if (this.connect.ConnectionString == "")
                {
                    this.connect.ConnectionString = this._dbconnectStr;
                }
                if (this.connect.ConnectionString != this._dbconnectStr)
                {
                    this.connect.Close();
                    this.connect.ConnectionString = this._dbconnectStr;
                }
                MySqlCommand command = new MySqlCommand
                {
                    Connection = this.connect
                };
                if (this.connect.State == ConnectionState.Closed)
                {
                    this.connect.Open();
                }
                command.CommandText = "use " + DbName;
                command.ExecuteNonQuery();
                return command;
            }
            catch (Exception exception)
            {
                string message = exception.Message;
                return null;
            }
        }

        public DataSet Query(string DbName, string SQLString)
        {
            DataSet dataSet = new DataSet();
            try
            {
                this.OpenDB(DbName);
                new MySqlDataAdapter(SQLString, this.connect).Fill(dataSet, "ds");
            }
            catch (MySqlException exception)
            {
                throw new Exception(exception.Message);
            }
            return dataSet;
        }

        public bool RenameTable(string DbName, string OldName, string NewName)
        {
            try
            {
                MySqlCommand command = this.OpenDB(DbName);
                command.CommandText = "RENAME TABLE " + OldName + " TO " + NewName;
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public DataSet RunProcedure(string DbName, string storedProcName, IDataParameter[] parameters, string tableName)
        {
            this.OpenDB(DbName);
            DataSet dataSet = new DataSet();
            new MySqlDataAdapter { SelectCommand = this.BuildQueryCommand(this.connect, storedProcName, parameters) }.Fill(dataSet, tableName);
            return dataSet;
        }

        private void TypeNameProcess(string strName, out string TypeName, out string Length, out string Preci, out string Scale)
        {
            TypeName = strName;
            int index = strName.IndexOf("(");
            Length = "";
            Preci = "";
            Scale = "";
            if (index > 0)
            {
                TypeName = strName.Substring(0, index);
                switch (TypeName.Trim().ToUpper())
                {
                    case "TINYINT":
                    case "SMALLINT":
                    case "MEDIUMINT":
                    case "INT":
                    case "INTEGER":
                    case "BIGINT":
                    case "TIMESTAMP":
                    case "CHAR":
                    case "VARCHAR":
                        {
                            int num2 = strName.IndexOf(")");
                            Length = strName.Substring(index + 1, (num2 - index) - 1);
                            return;
                        }
                    case "FLOAT":
                    case "DOUBLE":
                    case "REAL":
                    case "DECIMAL":
                    case "DEC":
                    case "NUMERIC":
                        {
                            int num3 = strName.IndexOf(")");
                            string str = strName.Substring(index + 1, (num3 - index) - 1);
                            int length = str.IndexOf(",");
                            Length = str.Substring(0, length);
                            Scale = str.Substring(length + 1);
                            return;
                        }
                    case "ENUM":
                    case "SET":
                        return;
                }
            }
        }

        // Properties
        public string DbType =>
            "MySQL";

        public string DbConnectStr
        {
            get =>
                this._dbconnectStr;
            set =>
                this._dbconnectStr = value;
        }
    }





}

using Microsoft.VisualStudio.TextTemplating;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;


namespace CGDemo
{
    class Program
    {

        static string dbname = "";
        static string tablename = "";
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //new Engine();
            new CodeGenerateDemo(
                "127.0.0.1(MySQL)(codeGenerate)", "codegenerate", "CG_PhysicalTable", "table", "Test.cmt"
                ).Generate();



            //string path = null;
            //TemplateHost host = new TemplateHost
            //{
            //    TemplateFile = path
            //};
            //string input = File.ReadAllText(path);
            ////var content = new Microsoft.VisualStudio.TextTemplating.Engine().ProcessTemplate(input, host);
            //var result = GenerateCode(input, host);
            //Console.WriteLine("content");
            //var k = Console.ReadLine();
        }





    }

    public class CodeGenerateDemo
    {


        private DbSettings dbset;
        private IDbObject dbobj;
        private string Templatefilename;
        private string servername;
        private string dbname;
        private string tablename;
        private string objtype;
        private string tableDescription;

        public CodeGenerateDemo(string servername, string dbname, string tablename, string tableDescription
            , string templatefilename, string objtype = "table"
            )
        {
            this.objtype = objtype;
            this.servername = servername;
            this.dbname = dbname;
            this.tablename = tablename;
            this.tableDescription = tableDescription;
            this.Templatefilename = templatefilename;
            this.dbset = DbConfig.GetSetting(this.servername);

            this.dbobj = new CGDemo.MySql.DbObject(true, "127.0.0.1", "root", "123456");
            //this.dbobj = ObjHelper.CreatDbObj(this.servername);
        }


        public string Generate()
        {
            CodeInfo code = new CodeInfo();
            string strContent = null;
            try
            {


                code = new BuilderTemp(this.dbobj, this.dbname, this.tablename, this.tableDescription, this.GetFieldlist(), this.GetKeyFields(), this.GetFKeyFields(), this.Templatefilename, this.dbset, this.objtype).GetCode();
                if ((code.ErrorMsg != null) && (code.ErrorMsg.Length > 0))
                {
                    strContent = code.Code + Environment.NewLine + "/*------ 代码生成时出现错误: ------" + Environment.NewLine + code.ErrorMsg + "*/";
                }
                else
                {
                    strContent = code.Code;
                }
            }
            catch (Exception exception2)
            {


            }
            return strContent;
        }

        private List<ColumnInfo> GetFKeyFields() =>
    this.dbobj.GetFKeyList(this.dbname, this.tablename);

        private List<ColumnInfo> GetKeyFields()
        {
            DataRow[] rowArray;
            DataTable columnInfoDt = CodeCommon.GetColumnInfoDt(this.dbobj.GetColumnInfoList(this.dbname, this.tablename));
            StringPlus plus = new StringPlus();
            List<ColumnInfo> columnList = this.dbobj.GetColumnList(this.dbname, this.tablename);
            foreach (ColumnInfo info in columnList)
            {
                string columnOrder = info.ColumnOrder;
                string columnName = info.ColumnName;
                string typeName = info.TypeName;
                string length = info.Length;
                string precision = info.Precision;
                string scale = info.Scale;
                string defaultVal = info.DefaultVal;
                string description = info.Description;
                string str7 = info.IsIdentity ? "√" : "";
                string text = info.IsPrimaryKey ? "√" : "";
                string str9 = info.Nullable ? "√" : "";

                if ((text == "√") && (str9.Trim() == ""))
                {
                    //this.list_KeyField.Items.Add(columnName);
                    //plus.Append(columnName + ",");
                    plus.Append("'" + columnName + "',");

                }
            }
            plus.DelLastComma();
            if (columnInfoDt == null)
            {
                return null;
            }
            if (plus.Value.Length > 0)
            {
                //rowArray = columnInfoDt.Select("ColumnName in ('" + plus.Value + "')", "colorder asc");
                rowArray = columnInfoDt.Select("ColumnName in (" + plus.Value + ")", "colorder asc");
            }
            else
            {
                rowArray = columnInfoDt.Select();
            }
            List<ColumnInfo> list2 = new List<ColumnInfo>();
            foreach (DataRow row in rowArray)
            {
                string str = row["Colorder"].ToString();
                string str2 = row["ColumnName"].ToString();
                string str3 = row["TypeName"].ToString();
                string str4 = row["IsIdentity"].ToString();
                string str5 = row["IsPK"].ToString();
                string str6 = row["Length"].ToString();
                string str7 = row["Preci"].ToString();
                string str8 = row["Scale"].ToString();
                string str9 = row["cisNull"].ToString();
                string str10 = row["DefaultVal"].ToString();
                string str11 = row["DeText"].ToString();
                ColumnInfo item = new ColumnInfo
                {
                    ColumnOrder = str,
                    ColumnName = str2,
                    TypeName = str3,
                    IsIdentity = (str4 == "√") ? true : false,
                    IsPrimaryKey = (str5 == "√") ? true : false,
                    Length = str6,
                    Precision = str7,
                    Scale = str8,
                    Nullable = (str9 == "√") ? true : false,
                    DefaultVal = str10,
                    Description = str11
                };
                list2.Add(item);
            }
            return list2;
        }

        public List<ColumnInfo> GetFieldlist()
        {

            List<ColumnInfo> columnList;
            DataRow[] rowArray;
            if (this.objtype == "proc")
            {
                columnList = this.dbobj.GetColumnList(this.dbname, this.tablename);
            }
            else
            {
                columnList = this.dbobj.GetColumnInfoList(this.dbname, this.tablename);
            }
            DataTable columnInfoDt = CodeCommon.GetColumnInfoDt(columnList);
            StringPlus plus = new StringPlus();
            foreach (DataColumn item in columnInfoDt.Columns)
            {

                plus.Append("'" + item.ColumnName + "',");

            }
            plus.DelLastComma();
            if (columnInfoDt == null)
            {
                return null;
            }
            if (plus.Value.Length > 0)
            {
                rowArray = columnInfoDt.Select("ColumnName in (" + plus.Value + ")", "colorder asc");
            }
            else
            {
                rowArray = columnInfoDt.Select();
            }
            List<ColumnInfo> list2 = new List<ColumnInfo>();
            foreach (DataRow row in rowArray)
            {
                string str = row["Colorder"].ToString();
                string str2 = row["ColumnName"].ToString();
                string str3 = row["TypeName"].ToString();
                string str4 = row["IsIdentity"].ToString();
                string str5 = row["IsPK"].ToString();
                string str6 = row["Length"].ToString();
                string str7 = row["Preci"].ToString();
                string str8 = row["Scale"].ToString();
                string str9 = row["cisNull"].ToString();
                string str10 = row["DefaultVal"].ToString();
                string str11 = row["DeText"].ToString();
                ColumnInfo info = new ColumnInfo
                {
                    ColumnOrder = str,
                    ColumnName = str2,
                    TypeName = str3,
                    IsIdentity = (str4 == "√") ? true : false,
                    IsPrimaryKey = (str5 == "√") ? true : false,
                    Length = str6,
                    Precision = str7,
                    Scale = str8,
                    Nullable = (str9 == "√") ? true : false,
                    DefaultVal = str10,
                    Description = str11
                };
                list2.Add(info);
            }
            return list2;
        }

    }

    public interface IDbObject
    {
        // Methods
        bool DeleteTable(string DbName, string TableName);
        int ExecuteSql(string DbName, string SQLString);
        List<ColumnInfo> GetColumnInfoList(string DbName, string TableName);
        List<ColumnInfo> GetColumnList(string DbName, string TableName);
        List<string> GetDBList();
        List<ColumnInfo> GetFKeyList(string DbName, string TableName);
        List<ColumnInfo> GetKeyList(string DbName, string TableName);
        DataTable GetKeyName(string DbName, string TableName);
        string GetObjectInfo(string DbName, string objName);
        List<TableInfo> GetProcInfo(string DbName);
        List<string> GetProcs(string DbName);
        DataTable GetTabData(string DbName, string TableName);
        List<string> GetTables(string DbName);
        DataTable GetTablesExProperty(string DbName);
        List<TableInfo> GetTablesInfo(string DbName);
        List<string> GetTableViews(string DbName);
        DataTable GetTabViews(string DbName);
        List<TableInfo> GetTabViewsInfo(string DbName);
        string GetVersion();
        DataTable GetVIEWs(string DbName);
        List<TableInfo> GetVIEWsInfo(string DbName);
        DataSet Query(string DbName, string SQLString);
        bool RenameTable(string DbName, string OldName, string NewName);

        // Properties
        string DbType { get; }
        string DbConnectStr { get; set; }
    }


    public class Cache
    {
        // Fields
        protected Hashtable _Cache = new Hashtable();
        protected object _LockObj = new object();

        // Methods
        public virtual void Clear()
        {
            lock (this._Cache.SyncRoot)
            {
                this._Cache.Clear();
            }
        }

        public virtual void DelObject(object key)
        {
            lock (this._Cache.SyncRoot)
            {
                this._Cache.Remove(key);
            }
        }

        public virtual object GetObject(object key)
        {
            if (this._Cache.ContainsKey(key))
            {
                return this._Cache[key];
            }
            return null;
        }

        private void Results(IAsyncResult ar)
        {
            //((EventSaveCache)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);
        }

        public void SaveCache(object key, object value)
        {
            new EventSaveCache(this.SetCache).BeginInvoke(key, value, new AsyncCallback(this.Results), null);
        }

        protected virtual void SetCache(object key, object value)
        {
            lock (this._LockObj)
            {
                if (!this._Cache.ContainsKey(key))
                {
                    this._Cache.Add(key, value);
                }
            }
        }

        // Properties
        public int Count =>
            this._Cache.Count;
    }





    internal delegate void EventSaveCache(object key, object value);








    public class DBOMaker
    {
        // Fields
        private static Cache cache = new Cache();

        // Methods
        public static IDbObject CreateDbObj(string dbTypename)
        {
            string typeName = "Maticsoft.DbObjects." + dbTypename + ".DbObject";
            return (IDbObject)CreateObject("Maticsoft.DbObjects", typeName);
        }

        private static object CreateObject(string path, string TypeName)
        {
            object obj2 = cache.GetObject(TypeName);
            if (obj2 == null)
            {
                try
                {
                    obj2 = Assembly.Load(path).CreateInstance(TypeName);
                    cache.SaveCache(TypeName, obj2);
                }
                catch (Exception exception)
                {
                    string message = exception.Message;
                }
            }
            return obj2;
        }

        public static IDbScriptBuilder CreateScript(string dbTypename)
        {
            string typeName = "Maticsoft.DbObjects." + dbTypename + ".DbScriptBuilder";
            return (IDbScriptBuilder)CreateObject("Maticsoft.DbObjects", typeName);
        }
    }

    public interface IDbScriptBuilder
    {
        // Methods
        string CreateDBTabScript(string dbname);
        string CreateTabScript(string dbname, string tablename);
        void CreateTabScript(string dbname, string tablename, string filename);
        string CreateTabScriptBySQL(string dbname, string strSQL);
        string CreatPROCADD();
        string CreatPROCDelete();
        string CreatPROCGetList();
        string CreatPROCGetMaxID();
        string CreatPROCGetModel();
        string CreatPROCIsHas();
        string CreatPROCUpdate();
        string GetPROCCode(string dbname);
        string GetPROCCode(string dbname, string tablename);
        string GetPROCCode(bool Maxid, bool Ishas, bool Add, bool Update, bool Delete, bool GetModel, bool List);
        string GetSQLDelete(string dbname, string tablename);
        string GetSQLInsert(string dbname, string tablename);
        string GetSQLSelect(string dbname, string tablename);
        string GetSQLUpdate(string dbname, string tablename);

        // Properties
        string DbConnectStr { get; set; }
        string DbName { get; set; }
        string TableName { get; set; }
        string ProcPrefix { get; set; }
        string ProjectName { get; set; }
        List<ColumnInfo> Fieldlist { get; set; }
        string Fields { get; }
        List<ColumnInfo> Keys { get; set; }
    }








    internal class ObjHelper
    {
        // Methods
        public static CodeBuilders CreatCB(string longservername) =>
            new CodeBuilders(CreatDbObj(longservername));

        public static IDbObject CreatDbObj(string longservername)
        {
            DbSettings setting = DbConfig.GetSetting(longservername);
            IDbObject obj2 = DBOMaker.CreateDbObj(setting.DbType);
            obj2.DbConnectStr = setting.ConnectStr;
            return obj2;
        }

        public static IDbScriptBuilder CreatDsb(string longservername)
        {
            DbSettings setting = DbConfig.GetSetting(longservername);
            IDbScriptBuilder builder = DBOMaker.CreateScript(setting.DbType);
            builder.DbConnectStr = setting.ConnectStr;
            return builder;
        }
    }

    public interface IBuilderWeb
    {
        // Methods
        string CreatSearchForm();
        string GetAddAspx();
        string GetAddAspxCs();
        string GetAddDesigner();
        string GetDeleteAspxCs();
        string GetListAspx();
        string GetListAspxCs();
        string GetListDesigner();
        string GetShowAspx();
        string GetShowAspxCs();
        string GetShowDesigner();
        string GetUpdateAspx();
        string GetUpdateAspxCs();
        string GetUpdateDesigner();
        string GetUpdateShowAspxCs();
        string GetWebCode(bool ExistsKey, bool AddForm, bool UpdateForm, bool ShowForm, bool SearchForm);
        string GetWebHtmlCode(bool ExistsKey, bool AddForm, bool UpdateForm, bool ShowForm, bool SearchForm);

        // Properties
        string NameSpace { get; set; }
        string Folder { get; set; }
        string ModelName { get; set; }
        string BLLName { get; set; }
        List<ColumnInfo> Fieldlist { get; set; }
        List<ColumnInfo> Keys { get; set; }
    }






    public class CodeBuilders
    {
        // Fields
        private IDbObject dbobj;
        private IBuilderWeb ibw;
        private string _dbtype;
        private string _dbconnectStr;
        private string _dbname;
        private string _tablename;
        private string _tabledescription = "";
        private string _modelname;
        private string _bllname;
        private string _dalname;
        private string _namespace = "Maticsoft";
        private string _folder;
        private string _dbhelperName = "DbHelperSQL";
        private List<ColumnInfo> _keys;
        private List<ColumnInfo> _fieldlist;
        private string _procprefix;
        private string _modelpath;
        private string _dalpath;
        private string _idalpath;
        private string _bllpath;
        private string _factoryclass;

        // Methods
        public CodeBuilders(IDbObject idbobj)
        {
            this.dbobj = idbobj;
            this.DbType = idbobj.DbType;
            if (this._dbhelperName == "")
            {
                switch (this.DbType)
                {
                    case "SQL2000":
                    case "SQL2005":
                    case "SQL2008":
                    case "SQL2012":
                        this._dbhelperName = "DbHelperSQL";
                        return;

                    case "Oracle":
                        this._dbhelperName = "DbHelperOra";
                        return;

                    case "MySQL":
                        this._dbhelperName = "DbHelperMySQL";
                        return;

                    case "OleDb":
                        this._dbhelperName = "DbHelperOleDb";
                        break;

                    default:
                        return;
                }
            }
        }

        public IBuilderWeb CreatBuilderWeb(string AssemblyGuid)
        {
            this.ibw = BuilderFactory.CreateWebObj(AssemblyGuid);
            this.ibw.NameSpace = this.NameSpace;
            this.ibw.Fieldlist = this.Fieldlist;
            this.ibw.Keys = this.Keys;
            this.ibw.ModelName = this.ModelName;
            this.ibw.Folder = this.Folder;
            this.ibw.BLLName = this.BLLName;
            return this.ibw;
        }

        public string GetAddAspx() =>
            this.ibw?.GetAddAspx();

        public string GetAddAspxCs()
        {
            if (this.ibw == null)
            {
                return "//请选择有效的表示层代码组件！";
            }
            string addAspxCs = this.ibw.GetAddAspxCs();
            StringPlus plus = new StringPlus();
            plus.AppendSpaceLine(2, "protected void btnSave_Click(object sender, EventArgs e)");
            plus.AppendSpaceLine(2, "{");
            plus.AppendSpaceLine(3, addAspxCs);
            plus.AppendSpaceLine(2, "}");
            return plus.ToString();
        }

        public string GetAddDesigner() =>
            this.ibw?.GetAddDesigner();

        //public string GetCodeFrameF3BLL(string AssemblyGuid, bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool GetModelByCache, bool List)
        //{
        //    BuilderFrameF3 ef = new BuilderFrameF3(this.dbobj, this.DbName, this.TableName, this.TableDescription, this.ModelName, this.BLLName, this.DALName, this.Fieldlist, this.Keys, this.NameSpace, this.Folder, this.DbHelperName);
        //    return ef.GetBLLCode(AssemblyGuid, Maxid, Exists, Add, Update, Delete, GetModel, GetModelByCache, List, List);
        //}

        //public string GetCodeFrameF3DAL(string AssemblyGuid, bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool List)
        //{
        //    BuilderFrameF3 ef = new BuilderFrameF3(this.dbobj, this.DbName, this.TableName, this.TableDescription, this.ModelName, this.BLLName, this.DALName, this.Fieldlist, this.Keys, this.NameSpace, this.Folder, this.DbHelperName);
        //    return ef.GetDALCode(AssemblyGuid, Maxid, Exists, Add, Update, Delete, GetModel, List, this.ProcPrefix);
        //}

        //public string GetCodeFrameF3DALFactory()
        //{
        //    BuilderFrameF3 ef = new BuilderFrameF3(this.dbobj, this.DbName, this.TableName, this.TableDescription, this.ModelName, this.BLLName, this.DALName, this.Fieldlist, this.Keys, this.NameSpace, this.Folder, this.DbHelperName);
        //    return ef.GetDALFactoryCode();
        //}

        //public string GetCodeFrameF3DALFactoryMethod()
        //{
        //    BuilderFrameF3 ef = new BuilderFrameF3(this.dbobj, this.DbName, this.TableName, this.TableDescription, this.ModelName, this.BLLName, this.DALName, this.Fieldlist, this.Keys, this.NameSpace, this.Folder, this.DbHelperName);
        //    return ef.GetDALFactoryMethodCode();
        //}

        //public string GetCodeFrameF3IDAL(bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool List, bool ListProc)
        //{
        //    BuilderFrameF3 ef = new BuilderFrameF3(this.dbobj, this.DbName, this.TableName, this.TableDescription, this.ModelName, this.BLLName, this.DALName, this.Fieldlist, this.Keys, this.NameSpace, this.Folder, this.DbHelperName);
        //    return ef.GetIDALCode(Maxid, Exists, Add, Update, Delete, GetModel, List, ListProc);
        //}

        //public string GetCodeFrameF3Model()
        //{
        //    BuilderFrameF3 ef = new BuilderFrameF3(this.dbobj, this.DbName, this.TableName, this.TableDescription, this.ModelName, this.BLLName, this.DALName, this.Fieldlist, this.Keys, this.NameSpace, this.Folder, this.DbHelperName);
        //    return ef.GetModelCode();
        //}

        //public string GetCodeFrameOne(string DALtype, bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool List)
        //{
        //    BuilderFrameOne one = new BuilderFrameOne(this.dbobj, this.DbName, this.TableName, this.ModelName, this.Fieldlist, this.Keys, this.NameSpace, this.Folder, this.DbHelperName);
        //    return one.GetCode(DALtype, Maxid, Exists, Add, Update, Delete, GetModel, List, this.ProcPrefix);
        //}

        //public string GetCodeFrameS3BLL(string AssemblyGuid, bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool GetModelByCache, bool List)
        //{
        //    BuilderFrameS3 es = new BuilderFrameS3(this.dbobj, this.DbName, this.TableName, this.TableDescription, this.ModelName, this.BLLName, this.DALName, this.Fieldlist, this.Keys, this.NameSpace, this.Folder, this.DbHelperName);
        //    return es.GetBLLCode(AssemblyGuid, Maxid, Exists, Add, Update, Delete, GetModel, GetModelByCache, List);
        //}

        //public string GetCodeFrameS3DAL(string AssemblyGuid, bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool List)
        //{
        //    BuilderFrameS3 es = new BuilderFrameS3(this.dbobj, this.DbName, this.TableName, this.TableDescription, this.ModelName, this.BLLName, this.DALName, this.Fieldlist, this.Keys, this.NameSpace, this.Folder, this.DbHelperName);
        //    return es.GetDALCode(AssemblyGuid, Maxid, Exists, Add, Update, Delete, GetModel, List, this.ProcPrefix);
        //}

        //public string GetCodeFrameS3Model()
        //{
        //    BuilderFrameS3 es = new BuilderFrameS3(this.dbobj, this.DbName, this.TableName, this.TableDescription, this.ModelName, this.BLLName, this.DALName, this.Fieldlist, this.Keys, this.NameSpace, this.Folder, this.DbHelperName);
        //    return es.GetModelCode();
        //}

        public string GetDeleteAspxCs()
        {
            if (this.ibw == null)
            {
                return "//请选择有效的表示层代码组件！";
            }
            StringPlus plus = new StringPlus();
            plus.AppendSpaceLine(3, "if (!Page.IsPostBack)");
            plus.AppendSpaceLine(3, "{");
            plus.AppendSpaceLine(4, this.BLLSpace + " bll=new " + this.BLLSpace + "();");
            string columnName = "ID";
            if (this._keys.Count != 1)
            {
                ColumnInfo identityKey = CodeCommon.GetIdentityKey(this._keys);
                if (identityKey == null)
                {
                    for (int i = 0; i < this._keys.Count; i++)
                    {
                        columnName = this._keys[i].ColumnName;
                        string str2 = CodeCommon.DbTypeToCS(this._keys[i].TypeName);
                        switch (str2)
                        {
                            case "int":
                            case "long":
                            case "decimal":
                                plus.AppendSpaceLine(4, str2 + " " + columnName + " = -1;");
                                break;

                            case "bool":
                                plus.AppendSpaceLine(4, str2 + " " + columnName + " = false;");
                                break;

                            case "guid":
                            case "Guid":
                                plus.AppendSpaceLine(4, str2 + " " + columnName + " ;");
                                break;

                            default:
                                plus.AppendSpaceLine(4, str2 + " " + columnName + " = \"\";");
                                break;
                        }
                        plus.AppendSpaceLine(4, "if (Request.Params[\"id" + i.ToString() + "\"] != null && Request.Params[\"id" + i.ToString() + "\"].Trim() != \"\")");
                        plus.AppendSpaceLine(4, "{");
                        switch (str2.ToLower())
                        {
                            case "int":
                                plus.AppendSpaceLine(5, columnName + "=(Convert.ToInt32(Request.Params[\"id" + i.ToString() + "\"]));");
                                break;

                            case "long":
                                plus.AppendSpaceLine(5, columnName + "=(Convert.ToInt64(Request.Params[\"id" + i.ToString() + "\"]));");
                                break;

                            case "decimal":
                                plus.AppendSpaceLine(5, columnName + "=(Convert.ToDecimal(Request.Params[\"id" + i.ToString() + "\"]));");
                                break;

                            case "bool":
                                plus.AppendSpaceLine(5, columnName + "=(Convert.ToBoolean(Request.Params[\"id" + i.ToString() + "\"]));");
                                break;

                            case "guid":
                                plus.AppendSpaceLine(5, columnName + "=new Guid(Request.Params[\"id\"]);");
                                break;

                            default:
                                plus.AppendSpaceLine(5, columnName + "= Request.Params[\"id" + i.ToString() + "\"];");
                                break;
                        }
                        plus.AppendSpaceLine(4, "}");
                    }
                    plus.AppendSpaceLine(4, "#warning 代码生成提示：删除页面,请检查确认传递过来的参数是否正确");
                    plus.AppendSpaceLine(4, "// bll.Delete(" + CodeCommon.GetFieldstrlist(this.Keys, true) + ");");
                    goto Label_062C;
                }
                columnName = identityKey.ColumnName;
                plus.AppendSpaceLine(4, "if (Request.Params[\"id\"] != null && Request.Params[\"id\"].Trim() != \"\")");
                plus.AppendSpaceLine(4, "{");
                switch (CodeCommon.DbTypeToCS(identityKey.TypeName).ToLower())
                {
                    case "int":
                        plus.AppendSpaceLine(5, "int " + columnName + "=(Convert.ToInt32(Request.Params[\"id\"]));");
                        goto Label_0309;

                    case "long":
                        plus.AppendSpaceLine(5, "long " + columnName + "=(Convert.ToInt64(Request.Params[\"id\"]));");
                        goto Label_0309;

                    case "decimal":
                        plus.AppendSpaceLine(5, "decimal " + columnName + "=(Convert.ToDecimal(Request.Params[\"id\"]));");
                        goto Label_0309;

                    case "bool":
                        plus.AppendSpaceLine(5, "bool " + columnName + "=(Convert.ToBoolean(Request.Params[\"id\"]));");
                        goto Label_0309;

                    case "guid":
                        plus.AppendSpaceLine(5, "Guid " + columnName + "=new Guid(Request.Params[\"id\"]);");
                        goto Label_0309;
                }
                plus.AppendSpaceLine(5, "string " + columnName + "= Request.Params[\"id\"];");
            }
            else
            {
                columnName = this._keys[0].ColumnName;
                plus.AppendSpaceLine(4, "if (Request.Params[\"id\"] != null && Request.Params[\"id\"].Trim() != \"\")");
                plus.AppendSpaceLine(4, "{");
                switch (CodeCommon.DbTypeToCS(this._keys[0].TypeName).ToLower())
                {
                    case "int":
                        plus.AppendSpaceLine(5, "int " + columnName + "=(Convert.ToInt32(Request.Params[\"id\"]));");
                        break;

                    case "long":
                        plus.AppendSpaceLine(5, "long " + columnName + "=(Convert.ToInt64(Request.Params[\"id\"]));");
                        break;

                    case "decimal":
                        plus.AppendSpaceLine(5, "decimal " + columnName + "=(Convert.ToDecimal(Request.Params[\"id\"]));");
                        break;

                    case "bool":
                        plus.AppendSpaceLine(5, "bool " + columnName + "=(Convert.ToBoolean(Request.Params[\"id\"]));");
                        break;

                    case "guid":
                        plus.AppendSpaceLine(5, "Guid " + columnName + "=new Guid(Request.Params[\"id\"]);");
                        break;

                    default:
                        plus.AppendSpaceLine(5, "string " + columnName + "= Request.Params[\"id\"];");
                        break;
                }
                plus.AppendSpaceLine(5, "bll.Delete(" + columnName + ");");
                plus.AppendSpaceLine(5, "Response.Redirect(\"list.aspx\");");
                plus.AppendSpaceLine(4, "}");
                goto Label_062C;
            }
        Label_0309:
            plus.AppendSpaceLine(4, "bll.Delete(" + columnName + ");");
            plus.AppendSpaceLine(4, "}");
        Label_062C:
            plus.AppendSpaceLine(3, "}");
            return plus.ToString();
        }

        public string GetListAspx() =>
            this.ibw?.GetListAspx();

        public string GetListAspxCs() =>
            this.ibw?.GetListAspxCs();

        public string GetListDesigner() =>
            this.ibw?.GetListDesigner();

        public string GetShowAspx() =>
            this.ibw?.GetShowAspx();

        public string GetShowAspxCs()
        {
            if (this.ibw == null)
            {
                return "//请选择有效的表示层代码组件！";
            }
            string showAspxCs = this.ibw.GetShowAspxCs();
            StringPlus plus = new StringPlus();
            plus.AppendSpaceLine(2, "public string strid=\"\"; ");
            plus.AppendSpaceLine(2, "protected void Page_Load(object sender, EventArgs e)");
            plus.AppendSpaceLine(2, "{");
            plus.AppendSpaceLine(3, "if (!Page.IsPostBack)");
            plus.AppendSpaceLine(3, "{");
            string columnName = "ID";
            if (this._keys.Count != 1)
            {
                ColumnInfo identityKey = CodeCommon.GetIdentityKey(this._keys);
                if (identityKey == null)
                {
                    for (int i = 0; i < this._keys.Count; i++)
                    {
                        columnName = this._keys[i].ColumnName;
                        string str3 = CodeCommon.DbTypeToCS(this._keys[i].TypeName);
                        switch (str3.ToLower())
                        {
                            case "int":
                            case "long":
                            case "decimal":
                                plus.AppendSpaceLine(4, str3 + " " + columnName + " = -1;");
                                break;

                            case "bool":
                                plus.AppendSpaceLine(4, str3 + " " + columnName + " = false;");
                                break;

                            case "guid":
                            case "Guid":
                                plus.AppendSpaceLine(4, str3 + " " + columnName + " ;");
                                break;

                            default:
                                plus.AppendSpaceLine(4, str3 + " " + columnName + " = \"\";");
                                break;
                        }
                        plus.AppendSpaceLine(4, "if (Request.Params[\"id" + i.ToString() + "\"] != null && Request.Params[\"id" + i.ToString() + "\"].Trim() != \"\")");
                        plus.AppendSpaceLine(4, "{");
                        switch (str3.ToLower())
                        {
                            case "int":
                                plus.AppendSpaceLine(5, columnName + "=(Convert.ToInt32(Request.Params[\"id" + i.ToString() + "\"]));");
                                break;

                            case "long":
                                plus.AppendSpaceLine(5, columnName + "=(Convert.ToInt64(Request.Params[\"id" + i.ToString() + "\"]));");
                                break;

                            case "decimal":
                                plus.AppendSpaceLine(5, columnName + "=(Convert.ToDecimal(Request.Params[\"id" + i.ToString() + "\"]));");
                                break;

                            case "bool":
                                plus.AppendSpaceLine(5, columnName + "=(Convert.ToBoolean(Request.Params[\"id" + i.ToString() + "\"]));");
                                break;

                            case "guid":
                                plus.AppendSpaceLine(5, columnName + "=new Guid(Request.Params[\"id\"]);");
                                break;

                            default:
                                plus.AppendSpaceLine(5, columnName + "= Request.Params[\"id" + i.ToString() + "\"];");
                                break;
                        }
                        plus.AppendSpaceLine(4, "}");
                    }
                    plus.AppendSpaceLine(4, "#warning 代码生成提示：显示页面,请检查确认该语句是否正确");
                    plus.AppendSpaceLine(4, "ShowInfo(" + CodeCommon.GetFieldstrlist(this.Keys, true) + ");");
                    goto Label_0654;
                }
                columnName = identityKey.ColumnName;
                plus.AppendSpaceLine(4, "if (Request.Params[\"id\"] != null && Request.Params[\"id\"].Trim() != \"\")");
                plus.AppendSpaceLine(4, "{");
                plus.AppendSpaceLine(5, "strid = Request.Params[\"id\"];");
                switch (CodeCommon.DbTypeToCS(identityKey.TypeName).ToLower())
                {
                    case "int":
                        plus.AppendSpaceLine(5, "int " + columnName + "=(Convert.ToInt32(strid));");
                        goto Label_0326;

                    case "long":
                        plus.AppendSpaceLine(5, "long " + columnName + "=(Convert.ToInt64(strid));");
                        goto Label_0326;

                    case "decimal":
                        plus.AppendSpaceLine(5, "decimal " + columnName + "=(Convert.ToDecimal(strid));");
                        goto Label_0326;

                    case "bool":
                        plus.AppendSpaceLine(5, "bool " + columnName + "=(Convert.ToBoolean(strid));");
                        goto Label_0326;

                    case "guid":
                        plus.AppendSpaceLine(5, "Guid " + columnName + "=new Guid(strid);");
                        goto Label_0326;
                }
                plus.AppendSpaceLine(5, "string " + columnName + "= strid;");
            }
            else
            {
                columnName = this._keys[0].ColumnName;
                plus.AppendSpaceLine(4, "if (Request.Params[\"id\"] != null && Request.Params[\"id\"].Trim() != \"\")");
                plus.AppendSpaceLine(4, "{");
                plus.AppendSpaceLine(5, "strid = Request.Params[\"id\"];");
                switch (CodeCommon.DbTypeToCS(this._keys[0].TypeName).ToLower())
                {
                    case "int":
                        plus.AppendSpaceLine(5, "int " + columnName + "=(Convert.ToInt32(strid));");
                        break;

                    case "long":
                        plus.AppendSpaceLine(5, "long " + columnName + "=(Convert.ToInt64(strid));");
                        break;

                    case "decimal":
                        plus.AppendSpaceLine(5, "decimal " + columnName + "=(Convert.ToDecimal(strid));");
                        break;

                    case "bool":
                        plus.AppendSpaceLine(5, "bool " + columnName + "=(Convert.ToBoolean(strid));");
                        break;

                    case "guid":
                        plus.AppendSpaceLine(5, "Guid " + columnName + "=new Guid(strid);");
                        break;

                    default:
                        plus.AppendSpaceLine(5, "string " + columnName + "= strid;");
                        break;
                }
                plus.AppendSpaceLine(5, "ShowInfo(" + columnName + ");");
                plus.AppendSpaceLine(4, "}");
                goto Label_0654;
            }
        Label_0326:
            plus.AppendSpaceLine(5, "ShowInfo(" + columnName + ");");
            plus.AppendSpaceLine(4, "}");
        Label_0654:
            plus.AppendSpaceLine(3, "}");
            plus.AppendSpaceLine(2, "}");
            plus.AppendSpaceLine(2, showAspxCs);
            return plus.ToString();
        }

        public string GetShowDesigner() =>
            this.ibw?.GetShowDesigner();

        public string GetUpdateAspx() =>
            this.ibw?.GetUpdateAspx();

        public string GetUpdateAspxCs()
        {
            if (this.ibw == null)
            {
                return "//请选择有效的表示层代码组件！";
            }
            string updateAspxCs = this.ibw.GetUpdateAspxCs();
            string updateShowAspxCs = this.ibw.GetUpdateShowAspxCs();
            StringPlus plus = new StringPlus();
            plus.AppendSpaceLine(2, "protected void Page_Load(object sender, EventArgs e)");
            plus.AppendSpaceLine(2, "{");
            plus.AppendSpaceLine(3, "if (!Page.IsPostBack)");
            plus.AppendSpaceLine(3, "{");
            string columnName = "ID";
            if (this._keys.Count != 1)
            {
                ColumnInfo identityKey = CodeCommon.GetIdentityKey(this._keys);
                if (identityKey == null)
                {
                    for (int i = 0; i < this._keys.Count; i++)
                    {
                        columnName = this._keys[i].ColumnName;
                        string str4 = CodeCommon.DbTypeToCS(this._keys[i].TypeName);
                        switch (str4.ToLower())
                        {
                            case "int":
                            case "long":
                            case "decimal":
                                plus.AppendSpaceLine(4, str4 + " " + columnName + " = -1;");
                                break;

                            case "bool":
                                plus.AppendSpaceLine(4, str4 + " " + columnName + " = false;");
                                break;

                            case "guid":
                                plus.AppendSpaceLine(4, str4 + " " + columnName + ";");
                                break;

                            default:
                                plus.AppendSpaceLine(4, str4 + " " + columnName + " = \"\";");
                                break;
                        }
                        plus.AppendSpaceLine(4, "if (Request.Params[\"id" + i.ToString() + "\"] != null && Request.Params[\"id" + i.ToString() + "\"].Trim() != \"\")");
                        plus.AppendSpaceLine(4, "{");
                        switch (str4)
                        {
                            case "int":
                                plus.AppendSpaceLine(5, columnName + "=(Convert.ToInt32(Request.Params[\"id" + i.ToString() + "\"]));");
                                break;

                            case "long":
                                plus.AppendSpaceLine(5, columnName + "=(Convert.ToInt64(Request.Params[\"id" + i.ToString() + "\"]));");
                                break;

                            case "decimal":
                                plus.AppendSpaceLine(5, columnName + "=(Convert.ToDecimal(Request.Params[\"id" + i.ToString() + "\"]));");
                                break;

                            case "bool":
                                plus.AppendSpaceLine(5, columnName + "=(Convert.ToBoolean(Request.Params[\"id" + i.ToString() + "\"]));");
                                break;

                            case "guid":
                                plus.AppendSpaceLine(5, columnName + "=new Guid(Request.Params[\"id" + i.ToString() + "\"]);");
                                break;

                            default:
                                plus.AppendSpaceLine(5, columnName + "= Request.Params[\"id" + i.ToString() + "\"];");
                                break;
                        }
                        plus.AppendSpaceLine(4, "}");
                    }
                    plus.AppendSpaceLine(4, "#warning 代码生成提示：显示页面,请检查确认该语句是否正确");
                    plus.AppendSpaceLine(4, "ShowInfo(" + CodeCommon.GetFieldstrlist(this.Keys, true) + ");");
                    goto Label_05E6;
                }
                columnName = identityKey.ColumnName;
                plus.AppendSpaceLine(4, "if (Request.Params[\"id\"] != null && Request.Params[\"id\"].Trim() != \"\")");
                plus.AppendSpaceLine(4, "{");
                switch (CodeCommon.DbTypeToCS(identityKey.TypeName).ToLower())
                {
                    case "int":
                        plus.AppendSpaceLine(5, "int " + columnName + "=(Convert.ToInt32(Request.Params[\"id\"]));");
                        goto Label_02EC;

                    case "long":
                        plus.AppendSpaceLine(5, "long " + columnName + "=(Convert.ToInt64(Request.Params[\"id\"]));");
                        goto Label_02EC;

                    case "decimal":
                        plus.AppendSpaceLine(5, "decimal " + columnName + "=(Convert.ToDecimal(Request.Params[\"id\"]));");
                        goto Label_02EC;

                    case "guid":
                        plus.AppendSpaceLine(5, "Guid " + columnName + "=new Guid(Request.Params[\"id\"]);");
                        goto Label_02EC;
                }
                plus.AppendSpaceLine(5, "string " + columnName + "= Request.Params[\"id\"];");
            }
            else
            {
                columnName = this._keys[0].ColumnName;
                plus.AppendSpaceLine(4, "if (Request.Params[\"id\"] != null && Request.Params[\"id\"].Trim() != \"\")");
                plus.AppendSpaceLine(4, "{");
                switch (CodeCommon.DbTypeToCS(this._keys[0].TypeName).ToLower())
                {
                    case "int":
                        plus.AppendSpaceLine(5, "int " + columnName + "=(Convert.ToInt32(Request.Params[\"id\"]));");
                        break;

                    case "long":
                        plus.AppendSpaceLine(5, "long " + columnName + "=(Convert.ToInt64(Request.Params[\"id\"]));");
                        break;

                    case "decimal":
                        plus.AppendSpaceLine(5, "decimal " + columnName + "=(Convert.ToDecimal(Request.Params[\"id\"]));");
                        break;

                    case "bool":
                        plus.AppendSpaceLine(5, "bool " + columnName + "=(Convert.ToBoolean(Request.Params[\"id\"]));");
                        break;

                    case "guid":
                        plus.AppendSpaceLine(5, "Guid " + columnName + "=new Guid(Request.Params[\"id\"]);");
                        break;

                    default:
                        plus.AppendSpaceLine(5, "string " + columnName + "= Request.Params[\"id\"];");
                        break;
                }
                plus.AppendSpaceLine(5, "ShowInfo(" + CodeCommon.GetFieldstrlist(this.Keys, true) + ");");
                plus.AppendSpaceLine(4, "}");
                goto Label_05E6;
            }
        Label_02EC:
            plus.AppendSpaceLine(5, "ShowInfo(" + CodeCommon.GetFieldstrlist(this.Keys, true) + ");");
            plus.AppendSpaceLine(4, "}");
        Label_05E6:
            plus.AppendSpaceLine(3, "}");
            plus.AppendSpaceLine(2, "}");
            plus.AppendSpaceLine(3, updateShowAspxCs);
            plus.AppendSpaceLine(2, "public void btnSave_Click(object sender, EventArgs e)");
            plus.AppendSpaceLine(2, "{");
            plus.AppendSpaceLine(3, updateAspxCs);
            plus.AppendSpaceLine(2, "}");
            return plus.ToString();
        }

        public string GetUpdateDesigner() =>
            this.ibw?.GetUpdateDesigner();

        public string GetUpdateShowAspxCs() =>
            this.ibw.GetUpdateShowAspxCs();

        public string GetWebCode(bool ExistsKey, bool AddForm, bool UpdateForm, bool ShowForm, bool SearchForm)
        {
            StringPlus plus = new StringPlus();
            if (AddForm)
            {
                plus.AppendLine("  /******************************增加窗体代码********************************/");
                plus.AppendLine(this.GetAddAspxCs());
            }
            if (UpdateForm)
            {
                plus.AppendLine("  /******************************修改窗体代码********************************/");
                plus.AppendLine("  /*修改代码-提交更新 */");
                plus.AppendLine(this.GetUpdateAspxCs());
            }
            if (ShowForm)
            {
                plus.AppendLine("  /******************************显示窗体代码********************************/");
                plus.AppendLine(this.GetShowAspxCs());
            }
            if (SearchForm)
            {
                plus.AppendLine("  /******************************列表窗体代码********************************/");
                plus.AppendLine(this.GetListAspxCs());
            }
            return plus.Value;
        }

        public string GetWebHtmlCode(bool ExistsKey, bool AddForm, bool UpdateForm, bool ShowForm, bool SearchForm)
        {
            StringPlus plus = new StringPlus();
            if (AddForm)
            {
                plus.AppendLine(" <!--******************************增加页面代码********************************-->");
                plus.AppendLine(this.GetAddAspx());
            }
            if (UpdateForm)
            {
                plus.AppendLine(" <!--******************************修改页面代码********************************-->");
                plus.AppendLine(this.GetUpdateAspx());
            }
            if (ShowForm)
            {
                plus.AppendLine("  <!--******************************显示页面代码********************************-->");
                plus.AppendLine(this.GetShowAspx());
            }
            if (SearchForm)
            {
                plus.AppendLine("  <!--******************************列表页面代码********************************-->");
                plus.AppendLine(this.GetListAspx());
            }
            return plus.ToString();
        }

        // Properties
        public string DbType
        {
            get =>
                this._dbtype;
            set =>
                this._dbtype = value;
        }

        public string DbConnectStr
        {
            get =>
                this._dbconnectStr;
            set =>
                this._dbconnectStr = value;
        }

        public string DbName
        {
            get =>
                this._dbname;
            set =>
                this._dbname = value;
        }

        public string TableName
        {
            get =>
                this._tablename;
            set =>
                this._tablename = value;
        }

        public string TableDescription
        {
            get =>
                this._tabledescription;
            set =>
                this._tabledescription = value;
        }

        public string NameSpace
        {
            get =>
                this._namespace;
            set =>
                this._namespace = value;
        }

        public string Folder
        {
            get =>
                this._folder;
            set =>
                this._folder = value;
        }

        public string ModelName
        {
            get =>
                this._modelname;
            set =>
                this._modelname = value;
        }

        public string BLLName
        {
            get =>
                this._bllname;
            set =>
                this._bllname = value;
        }

        public string DALName
        {
            get =>
                this._dalname;
            set =>
                this._dalname = value;
        }

        public string DbHelperName
        {
            get =>
                this._dbhelperName;
            set =>
                this._dbhelperName = value;
        }

        public List<ColumnInfo> Fieldlist
        {
            get =>
                this._fieldlist;
            set =>
                this._fieldlist = value;
        }

        public List<ColumnInfo> Keys
        {
            get =>
                this._keys;
            set =>
                this._keys = value;
        }

        public string ProcPrefix
        {
            get =>
                this._procprefix;
            set =>
                this._procprefix = value;
        }

        public string Modelpath
        {
            get
            {
                this._modelpath = this._namespace + ".Model";
                if (this._folder.Trim() != "")
                {
                    this._modelpath = this._modelpath + "." + this._folder;
                }
                return this._modelpath;
            }
            set =>
                this._modelpath = value;
        }

        public string ModelSpace =>
            (this.Modelpath + "." + this.ModelName);

        public string DALpath
        {
            get
            {
                string str = this._namespace + "." + this._dbtype + "DAL";
                if (this._folder.Trim() != "")
                {
                    str = str + "." + this._folder;
                }
                return str;
            }
            set =>
                this._dalpath = value;
        }

        public string IDALpath
        {
            get
            {
                this._idalpath = this._namespace + ".IDAL";
                if (this._folder.Trim() != "")
                {
                    this._idalpath = this._idalpath + "." + this._folder;
                }
                return this._idalpath;
            }
        }

        public string IClass =>
            ("I" + this.DALName);

        public string BLLpath
        {
            get
            {
                string str = this._namespace + ".BLL";
                if (this._folder.Trim() != "")
                {
                    str = str + "." + this._folder;
                }
                return str;
            }
            set =>
                this._bllpath = value;
        }

        public string BLLSpace =>
            (this.BLLpath + "." + this.BLLName);

        public string Factorypath
        {
            get
            {
                string str = this._namespace + ".DALFactory";
                if (this._folder.Trim() != "")
                {
                    str = str + "." + this._folder;
                }
                return str;
            }
        }

        public string FactoryClass
        {
            get
            {
                this._factoryclass = this._namespace + ".DALFactory";
                if (this._folder.Trim() != "")
                {
                    this._factoryclass = this._factoryclass + "." + this._folder;
                }
                this._factoryclass = this._factoryclass + "." + this._modelname;
                return this._factoryclass;
            }
        }

        public bool IsHasIdentity
        {
            get
            {
                bool flag = false;
                if (this.Keys.Count > 0)
                {
                    foreach (ColumnInfo info in this.Keys)
                    {
                        if (info.IsIdentity)
                        {
                            flag = true;
                        }
                    }
                }
                return flag;
            }
        }
    }







    public class CodeGenerator
    {
        // Methods
        public CodeInfo BatchGenerateCode(string[] templatefile)
        {
            string path = null;
            if (templatefile.Length == 0)
            {
                throw new Exception("you must provide a text template file path");
            }
            path = templatefile[0];
            if (path == null)
            {
                throw new ArgumentNullException("the file name cannot be null");
            }
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("the file cannot be found");
            }
            TemplateHost host = new TemplateHost
            {
                TemplateFile = path
            };
            string input = File.ReadAllText(path);
            return this.GenerateCode(input, host);
        }









        public CodeInfo GenerateCode(string input, TemplateHost host)
        {
            CodeInfo info = new CodeInfo();
            string lang;
            string[] refences;

            info.Code =
                new Mono.TextTemplating.TemplatingEngine().ProcessTemplate(input, host);
            /*new Engine().ProcessTemplate(input, host);*/
            StringBuilder builder = new StringBuilder();
            foreach (CompilerError error in host.ErrorCollection)
            {
                builder.AppendLine(error.ToString());
            }
            info.ErrorMsg = builder.ToString();
            info.FileExtension = host.FileExtension;
            return info;
        }
    }





    public class BuilderTemp
    {
        // Fields
        protected IDbObject dbobj;
        protected DbSettings dbset;
        private string _dbname;
        private string _tablename;
        private string _tabledescription = "";
        private string TemplateFile = "";
        private List<ColumnInfo> _keys;
        private List<ColumnInfo> _fkeys;
        private List<ColumnInfo> _fieldlist;
        private string _objtype;

        // Methods
        public BuilderTemp(IDbObject idbobj, string dbName, string tableName, string tableDescription, List<ColumnInfo> fieldlist, List<ColumnInfo> keys, List<ColumnInfo> fkeys, string templateFile, DbSettings dbSet, string objectype)
        {
            this.dbobj = idbobj;
            this.DbName = dbName;
            this.TableName = tableName;
            this.TableDescription = tableDescription;
            this.TemplateFile = templateFile;
            this.Fieldlist = fieldlist;
            this.Keys = keys;
            this.FKeys = fkeys;
            this.dbset = dbSet;
            this._objtype = objectype;
        }

        public CodeInfo GetCode()
        {
            CodeInfo info = new CodeInfo();
            if ((this.TemplateFile == null) || !File.Exists(this.TemplateFile))
            {
                info.ErrorMsg = "模板文件不存在！";
                return info;
            }
            string input = File.ReadAllText(this.TemplateFile);
            CodeGenerator generator = new CodeGenerator();
            if (this.ObjectType == "proc")
            {
                ProcedureHost host = new ProcedureHost
                {
                    TableList = this.dbobj.GetTablesInfo(this.DbName),
                    ViewList = this.dbobj.GetVIEWsInfo(this.DbName),
                    ProcedureList = this.dbobj.GetProcInfo(this.DbName),
                    TemplateFile = this.TemplateFile,
                    DbName = this.DbName,
                    TableName = this.TableName,
                    TableDescription = this.TableDescription,
                    NameSpace = this.dbset.Namepace,
                    Folder = this.dbset.Folder,
                    DbHelperName = this.dbset.DbHelperName,
                    Fieldlist = this.Fieldlist,
                    Keys = this.Keys,
                    FKeys = this.FKeys,
                    DbSet = this.dbset,
                    BLLPrefix = this.dbset.BLLPrefix,
                    BLLSuffix = this.dbset.BLLSuffix,
                    DALPrefix = this.dbset.DALPrefix,
                    DALSuffix = this.dbset.DALSuffix,
                    DbType = this.dbset.DbType,
                    ModelPrefix = this.dbset.ModelPrefix,
                    ModelSuffix = this.dbset.ModelSuffix,
                    ProcPrefix = this.dbset.ProcPrefix,
                    ProjectName = this.dbset.ProjectName,
                    TabNameRule = this.dbset.TabNameRule
                };
                return generator.GenerateCode(input, host);
            }
            TableHost host2 = new TableHost
            {
                TableList = this.dbobj.GetTablesInfo(this.DbName),
                ViewList = this.dbobj.GetVIEWsInfo(this.DbName),
                ProcedureList = this.dbobj.GetProcInfo(this.DbName),
                TemplateFile = this.TemplateFile,
                DbName = this.DbName,
                TableName = this.TableName,
                TableDescription = this.TableDescription,
                NameSpace = this.dbset.Namepace,
                Folder = this.dbset.Folder,
                DbHelperName = this.dbset.DbHelperName,
                Fieldlist = this.Fieldlist,
                Keys = this.Keys,
                FKeys = this.FKeys,
                DbSet = this.dbset,
                BLLPrefix = this.dbset.BLLPrefix,
                BLLSuffix = this.dbset.BLLSuffix,
                DALPrefix = this.dbset.DALPrefix,
                DALSuffix = this.dbset.DALSuffix,
                DbType = this.dbset.DbType,
                ModelPrefix = this.dbset.ModelPrefix,
                ModelSuffix = this.dbset.ModelSuffix,
                ProcPrefix = this.dbset.ProcPrefix,
                ProjectName = this.dbset.ProjectName,
                TabNameRule = this.dbset.TabNameRule
            };
            return generator.GenerateCode(input, host2);
        }

        private XmlDocument GetXml(DataRow[] dtrows)
        {
            Stream w = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(w, Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };
            writer.WriteStartDocument(true);
            writer.WriteStartElement("Schema");
            writer.WriteStartElement("TableName");
            writer.WriteAttributeString("value", "Authors");
            writer.WriteEndElement();
            writer.WriteStartElement("FIELDS");
            foreach (DataRow row in dtrows)
            {
                string str = row["ColumnName"].ToString();
                string str2 = row["TypeName"].ToString();
                writer.WriteStartElement("FIELD");
                writer.WriteAttributeString("Name", str);
                writer.WriteAttributeString("Type", str2);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Flush();
            writer.Close();
            TextReader txtReader = new StringReader(writer.ToString());
            XmlDocument document = new XmlDocument();
            document.Load(txtReader);
            return document;
        }

        private XmlDocument GetXml2()
        {
            string filename = @"Template\temp.xml";
            XmlTextWriter writer = new XmlTextWriter(filename, Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };
            writer.WriteStartDocument(true);
            writer.WriteStartElement("Schema");
            writer.WriteStartElement("TableName");
            writer.WriteAttributeString("value", this.TableName);
            writer.WriteEndElement();
            writer.WriteStartElement("FIELDS");
            foreach (ColumnInfo info in this.Fieldlist)
            {
                string columnName = info.ColumnName;
                string typeName = info.TypeName;
                string length = info.Length;
                bool isIdentity = info.IsIdentity;
                bool isPrimaryKey = info.IsPrimaryKey;
                string description = info.Description;
                string defaultVal = info.DefaultVal;
                writer.WriteStartElement("FIELD");
                writer.WriteAttributeString("Name", columnName);
                writer.WriteAttributeString("Type", CodeCommon.DbTypeToCS(typeName));
                writer.WriteAttributeString("Desc", description);
                writer.WriteAttributeString("defaultVal", defaultVal);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("PrimaryKeys");
            foreach (ColumnInfo info2 in this.Keys)
            {
                string columnName = info2.ColumnName;
                string typeName = info2.TypeName;
                string length = info2.Length;
                bool isIdentity = info2.IsIdentity;
                bool isPrimaryKey = info2.IsPrimaryKey;
                string description = info2.Description;
                string defaultVal = info2.DefaultVal;
                writer.WriteStartElement("FIELD");
                writer.WriteAttributeString("Name", columnName);
                writer.WriteAttributeString("Type", CodeCommon.DbTypeToCS(typeName));
                writer.WriteAttributeString("Desc", description);
                writer.WriteAttributeString("defaultVal", defaultVal);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Flush();
            writer.Close();
            XmlDocument document = new XmlDocument();
            document.Load(filename);
            return document;
        }

        // Properties
        public string DbName
        {
            get =>
                this._dbname;
            set =>
                this._dbname = value;
        }

        public string TableName
        {
            get =>
                this._tablename;
            set =>
                this._tablename = value;
        }

        public string TableDescription
        {
            get =>
                this._tabledescription;
            set =>
                this._tabledescription = value;
        }

        public List<ColumnInfo> Fieldlist
        {
            get =>
                this._fieldlist;
            set =>
                this._fieldlist = value;
        }

        public List<ColumnInfo> Keys
        {
            get =>
                this._keys;
            set =>
                this._keys = value;
        }

        public List<ColumnInfo> FKeys
        {
            get =>
                this._fkeys;
            set =>
                this._fkeys = value;
        }

        public string Fields
        {
            get
            {
                StringPlus plus = new StringPlus();
                foreach (object obj2 in this.Fieldlist)
                {
                    plus.Append("'" + obj2.ToString() + "',");
                }
                plus.DelLastComma();
                return plus.Value;
            }
        }

        public string ObjectType
        {
            get =>
                this._objtype;
            set =>
                this._objtype = value;
        }
    }



    [Serializable]
    public class TemplateHost : ITextTemplatingEngineHost
    {
        // Fields
        internal string _templateFileValue;
        private string _namespace = "Maticsoft";
        private string _fileExtensionValue = ".cs";
        private Encoding _fileEncodingValue = Encoding.UTF8;
        private CompilerErrorCollection _ErrorCollection;

        /// <summary>
        /// 由引擎调用以要求指定选项的值。如果您未找到该值，则返回 null。
        /// </summary>
        /// <param name="optionName"></param>
        /// <returns></returns>
        public object GetHostOption(string optionName)
        {
            string str;
            return (((str = optionName) != null) && (str == "CacheAssemblies"));
        }
        /// <summary>
        /// 获取文本，它对应于包含部分文本模板文件的请求。
        /// </summary>
        /// <param name="requestFileName"></param>
        /// <param name="content"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool LoadIncludeText(string requestFileName, out string content, out string location)
        {
            content = string.Empty;
            location = string.Empty;
            if (File.Exists(requestFileName))
            {
                content = File.ReadAllText(requestFileName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 接收来自转换引擎的错误和警告集合。
        /// </summary>
        /// <param name="errors"></param>
        public void LogErrors(CompilerErrorCollection errors)
        {
            this._ErrorCollection = errors;
        }
        /// <summary>
        /// 提供运行所生成转换类的应用程序域。
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public AppDomain ProvideTemplatingAppDomain(string content) =>
            AppDomain.CreateDomain("Generation App Domain");

        /// <summary>
        /// 允许主机提供有关程序集位置的附加信息
        /// </summary>
        /// <param name="assemblyReference"></param>
        /// <returns></returns>
        public string ResolveAssemblyReference(string assemblyReference)
        {
            if (File.Exists(assemblyReference))
            {
                return assemblyReference;
            }
            string path = Path.Combine(Path.GetDirectoryName(this.TemplateFile), assemblyReference);
            if (File.Exists(path))
            {
                return path;
            }
            return "";
        }
        /// <summary>
        /// 在已知指令处理器的友好名称时，返回其类型。
        /// </summary>
        /// <param name="processorName"></param>
        /// <returns></returns>
        public Type ResolveDirectiveProcessor(string processorName)
        {
            string.Compare(processorName, "XYZ", StringComparison.OrdinalIgnoreCase);
            throw new Exception("没有找到指令处理器");
        }
        /// <summary>
        /// 如果在模板文本中未指定某个参数，则为指令处理器解析该参数的值。
        /// </summary>
        /// <param name="directiveId"></param>
        /// <param name="processorName"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public string ResolveParameterValue(string directiveId, string processorName, string parameterName)
        {
            if (directiveId == null)
            {
                throw new ArgumentNullException("the directiveId cannot be null");
            }
            if (processorName == null)
            {
                throw new ArgumentNullException("the processorName cannot be null");
            }
            if (parameterName == null)
            {
                throw new ArgumentNullException("the parameterName cannot be null");
            }
            return string.Empty;
        }
        /// <summary>
        /// 允许主机提供完整路径以及给定文件名或相对路径。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string ResolvePath(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("the file name cannot be null");
            }
            if (!File.Exists(fileName))
            {
                string path = Path.Combine(Path.GetDirectoryName(this.TemplateFile), fileName);
                if (File.Exists(path))
                {
                    return path;
                }
            }
            return fileName;
        }
        /// <summary>
        /// 通知主机所生成文本输出需要的文件扩展名。
        /// </summary>
        /// <param name="extension"></param>
        public void SetFileExtension(string extension)
        {
            this._fileExtensionValue = extension;
        }
        /// <summary>
        /// 通知主机所生成文本输出需要的编码。
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="fromOutputDirective"></param>
        public void SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
        {
            this._fileEncodingValue = encoding;
        }

        /// <summary>
        /// 获取所处理文本模板的路径和文件名
        /// </summary>
        public string TemplateFile
        {
            get =>
                this._templateFileValue;
            set =>
                this._templateFileValue = value;
        }

        /// <summary>
        /// 获取程序集引用的列表
        /// </summary>
        public IList<string> StandardAssemblyReferences =>
            new string[] { typeof(Uri).Assembly.Location, typeof(ColumnInfo).Assembly.Location, typeof(CodeCommon).Assembly.Location, typeof(DbSettings).Assembly.Location, typeof(DatabaseHost).Assembly.Location, typeof(TableHost).Assembly.Location, typeof(ProcedureHost).Assembly.Location, typeof(TemplateHost).Assembly.Location, typeof(Mono.TextTemplating.TemplatingEngine).Assembly.Location };

        /// <summary>
        /// 获取命名空间的列表
        /// </summary>
        public IList<string> StandardImports =>
            new string[] { "System", "System.Text", "System.Collections.Generic",
            };

        public string NameSpace
        {
            get =>
                this._namespace;
            set =>
                this._namespace = value;
        }

        public string FileExtension =>
            this._fileExtensionValue;

        public Encoding FileEncoding =>
            this._fileEncodingValue;

        public CompilerErrorCollection ErrorCollection =>
            this._ErrorCollection;
    }

    [Serializable]
    public class DbSettings
    {
        // Fields
        private string _dbtype;
        private string _server;
        private string _connectstr;
        private string _dbname;
        private bool _connectSimple = true;
        private int _tabloadtype;
        private string _tabloadkeyword;
        private string _procprefix = "";
        private string _projectname = "";
        private string _namespace = "Maticsoft";
        private string _folder = "";
        private string _appframe = "s3";
        private string _daltype = "";
        private string _blltype = "";
        private string _webtype = "";
        private string _editfont = "新宋体";
        private float _editfontsize = 9f;
        private string _dbHelperName = "DbHelperSQL";
        private string modelPrefix = "";
        private string modelSuffix = "";
        private string bllPrefix = "";
        private string bllSuffix = "";
        private string dalPrefix = "";
        private string dalSuffix = "";
        private string tabnameRule = "same";
        private string _webTemplatepath = "";
        private string _replaceoldstr = "";
        private string _replacenewstr = "";

        // Properties
        [XmlElement]
        public string DbType
        {
            get =>
                this._dbtype;
            set =>
                this._dbtype = value;
        }

        [XmlElement]
        public string Server
        {
            get =>
                this._server;
            set =>
                this._server = value;
        }

        [XmlElement]
        public string ConnectStr
        {
            get =>
                this._connectstr;
            set =>
                this._connectstr = value;
        }

        [XmlElement]
        public string DbName
        {
            get =>
                this._dbname;
            set =>
                this._dbname = value;
        }

        [XmlElement]
        public bool ConnectSimple
        {
            get =>
                this._connectSimple;
            set =>
                this._connectSimple = value;
        }

        [XmlElement]
        public int TabLoadtype
        {
            get =>
                this._tabloadtype;
            set =>
                this._tabloadtype = value;
        }

        [XmlElement]
        public string TabLoadKeyword
        {
            get =>
                this._tabloadkeyword;
            set =>
                this._tabloadkeyword = value;
        }

        [XmlElement]
        public string ProcPrefix
        {
            get =>
                this._procprefix;
            set =>
                this._procprefix = value;
        }

        [XmlElement]
        public string ProjectName
        {
            get =>
                this._projectname;
            set =>
                this._projectname = value;
        }

        [XmlElement]
        public string Namepace
        {
            get =>
                this._namespace;
            set =>
                this._namespace = value;
        }

        [XmlElement]
        public string Folder
        {
            get =>
                this._folder;
            set =>
                this._folder = value;
        }

        [XmlElement]
        public string AppFrame
        {
            get =>
                this._appframe;
            set =>
                this._appframe = value;
        }

        [XmlElement]
        public string DALType
        {
            get =>
                this._daltype;
            set =>
               this._daltype = value;
        }

        [XmlElement]
        public string BLLType
        {
            get =>
                this._blltype;
            set =>
                this._blltype = value;
        }

        [XmlElement]
        public string WebType
        {
            get =>
                this._webtype;
            set =>
                this._webtype = value;
        }

        [XmlElement]
        public string EditFont
        {
            get =>
                this._editfont;
            set =>
                this._editfont = value;
        }

        [XmlElement]
        public float EditFontSize
        {
            get =>
                this._editfontsize;
            set =>
                this._editfontsize = value;
        }

        [XmlElement]
        public string DbHelperName
        {
            get =>
                this._dbHelperName;
            set =>
                this._dbHelperName = value;
        }

        [XmlElement]
        public string ModelPrefix
        {
            get =>
                this.modelPrefix;
            set =>
                this.modelPrefix = value;
        }

        [XmlElement]
        public string ModelSuffix
        {
            get =>
                this.modelSuffix;
            set =>
                this.modelSuffix = value;
        }

        [XmlElement]
        public string BLLPrefix
        {
            get =>
                this.bllPrefix;
            set =>
                this.bllPrefix = value;
        }

        [XmlElement]
        public string BLLSuffix
        {
            get =>
                this.bllSuffix;
            set =>
                this.bllSuffix = value;
        }

        [XmlElement]
        public string DALPrefix
        {
            get =>
                this.dalPrefix;
            set =>
                this.dalPrefix = value;
        }

        [XmlElement]
        public string DALSuffix
        {
            get =>
                this.dalSuffix;
            set =>
                this.dalSuffix = value;
        }

        [XmlElement]
        public string TabNameRule
        {
            get =>
                this.tabnameRule;
            set =>
                this.tabnameRule = value;
        }

        [XmlElement]
        public string WebTemplatePath
        {
            get =>
                this._webTemplatepath;
            set =>
                this._webTemplatepath = value;
        }

        [XmlElement]
        public string ReplacedOldStr
        {
            get =>
                this._replaceoldstr;
            set =>
                this._replaceoldstr = value;
        }

        [XmlElement]
        public string ReplacedNewStr
        {
            get =>
                this._replacenewstr;
            set =>
                this._replacenewstr = value;
        }
    }


    [Serializable]
    public class ProcedureHost : TableHost
    {
        // Methods
        public string GetMethodName(string ProcedureName) =>
            ProcedureName;

        // Properties
        public string ProcedureName =>
            base.TableName;

        public List<ColumnInfo> Parameterlist =>
            base.Fieldlist;

        public ColumnInfo OutParameter
        {
            get
            {
                ColumnInfo info = null;
                foreach (ColumnInfo info2 in this.Parameterlist)
                {
                    if (info2.Description == "isoutparam")
                    {
                        info = info2;
                    }
                }
                return info;
            }
        }
    }





    [Serializable]
    public class TableHost : DatabaseHost
    {
        // Fields
        private string _tablename;
        private string _tabledescription;
        private List<ColumnInfo> _keys;
        private List<ColumnInfo> _fkeys;
        private List<ColumnInfo> _fieldlist;
        private string _folder;

        // Methods
        public TableHost()
        {
            this._tabledescription = "";
        }

        public TableHost(string TableName)
        {
            this._tabledescription = "";
        }

        // Properties
        public string TableName
        {
            get =>
                this._tablename;
            set =>
                this._tablename = value;
        }

        public string TableDescription
        {
            get =>
                this._tabledescription;
            set =>
                this._tabledescription = value;
        }

        public List<ColumnInfo> Fieldlist
        {
            get =>
                this._fieldlist;
            set =>
                this._fieldlist = value;
        }

        public List<ColumnInfo> Keys
        {
            get =>
                this._keys;
            set =>
                this._keys = value;
        }

        public List<ColumnInfo> FKeys
        {
            get =>
                this._fkeys;
            set =>
                this._fkeys = value;
        }

        public string Folder
        {
            get =>
                this._folder;
            set =>
                this._folder = value;
        }

        public ColumnInfo IdentityKey
        {
            get
            {
                ColumnInfo info = null;
                foreach (ColumnInfo info2 in this._keys)
                {
                    if (info2.IsIdentity)
                    {
                        info = info2;
                    }
                }
                return info;
            }
        }
    }





    [Serializable]
    public class DatabaseHost : TemplateHost
    {
        // Fields
        private string _dbname;
        private string _dbhelperName = "DbHelperSQL";
        private List<TableInfo> _tablelist;
        private List<TableInfo> _viewlist;
        private List<TableInfo> _procedurelist;
        private string _dbtype;
        private string _procprefix = "";
        private string _projectname = "";
        private DbSettings _dbset;
        private string modelPrefix = "";
        private string modelSuffix = "";
        private string bllPrefix = "";
        private string bllSuffix = "";
        private string dalPrefix = "";
        private string dalSuffix = "";
        private string tabnameRule = "same";

        // Methods
        public string GetBLLClass(string TabName) =>
            (this._dbset.BLLPrefix + this.TabNameRuled(TabName) + this._dbset.BLLSuffix);

        public string GetDALClass(string TabName) =>
            (this._dbset.DALPrefix + this.TabNameRuled(TabName) + this._dbset.DALSuffix);

        public string GetModelClass(string TabName) =>
            (this._dbset.ModelPrefix + this.TabNameRuled(TabName) + this._dbset.ModelSuffix);

        private string TabNameRuled(string TabName)
        {
            string str = TabName;
            if (this._dbset.ReplacedOldStr.Length > 0)
            {
                str = str.Replace(this._dbset.ReplacedOldStr, this._dbset.ReplacedNewStr);
            }
            string str3 = this._dbset.TabNameRule.ToLower();
            if (str3 == null)
            {
                return str;
            }
            if (str3 != "lower")
            {
                if (str3 != "upper")
                {
                    if (str3 == "firstupper")
                    {
                        return (str.Substring(0, 1).ToUpper() + str.Substring(1));
                    }
                    if (str3 == "same")
                    {
                    }
                    return str;
                }
            }
            else
            {
                return str.ToLower();
            }
            return str.ToUpper();
        }

        // Properties
        public string DbName
        {
            get =>
                this._dbname;
            set =>
                this._dbname = value;
        }

        public string DbHelperName
        {
            get =>
                this._dbhelperName;
            set =>
                this._dbhelperName = value;
        }

        public List<TableInfo> ViewList
        {
            get =>
                this._viewlist;
            set =>
                this._viewlist = value;
        }

        public List<TableInfo> ProcedureList
        {
            get =>
                this._procedurelist;
            set =>
                this._procedurelist = value;
        }

        public List<TableInfo> TableList
        {
            get =>
                this._tablelist;
            set =>
                this._tablelist = value;
        }

        public string DbType
        {
            get =>
                this._dbtype;
            set =>
                this._dbtype = value;
        }

        public DbSettings DbSet
        {
            get =>
                this._dbset;
            set =>
                this._dbset = value;
        }

        public string ProcPrefix
        {
            get =>
                this._procprefix;
            set =>
                this._procprefix = value;
        }

        public string ProjectName
        {
            get =>
                this._projectname;
            set =>
                this._projectname = value;
        }

        public string ModelPrefix
        {
            get =>
                this.modelPrefix;
            set =>
                this.modelPrefix = value;
        }

        public string ModelSuffix
        {
            get =>
                this.modelSuffix;
            set =>
                this.modelSuffix = value;
        }

        public string BLLPrefix
        {
            get =>
                this.bllPrefix;
            set =>
                this.bllPrefix = value;
        }

        public string BLLSuffix
        {
            get =>
                this.bllSuffix;
            set =>
                this.bllSuffix = value;
        }

        public string DALPrefix
        {
            get =>
                this.dalPrefix;
            set =>
                this.dalPrefix = value;
        }

        public string DALSuffix
        {
            get =>
                this.dalSuffix;
            set =>
                this.dalSuffix = value;
        }

        public string TabNameRule
        {
            get =>
                this.tabnameRule;
            set =>
                this.tabnameRule = value;
        }

        public string DbParaHead =>
            CodeCommon.DbParaHead(this.DbType);

        public string DbParaDbType =>
            CodeCommon.DbParaDbType(this.DbType);

        public string preParameter =>
            CodeCommon.preParameter(this.DbType);
    }






    public class CodeCommon
    {
        // Fields
        private static string datatypefile = (AppDomain.CurrentDomain.BaseDirectory.TrimEnd(new char[] { '\\' }) + @"\DatatypeMap.cfg");
        private static char[] hexDigits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        // Methods
        public static int CompareByintOrder(ColumnInfo x, ColumnInfo y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                return -1;
            }
            if (y != null)
            {
                int num = 0;
                int num2 = 0;
                try
                {
                    num = Convert.ToInt32(x.ColumnOrder);
                }
                catch
                {
                    return -1;
                }
                try
                {
                    num2 = Convert.ToInt32(y.ColumnOrder);
                }
                catch
                {
                    return 1;
                }
                if (num < num2)
                {
                    return -1;
                }
                if (x == y)
                {
                    return 0;
                }
            }
            return 1;
        }

        public static int CompareByOrder(ColumnInfo x, ColumnInfo y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            return x.ColumnOrder.CompareTo(y.ColumnOrder);
        }

        public static string CSToProcType(string DbType, string cstype)
        {
            string str = cstype;
            switch (DbType)
            {
                case "SQL2000":
                case "SQL2005":
                case "SQL2008":
                case "SQL2012":
                    return CSToProcTypeSQL(cstype);

                case "Oracle":
                    return CSToProcTypeOra(cstype);

                case "MySQL":
                    return CSToProcTypeMySQL(cstype);

                case "OleDb":
                    return CSToProcTypeOleDb(cstype);

                case "SQLite":
                    return CSToProcTypeSQLite(cstype);
            }
            return str;
        }

        private static string CSToProcTypeMySQL(string cstype)
        {
            string str = cstype;
            if (!File.Exists(datatypefile))
            {
                return str;
            }
            string str2 = DatatypeMap.GetValueFromCfg(datatypefile, "MySQLDbType", cstype.ToLower().Trim());
            if (str2 == "")
            {
                return cstype.ToLower().Trim();
            }
            return str2;
        }

        private static string CSToProcTypeOleDb(string cstype)
        {
            string str = cstype;
            if (!File.Exists(datatypefile))
            {
                return str;
            }
            string str2 = DatatypeMap.GetValueFromCfg(datatypefile, "OleDbDbType", cstype.ToLower().Trim());
            if (str2 == "")
            {
                return cstype.ToLower().Trim();
            }
            return str2;
        }

        private static string CSToProcTypeOra(string cstype)
        {
            string str = cstype;
            if (!File.Exists(datatypefile))
            {
                return str;
            }
            string str2 = DatatypeMap.GetValueFromCfg(datatypefile, "OraDbType", cstype.ToLower().Trim());
            if (str2 == "")
            {
                return cstype.ToLower().Trim();
            }
            return str2;
        }

        private static string CSToProcTypeSQL(string cstype)
        {
            string str = cstype;
            if (!File.Exists(datatypefile))
            {
                return str;
            }
            string str2 = DatatypeMap.GetValueFromCfg(datatypefile, "SQLDbType", cstype.ToLower().Trim());
            if (str2 == "")
            {
                return cstype.ToLower().Trim();
            }
            return str2;
        }

        private static string CSToProcTypeSQLite(string cstype)
        {
            string str = cstype;
            if (!File.Exists(datatypefile))
            {
                return str;
            }
            string str2 = DatatypeMap.GetValueFromCfg(datatypefile, "SQLiteType", cstype.ToLower().Trim());
            if (str2 == "")
            {
                return cstype.ToLower().Trim();
            }
            return str2;
        }

        public static string CutDescText(string descText, int cutLen, string ReplaceText)
        {
            if (descText.Trim().Length > 0)
            {
                int num = 0;
                int index = descText.IndexOf(";");
                int num3 = descText.IndexOf("，");
                int num4 = descText.IndexOf(",");
                num = Math.Min(index, num3);
                if (num < 0)
                {
                    num = Math.Max(index, num3);
                }
                num = Math.Min(num, num4);
                if (num < 0)
                {
                    num = Math.Max(index, num3);
                }
                if (num > 0)
                {
                    return descText.Trim().Substring(0, num);
                }
                if (descText.Trim().Length > cutLen)
                {
                    return descText.Trim().Substring(0, cutLen);
                }
                return descText.Trim();
            }
            return ReplaceText;
        }

        public static string DbParaDbType(string DbType)
        {
            switch (DbType)
            {
                case "SQL2000":
                case "SQL2005":
                case "SQL2008":
                case "SQL2012":
                    return "SqlDbType";

                case "Oracle":
                    return "OracleType";

                case "OleDb":
                    return "OleDbType";

                case "MySQL":
                    return "MySqlDbType";

                case "SQLite":
                    return "DbType";
            }
            return "SqlDbType";
        }

        public static string DbParaHead(string DbType)
        {
            switch (DbType)
            {
                case "SQL2000":
                case "SQL2005":
                case "SQL2008":
                case "SQL2012":
                    return "Sql";

                case "Oracle":
                    return "Oracle";

                case "MySQL":
                    return "MySql";

                case "OleDb":
                    return "OleDb";

                case "SQLite":
                    return "SQLite";
            }
            return "Sql";
        }

        public static string DbTypeLength(string dbtype, string datatype, string Length)
        {
            switch (dbtype)
            {
                case "SQL2000":
                case "SQL2005":
                case "SQL2008":
                case "SQL2012":
                    return DbTypeLengthSQL(dbtype, datatype, Length);

                case "Oracle":
                    return DbTypeLengthOra(datatype, Length);

                case "MySQL":
                    return DbTypeLengthMySQL(datatype, Length);

                case "OleDb":
                    return DbTypeLengthOleDb(datatype, Length);

                case "SQLite":
                    return DbTypeLengthSQLite(datatype, Length);
            }
            return "";
        }

        private static string DbTypeLengthMySQL(string datatype, string Length)
        {
            string str = "";
            switch (datatype.Trim().ToLower())
            {
                case "number":
                    if (Length != "")
                    {
                        str = Length;
                        break;
                    }
                    str = "4";
                    break;

                case "varchar2":
                    if (Length != "")
                    {
                        str = Length;
                        break;
                    }
                    str = "50";
                    break;

                case "char":
                    if (Length != "")
                    {
                        str = Length;
                        break;
                    }
                    str = "50";
                    break;

                case "date":
                case "nchar":
                case "nvarchar2":
                case "long":
                case "long raw":
                case "bfile":
                case "blob":
                    break;

                default:
                    str = Length;
                    break;
            }
            if (str != "")
            {
                return (CSToProcType("MySQL", datatype) + "," + str);
            }
            return CSToProcType("MySQL", datatype);
        }

        private static string DbTypeLengthOleDb(string datatype, string Length)
        {
            string str = "";
            switch (datatype.Trim())
            {
                case "int":
                    if (Length != "")
                    {
                        str = Length;
                        break;
                    }
                    str = "4";
                    break;

                case "varchar":
                    if (Length != "")
                    {
                        str = Length;
                        break;
                    }
                    str = "50";
                    break;

                case "char":
                    if (Length != "")
                    {
                        str = Length;
                        break;
                    }
                    str = "50";
                    break;

                case "bit":
                    str = "1";
                    break;

                case "float":
                case "numeric":
                case "decimal":
                case "money":
                case "smallmoney":
                case "binary":
                case "smallint":
                case "bigint":
                    str = Length;
                    break;

                case "image":
                case "datetime":
                case "smalldatetime":
                case "nchar":
                case "nvarchar":
                case "ntext":
                case "text":
                    break;

                default:
                    str = Length;
                    break;
            }
            if (str != "")
            {
                return (CSToProcType("OleDb", datatype) + "," + str);
            }
            return CSToProcType("OleDb", datatype);
        }

        private static string DbTypeLengthOra(string datatype, string Length)
        {
            string str = "";
            switch (datatype.Trim().ToLower())
            {
                case "number":
                    if (Length != "")
                    {
                        str = Length;
                        break;
                    }
                    str = "4";
                    break;

                case "varchar2":
                    if (Length != "")
                    {
                        str = Length;
                        break;
                    }
                    str = "50";
                    break;

                case "char":
                    if (Length != "")
                    {
                        str = Length;
                        break;
                    }
                    str = "50";
                    break;

                case "date":
                case "nchar":
                case "nvarchar2":
                case "long":
                case "long raw":
                case "bfile":
                case "blob":
                    break;

                default:
                    str = Length;
                    break;
            }
            if (str != "")
            {
                return (CSToProcType("Oracle", datatype) + "," + str);
            }
            return CSToProcType("Oracle", datatype);
        }

        private static string DbTypeLengthSQL(string dbtype, string datatype, string Length)
        {
            string dataTypeLenVal = GetDataTypeLenVal(datatype, Length);
            switch (dataTypeLenVal)
            {
                case "":
                    return CSToProcType(dbtype, datatype);

                case "MAX":
                    dataTypeLenVal = "-1";
                    break;
            }
            return (CSToProcType(dbtype, datatype) + "," + dataTypeLenVal);
        }

        private static string DbTypeLengthSQLite(string datatype, string Length)
        {
            string str = "";
            switch (datatype.Trim())
            {
                case "int":
                case "integer":
                    if (Length != "")
                    {
                        str = Length;
                        break;
                    }
                    str = "4";
                    break;

                case "varchar":
                    if (Length != "")
                    {
                        str = Length;
                        break;
                    }
                    str = "50";
                    break;

                case "char":
                    if (Length != "")
                    {
                        str = Length;
                        break;
                    }
                    str = "50";
                    break;

                case "bit":
                    str = "1";
                    break;

                case "float":
                case "numeric":
                case "decimal":
                case "money":
                case "smallmoney":
                case "binary":
                case "smallint":
                case "bigint":
                case "blob":
                    str = Length;
                    break;

                case "image":
                case "datetime":
                case "smalldatetime":
                case "nchar":
                case "nvarchar":
                case "ntext":
                case "text":
                case "time":
                case "date":
                case "boolean":
                    break;

                default:
                    str = Length;
                    break;
            }
            if (str != "")
            {
                return (CSToProcType("SQLite", datatype) + "," + str);
            }
            return CSToProcType("SQLite", datatype);
        }

        public static string DbTypeToCS(string dbtype)
        {
            string str = "string";
            if (!File.Exists(datatypefile))
            {
                return str;
            }
            string str2 = DatatypeMap.GetValueFromCfg(datatypefile, "DbToCS", dbtype.ToLower().Trim());
            if (str2 == "")
            {
                return dbtype.ToLower().Trim();
            }
            return str2;
        }

        public static DataTable GetColumnInfoDt(List<ColumnInfo> collist)
        {
            DataTable table = new DataTable();
            table.Columns.Add("colorder");
            table.Columns.Add("ColumnName");
            table.Columns.Add("TypeName");
            table.Columns.Add("Length");
            table.Columns.Add("Preci");
            table.Columns.Add("Scale");
            table.Columns.Add("IsIdentity");
            table.Columns.Add("isPK");
            table.Columns.Add("cisNull");
            table.Columns.Add("defaultVal");
            table.Columns.Add("deText");
            foreach (ColumnInfo info in collist)
            {
                DataRow row = table.NewRow();
                row["colorder"] = info.ColumnOrder;
                row["ColumnName"] = info.ColumnName;
                row["TypeName"] = info.TypeName;
                row["Length"] = info.Length;
                row["Preci"] = info.Precision;
                row["Scale"] = info.Scale;
                row["IsIdentity"] = info.IsIdentity ? "√" : "";
                row["isPK"] = info.IsPrimaryKey ? "√" : "";
                row["cisNull"] = info.Nullable ? "√" : "";
                row["defaultVal"] = info.DefaultVal;
                row["deText"] = info.Description;
                table.Rows.Add(row);
            }
            return table;
        }

        public static List<ColumnInfo> GetColumnInfos(DataTable dt)
        {
            List<ColumnInfo> list = new List<ColumnInfo>();
            if (dt == null)
            {
                return null;
            }
            ArrayList list2 = new ArrayList();
            foreach (DataRow row in dt.Rows)
            {
                string str = row["Colorder"].ToString();
                string item = row["ColumnName"].ToString();
                string str3 = row["TypeName"].ToString();
                string str4 = row["IsIdentity"].ToString();
                string str5 = row["IsPK"].ToString();
                string str6 = row["Length"].ToString();
                string str7 = row["Preci"].ToString();
                string str8 = row["Scale"].ToString();
                string str9 = row["cisNull"].ToString();
                string str10 = row["DefaultVal"].ToString();
                string str11 = row["DeText"].ToString();
                ColumnInfo info = new ColumnInfo
                {
                    ColumnOrder = str,
                    ColumnName = item,
                    TypeName = str3,
                    IsIdentity = (str4 == "√") ? true : false,
                    IsPrimaryKey = (str5 == "√") ? true : false,
                    Length = str6,
                    Precision = str7,
                    Scale = str8,
                    Nullable = (str9 == "√") ? true : false,
                    DefaultVal = str10,
                    Description = str11
                };
                if (!list2.Contains(item))
                {
                    list.Add(info);
                    list2.Add(item);
                }
            }
            return list;
        }

        public static string GetDataTypeLenVal(string datatype, string Length)
        {
            string str = "";
            switch (datatype.Trim())
            {
                case "int":
                    if (Length != "")
                    {
                        return Length;
                    }
                    return "4";

                case "char":
                    if (Length.Trim() != "")
                    {
                        return Length;
                    }
                    return "10";

                case "nchar":
                    str = Length;
                    if (Length.Trim() == "")
                    {
                        str = "10";
                    }
                    return str;

                case "varchar":
                case "nvarchar":
                case "varbinary":
                    str = Length;
                    if (((Length.Length != 0) && (Length != "0")) && (Length != "-1"))
                    {
                        return str;
                    }
                    return "MAX";

                case "bit":
                    return "1";

                case "float":
                case "numeric":
                case "decimal":
                case "money":
                case "smallmoney":
                case "binary":
                case "smallint":
                case "bigint":
                    return Length;

                case "image":
                case "datetime":
                case "smalldatetime":
                case "ntext":
                case "text":
                    return str;
            }
            return Length;
        }

        public static string GetFieldstrlist(List<ColumnInfo> keys, bool IdentityisPrior)
        {
            StringPlus plus = new StringPlus();
            ColumnInfo identityKey = GetIdentityKey(keys);
            if (IdentityisPrior && (identityKey != null))
            {
                plus.Append(identityKey.ColumnName);
            }
            else
            {
                foreach (ColumnInfo info2 in keys)
                {
                    if (info2.IsPrimaryKey || !info2.IsIdentity)
                    {
                        plus.Append(info2.ColumnName + ",");
                    }
                }
                plus.DelLastComma();
            }
            return plus.Value;
        }

        public static string GetFieldstrlistAdd(List<ColumnInfo> keys, bool IdentityisPrior)
        {
            StringPlus plus = new StringPlus();
            ColumnInfo identityKey = GetIdentityKey(keys);
            if (IdentityisPrior && (identityKey != null))
            {
                plus.Append(identityKey.ColumnName);
            }
            else
            {
                foreach (ColumnInfo info2 in keys)
                {
                    if (info2.IsPrimaryKey || !info2.IsIdentity)
                    {
                        plus.Append(info2.ColumnName + "+");
                    }
                }
                plus.DelLastChar("+");
            }
            return plus.Value;
        }

        public static ColumnInfo GetIdentityKey(List<ColumnInfo> keys)
        {
            foreach (ColumnInfo info in keys)
            {
                if (info.IsIdentity)
                {
                    return info;
                }
            }
            return null;
        }

        public static string GetInParameter(List<ColumnInfo> keys, bool IdentityisPrior)
        {
            StringPlus plus = new StringPlus();
            ColumnInfo identityKey = GetIdentityKey(keys);
            if (IdentityisPrior && (identityKey != null))
            {
                plus.Append(DbTypeToCS(identityKey.TypeName) + " " + identityKey.ColumnName);
            }
            else
            {
                foreach (ColumnInfo info2 in keys)
                {
                    if (info2.IsPrimaryKey || !info2.IsIdentity)
                    {
                        plus.Append(DbTypeToCS(info2.TypeName) + " " + info2.ColumnName + ",");
                    }
                }
                plus.DelLastComma();
            }
            return plus.Value;
        }

        public static string GetModelWhereExpression(List<ColumnInfo> keys, bool IdentityisPrior)
        {
            StringPlus plus = new StringPlus();
            ColumnInfo identityKey = GetIdentityKey(keys);
            if (IdentityisPrior && (identityKey != null))
            {
                if (IsAddMark(identityKey.TypeName))
                {
                    plus.Append(identityKey.ColumnName + "='\"+ model." + identityKey.ColumnName + "+\"'");
                }
                else
                {
                    plus.Append(identityKey.ColumnName + "=\"+ model." + identityKey.ColumnName + "+\"");
                }
            }
            else
            {
                foreach (ColumnInfo info2 in keys)
                {
                    if (info2.IsPrimaryKey || !info2.IsIdentity)
                    {
                        if (IsAddMark(info2.TypeName))
                        {
                            plus.Append(info2.ColumnName + "='\"+ model." + info2.ColumnName + "+\"' and ");
                        }
                        else
                        {
                            plus.Append(info2.ColumnName + "=\"+ model." + info2.ColumnName + "+\" and ");
                        }
                    }
                }
                plus.DelLastChar("and");
            }
            return plus.Value;
        }

        public static string GetPreParameter(List<ColumnInfo> keys, bool IdentityisPrior, string DbType)
        {
            StringPlus plus = new StringPlus();
            StringPlus plus2 = new StringPlus();
            plus.AppendSpaceLine(3, DbParaHead(DbType) + "Parameter[] parameters = {");
            ColumnInfo identityKey = GetIdentityKey(keys);
            bool flag = HasNoIdentityKey(keys);
            if ((IdentityisPrior && (identityKey != null)) || (!flag && (identityKey != null)))
            {
                plus.AppendSpaceLine(5, "new " + DbParaHead(DbType) + "Parameter(\"" + preParameter(DbType) + identityKey.ColumnName + "\", " + DbParaDbType(DbType) + "." + DbTypeLength(DbType, identityKey.TypeName, "") + ")");
                plus2.AppendSpaceLine(3, "parameters[0].Value = " + identityKey.ColumnName + ";");
            }
            else
            {
                int num = 0;
                foreach (ColumnInfo info2 in keys)
                {
                    if (info2.IsPrimaryKey || !info2.IsIdentity)
                    {
                        plus.AppendSpaceLine(5, "new " + DbParaHead(DbType) + "Parameter(\"" + preParameter(DbType) + info2.ColumnName + "\", " + DbParaDbType(DbType) + "." + DbTypeLength(DbType, info2.TypeName, info2.Length) + "),");
                        plus2.AppendSpaceLine(3, "parameters[" + num.ToString() + "].Value = " + info2.ColumnName + ";");
                        num++;
                    }
                }
                plus.DelLastComma();
            }
            plus.AppendSpaceLine(3, "};");
            plus.Append(plus2.Value);
            return plus.Value;
        }

        public static string GetWhereExpression(List<ColumnInfo> keys, bool IdentityisPrior)
        {
            StringPlus plus = new StringPlus();
            ColumnInfo identityKey = GetIdentityKey(keys);
            if (IdentityisPrior && (identityKey != null))
            {
                if (IsAddMark(identityKey.TypeName))
                {
                    plus.Append(identityKey.ColumnName + "='\"+" + identityKey.ColumnName + "+\"'");
                }
                else
                {
                    plus.Append(identityKey.ColumnName + "=\"+" + identityKey.ColumnName + "+\"");
                }
            }
            else
            {
                foreach (ColumnInfo info2 in keys)
                {
                    if (info2.IsPrimaryKey || !info2.IsIdentity)
                    {
                        if (IsAddMark(info2.TypeName))
                        {
                            plus.Append(info2.ColumnName + "='\"+" + info2.ColumnName + "+\"' and ");
                        }
                        else
                        {
                            plus.Append(info2.ColumnName + "=\"+" + info2.ColumnName + "+\" and ");
                        }
                    }
                }
                plus.DelLastChar("and");
            }
            return plus.Value;
        }

        public static string GetWhereParameterExpression(List<ColumnInfo> keys, bool IdentityisPrior, string DbType)
        {
            StringPlus plus = new StringPlus();
            ColumnInfo identityKey = GetIdentityKey(keys);
            bool flag = HasNoIdentityKey(keys);
            if ((IdentityisPrior && (identityKey != null)) || (!flag && (identityKey != null)))
            {
                plus.Append(identityKey.ColumnName + "=" + preParameter(DbType) + identityKey.ColumnName);
            }
            else
            {
                foreach (ColumnInfo info2 in keys)
                {
                    if (info2.IsPrimaryKey || !info2.IsIdentity)
                    {
                        plus.Append(info2.ColumnName + "=" + preParameter(DbType) + info2.ColumnName + " and ");
                    }
                }
                plus.DelLastChar("and");
            }
            return plus.Value;
        }

        public static bool HasNoIdentityKey(List<ColumnInfo> keys)
        {
            foreach (ColumnInfo info in keys)
            {
                if (info.IsPrimaryKey && !info.IsIdentity)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsAddMark(string columnType)
        {
            bool flag = false;
            if (!File.Exists(datatypefile))
            {
                return flag;
            }
            string str = DatatypeMap.GetValueFromCfg(datatypefile, "AddMark", columnType.ToLower().Trim());
            if ((str != "true") && (str != "是"))
            {
                return flag;
            }
            return true;
        }

        public static bool IsHasIdentity(List<ColumnInfo> Keys)
        {
            bool flag = false;
            if (Keys.Count > 0)
            {
                foreach (ColumnInfo info in Keys)
                {
                    if (info.IsIdentity)
                    {
                        flag = true;
                    }
                }
            }
            return flag;
        }

        public static bool isValueType(string cstype)
        {
            bool flag = false;
            if (!File.Exists(datatypefile))
            {
                return flag;
            }
            string str = DatatypeMap.GetValueFromCfg(datatypefile, "ValueType", cstype.Trim());
            if ((str != "true") && (str != "是"))
            {
                return flag;
            }
            return true;
        }

        public static string preParameter(string DbType)
        {
            string str = "@";
            if (!File.Exists(datatypefile))
            {
                return str;
            }
            string str2 = DatatypeMap.GetValueFromCfg(datatypefile, "ParamePrefix", DbType.ToUpper().Trim());
            if (str2 == "")
            {
                return DbType.ToUpper().Trim();
            }
            return str2;
        }

        public static string Space(int num)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < num; i++)
            {
                builder.Append("\t");
            }
            return builder.ToString();
        }

        public static string ToHexString(byte[] bytes)
        {
            char[] chArray = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int num2 = bytes[i];
                chArray[i * 2] = hexDigits[num2 >> 4];
                chArray[(i * 2) + 1] = hexDigits[num2 & 15];
            }
            string str = new string(chArray);
            return ("0x" + str.Substring(0, bytes.Length));
        }
    }

    public class StringPlus
    {
        // Fields
        private StringBuilder str = new StringBuilder();

        // Methods
        public string Append(string Text)
        {
            this.str.Append(Text);
            return this.str.ToString();
        }

        public string AppendLine()
        {
            this.str.Append("\r\n");
            return this.str.ToString();
        }

        public string AppendLine(string Text)
        {
            this.str.Append(Text + "\r\n");
            return this.str.ToString();
        }

        public string AppendSpace(int SpaceNum, string Text)
        {
            this.str.Append(this.Space(SpaceNum));
            this.str.Append(Text);
            return this.str.ToString();
        }

        public string AppendSpaceLine(int SpaceNum, string Text)
        {
            this.str.Append(this.Space(SpaceNum));
            this.str.Append(Text);
            this.str.Append("\r\n");
            return this.str.ToString();
        }

        public void DelLastChar(string strchar)
        {
            string str = this.str.ToString();
            int length = str.LastIndexOf(strchar);
            if (length > 0)
            {
                this.str = new StringBuilder();
                this.str.Append(str.Substring(0, length));
            }
        }

        public void DelLastComma()
        {
            string str = this.str.ToString();
            int length = str.LastIndexOf(",");
            if (length > 0)
            {
                this.str = new StringBuilder();
                this.str.Append(str.Substring(0, length));
            }
        }

        public void Remove(int Start, int Num)
        {
            this.str.Remove(Start, Num);
        }

        public string Space(int SpaceNum)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < SpaceNum; i++)
            {
                builder.Append("\t");
            }
            return builder.ToString();
        }

        public override string ToString() =>
            this.str.ToString();

        // Properties
        public string Value =>
            this.str.ToString();
    }



    [Serializable]
    public class CodeInfo
    {
        // Fields
        private string _code;
        private string _errormsg;
        private string _fileExtensionValue = ".cs";

        // Properties
        public string Code
        {
            get =>
                this._code;
            set =>
                this._code = value;
        }

        public string ErrorMsg
        {
            get =>
                this._errormsg;
            set =>
                this._errormsg = value;
        }

        public string FileExtension
        {
            get =>
                this._fileExtensionValue;
            set =>
                this._fileExtensionValue = value;
        }
    }

    public class CodeKey
    {
        // Fields
        private string _keyName;
        private string _keyType;
        private bool _isPK;
        private bool _isIdentity;


        // Properties
        public string KeyName { get; set; }
        public string KeyType { get; set; }
        public bool IsPK { get; set; }
        public bool IsIdentity { get; set; }
    }

    public class CodeKeyMaker
    {
    }


    public static class DatatypeMap
    {
        // Methods
        public static string GetValueFromCfg(string filename, string xpathTypeName, string Key)
        {
            string str;
            try
            {
                Hashtable hashtable = new Hashtable();
                XmlDocument document = new XmlDocument();
                document.Load(filename);
                XmlNode node = document.SelectSingleNode("Map/" + xpathTypeName);
                if (node != null)
                {
                    foreach (XmlNode node2 in node.ChildNodes)
                    {
                        hashtable.Add(node2.Attributes["key"].Value, node2.Attributes["value"].Value);
                    }
                }
                object obj2 = hashtable[Key];
                if (obj2 != null)
                {
                    return obj2.ToString();
                }
                str = "";
            }
            catch (Exception exception)
            {
                throw new Exception("Load DatatypeMap file fail:" + exception.Message);
            }
            return str;
        }

        public static Hashtable LoadFromCfg(string filename, string xpathTypeName)
        {
            Hashtable hashtable2;
            try
            {
                Hashtable hashtable = new Hashtable();
                XmlDocument document = new XmlDocument();
                document.Load(filename);
                XmlNode node = document.SelectSingleNode("Map/" + xpathTypeName);
                if (node != null)
                {
                    foreach (XmlNode node2 in node.ChildNodes)
                    {
                        hashtable.Add(node2.Attributes["key"].Value, node2.Attributes["value"].Value);
                    }
                }
                hashtable2 = hashtable;
            }
            catch (Exception exception)
            {
                throw new Exception("Load DatatypeMap file fail:" + exception.Message);
            }
            return hashtable2;
        }

        public static Hashtable LoadFromCfg(XmlDocument doc, string TypeName)
        {
            Hashtable hashtable2;
            try
            {
                Hashtable hashtable = new Hashtable();
                XmlNode node = doc.SelectSingleNode("Map/" + TypeName);
                if (node != null)
                {
                    foreach (XmlNode node2 in node.ChildNodes)
                    {
                        hashtable.Add(node2.Attributes["key"].Value, node2.Attributes["value"].Value);
                    }
                }
                hashtable2 = hashtable;
            }
            catch (Exception exception)
            {
                throw new Exception("Load DatatypeMap file fail:" + exception.Message);
            }
            return hashtable2;
        }

        public static bool SaveCfg(string filename, string NodeText, Hashtable list)
        {
            bool flag;
            try
            {
                XmlDocument document = new XmlDocument();
                XmlNode newChild = document.CreateNode(XmlNodeType.XmlDeclaration, "", "");
                document.AppendChild(newChild);
                XmlElement element = document.CreateElement("", NodeText, "");
                document.AppendChild(element);
                foreach (DictionaryEntry entry in list)
                {
                    XmlElement element2 = document.CreateElement("", NodeText, "");
                    XmlAttribute attribute = document.CreateAttribute("key");
                    attribute.Value = entry.Key.ToString();
                    element2.Attributes.Append(attribute);
                    XmlAttribute attribute2 = document.CreateAttribute("value");
                    attribute2.Value = entry.Value.ToString();
                    element2.Attributes.Append(attribute2);
                    element.AppendChild(element2);
                }
                document.Save(filename);
                flag = true;
            }
            catch (Exception exception)
            {
                throw new Exception("Save DatatypeMap file fail:" + exception.Message);
            }
            return flag;
        }
    }



    public class DbObjectTool
    {
        // Methods
        public static string DefaultValToCS(string DefaultVal)
        {
            string str = DefaultVal;
            bool flag1 = DefaultVal.Substring(0, 2) == "N'";
            bool flag2 = DefaultVal == "N'";
            return str;
        }
    }

    [Serializable]
    public class TableInfo
    {
        // Fields
        private string _tabName = "";
        private string _tabUser = "";
        private string _tabType = "";
        private string _tabDate = "";

        // Properties
        public string TabName
        {
            get =>
                this._tabName;
            set =>
                this._tabName = value;
        }

        public string TabUser
        {
            get =>
                this._tabUser;
            set =>
                this._tabUser = value;
        }

        public string TabType
        {
            get =>
                this._tabType;
            set =>
                this._tabType = value;
        }

        public string TabDate
        {
            get =>
                this._tabDate;
            set =>
                this._tabDate = value;
        }
    }








    [Serializable]
    public class ColumnInfo
    {
        // Fields
        private string _colorder;
        private string _columnName;
        private string _typeName = "";
        private string _length = "";
        private string _precision = "";
        private string _scale = "";
        private bool _isIdentity;
        private bool _isprimaryKey;
        private bool _isForeignKey;
        private bool _nullable;
        private string _defaultVal = "";
        private string _description = "";

        // Properties
        public string ColumnOrder
        {
            get =>
                this._colorder;
            set =>
                this._colorder = value;
        }

        public string ColumnName
        {
            get =>
                this._columnName;
            set =>
                this._columnName = value;
        }

        public string TypeName
        {
            get =>
                this._typeName;
            set =>
                this._typeName = value;
        }

        public string Length
        {
            get =>
                this._length;
            set =>
                this._length = value;
        }

        public string Precision
        {
            get =>
                this._precision;
            set =>
                this._precision = value;
        }

        public string Scale
        {
            get =>
                this._scale;
            set =>
                this._scale = value;
        }

        public bool IsIdentity
        {
            get =>
                this._isIdentity;
            set =>
                this._isIdentity = value;
        }

        public bool IsPrimaryKey
        {
            get =>
                this._isprimaryKey;
            set =>
                this._isprimaryKey = value;
        }

        public bool IsForeignKey
        {
            get =>
                this._isForeignKey;
            set =>
                this._isForeignKey = value;
        }

        public bool Nullable
        {
            get =>
                this._nullable;
            set =>
                this._nullable = value;
        }

        public string DefaultVal
        {
            get =>
                this._defaultVal;
            set =>
                this._defaultVal = value;
        }

        public string Description
        {
            get =>
                this._description;
            set =>
                this._description = value;
        }
    }


    public class DbConfig
    {
        // Fields
        private static string fileName = (AppDomain.CurrentDomain.BaseDirectory + @"\DbSetting.config");

        // Methods
        public static void AddColForTable(DataTable table)
        {
            DataColumn column;
            if (!table.Columns.Contains("DbType"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "DbType"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("Server"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "Server"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("ConnectStr"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "ConnectStr"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("DbName"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "DbName"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("ConnectSimple"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.Boolean"),
                    ColumnName = "ConnectSimple"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("TabLoadtype"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.Int32"),
                    ColumnName = "TabLoadtype"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("TabLoadKeyword"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "TabLoadKeyword"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("ProcPrefix"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "ProcPrefix"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("ProjectName"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "ProjectName"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("Namepace"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "Namepace"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("Folder"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "Folder"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("AppFrame"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "AppFrame"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("DALType"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "DALType"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("BLLType"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "BLLType"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("WebType"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "WebType"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("EditFont"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "EditFont"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("EditFontSize"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.Double"),
                    ColumnName = "EditFontSize"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("DbHelperName"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "DbHelperName"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("ModelPrefix"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "ModelPrefix"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("ModelSuffix"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "ModelSuffix"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("BLLPrefix"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "BLLPrefix"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("BLLSuffix"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "BLLSuffix"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("DALPrefix"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "DALPrefix"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("DALSuffix"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "DALSuffix"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("TabNameRule"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "TabNameRule"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("WebTemplatePath"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "WebTemplatePath"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("ReplacedOldStr"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "ReplacedOldStr"
                };
                table.Columns.Add(column);
            }
            if (!table.Columns.Contains("ReplacedNewStr"))
            {
                column = new DataColumn
                {
                    DataType = Type.GetType("System.String"),
                    ColumnName = "ReplacedNewStr"
                };
                table.Columns.Add(column);
            }
        }

        public static int AddSettings(DbSettings dbobj)
        {
            try
            {
                DataSet set = new DataSet();
                if (!File.Exists(fileName))
                {
                    DataTable dt = CreateDataTable();
                    DataRow row = NewDataRow(dt, dbobj);
                    dt.Rows.Add(row);
                    set.Tables.Add(dt);
                }
                else
                {
                    set.ReadXml(fileName);
                    if (set.Tables.Count > 0)
                    {
                        if (set.Tables[0].Select("DbType='" + dbobj.DbType + "' and Server='" + dbobj.Server + "' and DbName='" + dbobj.DbName + "'").Length > 0)
                        {
                            return 2;
                        }
                        DataRow row = NewDataRow(set.Tables[0], dbobj);
                        set.Tables[0].Rows.Add(row);
                    }
                    else
                    {
                        DataTable dt = CreateDataTable();
                        DataRow row = NewDataRow(dt, dbobj);
                        dt.Rows.Add(row);
                        set.Tables.Add(dt);
                    }
                }
                set.WriteXml(fileName);
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public static DataTable CreateDataTable()
        {
            DataTable table = new DataTable("DBServer");
            DataColumn column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "DbType"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "Server"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "ConnectStr"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "DbName"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.Boolean"),
                ColumnName = "ConnectSimple"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.Int32"),
                ColumnName = "TabLoadtype"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "TabLoadKeyword"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "ProcPrefix"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "ProjectName"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "Namepace"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "Folder"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "AppFrame"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "DALType"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "BLLType"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "WebType"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "EditFont"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.Double"),
                ColumnName = "EditFontSize"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "DbHelperName"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "ModelPrefix"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "ModelSuffix"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "BLLPrefix"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "BLLSuffix"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "DALPrefix"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "DALSuffix"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "TabNameRule"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "WebTemplatePath"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "ReplacedOldStr"
            };
            table.Columns.Add(column);
            column = new DataColumn
            {
                DataType = Type.GetType("System.String"),
                ColumnName = "ReplacedNewStr"
            };
            table.Columns.Add(column);
            return table;
        }

        public static void DelSetting(string DbType, string Serverip, string DbName)
        {
            try
            {
                DataSet set = new DataSet();
                if (File.Exists(fileName))
                {
                    set.ReadXml(fileName);
                    if (set.Tables.Count > 0)
                    {
                        string filterExpression = "DbType='" + DbType + "' and Server='" + Serverip + "'";
                        if ((DbName.Trim() != "") && (DbName.Trim() != "master"))
                        {
                            filterExpression = filterExpression + " and DbName='" + DbName + "'";
                        }
                        DataRow[] rowArray = set.Tables[0].Select(filterExpression);
                        if (rowArray.Length > 0)
                        {
                            set.Tables[0].Rows.Remove(rowArray[0]);
                        }
                        set.Tables[0].AcceptChanges();
                    }
                }
                set.WriteXml(fileName);
            }
            catch
            {
            }
        }

        public static DbSettings GetSetting(string loneServername)
        {
            string dbType = "SQL2008";
            string serverip = "";
            string dbName = "";
            if (loneServername.StartsWith("(local)"))
            {
                int startIndex = 7;
                serverip = "(local)";
                int index = loneServername.IndexOf(")", startIndex);
                dbType = loneServername.Substring(startIndex + 1, (index - startIndex) - 1);
                if (loneServername.Length > (index + 1))
                {
                    dbName = loneServername.Substring(index + 2).Replace(")", "");
                }
            }
            else
            {
                int index = loneServername.IndexOf("(");
                serverip = loneServername.Substring(0, index);
                int num4 = loneServername.IndexOf(")", index);
                dbType = loneServername.Substring(index + 1, (num4 - index) - 1);
                if (loneServername.Length > (num4 + 1))
                {
                    dbName = loneServername.Substring(num4 + 2).Replace(")", "");
                }
            }
            return GetSetting(dbType, serverip, dbName);
        }

        public static DbSettings GetSetting(string DbType, string Serverip, string DbName)
        {
            try
            {
                DbSettings settings = null;
                DataSet ds = new DataSet();
                if (File.Exists(fileName))
                {
                    ds.ReadXml(fileName);
                    if (ds.Tables.Count > 0)
                    {
                        string filterExpression = "DbType='" + DbType + "' and Server='" + Serverip + "'";
                        if (DbName.Trim() != "")
                        {
                            filterExpression = filterExpression + " and DbName='" + DbName + "'";
                        }
                        DataRow[] rowArray = ds.Tables[0].Select(filterExpression);
                        if (rowArray.Length > 0)
                        {
                            DataRow dr = rowArray[0];
                            settings = TranDbSettings(ds, dr);
                        }
                    }
                }
                return settings;
            }
            catch
            {
                return null;
            }
        }

        public static DataSet GetSettingDs()
        {
            try
            {
                DataSet set = new DataSet();
                if (File.Exists(fileName))
                {
                    set.ReadXml(fileName);
                }
                return set;
            }
            catch
            {
                return null;
            }
        }

        public static DbSettings[] GetSettings()
        {
            try
            {
                DataSet ds = new DataSet();
                ArrayList list = new ArrayList();
                if (File.Exists(fileName))
                {
                    ds.ReadXml(fileName);
                    if (ds.Tables.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            DbSettings settings = TranDbSettings(ds, row);
                            list.Add(settings);
                        }
                    }
                }
                return (DbSettings[])list.ToArray(typeof(DbSettings));
            }
            catch
            {
                return null;
            }
        }

        private static DataRow NewDataRow(DataTable dt, DbSettings dbobj)
        {
            AddColForTable(dt);
            DataRow row = dt.NewRow();
            row["DbType"] = dbobj.DbType;
            row["Server"] = dbobj.Server;
            row["ConnectStr"] = dbobj.ConnectStr;
            row["DbName"] = dbobj.DbName;
            row["ConnectSimple"] = dbobj.ConnectSimple;
            row["TabLoadtype"] = dbobj.TabLoadtype;
            row["TabLoadKeyword"] = dbobj.TabLoadKeyword;
            row["ProcPrefix"] = dbobj.ProcPrefix;
            row["ProjectName"] = dbobj.ProjectName;
            row["Namepace"] = dbobj.Namepace;
            row["Folder"] = dbobj.Folder;
            row["AppFrame"] = dbobj.AppFrame;
            row["DALType"] = dbobj.DALType;
            row["BLLType"] = dbobj.BLLType;
            row["WebType"] = dbobj.WebType;
            row["EditFont"] = dbobj.EditFont;
            row["EditFontSize"] = dbobj.EditFontSize;
            row["DbHelperName"] = dbobj.DbHelperName;
            row["ModelPrefix"] = dbobj.ModelPrefix;
            row["ModelSuffix"] = dbobj.ModelSuffix;
            row["BLLPrefix"] = dbobj.BLLPrefix;
            row["BLLSuffix"] = dbobj.BLLSuffix;
            row["DALPrefix"] = dbobj.DALPrefix;
            row["DALSuffix"] = dbobj.DALSuffix;
            row["TabNameRule"] = dbobj.TabNameRule;
            row["WebTemplatePath"] = dbobj.WebTemplatePath;
            row["ReplacedOldStr"] = dbobj.ReplacedOldStr;
            row["ReplacedNewStr"] = dbobj.ReplacedNewStr;
            return row;
        }

        public static DbSettings TranDbSettings(DataSet ds, DataRow dr)
        {
            DbSettings settings = new DbSettings
            {
                DbType = dr["DbType"].ToString(),
                Server = dr["Server"].ToString(),
                ConnectStr = dr["ConnectStr"].ToString(),
                DbName = dr["DbName"].ToString()
            };
            if ((ds.Tables[0].Columns.Contains("ConnectSimple") && (dr["ConnectSimple"] != null)) && (dr["ConnectSimple"].ToString().Length > 0))
            {
                settings.ConnectSimple = bool.Parse(dr["ConnectSimple"].ToString());
            }
            if ((ds.Tables[0].Columns.Contains("TabLoadtype") && (dr["TabLoadtype"] != null)) && (dr["TabLoadtype"].ToString().Length > 0))
            {
                settings.TabLoadtype = int.Parse(dr["TabLoadtype"].ToString());
            }
            if ((ds.Tables[0].Columns.Contains("TabLoadKeyword") && (dr["TabLoadKeyword"] != null)) && (dr["TabLoadKeyword"].ToString().Length > 0))
            {
                settings.TabLoadKeyword = dr["TabLoadKeyword"].ToString();
            }
            if (ds.Tables[0].Columns.Contains("ProcPrefix") && (dr["ProcPrefix"] != null))
            {
                settings.ProcPrefix = dr["ProcPrefix"].ToString();
            }
            if (ds.Tables[0].Columns.Contains("ProjectName") && (dr["ProjectName"] != null))
            {
                settings.ProjectName = dr["ProjectName"].ToString();
            }
            if ((ds.Tables[0].Columns.Contains("Namepace") && (dr["Namepace"] != null)) && (dr["Namepace"].ToString().Length > 0))
            {
                settings.Namepace = dr["Namepace"].ToString();
            }
            if (ds.Tables[0].Columns.Contains("Folder") && (dr["Folder"] != null))
            {
                settings.Folder = dr["Folder"].ToString();
            }
            if ((ds.Tables[0].Columns.Contains("AppFrame") && (dr["AppFrame"] != null)) && (dr["AppFrame"].ToString().Length > 0))
            {
                settings.AppFrame = dr["AppFrame"].ToString();
            }
            if ((ds.Tables[0].Columns.Contains("DALType") && (dr["DALType"] != null)) && (dr["DALType"].ToString().Length > 0))
            {
                settings.DALType = dr["DALType"].ToString();
            }
            if ((ds.Tables[0].Columns.Contains("BLLType") && (dr["BLLType"] != null)) && (dr["BLLType"].ToString().Length > 0))
            {
                settings.BLLType = dr["BLLType"].ToString();
            }
            if ((ds.Tables[0].Columns.Contains("WebType") && (dr["WebType"] != null)) && (dr["WebType"].ToString().Length > 0))
            {
                settings.WebType = dr["WebType"].ToString();
            }
            if ((ds.Tables[0].Columns.Contains("EditFont") && (dr["EditFont"] != null)) && (dr["EditFont"].ToString().Length > 0))
            {
                settings.EditFont = dr["EditFont"].ToString();
            }
            if ((ds.Tables[0].Columns.Contains("EditFontSize") && (dr["EditFontSize"] != null)) && (dr["EditFontSize"].ToString().Length > 0))
            {
                settings.EditFontSize = float.Parse(dr["EditFontSize"].ToString());
            }
            if ((ds.Tables[0].Columns.Contains("DbHelperName") && (dr["DbHelperName"] != null)) && (dr["DbHelperName"].ToString().Length > 0))
            {
                settings.DbHelperName = dr["DbHelperName"].ToString();
            }
            if (ds.Tables[0].Columns.Contains("ModelPrefix") && (dr["ModelPrefix"] != null))
            {
                settings.ModelPrefix = dr["ModelPrefix"].ToString();
            }
            if (ds.Tables[0].Columns.Contains("ModelSuffix") && (dr["ModelSuffix"] != null))
            {
                settings.ModelSuffix = dr["ModelSuffix"].ToString();
            }
            if (ds.Tables[0].Columns.Contains("BLLPrefix") && (dr["BLLPrefix"] != null))
            {
                settings.BLLPrefix = dr["BLLPrefix"].ToString();
            }
            if (ds.Tables[0].Columns.Contains("BLLSuffix") && (dr["BLLSuffix"] != null))
            {
                settings.BLLSuffix = dr["BLLSuffix"].ToString();
            }
            if (ds.Tables[0].Columns.Contains("DALPrefix") && (dr["DALPrefix"] != null))
            {
                settings.DALPrefix = dr["DALPrefix"].ToString();
            }
            if (ds.Tables[0].Columns.Contains("DALSuffix") && (dr["DALSuffix"] != null))
            {
                settings.DALSuffix = dr["DALSuffix"].ToString();
            }
            if ((ds.Tables[0].Columns.Contains("TabNameRule") && (dr["TabNameRule"] != null)) && (dr["TabNameRule"].ToString().Length > 0))
            {
                settings.TabNameRule = dr["TabNameRule"].ToString();
            }
            if ((ds.Tables[0].Columns.Contains("WebTemplatePath") && (dr["WebTemplatePath"] != null)) && (dr["WebTemplatePath"].ToString().Length > 0))
            {
                settings.WebTemplatePath = dr["WebTemplatePath"].ToString();
            }
            if ((ds.Tables[0].Columns.Contains("ReplacedOldStr") && (dr["ReplacedOldStr"] != null)) && (dr["ReplacedOldStr"].ToString().Length > 0))
            {
                settings.ReplacedOldStr = dr["ReplacedOldStr"].ToString();
            }
            if ((ds.Tables[0].Columns.Contains("ReplacedNewStr") && (dr["ReplacedNewStr"] != null)) && (dr["ReplacedNewStr"].ToString().Length > 0))
            {
                settings.ReplacedNewStr = dr["ReplacedNewStr"].ToString();
            }
            return settings;
        }

        public static void UpdateSettings(DbSettings dbobj)
        {
            try
            {
                DataSet set = new DataSet();
                if (!File.Exists(fileName))
                {
                    DataTable dt = CreateDataTable();
                    DataRow row = NewDataRow(dt, dbobj);
                    dt.Rows.Add(row);
                    set.Tables.Add(dt);
                }
                else
                {
                    set.ReadXml(fileName);
                    if (set.Tables.Count > 0)
                    {
                        DataRow[] rowArray = set.Tables[0].Select("DbType='" + dbobj.DbType + "' and Server='" + dbobj.Server + "' and DbName='" + dbobj.DbName + "'");
                        if (rowArray.Length > 0)
                        {
                            AddColForTable(set.Tables[0]);
                            DataRow row2 = rowArray[0];
                            row2["DbType"] = dbobj.DbType;
                            row2["Server"] = dbobj.Server;
                            row2["ConnectStr"] = dbobj.ConnectStr;
                            row2["DbName"] = dbobj.DbName;
                            row2["ConnectSimple"] = dbobj.ConnectSimple;
                            row2["TabLoadtype"] = dbobj.TabLoadtype;
                            row2["TabLoadKeyword"] = dbobj.TabLoadKeyword;
                            row2["ProcPrefix"] = dbobj.ProcPrefix;
                            row2["ProjectName"] = dbobj.ProjectName;
                            row2["Namepace"] = dbobj.Namepace;
                            row2["Folder"] = dbobj.Folder;
                            row2["AppFrame"] = dbobj.AppFrame;
                            row2["DALType"] = dbobj.DALType;
                            row2["BLLType"] = dbobj.BLLType;
                            row2["WebType"] = dbobj.WebType;
                            row2["EditFont"] = dbobj.EditFont;
                            row2["EditFontSize"] = dbobj.EditFontSize;
                            row2["DbHelperName"] = dbobj.DbHelperName;
                            row2["ModelPrefix"] = dbobj.ModelPrefix;
                            row2["ModelSuffix"] = dbobj.ModelSuffix;
                            row2["BLLPrefix"] = dbobj.BLLPrefix;
                            row2["BLLSuffix"] = dbobj.BLLSuffix;
                            row2["DALPrefix"] = dbobj.DALPrefix;
                            row2["DALSuffix"] = dbobj.DALSuffix;
                            row2["TabNameRule"] = dbobj.TabNameRule;
                            row2["WebTemplatePath"] = dbobj.WebTemplatePath;
                            row2["ReplacedOldStr"] = dbobj.ReplacedOldStr;
                            row2["ReplacedNewStr"] = dbobj.ReplacedNewStr;
                        }
                        else
                        {
                            DataRow row = NewDataRow(set.Tables[0], dbobj);
                            set.Tables[0].Rows.Add(row);
                        }
                    }
                    else
                    {
                        DataTable dt = CreateDataTable();
                        DataRow row = NewDataRow(dt, dbobj);
                        dt.Rows.Add(row);
                        set.Tables.Add(dt);
                    }
                }
                set.WriteXml(fileName);
            }
            catch
            {
                throw new Exception("保存配置信息失败！");
            }
        }
    }
    public interface IBuilderBLL
    {
        // Methods
        string CreatBLLADD();
        string CreatBLLDelete();
        string CreatBLLExists();
        string CreatBLLGetList();
        string CreatBLLGetMaxID();
        string CreatBLLGetModel();
        string CreatBLLUpdate();
        string GetBLLCode(bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool GetModelByCache, bool List);

        // Properties
        List<ColumnInfo> Fieldlist { get; set; }
        List<ColumnInfo> Keys { get; set; }
        string NameSpace { get; set; }
        string Folder { get; set; }
        string Modelpath { get; set; }
        string ModelName { get; set; }
        string TableDescription { get; set; }
        string BLLpath { get; set; }
        string BLLName { get; set; }
        string Factorypath { get; set; }
        string IDALpath { get; set; }
        string IClass { get; set; }
        string DALpath { get; set; }
        string DALName { get; set; }
        bool IsHasIdentity { get; set; }
        string DbType { get; set; }
    }



    public interface IBuilderDALMTran
    {
        // Methods
        string GetDALCode();

        // Properties
        IDbObject DbObject { get; set; }
        string DbName { get; set; }
        List<ModelTran> ModelTranList { get; set; }
        string NameSpace { get; set; }
        string Folder { get; set; }
        string Modelpath { get; set; }
        string DALpath { get; set; }
        string IDALpath { get; set; }
        string IClass { get; set; }
        string DbHelperName { get; set; }
    }

    public class ModelTran
    {
        // Fields
        private string dbName;
        private string tableName;
        private string modelName;
        private string action;
        private List<ColumnInfo> _fieldlist;
        private List<ColumnInfo> _keys;

        // Properties
        public string DbName
        {
            get =>
                this.dbName;
            set =>
                this.dbName = value;
        }

        public string TableName
        {
            get =>
                this.tableName;
            set =>
                this.tableName = value;
        }

        public string ModelName
        {
            get =>
                this.modelName;
            set =>
                this.modelName = value;
        }

        public string Action
        {
            get =>
                this.action;
            set =>
                this.action = value;
        }

        public List<ColumnInfo> Fieldlist
        {
            get =>
                this._fieldlist;
            set =>
                this._fieldlist = value;
        }

        public List<ColumnInfo> Keys
        {
            get =>
                this._keys;
            set =>
                this._keys = value;
        }
    }









    public class BuilderFactory
    {
        // Fields
        private static Cache cache = new Cache();

        // Methods
        public static IBuilderBLL CreateBLLObj(string AssemblyGuid)
        {
            try
            {
                if (AssemblyGuid == "")
                {
                    return null;
                }
                AddIn @in = new AddIn(AssemblyGuid);
                string assembly = @in.Assembly;
                string classname = @in.Classname;
                return (IBuilderBLL)CreateObject(assembly, classname);
            }
            catch (SystemException exception)
            {
                string message = exception.Message;
                return null;
            }
        }

        public static IBuilderDALMTran CreateDALMTranObj(string AssemblyGuid)
        {
            try
            {
                if (AssemblyGuid == "")
                {
                    return null;
                }
                AddIn @in = new AddIn(AssemblyGuid);
                string assembly = @in.Assembly;
                string classname = @in.Classname;
                return (IBuilderDALMTran)CreateObject(assembly, classname);
            }
            catch (SystemException exception)
            {
                string message = exception.Message;
                return null;
            }
        }

        public static IBuilderDAL CreateDALObj(string AssemblyGuid)
        {
            try
            {
                if (AssemblyGuid == "")
                {
                    return null;
                }
                AddIn @in = new AddIn(AssemblyGuid);
                string assembly = @in.Assembly;
                string classname = @in.Classname;
                return (IBuilderDAL)CreateObject(assembly, classname);
            }
            catch (SystemException exception)
            {
                string message = exception.Message;
                return null;
            }
        }

        public static IBuilderDALTran CreateDALTranObj(string AssemblyGuid)
        {
            try
            {
                if (AssemblyGuid == "")
                {
                    return null;
                }
                AddIn @in = new AddIn(AssemblyGuid);
                string assembly = @in.Assembly;
                string classname = @in.Classname;
                return (IBuilderDALTran)CreateObject(assembly, classname);
            }
            catch (SystemException exception)
            {
                string message = exception.Message;
                return null;
            }
        }

        public static IBuilderIDAL CreateIDALObj()
        {
            try
            {
                return (IBuilderIDAL)CreateObject("Maticsoft.BuilderIDAL", "Maticsoft.BuilderIDAL.BuilderIDAL");
            }
            catch (SystemException exception)
            {
                string message = exception.Message;
                return null;
            }
        }

        public static IBuilderModel CreateModelObj(string AssemblyGuid)
        {
            try
            {
                if (AssemblyGuid == "")
                {
                    return null;
                }
                AddIn @in = new AddIn(AssemblyGuid);
                string assembly = @in.Assembly;
                string classname = @in.Classname;
                return (IBuilderModel)CreateObject(assembly, classname);
            }
            catch (SystemException exception)
            {
                string message = exception.Message;
                return null;
            }
        }

        private static object CreateObject(string path, string TypeName)
        {
            object obj2 = cache.GetObject(TypeName);
            if (obj2 == null)
            {
                try
                {
                    obj2 = Assembly.Load(path).CreateInstance(TypeName, true);
                    cache.SaveCache(TypeName, obj2);
                }
                catch (Exception exception)
                {
                    string message = exception.Message;
                }
            }
            return obj2;
        }

        public static IBuilderWeb CreateWebObj(string AssemblyGuid)
        {
            try
            {
                if (AssemblyGuid == "")
                {
                    return null;
                }
                AddIn @in = new AddIn(AssemblyGuid);
                string assembly = @in.Assembly;
                string classname = @in.Classname;
                return (IBuilderWeb)CreateObject(assembly, classname);
            }
            catch (SystemException exception)
            {
                string message = exception.Message;
                return null;
            }
        }
    }


    public interface IBuilderDALTran
    {
        // Methods
        string CreatAdd();
        string CreatDelete();
        string CreatExists();
        string CreatGetList();
        string CreatGetMaxID();
        string CreatGetModel();
        string CreatUpdate();
        string GetDALCode(bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool List);

        // Properties
        IDbObject DbObject { get; set; }
        string DbName { get; set; }
        string TableNameParent { get; set; }
        string TableNameSon { get; set; }
        List<ColumnInfo> FieldlistParent { get; set; }
        List<ColumnInfo> FieldlistSon { get; set; }
        List<ColumnInfo> KeysParent { get; set; }
        List<ColumnInfo> KeysSon { get; set; }
        string NameSpace { get; set; }
        string Folder { get; set; }
        string Modelpath { get; set; }
        string ModelNameParent { get; set; }
        string ModelNameSon { get; set; }
        string DALpath { get; set; }
        string DALNameParent { get; set; }
        string DALNameSon { get; set; }
        string IDALpath { get; set; }
        string IClass { get; set; }
        string DbHelperName { get; set; }
        string ProcPrefix { get; set; }
    }

    public interface IBuilderModel
    {
        // Methods
        string CreatModel();
        string CreatModelMethod();

        // Properties
        string ModelName { get; set; }
        string NameSpace { get; set; }
        string Modelpath { get; set; }
        List<ColumnInfo> Fieldlist { get; set; }
    }


    public class AddIn
    {
        // Fields
        private string fileAddin;
        private Cache cache;
        private string _guid;
        private string _name;
        private string _desc;
        private string _assembly;
        private string _classname;
        private string _version;

        // Methods
        public AddIn()
        {
            this.fileAddin = @"\CodeDAL.addin";
            this.cache = new Cache();
        }

        public AddIn(string AssemblyGuid)
        {
            this.fileAddin = @"\CodeDAL.addin";
            this.cache = new Cache();
            if (this.cache.GetObject(AssemblyGuid) == null)
            {
                try
                {
                    object addIn = this.GetAddIn(AssemblyGuid);
                    if (addIn != null)
                    {
                        this.cache.SaveCache(AssemblyGuid, addIn);
                        DataRow row = (DataRow)addIn;
                        this._guid = row["Guid"].ToString();
                        this._name = row["Name"].ToString();
                        this._desc = row["Decription"].ToString();
                        this._assembly = row["Assembly"].ToString();
                        this._classname = row["Classname"].ToString();
                        this._version = row["Version"].ToString();
                    }
                }
                catch (Exception exception)
                {
                    string message = exception.Message;
                }
            }
        }

        public void AddAddIn()
        {
            DataSet set = new DataSet();
            if (File.Exists(this.fileAddin))
            {
                set.ReadXml(this.fileAddin);
                if (set.Tables.Count > 0)
                {
                    DataRow row = set.Tables[0].NewRow();
                    row["Guid"] = this._guid;
                    row["Name"] = this._name;
                    row["Decription"] = this._desc;
                    row["Assembly"] = this._assembly;
                    row["Classname"] = this._classname;
                    row["Version"] = this._version;
                    set.Tables[0].Rows.Add(row);
                    XmlTextWriter writer = new XmlTextWriter(this.fileAddin, Encoding.Default);
                    writer.WriteStartDocument();
                    set.WriteXml(writer);
                    writer.Close();
                }
            }
        }

        public void DeleteAddIn(string AssemblyGuid)
        {
            DataSet addInList = this.GetAddInList();
            if (addInList.Tables.Count > 0)
            {
                addInList.Tables[0].Select("Guid='" + AssemblyGuid + "'")[0].Delete();
                addInList.WriteXml(this.fileAddin);
            }
        }

        private DataRow GetAddIn(string AssemblyGuid)
        {
            DataSet addInList = this.GetAddInList();
            if (addInList.Tables.Count > 0)
            {
                DataRow[] rowArray = addInList.Tables[0].Select("Guid='" + AssemblyGuid + "'");
                if (rowArray.Length > 0)
                {
                    return rowArray[0];
                }
            }
            return null;
        }

        public DataRow GetAddInByCache(string AssemblyGuid)
        {
            object addIn = this.cache.GetObject(AssemblyGuid);
            if (addIn == null)
            {
                try
                {
                    addIn = this.GetAddIn(AssemblyGuid);
                    this.cache.SaveCache(AssemblyGuid, addIn);
                }
                catch (Exception exception)
                {
                    string message = exception.Message;
                }
            }
            return (DataRow)addIn;
        }

        public DataSet GetAddInList()
        {
            try
            {
                DataSet set = new DataSet();
                if (File.Exists(this.fileAddin))
                {
                    set.ReadXml(this.fileAddin);
                    if (set.Tables.Count > 0)
                    {
                        return set;
                    }
                }
                return null;
            }
            catch (SystemException exception)
            {
                string message = exception.Message;
                return null;
            }
        }

        //public DataSet GetAddInList(string InterfaceName)
        //{
        //    try
        //    {
        //        DataSet set = new DataSet();
        //        if (File.Exists(this.fileAddin))
        //        {
        //            set.ReadXml(this.fileAddin);
        //            if (set.Tables.Count > 0)
        //            {
        //                List<DataRow> list = new List<DataRow>();
        //                foreach (DataRow row in set.Tables[0].Rows)
        //                {
        //                    string assemblyString = row["Assembly"].ToString();
        //                    bool flag = false;
        //                    try
        //                    {
        //                        foreach (Type type in Assembly.Load(assemblyString).GetTypes())
        //                        {
        //                            foreach (Type type2 in type.GetInterfaces())
        //                            {
        //                                if (type2.FullName == InterfaceName)
        //                                {
        //                                    flag = true;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    catch
        //                    {
        //                    }
        //                    if (!flag)
        //                    {
        //                        list.Add(row);
        //                    }
        //                }
        //                foreach (DataRow row2 in list)
        //                {
        //                    set.Tables[0].Rows.Remove(row2);
        //                }
        //                return set;
        //            }
        //        }
        //        return null;
        //    }
        //    catch (SystemException exception)
        //    {
        //        string message = exception.Message;
        //        return null;
        //    }
        //}

        public string LoadFile()
        {
            StreamReader reader = new StreamReader(this.fileAddin, Encoding.Default);
            string str = reader.ReadToEnd();
            reader.Close();
            return str;
        }

        // Properties
        public string Guid
        {
            get =>
                this._guid;
            set =>
                this._guid = value;
        }

        public string Name
        {
            get =>
                this._name;
            set =>
                this._name = value;
        }

        public string Decription
        {
            get =>
                this._desc;
            set =>
               this._desc = value;
        }

        public string Assembly
        {
            get =>
                this._assembly;
            set =>
                this._assembly = value;
        }

        public string Classname
        {
            get =>
                this._classname;
            set =>
                this._classname = value;
        }

        public string Version
        {
            get =>
                this._version;
            set =>
                this._version = value;
        }
    }





    public interface IBuilderDAL
    {
        // Methods
        string CreatAdd();
        string CreatDelete();
        string CreatExists();
        string CreatGetList();
        string CreatGetMaxID();
        string CreatGetModel();
        string CreatUpdate();
        string GetDALCode(bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool List);

        // Properties
        IDbObject DbObject { get; set; }
        string DbName { get; set; }
        string TableName { get; set; }
        List<ColumnInfo> Fieldlist { get; set; }
        List<ColumnInfo> Keys { get; set; }
        string NameSpace { get; set; }
        string Folder { get; set; }
        string Modelpath { get; set; }
        string ModelName { get; set; }
        string DALpath { get; set; }
        string DALName { get; set; }
        string IDALpath { get; set; }
        string IClass { get; set; }
        string DbHelperName { get; set; }
        string ProcPrefix { get; set; }
    }


    public interface IBuilderIDAL
    {
        // Methods
        string CreatAdd();
        string CreatDelete();
        string CreatExists();
        string CreatGetList();
        string CreatGetMaxID();
        string CreatGetModel();
        string CreatUpdate();
        string GetIDALCode(bool Maxid, bool Exists, bool Add, bool Update, bool Delete, bool GetModel, bool List);

        // Properties
        string DbType { get; set; }
        string TableDescription { get; set; }
        List<ColumnInfo> Fieldlist { get; set; }
        List<ColumnInfo> Keys { get; set; }
        string NameSpace { get; set; }
        string Folder { get; set; }
        string Modelpath { get; set; }
        string ModelName { get; set; }
        string IDALpath { get; set; }
        string IClass { get; set; }
        bool IsHasIdentity { get; set; }
    }












}

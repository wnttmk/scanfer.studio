using System;
using System.Collections.Generic;
using System.Text;

namespace CGT4
{
    /// <summary>
    /// 扩展表格模板，多表格支持
    /// </summary>
    public class MultipleTableTextTemplatingEngineHost : ScanferTextTemplatingEngineHost
    {

        /// <summary>
        /// 命名空间
        /// </summary>
        public string NameSpace { set; get; }

        /// <summary>
        /// 表格前缀，生成时会去掉
        /// </summary>
        public string TablePrefix { set; get; }

        /// <summary>
        /// 生成类前缀
        /// </summary>
        public string ClassPrefix { set; get; }
        /// <summary>
        /// 保存文件夹路径，即在设定好的模板上面再套一级目录
        /// </summary>
        public string SaveFolder { set; get; }
        /// <summary>
        /// 开发员
        /// </summary>
        public string Developers { set; get; }
        /// <summary>
        /// 要处理的表
        /// </summary>
        public TableInfo[] Tables { set; get; }
    }

    public class TableInfo
    {

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { set; get; }

        /// <summary>
        /// 表格描述 
        /// </summary>
        public string Description { set; get; }
        /// <summary>
        /// 列
        /// </summary>
        public ColumnInfo[] Columns { set; get; }
    }

    [Serializable]
    public class ColumnInfo
    {

        private string _columnName;
        private string _typeName = "";
        private string _length = "";
        private string _precision = "";

        private bool _isIdentity;
        private bool _isprimaryKey;
        private bool _isForeignKey;
        private bool _nullable;
        private string _defaultVal = "";
        private string _description = "";


        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName
        {
            get =>
                this._columnName;
            set =>
                this._columnName = value;
        }
        /// <summary>
        /// 逻辑表中的数据类型
        /// </summary>
        public string TypeName
        {
            get =>
                this._typeName;
            set =>
                this._typeName = value;
        }
        /// <summary>
        /// 逻辑表中的字符长度
        /// </summary>
        public string Length
        {
            get =>
                this._length;
            set =>
                this._length = value;
        }
        /// <summary>
        /// 逻辑表中的精度
        /// </summary>
        public string Precision
        {
            get =>
                this._precision;
            set =>
                this._precision = value;
        }

        /// <summary>
        /// 是否自增
        /// </summary>

        public bool IsIdentity
        {
            get =>
                this._isIdentity;
            set =>
                this._isIdentity = value;
        }
        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool IsPrimaryKey
        {
            get =>
                this._isprimaryKey;
            set =>
                this._isprimaryKey = value;
        }
        /// <summary>
        /// 是否是外键
        /// </summary>
        public bool IsForeignKey
        {
            get =>
                this._isForeignKey;
            set =>
                this._isForeignKey = value;
        }
        /// <summary>
        /// 是否可以为空
        /// </summary>
        public bool Nullable
        {
            get =>
                this._nullable;
            set =>
                this._nullable = value;
        }
        /// <summary>
        /// 默认值 
        /// </summary>
        public string DefaultVal
        {
            get =>
                this._defaultVal;
            set =>
                this._defaultVal = value;
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get =>
                this._description;
            set =>
                this._description = value;
        }
        /// <summary>
        /// 外键主表
        /// </summary>
        public string ForeignKeyTable
        {
            set; get;
        }
        /// <summary>
        /// 外键主表中的外键字段
        /// </summary>
        public string ForeignKeyTableColumn { set; get; }
        /// <summary>
        /// 与外键主表的关系 
        /// 0无，1一对1  2 一对多
        /// </summary>
        public int relationship { set; get; }
    }
}

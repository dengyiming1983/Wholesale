using System;
using System.Data;

namespace ServiceModel.Base
{
    public abstract class DBModel
    {
        public const string SQL_SERVER = "sql_server";
        public const string SYBASE = "sybase";

        protected string _type = "";
        public DBModel(string type)
        {
            _type = type;
        }

        public void setRecordType(string type)
        {
            _type = type;
        }

        public string getRecordType()
        {
            return _type;
        }

        /// <summary>
        /// Update convert Insert, or Insert convert Update
        /// </summary>
        /// <returns></returns>
        public abstract string CreateSaveSQL();
        public abstract string CreateInsertSQL();
        public abstract string CreateUpdateSQL();

        public abstract string CreateUpdateSQL(string[] updateColumn);
        public abstract string CreateDeleteSQL();

        public abstract void Parse(DataRow dataRow);

        public abstract string[] GetWhereClause();


    }
}
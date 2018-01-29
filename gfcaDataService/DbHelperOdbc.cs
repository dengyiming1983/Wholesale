using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.Odbc;
using System.Configuration;

namespace ServiceUitls.Database
{


    public abstract class DbHelperOdbc
    {
        public string connectionString = "Dsn=market;Uid=adm0;Pwd=systemcom";
        public  string OEM_SQL = "SET TEMPORARY OPTION CONNECTION_AUTHENTICATION='Company=MoleQ Inc.;Application=MoleQ Solutions;Signature=000fa55157edb8e14d818eb4fe3db41447146f1571g2a2ac0b2993b8eebd8393550f631933e5451b12b'";


        public  bool IsExist()
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (OdbcException ex)
                {
                    connection.Close();
                    return false;
                }
                finally
                {
                    connection.Close();
                }

            }

        }

        #region  执行简单SQL语句




        public  void MoleQOEMSetOption()
        {
            try
            {
                ExecuteSql("SET TEMPORARY OPTION CONNECTION_AUTHENTICATION='Company=MoleQ Inc.;Application=MoleQ Solutions;Signature=000fa55157edb8e14d818eb4fe3db41447146f1571g2a2ac0b2993b8eebd8393550f631933e5451b12b'");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public  void SetOEMOption(OdbcConnection connection)
        {
            string strSQL = "SET TEMPORARY OPTION CONNECTION_AUTHENTICATION='Company=MoleQ Inc.;Application=MoleQ Solutions;Signature=000fa55157edb8e14d818eb4fe3db41447146f1571g2a2ac0b2993b8eebd8393550f631933e5451b12b'";

            using (OdbcCommand cmd = new OdbcCommand(strSQL, connection))
            {
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                }
                catch (OdbcException ex)
                {
                    connection.Close();
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public  void TestConnection()
        {
            OdbcConnection conn = new OdbcConnection(connectionString);
            try
            {
                bool connected = false;
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    connected = true;
                    conn.Close();
                }

                if (!connected)
                    throw new Exception("Failed to connect ODBC database.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public  object ExecuteScalar(string SQLString)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                using (OdbcCommand cmd = new OdbcCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object rows = cmd.ExecuteScalar();
                        return rows;
                    }
                    catch (System.Data.Odbc.OdbcException ex)
                    {
                        connection.Close();
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }


        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public  int ExecuteSql(string SQLString)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                using (OdbcCommand cmd = new OdbcCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (OdbcException ex)
                    {
                        connection.Close();
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        public  int ExecuteSqlTran(ArrayList SQLStringList)
        {
            using (OdbcConnection conn = new OdbcConnection(connectionString))
            {
                conn.Open();
                OdbcCommand cmd = new OdbcCommand();
                cmd.Connection = conn;
                OdbcTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    int count = 0;
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            count += cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return count;
                }
                catch (System.Data.Odbc.OdbcException E)
                {
                    tx.Rollback();
                    throw new Exception(E.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public  int ExecuteSql(string SQLString, string content)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                OdbcCommand cmd = new OdbcCommand(SQLString, connection);
                System.Data.Odbc.OdbcParameter myParameter = new System.Data.Odbc.OdbcParameter("@content", OdbcType.VarChar);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.Odbc.OdbcException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public  int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                OdbcCommand cmd = new OdbcCommand(strSQL, connection);
                System.Data.Odbc.OdbcParameter myParameter = new System.Data.Odbc.OdbcParameter("@fs", OdbcType.Binary);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.Odbc.OdbcException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public  object GetSingle(string SQLString)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                using (OdbcCommand cmd = new OdbcCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.Odbc.OdbcException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回OdbcDataReader
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>OdbcDataReader</returns>
        public  OdbcDataReader ExecuteReader(string strSQL)
        {
            OdbcConnection connection = new OdbcConnection(connectionString);
            OdbcCommand cmd = new OdbcCommand(strSQL, connection);
            try
            {
                connection.Open();
                OdbcDataReader myReader = cmd.ExecuteReader();
                return myReader;
            }
            catch (System.Data.Odbc.OdbcException e)
            {
                throw new Exception(e.Message);
            }

        }
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public  DataSet Query(string SQLString)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    OdbcDataAdapter command = new OdbcDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.Odbc.OdbcException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                return ds;
            }
        }


        #endregion

        #region 执行带参数的SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public  int ExecuteSql(string SQLString, params OdbcParameter[] cmdParms)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                using (OdbcCommand cmd = new OdbcCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (System.Data.Odbc.OdbcException E)
                    {
                        throw new Exception(E.Message);
                    }
                }
            }
        }


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的OdbcParameter[]）</param>
        public  void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (OdbcConnection conn = new OdbcConnection(connectionString))
            {
                conn.Open();
                using (OdbcTransaction trans = conn.BeginTransaction())
                {
                    OdbcCommand cmd = new OdbcCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();

                            int idx = cmdText.IndexOf("-SQL");
                            if (idx >= 0)
                            {
                                cmdText = cmdText.Substring(idx + 4);
                            }

                            OdbcParameter[] cmdParms = (OdbcParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        trans.Dispose();
                        conn.Close();
                        throw;
                    }
                    finally
                    {
                        trans.Dispose();
                        conn.Close();
                    }
                }
            }
        }


        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public  object GetSingle(string SQLString, params OdbcParameter[] cmdParms)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                using (OdbcCommand cmd = new OdbcCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.Odbc.OdbcException e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回OdbcDataReader
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>OdbcDataReader</returns>
        public  OdbcDataReader ExecuteReader(string SQLString, params OdbcParameter[] cmdParms)
        {
            OdbcConnection connection = new OdbcConnection(connectionString);
            OdbcCommand cmd = new OdbcCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                OdbcDataReader myReader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (System.Data.Odbc.OdbcException e)
            {
                throw new Exception(e.Message);
            }

        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public  DataSet Query(string SQLString, params OdbcParameter[] cmdParms)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                OdbcCommand cmd = new OdbcCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (OdbcDataAdapter da = new OdbcDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.Odbc.OdbcException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }


        private static void PrepareCommand(OdbcCommand cmd, OdbcConnection conn, OdbcTransaction trans, string cmdText, OdbcParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (OdbcParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        #endregion

        #region 存储过程操作


        public  int RunProcedure(string storedProcName)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                int result;
                connection.Open();

                using (var cmd = new OdbcCommand(OEM_SQL, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                OdbcCommand command = new OdbcCommand(storedProcName, connection);
                command.CommandType = CommandType.StoredProcedure;
                result = command.ExecuteNonQuery();
                connection.Close();
                return result;
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OdbcDataReader</returns>
        public  OdbcDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            OdbcConnection connection = new OdbcConnection(connectionString);
            OdbcDataReader returnReader;
            connection.Open();
            OdbcCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            returnReader = command.ExecuteReader();
            return returnReader;
        }


        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public  DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                OdbcDataAdapter sqlDA = new OdbcDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }


        /// <summary>
        /// 构建 OdbcCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OdbcCommand</returns>
        private static OdbcCommand BuildQueryCommand(OdbcConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            OdbcCommand command = new OdbcCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (OdbcParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数		
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public  int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                int result;
                connection.Open();
                OdbcCommand command = BuildIntCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                result = (int)command.Parameters["ReturnValue"].Value;
                //Connection.Close();
                return result;
            }
        }

        /// <summary>
        /// 创建 OdbcCommand 对象实例(用来返回一个整数值)	
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OdbcCommand 对象实例</returns>
        private static OdbcCommand BuildIntCommand(OdbcConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            OdbcCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new OdbcParameter("ReturnValue",
                OdbcType.Int, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }
        #endregion


     
    }
}


using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Utility
{
    public class SQLServerHelper
    {
        public static string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        #region 注释
        //private static string GetConnString(string connName)
        //{
        //    string rtn = ConfigurationManager.ConnectionStrings[connName] != null ? ConfigurationManager.ConnectionStrings[connName].ConnectionString : string.Empty;
        //    return rtn;
        //}
        //public SQLHelper(string connStr)
        //{
        //    connString = connStr;
        //    conn = new SqlConnection(connString);
        //    //new SqlCommand().ExecuteNonQuery()
        //    //new SqlCommand().ExecuteReader();
        //    //new SqlCommand().ExecuteScalar()
        //    //new SqlCommand().ExecuteDataTable()
        //    //new SqlCommand().ExecuteDataSet()
        //}
        //public SQLHelper()
        //{
        //}
        #endregion

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, bool isProcedure, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;
            if (cmdParms != null && cmdParms.Length > 0)
            {
                cmd.Parameters.AddRange(cmdParms);
            }
        }

        #region 构造函数
        //public SQLServerHelper(string _connString)
        //{
        //    this.connString = _connString;
        //}

        //public SQLServerHelper()
        //{

        //}

        #endregion

        #region ExecuteNonQuery
        public static int ExecuteNonQuery(string commandText, bool isProcedure, params SqlParameter[] paras)
        {
            SqlConnection con = new SqlConnection(connString); 
            SqlCommand cmd = new SqlCommand(commandText, con);
           
            try
            {
                PrepareCommand(cmd, con, null, isProcedure, commandText, paras);
                int count = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        public static int ExecuteNonQuery(string commandText, params SqlParameter[] paras)
        {
            return ExecuteNonQuery(commandText, false, paras);
        }

        public static int ExecuteNonQuery(SqlTransaction trans, string commandText, bool isProcedure, params SqlParameter[] paras)
        {
            SqlConnection con = trans.Connection;
            SqlCommand cmd = new SqlCommand(commandText, con);
            int rtn = 0;
            
            try
            {
                PrepareCommand(cmd, con, trans, isProcedure, commandText, paras);
                rtn = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (trans == null)
                {
                    con.Close();
                }
            }
            return rtn;
        }

        public static int ExecuteNonQuery(SqlTransaction trans, string commandText, params SqlParameter[] paras)
        {
            return ExecuteNonQuery(trans, commandText, false, paras);
        }

        #endregion

        #region ExecuteQueryScalar

        /// <summary>
        /// 执行查询，并返回查询结果集中的第一行第一列，忽略其它行或列
        /// </summary>
        /// <param name="commandText">SQL语句或存储过程名</param>
        /// <param name="isProcedure">第一个参数是否为存储过程名,true为是,false为否</param>
        /// <param name="paras">SqlParameter参数列表，0个或多个参数</param>
        /// <returns></returns>
        public static object ExecuteQueryScalar(string commandText, bool isProcedure, params SqlParameter[] paras)
        {
            SqlConnection con = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(commandText, con);
           
            try
            {
                PrepareCommand(cmd, con, null, isProcedure, commandText, paras);
                object obj = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
                cmd.Dispose();
            }
        }

        /// <summary>
        /// 执行查询，并返回查询结果集中的第一行第一列，忽略其它行或列
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="paras">SqlParameter参数列表，0个或多个参数</param>
        /// <returns></returns>
        public static object ExecuteQueryScalar(string commandText, params SqlParameter[] paras)
        {
            return ExecuteQueryScalar(commandText, false, paras);
        }

        /// <summary>
        /// 执行查询，并返回查询结果集中的第一行第一列，忽略其它行或列
        /// </summary>
        /// <param name="trans">传递事务对象</param>
        /// <param name="commandText">SQL语句或存储过程名</param>
        /// <param name="isProcedure">第二个参数是否为存储过程名,true为是,false为否</param>
        /// <param name="paras">SqlParameter参数列表，0个或多个参数</param>
        /// <returns></returns>
        public static object ExecuteQueryScalar(SqlTransaction trans, string commandText, bool isProcedure, params SqlParameter[] paras)
        {
            SqlConnection con = trans.Connection;
            SqlCommand cmd = new SqlCommand(commandText, con);

            try
            {
                PrepareCommand(cmd, con, trans, isProcedure, commandText, paras);
                object obj = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (trans == null)
                {
                    con.Close();
                }
            }
        }

        /// <summary>
        /// 执行查询，并返回查询结果集中的第一行第一列，忽略其它行或列
        /// </summary>
        /// <param name="trans">传递事务对象</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="paras">SqlParameter参数列表，0个或多个参数</param>
        /// <returns></returns>
        public static object ExecuteQueryScalar(SqlTransaction trans, string commandText, params SqlParameter[] paras)
        {
            return ExecuteQueryScalar(trans, commandText, false, paras);
        }

        #endregion

        #region ExecuteDataReader

        /// <summary>
        /// 执行SQL，并返回结果集的只前进数据读取器
        /// </summary>
        /// <param name="commandText">SQL语句或存储过程名</param>
        /// <param name="isProcedure">第一个参数是否为存储过程名,true为是,false为否</param>
        /// <param name="paras">SqlParameter参数列表，0个或多个参数</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteDataReader(string commandText, bool isProcedure, params SqlParameter[] paras)
        {
            SqlConnection con = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(commandText, con);
            try
            {
                PrepareCommand(cmd, con, null, isProcedure, commandText, paras);
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return reader;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 执行SQL，并返回结果集的只前进数据读取器
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="paras">SqlParameter参数列表，0个或多个参数</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteDataReader(string commandText, params SqlParameter[] paras)
        {
            return ExecuteDataReader(commandText, false, paras);
        }

        /// <summary>
        /// 执行SQL，并返回结果集的只前进数据读取器
        /// </summary>
        /// <param name="trans">传递事务对象</param>
        /// <param name="commandText">SQL语句或存储过程名</param>
        /// <param name="isProcedure">第二个参数是否为存储过程名,true为是,false为否</param>
        /// <param name="paras">SqlParameter参数列表，0个或多个参数</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteDataReader(SqlTransaction trans, string commandText, bool isProcedure, params SqlParameter[] paras)
        {
            SqlConnection con = trans.Connection;
            SqlCommand cmd = new SqlCommand(commandText, con);
            try
            {
                PrepareCommand(cmd, con, trans, isProcedure, commandText, paras);
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return reader;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //finally
            //{
            //    if (trans == null)
            //    {
            //        con.Close();
            //    }
            //    cmd.Dispose();
            //}
        }

        /// <summary>
        /// 执行SQL，并返回结果集的只前进数据读取器
        /// </summary>
        /// <param name="trans">传递事务对象</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="paras">SqlParameter参数列表，0个或多个参数</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteDataReader(SqlTransaction trans, string commandText, params SqlParameter[] paras)
        {
            return ExecuteDataReader(trans, commandText, false, paras);
        }

        #endregion

        #region ExecuteDataSet

        /// <summary>
        /// 批量更新数据
        /// </summary>
        /// <param name="insertCommand">insertCommand</param>
        /// <param name="deleteCommand">deleteCommand</param>
        /// <param name="updateCommand">updateCommand</param>
        /// <param name="dt">DataTable</param>
        public static bool BatchUpdate(SqlCommand insertCommand, SqlCommand deleteCommand, SqlCommand updateCommand, DataTable dt)
        {
            //if (insertCommand == null) throw new ArgumentNullException("insertCommand");
            //if (deleteCommand == null) throw new ArgumentNullException("deleteCommand");
            //if (updateCommand == null) throw new ArgumentNullException("updateCommand");

            //// Create a SqlDataAdapter, and dispose of it after we are done
            //using (SqlDataAdapter ada = new SqlDataAdapter())
            //{
            //    // Set the data adapter commands
            //    ada.UpdateCommand = updateCommand;
            //    ada.InsertCommand = insertCommand;
            //    ada.DeleteCommand = deleteCommand;

            //    // Update the dataset changes in the data source
            //    ada.Update(dt);
            //    // Commit all the changes made to the DataSet
            //    dt.AcceptChanges();
            //}
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand myCmd = new SqlCommand("select * from " + dt.TableName, conn);
            SqlDataAdapter myAdapter = new SqlDataAdapter(myCmd);
            SqlCommandBuilder myCommandBuilder = new SqlCommandBuilder(myAdapter);

            myAdapter.InsertCommand = myCommandBuilder.GetInsertCommand();
            myAdapter.UpdateCommand = myCommandBuilder.GetUpdateCommand();
            myAdapter.DeleteCommand = myCommandBuilder.GetDeleteCommand();

            conn.Open();
            int count = myAdapter.Update(dt);
            conn.Close();
            return count > 0;

        }

        /// <summary>
        /// 执行SQL，并返回DataSet结果集
        /// </summary>
        /// <param name="commandText">SQL语句或存储过程名</param>
        /// <param name="isProcedure">第一个参数是否为存储过程名,true为是,false为否</param>
        /// <param name="paras">SqlParameter参数列表，0个或多个参数</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string commandText, bool isProcedure, params SqlParameter[] paras)
        {
            SqlConnection con = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(commandText, con);

            try
            {
                PrepareCommand(cmd, con, null, isProcedure, commandText, paras);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable ds = new DataTable();
                adapter.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
                
            }
        }

        /// <summary>
        /// 执行SQL，并返回DataSet结果集
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="paras">SqlParameter参数列表，0个或多个参数</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string commandText, params SqlParameter[] paras)
        {
            return ExecuteDataTable(commandText, false, paras);
        }

        /// <summary>
        /// 执行SQL，并返回DataSet结果集
        /// </summary>
        /// <param name="trans">传递事务对象</param>
        /// <param name="commandText">SQL语句或存储过程名</param>
        /// <param name="isProcedure">第二个参数是否为存储过程名,true为是,false为否</param>
        /// <param name="paras">SqlParameter参数列表，0个或多个参数</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(SqlTransaction trans, string commandText, bool isProcedure, params SqlParameter[] paras)
        {
            SqlConnection con = trans.Connection;
            SqlCommand cmd = new SqlCommand(commandText, con);

            
            try
            {
                PrepareCommand(cmd, con, trans, isProcedure, commandText, paras);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable ds = new DataTable();
                adapter.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (trans == null)
                {
                    con.Close();
                }
                
            }
        }

        /// <summary>
        /// 执行SQL，并返回DataSet结果集
        /// </summary>
        /// <param name="trans">传递事务对象</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="paras">SqlParameter参数列表，0个或多个参数</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(SqlTransaction trans, string commandText, params SqlParameter[] paras)
        {
            return ExecuteDataTable(trans, commandText, false, paras);
        }

        #endregion
    }


}

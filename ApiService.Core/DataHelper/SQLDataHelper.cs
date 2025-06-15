using ApiService.Core.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ApiService.Core.DataHelper
{
    public class SQLDataHelper1
    {
        public static List<T> ExecuteReaderToList<T>(string connection, string commandText, SqlParameter[] sqlparam, out string outValue, string outParamName = "@poReturnCode|@poReturnMessage")
        {
            var retval = new List<T>();
            var con = new SqlConnection(connection);
            var com = new SqlCommand
            {
                Connection = con,
                CommandType = CommandType.StoredProcedure,
                CommandText = commandText,
                CommandTimeout = 200
            };
            outValue = "";
            try
            {
                con.Open();
                if (sqlparam != null)
                {
                    com.Parameters.AddRange(sqlparam);
                }
                var dr = com.ExecuteReader(CommandBehavior.CloseConnection);
                retval = GetList<T>(dr);
                var c = outParamName.Split('|');
                foreach (var s in c)
                {
                    try
                    {
                        outValue = outValue + com.Parameters[s].Value + "|";
                    }
                    catch (Exception ex)
                    {
                        outValue = outValue + "|";
                    }
                }
                outValue = outValue.EndsWith("|") ? outValue.Substring(0, outValue.Length - 1) : outValue;
                return retval;
            }
            catch (DataException e)
            {
                Logger.Error(e);
                return null;
            }
            finally
            {
                con.Close();
                con.Dispose();
                com.Dispose();
            }
        }

        public static DataTable GetDataTable(string connection, string commandText, SqlParameter[] sqlparam, out string outValue, string outParamName = "@poReturnCode|@poReturnMessage")
        {
            var retval = new DataTable();
            var con = new SqlConnection(connection);
            var com = new SqlCommand
            {
                Connection = con,
                CommandType = CommandType.StoredProcedure,
                CommandText = commandText
            };
            outValue = "";
            try
            {
                con.Open();
                if (sqlparam != null)
                {
                    com.Parameters.AddRange(sqlparam);
                }
                var myDataAdapter = new SqlDataAdapter(com);
                var myDataSet = new DataSet();
                myDataAdapter.Fill(myDataSet);
                retval = myDataSet.Tables[0];
                var c = outParamName.Split('|');
                foreach (var s in c)
                {
                    try
                    {
                        outValue = outValue + com.Parameters[s].Value + "|";
                    }
                    catch (Exception ex)
                    {
                        outValue = outValue + "|";
                    }
                }
                outValue = outValue.EndsWith("|") ? outValue.Substring(0, outValue.Length - 1) : outValue;
                return retval;
            }
            catch (DataException e)
            {
                Logger.Error(e);
                return null;
            }
            finally
            {
                con.Close();
                con.Dispose();
                com.Dispose();
            }
        }

        public static List<DataTable> GetListDataTable(string connection, string commandText, SqlParameter[] sqlparam, out string outValue, string outParamName = "@poReturnCode|@poReturnMessage")
        {
            var lstTbl = new List<DataTable>();
            outValue = "";
            var con = new SqlConnection(connection);
            var com = new SqlCommand
            {
                Connection = con,
                CommandType = CommandType.StoredProcedure,
                CommandText = commandText
            };
            try
            {
                con.Open();
                if (sqlparam != null)
                {
                    com.Parameters.AddRange(sqlparam);
                }
                var myDataAdapter = new SqlDataAdapter(com);
                var myDataSet = new DataSet();
                myDataAdapter.Fill(myDataSet);
                for (int i = 0; i < myDataSet.Tables.Count; i++)
                {
                    var tb = new DataTable();
                    tb = myDataSet.Tables[i];
                    lstTbl.Add(tb);
                }
                var c = outParamName.Split('|');
                foreach (var s in c)
                {
                    try
                    {
                        outValue = outValue + com.Parameters[s].Value + "|";
                    }
                    catch (Exception ex)
                    {
                        outValue = outValue + "|";
                    }
                }
                outValue = outValue.EndsWith("|") ? outValue.Substring(0, outValue.Length - 1) : outValue;
                return lstTbl;
            }
            catch (DataException e)
            {
                Logger.Error(e);
                return null;
            }
            finally
            {
                con.Close();
                con.Dispose();
                com.Dispose();
            }
        }
        
        /// <summary>
        /// Trả về datareader với trường hợp CommandType=CommandType.Textr
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commndText"></param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(string connection, string commndText)
        {
            var con = new SqlConnection(connection);
            var com = new SqlCommand
            {
                CommandText = commndText,
                CommandType = CommandType.Text,
                Connection = con
            };
            try
            {
                con.Open();
                return com.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (DataException e)
            {
                Logger.Error(e);
                return null;
            }
            finally
            {
                con.Close();
                con.Dispose();
                com.Dispose();
            }
        }


        /// <summary>
        /// Chạy các function trả về bảng giá trị 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commndText"></param>
        /// <returns></returns>
        public static DataTable FunctionValueTable(string connection, string commndText)
        {
            var con = new SqlConnection(connection);
            var com = new SqlCommand
            {
                CommandText = commndText,
                CommandType = CommandType.Text,
                Connection = con
            };
            try
            {
                con.Open();

                var dt = new DataTable();
                var da = new SqlDataAdapter(com);
                da.Fill(dt);
                return dt;

                //return com.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (DataException e)
            {
                Logger.Error(e);
                return null;
            }
            finally
            {
                con.Close();
                con.Dispose();
                com.Dispose();
            }
        }


        #region ExecuteNonQuery
        /// <summary>
        /// ExecuteNonQuery trường hợp sử dụng storeprocedure
        /// </summary>
        /// <param name="connection">Config.Connectstring</param>
        /// <param name="commandText"></param>
        /// <param name="sqlparam">nếu store không có tham số truyền vào null</param>
        /// <returns>số bản ghi ảnh hưởng</returns>
        public static int ExecuteNonQuery(string connection, string commandText, SqlParameter[] sqlparam, out string outValue, string outParamName = "@poReturnCode|@poReturnMessage")
        {
            var con = new SqlConnection(connection);
            var com = new SqlCommand
            {
                CommandText = commandText,
                CommandType = CommandType.StoredProcedure
            };
            outValue = "";
            try
            {
                using (con)
                {
                    com.Connection = con;
                    con.Open();
                    if (sqlparam != null)
                    {
                        com.Parameters.AddRange(sqlparam);
                    }
                    var rowCount = com.ExecuteNonQuery();
                    var c = outParamName.Split('|');
                    foreach (var s in c)
                    {
                        try
                        {
                            outValue = outValue + com.Parameters[s].Value + "|";
                        }
                        catch (Exception ex)
                        {
                            outValue = outValue + "|";
                        }
                    }
                    outValue = outValue.EndsWith("|") ? outValue.Substring(0, outValue.Length - 1) : outValue;
                    return rowCount;
                }
            }
            catch (DataException e)
            {
                Logger.Error(e);
                return -1;
            }
            finally
            {
                con.Close();
                con.Dispose();
                com.Dispose();
            }
        }

        /// <summary>
        /// Trường hợp sử dụng commandText
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connection, string commandText)
        {
            var con = new SqlConnection(connection);
            var com = new SqlCommand
            {
                CommandText = commandText,
                CommandType = CommandType.StoredProcedure
            };
            try
            {
                using (con)
                {
                    com.Connection = con;
                    con.Open();
                    return com.ExecuteNonQuery();
                }
            }
            catch (DataException e)
            {
                Logger.Error(e);
                return -1;
            }
            finally
            {
                con.Close();
                con.Dispose();
                com.Dispose();
            }
        }
        #endregion

        #region ExecuteScalar
        public static object ExecuteScalar(string connection, string commandText, SqlParameter[] sqlparam)
        {
            var con = new SqlConnection(connection);
            var com = new SqlCommand
            {
                CommandText = commandText,
                CommandType = CommandType.StoredProcedure
            };
            try
            {
                using (con)
                {
                    com.Connection = con;
                    con.Open();
                    if (sqlparam != null)
                    {
                        com.Parameters.AddRange(sqlparam);
                    }
                    return com.ExecuteScalar();
                }
            }
            catch (DataException e)
            {
                Logger.Error(e);
                return -1;
            }
            finally
            {
                con.Close();
                con.Dispose();
                com.Dispose();
            }
        }

        public static object ExecuteScalar(string connection, string commandText)
        {
            var con = new SqlConnection(connection);
            var com = new SqlCommand
            {
                CommandText = commandText,
                CommandType = CommandType.Text
            };
            try
            {
                using (con)
                {
                    com.Connection = con;
                    con.Open();
                    return com.ExecuteScalar();
                }
            }
            catch (DataException e)
            {
                Logger.Error(e);
                return -1;
            }
            finally
            {
                con.Close();
                con.Dispose();
                com.Dispose();
            }
        }

        #endregion

        public static T GetInfo<T>(IDataReader r)
        {
            var builder = DynamicBuilder<T>.CreateBuilder(r);
            var x = default(T);
            if (r != null)
            {
                if (r.Read())
                {
                    x = builder.Build(r);
                }
                r.Close();
                r.Dispose();
                return x;
            }
            return default(T);
        }

        public static List<T> GetList<T>(IDataReader r)
        {
            if (r != null)
            {
                var list = new List<T>();
                var builder = DynamicBuilder<T>.CreateBuilder(r);
                while (r.Read())
                {
                    var x = builder.Build(r);
                    list.Add(x);
                }
                r.Close();
                r.Dispose();
                return list;
            }
            return null;
        }
    }
}

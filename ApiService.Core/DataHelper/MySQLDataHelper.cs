using ApiService.Core.Log;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ApiService.Core.DataHelper
{
    public class MySQLDataHelper
    {
        public static List<T> ExecuteReaderToList<T>(string connection, string commandText, MySqlParameter[] sqlparam, out string outValue, string outParamName = "@poReturnCode|@poReturnMessage")
        {
            var retval = new List<T>();
            var con = new MySqlConnection(connection);
            var com = new MySqlCommand
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

        public static DataTable GetDataTable(string connection, string commandText, MySqlParameter[] sqlparam, out string outValue, string outParamName = "@poReturnCode|@poReturnMessage")
        {
            var retval = new DataTable();
            var con = new MySqlConnection(connection);
            var com = new MySqlCommand
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
                var myDataAdapter = new MySqlDataAdapter(com);
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

        public static List<DataTable> GetListDataTable(string connection, string commandText, MySqlParameter[] sqlparam, out string outValue, string outParamName = "@poReturnCode|@poReturnMessage")
        {
            var lstTbl = new List<DataTable>();
            outValue = "";
            var con = new MySqlConnection(connection);
            var com = new MySqlCommand
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
                var myDataAdapter = new MySqlDataAdapter(com);
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
        /// Trả về datareader với trường hợp CommandType=CommandType.Text
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commndText"></param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(string connection, string commndText)
        {
            var con = new MySqlConnection(connection);
            var com = new MySqlCommand
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
            var con = new MySqlConnection(connection);
            var com = new MySqlCommand
            {
                CommandText = commndText,
                CommandType = CommandType.Text,
                Connection = con
            };
            try
            {
                con.Open();

                var dt = new DataTable();
                var da = new MySqlDataAdapter(com);
                da.Fill(dt);
                return dt;
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
        public static int ExecuteNonQuery(string connection, string commandText, MySqlParameter[] sqlparam, out string outValue, string outParamName = "@poReturnCode|@poReturnMessage")
        {
            var con = new MySqlConnection(connection);
            var com = new MySqlCommand
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
                var result = com.ExecuteNonQuery();
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
                return result;
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
        /// ExecuteNonQuery trường hợp sử dụng câu lệnh SQL
        /// </summary>
        /// <param name="connection">Config.Connectstring</param>
        /// <param name="commandText"></param>
        /// <returns>số bản ghi ảnh hưởng</returns>
        public static int ExecuteNonQuery(string connection, string commandText)
        {
            var con = new MySqlConnection(connection);
            var com = new MySqlCommand
            {
                Connection = con,
                CommandType = CommandType.Text,
                CommandText = commandText
            };
            try
            {
                con.Open();
                return com.ExecuteNonQuery();
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
        /// <summary>
        /// ExecuteScalar trường hợp sử dụng storeprocedure
        /// </summary>
        /// <param name="connection">Config.Connectstring</param>
        /// <param name="commandText"></param>
        /// <param name="sqlparam">nếu store không có tham số truyền vào null</param>
        /// <returns>giá trị đầu tiên của bản ghi đầu tiên</returns>
        public static object ExecuteScalar(string connection, string commandText, MySqlParameter[] sqlparam)
        {
            var con = new MySqlConnection(connection);
            var com = new MySqlCommand
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
                return com.ExecuteScalar();
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
        /// ExecuteScalar trường hợp sử dụng câu lệnh SQL
        /// </summary>
        /// <param name="connection">Config.Connectstring</param>
        /// <param name="commandText"></param>
        /// <returns>giá trị đầu tiên của bản ghi đầu tiên</returns>
        public static object ExecuteScalar(string connection, string commandText)
        {
            var con = new MySqlConnection(connection);
            var com = new MySqlCommand
            {
                Connection = con,
                CommandType = CommandType.Text,
                CommandText = commandText
            };
            try
            {
                con.Open();
                return com.ExecuteScalar();
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
        #endregion

        public static T GetInfo<T>(IDataReader r)
        {
            T obj = Activator.CreateInstance<T>();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                try
                {
                    if (!Equals(r[property.Name], DBNull.Value))
                    {
                        property.SetValue(obj, r[property.Name], null);
                    }
                }
                catch { }
            }
            return obj;
        }

        public static List<T> GetList<T>(IDataReader r)
        {
            var lst = new List<T>();
            while (r.Read())
            {
                lst.Add(GetInfo<T>(r));
            }
            return lst;
        }
    }
}

using ApiService.Core;
using ApiService.Core.DataHelper;
using ApiService.Core.Log;
using ApiService.Entity;
using ApiService.Interface;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApiService.Implement
{
    public class UserExamAttemptImp : IUserExamAttempt
    {
        public async Task<(int NewAttemptId, int AttemptNumber)> InsertUserExamAttempt(int userId, int examId)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@p_User_Id", userId),
                    new MySqlParameter("@p_Exam_Id", examId)
                };
                
                string outVal = "";
                var dt = MySQLDataHelper.GetDataTable(ConfigurationHelper.connectString, "InsertUserExamAttempt", param, out outVal);
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    return (
                        Convert.ToInt32(dt.Rows[0]["NewAttemptId"]),
                        Convert.ToInt32(dt.Rows[0]["AttemptNumber"])
                    );
                }
                
                return (0, 0);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return (0, 0);
            }
        }

        public async Task<UserExamAttemptInfo> GetUserExamAttemptById(int id)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@p_Id", id)
                };
                
                string outVal = "";
                var attempts = MySQLDataHelper.ExecuteReaderToList<UserExamAttemptInfo>(
                    ConfigurationHelper.connectString,
                    "GetUserExamAttemptById",
                    param,
                    out outVal
                );
                
                if (attempts != null && attempts.Count > 0)
                {
                    return attempts.FirstOrDefault();
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        public async Task<IEnumerable<UserExamAttemptInfo>> GetUserExamAttemptsByUserIdAndExamId(int userId, int examId)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@p_User_Id", userId),
                    new MySqlParameter("@p_Exam_Id", examId)
                };
                
                string outVal = "";
                var attempts = MySQLDataHelper.ExecuteReaderToList<UserExamAttemptInfo>(
                    ConfigurationHelper.connectString,
                    "GetUserExamAttemptsByUserIdAndExamId",
                    param,
                    out outVal
                );
                
                if (attempts != null)
                {
                    return attempts;
                }
                
                return new List<UserExamAttemptInfo>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new List<UserExamAttemptInfo>();
            }
        }

        public async Task<bool> UpdateUserExamAttempt(int id, DateTime endTime, decimal totalScore, bool passed)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@p_Id", id),
                    new MySqlParameter("@p_End_Time", endTime),
                    new MySqlParameter("@p_Total_Score", totalScore),
                    new MySqlParameter("@p_Passed", passed)
                };
                
                string outVal = "";
                var dt = MySQLDataHelper.GetDataTable(
                    ConfigurationHelper.connectString,
                    "UpdateUserExamAttempt",
                    param,
                    out outVal
                );
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0]["RowsAffected"]) > 0;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        public async Task<bool> DeleteUserExamAttempt(int id)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@p_Id", id)
                };
                
                string outVal = "";
                var dt = MySQLDataHelper.GetDataTable(
                    ConfigurationHelper.connectString,
                    "DeleteUserExamAttempt",
                    param,
                    out outVal
                );
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0]["RowsAffected"]) > 0;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        public async Task<(decimal FinalScore, bool Passed)> ScoreUserExamAttempt(int userId, int examId, int attemptNumber)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@p_User_Id", userId),
                    new MySqlParameter("@p_Exam_Id", examId),
                    new MySqlParameter("@p_Attempt_Number", attemptNumber)
                };
                
                string outVal = "";
                var dt = MySQLDataHelper.GetDataTable(
                    ConfigurationHelper.connectString,
                    "ScoreUserExamAttempt",
                    param,
                    out outVal
                );
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    decimal finalScore = 0;
                    bool passed = false;

                    if (dt.Rows[0]["FinalScore"] != DBNull.Value)
                    {
                        finalScore = Convert.ToDecimal(dt.Rows[0]["FinalScore"]);
                    }

                    if (dt.Rows[0]["Passed"] != DBNull.Value)
                    {
                        passed = Convert.ToBoolean(dt.Rows[0]["Passed"]);
                    }

                    return (finalScore, passed);
                }
                
                return (0, false);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return (0, false);
            }
        }
    }
} 
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
    public class UserExamAnswerImp : IUserExamAnswer
    {
        public async Task<int> InsertUserExamAnswer(int userId, int examId, int questionId, int attemptNumber, string answerGivenJson, int timeSpentSeconds)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@p_User_Id", userId),
                    new MySqlParameter("@p_Exam_Id", examId),
                    new MySqlParameter("@p_Question_Id", questionId),
                    new MySqlParameter("@p_Attempt_Number", attemptNumber),
                    new MySqlParameter("@p_Answer_Given_Json", answerGivenJson),
                    new MySqlParameter("@p_Time_Spent_Seconds", timeSpentSeconds)
                };
                
                string outVal = "";
                var dt = MySQLDataHelper.GetDataTable(
                    ConfigurationHelper.connectString,
                    "InsertUserExamAnswer",
                    param,
                    out outVal
                );
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0]["NewAnswerId"]);
                }
                
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return 0;
            }
        }

        public async Task<IEnumerable<UserExamAnswerInfo>> GetUserExamAnswersByAttempt(int userId, int examId, int attemptNumber)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@p_User_Id", userId),
                    new MySqlParameter("@p_Exam_Id", examId),
                    new MySqlParameter("@p_Attempt_Number", attemptNumber)
                };
                
                string outVal = "";
                var answers = MySQLDataHelper.ExecuteReaderToList<UserExamAnswerInfo>(
                    ConfigurationHelper.connectString,
                    "GetUserExamAnswersByAttempt",
                    param,
                    out outVal
                );
                
                if (answers != null)
                {
                    return answers;
                }
                
                return new List<UserExamAnswerInfo>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new List<UserExamAnswerInfo>();
            }
        }

        public async Task<bool> UpdateUserExamAnswer(int id, string answerGivenJson, bool isCorrect, decimal scoreAchieved, int timeSpentSeconds)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@p_Id", id),
                    new MySqlParameter("@p_Answer_Given_Json", answerGivenJson),
                    new MySqlParameter("@p_Is_Correct", isCorrect),
                    new MySqlParameter("@p_Score_Achieved", scoreAchieved),
                    new MySqlParameter("@p_Time_Spent_Seconds", timeSpentSeconds)
                };
                
                string outVal = "";
                var dt = MySQLDataHelper.GetDataTable(
                    ConfigurationHelper.connectString,
                    "UpdateUserExamAnswer",
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

        public async Task<bool> DeleteUserExamAnswer(int id)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@p_Id", id)
                };
                
                string outVal = "";
                var dt = MySQLDataHelper.GetDataTable(
                    ConfigurationHelper.connectString,
                    "DeleteUserExamAnswer",
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
    }
} 
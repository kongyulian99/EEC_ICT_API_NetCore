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
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiService.Implement
{
    public class QuestionImp : IQuestion
    {
        public async Task<int> CreateQuestion(QuestionInfo question)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@iTopic_Id", question.Topic_Id),
                    new MySqlParameter("@iExam_Id", question.Exam_Id),
                    new MySqlParameter("@iQuestion_Type", (int)question.Question_Type),
                    new MySqlParameter("@iDifficulty_Level", (int)question.Difficulty_Level),
                    new MySqlParameter("@sContent", question.Content),
                    new MySqlParameter("@jQuestion_Data_Json", question.Question_Data_Json),
                    new MySqlParameter("@sExplanation", question.Explanation ?? (object)DBNull.Value),
                    new MySqlParameter("@dScore", question.Score)
                };
                
                string outVal = "";
                var dt = MySQLDataHelper.GetDataTable(ConfigurationHelper.connectString, "CreateQuestion", param, out outVal);
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0]["New_Question_Id"]);
                }
                
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return 0;
            }
        }

        public async Task<bool> UpdateQuestion(QuestionInfo question)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@iId", question.Id),
                    new MySqlParameter("@iTopic_Id", question.Topic_Id),
                    new MySqlParameter("@iExam_Id", question.Exam_Id),
                    new MySqlParameter("@iQuestion_Type", (int)question.Question_Type),
                    new MySqlParameter("@iDifficulty_Level", (int)question.Difficulty_Level),
                    new MySqlParameter("@sContent", question.Content),
                    new MySqlParameter("@jQuestion_Data_Json", question.Question_Data_Json),
                    new MySqlParameter("@sExplanation", question.Explanation ?? (object)DBNull.Value),
                    new MySqlParameter("@dScore", question.Score)
                };
                
                string outVal = "";
                var result = MySQLDataHelper.ExecuteNonQuery(ConfigurationHelper.connectString, "UpdateQuestion", param, out outVal);
                
                return result >= 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        public async Task<bool> DeleteQuestion(int id)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@iId", id)
                };
                
                string outVal = "";
                var result = MySQLDataHelper.ExecuteNonQuery(ConfigurationHelper.connectString, "DeleteQuestion", param, out outVal);
                
                return result >= 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        public async Task<QuestionInfo> GetQuestionById(int id)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@iId", id)
                };
                
                string outVal = "";
                var questions = MySQLDataHelper.ExecuteReaderToList<QuestionInfo>(ConfigurationHelper.connectString, "GetQuestionById", param, out outVal);
                
                if (questions != null && questions.Count > 0)
                {
                    return questions.FirstOrDefault();
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        public async Task<IEnumerable<QuestionInfo>> GetQuestionsByTopicId(int topicId)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@iTopic_Id", topicId)
                };
                
                string outVal = "";
                var questions = MySQLDataHelper.ExecuteReaderToList<QuestionInfo>(ConfigurationHelper.connectString, "GetQuestionsByTopicId", param, out outVal);
                
                if (questions != null)
                {
                    return questions;
                }
                
                return new List<QuestionInfo>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new List<QuestionInfo>();
            }
        }

        public async Task<IEnumerable<QuestionInfo>> GetQuestionsByExamId(int examId)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@iExam_Id", examId)
                };
                
                string outVal = "";
                var questions = MySQLDataHelper.ExecuteReaderToList<QuestionInfo>(ConfigurationHelper.connectString, "GetQuestionsByExamId", param, out outVal);
                
                if (questions != null)
                {
                    return questions;
                }
                
                return new List<QuestionInfo>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new List<QuestionInfo>();
            }
        }
        
        public async Task<IEnumerable<QuestionInfo>> GetQuestionsByDifficultyLevel(Enum_DifficutyLevel difficultyLevel)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@iDifficulty_Level", (int)difficultyLevel)
                };
                
                string outVal = "";
                var questions = MySQLDataHelper.ExecuteReaderToList<QuestionInfo>(ConfigurationHelper.connectString, "GetQuestionsByDifficultyLevel", param, out outVal);
                
                if (questions != null)
                {
                    return questions;
                }
                
                return new List<QuestionInfo>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new List<QuestionInfo>();
            }
        }

        public async Task<IEnumerable<QuestionInfo>> GetAllQuestions()
        {
            try
            {
                string outVal = "";
                var questions = MySQLDataHelper.ExecuteReaderToList<QuestionInfo>(ConfigurationHelper.connectString, "GetAllQuestions", null, out outVal);
                
                if (questions != null)
                {
                    return questions;
                }
                
                return new List<QuestionInfo>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new List<QuestionInfo>();
            }
        }
    }
} 
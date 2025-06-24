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
    public class ExamImp : IExam
    {
        public async Task<int> CreateExam(ExamInfo exam)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@sTitle", exam.Title),
                    new MySqlParameter("@sDescription", exam.Description ?? (object)DBNull.Value),
                    new MySqlParameter("@iDuration_Minutes", exam.Duration_Minutes),
                    new MySqlParameter("@iTotal_Questions", exam.Total_Questions),
                    new MySqlParameter("@dPass_Score", exam.Pass_Score),
                };
                
                string outVal = "";
                var dt = MySQLDataHelper.GetDataTable(ConfigurationHelper.connectString, "CreateExam", param, out outVal);
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0]["New_Exam_Id"]);
                }
                
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return 0;
            }
        }

        public async Task<bool> UpdateExam(ExamInfo exam)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@iId", exam.Id),
                    new MySqlParameter("@sTitle", exam.Title),
                    new MySqlParameter("@sDescription", exam.Description ?? (object)DBNull.Value),
                    new MySqlParameter("@iDuration_Minutes", exam.Duration_Minutes),
                    new MySqlParameter("@iTotal_Questions", exam.Total_Questions),
                    new MySqlParameter("@dPass_Score", exam.Pass_Score),
                };
                
                string outVal = "";
                var result = MySQLDataHelper.ExecuteNonQuery(ConfigurationHelper.connectString, "UpdateExam", param, out outVal);
                
                return result >= 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        public async Task<bool> DeleteExam(int id)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@iId", id)
                };
                
                string outVal = "";
                var result = MySQLDataHelper.ExecuteNonQuery(ConfigurationHelper.connectString, "DeleteExam", param, out outVal);
                
                return result >= 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        public async Task<ExamInfo> GetExamById(int id)
        {
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@iId", id)
                };
                
                string outVal = "";
                var exams = MySQLDataHelper.ExecuteReaderToList<ExamInfo>(ConfigurationHelper.connectString, "GetExamById", param, out outVal);
                
                if (exams != null && exams.Count > 0)
                {
                    return exams.FirstOrDefault();
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        public async Task<IEnumerable<ExamInfo>> GetAllExams()
        {
            try
            {
                string outVal = "";
                var exams = MySQLDataHelper.ExecuteReaderToList<ExamInfo>(ConfigurationHelper.connectString, "GetAllExams", null, out outVal);
                
                if (exams != null)
                {
                    return exams;
                }
                
                return new List<ExamInfo>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new List<ExamInfo>();
            }
        }
    }
} 
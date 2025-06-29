using ApiService.Core;
using ApiService.Core.DataHelper;
using ApiService.Entity;
using ApiService.Interface;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ApiService.Implement
{
    public class DashboardImp : IDashboard
    {
        public DbReturnInfo<DashboardSummary> GetSystemSummary()
        {
            var retval = new DbReturnInfo<DashboardSummary>();
            
            try
            {
                string outVal = "";
                var dt = MySQLDataHelper.GetDataTable(ConfigurationHelper.connectString, "GetSystemSummary", null, out outVal);
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    retval.Result = new DashboardSummary
                    {
                        TotalUsers = Convert.ToInt32(dt.Rows[0]["TotalUsers"]),
                        TotalExams = Convert.ToInt32(dt.Rows[0]["TotalExams"]),
                        TotalQuestions = Convert.ToInt32(dt.Rows[0]["TotalQuestions"]),
                        TotalAttempts = Convert.ToInt32(dt.Rows[0]["TotalAttempts"]),
                        OverallPassRate = dt.Rows[0]["OverallPassRate"] != DBNull.Value 
                            ? Convert.ToDecimal(dt.Rows[0]["OverallPassRate"]) 
                            : 0
                    };
                    retval.Code = 1;
                    retval.Message = "Lấy thông tin tổng quan thành công";
                }
                else
                {
                    retval.Code = 0;
                    retval.Message = "Không thể lấy thông tin tổng quan";
                }
            }
            catch (Exception ex)
            {
                retval.Code = -1;
                retval.Message = "Lỗi: " + ex.Message;
            }
            
            return retval;
        }

        public DbReturnInfo<List<ExamAttemptsOverTime>> GetExamAttemptsOverTime(DateTime? fromDate, DateTime? toDate)
        {
            var retval = new DbReturnInfo<List<ExamAttemptsOverTime>>();
            
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@dFromDate", fromDate.HasValue ? (object)fromDate.Value : DBNull.Value),
                    new MySqlParameter("@dToDate", toDate.HasValue ? (object)toDate.Value : DBNull.Value)
                };
                
                string outVal = "";
                var dt = MySQLDataHelper.GetDataTable(ConfigurationHelper.connectString, "GetExamAttemptsOverTime", param, out outVal);
                
                if (dt != null)
                {
                    var result = new List<ExamAttemptsOverTime>();
                    foreach (DataRow row in dt.Rows)
                    {
                        result.Add(new ExamAttemptsOverTime
                        {
                            Date = Convert.ToDateTime(row["Date"]),
                            TotalAttempts = Convert.ToInt32(row["TotalAttempts"]),
                            PassedAttempts = Convert.ToInt32(row["PassedAttempts"])
                        });
                    }
                    
                    retval.Result = result;
                    retval.Code = 1;
                    retval.Message = "Lấy dữ liệu thành công";
                }
                else
                {
                    retval.Code = 0;
                    retval.Message = "Không có dữ liệu";
                }
            }
            catch (Exception ex)
            {
                retval.Code = -1;
                retval.Message = "Lỗi: " + ex.Message;
            }
            
            return retval;
        }

        public DbReturnInfo<List<ScoreDistribution>> GetScoreDistribution(int? examId)
        {
            var retval = new DbReturnInfo<List<ScoreDistribution>>();
            
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@iExamId", examId.HasValue ? (object)examId.Value : DBNull.Value)
                };
                
                string outVal = "";
                var dt = MySQLDataHelper.GetDataTable(ConfigurationHelper.connectString, "GetScoreDistribution", param, out outVal);
                
                if (dt != null)
                {
                    var result = new List<ScoreDistribution>();
                    foreach (DataRow row in dt.Rows)
                    {
                        result.Add(new ScoreDistribution
                        {
                            ScoreRange = row["ScoreRange"].ToString(),
                            Count = Convert.ToInt32(row["Count"])
                        });
                    }
                    
                    retval.Result = result;
                    retval.Code = 1;
                    retval.Message = "Lấy phân bố điểm thành công";
                }
                else
                {
                    retval.Code = 0;
                    retval.Message = "Không có dữ liệu phân bố điểm";
                }
            }
            catch (Exception ex)
            {
                retval.Code = -1;
                retval.Message = "Lỗi: " + ex.Message;
            }
            
            return retval;
        }

        public DbReturnInfo<List<TopExamByPassRate>> GetTopExamsByPassRate(int limit = 10)
        {
            var retval = new DbReturnInfo<List<TopExamByPassRate>>();
            
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@iLimit", limit)
                };
                
                string outVal = "";
                var dt = MySQLDataHelper.GetDataTable(ConfigurationHelper.connectString, "GetTopExamsByPassRate", param, out outVal);
                
                if (dt != null)
                {
                    var result = new List<TopExamByPassRate>();
                    foreach (DataRow row in dt.Rows)
                    {
                        result.Add(new TopExamByPassRate
                        {
                            ExamId = Convert.ToInt32(row["ExamId"]),
                            ExamTitle = row["ExamTitle"].ToString(),
                            TotalAttempts = Convert.ToInt32(row["TotalAttempts"]),
                            PassedAttempts = Convert.ToInt32(row["PassedAttempts"]),
                            PassRate = row["PassRate"] != DBNull.Value ? Convert.ToDecimal(row["PassRate"]) : 0
                        });
                    }
                    
                    retval.Result = result;
                    retval.Code = 1;
                    retval.Message = "Lấy danh sách bài thi theo tỷ lệ đậu thành công";
                }
                else
                {
                    retval.Code = 0;
                    retval.Message = "Không có dữ liệu bài thi";
                }
            }
            catch (Exception ex)
            {
                retval.Code = -1;
                retval.Message = "Lỗi: " + ex.Message;
            }
            
            return retval;
        }

        public DbReturnInfo<List<RecentActivity>> GetRecentActivities(int limit = 20)
        {
            var retval = new DbReturnInfo<List<RecentActivity>>();
            
            try
            {
                MySqlParameter[] param = {
                    new MySqlParameter("@iLimit", limit)
                };
                
                string outVal = "";
                var dt = MySQLDataHelper.GetDataTable(ConfigurationHelper.connectString, "GetRecentActivities", param, out outVal);
                
                if (dt != null)
                {
                    var result = new List<RecentActivity>();
                    foreach (DataRow row in dt.Rows)
                    {
                        result.Add(new RecentActivity
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            ActivityType = row["ActivityType"].ToString(),
                            UserId = Convert.ToInt32(row["UserId"]),
                            UserName = row["UserName"].ToString(),
                            Description = row["Description"].ToString(),
                            Timestamp = Convert.ToDateTime(row["Timestamp"]),
                            RelatedEntityId = row["RelatedEntityId"] != DBNull.Value ? Convert.ToInt32(row["RelatedEntityId"]) : null,
                            RelatedEntityName = row["RelatedEntityName"] != DBNull.Value ? row["RelatedEntityName"].ToString() : null
                        });
                    }
                    
                    retval.Result = result;
                    retval.Code = 1;
                    retval.Message = "Lấy hoạt động gần đây thành công";
                }
                else
                {
                    retval.Code = 0;
                    retval.Message = "Không có dữ liệu hoạt động gần đây";
                }
            }
            catch (Exception ex)
            {
                retval.Code = -1;
                retval.Message = "Lỗi: " + ex.Message;
            }
            
            return retval;
        }
    }
} 
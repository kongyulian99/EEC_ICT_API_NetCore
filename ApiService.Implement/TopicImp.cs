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

namespace ApiService.Implement
{
    public class TopicImp : ITopic
    {
        public DbReturnInfo<int> CreateTopic(TopicInfo topic)
        {
            var retval = new DbReturnInfo<int>();
            MySqlParameter[] param = {
                new MySqlParameter("@sName", topic.Name),
                new MySqlParameter("@sDescription", topic.Description ?? (object)DBNull.Value),
                new MySqlParameter("@iParent_Id", topic.Parent_Id ?? (object)DBNull.Value),
            };
            
            string outVal = "";
            var dt = MySQLDataHelper.GetDataTable(ConfigurationHelper.connectString, "CreateTopic", param, out outVal);
            
            if (dt != null && dt.Rows.Count > 0)
            {
                retval.Result = Convert.ToInt32(dt.Rows[0]["New_Topic_Id"]);
                retval.Code = 1;
                retval.Message = "Create topic successfully";
            }
            else
            {
                retval.Code = 0;
                retval.Message = "Failed to create topic";
            }
            
            return retval;
        }

        public DbReturnInfo<TopicInfo> GetTopicById(int id)
        {
            var retval = new DbReturnInfo<TopicInfo>();
            MySqlParameter[] param = {
                new MySqlParameter("@iId", id)
            };
            
            string outVal = "";
            var topics = MySQLDataHelper.ExecuteReaderToList<TopicInfo>(ConfigurationHelper.connectString, "GetTopicById", param, out outVal);
            
            if (topics != null && topics.Count > 0)
            {
                retval.Result = topics.FirstOrDefault();
                retval.Code = 1;
                retval.Message = "Success";
            }
            else
            {
                retval.Code = 0;
                retval.Message = "Topic not found";
            }
            
            return retval;
        }

        public DbReturnInfo<TopicInfo> GetTopicByName(string name)
        {
            var retval = new DbReturnInfo<TopicInfo>();
            MySqlParameter[] param = {
                new MySqlParameter("@sName", name)
            };
            
            string outVal = "";
            var topics = MySQLDataHelper.ExecuteReaderToList<TopicInfo>(ConfigurationHelper.connectString, "GetTopicByName", param, out outVal);
            
            if (topics != null && topics.Count > 0)
            {
                retval.Result = topics.FirstOrDefault();
                retval.Code = 1;
                retval.Message = "Success";
            }
            else
            {
                retval.Code = 0;
                retval.Message = "Topic not found";
            }
            
            return retval;
        }

        public DbReturnInfo<int> UpdateTopic(TopicInfo topic)
        {
            var retval = new DbReturnInfo<int>();
            MySqlParameter[] param = {
                new MySqlParameter("@iId", topic.Id),
                new MySqlParameter("@sName", topic.Name),
                new MySqlParameter("@sDescription", topic.Description ?? (object)DBNull.Value)
            };
            
            string outVal = "";
            var result = MySQLDataHelper.ExecuteNonQuery(ConfigurationHelper.connectString, "UpdateTopic", param, out outVal);
            
            if (result >= 0)
            {
                retval.Result = result;
                retval.Code = 1;
                retval.Message = "Topic updated successfully";
            }
            else
            {
                retval.Code = 0;
                retval.Message = "Failed to update topic";
            }
            
            return retval;
        }

        public DbReturnInfo<int> DeleteTopic(int id)
        {
            var retval = new DbReturnInfo<int>();
            MySqlParameter[] param = {
                new MySqlParameter("@iId", id)
            };
            
            string outVal = "";
            var result = MySQLDataHelper.ExecuteNonQuery(ConfigurationHelper.connectString, "DeleteTopic", param, out outVal);
            
            if (result >= 0)
            {
                retval.Result = result;
                retval.Code = 1;
                retval.Message = "Topic deleted successfully";
            }
            else
            {
                retval.Code = 0;
                retval.Message = "Failed to delete topic";
            }
            
            return retval;
        }

        public DbReturnInfo<List<TopicInfo>> GetAllTopics()
        {
            var retval = new DbReturnInfo<List<TopicInfo>>();
            string outVal = "";
            var topics = MySQLDataHelper.ExecuteReaderToList<TopicInfo>(ConfigurationHelper.connectString, "GetAllTopics", null, out outVal);
            
            if (topics != null)
            {
                retval.Result = topics;
                retval.Code = 1;
                retval.Message = "Success";
            }
            else
            {
                retval.Code = 0;
                retval.Message = "Failed to get topics list";
                retval.Result = new List<TopicInfo>();
            }
            
            return retval;
        }

        public DbReturnInfo<List<TopicInfo>> GetChildTopics(int parent_id)
        {
            var retval = new DbReturnInfo<List<TopicInfo>>();
            string outVal = "";

            MySqlParameter[] param = {
                new MySqlParameter("@iParent_Id", parent_id)
            };

            var topics = MySQLDataHelper.ExecuteReaderToList<TopicInfo>(ConfigurationHelper.connectString, "GetChildTopics", null, out outVal);

            if (topics != null)
            {
                retval.Result = topics;
                retval.Code = 1;
                retval.Message = "Success";
            }
            else
            {
                retval.Code = 0;
                retval.Message = "Failed to get child topics";
                retval.Result = new List<TopicInfo>();
            }

            return retval;
        }
    }
} 
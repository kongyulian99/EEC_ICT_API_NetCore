using ApiService.Entity;
using System;
using System.Collections.Generic;
using System.Data;

namespace ApiService.Interface
{
    public interface ITopic
    {
        DbReturnInfo<int> CreateTopic(TopicInfo topic);
        DbReturnInfo<TopicInfo> GetTopicById(int id);
        DbReturnInfo<int> UpdateTopic(TopicInfo topic);
        DbReturnInfo<int> DeleteTopic(int id);
        DbReturnInfo<List<TopicInfo>> GetAllTopics();
        DbReturnInfo<List<TopicInfo>> GetChildTopics(int parentId);
    }
} 
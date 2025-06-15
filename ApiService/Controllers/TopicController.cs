using ApiService.Business;
using ApiService.Common;
using ApiService.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : MyBaseController<TopicController>
    {
        private readonly IMemoryCache _memoryCache;

        public TopicController(
            //DI default service
            ILogger<TopicController> logger
            , AppSetting appSettingInfo
            //DI services
            , IMemoryCache memoryCache
         ) : base(logger, appSettingInfo)
        {
            _memoryCache = memoryCache;
        }
        
        /// <summary>
        /// Get all topics
        /// </summary>
        [HttpGet("GetAll")]
        public IActionResult GetAllTopics()
        {
            var retval = new ReturnBaseInfo<List<TopicInfo>>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Failed", Code = 0 };
            
            var lstTopic = ServiceFactory.Topic.GetAllTopics();
            retval.ReturnData = lstTopic.Result;
            retval.ReturnStatus.Code = lstTopic.Code;
            retval.ReturnStatus.Message = lstTopic.Code == 1 ? "Get topics list successful" : lstTopic.Message;
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Get topic by ID
        /// </summary>
        [HttpGet("GetById/{id}")]
        public IActionResult GetTopicById(int id)
        {
            var retval = new ReturnBaseInfo<TopicInfo>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Failed", Code = 0 };
            
            if (id <= 0)
            {
                retval.ReturnStatus.Message = "Invalid topic ID";
                return Ok(retval);
            }
            
            var topic = ServiceFactory.Topic.GetTopicById(id);
            retval.ReturnData = topic.Result;
            retval.ReturnStatus.Code = topic.Code;
            retval.ReturnStatus.Message = topic.Code == 1 ? "Get topic information successful" : topic.Message;
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Get all root topics (topics without parent)
        /// </summary>
        [HttpGet("GetRootTopics")]
        public IActionResult GetRootTopics()
        {
            var retval = new ReturnBaseInfo<List<TopicInfo>>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Failed", Code = 0 };
            
            var allTopics = ServiceFactory.Topic.GetAllTopics();
            if (allTopics.Code == 1 && allTopics.Result != null)
            {
                var rootTopics = allTopics.Result.Where(t => t.Parent_Id == null).ToList();
                retval.ReturnData = rootTopics;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Get root topics successful";
            }
            else
            {
                retval.ReturnStatus.Code = allTopics.Code;
                retval.ReturnStatus.Message = allTopics.Message;
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Get child topics of a parent topic
        /// </summary>
        [HttpGet("GetChildTopics/{parentId}")]
        public IActionResult GetChildTopics(int parentId)
        {
            var retval = new ReturnBaseInfo<List<TopicInfo>>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Failed", Code = 0 };
            
            if (parentId <= 0)
            {
                retval.ReturnStatus.Message = "Invalid parent topic ID";
                return Ok(retval);
            }
            
            var topics = ServiceFactory.Topic.GetChildTopics(parentId);
            retval.ReturnData = topics.Result;
            retval.ReturnStatus.Code = topics.Code;
            retval.ReturnStatus.Message = topics.Code == 1 ? "Get child topics successful" : topics.Message;
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Create new topic
        /// </summary>
        [HttpPost("Create")]
        public IActionResult CreateTopic([FromBody] TopicInfo topic)
        {
            var retval = new ReturnBaseInfo<int>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Failed", Code = 0 };
            
            if (topic == null || string.IsNullOrEmpty(topic.Name))
            {
                retval.ReturnStatus.Message = "Invalid data";
                return Ok(retval);
            }
            
            // Kiểm tra Parent_Id nếu có
            if (topic.Parent_Id.HasValue && topic.Parent_Id.Value > 0)
            {
                var parentTopic = ServiceFactory.Topic.GetTopicById(topic.Parent_Id.Value);
                if (parentTopic.Code != 1 || parentTopic.Result == null)
                {
                    retval.ReturnStatus.Message = "Parent topic not found";
                    return Ok(retval);
                }
            }
            
            var createResult = ServiceFactory.Topic.CreateTopic(topic);
            retval.ReturnData = createResult.Result;
            retval.ReturnStatus.Code = createResult.Code;
            retval.ReturnStatus.Message = createResult.Message;
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Update topic information
        /// </summary>
        [HttpPut("Update")]
        public IActionResult UpdateTopic([FromBody] TopicInfo topic)
        {
            var retval = new ReturnBaseInfo<int>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Failed", Code = 0 };
            
            if (topic == null || topic.Id <= 0 || string.IsNullOrEmpty(topic.Name))
            {
                retval.ReturnStatus.Message = "Invalid data";
                return Ok(retval);
            }
            
            // Kiểm tra Parent_Id nếu có
            if (topic.Parent_Id.HasValue && topic.Parent_Id.Value > 0)
            {
                // Kiểm tra xem có đang tạo chu trình không (topic không thể là cha của chính nó)
                if (topic.Parent_Id.Value == topic.Id)
                {
                    retval.ReturnStatus.Message = "Topic cannot be its own parent";
                    return Ok(retval);
                }
                
                var parentTopic = ServiceFactory.Topic.GetTopicById(topic.Parent_Id.Value);
                if (parentTopic.Code != 1 || parentTopic.Result == null)
                {
                    retval.ReturnStatus.Message = "Parent topic not found";
                    return Ok(retval);
                }
            }
            
            var updateResult = ServiceFactory.Topic.UpdateTopic(topic);
            retval.ReturnData = updateResult.Result;
            retval.ReturnStatus.Code = updateResult.Code;
            retval.ReturnStatus.Message = updateResult.Message;
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Delete topic
        /// </summary>
        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteTopic(int id)
        {
            var retval = new ReturnBaseInfo<int>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Failed", Code = 0 };
            
            if (id <= 0)
            {
                retval.ReturnStatus.Message = "Invalid topic ID";
                return Ok(retval);
            }
            
            var deleteResult = ServiceFactory.Topic.DeleteTopic(id);
            retval.ReturnData = deleteResult.Result;
            retval.ReturnStatus.Code = deleteResult.Code;
            retval.ReturnStatus.Message = deleteResult.Message;
            
            return Ok(retval);
        }
    }
} 
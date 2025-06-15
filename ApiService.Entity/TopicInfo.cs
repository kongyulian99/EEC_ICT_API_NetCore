using System;
using System.Collections.Generic;
using System.Text;

namespace ApiService.Entity
{
    public class TopicInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Parent_Id { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }
} 
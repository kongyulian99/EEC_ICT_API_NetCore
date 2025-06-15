using System;

namespace ApiService.Entity
{
    public class ExamInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration_Minutes { get; set; }
        public int Total_Questions { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }
} 
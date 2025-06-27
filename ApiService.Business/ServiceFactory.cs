using ApiService.Entity;
using ApiService.Implement;
using ApiService.Interface;
using Microsoft.Extensions.Configuration;
using System;

namespace ApiService.Business
{
    public class ServiceFactory
    {    
        private static IConfiguration _configuration;
        
        public static void SetConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        private static IUser iUser;
        public static IUser User
        {
            get { return iUser ?? (iUser = new UserImp()); }
        }
        
        private static ITopic iTopic;
        public static ITopic Topic
        {
            get { return iTopic ?? (iTopic = new TopicImp()); }
        }

        private static IExam iExam;
        public static IExam Exam
        {
            get { return iExam ?? (iExam = new ExamImp()); }
        }
        
        private static IQuestion iQuestion;
        public static IQuestion Question
        {
            get { return iQuestion ?? (iQuestion = new QuestionImp()); }
        }
        
        private static IUserExamAttempt iUserExamAttempt;
        public static IUserExamAttempt UserExamAttempt
        {
            get { return iUserExamAttempt ?? (iUserExamAttempt = new UserExamAttemptImp()); }
        }
        
        private static IUserExamAnswer iUserExamAnswer;
        public static IUserExamAnswer UserExamAnswer
        {
            get { return iUserExamAnswer ?? (iUserExamAnswer = new UserExamAnswerImp()); }
        }

        // mailjet
        private static IMailjet iMailjet;
        public static IMailjet Mailjet
        {
            get { return iMailjet ?? (iMailjet = new MailjetImp(_configuration)); }
        }
    }
}

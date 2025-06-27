using ApiService.Core;
using ApiService.Core.DataHelper;
using ApiService.Core.Log;
using ApiService.Entity;
using ApiService.Interface;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApiService.Implement
{
    public class MailjetImp : IMailjet
    {
        private readonly IConfiguration _configuration;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public MailjetImp(IConfiguration configuration)
        {
            _configuration = configuration;
            _senderEmail = _configuration["MailjetSettings:SenderEmail"] ?? "default@example.com";
            _senderName = _configuration["MailjetSettings:SenderName"] ?? "System";
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, string plainTextContent = null)
        {
            try
            {
                var apiKey = _configuration["MailjetSettings:ApiKey"];
                var apiSecret = _configuration["MailjetSettings:ApiSecret"];
                
                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
                {
                    //Logger.Error("Mailjet API key or secret is missing");
                    return false;
                }
                
                MailjetClient client = new MailjetClient(apiKey, apiSecret);

                MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource
                };

                JArray messages = new JArray {
                    new JObject {
                        {
                            "From", new JObject {
                                { "Email", _senderEmail },
                                { "Name", _senderName }
                            }
                        },
                        {
                            "To", new JArray {
                                new JObject {
                                    { "Email", toEmail }
                                }
                            }
                        },
                        { "Subject", subject },
                        { "HTMLPart", htmlContent }
                    }
                };

                if (!string.IsNullOrEmpty(plainTextContent))
                {
                    messages[0]["TextPart"] = plainTextContent;
                }

                request.Property(Send.Messages, messages);

                MailjetResponse response = await client.PostAsync(request);
                
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    //Logger.Error($"Mailjet error: {response.GetData()}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }
    }
} 
using System;
using System.Threading.Tasks;
using Serilog;
using RabbitMqSettings = Thorium.Core.MessageQueue.ConfigurationModel.RabbitMqSettings;

namespace Thorium.Aggregator.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly ILogger _logger;


        public EmailSender(RabbitMqSettings rabbitMqSettings, ILogger logger)
        {
            _rabbitMqSettings = rabbitMqSettings;
            _logger = logger;
        }

        public async Task SendEmail(string email, string subject, string message, string toUsername)
        {
            try
            {
               //todo: implement
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
            }

        }
    }
}

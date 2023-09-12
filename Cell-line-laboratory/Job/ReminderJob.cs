using Cell_line_laboratory.Utils;
using Quartz;

namespace Cell_line_laboratory.Job
{
    public class ReminderJob : IJob
    {
        

        public async Task Execute(IJobExecutionContext context)
        {
            // Implement logic to send reminder emails



            await EmailSender.SendEmailAsync("recipient@example.com", "Reminder", "Don't forget to do something!");
        }
    

    }
}

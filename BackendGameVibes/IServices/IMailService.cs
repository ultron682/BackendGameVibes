using BackendGameVibes.Models;

namespace BackendGameVibes.IServices
{
    public interface IMailService
    {
        bool SendMail(MailData Mail_Data);
    }
}

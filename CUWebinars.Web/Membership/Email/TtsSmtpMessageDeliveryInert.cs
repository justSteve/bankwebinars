using BrockAllen.MembershipReboot;

namespace CUWebinars.Web.Membership.Email
{
    public class TtsSmtpMessageDeliveryInert :  IMessageDelivery
    {
        public void Send(Message msg)
        {
            // do nada
        }
    }
}
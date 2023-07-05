using OperationManagement.Data.ViewModels;

namespace OperationManagement.Data.Common
{
    public class OTPServices
    {
        public const string SessionKey = "OTP";
        private string? recipient;
        public string? APIKey { get; private set; }
        static public bool SendEmailOTP(HttpContext httpContext, string email)
        {
            try
            {
                int otpValue = new Random().Next(100000, 999999);
                var durationInSeconds = 150;
                httpContext.Session.SetString(SessionKey, otpValue.ToString());
                var timer = new Timer(state =>
                {
                    httpContext.Session.Remove(SessionKey);
                }, null, TimeSpan.FromSeconds(durationInSeconds), Timeout.InfiniteTimeSpan);
                #region sendingProcess
                EmailHelper.SendOTP(email, otpValue.ToString());
                #endregion
                return true;
            }
            catch (Exception err)
            {
                return false;
            }
        }
        static public bool VerifyOTP(HttpContext httpContext, OTPVM receivedOTP)
        {
            bool result = false;
            /*var resceivedStringOTP = string.Join("", receivedOTP.OTP);*/
            var resceivedStringOTP = receivedOTP.OTP;

            var ValidOTP = httpContext.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(ValidOTP))
            {
                return false;
            }
            //var stringReceviedOTP = string.Join("", receivedOTP.OTP);

            if (resceivedStringOTP == ValidOTP)
            {
                return true;
            }
            return false;
        }
        static public bool HaveOTP(HttpContext httpContext)
        {
            var ValidOTP = httpContext.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(ValidOTP))
            {
                return false;
            }
            else return true;
        }
    }
    
}

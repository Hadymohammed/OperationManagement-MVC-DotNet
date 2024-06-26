﻿using System.Net.Mail;
using System.Net;

namespace OperationManagement.Data.Common
{
    public class EmailHelper
    {
        private const string SenderEmail = "operatobusiness@gmail.com";
        private const string SenderName = "Operato";
        private const string SenderPassword = "jqypqhbhbnlruzld";
        private const string AdminEmail = "engabdelhadymohamed@gmail.com";
        static private bool SendEmail(string ReciverEmail, string messege, string subject, string ReciverName)
        {
            try
            {
                #region sendingProcess
                var fromAddress = new MailAddress(SenderEmail, SenderName);
                var toAddress = new MailAddress(ReciverEmail, ReciverName);
                string body = messege;
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, SenderPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                })
                {
                    smtp.Send(message);
                }
                #endregion
                return true;
            }
            catch (Exception err)
            {
                return false;
            }
        }
        static public bool SendEmailAddStaff(string email, string Url, string EnterpriseName)
        {
            string body = $"Sent from {SenderName} , here is your invitation link to be a staff member at {EnterpriseName} , do not share it with anyone  {Url} . ";
            const string subject = "Your invetation";
            const string reciverName = "New Staff";
            return SendEmail(email, body, subject, reciverName);
        }
        static public bool SendEmailAddAdmin(string email, string Url)
        {
            string body = $"Sent from {SenderName} , here is your invitation link to be an admin at {SenderName} , do not share it with anyone  {Url} . ";
            const string subject = "Your invetation";
            const string reciverName = "New Admin";
            return SendEmail(email, body, subject, reciverName);
        }
        static public bool SendEnterpriseRejection(string email, string Messege, string EnterpriseName)
        {
            const string subject = "Enterprise Rejection";
            string reciverName = $"{EnterpriseName} Manager";
            return SendEmail(email, Messege, subject, reciverName);
        }
        static public bool SendEnterpriseAccept(string email, string Url, string EnterpriseName, string? messege)
        {
            string messegeToSend = messege ?? $"Congratulations {EnterpriseName} Manager , your request has been accepted.<br/>";
            string body = $"{messegeToSend}Here is your invitation link to be a staff member at {EnterpriseName} , do not share it with anyone  {Url} . ";
            const string subject = "Enterprise accepted!";
            const string reciverName = "New Staff";
            return SendEmail(email, body, subject, reciverName);
        }
        static public bool SendOTP(string email, string OTP)
        {
            string body = $"Sent from {SenderName} , Here is your OTP is {OTP} . Don't share it with anyone. ";
            const string subject = "Your OTP";
            const string reciverName = "New User";
            return SendEmail(email, body, subject, reciverName);
        }
        static public bool NotifyNewEnterprise(string SenderName, string EnterpriseName)
        {
            string body = $"{EnterpriseName} has been created by {SenderName} and waiting for your approval. ";
            const string subject = "New Enterprise";
            const string reciverName = "Admin";
            const string reciverEmail = AdminEmail;
            return SendEmail(reciverEmail, body, subject, reciverName);
        }
    }
}

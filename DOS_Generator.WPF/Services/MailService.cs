using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows;
using DOS_Generator.Core.Models;

namespace DOS_Generator.WPF.Services
{
    public static class MailService
    {
        public static SmtpClient SmtpServer = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            UseDefaultCredentials = false,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        public static async Task SendMail(MailMessage mail, NetworkCredential credentials)
        {
            if (mail == null) return;

            try
            {
                SmtpServer.Credentials = credentials;
                await SmtpServer.SendMailAsync(mail);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public static async Task SendDeclaration(Declaration declaration, NetworkCredential credentials)
            //Get the ships email or it's agency email
        {
            var destination = declaration.Ship.Email ?? declaration.Ship.Agency?.Email;

            if (string.IsNullOrEmpty(destination)) return;

            var output = await DeclarationCreatorService.GenerateDeclaration(declaration);

            if (!File.Exists(output)) return;


            using var reader = File.OpenText(@".\Resources\message.html");
            using var mail = new MailMessage
            {
                From = new MailAddress(credentials.UserName),
                Subject = $"DECLARATION OF SECURITY ({declaration.Ship.Name})",
                IsBodyHtml = true,
                Body = await reader.ReadToEndAsync(),
                To = {destination},
                Attachments = {new Attachment(output)}
            };
            await SendMail(mail, credentials);
        }

        public static void SendDeclarations(List<Declaration> declarations, NetworkCredential credentials)
        {
            if (credentials == null
                || string.IsNullOrWhiteSpace(credentials.UserName)
                || string.IsNullOrWhiteSpace(credentials.Password)
                || declarations.Count >= 0) return;

            

            declarations.ForEach(async declaration =>
                await SendDeclaration(declaration, credentials));
        }
    }
}
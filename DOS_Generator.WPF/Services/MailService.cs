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
        private static async Task SendMail(MailMessage mail, ICredentialsByHost credentials)
        {
            if (mail == null) return;

            try
            {
                var server = App.User.MailServer;
                var smtpServer = new SmtpClient
                {
                    Host = server.Host,
                    Port = server.Port,
                    UseDefaultCredentials = false,
                    EnableSsl = true,
                    Credentials = credentials
                };

                await smtpServer.SendMailAsync(mail);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private static async Task<string> GetMessage()
        {
            string content = null;
            const string htmlFile = @".\Resources\message.html";

            if (!File.Exists(htmlFile))
            {
                if (File.Exists(@".\Resources\message.docx"))
                    DeclarationCreatorService.ConvertWordToHtml(@".\Resources\message.docx", htmlFile);
                else
                    content =
                        @"<html>
                             <body>
                                 <p>Dear all,</p>
                                 <p>Good day,</p>
                                 <p>Welcome to our port,</p>
                                 <p>Please find as attachment:</p>
                                 <p><b>DECLARATION OF SECURITY</b></p>
                                 <p>- The <b>S.S.O</b> must fill up, sign and re-send this declaration please.</p>
                                 <p>Best Regards.</p>
                             </body>
                        </html>";
            }
            else
            {
                using (var reader = File.OpenText(htmlFile))
                {
                    content = await reader.ReadToEndAsync();
                }
            }

            return content;
        }

        public static async Task SendDeclaration(Declaration declaration, NetworkCredential credentials)
            //Get the ships email or it's agency email
        {
            var destination = declaration.Ship.Email ?? declaration.Ship.Agency?.Email;

            if (string.IsNullOrEmpty(destination)) return;

            var output = await DeclarationCreatorService.GenerateDeclaration(declaration);

            if (!File.Exists(output)) return;

            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(credentials.UserName);
                mail.Subject = $"DECLARATION OF SECURITY ({declaration.Ship.Name})";
                mail.IsBodyHtml = true;
                mail.Body = await GetMessage();
                mail.To.Add(destination);
                mail.Attachments.Add(new Attachment(output));

                await SendMail(mail, credentials);
            }
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
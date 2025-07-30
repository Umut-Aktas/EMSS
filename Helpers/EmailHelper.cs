using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace EMailSender
{
    /// <summary>
    /// Email gönderimi için yardımcı sınıf. Bu sınıf DLL olarak başka projelerde kullanılabilir.
    /// </summary>
    public static class EmailHelper
    {
        /// <summary>
        /// Yardım bilgilerini konsola yazdırır ve mevcut fonksiyonları açıklar
        /// </summary>
        public static void Help()
        {
            Console.WriteLine("=== EMAIL HELPER KULLANIM KILAVUZU ===");
            Console.WriteLine();

            Console.WriteLine("1. SendEmail() - Tek e-posta gönderir (Tüm SMTP ayarlarını manuel girin)");
            Console.WriteLine("   Parametreler:");
            Console.WriteLine("   - smtpHost: SMTP sunucu adresi (örn: smtp.gmail.com)");
            Console.WriteLine("   - smtpPort: SMTP port numarası (örn: 587)");
            Console.WriteLine("   - smtpUser: SMTP kullanıcı adı");
            Console.WriteLine("   - smtpPass: SMTP şifresi");
            Console.WriteLine("   - from: Gönderen e-posta adresi");
            Console.WriteLine("   - to: Alıcı e-posta adresi");
            Console.WriteLine("   - subject: E-posta konusu");
            Console.WriteLine("   - body: E-posta içeriği");
            Console.WriteLine("   - cc: CC alıcıları (opsiyonel)");
            Console.WriteLine("   - bcc: BCC alıcıları (opsiyonel)");
            Console.WriteLine("   - attachmentPaths: Dosya ekleri yolları (opsiyonel)");
            Console.WriteLine();

            Console.WriteLine("1b. SendEmailQuick() - Hızlı e-posta gönderir (Default ayarları kullanır)");
            Console.WriteLine("   Parametreler:");
            Console.WriteLine("   - to: Alıcı e-posta adresi");
            Console.WriteLine("   - subject: E-posta konusu");
            Console.WriteLine("   - body: E-posta içeriği");
            Console.WriteLine("   - cc, bcc, attachmentPaths: Opsiyonel");
            Console.WriteLine("   NOT: launchSettings.json'da DEFAULT_EMAIL ve DEFAULT_EMAIL_PASSWORD ayarlanmalı");
            Console.WriteLine();

            Console.WriteLine("2. SendBulkEmail() - Toplu e-posta gönderir (Manuel SMTP ayarları)");
            Console.WriteLine("   SendEmail ile aynı parametreler, ancak 'to' yerine 'recipients' listesi kullanır");
            Console.WriteLine();

            Console.WriteLine("2b. SendBulkEmailQuick() - Hızlı toplu e-posta gönderir (Default ayarları)");
            Console.WriteLine("   - recipients: Alıcı listesi");
            Console.WriteLine("   - subject, body: E-posta konusu ve içeriği");
            Console.WriteLine("   - cc, bcc, attachmentPaths: Opsiyonel");
            Console.WriteLine();

            Console.WriteLine("3. GetEmailHistory() - E-posta geçmişini getirir");
            Console.WriteLine("   Parametresiz, gönderilen e-postaların geçmişini döndürür");
            Console.WriteLine();

            Console.WriteLine("4. CheckHealth() - Sistem durumunu kontrol eder");
            Console.WriteLine("   Parametresiz, sistem sağlık bilgilerini döndürür");
            Console.WriteLine();

            Console.WriteLine("HIZLI KULLANIM ÖRNEĞİ (Önerilen):");
            Console.WriteLine("var result = EmailHelper.SendEmailQuick(");
            Console.WriteLine("    \"receiver@example.com\", \"Test Subject\", \"Test Body\");");
            Console.WriteLine();

            Console.WriteLine("MANUEL KULLANIM ÖRNEĞİ:");
            Console.WriteLine("var result = EmailHelper.SendEmail(");
            Console.WriteLine("    \"smtp.gmail.com\", 587, \"user@gmail.com\", \"password\",");
            Console.WriteLine("    \"sender@example.com\", \"receiver@example.com\",");
            Console.WriteLine("    \"Test Subject\", \"Test Body\");");
            Console.WriteLine();

            Console.WriteLine("DEFAULT AYARLAR YAPLANDIRMA:");
            Console.WriteLine("launchSettings.json dosyasında şu environment variable'ları ayarlayın:");
            Console.WriteLine("- DEFAULT_EMAIL: your-email@gmail.com");
            Console.WriteLine("- DEFAULT_EMAIL_PASSWORD: your-app-password");
            Console.WriteLine("- DEFAULT_SMTP_HOST: smtp.gmail.com (varsayılan)");
            Console.WriteLine("- DEFAULT_SMTP_PORT: 587 (varsayılan)");
            Console.WriteLine();

            Console.WriteLine("NOT: Gmail için uygulama şifresi gerekir.");
            Console.WriteLine("=====================================");
        }

        /// <summary>
        /// Environment variable'lardan default SMTP ayarlarını getirir
        /// </summary>
        private static (string host, int port, string user, string pass) GetDefaultSmtpSettings()
        {
            var accountInfosFilePath = Path.Combine(Directory.GetCurrentDirectory(), "AccountInfos.json");

            try
            {
                if (File.Exists(accountInfosFilePath))
                {
                    string json = File.ReadAllText(accountInfosFilePath);
                    var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                    if (data != null && data.ContainsKey("MailAdress") && data.ContainsKey("Password"))
                    {
                        string mail = data["MailAdress"];
                        string password = data["Password"];

                        var host = Environment.GetEnvironmentVariable("DEFAULT_SMTP_HOST") ?? "smtp.gmail.com";
                        var portStr = Environment.GetEnvironmentVariable("DEFAULT_SMTP_PORT") ?? "587";
                        var user = Environment.GetEnvironmentVariable("DEFAULT_EMAIL") ?? mail;
                        var pass = Environment.GetEnvironmentVariable("DEFAULT_EMAIL_PASSWORD") ?? password;

                        if (!int.TryParse(portStr, out int port))
                            port = 587;

                        return (host, port, user, pass);
                    }
                    else
                    {
                        throw new InvalidOperationException("Mail address or password not found in the JSON file. Please ensure SaveMailAccount has been called.");
                    }
                }
                else
                {
                    throw new FileNotFoundException("AccountInfos.json file not found. Please ensure SaveMailAccount has been called.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // Optionally, rethrow or handle the exception as needed.
                throw;
            }
        }


        /// <summary>
        /// Default ayarları kullanarak tek e-posta gönderir (Sadece alıcı, konu ve içerik belirtmeniz yeterli)
        /// </summary>
        public static EmailResponse<EmailSendResult> SendEmailQuick(
            string to,
            string subject,
            string body,
            List<string>? cc = null,
            List<string>? bcc = null,
            List<string>? attachmentPaths = null)
        {
            var (host, port, user, pass) = GetDefaultSmtpSettings();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                return new EmailResponse<EmailSendResult>
                {
                    Status = false,
                    Message = "Default email ayarları yapılandırılmamış. launchSettings.json dosyasında DEFAULT_EMAIL ve DEFAULT_EMAIL_PASSWORD ayarlarını kontrol edin.",
                    Data = new EmailSendResult { Success = false, EmailId = string.Empty }
                };
            }

            return SendEmail(host, port, user, pass, user, to, subject, body, cc, bcc, attachmentPaths);
        }

        /// <summary>
        /// Default ayarları kullanarak toplu e-posta gönderir
        /// </summary>
        public static EmailResponse<BulkEmailResult> SendBulkEmailQuick(
            List<string> recipients,
            string subject,
            string body,
            List<string>? cc = null,
            List<string>? bcc = null,
            List<string>? attachmentPaths = null)
        {
            var (host, port, user, pass) = GetDefaultSmtpSettings();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                return new EmailResponse<BulkEmailResult>
                {
                    Status = false,
                    Message = "Default email ayarları yapılandırılmamış. launchSettings.json dosyasında DEFAULT_EMAIL ve DEFAULT_EMAIL_PASSWORD ayarlarını kontrol edin.",
                    Data = new BulkEmailResult()
                };
            }

            return SendBulkEmail(host, port, user, pass, user, recipients, subject, body, cc, bcc, attachmentPaths);
        }

        /// <summary>
        /// Tek e-posta gönderir
        /// </summary>
        public static EmailResponse<EmailSendResult> SendEmail(
            string smtpHost,
            int smtpPort,
            string smtpUser,
            string smtpPass,
            string from,
            string to,
            string subject,
            string body,
            List<string>? cc = null,
            List<string>? bcc = null,
            List<string>? attachmentPaths = null)
        {
            var response = new EmailResponse<EmailSendResult>();
            var emailHistory = new EmailHistory
            {
                From = from,
                To = to,
                Subject = subject,
                Body = body,
                Cc = cc,
                Bcc = bcc,
                SmtpHost = smtpHost,
                SmtpPort = smtpPort,
                SmtpUser = smtpUser,
                SmtpPass = smtpPass,
                AttachmentNames = attachmentPaths
            };

            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(from);
                    mail.To.Add(to);
                    mail.Subject = subject;
                    mail.Body = body;

                    // CC
                    if (cc != null)
                    {
                        foreach (var c in cc)
                        {
                            if (!string.IsNullOrWhiteSpace(c))
                                mail.CC.Add(c);
                        }
                    }

                    // BCC
                    if (bcc != null)
                    {
                        foreach (var b in bcc)
                        {
                            if (!string.IsNullOrWhiteSpace(b))
                                mail.Bcc.Add(b);
                        }
                    }

                    // Attachments
                    if (attachmentPaths != null)
                    {
                        foreach (var path in attachmentPaths)
                        {
                            if (File.Exists(path))
                            {
                                mail.Attachments.Add(new Attachment(path));
                            }
                        }
                    }

                    using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
                    {
                        smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPass);
                        smtpClient.EnableSsl = true;
                        smtpClient.Send(mail);
                    }
                }

                var emailId = Guid.NewGuid().ToString();
                emailHistory.IsSuccess = true;
                emailHistory.ErrorMessage = null;
                emailHistory.Timestamp = DateTime.UtcNow;

                response.Data = new EmailSendResult { Success = true, EmailId = emailId };
                response.Status = true;
                response.Message = "E-posta başarıyla gönderildi.";

                SaveEmailHistory(emailHistory);
            }
            catch (Exception ex)
            {
                emailHistory.IsSuccess = false;
                emailHistory.ErrorMessage = ex.ToString();
                emailHistory.Timestamp = DateTime.UtcNow;

                response.Data = new EmailSendResult { Success = false, EmailId = string.Empty };
                response.Status = false;
                response.Message = $"E-posta gönderim hatası: {ex.Message}";

                SaveEmailHistory(emailHistory);
            }

            return response;
        }

        /// <summary>
        /// Toplu e-posta gönderir
        /// </summary>
        public static EmailResponse<BulkEmailResult> SendBulkEmail(
            string smtpHost,
            int smtpPort,
            string smtpUser,
            string smtpPass,
            string from,
            List<string> recipients,
            string subject,
            string body,
            List<string>? cc = null,
            List<string>? bcc = null,
            List<string>? attachmentPaths = null)
        {
            var response = new EmailResponse<BulkEmailResult>();
            var bulkResult = new BulkEmailResult();

            if (recipients == null || !recipients.Any())
            {
                response.Data = bulkResult;
                response.Status = false;
                response.Message = "En az bir alıcı e-posta adresi gerekli.";
                return response;
            }

            bulkResult.TotalCount = recipients.Count;
            bulkResult.BulkEmailId = Guid.NewGuid().ToString();

            foreach (var recipient in recipients)
            {
                try
                {
                    var singleResult = SendEmail(smtpHost, smtpPort, smtpUser, smtpPass, from, recipient, subject, body, cc, bcc, attachmentPaths);

                    if (singleResult.Status)
                    {
                        bulkResult.SuccessCount++;
                    }
                    else
                    {
                        bulkResult.FailedRecipients.Add(recipient);
                    }
                }
                catch (Exception ex)
                {
                    bulkResult.FailedRecipients.Add(recipient);
                    Console.WriteLine($"Alıcı {recipient} için hata: {ex.Message}");
                }
            }

            bulkResult.OverallSuccess = bulkResult.SuccessCount == bulkResult.TotalCount;

            response.Data = bulkResult;
            response.Status = true;
            response.Message = $"Toplu e-posta gönderimi tamamlandı. Başarılı: {bulkResult.SuccessCount}/{bulkResult.TotalCount}";

            return response;
        }

        /// <summary>
        /// E-posta geçmişini getirir
        /// </summary>
        public static EmailHistoryResponse GetEmailHistory()
        {
            try
            {
                var historyFilePath = Path.Combine(Directory.GetCurrentDirectory(), "email_history.json");
                var response = new EmailHistoryResponse();

                if (File.Exists(historyFilePath))
                {
                    var jsonContent = File.ReadAllText(historyFilePath);
                    if (!string.IsNullOrWhiteSpace(jsonContent))
                    {
                        var history = JsonSerializer.Deserialize<List<EmailHistory>>(jsonContent);

                        if (history != null)
                        {
                            response.History = history.OrderByDescending(h => h.Timestamp).ToList();
                            response.TotalCount = history.Count;
                            response.SuccessCount = history.Count(h => h.IsSuccess);
                            response.FailureCount = history.Count(h => !h.IsSuccess);
                        }
                    }
                }
                else
                {
                    // Dosya yoksa boş dosya oluştur
                    File.WriteAllText(historyFilePath, "[]");
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email history okuma hatası: {ex.Message}");
                return new EmailHistoryResponse
                {
                    History = new List<EmailHistory>(),
                    TotalCount = 0,
                    SuccessCount = 0,
                    FailureCount = 0
                };
            }
        }

        /// <summary>
        /// Sistem durumunu kontrol eder
        /// </summary>
        public static object CheckHealth()
        {
            return new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Service = "Email Helper Library",
                Version = "1.0.0"
            };
        }

        /// <summary>
        /// E-posta geçmişini dosyaya kaydeder
        /// </summary>
        private static void SaveEmailHistory(EmailHistory emailHistory)
        {
            try
            {
                var historyFilePath = Path.Combine(Directory.GetCurrentDirectory(), "email_history.json");
                List<EmailHistory> history = new List<EmailHistory>();

                if (File.Exists(historyFilePath))
                {
                    var existingContent = File.ReadAllText(historyFilePath);
                    if (!string.IsNullOrWhiteSpace(existingContent))
                    {
                        history = JsonSerializer.Deserialize<List<EmailHistory>>(existingContent) ?? new List<EmailHistory>();
                    }
                }

                history.Add(emailHistory);

                var jsonContent = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(historyFilePath, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email history kaydetme hatası: {ex.Message}");
            }
        }

        public static void SaveMailAccount(string mailAdress, string password)
		{
            var path = Path.Combine(Directory.GetCurrentDirectory(), "AccountInfos.json");

            var data = new Dictionary<string, string>
            {
                ["MailAdress"] = mailAdress,
                ["Password"] = password
            };

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

            if (File.Exists(path))
            {
                File.WriteAllText(path, json);
            }
            else
            {
                File.WriteAllText(path, "[]"); // Boş bir JSON dizisi yaz
                File.WriteAllText(path, json); // Yeni veriyi yaz
            }

        }
    }
}

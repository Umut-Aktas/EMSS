using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;

namespace EMailSender
{
    public class EmailSender
    {
        public EmailResponse<EmailSendResult> SendEmail(
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
            List<IFormFile>? attachments = null)
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
                AttachmentNames = attachments?.Select(f => f.FileName).ToList()
            };

            try
            {
                var mail = new MailMessage();
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
                if (attachments != null)
                {
                    foreach (var file in attachments)
                    {
                        if (file != null && file.Length > 0)
                        {
                            var stream = file.OpenReadStream();
                            var attachment = new Attachment(stream, file.FileName);
                            mail.Attachments.Add(attachment);
                        }
                    }
                }

                var smtpClient = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                smtpClient.Send(mail);
                response.Data = new EmailSendResult 
                { 
                    Success = true, 
                    EmailId = emailHistory.Id 
                };
                response.Status = true;
                response.Message = "E-posta başarıyla gönderildi.";
                
                // Başarılı gönderim kaydı
                emailHistory.IsSuccess = true;
                SaveEmailHistory(emailHistory);
            }
            catch (Exception ex)
            {
                response.Data = new EmailSendResult 
                { 
                    Success = false, 
                    EmailId = emailHistory.Id 
                };
                response.Status = false;
                response.Message = $"E-posta gönderilemedi: {ex.Message}";
                
                // Başarısız gönderim kaydı
                emailHistory.IsSuccess = false;
                emailHistory.ErrorMessage = ex.Message;
                SaveEmailHistory(emailHistory);
            }
            return response;
        }

        public EmailResponse<BulkEmailResult> SendBulkEmail(
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
            List<IFormFile>? attachments = null)
        {
            var response = new EmailResponse<BulkEmailResult>();
            var bulkResult = new BulkEmailResult();
            var bulkEmailId = Guid.NewGuid().ToString();
            bulkResult.BulkEmailId = bulkEmailId;
            bulkResult.TotalCount = recipients.Count;

            var emailHistory = new EmailHistory
            {
                Id = bulkEmailId,
                From = from,
                To = string.Join(", ", recipients.Take(3)) + (recipients.Count > 3 ? $" ve {recipients.Count - 3} kişi daha" : ""),
                Subject = subject,
                Body = body,
                Cc = cc,
                Bcc = bcc,
                SmtpHost = smtpHost,
                SmtpPort = smtpPort,
                SmtpUser = smtpUser,
                SmtpPass = smtpPass,
                AttachmentNames = attachments?.Select(f => f.FileName).ToList(),
                IsBulkEmail = true,
                BulkRecipients = recipients,
                TotalCount = recipients.Count
            };

            try
            {
                foreach (var recipient in recipients)
                {
                    if (string.IsNullOrWhiteSpace(recipient)) continue;

                    try
                    {
                        var mail = new MailMessage();
                        mail.From = new MailAddress(from);
                        mail.To.Add(recipient.Trim());
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
                        if (attachments != null)
                        {
                            foreach (var file in attachments)
                            {
                                if (file != null && file.Length > 0)
                                {
                                    var stream = file.OpenReadStream();
                                    var attachment = new Attachment(stream, file.FileName);
                                    mail.Attachments.Add(attachment);
                                }
                            }
                        }

                        var smtpClient = new SmtpClient(smtpHost, smtpPort)
                        {
                            Credentials = new NetworkCredential(smtpUser, smtpPass),
                            EnableSsl = true
                        };

                        smtpClient.Send(mail);
                        bulkResult.SuccessCount++;
                        Console.WriteLine($"✅ Başarılı: {recipient}");
                    }
                    catch (Exception ex)
                    {
                        bulkResult.FailedRecipients.Add(recipient.Trim());
                        Console.WriteLine($"❌ Başarısız: {recipient} - {ex.Message}");
                    }
                }

                bulkResult.OverallSuccess = bulkResult.SuccessCount > 0;
                emailHistory.IsSuccess = bulkResult.OverallSuccess;
                emailHistory.SuccessCount = bulkResult.SuccessCount;
                emailHistory.FailedRecipients = bulkResult.FailedRecipients;

                if (!bulkResult.OverallSuccess)
                {
                    emailHistory.ErrorMessage = "Tüm email gönderimları başarısız oldu.";
                }

                response.Data = bulkResult;
                response.Status = bulkResult.OverallSuccess;
                response.Message = $"Toplu e-posta gönderimi tamamlandı. Başarılı: {bulkResult.SuccessCount}/{bulkResult.TotalCount}";

                SaveEmailHistory(emailHistory);
            }
            catch (Exception ex)
            {
                bulkResult.OverallSuccess = false;
                emailHistory.IsSuccess = false;
                emailHistory.ErrorMessage = ex.Message;
                
                response.Data = bulkResult;
                response.Status = false;
                response.Message = $"Toplu e-posta gönderiminde hata: {ex.Message}";
                
                SaveEmailHistory(emailHistory);
            }

            return response;
        }

        private void SaveEmailHistory(EmailHistory emailHistory)
        {
            try
            {
                // Program çalıştığı dizinde email_history.json dosyasını oluştur/kullan
                var historyFilePath = Path.Combine(Directory.GetCurrentDirectory(), "email_history.json");
                Console.WriteLine($"=== EMAIL HISTORY KAYIT BAŞLADI ===");
                Console.WriteLine($"History dosya yolu: {historyFilePath}");
                Console.WriteLine($"Çalışma dizini: {Directory.GetCurrentDirectory()}");
                
                var history = new List<EmailHistory>();

                if (System.IO.File.Exists(historyFilePath))
                {
                    Console.WriteLine("Mevcut history dosyası bulundu, okunuyor...");
                    var jsonContent = System.IO.File.ReadAllText(historyFilePath);
                    if (!string.IsNullOrWhiteSpace(jsonContent))
                    {
                        var existingHistory = System.Text.Json.JsonSerializer.Deserialize<List<EmailHistory>>(jsonContent);
                        if (existingHistory != null)
                        {
                            history = existingHistory;
                            Console.WriteLine($"Mevcut {history.Count} kayıt bulundu");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("History dosyası bulunamadı, yeni dosya oluşturulacak");
                    // Boş JSON array dosyası oluştur
                    System.IO.File.WriteAllText(historyFilePath, "[]");
                    Console.WriteLine("Yeni boş history dosyası oluşturuldu");
                }

                history.Add(emailHistory);
                Console.WriteLine($"Yeni kayıt eklendi. Toplam kayıt sayısı: {history.Count}");
                Console.WriteLine($"Gönderen: {emailHistory.From} -> Alıcı: {emailHistory.To}");
                Console.WriteLine($"Başarı durumu: {emailHistory.IsSuccess}");
                
                var updatedJson = System.Text.Json.JsonSerializer.Serialize(history, new System.Text.Json.JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                System.IO.File.WriteAllText(historyFilePath, updatedJson);
                Console.WriteLine($"Email history başarıyla kaydedildi: {historyFilePath}");
                Console.WriteLine($"=== EMAIL HISTORY KAYIT TAMAMLANDI ===");
            }
            catch (Exception ex)
            {
                // History kaydetme hatası loglanabilir ama email gönderimini etkilemez
                Console.WriteLine($"=== EMAIL HISTORY KAYIT HATASI ===");
                Console.WriteLine($"Hata mesajı: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Console.WriteLine($"=== HATA DETAYLARI TAMAMLANDI ===");
            }
        }
    }
} 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using System.Linq;

namespace EMailSender
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly EmailSender _emailSender;

        public EmailController(EmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        /// <summary>
        /// E-posta gönderir (CC, BCC ve dosya ekleri destekler)
        /// </summary>
        [HttpPost("send")]
        [ProducesResponseType(typeof(EmailResponse<EmailSendResult>), 200)]
        [ProducesResponseType(typeof(EmailResponse<EmailSendResult>), 400)]
        [RequestSizeLimit(25_000_000)] // 25 MB
        public ActionResult<EmailResponse<EmailSendResult>> SendEmail(
            [FromForm] string smtpHost,
            [FromForm] int smtpPort,
            [FromForm] string smtpUser,
            [FromForm] string smtpPass,
            [FromForm] string from,
            [FromForm] string to,
            [FromForm] string subject,
            [FromForm] string body,
            [FromForm] List<string>? cc,
            [FromForm] List<string>? bcc,
            [FromForm] List<IFormFile>? attachments)
        {
            var result = _emailSender.SendEmail(
                smtpHost,
                smtpPort,
                smtpUser,
                smtpPass,
                from,
                to,
                subject,
                body,
                cc,
                bcc,
                attachments
            );

            if (result.Status)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        /// <summary>
        /// Toplu e-posta gönderir (birden fazla alıcıya)
        /// </summary>
        [HttpPost("bulk")]
        [ProducesResponseType(typeof(EmailResponse<BulkEmailResult>), 200)]
        [ProducesResponseType(typeof(EmailResponse<BulkEmailResult>), 400)]
        [RequestSizeLimit(25_000_000)] // 25 MB
        public ActionResult<EmailResponse<BulkEmailResult>> SendBulkEmail(
            [FromForm] string smtpHost,
            [FromForm] int smtpPort,
            [FromForm] string smtpUser,
            [FromForm] string smtpPass,
            [FromForm] string from,
            [FromForm] List<string> recipients, // List<string> olarak geri al
            [FromForm] string subject,
            [FromForm] string body,
            [FromForm] List<string>? cc,
            [FromForm] List<string>? bcc,
            [FromForm] List<IFormFile>? attachments)
        {
            var response = new EmailResponse<BulkEmailResult>();
            var bulkResult = new BulkEmailResult();
            
            if (recipients == null || !recipients.Any())
            {
                response.Data = bulkResult;
                response.Status = false;
                response.Message = "En az bir alıcı e-posta adresi gerekli.";
                return BadRequest(response);
            }

            bulkResult.TotalCount = recipients.Count;

            var result = _emailSender.SendBulkEmail(
                smtpHost,
                smtpPort,
                smtpUser,
                smtpPass,
                from,
                recipients,
                subject,
                body,
                cc,
                bcc,
                attachments
            );

            if (result.Status)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        /// <summary>
        /// API durumunu kontrol eder
        /// </summary>
        [HttpGet("health")]
        [ProducesResponseType(typeof(object), 200)]
        public ActionResult<object> HealthCheck()
        {
            return Ok(new { 
                Status = "Healthy", 
                Timestamp = DateTime.UtcNow,
                Service = "Email API"
            });
        }

        /// <summary>
        /// E-posta gönderimini zamanlar (background servisi olmadığı için çalışmıyor)
        /// </summary>
        [HttpPost("schedule(background servisi eklemedim bitmedi)")]
        [ProducesResponseType(typeof(EmailResponse<bool>), 200)]
        [ProducesResponseType(typeof(EmailResponse<bool>), 400)]
        [RequestSizeLimit(25_000_000)] // 25 MB
        public ActionResult<EmailResponse<bool>> ScheduleEmail(
            [FromForm] string smtpHost,
            [FromForm] int smtpPort,
            [FromForm] string smtpUser,
            [FromForm] string smtpPass,
            [FromForm] string from,
            [FromForm] string to,
            [FromForm] string subject,
            [FromForm] string body,
            [FromForm] DateTime sendAt,
            [FromForm] List<string>? cc,
            [FromForm] List<string>? bcc,
            [FromForm] List<IFormFile>? attachments)
        {
            var response = new EmailResponse<bool>();
            
            if (sendAt <= DateTime.UtcNow)
            {
                response.Data = false;
                response.Status = false;
                response.Message = "Gönderim zamanı gelecek bir tarih olmalıdır.";
                return BadRequest(response);
            }

            // Burada gerçek bir scheduler implementasyonu olacak
            // Şimdilik sadece başarılı mesajı döndürüyoruz
            response.Data = true;
            response.Status = true;
            response.Message = $"E-posta {sendAt:yyyy-MM-dd HH:mm:ss} zamanında gönderilecek.";
            
            return Ok(response);
        }

        /// <summary>
        /// Gönderilen e-postaların geçmişini getirir
        /// </summary>
        [HttpGet("history")]
        [ProducesResponseType(typeof(EmailHistoryResponse), 200)]
        public ActionResult<EmailHistoryResponse> GetEmailHistory()
        {
            try
            {
                var historyFilePath = Path.Combine(Directory.GetCurrentDirectory(), "email_history.json");
                var response = new EmailHistoryResponse();

                if (System.IO.File.Exists(historyFilePath))
                {
                    var jsonContent = System.IO.File.ReadAllText(historyFilePath);
                    if (!string.IsNullOrWhiteSpace(jsonContent))
                    {
                        var history = System.Text.Json.JsonSerializer.Deserialize<List<EmailHistory>>(jsonContent);
                        
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
                    System.IO.File.WriteAllText(historyFilePath, "[]");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email history okuma hatası: {ex.Message}");
                return BadRequest(new EmailHistoryResponse 
                { 
                    History = new List<EmailHistory>(),
                    TotalCount = 0,
                    SuccessCount = 0,
                    FailureCount = 0
                });
            }
        }
    }
} 
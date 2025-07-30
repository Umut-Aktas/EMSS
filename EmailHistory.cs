using System.Text.Json.Serialization;

namespace EMailSender
{
    public class EmailHistory
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public List<string>? Cc { get; set; }
        public List<string>? Bcc { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public List<string>? AttachmentNames { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; } = string.Empty;
        [JsonIgnore]
        public string SmtpPass { get; set; } = string.Empty;
        
        // Bulk email için ek alanlar
        public bool IsBulkEmail { get; set; } = false;
        public List<string>? BulkRecipients { get; set; }
        public int? SuccessCount { get; set; }
        public int? TotalCount { get; set; }
        public List<string>? FailedRecipients { get; set; }
    }

    public class EmailHistoryResponse
    {
        public List<EmailHistory> History { get; set; } = new List<EmailHistory>();
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
    }
}
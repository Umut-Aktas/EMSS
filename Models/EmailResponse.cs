namespace EMailSender
{
    public class EmailResponse<T>
    {
        public T Data { get; set; } = default!;
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class EmailSendResult
    {
        public bool Success { get; set; }
        public string EmailId { get; set; } = string.Empty;
    }

    public class BulkEmailResult
    {
        public bool OverallSuccess { get; set; }
        public int SuccessCount { get; set; }
        public int TotalCount { get; set; }
        public string BulkEmailId { get; set; } = string.Empty;
        public List<string> FailedRecipients { get; set; } = new List<string>();
    }
} 
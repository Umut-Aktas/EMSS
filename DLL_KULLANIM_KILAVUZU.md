# EmailSender Library - DLL Kullanım Kılavuzu

Bu proje, email gönderimi için hazırlanmış bir ASP.NET Core Web API projesidir. Aynı zamanda **EmailHelper** sınıfı sayesinde DLL olarak başka projelerde de kullanılabilir.

## 🚀 DLL Olarak Kullanım

### 0. Default Email Ayarları (Önerilen - Hızlı Kullanım İçin)
DLL'i kullanmadan önce, launchSettings.json dosyasında default email ayarlarınızı yapılandırabilirsiniz:

```json
{
  "profiles": {
    "http": {
      "environmentVariables": {
        "DEFAULT_EMAIL": "your-email@gmail.com",
        "DEFAULT_EMAIL_PASSWORD": "your-app-password",
        "DEFAULT_SMTP_HOST": "smtp.gmail.com",
        "DEFAULT_SMTP_PORT": "587"
      }
    }
  }
}
```

Bu ayarları yaptıktan sonra `SendEmailQuick()` ve `SendBulkEmailQuick()` fonksiyonlarını kullanarak sadece alıcı, konu ve içerik belirterek hızlıca email gönderebilirsiniz.

### 1. DLL Dosyasını Kopyalama
Projeyi build ettikten sonra `bin/Debug/net8.0/EMailSender.dll` dosyasını hedef projenize kopyalayın.

### 2. Referans Ekleme
Hedef projenizde DLL'e referans ekleyin:

```xml
<ItemGroup>
    <Reference Include="EMailSender">
        <HintPath>path\to\EMailSender.dll</HintPath>
    </Reference>
</ItemGroup>
```

### 3. Using Direktifi
```csharp
using EMailSender;
```

## 📚 Mevcut Fonksiyonlar

### 1. Help() - Yardım Bilgileri
```csharp
EmailHelper.Help();
```
Konsola kullanım kılavuzunu yazdırır.

### 2a. SendEmailQuick() - Hızlı Tek Email Gönderme (ÖNERİLEN)
```csharp
// Sadece alıcı, konu ve içerik belirtmeniz yeterli
var result = EmailHelper.SendEmailQuick(
    to: "receiver@example.com",
    subject: "Test Subject",
    body: "Email content",
    cc: new List<string> { "cc@example.com" },          // Opsiyonel
    bcc: new List<string> { "bcc@example.com" },        // Opsiyonel
    attachmentPaths: new List<string> { "file.pdf" }    // Opsiyonel
);

if (result.Status)
{
    Console.WriteLine($"Email başarıyla gönderildi. ID: {result.Data.EmailId}");
}
else
{
    Console.WriteLine($"Hata: {result.Message}");
}
```

### 2b. SendEmail() - Manuel Tek Email Gönderme
```csharp
var result = EmailHelper.SendEmail(
    smtpHost: "smtp.gmail.com",
    smtpPort: 587,
    smtpUser: "your-email@gmail.com",
    smtpPass: "your-app-password",
    from: "sender@example.com",
    to: "receiver@example.com",
    subject: "Test Subject",
    body: "Email content",
    cc: new List<string> { "cc@example.com" },          // Opsiyonel
    bcc: new List<string> { "bcc@example.com" },        // Opsiyonel
    attachmentPaths: new List<string> { "file.pdf" }    // Opsiyonel
);

if (result.Status)
{
    Console.WriteLine($"Email başarıyla gönderildi. ID: {result.Data.EmailId}");
}
else
{
    Console.WriteLine($"Hata: {result.Message}");
}
```

### 3a. SendBulkEmailQuick() - Hızlı Toplu Email Gönderme (ÖNERİLEN)
```csharp
var recipients = new List<string> 
{ 
    "user1@example.com", 
    "user2@example.com", 
    "user3@example.com" 
};

var bulkResult = EmailHelper.SendBulkEmailQuick(
    recipients: recipients,
    subject: "Bulk Email Test",
    body: "This is a bulk email test."
);

if (bulkResult.Status)
{
    Console.WriteLine($"Toplu email tamamlandı. Başarılı: {bulkResult.Data.SuccessCount}/{bulkResult.Data.TotalCount}");
}
```

### 3b. SendBulkEmail() - Manuel Toplu Email Gönderme
```csharp
var recipients = new List<string> 
{ 
    "user1@example.com", 
    "user2@example.com", 
    "user3@example.com" 
};

var bulkResult = EmailHelper.SendBulkEmail(
    smtpHost: "smtp.gmail.com",
    smtpPort: 587,
    smtpUser: "your-email@gmail.com",
    smtpPass: "your-app-password",
    from: "sender@example.com",
    recipients: recipients,
    subject: "Bulk Email Test",
    body: "This is a bulk email test."
);

if (bulkResult.Status)
{
    Console.WriteLine($"Toplu email tamamlandı. Başarılı: {bulkResult.Data.SuccessCount}/{bulkResult.Data.TotalCount}");
}
```

### 4. GetEmailHistory() - Email Geçmişi
```csharp
var history = EmailHelper.GetEmailHistory();
Console.WriteLine($"Toplam email: {history.TotalCount}");
Console.WriteLine($"Başarılı: {history.SuccessCount}");
Console.WriteLine($"Başarısız: {history.FailureCount}");

foreach (var email in history.History)
{
    Console.WriteLine($"[{email.Timestamp}] {email.From} -> {email.To}: {email.Subject} ({(email.IsSuccess ? "Başarılı" : "Başarısız")})");
}
```

### 5. CheckHealth() - Sistem Durumu
```csharp
var health = EmailHelper.CheckHealth();
Console.WriteLine($"Sistem durumu: {health}");
```

## 🔧 Gmail SMTP Ayarları

Gmail kullanmak için:
1. Gmail hesabınızda 2-Factor Authentication'ı aktifleştirin
2. Uygulama şifresi oluşturun (App Password)
3. Ayarlar:
   - **SMTP Host:** smtp.gmail.com
   - **Port:** 587
   - **Username:** your-email@gmail.com
   - **Password:** your-app-password (normal şifre değil!)

## 📝 Örnek Kullanım

```csharp
using EMailSender;
using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        // Yardım bilgilerini göster
        EmailHelper.Help();
        
        // HIZLI KULLANIM (Önerilen)
        // launchSettings.json'da default ayarlar yapılandırılmışsa:
        var quickResult = EmailHelper.SendEmailQuick(
            "receiver@example.com", 
            "Test Email", 
            "Bu bir test emailidir."
        );
        
        if (quickResult.Status)
        {
            Console.WriteLine("Email başarıyla gönderildi!");
        }
        else
        {
            Console.WriteLine($"Hata: {quickResult.Message}");
        }
        
        // MANUEL KULLANIM
        // Tüm SMTP ayarlarını kendiniz belirtmek isterseniz:
        var manualResult = EmailHelper.SendEmail(
            "smtp.gmail.com", 587,
            "your-email@gmail.com", "your-app-password",
            "sender@example.com", "receiver@example.com",
            "Test Email", "Bu bir test emailidir."
        );
        
        // Geçmişi kontrol et
        var history = EmailHelper.GetEmailHistory();
        Console.WriteLine($"Şu ana kadar {history.TotalCount} email gönderildi.");
    }
}
```

## 📂 Dosya Ekleri

Dosya ekleri için dosya yollarını `List<string>` olarak geçin:

```csharp
var attachments = new List<string> 
{ 
    @"C:\path\to\file1.pdf",
    @"C:\path\to\file2.jpg",
    @"C:\path\to\document.docx"
};

var result = EmailHelper.SendEmail(
    // ... diğer parametreler
    attachmentPaths: attachments
);
```

## 🚨 Önemli Notlar

1. **Güvenlik:** SMTP şifrelerini kodda saklamayın, configuration dosyalarından okuyun
2. **Rate Limiting:** Gmail günlük 500 email limiti vardır
3. **Error Handling:** Tüm email işlemlerini try-catch ile sarın
4. **Dosya Boyutu:** Ek dosyalar için 25MB limit vardır
5. **History:** Email geçmişi `email_history.json` dosyasında saklanır

## 🔄 Web API Endpoint'leri

Bu library aynı zamanda Web API olarak da çalışır:

- **POST** `/api/email/send` - Tek email gönder
- **POST** `/api/email/bulk` - Toplu email gönder  
- **GET** `/api/email/history` - Email geçmişi
- **GET** `/api/email/health` - Sistem durumu
- **GET** `/test-emailhelper` - EmailHelper örneklerini çalıştır

## 📞 Destek

Herhangi bir sorun için kod içindeki `EmailHelper.Help()` fonksiyonunu kullanarak detaylı kullanım bilgilerine ulaşabilirsiniz.

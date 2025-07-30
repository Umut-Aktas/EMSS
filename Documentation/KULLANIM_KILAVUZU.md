# 📧 EmailSender - Kapsamlı Kullanım Kılavuzu

## 📋 İçindekiler
1. [Genel Bakış](#genel-bakış)
2. [Hızlı Başlangıç](#hızlı-başlangıç)
3. [Kurulum](#kurulum)
4. [Gmail Ayarları](#gmail-ayarları)
5. [DLL Kullanımı](#dll-kullanımı)
6. [Web API Kullanımı](#web-api-kullanımı)
7. [Fonksiyon Referansı](#fonksiyon-referansı)
8. [Kullanım Örnekleri](#kullanım-örnekleri)
9. [Sorun Giderme](#sorun-giderme)
10. [Limitler ve Güvenlik](#limitler-ve-güvenlik)
11. [Lisans](#lisans)

---

## 🎯 Genel Bakış

EmailSender, .NET 8 tabanlı, kolay kullanımlı ve güvenli email gönderimi için tasarlanmış bir kütüphanedir. Hem DLL olarak hem de Web API olarak kullanılabilir.

### ✨ Özellikler
- ✅ **Hızlı Email Gönderimi** - 3 satır kod ile email gönder
- ✅ **Toplu Email Desteği** - Birden fazla alıcıya aynı anda
- ✅ **Dosya Eki Desteği** - 25MB'a kadar dosya ekleri
- ✅ **CC/BCC Desteği** - Kopyalı gönderimler
- ✅ **Email Geçmişi** - Otomatik kayıt ve takip
- ✅ **Web API & DLL** - İki farklı kullanım seçeneği
- ✅ **Gmail Entegrasyonu** - Gmail SMTP desteği
- ✅ **Güvenli Yapılandırma** - AccountInfos.json ile ayar
- ✅ **Hata Yönetimi** - Kapsamlı hata takibi

---

## 🚀 Hızlı Başlangıç

```csharp
using EMailSender;

// Önce email hesabınızı kaydedin (sadece bir kez)
EmailHelper.SaveMailAccount("your-email@gmail.com", "your-app-password");

// Tek satırda email gönderme
var result = EmailHelper.SendEmailQuick(
    "receiver@example.com", 
    "Test Subject", 
    "Email content"
);

if (result.Status)
{
    Console.WriteLine("Email gönderildi!");
}
else
{
    Console.WriteLine($"Hata: {result.Message}");
}
```

---

## ⚡ Kurulum

### 1. DLL Kurulumu (Önerilen)

#### Adım 1: Projeyi Build Edin
```bash
dotnet build
```

#### Adım 2: DLL Dosyasını Kopyalayın
`bin/Debug/net8.0/EMailSender.dll` dosyasını hedef projenize kopyalayın.

#### Adım 3: Referans Ekleyin
Hedef projenizde DLL'e referans ekleyin:

```xml
<ItemGroup>
    <Reference Include="EMailSender">
        <HintPath>path\to\EMailSender.dll</HintPath>
    </Reference>
</ItemGroup>
```

#### Adım 4: Using Direktifi
```csharp
using EMailSender;
```

### 2. Web API Kurulumu

```bash
# Projeyi çalıştır
dotnet run

# Swagger UI: http://localhost:5000/swagger
```

---

## 🔧 Gmail Ayarları

### 1. 2-Factor Authentication Aktifleştirme
1. Gmail hesabınızda **2-Factor Authentication**'ı aktifleştirin
2. Google Account Settings > Security > 2-Step Verification

### 2. App Password Oluşturma
1. Google Account Settings > Security > App passwords
2. "Mail" için yeni bir app password oluşturun
3. Oluşan 16 haneli şifreyi not alın

### 3. Email Hesabı Kaydetme

EmailHelper'ı kullanmadan önce email hesabınızı kaydetmeniz gerekmektedir:

```csharp
// Sadece bir kez çalıştırın
EmailHelper.SaveMailAccount("your-email@gmail.com", "your-16-digit-app-password");
```

Bu işlem `AccountInfos.json` dosyası oluşturacak ve email bilgilerinizi güvenli bir şekilde saklayacaktır.

**ÖNEMLİ:** 
- `your-email@gmail.com` yerine gerçek Gmail adresinizi yazın
- `your-16-digit-app-password` yerine Gmail App Password'ünüzü yazın (normal şifre değil!)
- Bu işlemi sadece bir kez yapmanız yeterlidir
- Eğer email hesabınızı kaydetmezseniz quick methodlarına erişemezsiniz

---

## 📚 DLL Kullanımı

### Hızlı Fonksiyonlar (Önerilen)

#### 1. SendEmailQuick() - Hızlı Tek Email
```csharp
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

#### 2. SendBulkEmailQuick() - Hızlı Toplu Email
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

### Manuel Fonksiyonlar

#### 1. SendEmail() - Manuel Tek Email
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
```

#### 2. SendBulkEmail() - Manuel Toplu Email
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
```

### Yardımcı Fonksiyonlar

#### 1. GetEmailHistory() - Email Geçmişi
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

#### 2. CheckHealth() - Sistem Durumu
```csharp
var health = EmailHelper.CheckHealth();
Console.WriteLine($"Sistem durumu: {health}");
```

#### 3. Help() - Yardım Bilgileri
```csharp
EmailHelper.Help();
```

#### 4. SaveMailAccount() - Email Hesabı Kaydetme
```csharp
// Email hesabınızı kaydetmek için (sadece bir kez)
EmailHelper.SaveMailAccount("your-email@gmail.com", "your-app-password");
```

---

## 🌐 Web API Kullanımı

### Endpoint'ler

- **POST** `/api/email/send` - Tek email gönder
- **POST** `/api/email/bulk` - Toplu email gönder  
- **GET** `/api/email/history` - Email geçmişi
- **GET** `/api/email/health` - Sistem durumu
- **GET** `/test-emailhelper` - EmailHelper örneklerini çalıştır

### Örnek API Kullanımı

#### Tek Email Gönderme
```bash
curl -X POST "http://localhost:5000/api/email/send" \
  -H "Content-Type: application/json" \
  -d '{
    "to": "receiver@example.com",
    "subject": "Test Email",
    "body": "This is a test email"
  }'
```

#### Toplu Email Gönderme
```bash
curl -X POST "http://localhost:5000/api/email/bulk" \
  -H "Content-Type: application/json" \
  -d '{
    "recipients": ["user1@example.com", "user2@example.com"],
    "subject": "Bulk Email",
    "body": "This is a bulk email"
  }'
```

---

## 📋 Fonksiyon Referansı

### Hızlı Fonksiyonlar (Önerilen)
```csharp
// Email hesabı kaydetme (sadece bir kez)
EmailHelper.SaveMailAccount(email, password)

// Tek email
EmailHelper.SendEmailQuick(to, subject, body, cc?, bcc?, attachments?)

// Toplu email
EmailHelper.SendBulkEmailQuick(recipients, subject, body, cc?, bcc?, attachments?)

// Email geçmişi
EmailHelper.GetEmailHistory()

// Sistem durumu
EmailHelper.CheckHealth()

// Yardım bilgileri
EmailHelper.Help()
```

### Manuel Fonksiyonlar
```csharp
// Tüm SMTP ayarlarını manuel belirtin
EmailHelper.SendEmail(host, port, user, pass, from, to, subject, body, ...)
EmailHelper.SendBulkEmail(host, port, user, pass, from, recipients, subject, body, ...)
```

---

## 💡 Kullanım Örnekleri

### 1. İlk Kurulum
```csharp
// İlk kullanımda email hesabınızı kaydedin
EmailHelper.SaveMailAccount("your-email@gmail.com", "your-app-password");
```

### 2. Basit Email
```csharp
var result = EmailHelper.SendEmailQuick(
    "user@example.com", 
    "Merhaba!", 
    "Bu bir test emailidir."
);
```

### 3. Dosya Ekli Email
```csharp
var attachments = new List<string> { @"C:\path\to\file.pdf" };
var result = EmailHelper.SendEmailQuick(
    "user@example.com", 
    "Dosya Eki", 
    "Ekte dosya bulunmaktadır.",
    attachmentPaths: attachments
);
```

### 4. CC/BCC'li Email
```csharp
var cc = new List<string> { "cc@example.com" };
var bcc = new List<string> { "bcc@example.com" };

var result = EmailHelper.SendEmailQuick(
    "user@example.com", 
    "CC/BCC Test", 
    "Bu email CC ve BCC ile gönderildi.",
    cc: cc,
    bcc: bcc
);
```

### 5. Toplu Email
```csharp
var recipients = new List<string> { "user1@example.com", "user2@example.com" };
var result = EmailHelper.SendBulkEmailQuick(
    recipients, 
    "Toplu Email", 
    "Bu email birden fazla kişiye gönderildi."
);
```

### 6. Tam Örnek Program
```csharp
using EMailSender;
using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        // İlk kullanımda email hesabınızı kaydedin (sadece bir kez)
        EmailHelper.SaveMailAccount("your-email@gmail.com", "your-app-password");
        
        // Yardım bilgilerini göster
        EmailHelper.Help();
        
        // HIZLI KULLANIM (Önerilen)
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
        
        // Toplu email gönderimi
        var recipients = new List<string> { "user1@example.com", "user2@example.com" };
        var bulkResult = EmailHelper.SendBulkEmailQuick(
            recipients, 
            "Toplu Email", 
            "Bu email birden fazla kişiye gönderildi."
        );
        
        // Geçmişi kontrol et
        var history = EmailHelper.GetEmailHistory();
        Console.WriteLine($"Şu ana kadar {history.TotalCount} email gönderildi.");
    }
}
```

---

## 🔍 Sorun Giderme

### "AccountInfos.json file not found" Hatası
- `EmailHelper.SaveMailAccount()` fonksiyonunu çağırdığınızdan emin olun
- `AccountInfos.json` dosyasının proje dizininde oluşturulduğunu kontrol edin
- Email ve şifre bilgilerinin doğru olduğundan emin olun

### "Mail address or password not found" Hatası
- `SaveMailAccount()` fonksiyonunu tekrar çağırın
- `AccountInfos.json` dosyasının içeriğini kontrol edin
- Dosya formatının doğru olduğundan emin olun

### Gmail Authentication Hatası
- 2-Factor Authentication aktif mi kontrol edin
- App Password kullandığınızdan emin olun (normal şifre değil!)
- SMTP ayarları: `smtp.gmail.com:587`

### Performans Sorunları
- Toplu email için `SendBulkEmailQuick()` kullanın
- Gmail günlük limitini aşmayın (500 email)

### Dosya Eki Sorunları
- Dosya boyutu 25MB'ı aşmamalı
- Dosya yolu doğru olmalı
- Dosya erişim izinleri kontrol edin

---

## 📊 Limitler ve Güvenlik

### Gmail Limitleri
- **Günlük Email:** 500 email (ücretsiz hesap)
- **Dosya Boyutu:** 25MB maksimum
- **Toplu Email:** Önerilen batch size: 50

### Error Handling
```csharp
try
{
    var result = EmailHelper.SendEmailQuick("user@example.com", "Test", "Content");
    if (result.Status)
    {
        Console.WriteLine("Başarılı!");
    }
    else
    {
        Console.WriteLine($"Hata: {result.Message}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Beklenmeyen hata: {ex.Message}");
}
```

---

## 📞 Destek

- 💻 **Kod İçi Yardım:** `EmailHelper.Help()` fonksiyonunu çağırın
- 📖 **Detaylı Dokümantasyon:** Bu kılavuz
- 🐛 **Sorun Bildirimi:** GitHub Issues kullanın

---

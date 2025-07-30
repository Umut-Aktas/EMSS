# 📧 EmailSender - .NET Email Library & Web API

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![C#](https://img.shields.io/badge/Language-C%23-purple.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)

Kolay kullanımlı, hızlı ve güvenli email gönderimi için tasarlanmış .NET 8 tabanlı kütüphane ve Web API. Hem DLL olarak hem de Web API olarak kullanılabilir.

## 🚀 Özellikler

- ✅ **Hızlı Email Gönderimi** - 3 satır kod ile email gönder
- ✅ **Toplu Email Desteği** - Birden fazla alıcıya aynı anda
- ✅ **Dosya Eki Desteği** - 25MB'a kadar dosya ekleri
- ✅ **CC/BCC Desteği** - Kopyalı gönderimler
- ✅ **Email Geçmişi** - Otomatik kayıt ve takip
- ✅ **Web API & DLL** - İki farklı kullanım seçeneği
- ✅ **Gmail Entegrasyonu** - Gmail SMTP desteği
- ✅ **Güvenli Yapılandırma** - Environment variables ile ayar
- ✅ **Hata Yönetimi** - Kapsamlı hata takibi

## 📦 Hızlı Başlangıç

### 1. DLL Kullanımı (Önerilen)

```csharp
using EMailSender;

// Hızlı email gönderimi (Default ayarları kullanır)
var result = EmailHelper.SendEmailQuick(
    "receiver@example.com", 
    "Test Subject", 
    "Email content"
);

if (result.Status)
{
    Console.WriteLine("Email başarıyla gönderildi!");
}
```

### 2. Web API Kullanımı

```bash
# Projeyi çalıştır
dotnet run

# Swagger UI: http://localhost:5000/swagger
```

## ⚡ Kurulum

### DLL İçin

1. Projeyi build edin:
   ```bash
   dotnet build
   ```

2. `bin/Debug/net8.0/EMailSender.dll` dosyasını projenize kopyalayın

3. DLL referansını ekleyin:
   ```xml
   <ItemGroup>
       <Reference Include="EMailSender">
           <HintPath>path\to\EMailSender.dll</HintPath>
       </Reference>
   </ItemGroup>
   ```

4. Gmail ayarlarınızı yapılandırın (örnek dosyayı kopyalayın):
   ```bash
   cp Properties/launchSettings.example.json Properties/launchSettings.json
   ```

5. `launchSettings.json` dosyasında email bilgilerinizi güncelleyin

## 🔧 Yapılandırma

### Gmail Ayarları

1. **2-Factor Authentication** aktifleştirin
2. **App Password** oluşturun ([Buradan](https://support.google.com/accounts/answer/185833))
3. `Properties/launchSettings.json` dosyasını düzenleyin:

```json
{
  "profiles": {
    "Default": {
      "environmentVariables": {
        "DEFAULT_EMAIL": "your-email@gmail.com",
        "DEFAULT_EMAIL_PASSWORD": "your-16-digit-app-password",
        "DEFAULT_SMTP_HOST": "smtp.gmail.com",
        "DEFAULT_SMTP_PORT": "587"
      }
    }
  }
}
```

## 📋 API Referansı

### Hızlı Fonksiyonlar (Önerilen)

```csharp
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

## 🌐 Web API Endpoints

- `POST /api/email/send` - Tek email gönder
- `POST /api/email/bulk` - Toplu email gönder  
- `GET /api/email/history` - Email geçmişi
- `GET /api/email/health` - Sistem durumu
- `GET /test-emailhelper` - EmailHelper test örnekleri

## 📁 Dosya Yapısı

```
├── EmailHelper.cs              # Ana DLL sınıfı
├── EmailController.cs          # Web API controller
├── EmailSender.cs             # Core email logic
├── EmailResponse.cs           # Response modelleri
├── EmailHistory.cs            # Geçmiş modelleri
├── Program.cs                 # Web API startup
├── DLL_KULLANIM_KILAVUZU.md   # Detaylı DLL kılavuzu
├── HIZLI_KURULUM.md           # 3 dakikada kurulum
└── Properties/
    ├── launchSettings.json        # Gerçek ayarlar (git'te yok)
    └── launchSettings.example.json # Örnek template
```

## 💡 Kullanım Örnekleri

### Basit Email

```csharp
var result = EmailHelper.SendEmailQuick(
    "user@example.com", 
    "Merhaba!", 
    "Bu bir test emailidir."
);
```

### Dosya Ekli Email

```csharp
var attachments = new List<string> { @"C:\path\to\file.pdf" };
var result = EmailHelper.SendEmailQuick(
    "user@example.com", 
    "Dosya Eki", 
    "Ekte dosya bulunmaktadır.",
    attachmentPaths: attachments
);
```

### Toplu Email

```csharp
var recipients = new List<string> { "user1@example.com", "user2@example.com" };
var result = EmailHelper.SendBulkEmailQuick(
    recipients, 
    "Toplu Email", 
    "Bu email birden fazla kişiye gönderildi."
);
```

## 🔍 Sorun Giderme

### "Default email ayarları yapılandırılmamış" Hatası
- `Properties/launchSettings.json` dosyasını kontrol edin
- `DEFAULT_EMAIL` ve `DEFAULT_EMAIL_PASSWORD` ayarlandığından emin olun

### Gmail Authentication Hatası
- 2-Factor Authentication aktif olmalı
- Normal şifre değil, App Password kullanın
- SMTP ayarları: `smtp.gmail.com:587`

## 📊 Limitler

- **Gmail:** Günde 500 email (ücretsiz hesap)
- **Dosya Boyutu:** 25MB maksimum
- **Toplu Email:** Önerilen batch size: 50

## 🤝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/awesome-feature`)
3. Commit edin (`git commit -m 'Add awesome feature'`)
4. Push edin (`git push origin feature/awesome-feature`)
5. Pull Request açın

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

## 📞 Destek

- 📖 **Detaylı Dokümantasyon:** [DLL_KULLANIM_KILAVUZU.md](DLL_KULLANIM_KILAVUZU.md)
- ⚡ **Hızlı Kurulum:** [HIZLI_KURULUM.md](HIZLI_KURULUM.md)
- 💻 **Kod İçi Yardım:** `EmailHelper.Help()` fonksiyonunu çağırın

---

⭐ Bu projeyi beğendiyseniz star vermeyi unutmayın!

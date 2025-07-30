# EmailSender DLL - Hızlı Kurulum Kılavuzu

## 🎯 Hızlı Başlangıç (3 Dakikada Email Gönderme)

### 1. DLL'i Projenize Ekleyin
```xml
<ItemGroup>
    <Reference Include="EMailSender">
        <HintPath>path\to\EMailSender.dll</HintPath>
    </Reference>
</ItemGroup>
```

### 2. launchSettings.json Dosyası Oluşturun
Proje klasörünüzde `Properties/launchSettings.json` dosyası oluşturun:

```json
{
  "profiles": {
    "Default": {
      "commandName": "Project",
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

**ÖNEMLİ:** 
- `your-email@gmail.com` yerine gerçek Gmail adresinizi yazın
- `your-app-password` yerine Gmail App Password'ünüzü yazın (normal şifre değil!)

### 3. Gmail App Password Oluşturma
1. Gmail hesabınızda **2-Factor Authentication**'ı aktifleştirin
2. Google Account Settings > Security > App passwords
3. "Mail" için yeni bir app password oluşturun
4. Oluşan 16 haneli şifreyi `DEFAULT_EMAIL_PASSWORD` kısmına yazın

### 4. Hızlı Email Gönderme Kodu
```csharp
using EMailSender;

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

### 5. Toplu Email Gönderme
```csharp
var recipients = new List<string> { "user1@example.com", "user2@example.com" };

var bulkResult = EmailHelper.SendBulkEmailQuick(
    recipients, 
    "Bulk Email", 
    "Bulk content"
);

Console.WriteLine($"Gönderildi: {bulkResult.Data.SuccessCount}/{bulkResult.Data.TotalCount}");
```

## 🔧 Avantajlar

### ✅ Hızlı Fonksiyonlar (Quick)
- `SendEmailQuick()` - Sadece alıcı, konu, içerik belirtmeniz yeterli
- `SendBulkEmailQuick()` - Toplu email için minimum parametre
- Default ayarları launchSettings.json'dan otomatik alır
- SMTP detayları ile uğraşmanıza gerek yok

### ✅ Manuel Fonksiyonlar
- `SendEmail()` - Tüm SMTP ayarlarını kendiniz belirtin
- `SendBulkEmail()` - Farklı SMTP sunucuları için esnek kullanım
- Her email için farklı ayarlar kullanabilirsiniz

### ✅ Ek Özellikler
- Otomatik email geçmiş kaydı
- Dosya eki desteği
- CC/BCC desteği
- Hata yönetimi
- Sistem durumu kontrolü

## 🚨 Önemli Notlar

1. **Güvenlik:** launchSettings.json dosyasını Git'e commit etmeyin
2. **Gmail Limiti:** Günde 500 email limiti vardır
3. **App Password:** Normal Gmail şifresi çalışmaz, App Password gereklidir
4. **Dosya Boyutu:** Ekler için 25MB limit vardır

## 📞 Sorun Giderme

### "Default email ayarları yapılandırılmamış" Hatası
- launchSettings.json dosyasında `DEFAULT_EMAIL` ve `DEFAULT_EMAIL_PASSWORD` ayarlarını kontrol edin
- Environment variable'ların doğru yüklendiğinden emin olun

### Gmail Authentication Hatası
- 2-Factor Authentication aktif mi kontrol edin
- App Password kullandığınızdan emin olun
- SMTP ayarları: smtp.gmail.com:587

### Performans
- Toplu email için `SendBulkEmailQuick()` kullanın
- Çok fazla alıcı varsa bölümlere ayırın (örn: 50'şer)

Bu ayarları tamamladıktan sonra sadece 3 satır kod ile email gönderebilirsiniz! 🚀

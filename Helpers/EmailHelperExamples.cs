using EMailSender;
using System;
using System.Collections.Generic;

namespace EMailSender
{
    /// <summary>
    /// EmailHelper sınıfının kullanım örneklerini gösteren test sınıfı
    /// </summary>
    public static class EmailHelperExamples
    {
        public static void RunExamples()
        {
            Console.WriteLine("EmailHelper Kullanım Örnekleri");
            Console.WriteLine("==============================");
            
            // 1. Yardım bilgilerini göster
            Console.WriteLine("\n1. Yardım bilgilerini gösteriliyor:");
            EmailHelper.Help();
            
            // 2. Sistem durumunu kontrol et
            Console.WriteLine("\n2. Sistem durumu kontrol ediliyor:");
            var health = EmailHelper.CheckHealth();
            Console.WriteLine($"Durum: {health}");
            
            // 3. E-posta geçmişini getir
            Console.WriteLine("\n3. E-posta geçmişi getiriliyor:");
            var history = EmailHelper.GetEmailHistory();
            Console.WriteLine($"Toplam e-posta: {history.TotalCount}");
            Console.WriteLine($"Başarılı: {history.SuccessCount}");
            Console.WriteLine($"Başarısız: {history.FailureCount}");
            
            // 4. Tek e-posta gönderme örneği (test amaçlı - gerçek bilgilerle değiştirin)
            Console.WriteLine("\n4. Hızlı tek e-posta gönderme örneği:");
            Console.WriteLine("(Default ayarları kullanır - launchSettings.json'da ayarlanmalı)");
            
            /*
            // Hızlı kullanım örneği (sadece alıcı, konu ve içerik):
            var quickResult = EmailHelper.SendEmailQuick(
                "recipient@example.com",    // Kime
                "Hızlı Test Konusu",        // Konu
                "Bu hızlı test e-postasıdır." // İçerik
            );
            
            if (quickResult.Status)
            {
                Console.WriteLine($"Hızlı e-posta başarıyla gönderildi. ID: {quickResult.Data.EmailId}");
            }
            else
            {
                Console.WriteLine($"Hızlı e-posta gönderim hatası: {quickResult.Message}");
            }
            */
            
            Console.WriteLine("\n4b. Manuel tek e-posta gönderme örneği:");
            Console.WriteLine("(Gerçek SMTP bilgileri gerekli - bu sadece örnek)");
            
            /*
            // Gerçek kullanım örneği (yorumda):
            var result = EmailHelper.SendEmail(
                "smtp.gmail.com",           // SMTP sunucu
                587,                        // Port
                "your-email@gmail.com",     // Gönderen e-posta
                "your-app-password",        // Uygulama şifresi
                "your-email@gmail.com",     // Kimden
                "recipient@example.com",    // Kime
                "Test Konusu",              // Konu
                "Bu bir test e-postasıdır." // İçerik
            );
            
            if (result.Status)
            {
                Console.WriteLine($"E-posta başarıyla gönderildi. ID: {result.Data.EmailId}");
            }
            else
            {
                Console.WriteLine($"E-posta gönderim hatası: {result.Message}");
            }
            */
            
            // 5. Toplu e-posta gönderme örneği
            Console.WriteLine("\n5. Hızlı toplu e-posta gönderme örneği:");
            Console.WriteLine("(Default ayarları kullanır - launchSettings.json'da ayarlanmalı)");
            
            /*
            // Hızlı toplu e-posta örneği:
            var recipients = new List<string> 
            { 
                "user1@example.com", 
                "user2@example.com", 
                "user3@example.com" 
            };
            
            var quickBulkResult = EmailHelper.SendBulkEmailQuick(
                recipients,
                "Hızlı Toplu E-posta Testi",
                "Bu hızlı toplu e-posta testidir."
            );
            
            if (quickBulkResult.Status)
            {
                Console.WriteLine($"Hızlı toplu e-posta tamamlandı. Başarılı: {quickBulkResult.Data.SuccessCount}/{quickBulkResult.Data.TotalCount}");
            }
            */
            
            Console.WriteLine("\n5b. Manuel toplu e-posta gönderme örneği:");
            Console.WriteLine("(Gerçek SMTP bilgileri gerekli - bu sadece örnek)");
            
            /*
            // Gerçek kullanım örneği (yorumda):
            var recipients = new List<string> 
            { 
                "user1@example.com", 
                "user2@example.com", 
                "user3@example.com" 
            };
            
            var bulkResult = EmailHelper.SendBulkEmail(
                "smtp.gmail.com",
                587,
                "your-email@gmail.com",
                "your-app-password",
                "your-email@gmail.com",
                recipients,
                "Toplu E-posta Testi",
                "Bu toplu e-posta testidir."
            );
            
            if (bulkResult.Status)
            {
                Console.WriteLine($"Toplu e-posta tamamlandı. Başarılı: {bulkResult.Data.SuccessCount}/{bulkResult.Data.TotalCount}");
            }
            */
            
            Console.WriteLine("\n==============================");
            Console.WriteLine("Örnekler tamamlandı!");
            Console.WriteLine("Gerçek kullanım için yukarıdaki yorumları açın ve kendi SMTP bilgilerinizi ekleyin.");
        }
    }
}

# Afet Yönetim Sistemi

Deprem ve afet anlarında afetzede, gönüllü ve koordinatörler arasında köprü kuran merkezi dijital koordinasyon platformu. ASP.NET Core MVC mimarisi, rol bazlı yetkilendirme, harita entegrasyonu ve audit log altyapısı ile gerçek bir afet senaryosunda kullanılabilir bir yapı sunar.

> **Bu proje Muğla Sıtkı Koçman Üniversitesi Web Programlama dersi final projesi olarak Betül Filiz tarafından geliştirilmiştir.**

---

## Özellikler

### Üç Rol, Tam Çalışan Akış
- **Admin**: Talepleri yönetir, gönüllü atar, duyuru yayınlar, bölge yönetir, audit log inceler.
- **Afetzede**: Haritadan konum seçerek yardım talebi oluşturur, durum takibi yapar, talebini iptal edebilir.
- **Gönüllü**: Atanan görevleri görür, durum günceller, Google Maps üzerinden yol tarifi alır.

### Görsel ve Etkileşimli Dashboard
- Leaflet Harita Entegrasyonu (Aciliyet renkli pin'ler ve durum filtreleri).
- Dinamik Grafikler (Chart.js): Kategori dağılımı, 7 günlük trend, bölge bazlı analiz.
- Canlı İşlem Akışı (Timeline): Sistemdeki tüm hareketlerin anlık izlenmesi.

### Güvenlik ve İzlenebilirlik
- **Audit Log**: Kritik tüm işlemlerin (onay, atama, teslim) kim tarafından ne zaman yapıldığının kaydı.
- **Kimlik Doğrulama**: ASP.NET Core Identity ile güvenli rol bazlı yetkilendirme.
- **Şifre Yönetimi**: Tüm kullanıcılar için profil güncelleme ve şifre değiştirme ekranları.

---

## Teknoloji Yığını
- **Framework:** ASP.NET Core MVC (.NET 9)
- **Veritabanı:** MS SQL Server (Docker)
- **Harita:** Leaflet 1.9.4 & OpenStreetMap
- **UI:** AdminLTE 3 & Bootstrap 4
- **Grafik:** Chart.js 4.4

---

## Kurulum ve Çalıştırma

```bash
# 1. Proje dizinine gir
cd AfetYonetim

# 2. Docker container'larını başlat (SQL Server)
docker compose up -d

# 3. Veritabanını oluştur ve örnek verileri (Seed) yükle
dotnet ef database update

# 4. Uygulamayı çalıştır
dotnet run
```

---

## Demo Hesaplar
| Rol | E-posta | Şifre |
|-----|---------|-------|
| Admin | admin@afet.gov.tr | Admin123! |
| Afetzede | ayse@example.com | Ayse123! |
| Gönüllü | mehmet@example.com | Mehmet123! |

---

## Yazar
**Betül Filiz** - Muğla Sıtkı Koçman Üniversitesi - Web Programlama (2026)
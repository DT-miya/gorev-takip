Görev Takip Sistemi (Task Management System)
ASP.NET Core Web API ve Angular Standalone mimarisi kullanılarak geliştirilmiş, Kanban panosu destekli, rol tabanlı yetkilendirme ve denetim izi (Audit Logging) özelliklerine sahip tam yığın (full-stack) görev ve proje yönetim platformu.

🚀 Teknolojiler ve Mimariler
Backend
Framework: .NET 8 / ASP.NET Core Web API

Veritabanı & ORM: SQLite, Entity Framework Core

Kimlik Doğrulama: JSON Web Token (JWT) & Role-Based Authorization

Güvenlik & Loglama: Custom Middleware, IHttpContextAccessor ile IP Tespiti, Activity Logging

E-Posta Servisi: Custom SMTP Entegrasyonu (HTML Şablon Desteği)

Dokümantasyon: Swagger / OpenAPI

Frontend
Framework: Angular 17+ (Standalone Components)

UI Kütüphanesi: Angular Material, Angular CDK (Drag & Drop)

Durum Yönetimi: Angular Signals, RxJS

Güvenlik & Ağ: Functional HTTP Interceptors (JWT Bearer Injection), Role-based Route Guards

Form Yönetimi: Reactive Forms & Dinamik Validasyonlar

🛠️ Kurulum ve Yerel Ortamda Çalıştırma
Gereksinimler
.NET 8 SDK

Node.js (v18.x veya üzeri) & npm

Angular CLI (npm install -g @angular/cli)

DB Browser for SQLite (Veritabanını görüntülemek için isteğe bağlı)

1. Backend Kurulumu
Proje ana dizininden API klasörüne geçin:

Bash
cd backend/TaskManagement.API
appsettings.json dosyasını yapılandırın (Gerekirse SMTP ve JWT gizli anahtarlarını güncelleyin):

JSON
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=TaskManagement.db"
  },
  "JwtSettings": {
    "Secret": "YourUltraSecureLongSecretKeyHere12345!",
    "Issuer": "TaskManagementAPI",
    "Audience": "TaskManagementClient",
    "ExpireInMinutes": 1440
  },
  "SmtpSettings": {
    "Host": "mail.yourdomain.com",
    "Port": 587,
    "EnableSsl": true,
    "UserName": "noreply@yourdomain.com",
    "Password": "YourPasswordHere",
    "SenderEmail": "noreply@yourdomain.com"
  }
}
Veritabanını oluşturun ve migration'ları uygulayın:

Bash
dotnet ef database update
API uygulamasını başlatın:

Bash
dotnet run
API varsayılan olarak https://localhost:5172 adresinde çalışır. Swagger arayüzüne https://localhost:5172/swagger adresinden erişebilirsiniz.

2. Frontend Kurulumu
Frontend dizinine geçin:

Bash
cd frontend/task-management-ui
Gerekli bağımlılıkları yükleyin:

Bash
npm install
Geliştirme sunucusunu başlatın:

Bash
ng serve -o
Uygulama otomatik olarak http://localhost:4200 adresinde tarayıcınızda açılacaktır.

📡 RESTful API Uç Noktaları (Endpoints)
Tüm korumalı uç noktalar HTTP Header alanında Authorization: Bearer <JWT_TOKEN> başlığı gerektirir.

🔐 Kimlik Doğrulama (/api/auth)
POST /api/auth/register — Yeni kullanıcı kaydı oluşturur.

POST /api/auth/login — Giriş yapar ve JWT Token döner.

📁 Proje Yönetimi (/api/projects)
GET /api/projects — Kullanıcının aktif üyesi olduğu projeleri listeler.

POST /api/projects — Yeni proje oluşturur.

GET /api/projects/{id} — Proje detayını, sütunlarını ve görevlerini getirir.

POST /api/projects/{id}/members — Projeye e-posta ile yeni üye davet eder.

DELETE /api/projects/{id}/members/{userId} — Üyeyi projeden çıkarır veya projeden ayrılmayı sağlar.

📋 Görev Yönetimi (/api/tasks)
GET /api/tasks/assigned — Kullanıcıya atanan ve aktif projelerdeki görevleri filtreler.

POST /api/tasks — Sütuna yeni görev ekler.

PUT /api/tasks/{id} — Görev bilgilerini günceller.

DELETE /api/tasks/{id} — Görevi siler.

POST /api/tasks/move — Görevi sürükle-bırak ile başka sütuna veya sıraya taşır.

🛡️ Yönetim Paneli (/api/admin) (Yalnızca Admin Rolü)
GET /api/admin — Toplam kullanıcı, proje ve görev metriklerini döner.

GET /api/admin-projects — Sistemdeki tüm projeleri ve arşiv durumlarını listeler.

PUT /api/admin/projects/{id}/archive — Projeyi arşivler veya arşivden çıkarır.

GET /api/admin-users — Tüm sistem kullanıcılarını ve rollerini listeler.

GET /api/admin-stats - Tüm sistem istatistiklerini listeler

GET /api/admin-logs — Sayfalanmış (paginated) denetim izi aktivite kayıtlarını döner.

🧪 Canlıya Alma (Production Build)
Backend:

Bash
dotnet publish -c Release -o ./publish
Frontend:

Bash
ng build --configuration production

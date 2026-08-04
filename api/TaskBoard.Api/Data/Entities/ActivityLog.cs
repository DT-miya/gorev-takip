namespace TaskBoard.Api.Data.Entities;

    public class ActivityLog
    {
        public int Id { get; set; }
        
        // İşlemi yapan kişi
        public int UserId { get; set; }
        public string UserMail { get; set; } = string.Empty; // Kullanıcı silinse bile logda ismi kalsın diye
        
        // İşlemin Türü ve Açıklaması
        public string Action { get; set; } = string.Empty; // Örn: "Project_Created", "Task_Moved", "Role_Changed"
        public string Description { get; set; } = string.Empty; // Örn: "Ahmet, 'X' projesini oluşturdu"
        
        public string? IpAddress { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

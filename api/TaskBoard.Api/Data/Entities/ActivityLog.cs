namespace TaskBoard.Api.Data.Entities;

    public class ActivityLog
    {
        public int Id { get; set; }
        
        // İşlemi yapan kişi
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty; // Kullanıcı silinse bile logda ismi kalsın diye
        
        // İşlemin Türü ve Açıklaması
        public string Action { get; set; } = string.Empty; // Örn: "Project_Created", "Task_Moved", "Role_Changed"
        public string Description { get; set; } = string.Empty; // Örn: "Ahmet, 'X' projesini oluşturdu"
        
        // İlişkili Modüller (Filtreleme kolaylığı için)
        public int? ProjectId { get; set; }
        public int? TaskId { get; set; }

        public string? IpAddress { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

namespace TaskBoard.Api.DTOs.Admin;


// Parametre Modeli
public class UserFilterParametersDto
{
    public string? Search { get; set; } // E-posta veya Ad-Soyad araması
    public int Page { get; set; } = 1;  // Varsayılan 1. sayfa
    public int PageSize { get; set; } = 20; // Varsayılan 20 kayıt

           // Genel Kullanıcı Araması için (Aynen kaldı)
    public string? ProjectName { get; set; }  // Proje Adı Filtresi
    public string? OwnerSearch { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
namespace TaskBoard.Api.DTOs.Admin;


// Parametre Modeli
public class UserFilterParametersDto
{
    public string? SearchEmail { get; set; } // E-posta

    public string? SearchName { get; set; } // Ad
    public int Page { get; set; } = 1;  // Varsayılan 1. sayfa
    public int PageSize { get; set; } = 20; // Varsayılan 20 kayıt
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
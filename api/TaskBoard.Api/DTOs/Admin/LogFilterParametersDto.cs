public class LogFilterParametersDto
{
    public string? SearchAction { get; set; } // action araması
    public string? SearchDescription { get; set; } // açıklama araması
    public string? SearchUserMail { get; set; } // kullanıcı e-postası araması

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
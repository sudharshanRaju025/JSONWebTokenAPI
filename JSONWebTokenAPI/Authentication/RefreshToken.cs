using JSONWebTokenAPI.Authentication;

public class RefreshToken
{
    public string Token { get; set; }
    public string JwtId { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool Invalidated { get; set; }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }  

    public DateTime CreatedAtUtc { get; set; }
   
}

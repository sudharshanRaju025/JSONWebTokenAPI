using JSONWebTokenAPI.Authentication;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

public class RefreshTokenService
{
    private readonly AppDbContext _dbContext;
    public RefreshTokenService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<string> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null || storedToken.Invalidated || storedToken.ExpiryDate <= DateTime.UtcNow)
            return null!;

        storedToken.Invalidated = true;
        var newRefreshToken = CreateRefreshToken(storedToken.JwtId, storedToken.UserId);
        _dbContext.RefreshTokens.Add(newRefreshToken);
        await _dbContext.SaveChangesAsync();

        return newRefreshToken.Token;
    }
    public RefreshToken CreateRefreshToken(string jwtId, string userId)
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
            JwtId = jwtId,
            UserId = userId,
            ExpiryDate = DateTime.UtcNow.AddMinutes(20), 
            Invalidated = false,
            CreatedAtUtc = DateTime.UtcNow
        };
        _dbContext.RefreshTokens.Add(refreshToken);
        return refreshToken;
    }
}

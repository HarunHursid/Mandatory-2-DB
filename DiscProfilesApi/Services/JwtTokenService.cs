using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DiscProfilesApi.Services;

// Interface der definerer kontrakten for JWT token generering
public interface IJwtTokenService
{
    // Metode til at generere en JWT token ud fra bruger-info
    string GenerateToken(int userId, string email, string role);
}

// Implementering af JWT token service
public class JwtTokenService : IJwtTokenService
{
    // Dependency injection af konfiguration fra appsettings.json
    private readonly IConfiguration _configuration;

    // Konstruktør der modtager konfigurationen
    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Hovedmetode der genererer JWT tokenen
    public string GenerateToken(int userId, string email, string role)
    {
        // Henter JWT konfigurationsværdier fra appsettings.json
        var key = _configuration["Jwt:Key"];              // Hemmelignøgle til at signere tokenen
        var issuer = _configuration["Jwt:Issuer"];        // Hvem der udsteder tokenen (f.eks. "DiscProfilesApi")
        var audience = _configuration["Jwt:Audience"];    // Hvem der må bruge tokenen (f.eks. "DiscProfilesUsers")

        // Validerer at alle påkrævede konfigurationsværdier er til stede
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            throw new InvalidOperationException("JWT configuration is missing in appsettings.json");

        // Konverterer hemmelignøglen fra string til en kryptografisk nøgle
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        
        // Opretter signaturlegitimationen ved hjælp af nøglen og HMAC SHA256 algoritmen
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Opretter "claims" (påstande/informationer) som skal inkluderes i tokenen
        var claims = new[]
        {
            // Bruger-ID som unik identifikator for brugeren
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            // Brugers email-adresse
            new Claim(ClaimTypes.Email, email),
            // Brugers rolle (f.eks. "user", "admin") for autorisation
            new Claim(ClaimTypes.Role, role)
        };

        // Opretter JWT tokenen med alle de nødvendige parametre
        var token = new JwtSecurityToken(
            issuer: issuer,                                    // Hvem der udsteder tokenen
            audience: audience,                                // Hvem der må bruge tokenen
            claims: claims,                                    // Brugerinformationen indlejret i tokenen
            expires: DateTime.UtcNow.AddDays(60),             // Tokenen udløber efter 60 dage
            signingCredentials: credentials);                  // Signatur for at validere at tokenen er ægte

        // Konverterer JWT tokenen til en string-format og returnerer den
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
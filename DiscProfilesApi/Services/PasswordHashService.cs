using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;

namespace DiscProfilesApi.Services;

public interface IPasswordHashService
{
    //Hash password on registration
    string HashPassword(string password);
    //Verify password on login
    bool VerifyPassword(string password, string hash);
}

//Her laver den hash med brug af BCrypt
public class PasswordHashService : IPasswordHashService
{
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        return BCrypt.Net.BCrypt.HashPassword(password);
        //Outputter token for brugeren
    }

    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }
}
using AuthenticationAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationAPI.Data;

public class AuthenticationContext : DbContext
{
    public AuthenticationContext(DbContextOptions<AuthenticationContext> options) : base(options)
    {
    }

    public required DbSet<User> Users { get; set; }
}
namespace MadWin.Infrastructure.Security
{
    public interface IPasswordHasher
    {
        string Hash(string password);
    }
}

using MadWin.Core.Entities.Users;

namespace MadWin.Core.Interfaces
{
    public interface IAccountRepository
    {
      Task AddUserAsync(User user);
      Task<bool> IsCellPhoneExistsAsync(string phone);
    }
}

using MadWin.Core.Entities.Users;
using MadWin.Core.Interfaces;
using MadWin.Infrastructure.Data;

namespace MadWin.Infrastructure.Repositories
{
    public class UserDiscountCodeRepository:GenericRepository<UserDiscountCode>, IUserDiscountCodeRepository
    {
        private readonly MadWinDBContext _context;
        public UserDiscountCodeRepository(MadWinDBContext context) : base(context)
        {
            _context = context;
        }
    }
}

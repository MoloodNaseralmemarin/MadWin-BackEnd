using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadWin.Application.Services
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
    }
}

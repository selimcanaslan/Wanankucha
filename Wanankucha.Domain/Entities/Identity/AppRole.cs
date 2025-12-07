using Microsoft.AspNetCore.Identity;
using Wanankucha.Domain.Common;

namespace Wanankucha.Domain.Entities.Identity;

public class AppRole : IdentityRole<Guid>, IEntity
{
    
}
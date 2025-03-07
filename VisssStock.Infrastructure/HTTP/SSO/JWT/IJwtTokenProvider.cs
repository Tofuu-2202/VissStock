using System.Security.Principal;
using VisssStock.Domain.DataObjects;

namespace VisssStock.Infrastructure.HTTP.SSO.JWT
{
    public interface IJwtTokenProvider
    {
        string CreateToken(IPrincipal userLogin);
    }
}

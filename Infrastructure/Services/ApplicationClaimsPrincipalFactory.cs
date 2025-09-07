using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Core.Entities;
using System.Security.Claims;

namespace Infrastructure.Services
{
    public class ApplicationClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUsers>
    {
        public ApplicationClaimsPrincipalFactory(
            UserManager<AppUsers> userManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUsers user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            
            // Add custom claims
            identity.AddClaim(new Claim("FirstName", user.FirstName ?? ""));
            identity.AddClaim(new Claim("LastName", user.LastName ?? ""));
            identity.AddClaim(new Claim("FullName", $"{user.FirstName} {user.LastName}".Trim()));
            
            return identity;
        }
    }
}

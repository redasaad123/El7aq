using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Core.Entities;
using System.Security.Claims;

namespace Web.Services
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
            
            // Add FirstName and LastName as claims if they exist
            if (!string.IsNullOrEmpty(user.FirstName))
            {
                identity.AddClaim(new Claim("FirstName", user.FirstName));
            }
            
            if (!string.IsNullOrEmpty(user.LastName))
            {
                identity.AddClaim(new Claim("LastName", user.LastName));
            }

            // Ensure roles are properly added to claims
            var roles = await UserManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            return identity;
        }
    }
}





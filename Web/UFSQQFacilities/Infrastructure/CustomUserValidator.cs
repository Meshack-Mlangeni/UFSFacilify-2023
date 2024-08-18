using Microsoft.AspNetCore.Identity;
using UFSQQFacilities.Models;

namespace UFSQQFacilities.Infrastructure
{
    public class CustomUserValidator : IUserValidator<User>
    {
        private static readonly string[] _allowedDomains = new[] { "ufs.ac.za", "ufs4life.ac.za", "gmail.com" };
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager,
            User user)
        {
            if (_allowedDomains.Any(domain => user.Email.ToLower().EndsWith($"@{domain}")))
            {
                return Task.FromResult(IdentityResult.Success);
            }
            else
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = "EmailDomainError",
                    Description = "Email address domain not allowed"
                }));
            }
        }
    }
}

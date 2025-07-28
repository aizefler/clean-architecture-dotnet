using TodoApp.Core.Common;

namespace TodoApp.Application.Common
{
    public class UserRolesContext : IUserRolesContext
    {
        public IEnumerable<string> Roles { get; }

        public UserRolesContext()
        {
            Roles = new List<string>
            {
                RolesConstants.FeatureA,
                RolesConstants.Admin
            };
        }

        public bool IsInRole(string role)
        {
            return !string.IsNullOrEmpty(Roles.First(a => a == role));
        }
    }
}
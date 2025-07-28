namespace TodoApp.Core.Common
{
    public interface IUserRolesContext
    {
        IEnumerable<string> Roles { get; }
        bool IsInRole(string role);
    }
}
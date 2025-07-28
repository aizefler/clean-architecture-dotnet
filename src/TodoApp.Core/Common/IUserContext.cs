namespace TodoApp.Core.Common
{
    public interface IUserContext
    {
        string Id { get; }
        string UserName { get; }
        string Email { get; }
        string GetTokenBearer();
        string GetXIdToken();
    }
}
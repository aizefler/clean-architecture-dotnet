using TodoApp.Application.Common;
using TodoApp.Core.Common;

namespace TodoApp.Api.Common
{
    public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public string Id
        {
            get
            {
                return UserName;
            }
        }

        public string UserName
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.Items.TryGetValue("Username", out var _userName) || _userName is null)
                    throw new InvalidOperationException(ResultError.UsuarioNaoAutenticado);

                return _userName.ToString();
            }
        }

        public string Email
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.Items.TryGetValue("Email", out var _email) || _email is null)
                    throw new InvalidOperationException(ResultError.UsuarioNaoAutenticado);

                return _email.ToString();  
            }
        }

        public string GetTokenBearer()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                .FirstOrDefault()?.Replace("Bearer ", "");

            return token ?? throw new InvalidOperationException("Bearer token not found in the context.");
        }

        public string GetXIdToken()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["x-id-token"]
                .FirstOrDefault();

            return token ?? throw new InvalidOperationException("x-id-token not found in the context.");
        }
    }
}
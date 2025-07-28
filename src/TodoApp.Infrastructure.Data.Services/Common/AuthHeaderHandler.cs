using System.Net.Http.Headers;
using TodoApp.Core.Common;

namespace TodoApp.Infrastructure.Data.Services.Common
{
    class AuthHeaderHandler : DelegatingHandler
    {
        private readonly IUserContext _userContext;

        public AuthHeaderHandler(IUserContext userContext)
        {
            this._userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _userContext.GetTokenBearer();
            var xIdtoken = _userContext.GetXIdToken();

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("x-id-token", xIdtoken);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
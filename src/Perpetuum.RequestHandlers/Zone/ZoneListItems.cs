using Perpetuum.Accounting;
using Perpetuum.Host.Requests;
using Perpetuum.Services.Sessions;


namespace Perpetuum.RequestHandlers.Zone
{
    public class ZoneListItems : IRequestHandler<IZoneRequest>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ISessionManager _sessionManager;

        public ZoneListItems(IAccountRepository accountRepository, ISessionManager sessionManager)
        {
            _accountRepository = accountRepository;
            _sessionManager = sessionManager;
        }

        public void HandleRequest(IZoneRequest request)
        {
            var zoneid = request.Data.GetOrDefault<int>(k.zone);

            var i = request.Zone.Units.ToDictionary("item", r =>
            {
                var d = r.ToDictionary();
                return d;
            });

            Message.Builder.FromRequest(request).WithData(i).Send();
        }
    }
}

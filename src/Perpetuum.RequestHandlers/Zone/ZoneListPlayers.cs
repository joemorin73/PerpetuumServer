using Perpetuum.Accounting;
using Perpetuum.Accounting.Characters;
using Perpetuum.Host.Requests;
using Perpetuum.Services.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perpetuum.RequestHandlers.Zone
{
    public class ZoneListPlayers : IRequestHandler<IZoneRequest>
    {

        private readonly IAccountRepository _accountRepository;
        private readonly ISessionManager _sessionManager;

        public ZoneListPlayers(IAccountRepository accountRepository, ISessionManager sessionManager)
        {
            _accountRepository = accountRepository;
            _sessionManager = sessionManager;
        }

        public void HandleRequest(IZoneRequest request)
        {
            var zoneid = request.Data.GetOrDefault<int>(k.zone);

            var c = _sessionManager.SelectedCharacters.Where(x => x.ZoneId == zoneid).ToDictionary("c", r =>
            {
                var d = new Dictionary<string, object>
                {
                    [k.accountID] = (int)r.AccountId,
                    [k.ID] = (int)r.Id,
                    [k.nick] = (string)r.Nick,
                    [k.accessLevel] = (int)r.AccessLevel,
                    [k.docked] = (bool)r.IsDocked,
                    [k.dockingBase] = (long)r.GetCurrentDockingBase().Eid,
                    [k.location] = r.GetPlayerRobotFromZone().CurrentPosition
                };
                return d;
            });

            Message.Builder.FromRequest(request).WithData(c).Send();
        }
    }
}


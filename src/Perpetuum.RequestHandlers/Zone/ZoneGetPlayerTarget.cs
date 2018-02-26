using Perpetuum.Host.Requests;
using Perpetuum.Services.Sessions;
using Perpetuum.Zones;
using Perpetuum.Zones.Locking.Locks;
using Perpetuum.Zones.Terrains;
using System.Collections.Generic;

namespace Perpetuum.RequestHandlers.Zone
{
    public class ZoneGetPlayerTarget : IRequestHandler<IZoneRequest>
    {
        private readonly IZoneManager _zoneManager;
        private readonly ISessionManager _sessionmanager;


        public ZoneGetPlayerTarget(IZoneManager zoneManager, ISessionManager sessionManager)
        {
            _zoneManager = zoneManager;
            _sessionmanager = sessionManager;
        }

        public void HandleRequest(IZoneRequest request)
        {
            var x = request.Data.GetOrDefault<int>(k.characterID);
            var character = _sessionmanager.GetByCharacter(x).Character;
            var lockedtarget = character.GetPlayerRobotFromZone().GetPrimaryLock();
            var tlock = (lockedtarget as TerrainLock);

            var d = new Dictionary<string, object>
            {
                [k.x] = (double)tlock.Location.X,
                [k.y] = (double)tlock.Location.Y,
                [k.z] = (double)tlock.Location.Z,
            };


            Message.Builder.FromRequest(request).WithData(d).Send();
        }
    }
}

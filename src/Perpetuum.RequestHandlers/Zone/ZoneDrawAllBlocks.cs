using Perpetuum.Data;
using Perpetuum.Host.Requests;
using Perpetuum.Units;
using Perpetuum.Zones;
using Perpetuum.Zones.Decors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Perpetuum.RequestHandlers.Zone
{
    public class ZoneDrawAllBlocks : IRequestHandler<IZoneRequest>
    {

        private readonly IZoneManager _zoneManager;

        public ZoneDrawAllBlocks(IZoneManager zoneManager)
        {
            _zoneManager = zoneManager;
        }


        public void HandleRequest(IZoneRequest request)
        {

                
            foreach(Unit zoneunit in request.Zone.Units)
            {
                bool blockit = true;
                if (zoneunit is Zones.NpcSystem.Npc) { blockit = false; }
                if (zoneunit is Services.RiftSystem.Rift) { blockit = false; }
                if (zoneunit is Players.Player) { blockit = false; }

                if (blockit)
                {
                    var unit = _zoneManager.GetUnit<Unit>(zoneunit.Eid);
                    unit.Zone.DrawEnvironmentByUnit(unit);
                }
            }

            foreach (var decor in request.Zone.DecorHandler.Decors)
            {
                try
                {
                    request.Zone.DecorHandler.DrawDecorEnvironment(decor.Key);
                    request.Zone.DecorHandler.SpreadDecorChanges(decor.Value);
                }
                catch { }
            }

            Message.Builder.FromRequest(request).WithOk().Send();
        }
    }
}

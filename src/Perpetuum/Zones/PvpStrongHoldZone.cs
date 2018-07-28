using Perpetuum.Groups.Gangs;
using Perpetuum.Services.Sessions;

namespace Perpetuum.Zones
{
    public class PvpStongHoldZone : Zone
    {
        public PvpStongHoldZone(ISessionManager sessionManager, IGangManager gangManager) : base(sessionManager, gangManager)
        {
        }
    }
}

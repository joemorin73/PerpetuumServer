using Perpetuum.Data;
using Perpetuum.Services.EventServices;
using Perpetuum.Services.EventServices.EventProcessors;
using Perpetuum.Units;
using Perpetuum.Zones.Intrusion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Perpetuum.Zones.NpcSystem
{
    public class NpcBossInfo
    {
        public static NpcBossInfo CreateBossInfoFromDB(IDataRecord record)
        {
            var id = record.GetValue<int>("id");
            var flockid = record.GetValue<int>("flockid");
            var respawnFactor = record.GetValue<double?>("respawnNoiseFactor");
            var lootSplit = record.GetValue<bool>("lootSplitFlag");
            var outpostEID = record.GetValue<long?>("outpostEID");
            var stabilityPts = record.GetValue<int?>("stabilityPts");
            var overrideRelations = record.GetValue<bool>("overrideRelations");
            var deathMessage = record.GetValue<string>("customDeathMessage");
            var aggressMessage = record.GetValue<string>("customAggressMessage");
            var info = new NpcBossInfo(id, flockid, respawnFactor, lootSplit, outpostEID, stabilityPts, overrideRelations, deathMessage, aggressMessage);

            return info;
        }

        public static NpcBossInfo GetBossInfoByFlockID(int flockid)
        {
            var bossInfos = Db.Query()
                .CommandText(@"SELECT TOP 1 id, flockid, respawnNoiseFactor, lootSplitFlag, outpostEID, stabilityPts, overrideRelations, customDeathMessage, customAggressMessage
                    FROM dbo.npcbossinfo WHERE flockid=@flockid;")
                .SetParameter("@flockid", flockid)
                .Execute()
                .Select(CreateBossInfoFromDB);

            return bossInfos.SingleOrDefault();
        }

        private readonly int _id;
        private readonly int _flockid;
        private readonly double? _respawnNoiseFactor;
        private readonly bool _lootSplit;
        private readonly long? _outpostEID;
        private readonly int? _stabilityPts;
        private readonly bool _overrideRelations;
        private readonly string _deathMsg;
        private readonly string _aggroMsg;

        public NpcBossInfo(int id, int flockid, double? respawnNoiseFactor, bool lootSplit, long? outpostEID, int? stabilityPts, bool overrideRelations, string customDeathMsg, string customAggroMsg)
        {
            _id = id;
            _flockid = flockid;
            _respawnNoiseFactor = respawnNoiseFactor;
            _lootSplit = lootSplit;
            _outpostEID = outpostEID;
            _stabilityPts = stabilityPts;
            _overrideRelations = overrideRelations;
            _deathMsg = customDeathMsg;
            _aggroMsg = customAggroMsg;
        }

        public void OnAggro(Unit aggressor, EventListenerService channel)
        {
            if (aggressor != null)
            {
                CommunicateAggression(aggressor, channel);
            }
        }

        public void OnDeath(Npc npc, Unit killer, EventListenerService channel)
        {
            CommunicateDeath(killer, channel);
            if (_outpostEID != null)
            {
                var zone = npc.Zone;
                IEnumerable<Unit> outposts = zone.Units.OfType<Outpost>();
                var outpost = outposts.First(o => o.Eid == _outpostEID);
                if (outpost is Outpost)
                {
                    var participants = npc.ThreatManager.Hostiles.Select(x => zone.ToPlayerOrGetOwnerPlayer(x.unit)).ToList();
                    EventMessage sapMessage = new StabilityAffectingEvent(outpost as Outpost,
                        zone.ToPlayerOrGetOwnerPlayer(killer),
                        npc.Definition,
                        npc.Eid,
                        StabilityPoints(),
                        participants,
                        OverrideRelations());
                    channel.PublishMessage(sapMessage);
                }
            }
        }

        public TimeSpan OnRespawn(TimeSpan respawnTime)
        {
            var factor = _respawnNoiseFactor ?? 0.0;
            return respawnTime.Multiply(FastRandom.NextDouble(1.0 - factor, 1.0 + factor));
        }

        public bool IsLootSplit()
        {
            return _lootSplit;
        }

        private bool OverrideRelations()
        {
            return _overrideRelations;
        }

        private int StabilityPoints()
        {
            return _stabilityPts ?? 0;
        }

        private void CommunicateAggression(Unit aggressor, EventListenerService channel)
        {
            SendMessage(aggressor, channel, _aggroMsg);
        }

        private void CommunicateDeath(Unit aggressor, EventListenerService channel)
        {
            SendMessage(aggressor, channel, _deathMsg);
        }

        private static void SendMessage(Unit src, EventListenerService eventChannel, string msg)
        {
            if (!msg.IsNullOrEmpty())
            {
                EventMessage eventMessage = new NpcMessage(msg, src);
                Task.Run(() => eventChannel.PublishMessage(eventMessage));
            }
        }
    }
}

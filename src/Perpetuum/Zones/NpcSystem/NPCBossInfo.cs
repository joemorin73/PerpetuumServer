using Perpetuum.Data;
using System.Data;
using System.Linq;

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
            var deathMessage = record.GetValue<string>("customDeathMessage");
            var info = new NpcBossInfo(id, flockid, respawnFactor, lootSplit, outpostEID, deathMessage);

            return info;
        }

        public static NpcBossInfo GetBossInfoByFlockID(int flockid)
        {
            var bossInfos = Db.Query()
                .CommandText("SELECT TOP 1 id, flockid, respawnNoiseFactor, lootSplitFlag, outpostEID, customDeathMessage FROM dbo.npcbossinfo WHERE flockid=@flockid;")
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
        private readonly string _deathMsg;

        public NpcBossInfo(int id, int flockid, double? respawnNoiseFactor, bool lootSplit, long? outpostEID, string customDeathMessage)
        {
            _id = id;
            _flockid = flockid;
            _respawnNoiseFactor = respawnNoiseFactor;
            _lootSplit = lootSplit;
            _outpostEID = outpostEID;
            _deathMsg = customDeathMessage;
        }

        public int FlockID()
        {
            return _flockid;
        }

        public double? RespawnNoise()
        {
            return _respawnNoiseFactor;
        }

        public bool LootSplit()
        {
            return _lootSplit;
        }

        public long? OutpostEID()
        {
            return _outpostEID;
        }

        public string DeathMessage()
        {
            return _deathMsg;
        }

    }
}

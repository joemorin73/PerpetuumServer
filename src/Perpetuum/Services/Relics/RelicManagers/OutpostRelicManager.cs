using Perpetuum.Zones;
using System;
using System.Collections.Generic;
using System.Drawing;
using Perpetuum.ExportedTypes;
using Perpetuum.Zones.Beams;
using Perpetuum.Zones.Intrusion;
using Perpetuum.Zones.Finders.PositionFinders;
using System.Threading;

namespace Perpetuum.Services.Relics
{
    public class OutpostRelicManager : AbstractRelicManager
    {
        //Spawn time params
        private readonly TimeSpan RESPAWN_RANDOM_WINDOW = TimeSpan.FromHours(1);
        private readonly TimeSpan _respawnRate = TimeSpan.FromHours(3.5);

        private Outpost _outpost;
        private Random _random;

        //Beam Draw refresh
        private readonly TimeSpan _relicRefreshRate = TimeSpan.FromSeconds(19.95);

        private IZone _zone;
        protected override IZone Zone
        {
            get
            {
                return _zone;
            }
        }
        private ReaderWriterLockSlim _lock;
        protected override ReaderWriterLockSlim Lock
        {
            get
            {
                return _lock;
            }
        }

        public OutpostRelicManager(Outpost outpost)
        {
            _lock = new ReaderWriterLockSlim();
            _outpost = outpost;
            _random = new Random();
            _relics = new List<IRelic>();
            _zone = outpost.Zone;
            relicLootGenerator = new RelicLootGenerator();
            _max_relics = 30;
            _respawnRandomized = RollNextSpawnTime();
        }

        protected override IRelic MakeRelic(RelicInfo info, Position position)
        {
            return SAPRelic.BuildAndAddToZone(info, _zone, position, relicLootGenerator.GenerateLoot(info), _outpost);
        }

        protected override TimeSpan RollNextSpawnTime()
        {
            var randomFactor = _random.NextDouble() - 0.5;
            var minutesToAdd = RESPAWN_RANDOM_WINDOW.TotalMinutes * randomFactor;
            return _respawnRate.Add(TimeSpan.FromMinutes(minutesToAdd));
        }

        protected override RelicInfo GetNextRelicType()
        {
            return new RelicInfo(1, "SAP_relic", null, null, 50); //Note: loot is associated by the id provided here!
        }

        protected override Point FindRelicPosition(RelicInfo info)
        {
            var randomPos = _outpost.CurrentPosition.GetRandomPositionInRange2D(900, 3500);
            var posFinder = new ClosestWalkablePositionFinder(_zone, randomPos);
            posFinder.Find(out Position p);
            return p;
        }

        protected override void RefreshBeam(Relic relic)
        {
            var position = relic.GetPosition();
            var p = _zone.FixZ(position);
            var beamBuilder = Beam.NewBuilder().WithType(BeamType.sap_scanner_beam).WithTargetPosition(position)
                .WithState(BeamState.AlignToTerrain)
                .WithDuration(60);
            _zone.CreateBeam(beamBuilder);
            beamBuilder = Beam.NewBuilder().WithType(BeamType.nature_effect).WithTargetPosition(position)
                .WithState(BeamState.AlignToTerrain)
                .WithDuration(_relicRefreshRate);
            _zone.CreateBeam(beamBuilder);
            for (var i = 0; i < 4; i++)
            {
                beamBuilder = Beam.NewBuilder().WithType(BeamType.green_20sec).WithTargetPosition(p.AddToZ(3.5 * i + 1.0))
                    .WithState(BeamState.Hit)
                    .WithDuration(_relicRefreshRate);
                _zone.CreateBeam(beamBuilder);
            }
        }
    }
}

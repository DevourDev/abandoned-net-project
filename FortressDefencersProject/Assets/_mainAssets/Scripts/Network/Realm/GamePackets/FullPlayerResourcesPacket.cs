using System.Collections.Generic;

namespace FD.Networking.Realm.GamePackets
{
    public class FullPlayerResourcesPacket : IPacketContent
    {
        public int UniqueID => 210_003;
        public int SideID;
        public int TeamID;

        public int[] UnitsObjectsIDs;
        public int[] UnitsItemsOnSceneIDs;
        public TransformPacket[] Transforms;

        public DynamicStatAllValuesPacket[][] UnitsDynamicStats;

        private readonly bool _filledWithLists;

        private List<int> _unitObjectsIDs;
        private List<int> _itemsOnSceneIDs;
        private List<TransformPacket> _transforms;
        private List<List<DynamicStatAllValuesPacket>> _unitsDynamicStats;


        public FullPlayerResourcesPacket()
        {

        }

        public FullPlayerResourcesPacket(bool filledWithLists, int amount = 8)
        {
            _filledWithLists = filledWithLists;
            if (!filledWithLists)
                return;

            _unitObjectsIDs = new(amount);
            _itemsOnSceneIDs = new(amount);
            _transforms = new(amount);
            _unitsDynamicStats = new(amount);
        }

        public FullPlayerResourcesPacket(ICollection<Units.UnitOnSceneBase> units) : this(true, units.Count)
        {
            foreach (var u in units)
            {
                AddUnit(u);
            }
        }


        public void AddUnit(Units.UnitOnSceneBase u)
        {
            if (!_filledWithLists)
                throw new System.Exception("пошел нахуй");

            _unitObjectsIDs.Add(u.Reference.UniqueID);
            _itemsOnSceneIDs.Add(u.UniqueID);
            var uTr = new TransformPacket
            {
                Position = u.transform.position,
                YRotation = u.transform.rotation.y
            };
            _transforms.Add(uTr);
            var dses = new List<DynamicStatAllValuesPacket>();
            foreach (var ds in u.DynamicStatsCollection.Stats)
            {
                var dsPacket = new DynamicStatAllValuesPacket()
                {
                    StatID = ds.Key.UniqueID,
                    Max = ds.Value.Max,
                    Min = ds.Value.Min,
                    Current = ds.Value.Current,
                    Regen = ds.Value.Regen,
                };

                dses.Add(dsPacket);
            }

            _unitsDynamicStats.Add(dses);

        }


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            SideID = d.ReadInt();
            TeamID = d.ReadInt();

            UnitsObjectsIDs = d.ReadInts();
            UnitsItemsOnSceneIDs = d.ReadInts();
            Transforms = d.ReadDecodables<TransformPacket>();
            UnitsDynamicStats = d.ReadDecodablesArray<DynamicStatAllValuesPacket>();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(SideID);
            e.Write(TeamID);

            if (_filledWithLists)
            {
                e.Write(_unitObjectsIDs);
                e.Write(_itemsOnSceneIDs);
                e.WriteEncodables(_transforms);
                e.WriteEncodables((ICollection<ICollection<DynamicStatAllValuesPacket>>)_unitsDynamicStats);
            }
            else
            {
                e.Write(UnitsObjectsIDs);
                e.Write(UnitsItemsOnSceneIDs);
                e.Write(Transforms);
                e.WriteEncodables(UnitsDynamicStats);
            }

        }
    }
}

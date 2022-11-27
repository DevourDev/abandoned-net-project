using System.Collections.Generic;
using UnityEngine;

namespace FD.Networking.Realm.GamePackets
{

    public class PlayerNewResourcesPacket : IPacketContent
    {
        public int UniqueID => 210_002;
        public int SideID;
        public bool TeamChanged;
        public int TeamID;

        public int[] UnitsObjectsIDs;
        public int[] ItemsOnSceneIDs;
        public TransformPacket[] UpdatedTransforms;

        public DynamicStatAllValuesPacket[][] UnitsDynamicStats;

        private readonly bool _filledWithLists;

        private List<int> _unitObjectsIDs;
        private List<int> _itemsOnSceneIDs;
        private List<TransformPacket> _updatedTransforms;
        private List<ICollection<DynamicStatAllValuesPacket>> _unitsDynamicStats;


        public PlayerNewResourcesPacket()
        {

        }

        public PlayerNewResourcesPacket(bool filledWithLists, int amount = 8)
        {
            _filledWithLists = filledWithLists;
            if (!filledWithLists)
                return;

            _unitObjectsIDs = new(amount);
            _itemsOnSceneIDs = new(amount);
            _updatedTransforms = new(amount);
            _unitsDynamicStats = new(amount);
        }

        public PlayerNewResourcesPacket(ICollection<Units.UnitOnSceneBase> units) : this(true, units.Count)
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
            _updatedTransforms.Add(uTr);
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
            TeamChanged = d.ReadBool();

            if (TeamChanged)
                TeamID = d.ReadInt();

            UnitsObjectsIDs = d.ReadInts();
            ItemsOnSceneIDs = d.ReadInts();
            UpdatedTransforms = d.ReadDecodables<TransformPacket>();
            UnitsDynamicStats = d.ReadDecodablesArray<DynamicStatAllValuesPacket>();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(SideID);
            e.Write(TeamChanged);

            if (TeamChanged)
                e.Write(TeamID);

            if (_filledWithLists)
            {
                e.Write(_unitObjectsIDs);
                e.Write(_itemsOnSceneIDs);
                e.WriteEncodables(_updatedTransforms);
                e.WriteEncodables(_unitsDynamicStats); // <-- problem (fixed somehow)
            }
            else
            {
                e.Write(UnitsObjectsIDs);
                e.Write(ItemsOnSceneIDs);
                e.Write(UpdatedTransforms);
                e.WriteEncodables(UnitsDynamicStats);
            }

        }
    }
}

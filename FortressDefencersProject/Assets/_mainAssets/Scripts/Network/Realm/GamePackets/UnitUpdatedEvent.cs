using FD.Units;
using System.Collections.Generic;

namespace FD.Networking.Realm.GamePackets
{
    public class UnitUpdatedEvent : IPacketContent
    {
        public int UniqueID => 100_000;
        public int UnitUniqueID;
        public bool SideChanged;
        public int UpdatedSideID;
        public bool TransformChanged;
        public TransformPacket UpdatedTransform;
        public bool StateChanged;
        public UnitStatePacket UpdatedState;
        public UpdatedDynamicStatPacket[] UpdatedDynamicStats;
        public AbilityStagePacket[] UpdatedAbilitiesStages;


        private bool _filledWithLists;
        private Dictionary<int, UpdatedDynamicStatPacket> _updatedDynamicStats;
        private Dictionary<int, AbilityStagePacket> _updatedAbilitiesStages;


        public UnitUpdatedEvent()
        {

        }

        public UnitUpdatedEvent(Dictionary<int, UpdatedDynamicStatPacket> updatedDynamicStats, Dictionary<int, AbilityStagePacket> updatedAbilitiesStages)
        {
            _filledWithLists = true;
            _updatedDynamicStats = updatedDynamicStats;
            _updatedAbilitiesStages = updatedAbilitiesStages;
        }


        public Dictionary<int, UpdatedDynamicStatPacket> UpdatedDynamicStatsDic => _updatedDynamicStats;
        public Dictionary<int, AbilityStagePacket> UpdatedAbilitiesStagesDic => _updatedAbilitiesStages;


        public void SetAsFilledWithLists()
        {
            _filledWithLists = true;
            _updatedDynamicStats = new Dictionary<int, UpdatedDynamicStatPacket>();
            _updatedAbilitiesStages = new Dictionary<int, AbilityStagePacket>();
        }



        public void AddUpdatedDynamicStat(int key, UpdatedDynamicStatPacket uds)
        {
            _updatedDynamicStats.Add(key, uds);
        }

        public void AddUpdatedAbilityStage(int key, AbilityStagePacket asp)
        {

            _updatedAbilitiesStages.Add(key, asp);
        }


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();

            UnitUniqueID = d.ReadInt();
            SideChanged = d.ReadBool();

            if (SideChanged)
                UpdatedSideID = d.ReadInt();

            TransformChanged = d.ReadBool();

            if (TransformChanged)
                UpdatedTransform = d.ReadDecodable<TransformPacket>();

            StateChanged = d.ReadBool();

            if (StateChanged)
                UpdatedState = d.ReadDecodable<UnitStatePacket>();

            UpdatedDynamicStats = d.ReadDecodables<UpdatedDynamicStatPacket>();
            UpdatedAbilitiesStages = d.ReadDecodables<AbilityStagePacket>();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(UnitUniqueID);
            e.Write(SideChanged);

            if (SideChanged)
            {
                e.Write(UpdatedSideID);
            }

            e.Write(TransformChanged);

            if (TransformChanged)
                e.Write(UpdatedTransform);

            e.Write(StateChanged);

            if (StateChanged)
                e.Write(UpdatedState);

            if (_filledWithLists)
            {
                e.WriteEncodables(_updatedDynamicStats.Values);
                e.WriteEncodables(_updatedAbilitiesStages.Values);
            }
            else
            {
                e.Write(UpdatedDynamicStats);
                e.Write(UpdatedAbilitiesStages);
            }

        }
    }

}

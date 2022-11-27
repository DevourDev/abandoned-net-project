using System.Collections.Generic;

namespace FD.Networking.Realm.GamePackets
{
    public class SharedGameStateMessage : IPacketContent
    {
        public int UniqueID => 500_003;

        public PlayerNewResourcesPacket[] NewSidesResources;
        public UnitUpdatedEvent[] UpdatedUnits;
        public UnitDiedEvent[] Deathes;

        private bool _filledWithLists;
        private ICollection<PlayerNewResourcesPacket> _newSidesResources;
        private ICollection<UnitUpdatedEvent> _updatedUnits;
        private List<UnitDiedEvent> _deathes;


        public SharedGameStateMessage()
        {

        }


        public SharedGameStateMessage(ICollection<PlayerNewResourcesPacket> newSidesResources, ICollection<UnitUpdatedEvent> updatedUnits, List<UnitDiedEvent> deathes)
        {
            _filledWithLists = true;
            _newSidesResources = newSidesResources;
            _updatedUnits = updatedUnits;
            _deathes = deathes;
        }


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();

            NewSidesResources = d.ReadDecodables<PlayerNewResourcesPacket>();
            UpdatedUnits = d.ReadDecodables<UnitUpdatedEvent>();
            Deathes = d.ReadDecodables<UnitDiedEvent>();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);

            if (_filledWithLists)
            {
                e.WriteEncodables(_newSidesResources);
                e.WriteEncodables(_updatedUnits);
                e.WriteEncodables(_deathes);
            }
            else
            {
                e.Write(NewSidesResources);
                e.Write(UpdatedUnits);
                e.Write(Deathes);
            }
        }
    }


}

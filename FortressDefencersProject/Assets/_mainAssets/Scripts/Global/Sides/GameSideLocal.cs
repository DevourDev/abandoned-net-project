using FD.Networking;

namespace FD.Global.Sides
{
    public class GameSideLocal : GameSideDefault
    {
        public GameSideLocal() : base()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected override void HandleResponse(IPacketContent res)
        {
            throw new System.NotImplementedException();
        }
    }
}
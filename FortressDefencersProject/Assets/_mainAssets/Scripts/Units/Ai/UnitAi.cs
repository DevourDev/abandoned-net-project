using DevourDev.MonoBase.Ai.ExExAct;
using DevourDev.Networking;
using FD.ClientSide.Units;
using FD.Networking;

namespace FD.Units.Ai
{
    public class UnitAi : AiBase<UnitAi, ConditionalUnitActions>
    {
        private readonly UnitOnSceneBase _serverSideUnit;
        private readonly UnitOnSceneClientSide _clientSideUnit;
        private readonly NetworkMode _networkMode;


        public UnitAi(UnitOnSceneBase u)
        {
            _serverSideUnit = u;
            //AggroTarget = new();
            _networkMode = NetworkMode.Server;
        }

        public UnitAi(UnitOnSceneClientSide u)
        {
            _clientSideUnit = u;
            //AggroTarget = new();
            _networkMode = NetworkMode.Client;
        }

        public UnitAi(UnitOnSceneBase serverSideUnit, UnitOnSceneClientSide clientSideUnit)
        {
            _serverSideUnit = serverSideUnit;
            _clientSideUnit = clientSideUnit;
            _networkMode = NetworkMode.Host;
        }


        //public Target AggroTarget { get; private set; } //why do i need that?

        public UnitOnSceneBase ServerSideUnit => _serverSideUnit;
        public UnitOnSceneClientSide ClientSideUnit => _clientSideUnit;
        public NetworkMode NetworkMode => _networkMode;


        public void Evaluate() => CurrentState.Evaluate(this);
    }

}

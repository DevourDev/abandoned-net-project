using FD.Units;


namespace FD.Global.Sides
{
    public abstract class GameSideDefault : GameSideBase
    {
        private int _teamID;
       


        public GameSideDefault() : base()
        {
        }


        public int TeamID => _teamID;
        

        public UnitOnSceneBase Fortress { get; set; }

        public UnitObject UnitInHand { get; set; }



        public void SetTeam(int teamID)
        {
            _teamID = teamID;
        }

        public override void AddUnit(UnitOnSceneBase u)
        {
            base.AddUnit(u);
        }

        public void RegistrateFortress(UnitOnSceneBase f)
        {
            base.AddUnit(f);
            Fortress = f;
            f.OnDeath += OnFortressDestroyedHandler;
        }

        private void OnFortressDestroyedHandler(UnitOnSceneBase obj)
        {
            //obj.Alive = false;
            GameManager.Instance.Eliminate(this);
        }

    }
}
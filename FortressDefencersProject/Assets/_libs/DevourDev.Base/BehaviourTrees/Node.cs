namespace DevourDev.Base.BehaviourTrees
{
    public abstract class Node
    {
        private NodeState _state;


        public NodeState State { get => _state; protected set => _state = value; }


        public abstract NodeState Evaluate();
    }
}

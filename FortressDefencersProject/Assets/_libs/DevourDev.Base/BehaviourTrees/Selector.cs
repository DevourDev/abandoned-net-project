using System;

namespace DevourDev.Base.BehaviourTrees
{
    public class Selector : Node
    {
        public Selector(params Node[] nodes)
        {
            Nodes = nodes;
        }


        protected Node[] Nodes { get; set; }


        public override NodeState Evaluate()
        {
            foreach (var n in Nodes)
            {
                var evaluateResult = n.Evaluate();

                switch (evaluateResult)
                {
                    case NodeState.RUNNING or NodeState.SUCCESS:
                        State = evaluateResult;
                        return State;
                    case NodeState.FAILURE:
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            State = NodeState.FAILURE;
            return State;
        }
    }
}

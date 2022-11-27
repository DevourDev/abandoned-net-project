using System;

namespace DevourDev.Base.BehaviourTrees
{
    public class Sequence : Node
    {
        public Sequence(params Node[] nodes)
        {
            Nodes = nodes;
        }


        protected Node[] Nodes { get; set; }


        public override NodeState Evaluate()
        {
            bool isAnyChildRunning = false;
            foreach (var n in Nodes)
            {
                var evaluateResult = n.Evaluate();

                switch (evaluateResult)
                {
                    case NodeState.RUNNING:
                        isAnyChildRunning = true;
                        break;
                    case NodeState.SUCCESS:
                        break;
                    case NodeState.FAILURE:
                        State = evaluateResult;
                        return State;
                    default:
                        throw new NotImplementedException();
                }
            }

            State = isAnyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return State;
        }
    }
}

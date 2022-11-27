using System;

namespace DevourDev.Base.BehaviourTrees
{
    public class Inverter : Node
    {
        protected Node Node { get; set; }


        public Inverter(Node node)
        {
            Node = node;
        }


        public override NodeState Evaluate()
        {
            var evaluateResult = Node.Evaluate();

            return evaluateResult switch
            {
                NodeState.RUNNING => State,
                NodeState.SUCCESS => NodeState.FAILURE,
                NodeState.FAILURE => NodeState.SUCCESS,
                _ => throw new NotImplementedException(),
            };
        }

    }
}

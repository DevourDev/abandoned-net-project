namespace FD.Global
{
    public class SpawnUnitResult
    {
        private readonly bool _result;
        private readonly Units.UnitOnSceneBase _spawnedUnit;
        private readonly FailureReasonEnum _failureReason;


        public SpawnUnitResult(Units.UnitOnSceneBase u)
        {
            _result = true;
            _spawnedUnit = u;
        }

        public SpawnUnitResult(FailureReasonEnum fr)
        {
            _result = false;
            _failureReason = fr;
        }


        public FailureReasonEnum FailureReason => _failureReason;


        public bool TryGetUnit(out Units.UnitOnSceneBase u)
        {
            u = _spawnedUnit;
            return _result;
        }


        public enum FailureReasonEnum : byte
        {
            None,
            Success,
            NotEnoughCoins,
            BadSpawnPosition,
            PlayerLost,
            Other
        }
    }
}

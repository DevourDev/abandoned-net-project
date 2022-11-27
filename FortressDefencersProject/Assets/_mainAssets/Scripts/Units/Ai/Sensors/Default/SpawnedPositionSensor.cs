using DevourDev.MonoBase.Ai.ExExAct;
using UnityEngine;

namespace FD.Units.Ai.Sensors
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Sensors/Spawned Position")]
    public class SpawnedPositionSensor : UnitSensor
    {
        protected override void Scan(UnitAi ai, out SensorData sd)
        {
            if (ai.ContainsSensorData(this, false))
            {
                sd = new Data(ai.ServerSideUnit.transform.position);
                ai.SetSensorData(this, sd);
            }
            else
            {
                sd = ai.GetSensorData<Data>(this, true);
            }
        }

        public class Data : SensorData
        {
            public Vector3 SpawnPos { get; private set; }


            public Data(Vector3 spawnPos)
            {
                SpawnPos = spawnPos;
                MarkAsAlwaysActual();
            }
        }
    }
}

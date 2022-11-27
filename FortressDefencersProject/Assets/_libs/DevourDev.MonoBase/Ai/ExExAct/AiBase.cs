using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevourDev.MonoBase.Ai.ExExAct
{

    public abstract class AiBase<AgentAi, ConditionalActions>
        where AgentAi : AiBase<AgentAi, ConditionalActions>
        where ConditionalActions : ConditionalAgentActions<AgentAi, ConditionalActions>
    {
        public AiBase()
        {
            SensorsData = new();
            EnquirersData = new();
        }


        public AiStateBase<AgentAi, ConditionalActions> CurrentState { get; set; }

        protected Dictionary<SensorBase<AgentAi, ConditionalActions>, SensorData> SensorsData { get; private set; }
        protected Dictionary<EnquirerBase<AgentAi, ConditionalActions>, EnquirerData> EnquirersData { get; private set; }


        public List<SensorData> GetExposedSensorsData() // for exposing in Inspector
        {
            var kvpList = SensorsData.ToList();
            var sdList = new List<SensorData>();
            foreach (var item in kvpList)
            {
                sdList.Add(item.Value);
            }

            return sdList;
        }

        public void SetOutdatedAllData()
        {
            foreach (var sd in SensorsData)
            {
                sd.Value.SetOutdated();
            }

            foreach (var ed in EnquirersData)
            {
                ed.Value.SetOutdated();
            }
        }

        public bool ContainsSensorData(SensorBase<AgentAi, ConditionalActions> s, bool relevant)
        {
            if (!SensorsData.TryGetValue(s, out var v))
            {
                return false;
            }

            return !relevant || v.IsActual;
        }

        public SensorData GetSensorData(SensorBase<AgentAi, ConditionalActions> s, bool relevant)
        {
            if (!ContainsSensorData(s, relevant))
            {
                s.Scan(this as AgentAi);
            }

            return SensorsData[s];
        }
        public SD GetSensorData<SD>(SensorBase<AgentAi, ConditionalActions> s, bool relevant) where SD : SensorData
        {
            return (SD)GetSensorData(s, relevant);
        }


        public SD GetOrCreateSensorData<SD>(SensorBase<AgentAi, ConditionalActions> s) where SD : SensorData, new()
        {
            if (ContainsSensorData(s, false))
            {
                return GetSensorData<SD>(s, false);
            }
            var d = new SD();
            SetSensorData(s, d);
            return d;
        }

        public void SetSensorData(SensorBase<AgentAi, ConditionalActions> s, SensorData d)
        {
            SensorsData.Add(s, d);
        }


        public bool ContainsEnquirerData(EnquirerBase<AgentAi, ConditionalActions> e, bool relevant)
        {
            if (!EnquirersData.TryGetValue(e, out var v))
            {
                return false;
            }

            return !relevant || v.IsActual;
        }

        public EnquirerData GetEnquirerData(EnquirerBase<AgentAi, ConditionalActions> e, bool relevant)
        {
            if (!ContainsEnquirerData(e, relevant))
            {
                e.Examine(this as AgentAi);
            }

            return EnquirersData[e];
        }

        public ED GetEnquirerData<ED>(EnquirerBase<AgentAi, ConditionalActions> e, bool relevant) where ED : EnquirerData
        {
            return (ED)GetEnquirerData(e, relevant);
        }

        public ED GetOrCreateEnquirerData<ED>(EnquirerBase<AgentAi, ConditionalActions> e) where ED : EnquirerData, new()
        {
            if (ContainsEnquirerData(e, false))
            {
                return GetEnquirerData<ED>(e, false);
            }
            var d = new ED();
            SetEnquirerData(e, d);
            return d;
        }

        public void SetEnquirerData(EnquirerBase<AgentAi, ConditionalActions> e, EnquirerData d)
        {
            EnquirersData.Add(e, d);
        }
    }

}

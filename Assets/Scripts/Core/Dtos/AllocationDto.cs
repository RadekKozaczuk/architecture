#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;

namespace Core.Dtos
{
    [Serializable]
    public class AllocationDto
    {
#pragma warning disable RAD201
        public string allocationId;
        public long buildConfigurationId;
        public string created;
        public bool failed;
        public string fleetId;
        public string fulfilled;
        public long gamePort;
        public string ipv4;
        public string ipv6;
        public long machineId;
        public bool readiness;
        public string ready;
        public string regionId;
        public string requestId;
        public string requested;
        public long serverId;
        public long timeout;
#pragma warning restore RAD201
    }
}
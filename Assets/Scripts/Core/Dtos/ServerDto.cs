#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;

namespace Core.Dtos
{
    [Serializable]
    public class ServerDto
    {
        public int BuildConfigurationID;
        public string BuildConfigurationName;
        public string BuildName;
        public bool Deleted;
        public string FleetID;
        public string FleetName;
        public string HardwareType;
        public int Id;
        public string Ip;
        public int LocationID;
        public string LocationName;
        public int MachineID;
        public int Port;
        public string Status;
    }
}
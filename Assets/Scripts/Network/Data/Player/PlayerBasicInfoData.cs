using UnityEngine;

namespace NMX
{
    public class PlayerBasicInfoData
    {
        public string PlayerName { get; private set; }
        public int ClientId { get; private set; }

        public PlayerBasicInfoData(string playerName, int id)
        {
            PlayerName = playerName;
            ClientId = id;
        }
    }
}

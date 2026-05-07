using System.Collections.Generic;
using EFT;

namespace RaidOverhaul.Models
{
    public static class WildSpawnTypeExtensions
    {
        public static List<int> LegionTypeEnums = new List<int> { 199, 200 };
        public static List<int> WolfTypeEnums = new List<int> { 201, 202 };

        public static bool IsLegion(WildSpawnType type)
        {
            return LegionTypeEnums.Contains((int)type);
        }

        public static bool IsWolf(WildSpawnType type)
        {
            return WolfTypeEnums.Contains((int)type);
        }
    }
}

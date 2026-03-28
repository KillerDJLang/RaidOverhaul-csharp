using System.Collections.Generic;
using EFT;

namespace RaidOverhaul.Models
{
    public static class WildSpawnTypeExtensions
    {
        public static List<int> TypeEnums = new List<int> { 199, 200 };

        public static bool IsLegion(WildSpawnType type)
        {
            return TypeEnums.Contains((int)type);
        }
    }
}

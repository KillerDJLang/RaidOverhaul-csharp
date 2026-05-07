using RaidOverhaul.Models;

namespace RaidOverhaul.Controllers
{
    internal static class ConfigController
    {
        public static ServerConfigs ServerConfig = new ServerConfigs();
        public static DebugConfigs DebugConfig = new DebugConfigs();
        public static EventsConfig EventConfig = new EventsConfig();
        public static SeasonalConfig SeasonConfig = new SeasonalConfig();
        public static LegionProgressionConfig LegionConfig = new LegionProgressionConfig();
        public static ProfileFlags ProfileFlags = new ProfileFlags();
    }
}

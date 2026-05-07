using RaidOverhaul.Controllers;
using UnityEngine;

namespace RaidOverhaul.Helpers
{
    public static class NotificationHelper
    {
        public static class NotificationLength
        {
            public const float Tick = 1f;
            public const float Short = 3f;
            public const float Medium = 5f;
            public const float Long = 7f;
            public const float Extended = 10f;
        }

        public static class NotificationColor
        {
            public static readonly Color White = Color.white;
            public static readonly Color Gold = new Color(0.78f, 0.66f, 0.29f);
            public static readonly Color Blue = new Color(0.29f, 0.62f, 0.75f);
            public static readonly Color Red = new Color(0.75f, 0.27f, 0.10f);
            public static readonly Color Green = new Color(0.24f, 0.60f, 0.24f);
            public static readonly Color Orange = new Color(0.85f, 0.50f, 0.10f);
        }

        public static void Show(string message, float duration, Color color)
        {
            NotificationUIController.Instance?.Show(message, duration, color);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using EFT.Weather;
using HarmonyLib;
using Newtonsoft.Json;
using SPT.Common.Http;

namespace RaidOverhaul.Helpers
{
    public static class Utils
    {
        public static FieldInfo FogField;
        public static FieldInfo LighteningThunderField;
        public static FieldInfo RainField;
        public static FieldInfo TemperatureField;
        private static readonly JsonConverter[] _defaultJsonConverters;

        public static readonly Dictionary<string, MongoID> Traders = new()
        {
            { "Prapor", "54cb50c76803fa8b248b4571" },
            { "Therapist", "54cb57776803fa99248b456e" },
            { "Fence", "579dc571d53a0658a154fbec" },
            { "Skier", "58330581ace78e27b8b10cee" },
            { "Peacekeeper", "5935c25fb3acc3127c3d8cd9" },
            { "Mechanic", "5a7c2eca46aef81a7ca2145d" },
            { "Ragman", "5ac3b934156ae10c4430e83c" },
            { "Jaeger", "5c0647fdd443bc2504c2d371" },
            { "Lightkeeper", "638f541a29ffd1183d187f57" },
            { "Btr", "656f0f98d80a697f855d34b1" },
            { "Ref", "6617beeaa9cfa777ca915b7c" },
            { "ReqShop", "66f0eaa93f6cc015bc1f3acb" },
        };

        public static readonly Dictionary<string, MongoID> TradersNoReq = new()
        {
            { "Prapor", "54cb50c76803fa8b248b4571" },
            { "Therapist", "54cb57776803fa99248b456e" },
            { "Fence", "579dc571d53a0658a154fbec" },
            { "Skier", "58330581ace78e27b8b10cee" },
            { "Peacekeeper", "5935c25fb3acc3127c3d8cd9" },
            { "Mechanic", "5a7c2eca46aef81a7ca2145d" },
            { "Ragman", "5ac3b934156ae10c4430e83c" },
            { "Jaeger", "5c0647fdd443bc2504c2d371" },
            { "Lightkeeper", "638f541a29ffd1183d187f57" },
            { "Btr", "656f0f98d80a697f855d34b1" },
            { "Ref", "6617beeaa9cfa777ca915b7c" },
        };

        public static readonly Dictionary<string, MongoID> Currency = new()
        {
            { "Roubles", "5449016a4bdc2d6f028b456f" },
            { "USD", "5696686a4bdc2da3298b456a" },
            { "Euros", "569668774bdc2da2298b4568" },
            { "GPCoins", "5d235b4d86f7742e017bc88a" },
            { "ReqCoins", "66292e79a4d9da25e683ab55" },
            { "ReqSlips", "668b3c71042c73c6f9b00704" },
            { "SpecialReqForms", "67c95a09708ee99e7a575da5" },
        };

        public const string SkeletonKey = "66a2fc926af26cc365283f23";
        public const string VipKeycard = "66a2fc9886fbd5d38c5ca2a6";
        public const string RedFlare = "624c09cfbc2e27219346d955";
        public const string RealismKey = "RealismMod";
        public const string ROStandaloneKey = "nameless.raidoverhaul.standalone";
        public const string Heal = "Heal";
        public const string Damage = "Damage";
        public const string Repair = "Repair";
        public const string Airdrop = "Airdrop";
        public const string Jokes = "Jokes";
        public const string Blackout = "Blackout";
        public const string Skill = "Skill";
        public const string Metabolism = "Metabolism";
        public const string Malf = "Malf";
        public const string LoyaltyLevel = "LoyaltyLevel";
        public const string Berserk = "Berserk";
        public const string Weight = "Weight";
        public const string MaxLoyaltyLevel = "MaxLoyaltyLevel";
        public const string CorrectRep = "CorrectRep";
        public const string Lockdown = "Lockdown";
        public const string GearExfilEvent = "GearExfilEvent";
        public const string Train = "Train";
        public const string PmcExfil = "PmcExfil";
        public const string Artillery = "Artillery";
        private static readonly Dictionary<string, ItemTemplate> _templates = [];

        public static T Get<T>(string url)
        {
            var req = RequestHandler.GetJson(url);

            if (string.IsNullOrEmpty(req))
            {
                throw new InvalidOperationException("The response from the server is null or empty.");
            }

            return JsonConvert.DeserializeObject<T>(req);
        }

        public static void LogToServerConsole(string message)
        {
            Plugin._log.Log(LogLevel.Info, message);
            RequestHandler.PutJson("/RaidOverhaul/LogToServer", new { message = message }.ToJson(_defaultJsonConverters));
        }

        public static readonly EquipmentSlot[] ArmbandFas = { EquipmentSlot.Pockets, EquipmentSlot.TacticalVest, EquipmentSlot.ArmBand };

        public static readonly EquipmentSlot[] ArmbandAas =
        {
            EquipmentSlot.TacticalVest,
            EquipmentSlot.ArmorVest,
            EquipmentSlot.Headwear,
            EquipmentSlot.FaceCover,
            EquipmentSlot.Eyewear,
            EquipmentSlot.ArmBand,
        };

        public static void GetWeatherFields()
        {
            FogField = AccessTools.Field(typeof(WeatherDebug), "Fog");
            LighteningThunderField = AccessTools.Field(typeof(WeatherDebug), "LightningThunderProbability");
            RainField = AccessTools.Field(typeof(WeatherDebug), "Rain");
            TemperatureField = AccessTools.Field(typeof(WeatherDebug), "Temperature");
        }

        private static void AddTemplatesToArray()
        {
            if (!Singleton<ItemFactoryClass>.Instantiated)
            {
                return;
            }

            var mongoTemplates = Singleton<ItemFactoryClass>.Instance.ItemTemplates;

            if (_templates.Count == mongoTemplates.Count)
            {
                return;
            }
            foreach (var keyValuePair in mongoTemplates)
            {
                _templates.Add(keyValuePair.Key.ToString(), keyValuePair.Value);
            }
        }

        public static ItemTemplate[] FindTemplates(string templateToFind)
        {
            AddTemplatesToArray();
            if (_templates.TryGetValue(templateToFind, out var template))
            {
                return [template];
            }
            return
            [
                .. _templates.Values.Where(t =>
                    t.ShortNameLocalizationKey.Localized().IndexOf(templateToFind, StringComparison.OrdinalIgnoreCase) >= 0
                    || t.NameLocalizationKey.Localized().IndexOf(templateToFind, StringComparison.OrdinalIgnoreCase) >= 0
                ),
            ];
        }

        public static bool IsInRaid()
        {
            if (!Singleton<GameWorld>.Instantiated)
            {
                return false;
            }

            var gameWorld = Singleton<GameWorld>.Instance;

            return gameWorld != null
                && gameWorld.AllAlivePlayersList != null
                && gameWorld.AllAlivePlayersList.Count > 0
                && gameWorld.MainPlayer != null
                && gameWorld.MainPlayer is not HideoutPlayer;
        }
    }
}

using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;

namespace RaidOverhaulMain.Models;

public class AmmoStackList
{
    public required string[] RifleList { get; set; }
    public required string[] ShotgunList { get; set; }
    public required string[] SmgList { get; set; }
    public required string[] SniperList { get; set; }
    public required string[] UbglList { get; set; }
}

public record BotSpawnData
{
    [JsonPropertyName("spawnData")]
    public Dictionary<string, Dictionary<string, BossLocationSpawn>> SpawnData { get; set; }
}

public class ShopInfoFile
{
    [JsonPropertyName("blacklist")]
    public MongoId[]? ShopBlacklist { get; set; }

    [JsonPropertyName("specialItems")]
    public MongoId[]? SpecialShopItems { get; set; }
}

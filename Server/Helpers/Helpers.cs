using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using Path = System.IO.Path;

namespace RaidOverhaulMain.Helpers;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 10)]
public class ROHelpers(
    ISptLogger<ROHelpers> logger,
    DatabaseService databaseService,
    LocaleService localeService,
    IReadOnlyList<SptMod> sptModsList,
    PresetHelper presetHelper,
    HandbookHelper handbookHelper,
    ItemHelper itemHelper,
    RandomUtil randomUtil,
    JsonUtil jsonUtil,
    FileUtil fileUtil,
    ModHelper modHelper
)
{
    public bool CheckForMod(string modGuid)
    {
        return sptModsList.Any(m => m.ModMetadata.ModGuid == modGuid);
    }

    public static bool CheckFilePath(string path, string fileName)
    {
        var filePath = Path.Combine(path, fileName);
        filePath += ".json";

        return File.Exists(filePath);
    }

    public int GenRandomCount(int min, int max)
    {
        return randomUtil.RandInt(min, max);
    }

    public void ProfileBackup(MongoId sessionID, Assembly assembly)
    {
        var modPath = modHelper.GetAbsolutePathToModFolder(assembly);
        var year = DateTime.Now.Year.ToString();
        var month = DateTime.Now.Month.ToString();
        var day = DateTime.Now.Day.ToString();
        var hour = DateTime.Now.Hour.ToString();
        var minute = DateTime.Now.Minute.ToString();
        var backupPath = Path.Combine(modPath, "profileBackup", sessionID, $"{year}/{month}/{day}", $"{sessionID}-{hour}-{minute}.json");
        var profilePath = Path.Combine(modPath, "../", "../", "profiles", $"{sessionID}.json");

        fileUtil.CopyFile(profilePath, backupPath, true);
    }

    public HandbookItem? GetItemInHandbook(string itemId)
    {
        var tables = databaseService.GetTables();
        var hbItem = tables.Templates.Handbook.Items.SingleOrDefault(x => x.Id == itemId);

        return hbItem;
    }

    public double? GetStackedItemPrice(MongoId itemTpl, IEnumerable<Item> items)
    {
        return itemHelper.IsOfBaseclass(itemTpl, BaseClasses.AMMO_BOX)
            ? GetAmmoBoxPrice(items) * 1.15
            : handbookHelper.GetTemplatePrice(itemTpl) * 1.15;
    }

    public double? GetAmmoBoxPrice(IEnumerable<Item> items)
    {
        var total = 0D;
        foreach (var item in items)
        {
            if (itemHelper.IsOfBaseclass(item.Template, BaseClasses.AMMO))
            {
                total += handbookHelper.GetTemplatePrice(item.Template) * (item.Upd?.StackObjectsCount ?? 1);
            }
        }

        return total;
    }

    public void AddToCases(string[] casesToAdd, MongoId itemToAdd)
    {
        var tables = databaseService.GetTables();
        var items = tables.Templates.Items;

        foreach (var cases in casesToAdd)
        {
            foreach (var (item, _) in items)
            {
                if (items[item].Id != cases)
                {
                    return;
                }
                if (items[item].Properties?.Grids?.First().Properties?.Filters?.First().Filter is null)
                {
                    return;
                }
                items[item].Properties?.Grids?.First().Properties?.Filters?.First().Filter?.Add(itemToAdd);
            }
        }
    }

    public void ModifyContainerSize(MongoId containerToModify, int horizontal, int vertical)
    {
        var tables = databaseService.GetTables();
        var items = tables.Templates.Items;

        if (!items.TryGetValue(containerToModify, out var container))
        {
            return;
        }
        var grids = container.Properties?.Grids?.First();

        if (grids is null)
        {
            return;
        }

        grids.Properties.CellsH = horizontal;
        grids.Properties.CellsV = vertical;
    }

    public string FetchIdFromMap(string key, Dictionary<string, MongoId> map)
    {
        if (MongoId.IsValidMongoId(key))
        {
            return key;
        }

        if (map.TryGetValue(key, out var fetchedKey))
        {
            var finalKey = fetchedKey.ToString();

            return finalKey;
        }
        throw new ArgumentException($"'{key}' was not found in map.");
    }

    public T LoadConfig<T>(Assembly assembly, string dataPath, string configName)
    {
        var pathToMod = modHelper.GetAbsolutePathToModFolder(assembly);
        var finalPath = Path.Combine(pathToMod, dataPath);
        var config = modHelper.GetJsonDataFromFile<T>(finalPath, configName);

        return config;
    }

    public void WriteConfigFile<T>(T data, Assembly assembly, string dataPath, string configName)
    {
        if (data == null)
        {
            return;
        }
        var pathToMod = modHelper.GetAbsolutePathToModFolder(assembly);
        var finalPath = Path.Combine(pathToMod, dataPath);

        if (!Directory.Exists(finalPath))
        {
            Directory.CreateDirectory(finalPath);
        }
        if (!CheckFilePath(finalPath, configName))
        {
            var filePath = Path.Combine(finalPath, configName);
            var jsonString = jsonUtil.Serialize(data, true);
            File.Create(filePath).Dispose();

            var streamWriter = new StreamWriter(filePath);
            streamWriter.Write(jsonString);
            streamWriter.Flush();
            streamWriter.Close();
        }
        else if (CheckFilePath(finalPath, configName))
        {
            var filePath = Path.Combine(finalPath, configName);
            var jsonString = jsonUtil.Serialize(data, true);
            File.Delete(filePath);
            File.Create(filePath).Dispose();

            var streamWriter = new StreamWriter(filePath);
            streamWriter.Write(jsonString);
            streamWriter.Flush();
            streamWriter.Close();
        }
    }

    public void DumpDataMaps(Assembly assembly)
    {
        var dumpedDataPath = Path.Combine("db", "devFiles", "dumpedData");
        var itemMap = new SortedDictionary<string, MongoId>();
        var presetMap = new SortedDictionary<string, Preset>();
        var items = databaseService.GetItems();
        var locales = localeService.GetLocaleDb();
        var defaultPresets = presetHelper.GetAllPresets();

        foreach (var (itemId, data) in items)
        {
            try
            {
                itemMap.TryAdd(
                    locales[data.Parent + " Name"].ToUpperInvariant().Replace(" ", "_").Replace(".", "").Replace("(", "").Replace(")", "")
                        + "_"
                        + locales[itemId + " Name"].ToUpperInvariant().Replace(" ", "_").Replace(".", "").Replace("(", "").Replace(")", ""),
                    itemId
                );
            }
            catch (Exception ex)
            {
                ROLogger.Log(logger, $"Error adding item {itemId} to item map => " + ex, LogTextColor.Yellow);
                continue;
            }
        }

        foreach (var defaultPreset in defaultPresets)
        {
            try
            {
                presetMap.TryAdd(defaultPreset.Name.ToUpperInvariant().Replace(" ", "_"), defaultPreset);
            }
            catch (Exception ex)
            {
                ROLogger.Log(logger, $"Error adding preset {defaultPreset.Name} to preset map => " + ex, LogTextColor.Yellow);
                continue;
            }
        }

        try
        {
            WriteConfigFile<SortedDictionary<string, MongoId>>(itemMap, assembly, dumpedDataPath, "dumpedItemMap.json");
            WriteConfigFile<SortedDictionary<string, Preset>>(presetMap, assembly, dumpedDataPath, "dumpedPresetMap.json");
        }
        catch (Exception ex)
        {
            ROLogger.Log(logger, $"Error writing maps => " + ex);
        }
    }
}

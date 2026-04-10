using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;
using Path = System.IO.Path;

namespace RaidOverhaulMain.Helpers;

[Injectable(InjectionType.Singleton)]
public class ROJsonHelper(ISptLogger<ROJsonHelper> logger, JsonUtil jsonUtil)
{
    public List<T> LoadCombinedJsons<T>(string directoryPath)
    {
        var output = new List<T>();
        var files = Directory
            .GetFiles(directoryPath, "*.*", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".json") || f.EndsWith(".jsonc"))
            .ToArray();

        foreach (var filePath in files)
        {
            var data = jsonUtil.DeserializeFromFile<T>(filePath);

            if (data == null)
            {
                continue;
            }
            output.Add(data);
            ROLogger.LogDebug(logger, $"Loaded file: {filePath}");
        }

        return output;
    }

    public Dictionary<string, Dictionary<string, string>> LoadCombinedLocaleJsons(string directoryPath)
    {
        var output = new Dictionary<string, Dictionary<string, string>>();
        var files = Directory.GetFiles(directoryPath, "*.json").Concat(Directory.GetFiles(directoryPath, "*.jsonc")).ToArray();

        foreach (var filePath in files)
        {
            var localeCode = Path.GetFileNameWithoutExtension(filePath);
            var data = jsonUtil.DeserializeFromFile<Dictionary<string, string>>(filePath);

            if (data == null)
            {
                continue;
            }
            output[localeCode] = data;
            ROLogger.LogDebug(logger, $"Loaded locale file: {filePath}");
        }

        return output;
    }

    public List<Dictionary<MongoId, Quest>> LoadCombinedQuestJsons(string directory)
    {
        var output = new List<Dictionary<MongoId, Quest>>();
        var questDicts = LoadCombinedJsons<Dictionary<MongoId, Quest>>(directory);

        foreach (var questData in questDicts)
        {
            if (questData.Count > 0)
            {
                output.Add(questData);
            }
        }

        return output;
    }
}

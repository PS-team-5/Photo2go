using System.Collections;

namespace Photo2GoAPI.Configuration;

public static class EnvFileLoader
{
    public static IDictionary<string, string?> Load(string filePath)
    {
        var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        if (!File.Exists(filePath))
        {
            return values;
        }

        foreach (var rawLine in File.ReadAllLines(filePath))
        {
            var line = rawLine.Trim();

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
            {
                continue;
            }

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim().Trim('"');

            values[key] = value;
            Environment.SetEnvironmentVariable(key, value);
        }

        return values;
    }

    public static IDictionary<string, string?> ToConfigurationMap(IDictionary<string, string?> values)
    {
        var configurationMap = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (DictionaryEntry environmentVariable in Environment.GetEnvironmentVariables())
        {
            var key = environmentVariable.Key?.ToString();
            if (string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            configurationMap[key] = environmentVariable.Value?.ToString();
        }

        foreach (var pair in values)
        {
            configurationMap[pair.Key] = pair.Value;
        }

        // Translate flat .env keys into the nested options structure used by ASP.NET configuration binding.
        Map(configurationMap, "OPENAI_PROVIDER", "AI:Provider");
        Map(configurationMap, "OPENAI_API_KEY", "AI:ApiKey");
        Map(configurationMap, "OPENAI_MODEL", "AI:Model");
        Map(configurationMap, "OPENAI_BASE_URL", "AI:BaseUrl");
        Map(configurationMap, "OPENAI_TIMEOUT_SECONDS", "AI:TimeoutSeconds");

        return configurationMap;
    }

    private static void Map(
        IDictionary<string, string?> source,
        string environmentKey,
        string configurationKey)
    {
        if (source.TryGetValue(environmentKey, out var value) && !string.IsNullOrWhiteSpace(value))
        {
            source[configurationKey] = value;
        }
    }
}

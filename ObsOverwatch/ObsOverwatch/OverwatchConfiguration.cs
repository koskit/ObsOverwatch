using Newtonsoft.Json;
using System;
using System.IO;

namespace ObsOverwatch
{
    public class OverwatchConfiguration
    {
        #region Constants

        public const string ConfigFileName = "configuration.json";

        #endregion Constants

        #region Instance Properties

        public int ConsecutiveStrains { get; set; }

        public double StrainRestartLimit { get; set; }

        [JsonIgnore]
        public TimeSpan TimeAfterStopBeforeStart =>
            TimeSpan.FromSeconds(SecondsAfterStopBeforeStart);

        public int SecondsAfterStopBeforeStart { get; set; }

        public string WebsocketAddress { get; set; }

        public string WebsocketPassword { get; set; }

        #endregion Instance Properties

        #region Static Methods

        public static OverwatchConfiguration Read()
        {
            string configFile = Path.Combine(
                Directory.GetCurrentDirectory(),
                ConfigFileName);

            if (!File.Exists(configFile))
                CreateDefaultConfig(configFile);

            string configFileContent = File.ReadAllText(configFile);

            if (string.IsNullOrWhiteSpace(configFileContent))
                throw new Exception($"Configuration file: {configFile} \r\n" +
                    $"did not have any content inside. Please try re-installing the service.");

            try
            {
                return JsonConvert.DeserializeObject<OverwatchConfiguration>(configFileContent);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Configuration file: {configFile} \r\n" +
                    $"had invalid content that could not be read from the service. Inner exception: {ex.Message}\r\n\r\n", ex);
            }
        }

        public static void CreateDefaultConfig(string configFilePath)
        {
            OverwatchConfiguration defaultConfig = new OverwatchConfiguration()
            {
                ConsecutiveStrains = 5,
                SecondsAfterStopBeforeStart = 3,
                StrainRestartLimit = 1,
                WebsocketAddress = "ws://localhost:4444",
                WebsocketPassword = string.Empty
            };

            File.WriteAllText(configFilePath, JsonConvert.SerializeObject(defaultConfig, Formatting.Indented));
        }

        #endregion Static Methods
    }
}

using EFT.Weather;
using RaidOverhaul.Helpers;
using RaidOverhaul.Models;
using UnityEngine;

namespace RaidOverhaul.Controllers
{
    internal class SeasonalWeatherController : MonoBehaviour
    {
        private WeatherController _cachedWeatherController;
        private WeatherController WeatherController
        {
            get
            {
                if (_cachedWeatherController == null)
                {
                    _cachedWeatherController = WeatherController.Instance;
                }
                return _cachedWeatherController;
            }
        }

        private static float _cloudDensity;
        private static float _fog;
        private static float _rain;
        private static float _lightningThunderProb;
        private static float _temperature;
        private static float _windMagnitude;
        private static WeatherDebug.Direction _windDirection;
        private static Vector2 _topWindDirection;
        private const bool WeatherDebugBool = false;
        private static bool _weatherChangesRun = false;

        private void Update()
        {
            var seasonalProgression = ConfigController.ServerConfig.SeasonalProgression;
            var seasonConfig = ConfigController.SeasonConfig;
            var isReady = Utils.IsInRaid();

            if (!seasonalProgression || !isReady)
            {
                if (_weatherChangesRun)
                {
                    _weatherChangesRun = false;
                }
                if (_cachedWeatherController != null)
                {
                    _cachedWeatherController = null;
                }
                return;
            }

            if (_weatherChangesRun)
            {
                return;
            }

            if (!StormActive(seasonConfig))
            {
                return;
            }

            _weatherChangesRun = true;

            var weatherDebug = WeatherController.WeatherDebug;
            var topWindDir = Random.Range(1, 6);
            var windDir = Random.Range(1, 8);

            _cloudDensity = 1f;
            _fog = 0.004f;
            _lightningThunderProb = 0.8f;
            _rain = 1f;
            _temperature = 22f;
            _windDirection = (WeatherDebug.Direction)windDir;
            _windMagnitude = 0.6f;
            switch (topWindDir)
            {
                case 1:
                    _topWindDirection = Vector2.down;
                    break;
                case 2:
                    _topWindDirection = Vector2.left;
                    break;
                case 3:
                    _topWindDirection = Vector2.one;
                    break;
                case 4:
                    _topWindDirection = Vector2.right;
                    break;
                case 5:
                    _topWindDirection = Vector2.up;
                    break;
                case 6:
                    _topWindDirection = Vector2.zero;
                    break;
            }

            weatherDebug.Enabled = WeatherDebugBool;
            weatherDebug.CloudDensity = _cloudDensity;
            weatherDebug.Fog = _fog;
            weatherDebug.LightningThunderProbability = _lightningThunderProb;
            weatherDebug.Rain = _rain;
            weatherDebug.Temperature = _temperature;
            weatherDebug.TopWindDirection = _topWindDirection;
            weatherDebug.WindDirection = _windDirection;
            weatherDebug.WindMagnitude = _windMagnitude;
        }

        private static bool StormActive(SeasonalConfig seasonProgression)
        {
            return seasonProgression.SeasonsProgression >= 4 && seasonProgression.SeasonsProgression <= 6;
        }
    }
}

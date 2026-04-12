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
        private const bool WeatherDebug = false;
        private static bool _weatherChangesRun = false;

        public void DoStorm()
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

            _cloudDensity = 0.05f;
            _fog = 0.004f;
            _lightningThunderProb = 0.8f;
            _rain = 1f;
            _temperature = 22f;
            _windMagnitude = 0.6f;

            var weatherDebug = WeatherController.WeatherDebug;
            weatherDebug.Enabled = WeatherDebug;
            weatherDebug.CloudDensity = _cloudDensity;

            Utils.FogField.SetValue(weatherDebug, _fog);
            Utils.LighteningThunderField.SetValue(weatherDebug, _lightningThunderProb);
            Utils.RainField.SetValue(weatherDebug, _rain);
            Utils.TemperatureField.SetValue(weatherDebug, _temperature);

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

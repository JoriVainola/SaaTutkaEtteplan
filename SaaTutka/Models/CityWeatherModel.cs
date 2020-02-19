using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaaTutka.Models
{
    public class Forecast
    {
        public string WeatherDescription { get; set; }

        public string weatherIcon { get; set; }

        public double Temperature { get; set; }

        public DateTime DateNow { get; set; }

        public string Month { get; set; }

        public double Wind { get; set; }

        public double Humidity { get; set; }

        public double Precitipation { get; set; }
    }
    public class CityWeatherModel
    {
        public string Name { get; set; }

        public List<Forecast> Forecasts { get; set; } 
    }
}

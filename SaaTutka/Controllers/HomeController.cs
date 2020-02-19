using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SaaTutka.Models;

namespace SaaTutka.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            string API_KEY = "LISÄÄ API KEY TÄHÄN"; 
            Dictionary<string,string> cityIDs = new Dictionary<string, string>() {
                { "Tampere","634964"}, 
                { "Jyväskylä","655195"}, 
                { "Kuopio","650225"},
                { "Helsinki","658225"} }; 

            List<CityWeatherModel> cityweathermodels = new List<CityWeatherModel>();
                
            //Suoritetaan 3:n tunnin ennuste api kutsu kaikille kaupungeille
            foreach (KeyValuePair<string, string> city in cityIDs)
            {
                CityWeatherModel cityweathermodel = new CityWeatherModel();
                ForecastOutputModel forecastOutput = new ForecastOutputModel();
                using (var httpClient = new HttpClient())
                {
                    string CURRENT_URL = "http://api.openweathermap.org/data/2.5/forecast?id=" + city.Value + "&appid =" + API_KEY;
                    using (var response = await httpClient.GetAsync(CURRENT_URL))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        string TrimmedResponse = apiResponse.Replace(System.Environment.NewLine, "");
                        forecastOutput = JsonConvert.DeserializeObject<ForecastOutputModel>(apiResponse);
                    }
            }
             
            //Luodaan model tarpeellisesta datasta
            cityweathermodel.Name = city.Key;
            cityweathermodel.Forecasts = new List<Forecast>();

            foreach (List data in forecastOutput.list.Take(6))
            {
                Forecast forecast = new Forecast();
                forecast.WeatherDescription = data.weather[0].description;
                forecast.weatherIcon = "http://openweathermap.org/img/wn/" + data.weather[0].icon + "@2x.png";
                forecast.Temperature = data.main.temp - 273.15;
                forecast.DateNow = DateTime.ParseExact(data.dt_txt, "yyyy-MM-dd HH:mm:ss", null);
                forecast.Wind = data.wind.speed;
                forecast.Humidity = data.main.humidity;
                forecast.Precitipation = data.snow != null ? data.snow._3h :
                    data.rain != null ?data.rain._3h :
                    0;
       
                var day = forecast.DateNow.Day;
                var suffix = (day % 10 == 1 && day != 11) ? "st"
                            : (day % 10 == 2 && day != 12) ? "nd"
                            : (day % 10 == 3 && day != 13) ? "rd"
                            : "th";
                forecast.Month = forecast.DateNow.Date.ToString("MMM", CultureInfo.InvariantCulture) + " " + day.ToString() + suffix;

                cityweathermodel.Forecasts.Add(forecast);
            }
            cityweathermodels.Add(cityweathermodel);

        }

        return View(cityweathermodels);
    }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

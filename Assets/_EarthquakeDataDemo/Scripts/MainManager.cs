using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using EarthquakeFeature = _EarthquakeDataDemo.EarthquakeDataFetcher.EarthquakeFeature;

namespace _EarthquakeDataDemo
{
    public class MainManager : MonoBehaviour
    {
        public EarthquakeDataFetcher earthquakeDataFetcher;
        public Plotter plotter;

        public List<EarthquakeFeature> features;
    
        private void Awake()
        {
            if (earthquakeDataFetcher == null)
            {
                earthquakeDataFetcher = gameObject.AddComponent<EarthquakeDataFetcher>();
            }
            
            if (plotter == null)
            {
                plotter = gameObject.AddComponent<Plotter>();
            }
        }

        private void Start()
        {
            MagnitudePlot2D();
        }
        
        private async void MagnitudePlot2D()
        {
            string url = "https://earthquake.usgs.gov/fdsnws/event/1/query?format=geojson&starttime=2024-01-01&endtime=2024-05-01&minmagnitude=5";
            
            await FetchEarthquakeData(url); // Sets features list

            plotter.startDate = new DateTime(2024, 01, 01);
            plotter.endDate = new DateTime(2024, 05, 01);
            plotter.SetupSphericalCoordinatePlot(features);
            plotter.DrawSphericalCoordinatePlot();
        }
        
        private async Task FetchEarthquakeData(string url)
        {
            features = await earthquakeDataFetcher.GetEarthquakesAsync(url);
            
            if (features != null && features.Count > 0)
            {
                foreach (var earthquake in features)
                {
                    var time = System.DateTimeOffset.FromUnixTimeMilliseconds(earthquake.properties.time).DateTime;
                    Debug.Log($"Magnitude: {earthquake.properties.mag}, Location: {earthquake.properties.place}, Time: {time}, URL: {earthquake.properties.url}");
                }
            }
        }
    }
}

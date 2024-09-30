using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EarthquakeFeature = DataVisualizationDemo.EarthquakeDataFetcher.EarthquakeFeature;

namespace DataVisualizationDemo
{
    public class EarthquakeDemoManager : MonoBehaviour
    {
        public EarthquakeDataFetcher earthquakeDataFetcher;
        public Plotter plotter;

        private List<EarthquakeFeature> _features;
    
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
            plotter.SetupSphericalCoordinatePlot(_features);
            plotter.DrawSphericalCoordinatePlot();
        }
        
        private async Task FetchEarthquakeData(string url)
        {
            _features = await earthquakeDataFetcher.GetEarthquakesAsync(url);
            
            if (_features != null && _features.Count > 0)
            {
                foreach (var earthquake in _features)
                {
                    var time = System.DateTimeOffset.FromUnixTimeMilliseconds(earthquake.properties.time).DateTime;
                    Debug.Log($"Magnitude: {earthquake.properties.mag}, Location: {earthquake.properties.place}, Time: {time}, URL: {earthquake.properties.url}");
                }
            }
        }
    }
}

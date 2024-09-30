using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EarthquakeFeature = DataVisualizationDemo.EarthquakeDataFetcher.EarthquakeFeature;

namespace DataVisualizationDemo
{
    public class MainManager : MonoBehaviour
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
            // PlotSphere();
            AnimatedPlotSphere();
        }
        
        /// <summary>
        /// 
        /// </summary>
        public async void PlotSphere()
        {
            string url = "https://earthquake.usgs.gov/fdsnws/event/1/query?format=geojson&starttime=2024-01-01&endtime=2024-05-01&minmagnitude=5";
            
            await FetchEarthquakeData(url); // Sets features list

            plotter.plotType = Plotter.PlotType.SphericalCoordinatePlot;
            plotter.startDate = new DateTime(2024, 01, 01);
            plotter.endDate = new DateTime(2024, 01, 01);
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
                }
            }
        }
        
        public async void AnimatedPlotSphere()
        {
            string url = "https://earthquake.usgs.gov/fdsnws/event/1/query?format=geojson&starttime=2024-01-01&endtime=2024-05-01&minmagnitude=5";
            
            await FetchEarthquakeData(url); // Sets features list

            plotter.plotType = Plotter.PlotType.AnimatedSphericalCoordinatePlot;
            plotter.startDate = new DateTime(2024, 01, 01);
            plotter.endDate = new DateTime(2024, 05, 01);
            plotter.SetupSphericalCoordinatePlot(_features);
            plotter.DrawSphericalCoordinatePlot();
        }
    }
}

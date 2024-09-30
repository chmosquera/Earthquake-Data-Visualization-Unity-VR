using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace DataVisualizationDemo
{
    public class EarthquakeDataFetcher : MonoBehaviour
    {
        /// <summary>
        /// The 'properties' attributes of an earthquake in the USGS data.
        /// </summary>
        [System.Serializable]
        public class EarthquakeProperties
        {
            public double mag; // Magnitude
            public string place; // Location description
            public long time; // Time in Unix timestamp
            public string url; // More info link
        }
        
        /// <summary>
        /// The 'geometry' attributes of an earthquake in the USGS data.
        /// </summary>
        public class EarthquakeGeometry
        {
            public List<double> coordinates; // Longitude, Latitude, Depth
        }

        /// <summary>
        /// A single item of the Earthquake data.
        /// </summary>
        [System.Serializable]
        public class EarthquakeFeature
        {
            public string type;
            public EarthquakeProperties properties;
            public EarthquakeGeometry geometry;
        }
    
        [System.Serializable]
        public class EarthquakeApiResponse
        {
            public string type;
            public List<EarthquakeFeature> features;
        }
    
        /// <summary>
        ///  Sends a GET request to the USGS API to retrieve information about earthquakes. If successful, the JSON response
        /// is parsed.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<List<EarthquakeFeature>> GetEarthquakesAsync(string url)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                var operation = webRequest.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield();
                }
            
                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(webRequest.error);
                    return null;
                }

                string jsonResponse = webRequest.downloadHandler.text;
                EarthquakeApiResponse response = JsonConvert.DeserializeObject<EarthquakeApiResponse>(jsonResponse);
            
                return response.features;
            }
        }
    }
}
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace DataVisualizationDemo
{
    public class USGSElevationFetcher : MonoBehaviour
    {
        /// <summary>
        /// Queries for elevation, given a longitude (x) and latitude (y) from the API https://apps.nationalmap.gov/epqs/.
        /// </summary>
        string USGS_API_URL =
            "https://epqs.nationalmap.gov/v1/json?x={0:0.0}&y={1:0.0}&wkid=4326&units=Meters&includeDate=false";

        /// <summary>
        ///  Function to fetch elevation data given coordinates: (x: longitude, y: latitude).
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public async Task<float> FetchElevation(float longitude, float latitude)
        {
            try
            {
                string apiUrl = string.Format(USGS_API_URL, longitude, latitude);

                using (HttpClient client = new HttpClient())
                {
                    // Get the response from the API
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // Process the response and get the elevation
                    JObject data = JObject.Parse(jsonResponse);
                    string value = (string)data["value"];
                    if (float.TryParse(value, out float elevation))
                    {
                        return elevation;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Failed to fetch elevation data. This could be because the location is in the ocean, where elevation may be unknown. Default elevation " + e.Message);
            }

            return 0.0f;
        }
    }
}
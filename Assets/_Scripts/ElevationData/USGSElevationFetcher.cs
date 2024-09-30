using System.Collections;
using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class USGSElevationFetcher : MonoBehaviour
{
// The USGS Elevation API URL
    private string USGS_API_URL = "https://nationalmap.gov/epqs/pqs.php?x={0}&y={1}&units=Meters&output=json";

    // Function to fetch elevation data given lat/long
    public async Task<float> FetchElevation(float latitude, float longitude)
    {
        string apiUrl = string.Format(USGS_API_URL, longitude, latitude);

        using (HttpClient client = new HttpClient())
        {
            // Get the response from the API
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(jsonResponse);
                float elevation = (float)data["USGS_Elevation_Point_Query_Service"]["Elevation_Query"]["Elevation"];
                return elevation;
            }
            else
            {
                Debug.LogError("Failed to fetch elevation data.");
                return 0f;
            }
        }
    }
}

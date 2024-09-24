using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class USGSProceduralTerrainGenerator : MonoBehaviour
{
    public int gridResolution = 100; // Number of points along X and Z axis
    public float latStart = 34.0f; // Starting latitude
    public float lonStart = -118.0f; // Starting longitude
    public float latEnd = 35.0f; // Ending latitude
    public float lonEnd = -117.0f; // Ending longitude
    public USGSElevationFetcher elevationFetcher; // Elevation fetcher

    private Mesh terrainMesh;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    void Start()
    {
        // Initialize mesh and other components
        terrainMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = terrainMesh;

        GenerateTerrain();
    }

    async void GenerateTerrain()
    {
        for (int x = 0; x <= gridResolution; x++)
        {
            for (int z = 0; z <= gridResolution; z++)
            {
                float lat = Mathf.Lerp(latStart, latEnd, (float)x / gridResolution);
                float lon = Mathf.Lerp(lonStart, lonEnd, (float)z / gridResolution);

                // Fetch elevation for each lat/long pair
                float elevation = await elevationFetcher.FetchElevation(lat, lon);

                Vector3 vertexPosition = LatLongToCartesian(lat, lon, elevation);
                vertices.Add(vertexPosition);
            }
        }

        GenerateTriangles();
        UpdateMesh();
    }

    void GenerateTriangles()
    {
        // Generate triangle indices for the mesh
        for (int x = 0; x < gridResolution; x++)
        {
            for (int z = 0; z < gridResolution; z++)
            {
                int topLeft = x * (gridResolution + 1) + z;
                int bottomLeft = (x + 1) * (gridResolution + 1) + z;
                int topRight = x * (gridResolution + 1) + z + 1;
                int bottomRight = (x + 1) * (gridResolution + 1) + z + 1;

                // Create two triangles for each grid cell
                triangles.Add(topLeft);
                triangles.Add(bottomLeft);
                triangles.Add(topRight);

                triangles.Add(topRight);
                triangles.Add(bottomLeft);
                triangles.Add(bottomRight);
            }
        }
    }

    void UpdateMesh()
    {
        terrainMesh.Clear();
        terrainMesh.vertices = vertices.ToArray();
        terrainMesh.triangles = triangles.ToArray();
        terrainMesh.RecalculateNormals();
    }
    
    public Vector3 LatLongToCartesian(float latitude, float longitude, float elevation)
    {
        float earthRadius = 6371000f; // Earth radius in meters
        float latRad = latitude * Mathf.Deg2Rad;
        float lonRad = longitude * Mathf.Deg2Rad;

        float x = (earthRadius + elevation) * Mathf.Cos(latRad) * Mathf.Cos(lonRad);
        float z = (earthRadius + elevation) * Mathf.Cos(latRad) * Mathf.Sin(lonRad);
        float y = (earthRadius + elevation) * Mathf.Sin(latRad);

        return new Vector3(x, y, z);
    }
}
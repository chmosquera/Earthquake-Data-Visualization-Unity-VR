using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DataVisualizationDemo
{
    public class USGSProceduralTerrainGenerator : MonoBehaviour
    {
        public int gridResolution = 5; // Number of points along X and Z axis
        public float distanceKm = 50.0f;
        public float latStart = 34.0f; // Starting latitude
        public float lonStart = -118.0f; // Starting longitude
        public float latEnd = 35.0f; // Ending latitude
        public float lonEnd = -117.0f; // Ending longitude
        public USGSElevationFetcher elevationFetcher; // Elevation fetcher

        private Mesh terrainMesh;
        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();

        [SerializeField] private bool generateOnStart = true; 

        // Path to the text file where the data will be saved
        private string filePath = "Assets/Positions.txt";

        void Start()
        {
            // Initialize mesh and other components
            terrainMesh = new Mesh();
            MeshFilter filter = gameObject.GetComponent<MeshFilter>();
            if (filter == null)
            {
                filter = gameObject.AddComponent<MeshFilter>();
            }
            filter.mesh = terrainMesh;

            // File.WriteAllText(filePath, "Longitude, Latitude, Elevation\n");

            if (generateOnStart)
            {
                GenerateTerrain();
            }
        }

        public void GenerateTerrain(float longitude, float latitude)
        {
            // 1 degree of latitude is approximately 111 km, and longitude is 111 km * cos(latitude in radians) 
            double deltaLat = distanceKm / 111.0;
            double deltaLon = distanceKm / (111.0 * Math.Cos(latitude * Math.PI / 180.0));
            
            latStart = (float)(latitude - deltaLat);
            latEnd = (float)(latitude + deltaLat);
            lonStart = (float)(longitude - deltaLon);
            lonEnd = (float)(longitude + deltaLon);
            
            GenerateTerrain();
        }

        public async void GenerateTerrain()
        {
            // Convert the longitude, latitude, and elevation to get a normalized vertex in world space.
            for (int x = 0; x <= gridResolution; x++)
            {
                for (int z = 0; z <= gridResolution; z++)
                {
                    float lat = Mathf.Lerp(latStart, latEnd, (float)x / gridResolution);
                    float lon = Mathf.Lerp(lonStart, lonEnd, (float)z / gridResolution);

                    // Fetch elevation for each lat/long pair
                    float elevation = await elevationFetcher.FetchElevation(lon, lat);

                    // Normalize
                    float minElevation = -430f;
                    float maxElevation = 8848f;

                    Vector3 vertexPosition = LatLongToCartesian(lon, lat, elevation);
                    float normalizedLat = (lat - latStart) / (latEnd - latStart);
                    float normalizedLon = (lon - lonStart) / (lonEnd - lonStart);
                    float normalizedElevation = (elevation - minElevation) / (maxElevation - minElevation);

                    vertexPosition = new Vector3(normalizedLat, normalizedLon, normalizedElevation);

                    vertices.Add(vertexPosition);

                    // File.AppendAllText(filePath, $"{vertexPosition.x}, {vertexPosition.z}, {vertexPosition.y}\n");
                }
            }

            Debug.Log("Finished getting data. Generating terrain...");
            GenerateTriangles();
            UpdateMesh();
        }

        private void GenerateTriangles()
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

        private void UpdateMesh()
        {
            terrainMesh.Clear();
            terrainMesh.vertices = vertices.ToArray();
            terrainMesh.triangles = triangles.ToArray();
            terrainMesh.RecalculateNormals();
        }

        public Vector3 LatLongToCartesian(float longitude, float latitude, float elevation)
        {
            float earthRadius = 6371000f; // Earth radius in meters
            // float earthRadius = 1f;
            float lonRad = longitude * Mathf.Deg2Rad;
            float latRad = latitude * Mathf.Deg2Rad;

            float x = (earthRadius + elevation) * Mathf.Cos(latRad) * Mathf.Cos(lonRad);
            float z = (earthRadius + elevation) * Mathf.Cos(latRad) * Mathf.Sin(lonRad);
            float y = (earthRadius + elevation) * Mathf.Sin(latRad);

            return new Vector3(x, y, z);
        }
    }
}
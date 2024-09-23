using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using EarthquakeFeature = _EarthquakeDataDemo.EarthquakeDataFetcher.EarthquakeFeature;

namespace _EarthquakeDataDemo
{
	public class Plotter : MonoBehaviour
	{
		public enum PlotType
		{
			SphericalCoordinatePlot,
			AnimatedSphericalCoordinatePlot
		}
		public PlotType plotType = PlotType.AnimatedSphericalCoordinatePlot;

		public enum PlottingState
		{
			Prepare,
			Draw,
			Stop
		}
		private PlottingState plotState = PlottingState.Stop;
		
		[Header("Point settings")]
		
		/// <summary>
		/// A prefab to represent a point in the plot. If none, a sphere primitive is created by default.
		/// </summary>
		[SerializeField] private GameObject pointPrefab;
		
		private List<GameObject> pointObjects;
		
		[Header("Appearance Settings")]
		[SerializeField] private float pointSize = 0.1f;
		
		[Header("Spherical Coordinate Plot Settings")]
		public float radius = 5.0f;
		public bool animateByTime = false;
		public DateTime startDate;
		public DateTime endDate;
		public GameObject globePrefab;
		public float timer = 1.0f;
		private float _timer = 1.0f;

		private Dictionary<int, List<EarthquakeFeature>> _dayToEarthquakes = new Dictionary<int, List<EarthquakeFeature>>();
		private int _dayCount = 0;
		private int _maxDayCount;
		private GameObject _globe;
		
		
		private void Awake()
		{
			if (pointPrefab == null)
			{
				pointPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				pointPrefab.SetActive(false);
			}

			_dayToEarthquakes = new Dictionary<int, List<EarthquakeFeature>>();
		}
		
		private void Update()
		{
			if (plotType == PlotType.AnimatedSphericalCoordinatePlot)
			{
				if (plotState == PlottingState.Draw)
				{
					UpdateAnimatedSphericalCoordinatePlot();
				}

			}
		}

		/// <summary>
		/// Update function for the animated spherical coordinate plot.
		/// </summary>
		private void UpdateAnimatedSphericalCoordinatePlot()
		{
			if (_dayCount > _maxDayCount)
			{
				plotState = PlottingState.Stop;
				return;
			}
			
			_timer -= Time.deltaTime;
			if (_timer <= 0)
			{
				_dayCount++;
				DrawSphericalCoordinatePlot();
				_timer = 1.0f;
			}
			
		}

		private void Reset()
		{
			_timer = timer;
		}
		
		/// <summary>
		/// Sets up a plot for spherical coordinates by creating a globe and preparing longitude and latitude properties from USGS data.
		/// </summary>
		/// <param name="features"></param>
		public void SetupSphericalCoordinatePlot(List<EarthquakeFeature> features)
		{
			plotState = PlottingState.Prepare;
			
			for (int i = 0; i < features.Count; i++)
			{
				DateTime date = DateTimeOffset.FromUnixTimeMilliseconds(features[i].properties.time).DateTime;
				int dayIdx = (int)(date - startDate).TotalDays;
				if (_dayToEarthquakes.ContainsKey(dayIdx) == false)
				{
					_dayToEarthquakes.Add(dayIdx, new List<EarthquakeFeature>());
				}

				_dayToEarthquakes[dayIdx].Add(features[i]);
			}
			
			// Set up globe game object
			_globe = Instantiate(globePrefab, this.transform);
			float diameter = radius * 2.0f;
			_globe.transform.localScale = new Vector3(diameter, diameter, diameter);
			_globe.transform.position = transform.position;

			_maxDayCount = (int) (endDate - startDate).TotalDays;

			plotState = PlottingState.Draw;
		}

		/// <summary>
		/// Called every frame. Draws data points on the spherical coordinate plot. If animatedByTime is enabled, points will appear in order of the day the earthquakes occurred.
		/// </summary>
		public void DrawSphericalCoordinatePlot()
		{
			if (_dayToEarthquakes.ContainsKey(_dayCount) == false)
			{
				return;
			}
			
			List<EarthquakeFeature> quakes = _dayToEarthquakes[_dayCount];
			for (int i = 0; i < quakes.Count; i++)
			{
				float magnitude = (float)quakes[i].properties.mag;
				float longitude = (float)quakes[i].geometry.coordinates[0];
				float latitude = (float)quakes[i].geometry.coordinates[1];
				
				// Instantiate a new point
				GameObject point = Instantiate(pointPrefab, transform);
				
				Vector3 position = LatLonToSphere(latitude, longitude);
				point.transform.localPosition = position;
				point.transform.LookAt(_globe.transform.position);
                point.transform.localScale = new Vector3(pointSize, pointSize, pointSize);
			
				Color color = Color.Lerp(Color.green, Color.red, magnitude / 10f);
				Renderer pointRenderer = point.GetComponent<Renderer>();
				if (pointRenderer != null)
				{
					pointRenderer.material.color = color;
				}
			}
		}
		
		private Vector3 LatLonToSphere(float latitude, float longitude)
		{
			// Convert degrees to radians
			float latRad = latitude * Mathf.Deg2Rad;
			float lonRad = longitude * Mathf.Deg2Rad;

			// Calculate the 3D position on the sphere using spherical-to-cartesian conversion
			float x = radius * Mathf.Cos(latRad) * Mathf.Cos(lonRad);
			float y = radius * Mathf.Sin(latRad);
			float z = radius * Mathf.Cos(latRad) * Mathf.Sin(lonRad);

			return new Vector3(x, y, z);
		}
	}
}
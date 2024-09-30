using System;
using DataVisualizationDemo;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace DataVisualizationDemo
{
	public class UIManager : MonoBehaviour
	{

		public enum State
		{
			NoSelection,
			Selected
		}
		State state = State.NoSelection;
		
		[System.Serializable]
		public struct EarthquakeDetailsUI
		{
			[Header("General UI")]
			public TextMeshProUGUI locationDescription;
			public TextMeshProUGUI dateDescription;
			public TextMeshProUGUI urlDescription;
			public TextMeshProUGUI magDescription;

			[Header("Terrain generation UI")]
			public Button generateLowDensityTerrain;
			public Button generateMediumDensityTerrain;
			
			/// <summary>
			/// When in the process of generating terrain, this text displays how many fetches happened so far.
			/// </summary>
			public TextMeshProUGUI fetchestText;
			
			/// <summary>
			/// When in the process of generating terrain, this text displays the total fetches that will occur.
			/// The amount is based on the grid resolution.
			/// </summary>
			public TextMeshProUGUI totalFetchesText;

			[Header("Terrain generation settings")]
			public int lowRes;
			public int mediumRes;
		}

		public RectTransform noSelectionPanel;
		public RectTransform selectionPanel;
		public EarthquakeDetailsUI earthquakeDetailsUI;
		public USGSProceduralTerrainGenerator terrainGenerator;

		private Point selectedPoint;
		
		public void Awake()
		{
			// var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
			//
			// if (earthquakeDetailsUI.generateLowDensityTerrain != null)
			// {
			// 	earthquakeDetailsUI.generateLowDensityTerrain = rootVisualElement.Q<Button>("GenerateLowDensity");
			// 	earthquakeDetailsUI.generateLowDensityTerrain.clicked += GenerateLowDensityTerrain;
			// }
			//
			// if (earthquakeDetailsUI.generateMediumDensityTerrain != null)
			// {
			// 	earthquakeDetailsUI.generateMediumDensityTerrain = rootVisualElement.Q<Button>("GenerateMediumDensity");
			// 	earthquakeDetailsUI.generateMediumDensityTerrain.clicked += GenerateMediumDensityTerrain;
			// }

			if (terrainGenerator == null)
			{
				Debug.LogError("No terrain generator selected");
			}
		}

		private void Update()
		{
			// Open the correct panel if it's not already.
			if (state == State.NoSelection)
			{
				if (noSelectionPanel.gameObject.activeSelf == false)
				{
					ResetPanels();
					noSelectionPanel.gameObject.SetActive(true);
				}
			}
			else if (state == State.Selected)
			{
				if (selectionPanel.gameObject.activeSelf == false)
				{
					ResetPanels();
					selectionPanel.gameObject.SetActive(true);
				}
			}
		}

		private void ResetPanels()
		{
			noSelectionPanel.gameObject.SetActive(false);
			selectionPanel.gameObject.SetActive(false);
		}
		
		public void UpdateEarthquakeDetailsPanel(EarthquakeDataFetcher.EarthquakeFeature earthquakeFeature)
		{
			double mag = earthquakeFeature.properties.mag;
			String place = earthquakeFeature.properties.place;
			long date = earthquakeFeature.properties.time;
			string url = earthquakeFeature.properties.url;

			earthquakeDetailsUI.locationDescription.text = place;
			earthquakeDetailsUI.dateDescription.text = date.ToString();
			earthquakeDetailsUI.urlDescription.text = url;
			earthquakeDetailsUI.magDescription.text = mag.ToString();

			state = State.Selected;
		}

		/// <summary>
		/// Base function to generate terrain using the user interface.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="density"></param>
		private void GenerateTerrain(Point point, int density)
		{
			if (point == null)
			{
				Debug.LogError("Terrain cannot be generated because point is null.");
				return;
			}
			
			float lon = (float)point.earthquakeFeature.geometry.coordinates[0];
			float lat = (float)point.earthquakeFeature.geometry.coordinates[1];

			terrainGenerator.gridResolution = density;
			terrainGenerator.GenerateTerrain(lon, lat);
		}
		
		/// <summary>
		/// Generates a low resolution terrain of the earth, relative to the selected earthquake data point's location.
		/// Adjust the resolution size in the class parameters. 
		/// </summary>
		public void GenerateLowDensityTerrain()
		{
			Debug.Log("Generating low density terrain");
			GenerateTerrain(selectedPoint, earthquakeDetailsUI.lowRes);
		}

		/// <summary>
		/// Generates a medium resolution terrain of the earth, relative to the selected earthquake data point's location.
		/// This takes longer time due to the API fetches and should be done less frequently. For example, 10-by-10 grid
		///  takes up a few minutes. Adjust the resolution size in the class parameters.
		/// </summary>
		public void GenerateMediumDensityTerrain()
		{
			Debug.Log("Generating medium density terrain");
			GenerateTerrain(selectedPoint, earthquakeDetailsUI.mediumRes);
		}

		public void SelectPoint(Point point)
		{
			selectedPoint = point;
		}
	}
}
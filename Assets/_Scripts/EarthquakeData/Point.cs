using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using EarthquakeFeature = DataVisualizationDemo.EarthquakeDataFetcher.EarthquakeFeature;

namespace DataVisualizationDemo
{
	/// <summary>
	/// Represents a data point object of a USGS earthquake item. 
	/// </summary>
	[RequireComponent(typeof(Renderer))]
	public class Point : MonoBehaviour
	{
		/// <summary>
		/// The USGS earthquake data item associated with this point object.
		/// </summary>
		public EarthquakeFeature earthquakeFeature = null;

		private Color oldColor;
		private Renderer renderer;
		
		/// <summary>
		/// If this point should only appear for a certain amount of time, set to True. 
		/// </summary>
		public bool timedExistance = true;
		
		/// <summary>
		/// The amount of time in seconds in which the point should render. After this time, the data point is destroyed.
		/// </summary>
		public float timeToDisappear = 2.0f;
		
		private void Awake()
		{
			renderer = GetComponent<Renderer>();
			if (renderer == null)
			{
				Debug.LogError("Renderer is null.");
			}
		}

		private void Start()
		{
			oldColor = renderer.material.color;
		}

		public void StartTimedRender()
		{
			StartCoroutine((RenderForXSeconds()));
		}
		/// <summary>
		/// A coroutine to show the data point object for a specified amount of seconds before disappearing by destruction.
		/// </summary>
		/// <returns></returns>
		private IEnumerator RenderForXSeconds()
		{
			yield return new WaitForSeconds(timeToDisappear);
        
			Destroy(this.gameObject);
		}

		public void HighlightRender()
		{
			renderer.material.color = oldColor * 2.0f; // Brightens the color
			
		}

		public void UnhighlightRender()
		{
			renderer.material.color = oldColor;
		}

		public void SelectPoint()
		{
			var obj = GameObject.FindGameObjectWithTag("UIManager");
			UIManager uiManager = obj.gameObject.GetComponent<UIManager>();
			if (obj == null || uiManager== null)
			{
				Debug.LogError("Could not find UI manager using tag 'UIManager'.");
			}
			
			uiManager.SelectPoint(this);
			uiManager.UpdateEarthquakeDetailsPanel(this.earthquakeFeature);
		}
	}
   
}
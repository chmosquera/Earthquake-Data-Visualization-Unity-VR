using System.Collections;
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
		
		/// <summary>
		/// If this point should only appear for a certain amount of time, set to True. 
		/// </summary>
		public bool timedExistance = true;
		
		/// <summary>
		/// The amount of time in seconds in which the point should render. After this time, the data point is destroyed.
		/// </summary>
		public float timeToDisappear = 2.0f;

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
	}
   
}
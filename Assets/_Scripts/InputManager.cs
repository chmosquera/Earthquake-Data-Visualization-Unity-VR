using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DataVisualizationDemo
{
    /// <summary>
    /// Manages input handling and performs corresponding actions based on input. 
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public UIManager uiManager;
        public InputActionAsset inputAsset;
        private InputAction _selectAction;
        public LayerMask includeLayer;

        private void OnEnable()
        {
            var globeActionMap = inputAsset.FindActionMap("Globe");
            if (globeActionMap != null)
            {
                _selectAction = globeActionMap.FindAction("SelectPoint");
            }

            if (_selectAction != null)
            {
                _selectAction.Enable();
                _selectAction.performed += OnSelectPoint;
            }
        }

        private void OnDisable()
        {
            if (_selectAction != null)
            {
                _selectAction.Disable();
            }
        }

        /// <summary>
        /// Callback function for when a data point is left-clicked on with a mouse. 
        /// </summary>
        /// <param name="context"></param>
        private void OnSelectPoint(InputAction.CallbackContext context)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            int layerMask = 1 << includeLayer.value;
            if (Physics.Raycast(ray, out hit, layerMask))
            {
                GameObject hitObj = hit.transform.gameObject;
                if ((includeLayer & (1 << hit.collider.gameObject.layer)) != 0) // Check if hit object is in layer
                {
                    try
                    {
                        Point dataPoint = hitObj.GetComponent<Point>();
                        uiManager.SelectPoint(dataPoint);
                        uiManager.UpdateEarthquakeDetailsPanel(dataPoint.earthquakeFeature);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Unable to select a Point object and retrieve it's earthquake feature. " +
                                       e.Message);
                    }
                    
                }
            }
        }
    }
}
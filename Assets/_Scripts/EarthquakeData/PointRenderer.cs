using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PointRenderer : MonoBehaviour
{
    public float seconds = 2.0f;
    void Start()
    {
        StartCoroutine((RenderForXSeconds(seconds)));
    }

    IEnumerator RenderForXSeconds(float s)
    {
        yield return new WaitForSeconds(s);
        
        Destroy(this.gameObject);
    }
}

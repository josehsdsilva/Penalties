using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] List<Collider2D> objectsToEncaplsulate;
    private Camera cam;
    
    private void Start()
    {   
        cam = GetComponent<Camera>();
        CalculateOrthoSize();
    }

    private void CalculateOrthoSize()
    {
        var bounds = new Bounds();


        foreach (var col in objectsToEncaplsulate)
        {
            bounds.Encapsulate(col.bounds);
        }

        var vertical = bounds.size.y;
        var horizontal = bounds.size.x * cam.pixelHeight / cam.pixelWidth;

        var size = Mathf.Max(horizontal, vertical) * 0.6f;
        var center = bounds.center + new Vector3(0, 0, -10);

        cam.transform.position = center;
        cam.orthographicSize = size;
    }
}

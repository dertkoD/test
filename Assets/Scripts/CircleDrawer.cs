using UnityEngine;

public class CircleDrawer : MonoBehaviour
{
    [SerializeField] private Transform Player;
    [SerializeField] private float Radius = 5f;
    [SerializeField] private int Segments = 100;
    [SerializeField] private LineRenderer lineRenderer;
    
    void Start()
    {
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = Segments;

        UpdateCircle();
    }

    void Update()
    {
        UpdateCircle();

        transform.position = new Vector3(Player.position.x, transform.position.y, Player.position.z);
    }

    void UpdateCircle()
    {
        for (int i = 0; i < Segments; i++)
        {
            float angle = ((float)i / Segments) * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * Radius;
            float z = Mathf.Sin(angle) * Radius;
            lineRenderer.SetPosition(i, new Vector3(x, 0f, z));
        }
    }
}

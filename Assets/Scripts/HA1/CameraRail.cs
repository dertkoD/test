using UnityEngine;

public class CameraRail : MonoBehaviour
{
    [Header("Rail points")] public Transform startPoint;
    public Transform endPoint;

    [Header("Movement")] public float moveSpeed = 3f;

    public float smoothTime = 0.12f;

    public bool clampToRail = true;

    [Range(0f, 1f)] public float t = 0f;

    float _tVel;
    private bool isGameActive = true; // Game activity flag

    void Update()
    {
        if (!startPoint || !endPoint) return;
        
        // Check if game is active
        if (!isGameActive) return;

        float input = 0f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) input += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) input -= 1f;

        float railLength = Vector3.Distance(startPoint.position, endPoint.position);
        float dt = Time.deltaTime;

        float targetT = t;
        if (railLength > 0.0001f)
        {
            float tSpeed = moveSpeed / railLength;
            targetT = t + input * tSpeed * dt;
        }

        if (clampToRail) targetT = Mathf.Clamp01(targetT);

        t = Mathf.SmoothDamp(t, targetT, ref _tVel, smoothTime);

        transform.position = Vector3.Lerp(startPoint.position, endPoint.position, t);
    }
    
    // Method to control game activity
    public void SetGameActive(bool active)
    {
        isGameActive = active;
    }

    void OnDrawGizmos()
    {
        if (!startPoint || !endPoint) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(startPoint.position, endPoint.position);
        Gizmos.DrawSphere(startPoint.position, 0.08f);
        Gizmos.DrawSphere(endPoint.position, 0.08f);
    }
}
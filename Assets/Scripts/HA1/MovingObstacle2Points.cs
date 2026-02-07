using UnityEngine;

public class MovingObstacle2Points : MonoBehaviour
{
    [Header("Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Motion")]
    public float speed = 2f;

    public float waitTime = 0f;

    public float smoothTime = 0.1f;

    public bool useLocalPosition = false;

    Vector3 _vel;
    bool _toB = true;
    float _waitTimer = 0f;

    void Update()
    {
        if (!pointA || !pointB) return;

        Vector3 a = useLocalPosition ? pointA.localPosition : pointA.position;
        Vector3 b = useLocalPosition ? pointB.localPosition : pointB.position;

        Vector3 target = _toB ? b : a;

        if (_waitTimer > 0f)
        {
            _waitTimer -= Time.deltaTime;
            return;
        }

        Vector3 current = useLocalPosition ? transform.localPosition : transform.position;

        Vector3 next = Vector3.MoveTowards(current, target, speed * Time.deltaTime);

        if (smoothTime > 0f)
            next = Vector3.SmoothDamp(current, next, ref _vel, smoothTime);

        if (useLocalPosition) transform.localPosition = next;
        else transform.position = next;

        if ((next - target).sqrMagnitude < 0.0001f)
        {
            _toB = !_toB;
            _waitTimer = waitTime;
            _vel = Vector3.zero;
        }
    }

    void OnDrawGizmos()
    {
        if (!pointA || !pointB) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pointA.position, 0.08f);
        Gizmos.DrawSphere(pointB.position, 0.08f);
        Gizmos.DrawLine(pointA.position, pointB.position);
    }
}

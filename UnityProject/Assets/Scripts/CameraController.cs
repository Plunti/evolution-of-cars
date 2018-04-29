using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Zoom
    [SerializeField] private float CurrentZoomSize = 5f;
    [SerializeField] private float MinZoomSize = 1;
    [SerializeField] private float MaxZoomSize = 10;
    [SerializeField] private float ZoomSpeed = 0.5f;

    // Follow
    public Transform Target { get; set; }
    [SerializeField] private float MoveSpeed = 0.05f;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        // Zoom
        CurrentZoomSize = Mathf.Clamp(CurrentZoomSize - Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * CurrentZoomSize, MinZoomSize, MaxZoomSize);
        cam.orthographicSize = CurrentZoomSize;
    }

    private void FixedUpdate()
    {
        // Follow
        if (Target != null)
            transform.position = Vector2.Lerp(transform.position, Target.position, MoveSpeed);
    }
}
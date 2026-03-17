using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public class CameraFrameSetter : MonoBehaviour
{
    [SerializeField] private CinemachineCamera followCamera;
    [SerializeField] private Transform[] targets;
    [SerializeField] private float paddingMultiplier = 1.1f;
    [SerializeField] private float minFov = 25f;
    [SerializeField] private float maxFov = 75f;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private bool updateEveryFrame = true;

    private readonly Vector3[] boundsCorners = new Vector3[8];
    private float fovVelocity;

    private void Reset()
    {
        followCamera = GetComponent<CinemachineCamera>();
    }

    private void Awake()
    {
        if (followCamera == null)
        {
            followCamera = GetComponent<CinemachineCamera>();
        }
    }

    private void LateUpdate()
    {
        if (updateEveryFrame)
        {
            RefreshFraming(Time.deltaTime);
        }
    }

    [ContextMenu("Refresh Framing")]
    public void RefreshFraming()
    {
        RefreshFraming(0f);
    }

    private void RefreshFraming(float deltaTime)
    {
        if (followCamera == null || targets == null || targets.Length == 0)
        {
            return;
        }

        if (!TryBuildWorldBounds(out Bounds worldBounds))
        {
            return;
        }

        float targetFov = CalculateRequiredFov(worldBounds, followCamera.transform);

        LensSettings lens = followCamera.Lens;
        float nextFov = deltaTime > 0f
            ? Mathf.SmoothDamp(lens.FieldOfView, targetFov, ref fovVelocity, smoothTime, Mathf.Infinity, deltaTime)
            : targetFov;

        lens.FieldOfView = Mathf.Clamp(nextFov, minFov, maxFov);
        followCamera.Lens = lens;
    }

    private bool TryBuildWorldBounds(out Bounds worldBounds)
    {
        worldBounds = default;
        bool hasBounds = false;

        for (int i = 0; i < targets.Length; i++)
        {
            Transform target = targets[i];
            if (target == null)
            {
                continue;
            }

            Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
            bool foundRenderer = false;

            for (int j = 0; j < renderers.Length; j++)
            {
                Renderer currentRenderer = renderers[j];
                if (currentRenderer == null || !currentRenderer.enabled)
                {
                    continue;
                }

                if (!hasBounds)
                {
                    worldBounds = currentRenderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    worldBounds.Encapsulate(currentRenderer.bounds);
                }

                foundRenderer = true;
            }

            if (!foundRenderer)
            {
                if (!hasBounds)
                {
                    worldBounds = new Bounds(target.position, Vector3.zero);
                    hasBounds = true;
                }
                else
                {
                    worldBounds.Encapsulate(target.position);
                }
            }
        }

        return hasBounds;
    }

    private float CalculateRequiredFov(Bounds bounds, Transform cameraTransform)
    {
        FillBoundsCorners(bounds);

        float aspect = Mathf.Max(0.01f, (float)Screen.width / Screen.height);
        float requiredHalfAngle = 0f;

        for (int i = 0; i < boundsCorners.Length; i++)
        {
            Vector3 localPoint = cameraTransform.InverseTransformPoint(boundsCorners[i]);

            if (localPoint.z <= 0.01f)
            {
                return maxFov;
            }

            float verticalHalfAngle = Mathf.Atan2(Mathf.Abs(localPoint.y), localPoint.z);
            float horizontalAsVerticalHalfAngle = Mathf.Atan2(Mathf.Abs(localPoint.x), localPoint.z * aspect);

            requiredHalfAngle = Mathf.Max(requiredHalfAngle, verticalHalfAngle);
            requiredHalfAngle = Mathf.Max(requiredHalfAngle, horizontalAsVerticalHalfAngle);
        }

        float requiredFov = requiredHalfAngle * 2f * Mathf.Rad2Deg * paddingMultiplier;
        return Mathf.Clamp(requiredFov, minFov, maxFov);
    }

    private void FillBoundsCorners(Bounds bounds)
    {
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        boundsCorners[0] = new Vector3(min.x, min.y, min.z);
        boundsCorners[1] = new Vector3(min.x, min.y, max.z);
        boundsCorners[2] = new Vector3(min.x, max.y, min.z);
        boundsCorners[3] = new Vector3(min.x, max.y, max.z);
        boundsCorners[4] = new Vector3(max.x, min.y, min.z);
        boundsCorners[5] = new Vector3(max.x, min.y, max.z);
        boundsCorners[6] = new Vector3(max.x, max.y, min.z);
        boundsCorners[7] = new Vector3(max.x, max.y, max.z);
    }
}
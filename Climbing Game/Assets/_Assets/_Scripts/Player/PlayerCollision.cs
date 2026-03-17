using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerCollision : MonoBehaviour {
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private CapsuleCollider capsule;
    [SerializeField] private LayerMask collisionMask = ~0;
    [SerializeField] private float skinWidth = 0.02f;
    [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;

    private readonly RaycastHit[] castHits = new RaycastHit[16];
    private readonly Collider[] overlapHits = new Collider[16];

    private Logs currentLog;
    private bool rightJump;
    private bool hasTriggeredHazard;
    private Vector3 previousPosition;
    private Rigidbody rb;

    private void Awake(){
        rb = GetComponent<Rigidbody>();

        if (capsule == null)
        {
            capsule = GetComponent<CapsuleCollider>();
        }

        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        previousPosition = transform.position;
    }

    private void LateUpdate(){
        tr.gameObject.SetActive(false);

        Physics.SyncTransforms();

        UpdateSweptContacts();
        UpdateFacing();

        previousPosition = transform.position;
    }

    public void TurnRight(){
        rightJump = true;
    }

    public void TurnLeft(){
        rightJump = false;
    }

    public void CollidedWithHazards(){
        if (hasTriggeredHazard)
        {
            return;
        }

        hasTriggeredHazard = true;
        rb.linearVelocity = Vector3.zero;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90f, transform.eulerAngles.z);
        Invoke(nameof(InvokeJump), 0.5f);
    }

    private void InvokeJump(){
        rb.useGravity = true;
        rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
    }

    private void UpdateSweptContacts(){
        currentLog = null;

        Vector3 movement = transform.position - previousPosition;
        float distance = movement.magnitude;

        GetCapsuleWorldPoints(transform.position, out Vector3 point1, out Vector3 point2, out float radius);

        if (distance > 0.0001f)
        {
            Vector3 direction = movement / distance;
            int hitCount = Physics.CapsuleCastNonAlloc(
                point1,
                point2,
                Mathf.Max(0.001f, radius - skinWidth),
                direction,
                castHits,
                distance + skinWidth,
                collisionMask,
                triggerInteraction);

            for (int i = 0; i < hitCount; i++)
            {
                ProcessCollider(castHits[i].collider);
            }
        }

        int overlapCount = Physics.OverlapCapsuleNonAlloc(
            point1,
            point2,
            Mathf.Max(0.001f, radius - skinWidth),
            overlapHits,
            collisionMask,
            triggerInteraction);

        for (int i = 0; i < overlapCount; i++)
        {
            ProcessCollider(overlapHits[i]);
        }
    }

    private void ProcessCollider(Collider hitCollider)
    {
        if (hitCollider == null || hitCollider.transform == transform)
        {
            return;
        }

        if (hitCollider.TryGetComponent(out Logs hitLog) || hitCollider.GetComponentInParent<Logs>() != null)
        {
            currentLog = hitCollider.GetComponentInParent<Logs>();
        }

        Interactable interactable = hitCollider.GetComponent<Interactable>();
        if (interactable == null)
        {
            interactable = hitCollider.GetComponentInParent<Interactable>();
        }

        if (interactable != null)
        {
            interactable.Interact(this);
        }
    }

    private void UpdateFacing()
    {
        if (currentLog != null)
        {
            Vector3 dir = (currentLog.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg - 90f;
            transform.localRotation = Quaternion.AngleAxis(angle, Vector3.up);
            return;
        }

        transform.localRotation = rightJump
            ? Quaternion.Euler(0f, 0f, 0f)
            : Quaternion.Euler(0f, -180f, 0f);
    }

    private void GetCapsuleWorldPoints(Vector3 worldCenter, out Vector3 point1, out Vector3 point2, out float radius)
    {
        Vector3 center = worldCenter + transform.rotation * capsule.center;
        radius = capsule.radius * Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.z));

        float height = Mathf.Max(capsule.height * Mathf.Abs(transform.lossyScale.y), radius * 2f);
        float offset = height * 0.5f - radius;

        Vector3 axis = transform.up;
        point1 = center + axis * offset;
        point2 = center - axis * offset;
    }
	private void OnDrawGizmos()
	{
		if (capsule == null)
		{
			return;
		}

		GetCapsuleWorldPoints(transform.position, out Vector3 point1, out Vector3 point2, out float radius);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(point1, radius);
		Gizmos.DrawWireSphere(point2, radius);
		Gizmos.DrawLine(point1 + transform.right * radius, point2 + transform.right * radius);
		Gizmos.DrawLine(point1 - transform.right * radius, point2 - transform.right * radius);
		Gizmos.DrawLine(point1 + transform.forward * radius, point2 + transform.forward * radius);
		Gizmos.DrawLine(point1 - transform.forward * radius, point2 - transform.forward * radius);
	}
}

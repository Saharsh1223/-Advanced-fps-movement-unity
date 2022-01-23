using UnityEngine;

public class GrapplingGun : MonoBehaviour
{

    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    private float maxDistance = 100f;
    private SpringJoint joint;
    public GunSway gunSway;
    public GameObject debugAssist;
    public float aimAssistSize = 1f;
    public float debugSmoothSpeed = 8f;
    public float debugSmoothMultiplier = 1.5f;
    public float spring = 4.5f;
    public float damper = 0.24f;
    public float massScale = 4.5f;
    public PlayerMovement playerMovement;
    public CalculateSpeed playerSpeed;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }

        if (IsGrappling())
        {
            gunSway.enabled = false;
        }
        else
        {
            gunSway.enabled = true;
        }

        if (Physics.SphereCast(camera.position, aimAssistSize, camera.forward, out RaycastHit hit, maxDistance, whatIsGrappleable))
        {
            debugAssist.SetActive(true);

            if (playerMovement.isMoving && !playerMovement.isGrounded || playerMovement.isMoving && playerMovement.isGrounded)
            {
                debugAssist.transform.position = hit.point;
                LerpMove(hit);
            }
            else
            {
                LerpMove(hit);
            }
        }
        else
        {
            debugAssist.SetActive(false);
            CancelInvoke();
        }
    }

    private void FixedUpdate()
    {
        
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.SphereCast(camera.position, aimAssistSize, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //The distance grapple will try to keep from grapple point.
            joint.maxDistance = distanceFromPoint * 0.85f;
            joint.minDistance = distanceFromPoint * 0.2f;

            joint.spring = spring;
            joint.damper = damper;
            joint.massScale = massScale;
        }
    }

    void LerpMove(RaycastHit hit)
    {
        Vector3 previousPos = debugAssist.transform.position;
        debugAssist.transform.position = Vector3.Lerp(previousPos, hit.point, Time.fixedDeltaTime * debugSmoothSpeed * debugSmoothMultiplier);
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple()
    {
        Destroy(joint);
        gunSway.enabled = true;
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
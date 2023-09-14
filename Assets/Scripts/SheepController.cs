using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour
{
    [HideInInspector] public float speed = 0f;
    [HideInInspector] public float turnSpeed = 0f;
    [HideInInspector] public float neighborRadius = 0f;
    [HideInInspector] public float avoidanceRadius = 0f;
    [HideInInspector] public float fieldAreaRadius = 0;
    [HideInInspector] public float cursorAvoidanceRadius = 0;
    [HideInInspector] public float viewAngle = 0f;
    [HideInInspector] public float avoidanceForce = 0f;
    [HideInInspector] public float alignmentForce = 0f;
    [HideInInspector] public float cohesionForce = 0f;
    [HideInInspector] public bool avoidance = false;
    [HideInInspector] public bool alignment = false;
    [HideInInspector] public bool cohesion = false;

    private bool goal = false;

    private HerdController herdController;
    private GameManager gameManager;

    private void Start()
    {
        herdController = HerdController.instance;
        gameManager = GameManager.instance;
    }

    public void Movement(List<SheepController> sheepList)
    {
        Vector3 direction = Vector3.zero;

        Vector3 avoidanceDirection = Vector3.zero;
        int avoidanceCount = 0;
        Vector3 alignmentDirection = Vector3.zero;
        int alignmentCount = 0;
        Vector3 cohesionDirection = Vector3.zero;
        int cohesionCount = 0;

        Vector3 thisPos = transform.position;

        foreach (SheepController sheep in sheepList)
        {
            if (sheep == this)
                continue;

            Vector3 otherPos = sheep.transform.position;

            float distance = Vector3.Distance(thisPos, otherPos);

            if (distance < avoidanceRadius)
            {
                avoidanceDirection += (thisPos - otherPos);
                avoidanceCount++;
            }

            if (distance < neighborRadius && Vector3.Angle(transform.forward, otherPos - thisPos) < viewAngle)
            {
                alignmentDirection += sheep.transform.forward;
                alignmentCount++;

                cohesionDirection += (otherPos - thisPos);
                cohesionCount++;
            }
        }

        // avoidance
        if (avoidanceCount > 0 && avoidance)
        {
            avoidanceDirection /= avoidanceCount;
            avoidanceDirection = avoidanceDirection.normalized;
            avoidanceDirection *= avoidanceForce;
            direction += avoidanceDirection;
        }

        // alignment
        if (alignmentCount > 0 && alignment)
        {
            alignmentDirection /= alignmentCount;
            alignmentDirection = alignmentDirection.normalized;
            alignmentDirection *= alignmentForce;
            direction += alignmentDirection;
        }

        // cohesion
        if (cohesionCount > 0 && cohesion)
        {
            cohesionDirection /= cohesionCount;
            cohesionDirection = cohesionDirection.normalized;
            cohesionDirection *= cohesionForce;
            direction += cohesionDirection;
        }

        // obstacle avoidance
        float obstacleRadius = 10f;
        RaycastHit castInfo;
        if (Physics.Raycast(thisPos, transform.forward, out castInfo, obstacleRadius, LayerMask.GetMask("Obstacles")))
            direction = ((castInfo.point + castInfo.normal) - thisPos).normalized;

        // field area avoidance
        if (Vector3.Distance(thisPos, Vector3.zero) > fieldAreaRadius)
        {
            Vector3 centerDirection = Vector3.zero - thisPos;
            centerDirection = centerDirection.normalized;
            centerDirection *= 10f;
            direction += centerDirection;
        }

        // cursor avoidance
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, 0, 0));
        Vector3 hitPoint = Vector3.one * float.MaxValue;
        if (plane.Raycast(ray, out float hitDistance))
        {
            hitPoint = ray.GetPoint(hitDistance);
        }

        if (Vector3.Distance(thisPos, hitPoint) < cursorAvoidanceRadius)
        {
            Vector3 cursorDirection = thisPos - hitPoint;
            cursorDirection = cursorDirection.normalized;
            cursorDirection *= 4f;
            direction += cursorDirection;
        }

        if (goal)
        {
            Vector3 goalDirection = new Vector3(0, 0, -55) - thisPos;
            goalDirection = goalDirection.normalized;
            direction = goalDirection;
            if (Vector3.Distance(thisPos, new Vector3(0, 0, -55)) < 1f)
            {
                herdController.sheepList.Remove(this);
                Destroy(gameObject);
            }
        }

        if (direction != Vector3.zero)
        {
            direction = direction.normalized;
            direction.y = 0f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
        }

        transform.position += transform.TransformDirection(Vector3.forward) * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Goal"))
        {
            goal = true;
            gameManager.sheep++;
        }
    }
}
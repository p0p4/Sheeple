using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerdController : MonoBehaviour
{
    public SheepController sheepPrefab;
    public List<SheepController> sheepList;
    public GameObject fieldArea;

    [Header("Herd Settings")]

    [Range(1, 500)]
    public int sheepCount = 100;
    [SerializeField] private float speed = 0f;
    [SerializeField] private float turnSpeed = 0f;
    [SerializeField] private float neighborRadius = 0f;
    [SerializeField] private float avoidanceRadius = 0f;
    [SerializeField] private float fieldAreaRadius = 0f;
    [SerializeField] private float cursorAvoidanceRadius = 0f;
    [SerializeField] private float viewAngle = 0f;
    [SerializeField] private float avoidanceForce = 0f;
    [SerializeField] private float alignmentForce = 0f;
    [SerializeField] private float cohesionForce = 0f;
    [SerializeField] private bool avoidance;
    [SerializeField] private bool alignment;
    [SerializeField] private bool cohesion;
    private GameManager gameManager;

    public static HerdController instance;
    private void Awake()
    {
        instance = this;
    }

    public void Init()
    {
        if (sheepList != null)
        {
            foreach (SheepController sheep in sheepList)
            {
                Destroy(sheep.gameObject);
            }
            sheepList.Clear();
        }

        sheepList = new List<SheepController>();
        for (int i = 0; i < sheepCount; i++)
        {
            SpawnSheep(sheepPrefab.gameObject);
        }
    }

    private void Update()
    {
        if (!GameManager.instance.start)
            return;

        for (int i = 0; i < sheepList.Count; i++)
        {
            if (sheepList[i] == null)
            {
                sheepList.RemoveAt(i);
                i--;
                continue;
            }
            sheepList[i].alignment = alignment;
            sheepList[i].cohesion = cohesion;
            sheepList[i].avoidance = avoidance;
            sheepList[i].Movement(sheepList);
        }
    }

    private void SpawnSheep(GameObject sheepPrefab)
    {
        GameObject sheepInstance = Instantiate(sheepPrefab);

        sheepInstance.transform.position = new Vector3(
            Random.Range((-fieldArea.transform.localScale.x + 20) / 2, (fieldArea.transform.localScale.x - 20) / 2),
            0.5f,
            Random.Range((-fieldArea.transform.localScale.y + 20) / 2, (fieldArea.transform.localScale.y - 20) / 2)
        );
        sheepInstance.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        SheepController sheepController = sheepInstance.GetComponent<SheepController>();

        sheepController.speed = speed;
        sheepController.turnSpeed = turnSpeed;
        sheepController.neighborRadius = neighborRadius;
        sheepController.avoidanceRadius = avoidanceRadius;
        sheepController.viewAngle = viewAngle;
        sheepController.avoidanceForce = avoidanceForce;
        sheepController.alignmentForce = alignmentForce;
        sheepController.cohesionForce = cohesionForce;
        sheepController.avoidance = avoidance;
        sheepController.alignment = alignment;
        sheepController.cohesion = cohesion;
        sheepController.fieldAreaRadius = fieldAreaRadius;
        sheepController.cursorAvoidanceRadius = cursorAvoidanceRadius;

        sheepList.Add(sheepController);
    }
}
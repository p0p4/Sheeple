using System.Collections.Generic;
using UnityEngine;

public class HerdController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SheepController sheepPrefab;
    [SerializeField] private GameObject fieldArea;
    [SerializeField] private GameObject herd;

    [Header("Settings")]

    [Range(1, 500)]
    public int sheepCount = 0;
    [SerializeField] private float speed = 0f;
    [SerializeField] private float turnSpeed = 0f;
    [SerializeField] private float neighborRadius = 0f;
    [SerializeField] private float avoidanceRadius = 0f;
    [SerializeField] private float fieldAreaRadius = 0f;
    public float cursorAvoidanceRadius = 0f;
    [SerializeField] private float viewAngle = 0f;

    [Header("Forces")]
    [SerializeField] private float avoidanceForce = 0f;
    [SerializeField] private float alignmentForce = 0f;
    [SerializeField] private float cohesionForce = 0f;
    [SerializeField] private bool avoidance;
    [SerializeField] private bool alignment;
    [SerializeField] private bool cohesion;

    [HideInInspector] public List<SheepController> sheepList;

    GameManager gameManager;

    public static HerdController instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameManager = GameManager.instance;
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
        GameObject sheepInstance = Instantiate(sheepPrefab, herd.transform);

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
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI sheepText;
    [SerializeField] private GameObject cursorRadius;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject quitButton;

    HerdController herdController;

    [Header("Settings")]
    [SerializeField] private float time = 0f;
    [HideInInspector] public bool start = false;
    [HideInInspector] public int sheep = 0;
    private float timeBackup = 0f;

    // singleton
    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        herdController = HerdController.instance;

        Time.timeScale = 0;
        timeBackup = time;
        startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
        startButton.SetActive(true);

        Init();
    }

    private void Init()
    {
        timerText.text = timerText.text = "00:00";
        sheepText.text = string.Format("{0:0}/{1:0}", sheep, herdController.sheepCount);
    }

    private void Update()
    {
        time -= Time.deltaTime;
        timerText.text = TimeFormat(time);
        sheepText.text = string.Format("{0:0}/{1:0}", sheep, herdController.sheepCount);

        if (time <= 0 || sheep == herdController.sheepCount)
        {
            Time.timeScale = 0;
            if (time <= 0) timerText.text = "00:00";
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Restart";
            startButton.SetActive(true);
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, 0, 0));
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 point = ray.GetPoint(distance);
            cursorRadius.transform.position = point;
            cursorRadius.transform.localScale = new Vector3(1, 0.001f, 1) * 2 * herdController.cursorAvoidanceRadius;
        }
    }

    private string TimeFormat(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        Init();
        herdController.Init();
        startButton.SetActive(false);
        time = timeBackup;
        sheep = 0;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
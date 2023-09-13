using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI sheepText;
    [SerializeField] private float time = 60f;
    private float timeBackup = 0f;
    [HideInInspector] public float sheep = 0f;
    public bool start = false;

    public GameObject button;
    [SerializeField] private GameObject cursorRadius;

    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        timeBackup = time;
        start = false;
        button.SetActive(true);
        button.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0}:{1:00}", minutes, seconds);

        sheepText.text = string.Format("{0:0}/{1:0}", sheep, HerdController.instance.sheepCount);
    }

    void Init()
    {
        button.SetActive(true);
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0}:{1:00}", minutes, seconds);

        sheepText.text = string.Format("{0:0}/{1:0}", sheep, HerdController.instance.sheepCount);
    }

    void Update()
    {
        if (!start)
            return;

        time -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0}:{1:00}", minutes, seconds);

        sheepText.text = string.Format("{0:0}/{1:0}", sheep, HerdController.instance.sheepCount);

        if (time <= 0)
        {
            Time.timeScale = 0;
            timerText.text = "00:00";
            button.SetActive(true);
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Restart";
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, 0, 0));
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 point = ray.GetPoint(distance);
            cursorRadius.transform.position = point;
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        Init();
        HerdController.instance.Init();
        button.SetActive(false);
        start = true;
        time = timeBackup;
        sheep = 0f;
    }
}
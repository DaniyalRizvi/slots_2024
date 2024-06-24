using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mkey;

public class EventShowRandomizer : MonoBehaviour
{
    public static EventShowRandomizer Instance;
    [SerializeField] private PopUpsController[] events;

    private bool IsFirstScene;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        IsFirstScene = true;
    }

    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    void OnActiveSceneChanged(Scene previousScene, Scene newScene)
    {
        if (!IsFirstScene)
            ShowEvent();
        else
            IsFirstScene = false;
    }

    private void ShowEvent()
    {
        print("ONLEVELLOADED");
        float rand = Random.Range(0f, 1f);

        if (rand < 0.25f)
        {
            print("EVENTLOADED");
            int eventRand = Random.Range(0, events.Length);
            GetComponent<ShowGuiPopUp>().ShowPopUp(events[eventRand]);
        }
        else
            print("NOT LOADING EVENT");
    }
}

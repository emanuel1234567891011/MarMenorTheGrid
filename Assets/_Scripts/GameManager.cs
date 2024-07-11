using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{

    public float timeScale = 1;

    public bool Playing;
    public float CurrentSimTime = 0;

    private void Update()
    {
        if (Playing)
            CurrentSimTime += Time.deltaTime;
    }

    public void StartSim()
    {
        Playing = true;
    }

    public void PauseSim()
    {
        Playing = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void AddDrones(int count)
    {

    }
}

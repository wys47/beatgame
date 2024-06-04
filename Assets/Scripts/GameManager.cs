using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private WaitForSeconds waitForMenuExit = new WaitForSeconds(0.5f);

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void onPlayButtonUp()
    {
        StartCoroutine(gameSceneLoad());
    }

    private IEnumerator gameSceneLoad()
    {
        yield return waitForMenuExit;
        SceneManager.LoadSceneAsync("GameScene");
    }
}

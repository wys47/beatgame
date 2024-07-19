using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject titleManagerObj;
    public Animator titleAnim;

    public GameObject homeManagerObj;

    private WaitForSeconds waitForTitleExit = new WaitForSeconds(0.5f);
    private WaitForSeconds waitForMenuExit = new WaitForSeconds(0.5f);

    void Awake()
    {
        titleManagerObj.SetActive(true);
        homeManagerObj.SetActive(false);
    }

    //타이틀화면 버튼
    public void OnStartButtonUp()
    {
        StartCoroutine(homeActivate());
    }

    public void onPlayButtonUp()
    {
        StartCoroutine(gameSceneLoad());
    }

    private IEnumerator homeActivate()
    {
        titleAnim.SetTrigger("disabled");

        yield return waitForMenuExit;

        homeManagerObj.SetActive(true);
    }

    private IEnumerator gameSceneLoad()
    {
        yield return waitForMenuExit;
        SceneManager.LoadSceneAsync("GameScene");
    }
}

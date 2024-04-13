using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public GameObject scoreBoardCSObj;
    public ScoreBoardCS scoreBoardCS;

    public GameObject[] menuButtonObj;
    public RectTransform[] menuButtonTr;
    public Image[] menuButtonImage;
    public Sprite[] menuButtonSprite;
    private bool menuOpen;
    private bool menuOnProcess;

    private WaitForSeconds[] waitForSec = { new WaitForSeconds(0.01f)};

    void OnEnable()
    {
        scoreBoardCSObj.SetActive(true);
        scoreBoardCS.activate();
        scoreBoardCSObj.SetActive(false);

        menuButtonObj[1].SetActive(true);
        for (int i = 2; i <= 4; ++i) menuButtonObj[i].SetActive(false);
        for (int i = 1; i <= 4; ++i) menuButtonImage[i].sprite = menuButtonSprite[2 * i - 1];
        menuOpen = false;
        menuOnProcess = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) onMenuButtonDown(1);
        if (Input.GetKeyUp(KeyCode.Space)) onMenuButtonUp(1);

    }

    public void onMenuButtonDown(int n)
    {
        if (!menuOnProcess)
        {
            if (n == 1) StartCoroutine(openAndCloseMenu());
            menuButtonImage[n].sprite = menuButtonSprite[2 * n];
        }
    }
    public void onMenuButtonUp(int n)
    {
        if (n == 1) menuButtonImage[n].sprite = menuButtonSprite[menuOpen ? 9 : 1];
        else menuButtonImage[n].sprite = menuButtonSprite[2 * n - 1];
    }
    private IEnumerator openAndCloseMenu()
    {
        if (!menuOpen)
        {
            menuOpen = true;
            menuOnProcess = true;
            for (int i = 2; i <= 4; ++i)
            {
                menuButtonObj[i].SetActive(true);
                menuButtonTr[i].anchoredPosition = Vector2.right * (i - 2) * 120;
                for (int k = 1; k <= 10; ++k)
                {
                    yield return waitForSec[0];
                    menuButtonTr[i].anchoredPosition += Vector2.right * 12;
                }
            }
            menuOnProcess = false;
        }
        else
        {
            menuOpen = false;
            menuOnProcess = true;
            for (int i = 4; i >= 2; --i)
            {
                for (int k = 1; k <= 10; ++k)
                {
                    yield return waitForSec[0];
                    menuButtonTr[i].anchoredPosition += Vector2.left * 12;
                }
                menuButtonTr[i].anchoredPosition = Vector2.zero;
                menuButtonObj[i].SetActive(false);
            }
            menuOnProcess = false;
        }
    }

    public void onMusicEnd()
    {
        scoreBoardCSObj.SetActive(true);
        StartCoroutine(scoreBoardCS.showScoreboard());
    }
}

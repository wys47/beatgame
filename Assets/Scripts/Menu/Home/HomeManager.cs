using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    private WaitForSeconds[] waitForSeconds = new WaitForSeconds[1];

    public RecordBoardCS recordBoardCS;
    public GameObject background;
    public UIFadeInOut left;
    public UIFadeInOut right;
    public UIFadeInOut setting;
    public UIFadeInOut musicInfo;
    public UIFadeInOut play;
    public GameObject setting_screen;

    void Start()
    {
        waitForSeconds[0] = new WaitForSeconds(1);
    }

    public IEnumerator activate(bool isActive)
    {
        if (isActive)
        {
            StartCoroutine(recordBoardCS.move(true, 1));
            StartCoroutine(left.activate(true, Vector2.zero));
            StartCoroutine(right.activate(true, Vector2.zero));
            StartCoroutine(setting.activate(true, Vector2.zero));
            StartCoroutine(musicInfo.activate(true, Vector2.zero));
            StartCoroutine(play.activate(true, Vector2.zero));
            setting_screen.SetActive(false);
        }
        else
        {
            background.SetActive(false);
            StartCoroutine(recordBoardCS.move(true, -1, false));
            StartCoroutine(right.activate(false, Vector2.right));
            StartCoroutine(left.activate(false, Vector2.left));
            StartCoroutine(setting.activate(false, Vector2.up));
            StartCoroutine(musicInfo.activate(false, Vector2.down));
            StartCoroutine(play.activate(false, Vector2.down));
            setting_screen.SetActive(false);
            yield return waitForSeconds[0];

            gameObject.SetActive(false);
        }
    }

    public void settingPointerUp()
    {
        setting_screen.SetActive(true);
    }

    public void settingExitPointerUp()
    {
        setting_screen.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    public RecordBoardCS recordBoardCS;
    public GameObject setting_screen;

    public void activate(bool isActive)
    {
        if (isActive)
        {
            StartCoroutine(recordBoardCS.move(true, 1));
            setting_screen.SetActive(false);
        }
        else
        {
            StartCoroutine(recordBoardCS.move(true, -1, false));
            setting_screen.SetActive(false);

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

    public void leftAndRightPointerUp()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private WaitForSeconds waitForMenuExit = new WaitForSeconds(0.5f);

    void Start()
    {
        Application.targetFrameRate = 60;
    }
}

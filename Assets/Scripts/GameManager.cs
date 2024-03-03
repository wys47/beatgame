using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject menuUI;

    public GameObject gameObject_;
    public GameObject gameUI;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        menuUI.SetActive(true);

        gameObject_.SetActive(false);
        gameUI.SetActive(false);
    }

    public void onPlayButtonUp()
    {
        menuUI.SetActive(false);

        gameObject_.SetActive(true);
        gameUI.SetActive(true);
    }
}

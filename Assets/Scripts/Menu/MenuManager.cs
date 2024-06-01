using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject titleManagerObj;
    public TitleManager titleManager;

    public GameObject homeManagerObj;
    public HomeManager homeManager;

    void Awake()
    {
        titleManagerObj.SetActive(true);
        homeManagerObj.SetActive(false);
    }

    //타이틀화면 버튼
    public void OnStartButtonUp()
    {
        StartCoroutine(titleManager.onDeActivate());
        homeManagerObj.SetActive(true);
        StartCoroutine(homeManager.activate(true));
    }

    public void onPlayButtonUp()
    {
        StartCoroutine(homeManager.activate(false));
    }
}

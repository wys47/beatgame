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

    //Ÿ��Ʋȭ�� ��ư
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

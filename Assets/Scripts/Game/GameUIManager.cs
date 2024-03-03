using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public GameObject scoreBoardCSObj;
    public ScoreBoardCS scoreBoardCS;

    void OnEnable()
    {
        scoreBoardCSObj.SetActive(false);
    }

    public void onMusicEnd()
    {
        scoreBoardCSObj.SetActive(true);
        scoreBoardCS.activate();
    }
}

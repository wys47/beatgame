using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicInfoCS : MonoBehaviour
{
    public AudioSource audio;

    public TextMeshProUGUI[] musicInfoText = new TextMeshProUGUI[4];

    private WaitForSeconds waitForSeconds = new WaitForSeconds(0.2f);

    private int currentMusicNumber;

    private void OnEnable()
    {
        currentMusicNumber = 1;

        musicInfoText[1].text = MusicManager.musicName[currentMusicNumber];
        musicInfoText[2].text = MusicManager.musicLength[currentMusicNumber];
        musicInfoText[3].text = MusicManager.musicBPM[currentMusicNumber].ToString() + "BPM";

        audio.clip = MusicManager.music[1];
        audio.Play();
    }

    public void onLeftOrRightButtonUp(int leftOrRight)
    {
        currentMusicNumber += leftOrRight;
        if (currentMusicNumber > MusicManager.musicCnt) currentMusicNumber = 1;
        if (currentMusicNumber < 1) currentMusicNumber = MusicManager.musicCnt;

        StartCoroutine(update());
    }

    private IEnumerator update()
    {
        yield return waitForSeconds;

        musicInfoText[1].text = MusicManager.musicName[currentMusicNumber];
        musicInfoText[2].text = MusicManager.musicLength[currentMusicNumber];
        musicInfoText[3].text = MusicManager.musicBPM[currentMusicNumber].ToString() + "BPM";

        audio.clip = MusicManager.music[currentMusicNumber];
        audio.Play();
    }
}

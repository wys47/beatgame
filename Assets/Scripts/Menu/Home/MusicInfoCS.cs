using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicInfoCS : MonoBehaviour
{
    public MusicManager musicManager;

    public AudioSource audio;

    public Image image;
    public Image button;
    public TextMeshProUGUI[] musicInfoText = new TextMeshProUGUI[4];

    private WaitForSeconds waitForSeconds = new WaitForSeconds(0.01f);

    private int currentMusicNumber;

    private void OnEnable()
    {
        currentMusicNumber = 1;

        musicInfoText[1].text = musicManager.musicName[currentMusicNumber];
        musicInfoText[2].text = musicManager.musicLength[currentMusicNumber];
        musicInfoText[3].text = musicManager.musicBPM[currentMusicNumber].ToString() + "BPM";

        audio.clip = musicManager.music[1];
        audio.Play();
    }

    public void onLeftOrRightButtonUp(int leftOrRight)
    {
        currentMusicNumber += leftOrRight;
        if (currentMusicNumber > musicManager.musicCnt) currentMusicNumber = 1;
        if (currentMusicNumber < 1) currentMusicNumber = musicManager.musicCnt;

        StartCoroutine(update());
    }

    private IEnumerator update()
    {
        for (int i = 1; i <= 10; ++i)
        {
            image.color -= Color.black * 0.1f;
            button.color -= Color.black * 0.1f;
            yield return waitForSeconds;
        }

        musicInfoText[1].text = musicManager.musicName[currentMusicNumber];
        musicInfoText[2].text = musicManager.musicLength[currentMusicNumber];
        musicInfoText[3].text = musicManager.musicBPM[currentMusicNumber].ToString() + "BPM";

        for (int i = 1; i <= 10; ++i)
        {
            image.color += Color.black * 0.1f;
            button.color += Color.black * 0.1f;
            yield return waitForSeconds;
        }

        audio.clip = musicManager.music[currentMusicNumber];
        audio.Play();
    }
}

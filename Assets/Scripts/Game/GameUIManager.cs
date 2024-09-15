using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public MapCtrl mapCtrl;

    public GameObject scoreBoardCSObj;
    public ScoreBoardCS scoreBoardCS;

    public Image musicImage;
    public TMP_Text musicTitle;
    public TMP_Text musicArtist;

    public GameObject volumeSlider;
    private float[] volumeSliderMax = { -35.6f, 33.6f };
    public RectTransform volumeKnobTr;
    public AudioSource audioSource;

    public TMP_Text time;
    private double sec;
    [HideInInspector] public static bool musicStop;

    public Image stopAndRecImage;
    public Sprite[] stopAndRecSprite;
    public Image pauseAndStopImage;
    public Sprite[] pauseAndStopSprite;

    private WaitForSeconds[] waitForSec = { new WaitForSeconds(0.01f)};

    void OnEnable()
    {
        scoreBoardCSObj.SetActive(true);
        scoreBoardCS.activate();
        scoreBoardCSObj.SetActive(false);

        volumeSlider.SetActive(false);
        volumeKnobTr.anchoredPosition = Vector2.zero;
        audioSource.volume = 0.5f;

        musicImage.sprite = MusicManager.albumSprites[MusicPlayerImageCS.currentAlbumNum];
        musicTitle.text = MusicManager.musicName[MusicPlayerImageCS.currentMusicNum];
        musicArtist.text = MusicManager.musicArtist[MusicPlayerImageCS.currentMusicNum];

        sec = 0;
        musicStop = false;

        stopAndRecImage.sprite = stopAndRecSprite[0];
        pauseAndStopImage.sprite = pauseAndStopSprite[0];
    }

    private void Update()
    {
        if (!musicStop)
        {
            int min = (int)sec / 60;
            sec += Time.deltaTime;
            time.text = "00:" + (min >= 10 ? "" : "0") + min + ":" + (sec >= 10 ? "" : "0") + (int)sec;
        }
    }

    public void volumeButtonUp()
    {
        volumeSlider.SetActive(!volumeSlider.activeSelf);
    }
    public void volumeKnobDrag()
    {
        volumeKnobTr.position = Vector2.right * volumeSlider.transform.position + Vector2.up * Input.mousePosition.y;
        if (volumeKnobTr.anchoredPosition.y > volumeSliderMax[1]) volumeKnobTr.anchoredPosition = Vector2.up * volumeSliderMax[1];
        else if (volumeKnobTr.anchoredPosition.y < volumeSliderMax[0]) volumeKnobTr.anchoredPosition = Vector2.up * volumeSliderMax[0];

        audioSource.volume = (volumeKnobTr.anchoredPosition.y - volumeSliderMax[0]) / (volumeSliderMax[1] - volumeSliderMax[0]);
    }

    public void stopAndRecUp()
    {
        if (!musicStop)
        {
            stopAndRecImage.sprite = stopAndRecSprite[1];
            mapCtrl.onMusicEnd();
        }
        else
        {
            playMusic();
            stopAndRecImage.sprite = stopAndRecSprite[0];
            pauseAndStopImage.sprite = pauseAndStopSprite[0];
        }
    }
    public void pauseAndStopUp()
    {
        if (!musicStop)
        {
            stopMusic();
            pauseAndStopImage.sprite = pauseAndStopSprite[1];
            stopAndRecImage.sprite = stopAndRecSprite[1];
        }
        else
        {
            pauseAndStopImage.sprite = pauseAndStopSprite[0];
            mapCtrl.onMusicEnd();
        }
    }
    public void settingUp()
    {

    }

    public void stopMusic()
    {
        musicStop = true;
        audioSource.Pause();
        Time.timeScale = 0;
    }
    public void playMusic()
    {
        print(1);
        musicStop = false;
        audioSource.Play();
        Time.timeScale = 1;
    }
    public void onMusicEnd()
    {
        scoreBoardCSObj.SetActive(true);
        StartCoroutine(scoreBoardCS.showScoreboard());
    }
}

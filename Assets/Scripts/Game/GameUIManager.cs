using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public GameObject scoreBoardCSObj;
    public ScoreBoardCS scoreBoardCS;

    public Image musicImage;
    public TMP_Text musicTitle;
    public TMP_Text musicArtist;
    public GameObject volumeSlider;
    private float[] volumeSliderMax = { -35.6f, 33.6f };
    public RectTransform volumeKnobTr;
    public AudioSource audioSource;

    private WaitForSeconds[] waitForSec = { new WaitForSeconds(0.01f)};

    void OnEnable()
    {
        scoreBoardCSObj.SetActive(true);
        scoreBoardCS.activate();
        scoreBoardCSObj.SetActive(false);

        musicImage.sprite = MusicManager.albumSprites[MusicPlayerImageCS.currentAlbumNum];
        musicTitle.text = MusicManager.musicName[MusicPlayerImageCS.currentMusicNum];
        musicArtist.text = MusicManager.musicArtist[MusicPlayerImageCS.currentMusicNum];
    }

    private void Update()
    {
        
    }

    public void volumeButtonUp()
    {
        volumeSlider.SetActive(!volumeSlider.activeSelf);
        print(1);
    }
    public void volumeKnobDrag()
    {
        volumeKnobTr.position = Vector2.right * volumeSlider.transform.position + Vector2.up * Input.mousePosition.y;
        if (volumeKnobTr.anchoredPosition.y > volumeSliderMax[1]) volumeKnobTr.anchoredPosition = Vector2.up * volumeSliderMax[1];
        else if (volumeKnobTr.anchoredPosition.y < volumeSliderMax[0]) volumeKnobTr.anchoredPosition = Vector2.up * volumeSliderMax[0];

        audioSource.volume = (volumeKnobTr.anchoredPosition.y - volumeSliderMax[0]) / (volumeSliderMax[1] - volumeSliderMax[0]);
    }

    public void onMusicEnd()
    {
        scoreBoardCSObj.SetActive(true);
        StartCoroutine(scoreBoardCS.showScoreboard());
    }
}

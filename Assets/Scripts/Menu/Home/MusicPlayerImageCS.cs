using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicPlayerImageCS : MonoBehaviour
{
    private const int maxMusicBar = 8;

    public GameObject interfaceObj;

    public Image currentAlbumImage;
    public TMP_Text currentTitle;
    public TMP_Text currentArtist;

    public GameObject[] musicBars;
    private Image[] albumImages = new Image[maxMusicBar + 1];
    private TMP_Text[] titles = new TMP_Text[maxMusicBar + 1];
    private TMP_Text[] artists = new TMP_Text[maxMusicBar + 1];

    public AudioSource audioSource;

    [HideInInspector] public static int currentMusicNum;
    [HideInInspector] public static int currentAlbumNum;
    public Sprite[] albumSprites;

    public GameObject volumeSlider;
    private float[] volumeSliderMax = {-35.6f, 33.6f};
    public RectTransform volumeKnobTr;

    public Image pausePlayButton;
    public Sprite[] pausePlaySprite;
    private int pausePlayState;

    public RectTransform playBarKnobTr;
    private float[] playBarMax = { -133.5f, 128f };

    public ProfileCS profileCS;

    private void Awake()
    {
        for (int i = 1; i <= maxMusicBar; ++i)
        {
            albumImages[i] = musicBars[i].transform.GetChild(2).GetComponent<Image>();
            titles[i] = musicBars[i].transform.GetChild(0).GetComponent<TMP_Text>();
            artists[i] = musicBars[i].transform.GetChild(1).GetComponent<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        interfaceObj.SetActive(false);

        int defaultMusicNum = MusicManager.defaultMusic;
        currentMusicNum = defaultMusicNum;
        currentAlbumNum = MusicManager.defaultAlbum;

        audioSource.clip = MusicManager.music[currentMusicNum];
        audioSource.Play();

        currentAlbumImage.sprite = albumSprites[MusicManager.defaultAlbum];

        currentTitle.text = MusicManager.musicName[currentMusicNum];
        currentArtist.text = MusicManager.musicArtist[currentMusicNum];

        for (int i = 1; i <= maxMusicBar; ++i)
        {
            if (i <= MusicManager.musicCntByAlbum[currentAlbumNum])
            {
                musicBars[i].SetActive(true);
                albumImages[i].sprite = albumSprites[currentAlbumNum];
                titles[i].text = MusicManager.musicName[MusicManager.musicIndexByAlbum[currentAlbumNum, i]];
                artists[i].text = MusicManager.musicArtist[MusicManager.musicIndexByAlbum[currentAlbumNum, i]];
            }
            else musicBars[i].SetActive(false);
        }

        volumeSlider.SetActive(false);
        volumeKnobTr.anchoredPosition = Vector2.up * (volumeSliderMax[1] + volumeSliderMax[0]) * 0.5f;
        audioSource.volume = 0.5f;

        pausePlayState = 1;
        pausePlayButton.sprite = pausePlaySprite[1];

        playBarKnobTr.anchoredPosition = Vector2.right * playBarMax[0] + Vector2.up * -88.7f;
    }

    private void Update()
    {
        if (pausePlayState == 1)
        {
            playBarKnobTr.anchoredPosition = Vector2.right * (playBarMax[0] + (playBarMax[1] - playBarMax[0]) * audioSource.time / MusicManager.music[MusicManager.musicIndexByAlbum[currentAlbumNum, currentMusicNum]].length) + Vector2.up * -88.7f;
        }
    }

    public void interfaceActivate()
    {
        interfaceObj.SetActive(!interfaceObj.activeSelf);
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

    public void pausePlayButtonUp()
    {
        if (pausePlayState == 0)
        {
            audioSource.Play();
            pausePlayState = 1;
        }
        else if (pausePlayState == 1)
        {
            audioSource.Pause();
            pausePlayState = 0;
        }
        pausePlayButton.sprite = pausePlaySprite[pausePlayState];
    }

    public void frontBackButtonUp(int dir)
    {
        currentMusicNum = currentMusicNum + dir;
        if (currentMusicNum == 0) currentMusicNum = MusicManager.musicCntByAlbum[currentAlbumNum];
        else if (currentMusicNum == MusicManager.musicCntByAlbum[currentAlbumNum] + 1) currentMusicNum = 1;
        audioSource.clip = MusicManager.music[MusicManager.musicIndexByAlbum[currentAlbumNum, currentMusicNum]];

        currentTitle.text = MusicManager.musicName[MusicManager.musicIndexByAlbum[currentAlbumNum, currentMusicNum]];
        currentArtist.text = MusicManager.musicArtist[MusicManager.musicIndexByAlbum[currentAlbumNum, currentMusicNum]];

        audioSource.Play();
        pausePlayState = 1;
        pausePlayButton.sprite = pausePlaySprite[1];

        profileCS.updateVideos(currentMusicNum);
    }

    public void playBarKnobDrag()
    {
        audioSource.Pause();
        pausePlayState = 0;
        pausePlayButton.sprite = pausePlaySprite[0];

        playBarKnobTr.position = Vector2.right * Input.mousePosition.x + Vector2.up * 145.3f;
        if (playBarKnobTr.anchoredPosition.x > playBarMax[1]) playBarKnobTr.anchoredPosition = Vector2.right * playBarMax[1] + Vector2.up * -88.7f;
        else if (playBarKnobTr.anchoredPosition.x < playBarMax[0]) playBarKnobTr.anchoredPosition = Vector2.right * playBarMax[0] + Vector2.up * -88.7f;

        audioSource.time = MusicManager.music[MusicManager.musicIndexByAlbum[currentAlbumNum, currentMusicNum]].length * (playBarKnobTr.anchoredPosition.x - playBarMax[0]) / (playBarMax[1] - playBarMax[0]) ;
    }
    public void playBarKnobUp()
    {
        audioSource.Play();
        pausePlayState = 1;
        pausePlayButton.sprite = pausePlaySprite[1];
    }
}

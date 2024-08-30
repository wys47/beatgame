using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ProfileCS : MonoBehaviour
{
    public Image profileImage;
    public Sprite[] profileSprites;
    [HideInInspector] public int profileSpriteNumber = 1;

    private const int infoBarCnt = 2;

    private const int video1DefaultPosY = 197;
    private const int video1MovedPosY = 121;
    private const int heightBetweenVideosHolderOpen = 58;
    private const int heightBetweenVideosHolderClose = 50;

    public GameObject recentVideoHolder;
    public Image recentVideoImage;
    public TMP_Text recentVideoTitle;
    public TMP_Text recentVideoInfo;

    public RectTransform[] videosArrowTr;
    public RectTransform[] infoBarTr;
    public GameObject[] videosHolder;
    private GameObject[,] videosObj = new GameObject[infoBarCnt + 1, PlayedMusicDataCS.difficaltyCnt + 1];
    private RectTransform[,] videosTr = new RectTransform[infoBarCnt + 1, PlayedMusicDataCS.difficaltyCnt + 1];
    private Image[,] videosImages = new Image[infoBarCnt + 1, PlayedMusicDataCS.difficaltyCnt + 1];
    private TMP_Text[,] videosTitles = new TMP_Text[infoBarCnt + 1, PlayedMusicDataCS.difficaltyCnt + 1];
    private TMP_Text[,] videosInfos = new TMP_Text[infoBarCnt + 1, PlayedMusicDataCS.difficaltyCnt + 1];
    public Animator[] screenAnim;
    private int[,] videoDifficulty = new int[infoBarCnt + 1, PlayedMusicDataCS.difficaltyCnt + 1];

    private int lastPlayedMusicNum = MusicManager.defaultMusic;
    [HideInInspector] public static int currentDifficulty;

    private void Awake()
    {
        profileImage.sprite = profileSprites[profileSpriteNumber];
        recentVideoImage.sprite = profileSprites[profileSpriteNumber];

        for (int i = 1; i <= infoBarCnt; ++i)
        {
            string name = "";
            switch (i)
            {
                case 1:
                    name = "UploadedVideos";
                    break;
                case 2:
                    name = "ToBeUploaded";
                    break;
            }

            for (int j = 1; j <= PlayedMusicDataCS.difficaltyCnt; ++j)
            {
                videosObj[i, j] = GameObject.Find(name).transform.Find("Videos").GetChild(j - 1).gameObject;
                videosTr[i, j] = videosObj[i, j].GetComponent<RectTransform>();

                videosImages[i, j] = videosObj[i, j].transform.Find("Image").GetComponent<Image>();
                videosImages[i, j].sprite = profileSprites[profileSpriteNumber];

                videosTitles[i, j] = videosObj[i, j].transform.Find("Title").GetComponent<TMP_Text>();
                videosInfos[i, j] = videosObj[i, j].transform.Find("Info").GetComponent<TMP_Text>();
            }
        }
    }

    private void OnEnable()
    {
        for (int i = 1; i <= infoBarCnt; ++i)
        {
            videosArrowTr[i].rotation = Quaternion.Euler(0, 0, 0);
            videosHolder[i].SetActive(false);
        }

        updateVideos(lastPlayedMusicNum);
    }

    private string generateTitleName(int difficalty, string musicArtist, string musicName, bool isPerfect, int cnt)
    {
        string difficaltyStr = "";
        switch (difficalty)
        {
            case 1:
                difficaltyStr = "(Easy Mode)";
                break;
            case 2:
                difficaltyStr = "(Normal Mode)";
                break;
            case 3:
                difficaltyStr = "(Hard Mode)";
                break;
            case 4:
                difficaltyStr = "(Extreme Mode)";
                break;
        }

        return difficaltyStr + musicArtist + " - " + musicName + " | " + (isPerfect ? "Cover" : "Practice") + " #" + cnt;
    }

    public void VideosArrowUp(int pushedButtonNum)
    {
        if (!videosHolder[pushedButtonNum].activeSelf)
        {
            for (int i = 1; i <= infoBarCnt; ++i)
            {
                if (i == pushedButtonNum)
                {
                    videosArrowTr[pushedButtonNum].rotation = Quaternion.Euler(0, 0, 90);
                    videosHolder[pushedButtonNum].SetActive(true);
                }
                else
                {
                    videosArrowTr[i].rotation = Quaternion.Euler(0, 0, 0);
                    videosHolder[i].SetActive(false);
                }
            }

            updateVideos(MusicPlayerImageCS.currentMusicNum);
        }
        else
        {
            for (int i = 1; i <= infoBarCnt; ++i)
            {
                videosArrowTr[i].rotation = Quaternion.Euler(0, 0, 0);
                videosHolder[i].SetActive(false);
            }

            updateVideos(MusicPlayerImageCS.currentMusicNum);
        }
    }

    public void updateVideos(int musicNum)
    {
        if (PlayedMusicDataCS.isRecentPlay[musicNum])
        {
            recentVideoHolder.SetActive(true);
            infoBarTr[1].anchoredPosition = new Vector2(0, video1MovedPosY);
        }
        else
        {
            recentVideoHolder.SetActive(false);
            infoBarTr[1].anchoredPosition = new Vector2(0, video1DefaultPosY);
        }

        for (int i = 1, k = 1; i <= PlayedMusicDataCS.difficaltyCnt; ++i)
        {
            videosObj[1, i].SetActive(false);
            if (PlayedMusicDataCS.playedCntByDifficalty[musicNum, i] != 0)
            {
                videosObj[1, k].SetActive(true);

                videoDifficulty[1, k] = i;
                videosTitles[1, k].text = generateTitleName(i, MusicManager.musicArtist[musicNum], MusicManager.musicName[musicNum], PlayedMusicDataCS.isRecentPerfect[musicNum, i], PlayedMusicDataCS.playedCntByDifficalty[musicNum, i]);
                videosInfos[1, k].text = PlayedMusicDataCS.recentViewer[musicNum, i] + " views, " + PlayedMusicDataCS.recentPlusSubscriber[musicNum, i] + " new subs";

                ++k;
            }
        }
        for (int i = 1, k = 1; i <= PlayedMusicDataCS.difficaltyCnt; ++i)
        {
            videosObj[2, i].SetActive(false);
            if (PlayedMusicDataCS.playedCntByDifficalty[musicNum, i] == 0)
            {
                videosObj[2, k].SetActive(true);

                videoDifficulty[2, k] = i;
                videosTitles[2, k].text = generateTitleName(i, MusicManager.musicArtist[musicNum], MusicManager.musicName[musicNum], PlayedMusicDataCS.isRecentPerfect[musicNum, i], PlayedMusicDataCS.playedCntByDifficalty[musicNum, i] + 1);
                videosInfos[2, k].text = "no info";

                ++k;
            }
        }

        for (int i = 2; i <= infoBarCnt; ++i)
        {
            if (videosHolder[i - 1].activeSelf)
            {
                for (int j = 1; j <= PlayedMusicDataCS.difficaltyCnt; ++j)
                {
                    if (!videosObj[i - 1, j].activeSelf)
                    {
                        if (j == 1) infoBarTr[i].position = (Vector2)infoBarTr[i - 1].position - Vector2.up * heightBetweenVideosHolderClose;
                        else infoBarTr[i].position = (Vector2)videosTr[i - 1, j].position - Vector2.up * heightBetweenVideosHolderOpen;

                        break;
                    }
                }
            }
            else infoBarTr[i].position = (Vector2)infoBarTr[i - 1].position - Vector2.up * heightBetweenVideosHolderClose;
        }
    }

    public void playButtonUp(int n)
    {
        currentDifficulty = videoDifficulty[n / 10, n % 10];
        SceneManager.LoadScene("GameScene");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [HideInInspector] public static int musicCnt = 4;
    [HideInInspector] public static int gameSoundCnt = 6;
    private const int maxAlbum = 1;

    public AudioClip[] musicInput;
    public string[] musicNameInput;
    public string[] musicArtistInput;
    public string[] musicLengthInput;
    public int[] musicBPMInput;
    public float[] musicStartTimeInput;
    public AudioClip[] gameSoundInput;
    public Sprite[] albumSpriteInput;

    [HideInInspector] public static AudioClip[] music = new AudioClip[musicCnt + 1];
    [HideInInspector] public static string[] musicName = new string[musicCnt + 1];
    [HideInInspector] public static string[] musicArtist = new string[musicCnt + 1];
    [HideInInspector] public static string[] musicLength = new string[musicCnt + 1];
    [HideInInspector] public static int[] musicBPM = new int[musicCnt + 1];
    [HideInInspector] public static float[] musicStartTime = new float[musicCnt + 1];
    [HideInInspector] public static AudioClip[] gameSound = new AudioClip[gameSoundCnt + 1];
    [HideInInspector] public static Sprite[] albumSprites = new Sprite[maxAlbum + 1];
    [HideInInspector] public static int[] musicCntByAlbum = { 0, 2, 2 };
    [HideInInspector] public static int[,] musicIndexByAlbum = 
    {
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 1, 2, 0, 0, 0, 0, 0, 0},
        {0, 3, 4, 0, 0, 0, 0, 0, 0},
    };//1.DJ, 2.Tropical

    //플레이어 커스터마이징
    [HideInInspector] public static int defaultAlbum = 1;
    [HideInInspector] public static int defaultMusic = 1;//음악 숫자는 전체 음악 기준이 아닌 앨범 기준

    private void Awake()
    {
        for (int i = 0; i < musicInput.Length; ++i) music[i] = musicInput[i];
        for (int i = 0; i < musicNameInput.Length; ++i) musicName[i] = musicNameInput[i];
        for (int i = 0; i < musicArtistInput.Length; ++i) musicArtist[i] = musicArtistInput[i];
        for (int i = 0; i < musicLengthInput.Length; ++i) musicLength[i] = musicLengthInput[i];
        for (int i = 0; i < musicBPMInput.Length; ++i) musicBPM[i] = musicBPMInput[i];
        for (int i = 0; i < musicStartTimeInput.Length; ++i) musicStartTime[i] = musicStartTimeInput[i];
        for (int i = 0; i < gameSoundInput.Length; ++i) gameSound[i] = gameSoundInput[i];
        for (int i = 0; i < albumSpriteInput.Length; ++i) albumSprites[i] = albumSpriteInput[i];
    }
}

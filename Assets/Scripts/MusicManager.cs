using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [HideInInspector] public static int musicCnt = 1;
    [HideInInspector] public static int gameSoundCnt = 6;

    public AudioClip[] musicInput;
    public string[] musicNameInput;
    public string[] musicLengthInput;
    public int[] musicBPMInput;
    public float[] musicStartTimeInput;
    public AudioClip[] gameSoundInput;

    [HideInInspector] public static AudioClip[] music = new AudioClip[musicCnt + 1];
    [HideInInspector] public static string[] musicName = new string[musicCnt + 1];
    [HideInInspector] public static string[] musicLength = new string[musicCnt + 1];
    [HideInInspector] public static int[] musicBPM = new int[musicCnt + 1];
    [HideInInspector] public static float[] musicStartTime = new float[musicCnt + 1];
    [HideInInspector] public static AudioClip[] gameSound = new AudioClip[gameSoundCnt + 1];

    private void Awake()
    {
        for (int i = 0; i < musicInput.Length; ++i) music[i] = musicInput[i];
        for (int i = 0; i < musicNameInput.Length; ++i) musicName[i] = musicNameInput[i];
        for (int i = 0; i < musicLengthInput.Length; ++i) musicLength[i] = musicLengthInput[i];
        for (int i = 0; i < musicBPMInput.Length; ++i) musicBPM[i] = musicBPMInput[i];
        for (int i = 0; i < musicStartTimeInput.Length; ++i) musicStartTime[i] = musicStartTimeInput[i];
        for (int i = 0; i < gameSoundInput.Length; ++i) gameSound[i] = gameSoundInput[i];
    }
}

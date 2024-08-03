using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayedMusicDataCS : MonoBehaviour
{
    [HideInInspector] public const int difficaltyCnt = 4;

    [HideInInspector] public static int[,] playedCntByDifficalty = new int[MusicManager.musicCnt + 1, difficaltyCnt + 1];
    [HideInInspector] public static bool[,] isPerfectRewardGiven = new bool[MusicManager.musicCnt + 1, difficaltyCnt + 1];

    [HideInInspector] public static bool[] isRecentPlay = new bool[MusicManager.musicCnt + 1];
    [HideInInspector] public static int[,] recentViewer = new int[MusicManager.musicCnt + 1, difficaltyCnt + 1];
    [HideInInspector] public static int[,] recentPlusSubscriber = new int[MusicManager.musicCnt + 1, difficaltyCnt + 1];
    [HideInInspector] public static bool[,] isRecentPerfect = new bool[MusicManager.musicCnt + 1, difficaltyCnt + 1];

    private void Awake()
    {
        for (int i = 1; i <= MusicManager.musicCnt; ++i)
        {
            for (int j = 1; j <= difficaltyCnt; ++j)
            {
                playedCntByDifficalty[i, j] = 0;
                isPerfectRewardGiven[i, j] = false;

                isRecentPlay[i] = false;
                recentViewer[i, j] = 0;
                recentPlusSubscriber[i, j] = 0;
                isRecentPerfect[i, j] = false;
            }
        }
    }
}

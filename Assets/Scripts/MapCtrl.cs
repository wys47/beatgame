using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCtrl : MonoBehaviour //���� ������ �Ǹ� ���� �����ϰ� ������ �����ϴ� ������ �Ѵ�.
{
    //����
    public AudioClip[] musics;

    public int[] musicsBPM;
    private WaitForSeconds[] waitSec = new WaitForSeconds[100];

    public int[] musicStartBeat;

    //�����Ŭ��
    public AudioClip[] GameSound;

    //������ҽ�
    public AudioSource musicPlayer;
    public AudioSource[] gameSoundPlayers;
    public int audioPlayerCnt;

    //���� ������Ʈ
    public Transform tilesFile;
    public GameObject tileObj;

    //���Ӱ����� ����
    private int CurrentplayingTrack;
    private int GameState;
    private float BeatCnt;

    void Start()//�� ���� ������(���̾Ƹ�� ���)
    {
        for (int y = 4; y >= -4; --y)
        {
            for (int x = -4; x <= 4; ++x)
            {
                GameObject tile = Instantiate(tileObj, tilesFile);
                tile.transform.position = new Vector2(x, y);
            }
        }

        for (int i = 1; ; ++i)
        {
            if (musics[i] != null)
            {
                waitSec[i] = new WaitForSeconds(240f / musicsBPM[i]);
            }
            else break;
        }
    }

    void Awake()
    {
        musicPlayer.clip = musics[1];
        CurrentplayingTrack = 1;
        musicPlayer.Play();

        GameState = 0;
        BeatCnt = 0;

        StartCoroutine(UpdateOnNewBeat());
    }

    IEnumerator UpdateOnNewBeat()
    {
        while (GameState < 2)
        {
            if (GameState == 0)
            {
                if (BeatCnt >= 3.25f) GameState++;
            }
            if (GameState == 1)
            {
                PlayGameSound(GameSound[1]);
            }
            
            BeatCnt++;
            /*if (BeatCnt % 1 == 0 && BeatCnt % 2 == 0) PlayGameSound(GameSound[1]);
            BeatCnt += 0.25f;*/
            yield return waitSec[CurrentplayingTrack];
        }
    }

    void PlayGameSound(AudioClip clip)
    {
        for (int i = 1; i <= audioPlayerCnt; ++i)
        {
            AudioSource audio = gameSoundPlayers[i];
            if (!audio.isPlaying)
            {
                audio.clip = clip;
                audio.Play();
            }
            else continue;
            return;
        }

        print("����� ������� �� �ʰ�");
    }
}

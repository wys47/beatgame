using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCtrl : Variables //게임 시작이 되면 맵을 생성하고 게임을 진행하는 역할을 한다.
{
    //게임 규모
    private const int maxAudioPlayer = 3;
    private const int maxNode = 40;
    private const float maxMapAnimationPlayBeat = 1;

    //게임 수치 변수
    [HideInInspector]
    public Color[] ColorByCode = new Color[maxColor + 2];
    private const float zoneAlpha = 0.7f;

    //UI
    public GameUIManager gameUIManager;
    public DifficultyViewerCS difficultyViewerCS;

    //음악
    public MusicManager musicManager;
    private WaitForSeconds waitForStartFuzz = new WaitForSeconds(2f);
    private WaitForSeconds[] wiatForMusicStart = new WaitForSeconds[100];
    private float beatSensorRotateArc;

    public NodesTimingCS nodesTimingCS;

    //맵메이킹
    public int makeMode;

    //오디오클립
    public AudioClip[] gameSound;

    //오디오소스
    public AudioSource musicPlayer;
    public AudioSource[] gameSoundPlayers;

    //게임 컴포넌트
    public Transform tilesHolder;
    public GameObject tileObj;
    [HideInInspector] public Tile[] tileCS = new Tile[maxMapSize * maxMapSize + 1];

    public GameObject[] zoneObj;
    private SpriteRenderer[] zoneSprites = new SpriteRenderer[6];
    public struct zoneActiveInfoDef
    {
        public int colorCode;
        public float timing;
    } 
    [HideInInspector] public zoneActiveInfoDef[] zoneActiveInfo = new zoneActiveInfoDef[6];
    private WaitForSeconds zoneFadeTime = new WaitForSeconds(0.05f);

    public GameObject mapGlowObj;
    private SpriteRenderer mapGlowSprite;
    private WaitForSeconds[] animationDelay = new WaitForSeconds[4];

    public Transform nodesHolder;
    public GameObject nodeObj;
    private GameObject[] nodeCopys = new GameObject[maxNode + 1];
    [HideInInspector] public NodeCS[] nodeCS = new NodeCS[maxNode + 1];

    public Transform beatSensor;

    private bool AIPlay = false;

    //게임관리용 변수
    private int currentPlayingTrack;

    private int gameState;
    [HideInInspector] public float beatCnt;
    private bool afterBeatCntPlus;

    private struct nodeActiveInfoDef
    {
        public float activeTiming;
        public float timing;
        public int eventNum;
    }
    private nodeActiveInfoDef[] nodeActiveInfo = new nodeActiveInfoDef[maxTiming];
    private int nodeCntActive;
    private int nodeCycle;

    private int[] palette = new int[maxColor + 1];
    private int useColorCnt;

    private struct collideDef
    {
        public int tileNum;
        public int tileColor;
        public int collideTileNum;
        public int primeNodeRank;
        public int dir;
    }
    private collideDef[] collide = new collideDef[4];

    private int itsTime;

    //디버깅용 변수
    private int debug = 0;

    void Awake()
    {
        ColorByCode[0] = new Color(0.2f, 0.2f, 0.2f, 1);
        ColorByCode[1] = new Color(1, 0, 0, 1);
        ColorByCode[2] = new Color(1, 0.5f, 0, 1);
        ColorByCode[3] = new Color(1, 1, 0, 1);
        ColorByCode[4] = new Color(0.65f, 1, 0, 1);
        ColorByCode[5] = new Color(0, 0.8f, 0, 1);
        ColorByCode[6] = new Color(0, 1, 0.8f, 1);
        ColorByCode[7] = new Color(0, 0.96f, 1, 1);
        ColorByCode[8] = new Color(0, 0.3f, 1, 1);
        ColorByCode[9] = new Color(0.67f, 0, 1, 1);
        ColorByCode[10] = new Color(1, 0, 0.7f, 1);
        ColorByCode[maxColor + 1] = new Color(1, 1, 1, 1);

        for (int y = 1; y <= maxMapSize; ++y)
        {
            for (int x = 1; x <= maxMapSize; ++x)
            {
                GameObject tile = Instantiate(tileObj, tilesHolder);
                tile.transform.position = new Vector2(x * 0.7f - 0.25f - maxMapSize * 0.35f, y * 0.7f - 0.25f - maxMapSize * 0.35f);
                tileCS[(y - 1) * maxMapSize + x] = tile.GetComponent<Tile>();
                tileCS[(y - 1) * maxMapSize + x].mapCtrl = gameObject.GetComponent<MapCtrl>();
                tileCS[(y - 1) * maxMapSize + x].OnGenerate((y - 1) * maxMapSize + x);
            }
        }
        for (int i = 0; i <= 5; ++i)
        {
            zoneSprites[i] = zoneObj[i].GetComponent<SpriteRenderer>();
            zoneObj[i].SetActive(false);
        }
        mapGlowSprite = mapGlowObj.GetComponent<SpriteRenderer>();
        tilesHolder.Rotate(0, 0, 45);

        for (int i = 1; i <= musicManager.musicCnt; ++i)
        {
            wiatForMusicStart[i] = new WaitForSeconds(musicManager.musicStartTime[i]);
            beatSensorRotateArc = musicManager.musicBPM[i] / 5f;
        }

        for (int i = 1; i <= maxNode; ++i)
        {
            nodeCopys[i] = Instantiate(nodeObj, nodesHolder);
            nodeCS[i] = nodeCopys[i].GetComponent<NodeCS>();
            nodeCopys[i].SetActive(false);
        }
    }

    void OnEnable()
    {
        StartCoroutine(onEnabledFunc());
    }

    void Update()
    {
        if (gameState == 0)
        {
            onFirstUpdate();
            gameState = 1;
        }
        if (gameState == 1)
        {
            if (itsTime == 0) StartCoroutine(timer(wiatForMusicStart[currentPlayingTrack]));
            if (itsTime == 2)
            {
                gameState = 2;
                itsTime = 0;
            }
        }
        if (gameState == 2)
        {
            if (afterBeatCntPlus) onAfterBeatCntPlus();

            float arc = beatSensor.eulerAngles.z;
            if (arc > 360 - beatSensorRotateArc * 0.5f || arc <= beatSensorRotateArc * 0.5f || (arc > 180 - beatSensorRotateArc * 0.5f && arc <= 180 + beatSensorRotateArc * 0.5f)) beatCntPlus();

            clickTileCheck();
            tapTileCheck();

            beatSensor.Rotate(0, 0, beatSensorRotateArc);
        }
    }

    private IEnumerator onEnabledFunc()
    {
        currentPlayingTrack = 1;
        for (int i = 1; i <= 3; ++i) zoneObj[i].SetActive(false);
        for (int i = 1; i <= 3; ++i) zoneActiveInfo[i].colorCode = 0;
        mapGlowObj.SetActive(false);

        beatSensor.rotation = Quaternion.Euler(0, 0, 0);
        gameState = 0;
        beatCnt = -PlusBeatInOneUpdate * maxMapSize - PlusBeatInOneUpdate;

        collide[0].tileNum = 0;

        nodeCycle = 0;
        nodeCntActive = 1;

        itsTime = 0;

        yield return waitForStartFuzz;

        musicPlayer.clip = musicManager.music[currentPlayingTrack];
        musicPlayer.Play();
    }

    private void onFirstUpdate()
    {
        for (int i = 1; i <= nodesTimingCS.nodesTiming[currentPlayingTrack, 0].maxTiming + 1; ++i) nodeActiveInfo[i].activeTiming = PlusBeatInOneUpdate * 0.5f;

        //위치 지정 노드 숫자 랜덤 부여
        int[] positionedNodeEventNum = { 0, 0, 0, 0, 0, 0 };
        int n = 3;
        positionedNodeEventNum[Random.Range(eventNumPositionedNodeRange[0], eventNumPositionedNodeRange[1] + 1)] = n;
        bool upOrDown = Random.Range(0, 2) == 0;
        for (int i = upOrDown ? 3 : 5; upOrDown ? i <= 5 : i >= 3; i += upOrDown ? 1 : -1)
        {
            if (positionedNodeEventNum[i] != 0) continue;
            else positionedNodeEventNum[i] = ++n;
        }

        gameUIManager.scoreBoardCS.nodeCnt = nodesTimingCS.nodesTiming[currentPlayingTrack, 0].maxTiming;

        for (int i = 1; i <= nodesTimingCS.nodesTiming[currentPlayingTrack, 0].maxTiming; ++i)
        {
            nodeActiveInfo[i].eventNum = nodesTimingCS.nodesTiming[currentPlayingTrack, i].eventNum;
            if (difficultyViewerCS.difficulty <= 2 && nodeActiveInfo[i].eventNum >= eventNumClickNodeRange[0] && nodeActiveInfo[i].eventNum <= eventNumClickNodeRange[1]) nodeActiveInfo[i].eventNum = 0;

            float activeTiming = nodesTimingCS.nodesTiming[currentPlayingTrack, i].timing + PlusBeatInOneUpdate - (nodeActiveInfo[i].eventNum < eventNumClickNodeRange[0] && nodeActiveInfo[i].eventNum > eventNumClickNodeRange[1] ? Random.Range((int)(maxMapSize * 0.5f) + 1, maxMapSize) : 8) * PlusBeatInOneUpdate;

            if (nodeActiveInfo[i].eventNum >= eventNumPositionedNodeRange[0] && nodeActiveInfo[i].eventNum <= eventNumPositionedNodeRange[1]) nodeActiveInfo[i].eventNum = positionedNodeEventNum[nodeActiveInfo[i].eventNum];

            for (int k = 1; k <= nodesTimingCS.nodesTiming[currentPlayingTrack, 0].maxTiming; ++k)
            {
                if (nodeActiveInfo[k].activeTiming == PlusBeatInOneUpdate * 0.5f)
                {
                    nodeActiveInfo[k].activeTiming = activeTiming;
                    nodeActiveInfo[k].timing = nodesTimingCS.nodesTiming[currentPlayingTrack, i].timing;
                    break;
                }
                else if (nodeActiveInfo[k].activeTiming > activeTiming)
                {
                    for (int j = i; j >= k + 1; --j)
                    {
                        nodeActiveInfo[j].activeTiming = nodeActiveInfo[j - 1].activeTiming;
                        nodeActiveInfo[j].timing = nodeActiveInfo[j - 1].timing;
                    }
                    nodeActiveInfo[k].activeTiming = activeTiming;
                    nodeActiveInfo[k].timing = nodesTimingCS.nodesTiming[currentPlayingTrack, i].timing;
                    break;
                }
            }
        }

        animationDelay[0] = new WaitForSeconds(0);
        animationDelay[1] = new WaitForSeconds(60 / musicManager.musicBPM[currentPlayingTrack] / 4 * 0.9f);
        animationDelay[2] = new WaitForSeconds(60 / musicManager.musicBPM[currentPlayingTrack] / 15 * 0.9f);
        animationDelay[3] = new WaitForSeconds(0.05f);
    }

    private void beatCntPlus()
    {
        beatCnt += PlusBeatInOneUpdate;
        afterBeatCntPlus = true;

        if (makeMode == 0)
        {
            for (int i = 1; i <= maxMapSize * maxMapSize; ++i) tileCS[i].changeTempTileColor(i);
            for (int i = 1; i <= maxMapSize * maxMapSize; ++i) tileCS[i].changeTileColorFromTemp();

            if (nodeActiveInfo[nodeCntActive].activeTiming == beatCnt) onNodeActiveTiming();

            for (int i = 1; i <= maxNode; ++i)
            {
                if (nodeCopys[i].activeSelf) nodeCS[i].beatCntUpdated(beatCnt);
            }
        }
        else
        {
            if (makeMode == 1)
            {
                if (beatCnt % 1 == 0) playGameSound(gameSound[1]);
            }
            else if (makeMode == 2)
            {
                if (nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive].timing == beatCnt)
                {
                    playGameSound(gameSound[1]);
                    if (nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive].eventNum != 0)
                    {
                        if (nodeCntActive >= nodesTimingCS.nodesTiming[currentPlayingTrack, 0].maxTiming || nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive + 1].timing - beatCnt > PlusBeatInOneUpdate * maxMapSize + maxMapAnimationPlayBeat) StartCoroutine(MapAnimation(nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive].eventNum));
                    }

                    ++nodeCntActive;
                }
            }
        }
    }

    private void clickTileCheck()
    {
        int clickTileNum = collide[0].tileNum;
        if (clickTileNum != 0)
        {
            if (Input.GetMouseButton(0) || AIPlay)
            {
                RaycastHit2D raycastHit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition) - Vector3.forward, Vector3.forward, 2);
                if ((raycastHit2D && int.Parse(raycastHit2D.collider.name) == clickTileNum) || AIPlay)
                {
                    StartCoroutine(tileCS[clickTileNum].glowImageSwitch(true, true));
                    gameUIManager.scoreBoardCS.onClickTiming(true);

                    int eventNum = nodeActiveInfo[nodeCntActive - 1].eventNum;
                    if (nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive - 1].timing == beatCnt && eventNum == eventNumClickNodeAnim)
                    {
                        if (gameUIManager.scoreBoardCS.clickAll) StartCoroutine(MapAnimation(eventNum, clickTileNum));
                        gameUIManager.scoreBoardCS.clickAll = true;
                    }

                    collide[0].tileNum = 0;
                }
            }
        }
    }

    private void tapTileCheck()
    {
        bool tapSuccess = false;

        for (int i = 1; i <= 3; ++i)
        {
            KeyCode key = KeyCode.None;
            if (i == 1) key = KeyCode.A;
            else if (i == 2) key = KeyCode.S;
            else if (i == 3) key = KeyCode.D;

            if (collide[i].primeNodeRank != 0)
            {
                if (Input.GetKeyDown(key) || (AIPlay && collide[i].primeNodeRank == 3))
                {
                    if (collide[i].primeNodeRank == 3)
                    {
                        StartCoroutine(tileCS[collide[i].collideTileNum].glowImageSwitch(true));
                        gameUIManager.scoreBoardCS.onTapTiming(true);
                        tapSuccess = true;
                    }
                    else
                    {
                        if (difficultyViewerCS.difficulty > 1) gameUIManager.scoreBoardCS.onCommonTap();
                        else
                        {
                            gameUIManager.scoreBoardCS.onTapTiming(true);
                            tapSuccess = true;
                        }
                    }

                    if (difficultyViewerCS.difficulty == 4) StartCoroutine(zoneFade(i, collide[i].tileColor, true, true));

                    StartCoroutine(tileCS[collide[i].collideTileNum].ActivateTapSuccessEffect());
                    tileCS[collide[i].tileNum].changeTileColorAndInfo(collide[i].dir, false, -1, 0);
                    collide[i].primeNodeRank = 0;
                }
            }
        }

        if (tapSuccess)
        {
            int eventNum = nodeActiveInfo[nodeCntActive - 1].eventNum;
            if (nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive - 1].timing == beatCnt && eventNum >= eventNumGlobalNodeRange[0] && eventNum <= eventNumGlobalNodeRange[1])
            {
                if (gameUIManager.scoreBoardCS.tapAll) StartCoroutine(MapAnimation(eventNum));
                gameUIManager.scoreBoardCS.tapAll = true;
            }
        }
    }

    private void onAfterBeatCntPlus()
    {
        for (int i = 1; i <= maxMapSize * maxMapSize; ++i) tileCS[i].updateTileColor();//타일 업데이트

        if (collide[0].tileNum != 0)
        {
            gameUIManager.scoreBoardCS.onClickTiming(false);
            collide[0].tileNum = 0;
        }//클릭타일 클릭 실패

        for (int i = 1; i <= 3; ++i)
        {
            collide[i].tileNum = 0;

            if (collide[i].primeNodeRank == 2) gameUIManager.scoreBoardCS.onTapTiming(false);
            collide[i].primeNodeRank = 0;

            collide[i].dir = 0;
        }//탭타일 탭 실패

        for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
        {
            for (int k = 1; k < maxNodeDir; ++k)
            {
                if (tileCS[i].tileColor[0] == tileCS[i].tileColor[k]) tileCS[i].changeTileColorAndInfo(0, false, 0, 0);

                int targetTileNum = tileCS[i].tileNodeTargetTileNum[k];
                if (targetTileNum == 0) continue;
                int zone = zoneByTileNum(targetTileNum);

                if (zone != 0)
                {
                    if (k == 1)
                    {
                        int diff = targetTileNum - i;
                        onAfterBeatCntPlus_1(diff, maxMapSize, zone, targetTileNum, i, k);
                    }
                    else if (k == 2)
                    {
                        int diff = i - targetTileNum;
                        onAfterBeatCntPlus_1(diff, maxMapSize, zone, targetTileNum, i, k);
                    }
                    else if (k == 3)
                    {
                        int diff = i - targetTileNum;
                        onAfterBeatCntPlus_1(diff, 1, zone, targetTileNum, i, k);
                    }
                    else if (k == 4)
                    {
                        int diff = targetTileNum - i;
                        onAfterBeatCntPlus_1(diff, 1, zone, targetTileNum, i, k);
                    }
                }
                else if (targetTileNum == i) collide[zone].tileNum = i;
            }
        }
        for (int i = difficultyViewerCS.difficulty <= 3 ? 1 : 4; i <= 5; ++i)
        {
            if (zoneActiveInfo[i].colorCode != 0)
            {
                if (zoneActiveInfo[i].timing != beatCnt)
                {
                    if (!zoneObj[i].activeSelf) StartCoroutine(zoneFade(i, zoneActiveInfo[i].colorCode, true));
                }
                else
                {
                    if (zoneObj[i].activeSelf) StartCoroutine(zoneFade(i, zoneActiveInfo[i].colorCode, false));
                    zoneActiveInfo[i].colorCode = 0;
                }
            }
        }

        afterBeatCntPlus = false;

        if (!musicPlayer.isPlaying) onMusicEnd();
    }
    private void onAfterBeatCntPlus_1(int diff, int comparison, int zone, int targetTileNum, int i, int k)
    {
        int isCollide = 0;

        if (diff == comparison)
        {
            if (collide[zone].primeNodeRank == 0) isCollide = 1;
        }
        else if (diff == -comparison)
        {
            if (collide[zone].primeNodeRank <= 1) isCollide = 2;
        }
        else if (diff == 0) isCollide = 3;

        if (isCollide != 0)
        {
            collide[zone].tileNum = i;
            collide[zone].collideTileNum = targetTileNum;
            collide[zone].primeNodeRank = isCollide;
            collide[zone].dir = k;
            collide[zone].tileColor = tileCS[i].tileColor[k];
        }
    }

    private void onMusicEnd()
    {
        gameUIManager.onMusicEnd();
        gameObject.SetActive(false);
    }

    private void onNodeActiveTiming()
    {
        useColorCnt = 0;
        for (int i = 1; i <= maxColor; ++i) palette[i] = 1;
        for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
        {
            for (int k = 0; k < maxNodeDir; ++k)
            {
                if (tileCS[i].tileColor[k] != 0 && tileCS[i].tileColor[k] != maxColor + 1 && palette[tileCS[i].tileColor[k]] == 1)
                {
                    palette[tileCS[i].tileColor[k]] = 0;
                    ++useColorCnt;
                }
            }
        }

        for (int i = 1, k = 1; k <= maxColor - useColorCnt; ++i)
        {
            if (palette[i] == 1)
            {
                palette[k++] = i;
            }
        }

        do
        {
            int color = palette[Random.Range(1, maxColor - useColorCnt + 1)];
            activateNode(nodeActiveInfo[nodeCntActive].activeTiming, nodeActiveInfo[nodeCntActive++].timing, color, 1);
        }
        while (nodeActiveInfo[nodeCntActive].activeTiming == beatCnt);
    }

    public void activateNode(float activeTiming, float timing, int color, int generateMode, int targetTileNum = 0, int dir = 0)
    {
        if (nodeCycle == maxNode) nodeCycle = 1;
        else ++nodeCycle;

        NodeCS node = nodeCS[nodeCycle];
        if (!nodeCopys[nodeCycle].activeSelf)
        {
            nodeCopys[nodeCycle].SetActive(true);

            node.ActivatedTiming = activeTiming;
            node.timing = timing;
            node.color = color;
            node.generateMode = generateMode;
            node.targetZone = 0;
            node.targetTileNum = targetTileNum;
            node.dir = dir;

            if (generateMode == 1)
            {
                int eventNum = nodeActiveInfo[nodeCntActive - 1].eventNum;

                if (eventNum >= eventNumClickNodeRange[0] && eventNum <= eventNumClickNodeRange[1])
                {
                    int dir_ = 1 + 3 * Random.Range(0, 2);
                    int genTileNum;
                    int targetTileNum_;
                    if (dir_ == 1)
                    {
                        do genTileNum = Random.Range(5, 9);
                        while (tileCS[genTileNum].tileColor[1] != 0);

                        targetTileNum_ = genTileNum + maxMapSize * (maxMapSize - 1);

                        zoneActiveInfo[4].colorCode = maxColor + 1;
                        zoneActiveInfo[4].timing = beatCnt + 8 * PlusBeatInOneUpdate;
                    }
                    else
                    {
                        do genTileNum = maxMapSize * Random.Range(4, 8) + 1;
                        while (tileCS[genTileNum].tileColor[4] != 0);

                        targetTileNum_ = genTileNum + maxMapSize - 1;

                        zoneActiveInfo[5].colorCode = maxColor + 1;
                        zoneActiveInfo[5].timing = beatCnt + 8 * PlusBeatInOneUpdate;
                    }

                    tileCS[genTileNum].changeTileColorAndInfo(dir_, false, maxColor + 1, targetTileNum_);
                }
                else if (eventNum >= eventNumChainNodeRange[0] && eventNum <= eventNumChainNodeRange[1])
                {
                    tileCS[5].changeTileColorAndInfo(1, false, maxColor + 1, 37);
                    tileCS[33].changeTileColorAndInfo(4, false, maxColor + 1, 37);
                }
                else if (eventNum >= eventNumPositionedNodeRange[0] && eventNum <= eventNumPositionedNodeRange[1])
                {
                    node.targetZone = eventNum - 2;
                    node.generate();
                }
                else node.generate();
            }
        }
        else activateNode(activeTiming, timing, color, generateMode, targetTileNum, dir);
        return;
    }

    public int zoneByTileNum(int tileNum)
    {
        int zoneNum = 0;
        if ((tileNum - 1) / maxMapSize >= maxMapSize * 0.5f)
        {
            if ((tileNum - 1) % maxMapSize + 1 <= maxMapSize * 0.5f) zoneNum = 1;
            else zoneNum = 0;
        }
        else if ((tileNum - 1) % maxMapSize + 1 <= maxMapSize * 0.5f) zoneNum = 2;
        else zoneNum = 3;

        return zoneNum;
    }

    private IEnumerator zoneFade(int zoneNum, int color, bool onOff, bool autoOff = false)
    {
        if (onOff) zoneObj[zoneNum].SetActive(true);

        for (int i = 1; i <= 5; ++i)
        {
            if (onOff) zoneSprites[zoneNum].color = ColorByCode[color] + Color.black * (zoneAlpha * 0.2f * i - 1);
            else zoneSprites[zoneNum].color = ColorByCode[color] + Color.black * (zoneAlpha - zoneAlpha * 0.2f * i - 1);

            yield return zoneFadeTime;

            if (autoOff && i == 5)
            {
                print(1);
                i = 1;
                onOff = false;
                autoOff = false;
            }
        }

        if (!onOff) zoneObj[zoneNum].SetActive(false);
    }

    IEnumerator timer(WaitForSeconds sec)
    {
        itsTime = 1;
        yield return sec;
        itsTime = 2;
    }

    IEnumerator MapAnimation(int animNum, int originTileNum = 0)
    {
        if (animNum == 2)
        {
            for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
            {
                if (zoneByTileNum(i) == 0)
                {
                    tileCS[i].changeTileColorAndInfo(0, false, 0);
                    tileCS[i].updateTileColor();
                }
            }

            zoneObj[0].SetActive(true);
            tileCS[originTileNum].changeTileColorAndInfo(0, false, maxColor + 1);
            tileCS[originTileNum].updateTileColor();

            yield return animationDelay[3];

            int a = (originTileNum - 1) % maxMapSize + 1;
            int b = (originTileNum - 1) / maxMapSize + 1;

            for (int n = 1; n <= (maxMapSize * 0.5f - 1) * 2; ++n)
            {
                for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
                {
                    int x = (i - 1) % maxMapSize + 1;
                    int y = (i - 1) / maxMapSize + 1;

                    if (zoneByTileNum(i) == 0 && x >= a - n && x <= a + n && y >= b - n && y <= b + n)
                    {
                        if (y - b == x - a + n || y - b == x - a - n || y - b == -(x - a) + n || y - b == -(x - a) - n)
                        {
                            tileCS[i].changeTileColorAndInfo(0, false, maxColor + 1);
                            tileCS[i].updateTileColor();
                        }
                    }
                }

                yield return animationDelay[3];
            }

            for (int i = 1; i <= maxMapSize * maxMapSize; ++i) if (zoneByTileNum(i) == 0) tileCS[i].changeTileColorAndInfo(0, false, 0);
            zoneObj[0].SetActive(false);
        }
        else if (animNum == 11)
        {
            for (int i = 1; i <= 3; ++i) zoneObj[i].SetActive(false);

            mapGlowObj.SetActive(true);
            for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
            {
                for (int k = 0; k < maxNodeDir; ++k) tileCS[i].changeTileColorAndInfo(k, false, 0, 0);
                tileCS[i].glowObj.SetActive(false);
                tileCS[i].updateTileColor();
            }

            yield return animationDelay[0];

            for (int n = 3; n >= 0; --n)
            {
                for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
                {
                    if ((i - 1) / maxMapSize + 1 == maxMapSize * 0.5f - n || (i - 1) / maxMapSize + 1 == maxMapSize * 0.5f + 1 + n)
                    {
                        if ((i - 1) % maxMapSize + 1 >= maxMapSize * 0.5f - n && (i - 1) % maxMapSize + 1 <= maxMapSize * 0.5f + n + 1) tileCS[i].changeTileColorAndInfo(0, false, maxColor + 1);
                    }
                    else if ((i - 1) % maxMapSize + 1 == maxMapSize * 0.5f - n || (i - 1) % maxMapSize + 1 == maxMapSize * 0.5f + n + 1) tileCS[i].changeTileColorAndInfo(0, false, maxColor + 1);

                    tileCS[i].updateTileColor();
                }

                yield return animationDelay[1];
            }

            for (int i = 1; i <= maxMapSize * maxMapSize; ++i) tileCS[i].changeTileColorAndInfo(0, false, 0);
            mapGlowObj.SetActive(false);
        }
        else if (animNum == 12 && makeMode == 0)
        {
            for (int i = 1; i <= 3; ++i) zoneObj[i].SetActive(false);

            mapGlowObj.SetActive(true);
            for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
            {
                for (int k = 0; k < maxNodeDir; ++k) tileCS[i].changeTileColorAndInfo(k, false, 0, 0);
                tileCS[i].glowObj.SetActive(false);
                tileCS[i].updateTileColor();
            }

            int pointTileNum = 0;
            for (int i = 1; i <= 3; ++i) if (collide[i].tileNum != 0) pointTileNum = collide[i].tileNum;

            print(pointTileNum);
            print(tileCS[pointTileNum]);
            tileCS[pointTileNum].changeTileColorAndInfo(0, false, maxColor + 1);
            tileCS[pointTileNum].updateTileColor();
            yield return animationDelay[2];

            int a = (pointTileNum - 1) % maxMapSize + 1;
            int b = (pointTileNum - 1) / maxMapSize + 1;

            for (int n = 1; n <= (maxMapSize - 1) * 2; ++n)
            {
                for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
                {
                    int x = (i - 1) % maxMapSize + 1;
                    int y = (i - 1) / maxMapSize + 1;

                    if (x >= a - n && x <= a + n && y >= b - n && y <= b + n)
                    {
                        if (y - b == x - a + n || y - b == x - a - n || y - b == -(x - a) + n || y - b == -(x - a) - n)
                        {
                            tileCS[i].changeTileColorAndInfo(0, false, maxColor + 1);
                            tileCS[i].updateTileColor();
                        }
                    }
                }

                yield return animationDelay[2];
            }

            for (int i = 1; i <= maxMapSize * maxMapSize; ++i) tileCS[i].changeTileColorAndInfo(0, false, 0);
            mapGlowObj.SetActive(false);
        }
    }

    void playGameSound(AudioClip clip)
    {
        for (int i = 1; i <= maxAudioPlayer; ++i)
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

        print("오디오 동시재생 수 초과");
    }
}

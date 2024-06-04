using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCtrl : Variables //게임 시작이 되면 맵을 생성하고 게임을 진행하는 역할을 한다.
{
    //게임 규모
    private const int maxAudioPlayer = 3;
    private const int maxNode = 40;
    private const float maxMapAnimationPlayBeat = 1;
    private const int plusWrongInputPenalty = 3;

    //게임 수치 변수
    [HideInInspector] public static Color[] ColorByCode = new Color[maxColor + 2];
    private const float zoneAlpha = 0.7f;

    //UI
    public GameUIManager gameUIManager;

    //음악
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
    [HideInInspector] public static Tile[] tileCS = new Tile[maxMapSize * maxMapSize + 1];

    public GameObject[] zoneObj;
    private SpriteRenderer[] zoneSprites = new SpriteRenderer[6];
    public struct zoneActiveInfoDef
    {
        public int colorCode;
        public float timing;
    } 
    [HideInInspector] public zoneActiveInfoDef[] zoneActiveInfo = new zoneActiveInfoDef[6];
    private WaitForSeconds zoneFadeTime = new WaitForSeconds(0.05f);
    private WaitForSeconds mapGlowFadeTime = new WaitForSeconds(0.01f);

    public GameObject mapGlowObj;
    public SpriteRenderer mapGlowSprite;

    private WaitForSeconds[] animationDelay = new WaitForSeconds[4];
    private WaitUntil animationDelayCondition;
    private float conditionBeatCnt;

    public Transform nodesHolder;
    public GameObject nodeObj;
    private GameObject[] nodeCopys = new GameObject[maxNode + 1];
    [HideInInspector] public NodeCS[] nodeCS = new NodeCS[maxNode + 1];

    public Transform beatSensor;

    //게임관리용 변수
    private int currentPlayingTrack;
    private int gameState;
    [HideInInspector] public float beatCnt;
    private bool afterBeatCntPlus;
    private int wrongInputPenalty;

    //AI
    private bool AIPlay = false;
    private int AIDebug = 0;

    private struct nodeActiveInfoDef
    {
        public float activeTiming;
        public float timing;
        public int eventNum;
    }
    private nodeActiveInfoDef[] nodeActiveInfo = new nodeActiveInfoDef[maxTiming];
    private int nodeCntActive;
    private int nodeCycle;
    private float chainNodeActiveUntil;
    private float chainNodeCntBeatCnt;
    private bool chainNodeAnimActive;

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

    void Start()
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
                tileCS[(y - 1) * maxMapSize + x].onGenerate((y - 1) * maxMapSize + x);
            }
        }
        for (int i = 0; i <= 5; ++i)
        {
            zoneSprites[i] = zoneObj[i].GetComponent<SpriteRenderer>();
            zoneObj[i].SetActive(false);
        }
        tilesHolder.Rotate(0, 0, 45);

        for (int i = 1; i <= MusicManager.musicCnt; ++i)
        {
            wiatForMusicStart[i] = new WaitForSeconds(MusicManager.musicStartTime[i]);
            beatSensorRotateArc = MusicManager.musicBPM[i] / 5f;
        }

        for (int i = 1; i <= maxNode; ++i)
        {
            nodeCopys[i] = Instantiate(nodeObj, nodesHolder);
            nodeCS[i] = nodeCopys[i].GetComponent<NodeCS>();
            nodeCopys[i].SetActive(false);
        }

        currentPlayingTrack = 1;
        for (int i = 1; i <= 3; ++i) zoneObj[i].SetActive(false);
        for (int i = 1; i <= 3; ++i) zoneActiveInfo[i].colorCode = 0;
        mapGlowObj.SetActive(false);

        beatSensor.rotation = Quaternion.Euler(0, 0, 0);
        gameState = -1;
        beatCnt = -PlusBeatInOneUpdate * maxMapSize - PlusBeatInOneUpdate;

        collide[0].tileNum = 0;

        afterBeatCntPlus = false;
        wrongInputPenalty = 0;

        nodeCycle = 0;
        nodeCntActive = 1;
        chainNodeActiveUntil = 0;
        chainNodeCntBeatCnt = 0;

        itsTime = 0;

        musicPlayer.clip = MusicManager.music[currentPlayingTrack];
        musicPlayer.Play();
        gameState = 0;
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
            chainTileCheck();

            beatSensor.Rotate(0, 0, beatSensorRotateArc);
        }
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
            if ((DifficultyViewerCS.difficulty <= 2 && nodeActiveInfo[i].eventNum >= eventNumClickNodeRange[0] && nodeActiveInfo[i].eventNum <= eventNumClickNodeRange[1]) || (DifficultyViewerCS.difficulty == 1 && nodeActiveInfo[i].eventNum >= eventNumChainNodeRange[0] && nodeActiveInfo[i].eventNum <= eventNumChainNodeRange[1])) nodeActiveInfo[i].eventNum = 0;

            float minusTiming =  Random.Range((int)(maxMapSize * 0.5f) + 1, maxMapSize);
            if (nodeActiveInfo[i].eventNum >= eventNumClickNodeRange[0] && nodeActiveInfo[i].eventNum <= eventNumClickNodeRange[1]) minusTiming = 8;
            else if (nodeActiveInfo[i].eventNum >= eventNumChainNodeRange[0] && nodeActiveInfo[i].eventNum <= eventNumChainNodeRange[1])
            {
                minusTiming = 5;
                gameUIManager.scoreBoardCS.nodeCnt += eventNumChainNodeGenerate[nodeActiveInfo[i].eventNum - eventNumChainNodeRange[0] + 1] - 1;
            }

            float activeTiming = nodesTimingCS.nodesTiming[currentPlayingTrack, i].timing + PlusBeatInOneUpdate - minusTiming * PlusBeatInOneUpdate;

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

        float currActiveTiming = 0;
        int first = 0;
        for (int i = 1; i <= nodesTimingCS.nodesTiming[currentPlayingTrack, 0].maxTiming; ++i)
        {
            if (currActiveTiming < nodeActiveInfo[i].activeTiming)
            {
                currActiveTiming = nodeActiveInfo[i].activeTiming;
                first = i;
            }
            else if (nodeActiveInfo[i].eventNum >= eventNumChainNodeRange[0] && nodeActiveInfo[i].eventNum <= eventNumChainNodeRange[1])
            {
                nodeActiveInfoDef chainNodeInfo = nodeActiveInfo[i];
                for (int k = i; k >= first + 1; --k) nodeActiveInfo[k] = nodeActiveInfo[k - 1];
                nodeActiveInfo[first] = chainNodeInfo;
            }
        }

        animationDelay[0] = new WaitForSeconds(0);
        animationDelay[1] = new WaitForSeconds(60 / MusicManager.musicBPM[currentPlayingTrack] / 4 * 0.9f);
        animationDelay[2] = new WaitForSeconds(60 / MusicManager.musicBPM[currentPlayingTrack] / (1 + (maxMapSize - 1) * 2) * 0.9f);
        animationDelay[3] = new WaitForSeconds(0.05f);

        animationDelayCondition = new WaitUntil(() => beatCnt == conditionBeatCnt);
    }

    private void beatCntPlus()
    {
        beatCnt += PlusBeatInOneUpdate;
        afterBeatCntPlus = true;

        if (wrongInputPenalty > 0)
        {
            --wrongInputPenalty;
            if (wrongInputPenalty == 0) StartCoroutine(mapGlowFade(0, false));
        }

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
            if (Input.GetMouseButtonUp(0) || AIPlay)
            {
                RaycastHit2D raycastHit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition) - Vector3.forward, Vector3.forward, 2);
                if ((raycastHit2D && int.Parse(raycastHit2D.collider.name) == clickTileNum) || AIPlay)
                {
                    StartCoroutine(tileCS[clickTileNum].glowImageSwitch(true, true));
                    gameUIManager.scoreBoardCS.onClickTiming(true);

                    int eventNum = (int)findActivatedNodeByBeatCnt(nodeCntActive - 1, beatCnt, 2);
                    if (eventNum == eventNumClickNodeAnim)
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
                if ((Input.GetKeyDown(key) && wrongInputPenalty == 0) || (AIPlay && collide[i].primeNodeRank == 3))
                {
                    if (collide[i].primeNodeRank == 3)
                    {
                        StartCoroutine(tileCS[collide[i].collideTileNum].glowImageSwitch(true));
                        gameUIManager.scoreBoardCS.onTapTiming(true);
                        tapSuccess = true;
                    }
                    else
                    {
                        if (DifficultyViewerCS.difficulty > 1) gameUIManager.scoreBoardCS.onCommonTap();
                        else
                        {
                            gameUIManager.scoreBoardCS.onTapTiming(true);
                            tapSuccess = true;
                        }
                    }

                    if (DifficultyViewerCS.difficulty == 4) StartCoroutine(zoneFade(i, collide[i].tileColor, true, true));

                    StartCoroutine(tileCS[collide[i].collideTileNum].ActivateTapSuccessEffect());
                    tileCS[collide[i].tileNum].changeTileColorAndInfo(collide[i].dir, false, -1, 0);
                    collide[i].primeNodeRank = 0;
                }
            }
            else if (Input.GetKeyDown(key))
            {
                StartCoroutine(mapGlowFade(0, true));
                wrongInputPenalty = plusWrongInputPenalty;
            }
        }

        if (tapSuccess)
        {
            int eventNum = (int)findActivatedNodeByBeatCnt(nodeCntActive - 1, beatCnt, 2);
            if (eventNum >= eventNumGlobalNodeRange[0] && eventNum <= eventNumGlobalNodeRange[1])
            {
                if (gameUIManager.scoreBoardCS.tapAll) StartCoroutine(MapAnimation(eventNum));
                gameUIManager.scoreBoardCS.tapAll = true;
            }
        }
    }

    private void chainTileCheck()
    {
        if (beatCnt > 0)
        {
            if (beatCnt < chainNodeActiveUntil)
            {
                if ((Input.GetKeyDown(KeyCode.LeftShift) && wrongInputPenalty == 0) || (AIPlay && AIDebug == 0))
                {
                    chainNodeCntBeatCnt = beatCnt;
                    if (AIPlay) AIDebug = 1;
                }
                if (Input.GetKeyUp(KeyCode.LeftShift) && chainNodeCntBeatCnt > 0) chainTilePointCnt();
                if (!chainNodeAnimActive)
                {
                    StartCoroutine(MapAnimation((int)findActivatedNodeByEventNum(nodeCntActive - 1, eventNumChainNodeRange[0], eventNumChainNodeRange[1], 2), eventChainNodeCollideTileNum));
                    chainNodeAnimActive = true;
                }
            }
            else if (chainNodeActiveUntil > 0) chainTilePointCnt();
        }
    }
    private void chainTilePointCnt()
    {
        int point = (int)((chainNodeCntBeatCnt > 0 ? beatCnt - chainNodeCntBeatCnt : 0) / PlusBeatInOneUpdate);
        gameUIManager.scoreBoardCS.onChainNodeTiming(point);
        chainNodeCntBeatCnt = 0;
        chainNodeActiveUntil = 0;
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
                if (tileCS[i].tileColor[0] == tileCS[i].tileColor[k]) tileCS[i].tileColorAndInfoToDefualt(0, false);

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
                else if (targetTileNum == i)
                {
                    if (i == eventChainNodeCollideTileNum)
                    {
                        tileCS[i].tileColorAndInfoToDefualt(1, false);
                        tileCS[i].tileColorAndInfoToDefualt(4, false);
                        chainNodeActiveUntil *= -1;
                        chainNodeAnimActive = false;
                    }
                    else collide[zone].tileNum = i;
                }
            }
        }

        for (int i = DifficultyViewerCS.difficulty <= 3 ? 1 : 4; i <= 5; ++i)
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
                    tileCS[5].changeTileColorAndInfo(1, false, maxColor + 1, eventChainNodeCollideTileNum);
                    tileCS[33].changeTileColorAndInfo(4, false, maxColor + 1, eventChainNodeCollideTileNum);
                    chainNodeActiveUntil = -(beatCnt + (maxMapSize * 0.5f + eventNumChainNodeGenerate[eventNum - eventNumChainNodeRange[0] + 1]) * PlusBeatInOneUpdate);
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

    private float findActivatedNodeByBeatCnt(int nodeCnt, float currBeatCnt, int returnType)
    {
        for (int i = nodeCnt; i >= 1; --i)
        {
            if (nodeActiveInfo[i].timing == currBeatCnt)
            {
                if (returnType == 1) return nodeActiveInfo[i].activeTiming;
                else if (returnType == 2) return nodeActiveInfo[i].eventNum;
            }
        }

        return -1;
    }
    private float findActivatedNodeByEventNum(int nodeCnt, int minEventNum, int maxEventNum, int returnType)
    {
        for (int i = nodeCnt; i >= 1; --i)
        {
            if (nodeActiveInfo[i].eventNum >= minEventNum && nodeActiveInfo[i].eventNum <= maxEventNum)
            {
                if (returnType == 1) return nodeActiveInfo[i].activeTiming;
                else if (returnType == 2) return nodeActiveInfo[i].eventNum;
            }
        }

        return -1;
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
                i = 1;
                onOff = false;
                autoOff = false;
            }
        }

        if (!onOff) zoneObj[zoneNum].SetActive(false);
    }
    private IEnumerator mapGlowFade(int color, bool onOff, bool autoOff = false)
    {
        if (onOff) mapGlowObj.SetActive(true);

        for (int i = 1; i <= 5; ++i)
        {
            if (onOff) mapGlowSprite.color = ColorByCode[color] + Color.black * (zoneAlpha * 0.2f * i - 1);
            else mapGlowSprite.color = ColorByCode[color] + Color.black * (zoneAlpha - zoneAlpha * 0.2f * i - 1);

            yield return mapGlowFadeTime;

            if (autoOff && i == 5)
            {
                i = 1;
                onOff = false;
                autoOff = false;
            }
        }

        if (!onOff) mapGlowObj.SetActive(false);
    }

    IEnumerator timer(WaitForSeconds sec)
    {
        itsTime = 1;
        yield return sec;
        itsTime = 2;
    }

    IEnumerator MapAnimation(int animNum, int originTileNum = 0)
    {
        if (animNum == eventNumClickNodeAnim)
        {
            for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
            {
                if (zoneByTileNum(i) == 0)
                {
                    tileCS[i].tileColorAndInfoToDefualt(0, false, true);
                }
            }

            zoneObj[0].SetActive(true);
            tileCS[originTileNum].changeTileColorAndInfo(0, false, maxColor + 1, -1, -1, true);

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
                            tileCS[i].changeTileColorAndInfo(0, false, maxColor + 1, -1, -1, true);
                        }
                    }
                }

                yield return animationDelay[3];
            }

            for (int i = 1; i <= maxMapSize * maxMapSize; ++i) if (zoneByTileNum(i) == 0) tileCS[i].tileColorAndInfoToDefualt(0, false);
            zoneObj[0].SetActive(false);
        }
        else if (animNum == eventNumChainNodeRange[0])
        {
            for (int i = 1; i <= eventNumChainNodeGenerate[1] / 2; ++i)
            {
                for (int k = 1; k <= i; ++k)
                {
                    tileCS[originTileNum + (i - 1) * maxMapSize + k - 1].changeTileColorAndInfo(0, false, maxColor + 1, -1, -1, true);
                    tileCS[originTileNum + (k - 1) * maxMapSize + i - 1].changeTileColorAndInfo(0, false, maxColor + 1, -1, -1, true);
                    if (chainNodeCntBeatCnt != 0)
                    {
                        StartCoroutine(tileCS[originTileNum + (i - 1) * maxMapSize + k - 1].glowImageSwitch(true, true));
                        StartCoroutine(tileCS[originTileNum + (k - 1) * maxMapSize + i - 1].glowImageSwitch(true, true));
                    }
                }
                conditionBeatCnt = beatCnt + 2 * PlusBeatInOneUpdate;
                yield return animationDelayCondition;
            }

            for (int y = maxMapSize / 2 + 1; y <= 8; ++y)
            {
                for (int x = maxMapSize / 2 + 1; x <= 8; ++x)
                {
                    tileCS[(y - 1) * maxMapSize + x].tileColorAndInfoToDefualt(0, false);
                }
            }
        }
        else if (animNum == eventNumChainNodeRange[0] + 1)
        {

        }
        else if (animNum == eventNumChainNodeRange[0] + 2)
        {

        }
        else if (animNum == eventNumGlobalAnim[1])
        {
            for (int i = 1; i <= 3; ++i) zoneObj[i].SetActive(false);

            mapGlowObj.SetActive(true);
            mapGlowSprite.color = ColorByCode[maxColor + 1];
            for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
            {
                for (int k = 0; k < maxNodeDir; ++k) tileCS[i].tileColorAndInfoToDefualt(k, false);
                StartCoroutine(tileCS[i].glowImageSwitch(false));
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

            for (int i = 1; i <= maxMapSize * maxMapSize; ++i) tileCS[i].tileColorAndInfoToDefualt(0, false);
            mapGlowObj.SetActive(false);
        }
        else if (animNum == eventNumGlobalAnim[2])
        {
            for (int i = 1; i <= 3; ++i) zoneObj[i].SetActive(false);

            mapGlowObj.SetActive(true);
            mapGlowSprite.color = ColorByCode[maxColor + 1];
            for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
            {
                for (int k = 0; k < maxNodeDir; ++k) tileCS[i].tileColorAndInfoToDefualt(k, false);
                StartCoroutine(tileCS[i].glowImageSwitch(false));
                tileCS[i].updateTileColor();
            }

            int pointTileNum = 0;
            for (int i = 1; i <= 3; ++i) if (collide[i].tileNum != 0) pointTileNum = collide[i].tileNum;

            tileCS[pointTileNum].changeTileColorAndInfo(0, false, maxColor + 1, -1, -1, true);
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
                            tileCS[i].changeTileColorAndInfo(0, false, maxColor + 1, -1, -1, true);
                        }
                    }
                }

                yield return animationDelay[2];
            }

            for (int i = 1; i <= maxMapSize * maxMapSize; ++i) tileCS[i].tileColorAndInfoToDefualt(0, false);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordBoardCS : MonoBehaviour
{
    private WaitForSeconds[] waitForSeconds = new WaitForSeconds[1];

    public RectTransform Rt;
    public RectTransform[] record = new RectTransform[9];
    public RectTransform[] recordPos = new RectTransform[9];

    private int recordCnt;

    void Start()
    {
        waitForSeconds[0] = new WaitForSeconds(0.01f);
    }

    void OnEnable()
    {
        Rt.anchoredPosition = new Vector2(0, -1276);
        Rt.rotation = Quaternion.Euler(0, 0, 0);

        recordCnt = 1;

        for (int i = 1; i <= 8; ++i) record[i].position = recordPos[i].position;
    }

    public IEnumerator move(bool allOrOne, int upOrDown, bool isMenu = true)
    {
        if (allOrOne)
        {
            for (int i = 1; i <= 10; ++i)
            {
                Rt.anchoredPosition += Vector2.up * upOrDown * 40;
                yield return waitForSeconds[0];
            }
            if (isMenu) StartCoroutine(move(false, 1));
        }
        else
        {
            for (int i = 1; i <= 10; ++i)
            {
                record[recordCnt].anchoredPosition += record[recordCnt].anchoredPosition.normalized * upOrDown * 6;
                yield return waitForSeconds[0];
            }
        }
    }

    public IEnumerator rotate(int leftOrRight)
    {
        int cnt = recordCnt - leftOrRight;
        if (cnt == 9) cnt = 1;
        if (cnt == 0) cnt = 8;

        record[cnt].position = recordPos[cnt].position;

        for (int i = 1; i <= 10; ++i)
        {
            transform.Rotate(0, 0, leftOrRight * -4.5f);
            for (int k = 1; k <= 8; ++k) record[k].Rotate(0, 0, leftOrRight * 4.5f);

            yield return waitForSeconds[0];
        }

        StartCoroutine(move(false, 1));
    }

    public void leftOrRightButtonUp(int leftOrRight)
    {
        recordCnt += leftOrRight;
        if (recordCnt == 9) recordCnt = 1;
        if (recordCnt == 0) recordCnt = 8;

        StartCoroutine(rotate(leftOrRight));
    }
}

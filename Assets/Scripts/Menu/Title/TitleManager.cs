using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    private WaitForSeconds[] waitForSeconds = new WaitForSeconds[2];

    public TitleCS titleCS;
    public LineCS lineCS;

    void Start()
    {
        waitForSeconds[0] = new WaitForSeconds(0.01f);
        waitForSeconds[1] = new WaitForSeconds(1f);
    }

    public IEnumerator activate(bool isActive)
    {
        if (isActive)
        {
            StartCoroutine(titleCS.moveUpOrDown(-1));
            yield return waitForSeconds[1];
            StartCoroutine(lineCS.enter());
        }
        else
        {
            StartCoroutine(titleCS.moveUpOrDown(1));
            StartCoroutine(lineCS.exit());
            yield return waitForSeconds[1];
            gameObject.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    private WaitForSeconds[] waitForSeconds = new WaitForSeconds[1];

    public RecordBoardCS recordBoardCS;
    public GameObject background;
    public GameObject setting_screen;

    public Animator leftAnim;
    public Animator rightAnim;
    public Animator settingAnim;
    public Animator musicInfoAnim;
    public Animator playAnim;

    void Start()
    {
        waitForSeconds[0] = new WaitForSeconds(1);
    }

    public IEnumerator activate(bool isActive)
    {
        if (isActive)
        {
            StartCoroutine(recordBoardCS.move(true, 1));
            setting_screen.SetActive(false);
        }
        else
        {
            background.SetActive(false);
            StartCoroutine(recordBoardCS.move(true, -1, false));
            setting_screen.SetActive(false);

            leftAnim.SetTrigger("reverse");
            rightAnim.SetTrigger("reverse");
            settingAnim.SetTrigger("reverse");
            musicInfoAnim.SetTrigger("reverse");
            playAnim.SetTrigger("reverse");

            yield return waitForSeconds[0];

            gameObject.SetActive(false);
        }
    }

    public void settingPointerUp()
    {
        setting_screen.SetActive(true);
    }

    public void settingExitPointerUp()
    {
        setting_screen.SetActive(false);
    }

    public void leftAndRightPointerUp()
    {
        leftAnim.SetTrigger("anim1");
        rightAnim.SetTrigger("anim1");
        musicInfoAnim.SetTrigger("anim1");
        playAnim.SetTrigger("anim1");
    }
}

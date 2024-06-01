using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    public Animator title;
    public Animator title_t;
    public Animator buttons;
    public Animator line_b;
    public Animator line_o;

    public AudioSource menuAudio;
    public MusicManager musicManager;

    private WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);

    private void OnEnable()
    {
        menuAudio.clip = musicManager.music[0];
        menuAudio.Play();
    }

    public IEnumerator onDeActivate()
    {
        title.SetTrigger("reverse");
        title_t.SetTrigger("reverse");
        buttons.SetTrigger("reverse");
        line_b.SetTrigger("disabled");
        line_o.SetTrigger("disabled");

        yield return waitForSeconds;
        gameObject.SetActive(false);
    }
}

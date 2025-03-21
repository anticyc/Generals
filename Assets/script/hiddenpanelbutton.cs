using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
public class hiddenpanelbutton : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator anim;
    public Animator hiddenpanel;

    public void jumpout()
    {
        StartCoroutine(changeanim(hiddenpanel, anim));
    }
    public void jumpback()
    {
        StartCoroutine(changeanim(anim, hiddenpanel));
    }
    IEnumerator changeanim(Animator anim1, Animator anim2)
    {
        anim1.SetBool("hiddenin", false);
        anim1.SetBool("hiddenout", true);
        yield return new WaitForSeconds(1f);
        anim2.SetBool("hiddenout", false);
        anim2.SetBool("hiddenin", true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hidddenout : MonoBehaviour
{
    public Animator hidden;
    public Animator fade;
    public void jump()
    {
        fade.SetBool("fadein",true);
        fade.SetBool("fadeout",false);
        
        hidden.SetBool("hiddenin",true);
        hidden.SetBool("hiddenout",false);
    }
    public void jumpback()
    { 
        hidden.SetBool("hiddenin",false);
        hidden.SetBool("hiddenout",true);  
        fade.SetBool("fadein",false);
        fade.SetBool("fadeout",true);        
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class singleselection : MonoBehaviour
{
    public Toggle pvp,pvpc;
    // private bool pvpclick=false;
    // Start is called before the first frame update
    void Start() {  pvp.isOn=false; pvpc.isOn=false; }
    // Update is called once per frame
    void Update()
    {   
        // if(pvp.isOn&&pvpc.isOn)
        // {
        //     if(pvpclick){pvp.isOn=false;pvpclick=false;}
        //     else {
        //         pvpc.isOn=false;
        //         pvpclick=true;
        //     }
        // }
    }
}

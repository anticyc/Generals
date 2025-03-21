using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class plslider : MonoBehaviour
{
    public Slider slider;
    // Start is called before the first frame update
   void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        GameObject.Find("gamedata").GetComponent<plslider>().slider= slider;
    }
}

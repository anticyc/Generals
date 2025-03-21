using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playernum : MonoBehaviour
{
    // Start is called before the first frame update
    public Toggle toggle;
    public Slider slider;
    // Update is called once per frame
    void Update()
    {
        if (toggle.isOn)
        {
            if (slider.value % 2 == 1)
                slider.value++;
        }
    }
}

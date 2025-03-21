using UnityEngine;
using UnityEngine.UI;

public class choosemapmode : MonoBehaviour
{
    // Start is called before the first frame update
    public Toggle toggle;
    public GameObject Hex;
    public GameObject Rec;
    void Start()
    {
        Hex.SetActive(false);
        Rec.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (toggle.isOn)
        {
            Hex.SetActive(true);
            Rec.SetActive(false);
        }
        else
        {
            Hex.SetActive(false);
            Rec.SetActive(true);
        }
    }
}

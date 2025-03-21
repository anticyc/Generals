using UnityEngine;
using UnityEngine.UI;

public class Mousezoom : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        GameObject.Find("gamedata").GetComponent<Mousezoom>().slider= slider;
    }
}


using UnityEngine;
using UnityEngine.UI;

public class Mousemove : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        GameObject.Find("gamedata").GetComponent<Mousemove>().slider = slider;
    }
}

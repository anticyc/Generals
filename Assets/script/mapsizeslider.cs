using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Mapsizeslider : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;

    /*
    在游戏开始时，确保特定的游戏对象（在这个例子中是"gamedata"）在场景切换时不被销毁，并且将当前脚本中的slider变量与"gamedata"游戏对象上的"Mapsizeslider"组件的slider属性进行绑定。
    */
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        GameObject.Find("gamedata").GetComponent<Mapsizeslider>().slider = slider;
    }
}

using UnityEngine;
using UnityEngine.UI; // 引入UI命名空间以使用Slider

public class Map_slider : MonoBehaviour
{
    public static Map_slider Instance;
    public Slider slider; // 拖拽Slider组件到这个字段
    public float value;
    private void Awake()
    {
        // 单例实现
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 确保在场景加载时不会销毁这个对象
        }
        else
            Destroy(gameObject); // 如果已经有一个实例存在，销毁这个新的实例
    }
    public void GetSliderValue()
    {
        Instance.value = slider.value;
    }
}
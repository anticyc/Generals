using UnityEngine;
using UnityEngine.UI;

public class SliderValueDisplay : MonoBehaviour
{
    public Slider targetSlider; // 拖拽Slider到这个字段
    public Text displayText; // 拖拽Text到这个字段

    void Update()
    {
        // 更新Text组件以显示Slider的当前值
        displayText.text = targetSlider.value.ToString(); // "F2"表示保留两位小数
    }
}

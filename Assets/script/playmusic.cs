using UnityEngine;
using UnityEngine.UI;

public class PlayMusicOnMouseClick : MonoBehaviour
{
    public AudioSource audioSource; // 指向AudioSource组件

    // Update is called once per frame
    void Update()
    {
        // 检测鼠标左键是否被点击
        if (Input.GetMouseButtonDown(0))
        {
            // 播放音乐
            audioSource.Play();
        }
    }
}


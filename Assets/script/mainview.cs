using UnityEngine;
using UnityEngine.UI;


public class Mainview : MonoBehaviour
{
    public float movementSpeed = 10.0f; // 控制摄像机移动速度
    public float zoomSpeed = 10f; // 缩放速度
    public Camera cam;
    public Vector3 newPosition = new Vector3(-4.4f, 0f, -74.9f);

    void Start()
    {
        // 获取场景中的主摄像机
        Slider slider = GameObject.Find("gamedata").GetComponent<Mousemove>().slider;
        movementSpeed = slider.value;
        Slider slider1 = GameObject.Find("gamedata").GetComponent<Mousezoom>().slider;
        zoomSpeed = slider1.value;
        // 设置摄像机的位置
        if (cam != null)
        {
            cam.transform.position = newPosition;
        }
    }
    public void targetatplayer()
    {
        if (cam != null)
        {
            cam.transform.position = newPosition;
        }
    }
    void Update()
    {
        // 获取摄像机的右向量和前向量
        Vector3 right = transform.right;
        Vector3 up = transform.up;
        // 将前向量投影到水平面上，忽略Y轴分量
        up.z = 0;
        up.Normalize();

        // 根据输入和速度计算移动向量
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = (right * horizontal + up * vertical).normalized * movementSpeed * Time.deltaTime;

        // 更新摄像机的位置
        transform.position += moveDirection;

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.Minus))
        {
            //调整高度
            cam.orthographicSize += zoomSpeed;
        }
        else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.Equals))
        {
            // 放大摄像机的视野（可选，如果你也想要实现放大功能）
            if (cam.orthographicSize > 20)
                cam.orthographicSize -= zoomSpeed;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class CameraHeightAdjuster : MonoBehaviour
{
    public Camera minicamera;
    void Start()
    {
        AdjustCameraHeight();
    }

    public void AdjustCameraHeight()
    {
        // 根据地图高度和摄像机与中心的距离计算摄像机的高度
        // 使用相似三角形原理，摄像机的高度 = 地图高度的一半 / 摄像机与地图中心的水平距离 * 摄像机的视野角度的一半
        // 注意：这里假设摄像机视野角度是垂直方向，且地图中心在水平面上
        float cameraHeight = minicamera.transform.position.z;
        int border_length = GameObject.Find("object(Clone)").GetComponent<Buildmap>().border_length;
        int max_j = border_length;
        minicamera.fieldOfView = Mathf.Atan(4.9f * max_j / Mathf.Abs(cameraHeight)) * 2f * Mathf.Rad2Deg;
    }
}

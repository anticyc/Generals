using UnityEngine;

public class MiniMapCameraView : MonoBehaviour
{
    public Camera mainCamera; // 大摄像机的引用
    public RectTransform miniMapRect; // 小地图矩形的RectTransform
    public float miniMapSize; // 小地图的大小（假设是正方形）

    void Update()
    {
        // 计算摄像机视野的边界点
        Vector3[] frustumCorners = new Vector3[4];
        mainCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), mainCamera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

        // 将世界坐标转换为小地图坐标
        Vector3 miniMapCenter = miniMapRect.transform.position;
        Vector3[] miniMapCorners = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            // 转换坐标并缩放到小地图尺寸
            Vector3 worldCorner = mainCamera.transform.TransformPoint(frustumCorners[i]);
            Vector3 viewPortCorner = mainCamera.WorldToViewportPoint(worldCorner);
            Vector3 miniMapCorner = new Vector3(viewPortCorner.x * miniMapSize, viewPortCorner.y * miniMapSize, 0) + miniMapCenter;
            miniMapCorners[i] = miniMapCorner;
        }

        // 设置小地图矩形的顶点
        SetRectCorners(miniMapRect, miniMapCorners);
    }

    void SetRectCorners(RectTransform rect, Vector3[] corners)
    {
        // 顶点顺序为左下、左上、右上、右下
        rect.anchorMin = new Vector2(corners[0].x, corners[0].y);
        rect.anchorMax = new Vector2(corners[2].x, corners[2].y);
        rect.offsetMin = new Vector2(0, 0);
        rect.offsetMax = new Vector2(0, 0);
    }
}

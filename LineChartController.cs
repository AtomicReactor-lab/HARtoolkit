using UnityEngine;

public class LineChartController : MonoBehaviour
{
    public GameObject pointPrefab; // 拖拽点的预设
    public LineRenderer lineRenderer; // LineRenderer组件
    public float[] dataPoints; // Y轴上的数据点
    public Vector2 origin; // 原点位置
    public float xSpacing = 50f; // X轴上点之间的间隔

    private GameObject[] points; // 存储实际点的GameObject数组

    void Start()
    {
        CreatePoints();
        UpdateLine();
    }

    void Update()
    {
        UpdateLine(); // 实时更新线条
    }

    void CreatePoints()
    {
        points = new GameObject[dataPoints.Length];
        for (int i = 0; i < dataPoints.Length; i++)
        {
            Vector2 pointPosition = new Vector2(origin.x + i * xSpacing, origin.y + dataPoints[i]);
            GameObject point = Instantiate(pointPrefab, pointPosition, Quaternion.identity);
            point.transform.SetParent(transform, false);
            points[i] = point;
        }
    }

    void UpdateLine()
    {
        lineRenderer.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i].transform.position);
        }
    }
}

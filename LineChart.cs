using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LineChart : MonoBehaviour
{
    public RectTransform graphContainer;
    public GameObject pointPrefab;
    public float[] dataArray; // 公开的数组，用于在Inspector中输入数据

    private List<GameObject> points;

    void Start()
    {
        points = new List<GameObject>();
        if (dataArray.Length > 0)
        {
            UpdateGraph(dataArray);
        }
    }

    public void UpdateGraph(float[] data)
    {
        ClearGraph();
        float graphHeight = graphContainer.sizeDelta.y;
        float yMaximum = 10f; // 设置Y轴的最大值
        float xSize = 50f; // 设置X轴间隔

        for (int i = 0; i < data.Length; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = (data[i] / yMaximum) * graphHeight;
            GameObject newPoint = CreatePoint(new Vector2(xPosition, yPosition));
            points.Add(newPoint);

            if (i > 0)
            {
                // 连接点
                GameObject newLine = CreateLine(points[i - 1].transform.position, newPoint.transform.position, graphContainer);
                newLine.transform.SetParent(graphContainer, false);
            }
        }
    }

    GameObject CreatePoint(Vector2 anchoredPosition)
    {
        GameObject point = Instantiate(pointPrefab);
        point.transform.SetParent(graphContainer, false);
        point.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        return point;
    }

    GameObject CreateLine(Vector2 start, Vector2 end, Transform container)
    {
        GameObject lineObj = new GameObject("Line", typeof(Image));
        lineObj.transform.SetParent(container, false);
        lineObj.GetComponent<Image>().color = Color.green; // 线条颜色

        RectTransform rt = lineObj.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = Vector2.zero;
        rt.sizeDelta = new Vector2(Vector2.Distance(start, end), 3f); // 线条宽度

        Vector2 middlePoint = (start + end) / 2;
        rt.localPosition = middlePoint;
        rt.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(end - start));

        return lineObj;
    }

    float GetAngleFromVectorFloat(Vector2 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    void ClearGraph()
    {
        foreach (GameObject point in points)
        {
            Destroy(point);
        }
        points.Clear();

        foreach (Transform child in graphContainer.transform)
        {
            if (child.name == "Line")
            {
                Destroy(child.gameObject);
            }
        }
    }
}

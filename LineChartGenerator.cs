using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using System.Text.RegularExpressions;

public class LineChartGenerator : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private float xSize = 50f; // 点之间的距离
    [SerializeField] private Vector2 circleSize = new Vector2(11, 11); // 点的大小
    [SerializeField] private float lineWidth = 3f; // 线条粗细
    [SerializeField] private List<int> valueList = new List<int>(); // 数据列表
        // 定义额外的数据列表
    [SerializeField] private List<int> valueListLine2 = new List<int>();
    [SerializeField] private List<int> valueListLine3 = new List<int>();
    [SerializeField] private List<int> valueListLine4 = new List<int>();

    // 可以为每条线定义不同的颜色
    [SerializeField] private Color lineColor = Color.white; // 线条颜色
    [SerializeField] private Color lineColorLine2 = Color.red;
    [SerializeField] private Color lineColorLine3 = Color.blue;
    [SerializeField] private Color lineColorLine4 = Color.green;

    [SerializeField] private GameObject lineGraph1;
    [SerializeField] private GameObject lineGraph2;
    [SerializeField] private GameObject lineGraph3;
    [SerializeField] private GameObject lineGraph4;


    private RectTransform graphContainer;
    private List<GameObject> graphObjects; // 用于存储图表中的所有游戏对象

    private void Awake() {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        graphObjects = new List<GameObject>();

        lineGraph1 = new GameObject("LineGraph1");
        lineGraph1.transform.SetParent(graphContainer.transform, false);
        lineGraph2 = new GameObject("LineGraph2");
        lineGraph2.transform.SetParent(graphContainer.transform, false);
        lineGraph3 = new GameObject("LineGraph3");
        lineGraph3.transform.SetParent(graphContainer.transform, false);
        lineGraph4 = new GameObject("LineGraph4");
        lineGraph4.transform.SetParent(graphContainer.transform, false);

        ShowGraph(valueList, lineColor, lineGraph1);
        ShowGraph(valueListLine2, lineColorLine2, lineGraph2);
        ShowGraph(valueListLine3, lineColorLine3, lineGraph3);
        ShowGraph(valueListLine4, lineColorLine4, lineGraph4);

        lineGraph1.SetActive(false);
        lineGraph2.SetActive(false);
        lineGraph3.SetActive(false);
        lineGraph4.SetActive(false);
    }

    void Start() {
        GPTCommunication gptComm = FindObjectOfType<GPTCommunication>();
        if (gptComm != null)
        {
            string gptCommand = gptComm.latestGPTCommand;
            if (!string.IsNullOrEmpty(gptCommand))
            {
                // 解析命令并执行
                LineProcessGPTCommands(gptCommand);
                gptComm.latestGPTCommand = ""; // 清除命令以避免重复处理
            }
        }
    }
    private void UpdateGraph() {
        // 先清除旧的图表对象
        foreach (var obj in graphObjects) {
            Destroy(obj);
        }
        graphObjects.Clear();

        // 重新绘制图表
        ShowGraph(valueList, lineColor, lineGraph1);
        ShowGraph(valueListLine2, lineColorLine2, lineGraph2);
        ShowGraph(valueListLine3, lineColorLine3, lineGraph3);
        ShowGraph(valueListLine4, lineColorLine4, lineGraph4);
    }

    private GameObject CreateCircle(Vector2 anchoredPosition, GameObject parentObject) {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(parentObject.transform, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = circleSize;
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        graphObjects.Add(gameObject); // 添加到图表对象列表中
        return gameObject;
    }

    private void ShowGraph(List<int> valueList, Color lineColor, GameObject lineGraph) {
        float graphHeight = graphContainer.sizeDelta.y;
        float yMaximum = 100f;

        GameObject lastCircleGameObject = null;
        for (int i = 0; i < valueList.Count; i++) {
            float xPosition = i * xSize;
            float yPosition = (valueList[i] / yMaximum) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition), lineGraph);
            if (lastCircleGameObject != null) {
                CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition, lineColor, lineGraph);
            }
            lastCircleGameObject = circleGameObject;
        }
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB, Color lineColor, GameObject parentObject) {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(parentObject.transform, false);
        Image lineImage = gameObject.GetComponent<Image>();
        lineImage.color = lineColor;
        RectTransform rectTransform = lineImage.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, lineWidth);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
        graphObjects.Add(gameObject); // 添加到图表对象列表中
    }

    // 可以从其他脚本或编辑器中调用此方法来更新图表
    public void RedrawGraph() {
        UpdateGraph();
    }

    private void LineProcessGPTCommands(string command){
        Regex commandRegex = new Regex(@"charttype:\s*(.+?);\s*operation:\s*(.+?);\s*parameter:\s*(\d+);\s*data:\s*(\d+);");
        Match match = commandRegex.Match(command);
        if (match.Success){
            string chartType = match.Groups[1].Value;
            string operation = match.Groups[2].Value;
            int parameter = int.Parse(match.Groups[3].Value);
            int getdata = int.Parse(match.Groups[4].Value);
            // 根据解析出的参数更新 line chart
            if (chartType == "linechart" && operation == "highlight") {
                // 首先隐藏所有lineGraph
                lineGraph1.SetActive(false);
                lineGraph2.SetActive(false);
                lineGraph3.SetActive(false);
                lineGraph4.SetActive(false);

                // 根据parameter的值显示对应的lineGraph
                switch (parameter){
                    case 1:
                        lineGraph1.SetActive(true);
                        break;
                    case 2:
                        lineGraph2.SetActive(true);
                        break;
                    case 3:
                        lineGraph3.SetActive(true);
                        break;
                    case 4:
                        lineGraph4.SetActive(true);
                        break;
                    default:
                        // 可能需要处理无效的parameter值
                        break;
                }
                // 可以添加更多操作的处理
            }
        }
    }
}

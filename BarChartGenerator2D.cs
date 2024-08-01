using UnityEngine;
using System.Text.RegularExpressions;
public class BarChartGenerator2D : MonoBehaviour
{
    public Vector3 firstBarPosition = new Vector3(0f, 0f, 0f);// 第一个bar的位置
    public float barSpacing; // 每个bar之间的距离
    public float barWidth; // Bar的宽度
    public float[] data; // 数据数组
    public float[] inspectorData; //creation用的bardata
    private GameObject[] creationBarObjects; // 用于存储creation对应所有的bars
    private GameObject[] bars; // 用于存储所有的bars
    private GameObject CreationBarsParent;
    private GameObject CreationBarG2;
    private GameObject CreationBarG2txt;
    private bool barsVisible = true; // 跟踪bars的可见状态
    private GameObject[] textObjects; // 用于存储所有的文字对象

    public float referenceLineValue; // 辅助线对应的值
    private GameObject referenceLine; // 辅助线对象

    private bool referenceLineVisible = true; // 跟踪辅助线的可见状态
    private bool creationBarVisible = false; // 跟踪creationbar的可见状态
    private bool trendlineStraightVisible = true; // 跟踪趋势线的可见状态

    public GameObject trendlineStraight;
    public GameObject reflines;

    void Start()
    {

        CreationBarsParent = GameObject.Find("creationbars");
        CreationBarG2 = GameObject.Find("barG2");
        CreationBarG2txt = GameObject.Find("barG2txt");

        reflines = GameObject.Find("reflines");

        trendlineStraight = GameObject.Find("trendlinestraight");

        CreationBarsParent.SetActive(false);
        CreationBarG2.SetActive(false);
        CreationBarG2txt.SetActive(false);
        
        trendlineStraight.SetActive(false);
        reflines.SetActive(false);

        bars = new GameObject[data.Length];
        textObjects = new GameObject[data.Length]; // 初始化文字对象数组
        GenerateBarChart();
        BarGenerateTrendlinePoly();
        BarGenerateTrendlineStraight(trendlineStraight);
        
        // 初始时隐藏所有bars和文本
        ToggleBarsVisibility(false);
        // 初始时隐藏辅助线
        if (referenceLine != null)
        {
            referenceLine.SetActive(false);
            referenceLineVisible = false;
        }
        // 初始化creationBarObjects数组
        creationBarObjects = new GameObject[inspectorData.Length];
        for (int i = 0; i < inspectorData.Length; i++)
        {
            string barName = "bar" + (i + 1).ToString(); // 根据命名规则构建bar名称
            creationBarObjects[i] = GameObject.Find(barName); // 从场景中找到bar
            if (creationBarObjects[i] == null)
            {
                Debug.LogError("Bar named " + barName + " is not found!");
            }
        }

        // 调用Creation方法更新bars
        CreationBarsFunc();
    }

    void CreationBarsFunc()
    {
        for (int i = 0; i < creationBarObjects.Length; i++)
        {
            if (creationBarObjects[i] != null && i < inspectorData.Length)
            {
                Transform barTransform = creationBarObjects[i].transform;
                float oldScaleZ = barTransform.localScale.z;
                float oldPosY = barTransform.position.y;

                // 根据inspectorData数组中的数据调整bars的scale的z值
                Vector3 newScale = new Vector3(barTransform.localScale.x, barTransform.localScale.y, inspectorData[i]);
                barTransform.localScale = newScale;

                // 根据新的scale的z值调整position的y值
                Vector3 newPosition = new Vector3(barTransform.position.x, (newScale.z - oldScaleZ) * 5 + oldPosY, barTransform.position.z); //5倍关系
                barTransform.position = newPosition;
            }
        }
    }

    void BarGenerateTrendlinePoly() //trendline #polyline
    {
        referenceLine = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Material redMaterial = new Material(Shader.Find("Unlit/Color"));
        redMaterial.color = Color.red;

        referenceLine.GetComponent<Renderer>().material = redMaterial;
        referenceLine.transform.parent = transform;
        referenceLine.transform.rotation = Quaternion.Euler(0, 0, 0); // 将Plane旋转为水平

        
        float lineLength = barWidth * data.Length + (barSpacing/10) * (data.Length - 1);
        float linex = barWidth * data.Length + barSpacing * (data.Length - 1);
        referenceLine.transform.localScale = new Vector3(lineLength, 1, 0.1f); // 设置辅助线的长度和厚度
        firstBarPosition.z = -0.1f; //负的在上，我也不知道为什么
        referenceLine.transform.localPosition = new Vector3(firstBarPosition.x + linex / 2f - barWidth / 2f, referenceLineValue, firstBarPosition.z); //辅助线的位置
    }


    void BarGenerateTrendlineStraight(GameObject trendlineStraight)
    {
        if (trendlineStraight == null) return;

        // 计算线性回归的斜率和截距
        float sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;
        int n = data.Length;
        for (int i = 0; i < n; i++)
        {
            float x = i * (barWidth + barSpacing);
            float y = data[i];
            sumX += x;
            sumY += y;
            sumXY += x * y;
            sumX2 += x * x;
        }

        float slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
        float intercept = (sumY - slope * sumX) / n;

        // 计算旋转角度（弧度转为度）
        float angle = Mathf.Atan(slope) * Mathf.Rad2Deg;

        // 应用旋转和位置
        trendlineStraight.transform.rotation = Quaternion.Euler(0, angle, 0); // Y轴旋转
        trendlineStraight.transform.position = new Vector3(
            trendlineStraight.transform.position.x, 
            trendlineStraight.transform.position.y, 
            trendlineStraight.transform.position.z + intercept); // Z轴位置设置为截距
    }
    void GenerateBarChart()
    {
        Vector3 nextBarPosition = firstBarPosition;

        // 创建材质
        Material blueMaterial = new Material(Shader.Find("Unlit/Color"));
        blueMaterial.color = Color.cyan;

        for (int i = 0; i < data.Length; i++)
        {
            // 创建bar
            GameObject newBar = GameObject.CreatePrimitive(PrimitiveType.Plane);
            Destroy(newBar.GetComponent<Collider>()); // 移除Collider
            newBar.transform.parent = transform;

            // 创建显示数据的文本
            GameObject textObj = new GameObject("DataText");
            TextMesh textMesh = textObj.AddComponent<TextMesh>();
            textMesh.text = data[i].ToString();
            textMesh.color = Color.black; // 直接设置文本颜色为黑色
            textMesh.alignment = TextAlignment.Center;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.characterSize = 0.1f;
            textMesh.fontSize = 300;

            // 底边对齐
            nextBarPosition.y = firstBarPosition.y + (data[i] - data[0]) * 5;
        
            // 设置Bar的位置
            Vector3 barPosition = new Vector3(nextBarPosition.x, nextBarPosition.y, nextBarPosition.z);
            newBar.transform.localPosition = barPosition;

            // 设置Bar的尺寸和方向
            newBar.transform.localScale = new Vector3(barWidth, 1, data[i]);
            newBar.transform.rotation = Quaternion.Euler(0, 0, 0);

            // 应用蓝色材质到bar
            newBar.GetComponent<Renderer>().material = blueMaterial;

            // 存储bar对象
            bars[i] = newBar;
            // 存储文字对象
            textObjects[i] = textObj;

            // 设置文本位置
            textObj.transform.parent = transform;
            textObj.transform.localPosition = new Vector3(nextBarPosition.x, nextBarPosition.y + 2 * data[i] + 0.1f, nextBarPosition.z); // 放在bar上方
            textObj.transform.rotation = Quaternion.Euler(90, 0, 0);

            // 设置合适的本地缩放
            textObj.transform.localScale = new Vector3(1, 1, 1);

            // 更新下一个bar的位置 #一定要放在最后！！！
            nextBarPosition.x += barWidth + barSpacing;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket)) // [键控制
        {
            barsVisible = !barsVisible; // 切换可见状态
            ToggleBarsVisibility(barsVisible);
        }

        if (Input.GetKeyDown(KeyCode.RightBracket)) // ]键控制
        {
            referenceLineVisible = !referenceLineVisible; // 切换可见状态
            if (referenceLine != null)
            {
                referenceLine.SetActive(referenceLineVisible); // 设置辅助线的可见性
            }
        }

        for (KeyCode k = KeyCode.Alpha1; k <= KeyCode.Alpha9; k++)
        {
            if (Input.GetKeyDown(k))
            {
                int index = k - KeyCode.Alpha1;
                if (index < bars.Length)
                {
                    ToggleBarsVisibility(false); // 隐藏所有bars和文字
                    bars[index].SetActive(true); // 显示选定的bar
                    textObjects[index].SetActive(true); // 显示选定bar的文字
                    barsVisible = true; // 更新状态为可见
                }
            }
        }

        GPTCommunication gptComm = FindObjectOfType<GPTCommunication>();
        if (gptComm != null)
        {
            string gptCommand = gptComm.latestGPTCommand;
            if (!string.IsNullOrEmpty(gptCommand))
            {
                // 解析命令并执行
                BarProcessGPTCommands(gptCommand);
                gptComm.latestGPTCommand = ""; // 清除命令以避免重复处理
            }
        }

    }

    void ToggleBarsVisibility(bool isVisible)
    {
        for (int i = 0; i < bars.Length; i++)
        {
            if (bars[i] != null)
                bars[i].SetActive(isVisible);
            if (textObjects[i] != null)
                textObjects[i].SetActive(isVisible); // 同步更改文字对象的可见性
        }
    }

    public void HighlightBar(int index)
    {
        if (index >= 0 && index < bars.Length)
        {
            ToggleBarsVisibility(false); // 隐藏所有 bars
            bars[index].SetActive(true); // 显示指定的 bar
            if (textObjects[index] != null)
                textObjects[index].SetActive(true); // 显示选定 bar 的文字
        }
    }

    private void BarProcessGPTCommands(string command)
    {
        int n = data.Length;
        Regex commandRegex = new Regex(@"charttype:\s*(.+?);\s*operation:\s*(.+?);\s*parameter:\s*(\d+);\s*data:\s*(\d+);");
        Match match = commandRegex.Match(command);
        if (match.Success)
        {
            string chartType = match.Groups[1].Value;
            string operation = match.Groups[2].Value;
            int parameter = int.Parse(match.Groups[3].Value);
            int getdata = int.Parse(match.Groups[4].Value);
            // 根据解析出的参数更新 bar chart
            if (chartType == "barchart" && operation == "highlight") // highlight
            {
                parameter = parameter - 1;
                HighlightBar(parameter);
            }

            if (chartType == "barchart" && operation == "extend") // extension
            {
                HighlightBar(n);
            }

            if (chartType == "barchart" && operation == "trend") // trendline
            {
                // 设置趋势线的可见性
                if (trendlineStraight != null)
                {
                    trendlineStraightVisible = !trendlineStraightVisible; // 切换趋势线的可见状态
                    // trendlineStraight.SetActive(trendlineStraightVisible); // 根据状态设置趋势线的可见性
                    trendlineStraight.SetActive(true);
                }
            }

            if (chartType == "barchart" && operation == "reference") // reference structure
            {
                if (reflines != null)
                {
                    reflines.SetActive(true);
                }
            }

            if (chartType == "barchart" && operation == "number")
            {
                // 遍历所有文字对象，并设置它们的可见性为true
                foreach (GameObject textObject in textObjects)
                {
                    if (textObject != null)
                    {
                        textObject.SetActive(true);
                    }
                }
            }

            if (chartType == "barchart" && operation == "create")
            {
                if (creationBarObjects != null)
                {
                    creationBarVisible = !creationBarVisible;
                    CreationBarsParent.SetActive(creationBarVisible);
                    CreationBarG2.SetActive(true);
                    CreationBarG2txt.SetActive(true);
                }
            }

            if (chartType == "barchart" && operation == "summerize")
            {
                // 设置参考线的可见性
                if (referenceLine != null)
                {
                    referenceLineVisible = !referenceLineVisible; // 切换参考线的可见状态
                    referenceLine.SetActive(referenceLineVisible); // 根据状态设置参考线的可见性
                }
            }
            // 可以添加更多操作的处理
        }
    }
}

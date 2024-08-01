using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // 引入TextMesh Pro命名空间

public class PieChartGenerator : MonoBehaviour
{
    public GameObject piePiecePrefab; // 预制体用于动态创建饼图部分
    public TextMeshProUGUI pieceLabelPrefab; // TextMesh Pro文本预制体用于显示数据
    public Color textColor; // 所有文本标签的颜色
    private List<GameObject> piePieces = new List<GameObject>(); // 存储动态创建的饼图部分
    private List<TextMeshProUGUI> pieceLabels = new List<TextMeshProUGUI>(); // 存储每个部分的文本标签
    private int currentPieceIndex = 0; // 当前显示的饼图部分索引
    private bool isPieVisible = false; // 控制饼图的显示状态

    public float[] values; // 从Inspector中输入的数据
    public Color[] colors; // 饼图每部分的颜色

    // Start is called before the first frame update
    void Start()
    {
        TogglePieVisibility(false);
        SetValues(values);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePieVisibility(!isPieVisible);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ShowNextPiece();
        }
    }

    public void SetValues(float[] valuesToSet)
    {
        ClearPieChart();

        float total = 0;
        float lastAngle = 0;
        Color[] generatedColors = GenerateColors(valuesToSet.Length); // 自动生成颜色

        for (int i = 0; i < valuesToSet.Length; i++)
        {
            total += valuesToSet[i];
        }

        for (int i = 0; i < valuesToSet.Length; i++)
        {
            float fillAmount = valuesToSet[i] / total;
            GameObject piePiece = Instantiate(piePiecePrefab, transform);
            piePiece.GetComponent<Image>().fillAmount = fillAmount;
            piePiece.GetComponent<Image>().color = generatedColors[i]; // 使用自动生成的颜色
            piePiece.transform.rotation = Quaternion.Euler(new Vector3(90, 0, -lastAngle * 360)); //这个地方改角度

            lastAngle += fillAmount;
            piePieces.Add(piePiece);

            // 创建并设置TextMesh Pro文本标签
            TextMeshProUGUI label = Instantiate(pieceLabelPrefab, transform);
            label.text = valuesToSet[i].ToString();
            // 将文本标签置于饼图块上方
            label.transform.SetAsLastSibling();
            label.transform.position = CalculateLabelPosition(piePiece.transform.position, fillAmount, lastAngle);
            label.color = textColor; // 使用Inspector中设置的文本颜色
            pieceLabels.Add(label);
        }

        TogglePieVisibility(isPieVisible);
    }

    // 自动生成颜色的方法
    private Color[] GenerateColors(int numberOfColors)
    {
        Color[] colors = new Color[numberOfColors];
        for (int i = 0; i < numberOfColors; i++)
        {
            float hue = (float)i / numberOfColors;
            colors[i] = Color.HSVToRGB(hue, 0.7f, 0.9f); // 调整饱和度和亮度值以获得鲜艳的颜色
        }
        return colors;
    }

    private void TogglePieVisibility(bool visible)
    {
        isPieVisible = visible;
        foreach (var piece in piePieces)
        {
            piece.SetActive(visible);
        }
        foreach (var label in pieceLabels)
        {
            label.gameObject.SetActive(visible);
        }
    }

    private void ShowNextPiece()
    {
        if (piePieces.Count == 0) return;

        piePieces[currentPieceIndex].SetActive(false);
        pieceLabels[currentPieceIndex].gameObject.SetActive(false);

        currentPieceIndex = (currentPieceIndex + 1) % piePieces.Count;

        piePieces[currentPieceIndex].SetActive(true);
        pieceLabels[currentPieceIndex].gameObject.SetActive(true);
    }

    private Vector3 CalculateLabelPosition(Vector3 piecePosition, float fillAmount, float angle)
    {
        // 计算文本标签的位置
        // ...
        return new Vector3(); // 返回计算后的位置
    }

    private void ClearPieChart()
    {
        foreach (var piece in piePieces)
        {
            Destroy(piece);
        }
        piePieces.Clear();

        foreach (var label in pieceLabels)
        {
            Destroy(label.gameObject);
        }
        pieceLabels.Clear();
    }
}

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

public class GPTCommunication : MonoBehaviour
{
    private UIManager uiManager; // UI管理器的引用
    public string latestGPTCommand; // 用于存储最新的 GPT 命令

    // 存储对话历史的列表
    private List<Message> conversationHistory = new List<Message>();

    private void Start()

    {
        uiManager = FindObjectOfType<UIManager>(); // 获取UIManager组件
        SendMessageToGPT("The task is like this: I will give you some parameters, including charttype, operation, parameter, and data. I need you to convert these parameters into instructions in this format: #charttype:; #operation:; #parameter:; #data:; Also, the parameter and data should be numbers. Furthermore, if I have not specially assgined the data and parameter, they are default 0. Your output will be automatically entered into my program. The output should be in a line. The #charttype you got 4 choices: barchart, linechart, piechart and scatterplot. The #operation you got 7 choices: highlight, extend, trend, create, number, reference, summerize. The final answer should be like this example: #charttype:linechart; #operation:extend; #parameter:2; #data:0; Furthermore, since these instructions are used for drawing charts, when I ask you to draw various charts, you just need to output the instructions. There's no need to actually draw a chart. My program will automatically create the chart based on your instructions. Do you understand? If you do it well, I'll give you a $100 reward");
    }

    public void SendMessageToGPT(string userInput)
    {
        // 将用户输入添加到对话历史
        conversationHistory.Add(new Message { role = "user", content = userInput });

        // 开始发送请求
        StartCoroutine(PostRequest());
    }

    IEnumerator PostRequest()
    {
        var data = new Data
        {
            // model = "gpt-3.5-turbo",
            model = "gpt-4-0613",
            messages = conversationHistory.ToArray(),
            temperature = 1.0
        };

        string jsonData = JsonConvert.SerializeObject(data);

        using (var request = new UnityWebRequest("https://gpt-api.hkust-gz.edu.cn/v1/chat/completions", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "open_ai_key"); // 替换为你的API密钥

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                var response = JsonConvert.DeserializeObject<GPTResponse>(request.downloadHandler.text);
                var responseMessage = response.choices[0].message.content;

                // 将API的响应添加到对话历史
                conversationHistory.Add(new Message { role = "system", content = responseMessage });

                // 显示响应
                uiManager.DisplayGPTResponse(responseMessage);

                ExtractGPTResponseParameters(responseMessage);
            }
        }
    }

    private void ExtractGPTResponseParameters(string gptResponse)
    {
        // 定义正则表达式模式
        string pattern = @"#(charttype|operation|parameter|data):([^;]+);";

        // 使用正则表达式匹配
        MatchCollection matches = Regex.Matches(gptResponse, pattern);

        // 用于存储所有提取参数的字符串
        string extractedParameters = "";

        // 遍历所有匹配项并提取
        foreach (Match match in matches)
        {
            if (match.Groups.Count == 3)
            {
                string key = match.Groups[1].Value;  // charttype, operation, parameter, 或 data
                string value = match.Groups[2].Value; // 对应的值

                // 将提取的键值对添加到字符串中
                extractedParameters += key + ": " + value + "; ";
            }
        }

        latestGPTCommand = extractedParameters;
        // 输出提取的参数
        Debug.Log("Extracted GPT Parameters: " + extractedParameters);
    }

    [System.Serializable]
    public class Data
    {
        public string model;
        public Message[] messages;
        public double temperature;
    }

    [System.Serializable]
    public class GPTResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    public class Choice
    {
        public Message message;
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }
}

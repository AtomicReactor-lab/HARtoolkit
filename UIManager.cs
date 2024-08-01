using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public TMP_InputField inputField; // 用户输入字段
    public TextMeshProUGUI conversationText;  // 对话文本
    public Button sendButton; // 发送按钮
    public float typingSpeed = 0.05f; // 打字机效果的速度

    private GPTCommunication gptCommunication; // 用于通信的脚本引用
    private bool isTyping = false; // 标记是否正在进行打字机效果

    private void Start()
    {
        gptCommunication = FindObjectOfType<GPTCommunication>(); // 获取GPTCommunication组件
        sendButton.onClick.AddListener(SendMessage);
    }

    private void SendMessage()
    {
        // if (isTyping) return; // 如果正在打字机效果中，则不允许发送新消息

        string userInput = inputField.text;
        gptCommunication.SendMessageToGPT(userInput);
        conversationText.text += $"\nUser: {userInput}";
        inputField.text = ""; // 清空输入字段
    }

    public void DisplayGPTResponse(string message)
    {
        StartCoroutine(TypeResponseText($"\nGPT: {message}"));
    }

    IEnumerator TypeResponseText(string fullText)
    {
        isTyping = true;
        int currentLength = 0;

        while (currentLength <= fullText.Length)
        {
            conversationText.text += fullText.Substring(currentLength, 1);
            currentLength++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}

// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;

// public class UIManager : MonoBehaviour
// {
//     public TMP_InputField inputField; // 用户输入字段
//     public TextMeshProUGUI conversationText;  // 对话文本
//     public Button sendButton; // 发送按钮

//     private OpenAIGPTCommunication openAIGPTCommunication; // 用于通信的脚本引用

//     private void Start()
//     {
//         openAIGPTCommunication = FindObjectOfType<OpenAIGPTCommunication>(); // 获取OpenAIGPTCommunication组件
//         sendButton.onClick.AddListener(SendMessage);
//     }

//     private void SendMessage()
//     {
//         string userInput = inputField.text;
//         if (openAIGPTCommunication != null && !string.IsNullOrWhiteSpace(userInput))
//         {
//             openAIGPTCommunication.SendMessageToGPT(userInput);
//             conversationText.text += $"\nUser: {userInput}";
//             inputField.text = ""; // 清空输入字段
//         }
//     }

//     public void DisplayGPTResponse(string message)
//     {
//         conversationText.text += $"\nGPT: {message}";
//     }
// }

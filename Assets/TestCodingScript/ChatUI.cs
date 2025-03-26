// ✅ 这是一个完整的 Unity 聊天界面脚本（支持 TextMeshPro）
// UI 要求：
// - TMP_InputField 命名为 inputField
// - Button 命名为 sendButton
// - TextMeshProUGUI 命名为 chatHistoryText

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Text.RegularExpressions;

public class ChatUI : MonoBehaviour
{
    public TMP_InputField inputField;         // 输入框（TMP）
    public Button sendButton;                 // 发送按钮
    public TextMeshProUGUI chatHistoryText;   // 聊天记录显示（TMP）

    private const string apiUrl = "http://localhost:11434/api/generate";
    private const string modelName = "deepseek-r1:32b";

    void Start()
    {
        sendButton.onClick.AddListener(OnSendClicked);
    }

    void OnSendClicked()
    {
        string userInput = inputField.text;
        if (!string.IsNullOrEmpty(userInput))
        {
            chatHistoryText.text += "\n🧑 你：" + userInput;
            inputField.text = "";
            StartCoroutine(CallLocalAI(userInput));
        }
    }

    IEnumerator CallLocalAI(string message)
    {
        string requestData = "{\"model\":\"" + modelName + "\",\"prompt\":\"" + message + "\",\"stream\":false}";

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseJson = request.downloadHandler.text;
            Match match = Regex.Match(responseJson, "\"response\":\"(.*?)\"", RegexOptions.Singleline);
            string reply = match.Success ? match.Groups[1].Value : "（未找到回复）";
            reply = Regex.Unescape(reply);
            reply = reply.Replace("<think>", "").Replace("</think>", "").Trim();
            chatHistoryText.text += "\n🤖 AI：" + reply;
        }
        else
        {
            chatHistoryText.text += "\n❌ 请求失败：" + request.error;
        }
    }
}
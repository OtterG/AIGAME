using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text.RegularExpressions;

public class ChatUI : MonoBehaviour
{
    public InputField inputField;
    public Button sendButton;
    public Text sendButtonText;
    public Text chatHistoryText;
    public ScrollRect chatScroll; // 🆕 滚动区域

    private const string apiUrl = "http://localhost:11434/api/generate";
    private const string modelName = "deepseek-r1:32b";

    void Start()
    {
     

        if (sendButton != null)
            sendButton.onClick.AddListener(OnSendClicked);
        else
            Debug.LogError("❌ Send Button 未挂载！");

        if (inputField != null)
            inputField.onEndEdit.AddListener(HandleEnterKey); // 🆕 回车监听

        if (sendButtonText != null)
            sendButtonText.text = "发送";
    }

    void HandleEnterKey(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnSendClicked();
        }
    }

    void OnSendClicked()
    {
        string userInput = inputField.text;
        if (!string.IsNullOrEmpty(userInput))
        {
            chatHistoryText.text += "\n🧑 你：" + userInput;
            inputField.text = "";
            StartCoroutine(CallLocalAI(userInput));
            ScrollToBottom(); // 🆕 发送后滚动
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

        ScrollToBottom(); // 🆕 回复后滚动
    }

    void ScrollToBottom()
    {
        if (chatScroll != null)
        {
            Canvas.ForceUpdateCanvases();
            chatScroll.verticalNormalizedPosition = 0f;
        }
    }
}

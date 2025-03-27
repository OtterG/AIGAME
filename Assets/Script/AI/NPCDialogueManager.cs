using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Text.RegularExpressions;

public class NPCDialogueManager : MonoBehaviour
{
    public NPCProfile profile;
    private DialogueMemory memory = new DialogueMemory();

    private string apiUrl = "http://localhost:11434/api/generate";
    public string model = "deepseek-r1:32b"; // 根据你实际运行的模型名修改

    public IEnumerator SendMessageToAI(string prompt, System.Action<string> onReply, bool extractFinalQuote = true)
    {
        // ✅ 构造本地请求体（使用 prompt 字段）
        OllamaRequest requestData = new OllamaRequest
        {
            model = model,
            prompt = prompt,
            stream = false
        };

        string jsonBody = JsonUtility.ToJson(requestData);

        Debug.Log("📤 发送请求体:\n" + jsonBody);

        var request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            Debug.Log("📩 原始AI响应:\n" + response);

            string reply = "";

            try
            {
                var responseJson = JsonUtility.FromJson<OllamaResponse>(response);
                reply = responseJson.response.Trim();
            }
            catch
            {
                Debug.LogWarning("⚠️ 无法解析 AI 回复 JSON：\n" + response);
                reply = "[无回复]";
            }

            if (extractFinalQuote)
            {
                Match finalQuote = Regex.Match(reply, "“([^”]{5,})”");
                if (finalQuote.Success)
                {
                    reply = finalQuote.Groups[1].Value;
                }
                else
                {
                    Match fallback = Regex.Match(reply, "\"([^\"]{5,})\"");
                    if (fallback.Success)
                        reply = fallback.Groups[1].Value;
                }
            }

            Debug.Log("🧠 AI 回复内容:\n" + reply);
            onReply?.Invoke(reply);
        }
        else
        {
            Debug.LogError("❌ 请求失败: " + request.error);
            onReply?.Invoke("对不起，我现在无法回应……");
        }
    }

    [System.Serializable]
    public class OllamaRequest
    {
        public string model;
        public string prompt;
        public bool stream;
    }

    [System.Serializable]
    public class OllamaResponse
    {
        public string response;
    }
}

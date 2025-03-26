using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text.RegularExpressions; // ⬅️ 加到文件最上面（如果还没有）

public class LocalOllamaChat : MonoBehaviour
{
    public void SendMessageToLocalAI(string message)
    {
        Debug.Log("🧑 我说：" + message);
        StartCoroutine(CallLocalOllama(message));
    }

    IEnumerator CallLocalOllama(string message)
    {
        string apiUrl = "http://localhost:11434/api/generate";

        string requestData = "{\"model\":\"deepseek-r1:32b\",\"prompt\":\"" + message + "\",\"stream\":false}";


        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseJson = request.downloadHandler.text;

            // 用正则表达式提取 response 字段的完整内容
            Match match = Regex.Match(responseJson, "\"response\":\"(.*?)\"", RegexOptions.Singleline);

            string reply = match.Success ? match.Groups[1].Value : "（未找到 response 字段）";

            // 解码 Unicode 转义字符，例如 \u003c → <
            reply = Regex.Unescape(reply);
            reply = reply.Replace("<think>", "").Replace("</think>", "").Trim();
            Debug.Log("💬 AI 回复内容：" + reply);
        }
        else
        {
            Debug.LogError("请求失败：" + request.error);
        }
    }
}

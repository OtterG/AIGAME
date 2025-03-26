// ✅ NPCDialogueManager.cs（适配本地 Ollama generate 接口 + 修复匿名类序列化）
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

    public IEnumerator SendMessageToAI(string playerInput, System.Action<string> onReply)
    {
        memory.Add("user", playerInput);

        string fullPrompt = PromptBuilder.BuildRawPrompt(profile, memory, playerInput);

        // ✅ 使用可序列化类替代匿名对象
        OllamaRequest requestData = new OllamaRequest
        {
            model = model,
            prompt = fullPrompt,
            stream = false
        };

        string jsonBody = JsonUtility.ToJson(requestData);
        Debug.Log("玩家接触NPC:\n");

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseJson = request.downloadHandler.text;

            Match match = Regex.Match(responseJson, "\"response\":\"(.*?)\"", RegexOptions.Singleline);
            string reply = match.Success ? match.Groups[1].Value : "（未找到 response 字段）";
            reply = Regex.Unescape(reply)
            .Replace("<think>", "")
            .Replace("</think>", "")
            .Trim();

            // ✅ 正则提取最后一句引号内内容（中文优先）
            Match finalQuote = Regex.Match(reply, "“([^”]{5,})”"); // 中文书名号
            if (finalQuote.Success)
            {
                reply = finalQuote.Groups[1].Value;
            }
            else
            {
                // 如果没用中文书名号，就退而求其次提取普通引号内的句子
                Match fallback = Regex.Match(reply, "\"([^\"]{5,})\"");
                if (fallback.Success)
                    reply = fallback.Groups[1].Value;
            }


            memory.Add("assistant", reply);
            onReply?.Invoke(reply);
        }
        else
        {
            Debug.LogError("🛑 Ollama DeepSeek 请求失败：" + request.error);
            onReply?.Invoke("我好像思考卡住了……过会儿再来找我吧！");
        }
    }
}

// ✅ Ollama 请求数据结构（用于 JsonUtility.ToJson）
[System.Serializable]
public class OllamaRequest
{
    public string model;
    public string prompt;
    public bool stream;
}
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Unity.VisualScripting.FullSerializer;

public class NPCDialogueManager : MonoBehaviour
{
    public NPCProfile profile;
    public string apiKey;

    private DialogueMemory memory = new DialogueMemory();
    private string apiUrl = "https://api.deepseek.com/chat/completions";

    public IEnumerator SendMessageToAI(string playerInput, System.Action<string> onReply)
    {
        memory.Add("user", playerInput);
        string jsonBody = "{\"model\":\"deepseek-chat\",\"messages\":" + PromptBuilder.Build(profile, memory, playerInput) + ",\"max_tokens\":100}";

        var request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            string reply = JsonParser.ExtractContent(json);
            memory.Add("assistant", reply);
            onReply?.Invoke(reply);
        }
        else
        {
            Debug.LogError("AI请求失败: " + request.error);
            onReply?.Invoke("对不起，我现在无法回应……");
        }
    }
}

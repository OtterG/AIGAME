using UnityEngine;

public class PromptBuilder
{
    public static string Build(NPCProfile profile, DialogueMemory memory, string userInput)
    {
        string systemPrompt = $"你是一个NPC角色，名叫{profile.npcName}，背景如下：{profile.background}。你性格{profile.personality}，说话风格是{profile.speechStyle}。请代入角色自然说话。";

        string messagesJson = "[" +
            $"{{\"role\":\"system\",\"content\":\"{systemPrompt}\"}}," +
            memory.GetFormattedHistory() + "," +
            $"{{\"role\":\"user\",\"content\":\"{userInput}\"}}]";

        return messagesJson;
    }
}

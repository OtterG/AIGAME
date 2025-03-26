public class PromptBuilder
{
    public static string BuildRawPrompt(NPCProfile profile, DialogueMemory memory, string userInput)
    {
        string intro = $"你是小镇中的一位 NPC，名叫 {profile.npcName}。\n背景是：{profile.background}。\n性格是：{profile.personality}。\n说话风格是：{profile.speechStyle}。\n请代入角色自然地说一句话，不要解释，不要提到扮演，只说内容。";

        // 👇 最后一段引导模型“开口说一句”
        string final = $"\n\n现在你看到有一位玩家路过你，请你说一句话：";

        return intro + final;
    }
}

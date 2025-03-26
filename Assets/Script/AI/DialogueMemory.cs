using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueMemory
{
    private Queue<string> history = new Queue<string>();
    private int maxMemory = 6; // 记录最近3轮对话

    public void Add(string role, string content)
    {
        if (history.Count >= maxMemory)
            history.Dequeue();

        history.Enqueue($"{{\"role\":\"{role}\",\"content\":\"{content}\"}}");
    }

    public string GetFormattedHistory()
    {
        return string.Join(",", history);
    }

    public void Clear() => history.Clear();

    // ✅ 新增：生成纯文本形式的对话历史（用于 prompt 拼接）
    public string GetPlainTextHistory()
    {
        var lines = history.Select(entry =>
        {
            RoleEntry parsed = JsonUtility.FromJson<RoleEntry>(entry);
            return parsed.role == "user" ? $"玩家：{parsed.content}" : $"你：{parsed.content}";
        });

        return string.Join("\n", lines);
    }

    [System.Serializable]
    private class RoleEntry
    {
        public string role;
        public string content;
    }
}
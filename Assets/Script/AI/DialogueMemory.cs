using System.Collections.Generic;

public class DialogueMemory
{
    private Queue<string> history = new Queue<string>();
    private int maxMemory = 6; // 三轮对话

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
}

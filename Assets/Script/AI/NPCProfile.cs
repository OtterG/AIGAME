using UnityEngine;

[CreateAssetMenu(fileName = "NPCProfile", menuName = "AI/NPC Profile")]
public class NPCProfile : ScriptableObject
{
    public string npcName;
    [TextArea] public string background;
    [TextArea] public string personality;
    public string speechStyle; // 说话风格：幽默、冷静、热情等
}

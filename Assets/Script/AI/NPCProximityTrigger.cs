using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NPCProximityTrigger : MonoBehaviour
{
    public string triggerMessage = "玩家靠近触发";
    public NPCDialogueManager dialogueManager;
    public Transform player;
    public float cooldownTime = 5f;

    private bool hasSpoken = false;
    private float timer = 0f;

    void Update()
    {
        if (hasSpoken)
        {
            timer += Time.deltaTime;
            if (timer >= cooldownTime)
            {
                hasSpoken = false;
                timer = 0f;
            }
        }

        if (!hasSpoken && Vector3.Distance(transform.position, player.position) < 3f)
        {
            hasSpoken = true;
            StartCoroutine(dialogueManager.SendMessageToAI(triggerMessage, OnReply));
        }
    }

    void OnReply(string reply)
    {
        Debug.Log("NPC说：" + reply);
        // TODO：展示到UI 或气泡文字
    }
}

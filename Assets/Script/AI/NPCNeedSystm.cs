using UnityEngine;
using UnityEngine.AI;
using System.Text.RegularExpressions;
using System.Collections;

public class NPCNeedSystem : MonoBehaviour
{
    [Header("系统组件")]
    public NPCDialogueManager dialogueManager;

    [Header("行为目标点")]
    public Transform bed;
    public Transform kitchen;
    public Transform diningTable;

    private NavMeshAgent agent;

    [Header("属性")]
    public float hunger = 100f;
    public float energy = 100f;

    [Header("状态")]
    private bool isSleeping = false;
    private bool isCooking = false;
    private bool isEating = false;
    private bool decisionPending = false;

    [Header("调节参数")]
    public float hungerDecayRate = 1f;
    public float energyDecayRate = 1f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        //if (!isSleeping && !isCooking && !isEating)
        {
            hunger -= hungerDecayRate * Time.deltaTime;
            energy -= energyDecayRate * Time.deltaTime;
        }

        hunger = Mathf.Clamp(hunger, 0f, 100f);
        energy = Mathf.Clamp(energy, 0f, 100f);

        DecideNextAction();
    }

    private void DecideNextAction()
    {
        if (decisionPending || isSleeping || isCooking || isEating) return;

        if (hunger <= 30f || energy <= 30f)
        {
            string prompt =
                "你是一个NPC行为调度模块，接收身体参数后判断下一步行为。\n" +
                "返回格式： {\"action\": \"睡觉\"} 或 {\"action\": \"做饭\"}\n" +
                $"体力值: {energy:F0}, 饥饿值: {hunger:F0}\n" +
                "请选择最适合的行为，禁止加入解释、角色台词、标点或多余内容。只回复 JSON 格式结果。";

            decisionPending = true;
            StartCoroutine(dialogueManager.SendMessageToAI(prompt, OnDecisionReply, false));
        }
    }

    private void OnDecisionReply(string reply)
    {
        decisionPending = false;

        Match match = Regex.Match(reply, "\"action\"\\s*:\\s*\"(.*?)\"");
        if (!match.Success)
        {
            Debug.Log("🤔 AI 未能给出结构化决策：" + reply);
            return;
        }

        string action = match.Groups[1].Value.Trim();

        if (action == "睡觉")
        {
            StartSleeping();
        }
        else if (action == "做饭")
        {
            StartCooking();
        }
        else
        {
            Debug.Log("🤔 AI 返回了未知行为：" + action);
        }
    }

    // ✅ 睡觉
    private void StartSleeping()
    {
        Debug.Log("😴 NPC决定去睡觉...");
        isSleeping = true;
        if (bed != null && agent != null)
        {
            agent.SetDestination(bed.position);
            StartCoroutine(WaitUntilArrival(() =>
            {
                Debug.Log("💤 NPC到达床上，开始睡觉！");
                StartCoroutine(SleepRoutine());
            }));
        }
    }

    private IEnumerator SleepRoutine()
    {
        yield return new WaitForSeconds(15f); // 睡觉时间
        isSleeping = false;
        energy = 100f;
        Debug.Log("🛌 NPC睡醒了，精神满满！");
    }

    // ✅ 做饭
    private void StartCooking()
    {
        Debug.Log("🍳 NPC决定去做饭...");
        isCooking = true;
        if (kitchen != null && agent != null)
        {
            agent.SetDestination(kitchen.position);
            StartCoroutine(WaitUntilArrival(() =>
            {
                Debug.Log("👨‍🍳 NPC到达厨房，开始做饭！");
                StartCoroutine(CookingRoutine());
            }));
        }
    }

    private IEnumerator CookingRoutine()
    {
        yield return new WaitForSeconds(10f); // 做饭时间
        isCooking = false;
        StartEating();
    }

    // ✅ 吃饭
    private void StartEating()
    {
        Debug.Log("🍽️ NPC准备去吃饭...");
        isEating = true;
        if (diningTable != null && agent != null)
        {
            agent.SetDestination(diningTable.position);
            StartCoroutine(WaitUntilArrival(() =>
            {
                Debug.Log("😋 NPC坐下吃饭啦～");
                StartCoroutine(EatingRoutine());
            }));
        }
    }

    private IEnumerator EatingRoutine()
    {
        yield return new WaitForSeconds(5f); // 吃饭时间
        isEating = false;
        hunger = 100f;
        Debug.Log("🫄 NPC吃饱了！");
    }

    // ✅ 等待抵达目标再执行行为
    private IEnumerator WaitUntilArrival(System.Action onArrived)
    {
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            yield return null;

        onArrived?.Invoke();
    }
}

using UnityEngine;
using UnityEngine.UI;

public class NPCStatusUI : MonoBehaviour
{
    public NPCNeedSystem npc;
    public Text statusText;
    public Vector3 offset = new Vector3(0, 2.2f, 0); // 头顶偏移

    private Transform mainCam;

    void Start()
    {
        mainCam = Camera.main.transform;
    }

    void Update()
    {
        if (npc != null && statusText != null)
        {
            statusText.text = $"🍖 {npc.hunger:F0}  ⚡{npc.energy:F0}";
            transform.position = npc.transform.position + offset;
            transform.LookAt(transform.position + mainCam.forward); // 面向摄像机
        }
    }
}

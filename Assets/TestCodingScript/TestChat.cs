using UnityEngine;

public class TestLocalAI : MonoBehaviour
{
    public LocalOllamaChat localAI;

    void Start()
    {
        localAI.SendMessageToLocalAI("你好，DeepSeek！");
    }
}

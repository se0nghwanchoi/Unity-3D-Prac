using UnityEngine;

public class NPCActivator : MonoBehaviour
{
    public GameObject npcObject; // NPC 스크립트가 들어있는 게임 오브젝트

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            npcObject.SetActive(true); // NPC 스크립트가 들어있는 게임 오브젝트를 활성화
        }
    }
}

using UnityEngine;

public class NPCActivator : MonoBehaviour
{
    public GameObject npcObject; // NPC ��ũ��Ʈ�� ����ִ� ���� ������Ʈ

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            npcObject.SetActive(true); // NPC ��ũ��Ʈ�� ����ִ� ���� ������Ʈ�� Ȱ��ȭ
        }
    }
}

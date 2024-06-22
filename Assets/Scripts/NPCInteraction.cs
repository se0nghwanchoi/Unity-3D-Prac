using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject initialPrefab; // �ʱ⿡ Ȱ��ȭ�� ������
    public GameObject alternatePrefab; // E Ű�� ������ �� Ȱ��ȭ�� ������
    public GameObject canvasPanel; // ������ �г��� ����Ű�� ����

    private Coroutine panelDisableCoroutine; // �г� ��Ȱ��ȭ�� ���� �ڷ�ƾ ���� ����

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Press 'E' to interact with NPC");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Goodbye!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TogglePrefab(); // 'E' Ű�� ������ �������� ��ü�ϰ� �г��� ����մϴ�.
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // 'R' Ű�� ������ �г��� ��Ȱ��ȭ�մϴ�.
            canvasPanel.SetActive(false);
        }
    }

    void TogglePrefab()
    {
        // �г��� ����մϴ�.
        canvasPanel.SetActive(true);

     

        // �������� ��ü�մϴ�.
        if (initialPrefab.activeSelf)
        {
            initialPrefab.SetActive(false); // �ʱ� ������ ��Ȱ��ȭ
            alternatePrefab.SetActive(true); // ��ü ������ Ȱ��ȭ
        }
        else
        {
            initialPrefab.SetActive(true); // �ʱ� ������ Ȱ��ȭ
            alternatePrefab.SetActive(false); // ��ü ������ ��Ȱ��ȭ
        }
    }

    


}

using UnityEngine;

public class NPCInteraction2 : MonoBehaviour
{

    public GameObject canvasPanel; // ������ �г��� ����Ű�� ����

    

    void Update()
    {
       

        if (Input.GetKeyDown(KeyCode.R))
        {
            // 'R' Ű�� ������ �г��� ��Ȱ��ȭ�մϴ�.
            canvasPanel.SetActive(false);
        }
    }

    




}

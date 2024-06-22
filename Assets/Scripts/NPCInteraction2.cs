using UnityEngine;

public class NPCInteraction2 : MonoBehaviour
{

    public GameObject canvasPanel; // 연결할 패널을 가리키는 변수

    

    void Update()
    {
       

        if (Input.GetKeyDown(KeyCode.R))
        {
            // 'R' 키를 누르면 패널을 비활성화합니다.
            canvasPanel.SetActive(false);
        }
    }

    




}

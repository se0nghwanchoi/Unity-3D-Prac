using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject initialPrefab; // 초기에 활성화될 프리팹
    public GameObject alternatePrefab; // E 키를 눌렀을 때 활성화될 프리팹
    public GameObject canvasPanel; // 연결할 패널을 가리키는 변수

    private Coroutine panelDisableCoroutine; // 패널 비활성화를 위한 코루틴 참조 변수

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
            TogglePrefab(); // 'E' 키를 누르면 프리팹을 교체하고 패널을 토글합니다.
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // 'R' 키를 누르면 패널을 비활성화합니다.
            canvasPanel.SetActive(false);
        }
    }

    void TogglePrefab()
    {
        // 패널을 토글합니다.
        canvasPanel.SetActive(true);

     

        // 프리팹을 교체합니다.
        if (initialPrefab.activeSelf)
        {
            initialPrefab.SetActive(false); // 초기 프리팹 비활성화
            alternatePrefab.SetActive(true); // 대체 프리팹 활성화
        }
        else
        {
            initialPrefab.SetActive(true); // 초기 프리팹 활성화
            alternatePrefab.SetActive(false); // 대체 프리팹 비활성화
        }
    }

    


}

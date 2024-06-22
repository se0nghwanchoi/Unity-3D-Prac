using UnityEngine;

public class UpdateWeapon : MonoBehaviour
{
    bool isGathering = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleObjects();
           
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ToggleDSword();
        }
    }

    void ToggleObjects()
    {
        // Swordon ��ü�� ��Ȱ��ȭ�ϰ� Axeon ��ü�� Ȱ��ȭ�մϴ�.
        GameObject swordonObject = gameObject.transform.Find("Swordon").gameObject;
        GameObject axeonObject = gameObject.transform.Find("Axeon").gameObject;

        if (swordonObject != null)
            swordonObject.SetActive(false);

        if (axeonObject != null)
            axeonObject.SetActive(true);

        
    }

    void ToggleDSword()
    {
        // Swordon�� Axeon ��ü�� ��Ȱ��ȭ�ϰ� DSword ��ü�� Ȱ��ȭ�մϴ�.
        GameObject swordonObject = gameObject.transform.Find("Swordon").gameObject;
        GameObject axeonObject = gameObject.transform.Find("Axeon").gameObject;
        GameObject dswordObject = gameObject.transform.Find("DSwordon").gameObject;

        if (swordonObject != null)
            swordonObject.SetActive(false);

        if (axeonObject != null)
            axeonObject.SetActive(false);

        if (dswordObject != null)
            dswordObject.SetActive(true);
    }
}

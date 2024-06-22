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
        // Swordon 객체를 비활성화하고 Axeon 객체를 활성화합니다.
        GameObject swordonObject = gameObject.transform.Find("Swordon").gameObject;
        GameObject axeonObject = gameObject.transform.Find("Axeon").gameObject;

        if (swordonObject != null)
            swordonObject.SetActive(false);

        if (axeonObject != null)
            axeonObject.SetActive(true);

        
    }

    void ToggleDSword()
    {
        // Swordon과 Axeon 객체를 비활성화하고 DSword 객체를 활성화합니다.
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

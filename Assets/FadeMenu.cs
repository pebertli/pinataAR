using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMenu : MonoBehaviour
{

    public void ActivateEntryMenu(int activate)
    {
        this.gameObject.SetActive((activate == 0) ? false : true);
    }
}

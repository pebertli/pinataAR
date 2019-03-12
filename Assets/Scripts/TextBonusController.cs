using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBonusController : MonoBehaviour {

    public void EndAnimation()
    {
        Destroy(transform.parent.gameObject);
    }
}

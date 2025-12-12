using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GlobalVolume : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ReativarAposTempo());
    }

    // Corrotina para reativar o objeto após um determinado tempo
    IEnumerator ReativarAposTempo()
    {
        Volume vol = GetComponent<Volume>();
        yield return new WaitForSeconds(.1f);
        vol.enabled = false;
        yield return new WaitForSeconds(.1f);
        vol.enabled = true;
    }
}

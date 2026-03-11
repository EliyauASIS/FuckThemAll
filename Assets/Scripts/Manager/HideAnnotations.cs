using UnityEngine;
using UnityEngine.UI;

public class HideAnnotations : MonoBehaviour
{
    private void LateUpdate()
    {
        foreach (var lr in GetComponentsInChildren<LineRenderer>(true))
        {
            lr.startColor = Color.clear;
            lr.endColor = Color.clear;
        }

        foreach (var mr in GetComponentsInChildren<MeshRenderer>(true))
        {
            foreach (var mat in mr.materials)
                mat.color = Color.clear;
        }

        foreach (var img in GetComponentsInChildren<Image>(true))
            img.color = Color.clear;

        foreach (var tmp in GetComponentsInChildren<TMPro.TMP_Text>(true))
            tmp.color = Color.clear;
    }
}
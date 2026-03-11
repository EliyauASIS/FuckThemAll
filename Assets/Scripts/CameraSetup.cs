using UnityEngine;
using TrackerPro.Unity;
using TrackerPro.Settings;

public class CameraSetup : MonoBehaviour
{
    [SerializeField] private SettingsManager settingsManager;

    private void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        StartCoroutine(WaitAndSelect());
#endif
    }

    private System.Collections.IEnumerator WaitAndSelect()
    {
        // ממתין ש-ImageSource יהיה מוכן
        yield return new WaitUntil(() => ImageSourceProvider.ImageSource != null);
        yield return new WaitForSeconds(0.5f);

        var imageSource = ImageSourceProvider.ImageSource;
        var devices = WebCamTexture.devices;

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing)
            {
                Debug.Log($"[CameraSetup] מצלמה קדימית: {devices[i].name} (index {i})");
                imageSource.SelectSource(i);
                StartCoroutine(settingsManager.RestartSolution(true));
                yield break;
            }
        }

        Debug.LogWarning("[CameraSetup] לא נמצאה מצלמה קדימית");
    }
}
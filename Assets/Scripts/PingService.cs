using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PingService : MonoBehaviour
{
    public static PingService Instance { get; private set; }

    public static bool IsBackendReady { get; private set; } = false;

    private void Awake()
    {
        Instance = this;
        StartCoroutine(KeepAliveRoutine());
    }

    IEnumerator KeepAliveRoutine()
    {
        while (true)
        {
            yield return WaitForBackendReady();
            yield return new WaitForSeconds(5 * 60); // 5 minutes
        }
    }

    IEnumerator WaitForBackendReady()
    {
        while (true)
        {
            using (UnityWebRequest req = UnityWebRequest.Get(AppConstants.PingUrl))
            {
                req.timeout = 5;
                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Backend is ready");
                    IsBackendReady = true;
                    yield break;
                }
                IsBackendReady = false;
                Debug.LogError(req.error);
            }

            Debug.Log("Waiting for backend...");
            yield return new WaitForSeconds(2f);
        }
    }
}

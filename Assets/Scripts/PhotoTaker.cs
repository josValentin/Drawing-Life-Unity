using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PhotoTaker : MonoBehaviour
{
    [SerializeField] private Button btnTakePhoto, btnTryAgain, btnAccept, btnCancel;
    [SerializeField] private GameObject onPhotoTakenGo, onTakingPhotoGo;
    [SerializeField] private RawImage camRenderer;

    [field: SerializeField] public UnityEvent<Texture2D> onPhotoSelected;
    [field: SerializeField] public UnityEvent onCancel;

    private WebCamTexture webCamTex;

    private Texture2D photoTaken;
    [SerializeField] private AspectRatioFitter fitter;
    private void Awake()
    {
        btnTakePhoto.onClick.AddListener(TakePhoto);
        btnAccept.onClick.AddListener(Accept);
        btnCancel.onClick.AddListener(Cancel);
        btnTryAgain.onClick.AddListener(TryAgain);
    }

    private void OnEnable()
    {
        onTakingPhotoGo.SetActive(true);
        onPhotoTakenGo.SetActive(false);
    }

    public void StartCamera()
    {
        gameObject.SetActive(true);
        StartCoroutine(IStartCamera());
    }

    private IEnumerator IStartCamera()
    {
        if (webCamTex == null)
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            }

            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                Debug.LogError("Permiso de cámara DENEGADO");
                yield break;
            }

            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => WebCamTexture.devices.Length > 0);

            foreach (var device in WebCamTexture.devices)
            {
                if (!device.isFrontFacing)
                {
                    webCamTex = new WebCamTexture(device.name, 2560, 1440, 144);
                    break;
                }
            }

#elif UNITY_STANDALONE || UNITY_EDITOR

            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => WebCamTexture.devices.Length > 0);

            foreach (var device in WebCamTexture.devices)
            {
                if (!device.isFrontFacing)
                {
                    webCamTex = new WebCamTexture(device.name, 4096, 4096, 144);
                    break;
                }
            }

            if (webCamTex == null)
            {
                Debug.LogWarning("No forward camera found, using any default");
                webCamTex = new WebCamTexture(4096, 4096, 144);
            }
#endif
            // Start the camera
            camRenderer.texture = webCamTex;
        }

        webCamTex.Play();
    }

    void Update()
    {
        if (webCamTex == null || !webCamTex.isPlaying)
            return;

        float ratio = (float)webCamTex.width / (float)webCamTex.height;
        fitter.aspectRatio = ratio;

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR

        float scaleY = webCamTex.videoVerticallyMirrored ? -1f : 1f;
        camRenderer.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -webCamTex.videoRotationAngle;
        camRenderer.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
#endif

        // 

        //int rotation = webCamTex.videoRotationAngle;

        //// Dimensiones reales del frame según rotación
        //float camW = (rotation == 90 || rotation == 270)
        //    ? webCamTex.height
        //    : webCamTex.width;

        //float camH = (rotation == 90 || rotation == 270)
        //    ? webCamTex.width
        //    : webCamTex.height;

        //float camAspect = camW / camH;
        //float screenAspect = (float)Screen.width / Screen.height;

        //RectTransform rt = camRenderer.rectTransform;

        //// RawImage ya está stretch, ahora escalamos
        //if (camAspect > screenAspect)
        //{
        //    // Cámara más ancha escalar por alto
        //    float scale = camAspect / screenAspect;
        //    rt.localScale = new Vector3(scale, scale, 1);
        //}
        //else
        //{
        //    // Cámara más alta escalar por ancho
        //    float scale = screenAspect / camAspect;
        //    rt.localScale = new Vector3(scale, scale, 1);
        //}

        //#if UNITY_ANDROID || UNITY_IOS
        //        // Rotación correcta
        //        rt.localEulerAngles = new Vector3(0, 0, -rotation);

        //        // Mirror correcto (vertical)
        //        camRenderer.uvRect = webCamTex.videoVerticallyMirrored
        //            ? new Rect(0, 1, 1, -1)
        //            : new Rect(0, 0, 1, 1);
        //#endif
    }

    private void Accept()
    {
        StopCamera();
        onPhotoSelected.Invoke(photoTaken);
        gameObject.SetActive(false);
    }

    private void Cancel()
    {
        if (webCamTex != null)
            StopCamera();
        onCancel?.Invoke();
        gameObject.SetActive(false);
    }

    private void StopCamera()
    {
        webCamTex.Stop();
    }

    private void TryAgain()
    {
        webCamTex.Play();


        onTakingPhotoGo.SetActive(true);
        onPhotoTakenGo.SetActive(false);
    }

    private void TakePhoto()
    {
        webCamTex.Pause();
        photoTaken = new(
        webCamTex.width,
        webCamTex.height,
        TextureFormat.RGB24,
        false
        );

        photoTaken.SetPixels(webCamTex.GetPixels());
        photoTaken.Apply();

        onTakingPhotoGo.SetActive(false);
        onPhotoTakenGo.SetActive(true);
    }

    public byte[] GetPhotoBytes()
    {
        byte[] jpg = photoTaken.EncodeToJPG();
        return jpg;
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }

    [SerializeField] private PhotoTaker photoTaker;
    [SerializeField] private PictureProcessor processor;

    [SerializeField] private SLDictionary<string, AnimalObject> animalPrefabs;
    [SerializeField] private Button btnOpenCamera;
    [SerializeField] private Button btnExit;

    [SerializeField] private LoadingPanel mainLoadingPanel;
    [SerializeField] private LoadingPanel processingLoadingPanel;
    [SerializeField] private PopupWindow errorPopup;

    private List<AnimalObject> animals = new List<AnimalObject>();

    private void Awake()
    {
        Instance = this;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 144;

        btnOpenCamera.onClick.AddListener(OpenCamera);

#if UNITY_STANDALONE
        btnExit.gameObject.SetActive(true);
        btnExit.onClick.AddListener(Application.Quit);
#elif UNITY_ANDROID || UNITY_IOS
        btnExit.gameObject.SetActive(false);
#endif

        photoTaker.onPhotoSelected.AddListener(OnPhotoEncoded);
        photoTaker.onCancel.AddListener(OnPhotoCancel);

        photoTaker.gameObject.SetActive(false);

        mainLoadingPanel.Show();

        this.ActionWaitUntilCondition(() => PingService.IsBackendReady, () => mainLoadingPanel.Hide());
    }

    public void OpenCamera()
    {
        photoTaker.StartCamera();
    }

    private async void OnPhotoEncoded(Texture2D photo)
    {
        processingLoadingPanel.Show(true);
        var result = await processor.ProcessImage(photo);
        processingLoadingPanel.Hide(true);

        if (result.error != null)
        {
            errorPopup.Show();
            return;
        }

        var animal = Instantiate(animalPrefabs[result.key]);
        animal.SetSkin(result.result);
        animals.Add(animal);
    }

    private void Update()
    {
        int sortingOrder = 0;
        foreach (var item in animals.OrderByDescending(a => a.transform.position.y))
        {
            item.GetChildComponent<SpriteRenderer>(0).sortingOrder = sortingOrder;
            sortingOrder++;
        }

    }

    private void OnPhotoCancel()
    {
        Debug.Log("Photo canceled");
    }
}

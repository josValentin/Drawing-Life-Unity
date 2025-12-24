using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PictureProcessor : MonoBehaviour
{
    [Header("Output")]
    public RawImage outputImage;

    //IEnumerator SendImage(Texture2D texture)
    //{
    //    byte[] imageBytes = texture.EncodeToPNG();

    //    WWWForm form = new WWWForm();
    //    form.AddBinaryData("image", imageBytes, "paper.png", "image/png");

    //    UnityWebRequest request = UnityWebRequest.Post(serverUrl, form);
    //    request.downloadHandler = new DownloadHandlerBuffer();

    //    yield return request.SendWebRequest();

    //    if (request.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.LogError("Error: " + request.error);
    //        yield break;
    //    }

    //    // Leer headers
    //    string animal = request.GetResponseHeader("X-Detected-Animal");
    //    string score = request.GetResponseHeader("X-Match-Score");

    //    Debug.Log($"Animal detectado: {animal} (score={score})");

    //    // Convertir respuesta a textura
    //    Texture2D result = new Texture2D(2, 2);
    //    result.LoadImage(request.downloadHandler.data);

    //    // Mostrar resultado
    //    outputImage.texture = result;
    //}

    public async Task<(Texture2D result, string key, string error)> ProcessImage(Texture2D texture)
    {
        byte[] imageBytes = texture.EncodeToPNG();

        WWWForm form = new();
        form.AddBinaryData("image", imageBytes, "paper.png", "image/png");

        //UnityWebRequest request = UnityWebRequest.Post(AppConstants.ProcessImageUrl, form);

        using (UnityWebRequest request = UnityWebRequest.Post(AppConstants.ProcessImageUrl, form))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = 30;
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                return (null, null, request.error);
            }

            // Leer headers
            string animal = request.GetResponseHeader("X-Detected-Animal");
            string score = request.GetResponseHeader("X-Match-Score");

            Debug.Log($"Animal detectado: {animal} (score={score})");

            // Convertir respuesta a textura
            Texture2D result = new Texture2D(2, 2);
            result.LoadImage(request.downloadHandler.data);

            // Mostrar resultado
            return (result, animal, null);
        }
    }
}

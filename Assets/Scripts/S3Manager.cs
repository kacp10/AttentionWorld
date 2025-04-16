using UnityEngine;
using UnityEngine.UI;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;
using System.Threading.Tasks;

public class S3Manager : MonoBehaviour
{
    private AmazonS3Client s3Client;

    void Start()
    {
        // Inicializar las credenciales de S3
        
        s3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.USEast2);

        Debug.Log("Cliente S3 inicializado");
    }

    // Método para cargar la imagen desde S3
    public async Task LoadImage(string bucketName, string s3Key, Image targetImage)
    {
        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = s3Key
        };

        try
        {
            var response = await s3Client.GetObjectAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                MemoryStream memoryStream = new MemoryStream();
                response.ResponseStream.CopyTo(memoryStream);
                byte[] data = memoryStream.ToArray();

                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(data);
                targetImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error al cargar la imagen de S3: " + ex.Message);
        }
    }
}

// S3Manager.cs
using UnityEngine;
using UnityEngine.UI;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.CognitoIdentity;
using System.IO;
using System.Threading.Tasks;

public class S3Manager : MonoBehaviour
{
    public static S3Manager Instance { get; private set; }

    // === AJUSTA SOLO ESTA CONSTANTE SI CAMBIA TU POOL ===
    private const string identityPoolId = "";

    private AmazonS3Client s3Client;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitS3Client();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitS3Client()
    {
        // Credenciales temporales vía Cognito
        var credentials = new CognitoAWSCredentials(identityPoolId, RegionEndpoint.USEast1);

        // El bucket está en us-east-2 (Ohio)
        var config = new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.USEast2,
            ForcePathStyle = true   // evita problemas de firma con “virtual host style”
        };

        s3Client = new AmazonS3Client(credentials, config);
        Debug.Log("✅ S3Manager listo (Cognito)");
    }

    /// <summary>Carga un .png de S3 en un <see cref="UnityEngine.UI.Image"/>.</summary>
    public async Task LoadImage(string bucket, string key, Image target)
    {
        try
        {
            var resp = await s3Client.GetObjectAsync(bucket, key);
            if (resp.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new System.Exception($"Estado HTTP {resp.HttpStatusCode}");

            using var ms = new MemoryStream();
            await resp.ResponseStream.CopyToAsync(ms);
            var tex = new Texture2D(2, 2);
            tex.LoadImage(ms.ToArray());

            target.sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f));
            target.color = Color.white;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ S3 [{bucket}/{key}] → {ex.Message}");
        }
    }
}

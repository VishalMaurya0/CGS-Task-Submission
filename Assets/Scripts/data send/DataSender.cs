using System;
using System.Collections;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class DataSender : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The Input Field where the player pastes their auth code")]
    public TMP_InputField loginCodeInputField;
    public TMP_Text scoreText;
    public Button sendButton;

    [Header("Visual Scripting Data")]
    [Tooltip("Type the EXACT name of your Saved Variable from Visual Scripting")]
    public string scoreVariableName = "Score";

    [Header("Server & Game Configuration")]
    public string baseUrl = "https://www.cgsiitkgp.org/api";
    public string gameId = "Lava-Run";
    public string gameSecret = "hamaigayhu6769";

    private const string AUTH_CODE_PREF_KEY = "SavedAuthCode";

    private void Start()
    {
        if (sendButton != null)
        {
            sendButton.onClick.AddListener(OnSendButtonClicked);
        }

        // Load the saved code when the scene starts
        if (loginCodeInputField != null && PlayerPrefs.HasKey(AUTH_CODE_PREF_KEY))
        {
            loginCodeInputField.text = PlayerPrefs.GetString(AUTH_CODE_PREF_KEY);
        }
    }

    // --- NEW: This runs every frame to keep the timer text updated ---
    private void Update()
    {
        if (scoreText != null && Variables.Saved.IsDefined(scoreVariableName))
        {
            // Fetch the raw time from Visual Scripting and display it
            float rawTime = Variables.Saved.Get<float>(scoreVariableName);
            scoreText.text = rawTime.ToString("F3");
        }
    }

    public void OnSendButtonClicked()
    {
        string authCode = loginCodeInputField != null ? loginCodeInputField.text : "";

        if (string.IsNullOrEmpty(authCode))
        {
            Debug.LogWarning("Login code is empty! Please type the code before sending.");
            return;
        }

        sendButton.interactable = false; // Prevent spam clicking
        StartCoroutine(ProcessLoginAndSendScore(authCode));
    }

    private IEnumerator ProcessLoginAndSendScore(string authCode)
    {
        // ---------------------------------------------------------
        // 1. FETCH SCORE FROM VISUAL SCRIPTING & APPLY SPEEDRUN MATH
        // ---------------------------------------------------------
        int currentScore = 0;
        if (Variables.Saved.IsDefined(scoreVariableName))
        {
            int rawTimeScore = Variables.Saved.Get<int>(scoreVariableName);

            // Subtract from 10,000 for the speedrun calculation
            currentScore = 10000 - rawTimeScore;

            // Prevent the score from going into the negatives if they took too long
            currentScore = Mathf.Max(0, currentScore);
        }
        else
        {
            Debug.LogWarning($"Saved Variable '{scoreVariableName}' not found! Sending 0.");
        }

        Debug.Log($"[DataSender] Attempting to login with code and send Score: {currentScore}...");

        // ---------------------------------------------------------
        // 2. PHASE ONE: EXCHANGE AUTH CODE FOR TOKEN
        // ---------------------------------------------------------
        string exchangeEndpoint = $"{baseUrl}/game/session/exchange";
        string exchangeJson = JsonUtility.ToJson(new ExchangeRequest { gameAuthCode = authCode });

        string userId = "";
        string gameToken = "";

        using (UnityWebRequest authRequest = new UnityWebRequest(exchangeEndpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(exchangeJson);
            authRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            authRequest.downloadHandler = new DownloadHandlerBuffer();
            authRequest.SetRequestHeader("Content-Type", "application/json");

            yield return authRequest.SendWebRequest();

            if (authRequest.result == UnityWebRequest.Result.Success)
            {
                ExchangeResponse res = JsonUtility.FromJson<ExchangeResponse>(authRequest.downloadHandler.text);
                if (res != null && res.action && res.data != null)
                {
                    userId = res.data.userId;
                    gameToken = res.data.gameToken;
                    Debug.Log($"[DataSender] Login Successful! UserID: {userId}");

                    // Save the successfully verified code so it persists!
                    PlayerPrefs.SetString(AUTH_CODE_PREF_KEY, authCode);
                    PlayerPrefs.Save();
                }
                else
                {
                    Debug.LogError($"[DataSender] Login Rejected: {(res != null ? res.message : "No message")}");
                    sendButton.interactable = true;
                    yield break; // Stop execution
                }
            }
            else
            {
                Debug.LogError($"[DataSender] Network Error during login: {authRequest.error}");
                sendButton.interactable = true;
                yield break; // Stop execution
            }
        }

        // ---------------------------------------------------------
        // 3. PHASE TWO: SECURE AND SUBMIT THE SCORE
        // ---------------------------------------------------------
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        string matchSeed = System.Guid.NewGuid().ToString();

        // Generate SHA256 Signature
        string signaturePayload = $"{userId}:{currentScore}:{timestamp}:{gameSecret}";
        string signature = ComputeSha256(signaturePayload);

        ScoreRequest scorePayload = new ScoreRequest
        {
            gameId = this.gameId,
            score = currentScore,
            scoreStr = currentScore.ToString(),
            seed = matchSeed,
            timestamp = timestamp,
            gameToken = gameToken,
            signature = signature
        };

        string scoreEndpoint = $"{baseUrl}/game/score";
        string scoreJson = JsonUtility.ToJson(scorePayload);

        using (UnityWebRequest scoreRequest = new UnityWebRequest(scoreEndpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(scoreJson);
            scoreRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            scoreRequest.downloadHandler = new DownloadHandlerBuffer();
            scoreRequest.SetRequestHeader("Content-Type", "application/json");

            yield return scoreRequest.SendWebRequest();

            if (scoreRequest.result == UnityWebRequest.Result.Success)
            {
                BaseResponse res = JsonUtility.FromJson<BaseResponse>(scoreRequest.downloadHandler.text);

                if (res != null && res.action)
                {
                    Debug.Log($"[DataSender] ✅ Score of {currentScore} submitted successfully!");

                    // ---------------------------------------------------------
                    // 4. RELOAD SCENE ON SUCCESS
                    // ---------------------------------------------------------
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                else
                {
                    Debug.LogError($"[DataSender] Score upload rejected: {res?.message}");
                    sendButton.interactable = true;
                }
            }
            else
            {
                Debug.LogError($"[DataSender] Network error during score submission: {scoreRequest.error}");
                sendButton.interactable = true;
            }
        }
    }

    // Helper function to create the security hash
    private static string ComputeSha256(string rawData)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    #region Data Transfer Classes
    [Serializable] private class ExchangeRequest { public string gameAuthCode; }
    [Serializable] private class ExchangeResponse { public bool action; public ExchangeData data; public string message; }
    [Serializable] private class ExchangeData { public string userId; public string username; public string gameToken; }
    [Serializable] private class ScoreRequest { public string gameId; public int score; public string scoreStr; public string seed; public long timestamp; public string gameToken; public string signature; }
    [Serializable] private class BaseResponse { public bool action; public string message; public string[] errors; }
    #endregion
}
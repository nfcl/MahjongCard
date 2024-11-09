using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class LogEvent : MonoBehaviour
{
    public static LogEvent instance;

    public Color[] logTextColors;

    public TMP_Text logText;

    private void Awake()
    {
        if(instance != null)
        {
            DestroyImmediate(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(instance.gameObject);

        Application.logMessageReceived += HandleLog;
    }

    //private void Start()
    //{
    //    logText.text = "";
    //    Debug.Log("Test Log");
    //    Debug.LogWarning("Test Warning");
    //    Debug.LogException(new System.Exception("Test Exception"));
    //    Debug.LogError("Test Error");
    //    Debug.LogAssertion("Test Assertion");
    //}

    public void HandleLog(string condition, string stackTrace, LogType type)
    {
        string message = $"type = {type}\ncondition = {condition}\nstackTrace = {stackTrace.Trim()}\n";
        SendLog($"<color=#{logTextColors[(int)type].ToHexString()}>{message}</color>");
    }

    private void SendLog(string message)
    {
        logText.text += message;
    }
}
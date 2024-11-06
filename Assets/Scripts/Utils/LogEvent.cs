using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class LogEvent : MonoBehaviour
{
    public Color[] logTextColors;

    public TMP_Text logText;

    public static bool isExsist = false;

    private void Awake()
    {
        Application.logMessageReceived += HandleLog;
        if (isExsist)
        {
            DestroyImmediate(this.gameObject);
        }
        isExsist = true;
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

    void HandleLog(string condition, string stackTrace, LogType type)
    {
        string message = $"type = {type}\ncondition = {condition}\nstackTrace = {stackTrace.Trim()}\n";
        SendLog($"<color=#{logTextColors[(int)type].ToHexString()}>{message}</color>");
    }

    void SendLog(string message)
    {
        logText.text += message;
    }
}
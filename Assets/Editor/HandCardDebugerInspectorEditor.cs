using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HandCardDebuger))]
[CanEditMultipleObjects]
public class HandCardDebugerInspectorEditor : Editor
{
    //��дOnInspectorGUI��(ˢ��Inspector���)
    public override void OnInspectorGUI()
    {
        //�̳л��෽��
        base.OnInspectorGUI();

        //��ȡҪִ�з�������
        HandCardDebuger debugger = (HandCardDebuger)target;
        //����Button
        if (GUILayout.Button("��ʽ��"))
        {
            //ִ�з���
            debugger.FormatCards();
        }
    }
}
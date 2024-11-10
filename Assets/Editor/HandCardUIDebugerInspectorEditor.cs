using Card;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HandCardUIDebuger))]
[CanEditMultipleObjects]
public class HandCardUIDebugerInspectorEditor : Editor
{
    //��дOnInspectorGUI��(ˢ��Inspector���)
    public override void OnInspectorGUI()
    {
        //�̳л��෽��
        base.OnInspectorGUI();

        //��ȡҪִ�з�������
        HandCardUIDebuger debugger = (HandCardUIDebuger)target;
        //����Button
        if (GUILayout.Button("��ʽ��"))
        {
            //ִ�з���
            debugger.FormatCards();
        }
    }
}
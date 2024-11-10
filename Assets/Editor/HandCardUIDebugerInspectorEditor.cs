using Card;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HandCardUIDebuger))]
[CanEditMultipleObjects]
public class HandCardUIDebugerInspectorEditor : Editor
{
    //重写OnInspectorGUI类(刷新Inspector面板)
    public override void OnInspectorGUI()
    {
        //继承基类方法
        base.OnInspectorGUI();

        //获取要执行方法的类
        HandCardUIDebuger debugger = (HandCardUIDebuger)target;
        //绘制Button
        if (GUILayout.Button("格式化"))
        {
            //执行方法
            debugger.FormatCards();
        }
    }
}
using UnityEditor;
using UnityEngine;

public class AnchorsToCorners : MonoBehaviour
{
    //Fragment obtanied from public repository:
    //https://github.com/jesenzhang/unity-ui-extensions/blob/master/Editor/uGUITools.cs
    
    [MenuItem("Tools/UI/Anchors to Corners #c")]
    static void MoveAnchorsToCorners()
    {
        foreach (Transform transform in Selection.transforms)
        {
            RectTransform t = transform as RectTransform;
            if (t == null || t.parent == null) continue;

            Undo.RecordObject(t, "Anchors to Corners");

            RectTransform pt = t.parent as RectTransform;

            Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
                                                t.anchorMin.y + t.offsetMin.y / pt.rect.height);
            Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
                                                t.anchorMax.y + t.offsetMax.y / pt.rect.height);

            t.anchorMin = newAnchorsMin;
            t.anchorMax = newAnchorsMax;
            t.offsetMin = t.offsetMax = new Vector2(0, 0);
        }
    }
}

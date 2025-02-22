using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAnimationFromSprite : MonoBehaviour
{
    [MenuItem("Tools/Create Animation From Sprite")]
    static void CreateAnimations()
    {
        Object[] selectedObjects = Selection.objects;
        
        if(selectedObjects.Length == 0)
        {
            Debug.LogWarning("No sprites selected.");
            return;
        }
        
        var folderPath = "Assets/Art/DWM/Anims/Monsters";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "GeneratedAnimations");
        }

        var clip = new AnimationClip();
        clip.frameRate = 2;
        
        var spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";

        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[selectedObjects.Length];
        for (int i = 0; i < selectedObjects.Length; i++)
        {
            Sprite sprite = selectedObjects[i] as Sprite;
            keyFrames[i] = new ObjectReferenceKeyframe
            {
                time = i / clip.frameRate,
                value = sprite
            };
        }

        // 5. Set the curve
        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyFrames);

        // 6. Save the AnimationClip asset
        string clipPath = Path.Combine(folderPath, "MyAutoAnim.anim");
        AssetDatabase.CreateAsset(clip, clipPath);
        AssetDatabase.SaveAssets();

        Debug.Log("Animation Clip created at: " + clipPath);
    }

    [MenuItem("Tools/Create Four Direction Animations")]
    static void CreateFourDirectionAnimations()
    {
        Object[] selectedObjects = Selection.objects;
        
        if(selectedObjects.Length != 8)
        {
            Debug.LogWarning("8 sprites must be selected.");
            return;
        }
        
        var folderPath = "Assets/Art/DWM/Anims/Monsters/AutoAnimation";

        for (var i = 0; i < 8; i += 2)
        {
            for(var j = i; j < i + 2; j++)
            {
                var clip = new AnimationClip();
                clip.frameRate = 2;
                var spriteBinding = new EditorCurveBinding();
                spriteBinding.type = typeof(SpriteRenderer);
                spriteBinding.path = "";
                spriteBinding.propertyName = "m_Sprite";
                
                ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[2];
                for (var k = 0; k < 2; k++)
                {
                    Sprite sprite = selectedObjects[j] as Sprite;
                    keyFrames[k] = new ObjectReferenceKeyframe
                    {
                        time = k / clip.frameRate,
                        value = sprite
                    };
                }

                // 5. Set the curve
                AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyFrames);
                var animName = DetermineAnimationName(i);
                // 6. Save the AnimationClip asset
                string clipPath = Path.Combine(folderPath, $"{animName}.anim");
                AssetDatabase.CreateAsset(clip, clipPath);
                AssetDatabase.SaveAssets();

                Debug.Log($"Animation Clip - {animName} - created at: {clipPath}");
            }
        }
    }

    private static string DetermineAnimationName(int i)
    {
        switch (i)
        {
            case 0:
                return "Down";
            case 2:
                return "Left";
            case 4:
                return "Up";
            case 6:
                return "Right";
            default:
                return "Unknown";
        }
    }
}

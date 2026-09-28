#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

public class AutoAnimationCreator
{
    // Esto añade una opción al menú de clic derecho en tus Assets
    [MenuItem("Assets/Crear Animaciones desde Spritesheets")]
    public static void CreateAnimations()
    {
        // Recorre todos los archivos que hayas seleccionado en el Project
        foreach (Object obj in Selection.objects)
        {
            if (obj is Texture2D texture)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                
                // Carga todos los sub-sprites recortados dentro del PNG
                Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();

                if (sprites.Length > 0)
                {
                    AnimationClip clip = new AnimationClip();
                    clip.frameRate = 12; // Cambia esto si usas otros FPS para tu Pixel Art

                    // --- NUEVO: CONFIGURAR EL LOOP TIME A TRUE ---
                    AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
                    settings.loopTime = true;
                    AnimationUtility.SetAnimationClipSettings(clip, settings);
                    // ---------------------------------------------

                    // Configuramos la curva para animar el componente SpriteRenderer
                    EditorCurveBinding curveBinding = new EditorCurveBinding();
                    curveBinding.type = typeof(SpriteRenderer);
                    curveBinding.path = "";
                    curveBinding.propertyName = "m_Sprite";

                    ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[sprites.Length];
                    for (int i = 0; i < sprites.Length; i++)
                    {
                        keyframes[i] = new ObjectReferenceKeyframe();
                        keyframes[i].time = i / clip.frameRate;
                        keyframes[i].value = sprites[i];
                    }

                    AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyframes);

                    // Lo guarda en la misma carpeta y con el mismo nombre que el PNG
                    string folderPath = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
                    string animPath = $"{folderPath}/{texture.name}.anim";
                    
                    AssetDatabase.CreateAsset(clip, animPath);
                }
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log("¡Animaciones creadas con éxito y Loop activado!");
    }
}
#endif
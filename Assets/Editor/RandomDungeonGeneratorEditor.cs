using UnityEditor;
using UnityEngine;

// If true, child classes of inspectedType will also show this editor. Defaults to false.
[CustomEditor(typeof(AbstractDungeonGenerator), true)]
public class RandomDungeonGeneratorEditor : Editor
{
   AbstractDungeonGenerator generator;

    private void Awake()
    {
        // Reference 
        generator = (AbstractDungeonGenerator) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Crear Mazmorra"))
        {
            generator.GenerateDungeon();
        }
    }
}

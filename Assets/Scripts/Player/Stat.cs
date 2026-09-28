using System.Collections.Generic;
[System.Serializable]
public class Stat
{
    private float baseValue;
    private List<float> modifiers = new List<float>();

    public void Initialize(float value)
    {
        baseValue = value;
        modifiers.Clear(); 
    }

    public float GetValue()
    {
        float finalValue = baseValue;
        foreach (float modifier in modifiers)
        {
            finalValue += modifier;
        }
        return finalValue;
    }

    public void AddModifier(float modifier)
    {
        if (modifier != 0) modifiers.Add(modifier);
    }

    public void RemoveModifier(float modifier)
    {
        if (modifier != 0) modifiers.Remove(modifier);
    }
}
using UnityEngine;

[System.Serializable]
public class ResourceValue
{
    public string resourceName;
    public Sprite resourceIcon;
    public int amount;
    public ResourceType Type; //chemical, materials, coins

    public ResourceValue(string name, int quantity, ResourceType type, Sprite icon = null)
    {
        resourceName = name;
        amount = quantity;
        Type = type;
        resourceIcon = icon;
    }
}

public enum ResourceType
{
    Chemical, 
    Material
}
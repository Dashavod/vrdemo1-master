using TriInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Project/Settings/Planet")]
public class PlanetSetting : ScriptableObject
{
    [AssetsOnly]
    public Sprite icon;
    public string planetName;
    public int position;
    [TextArea(10, 30)] public string description;
    public GameObject model;
}
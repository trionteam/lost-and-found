using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "LostItemCollection", menuName = "Create Lost Menu Item Collection")]
public class LostItemCollection : ScriptableObject
{
    public LostItemType[] lostItems;

    public void Awake()
    {
        Debug.Assert(lostItems != null);
        Debug.Assert(lostItems.Length > 0);
    }

    public LostItemType RandomItem()
    {
        int index = Random.Range(0, lostItems.Length);
        return lostItems[index];
    }
}


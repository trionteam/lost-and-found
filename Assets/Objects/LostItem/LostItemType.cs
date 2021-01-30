using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LostItemType", menuName = "LostAndFound/LostItemType")]
public class LostItemType : ScriptableObject
{
    public Sprite sprite;

    public GameObject prefab;

    public string itemName;

    public int score = 100;
}

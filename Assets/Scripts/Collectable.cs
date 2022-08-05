using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    COIN, BULLET, LIFE
}

public class Collectable : MonoBehaviour
{
    public ItemType item_type;
    public int item_value;
}

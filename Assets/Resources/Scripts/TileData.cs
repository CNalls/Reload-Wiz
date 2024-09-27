using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
sealed class TileData 
{
    [SerializeField] private bool isExplored, isVisible;

    public bool IsExplored { get => isExplored; set => isExplored = value; }
    public bool IsVisible { get => isVisible; set => isVisible = value; }
}

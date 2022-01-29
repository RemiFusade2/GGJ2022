using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MOVEMENT_PATTERN
{
    BACK_AND_FORTH,
    NOT_MOVING
}

[CreateAssetMenu(fileName = "Data", menuName = "Base Data/Move Data", order = 1)]
public class MoveData : ScriptableObject
{
    public float moveSpeed = 5;
    
    public MOVEMENT_PATTERN movementPattern = MOVEMENT_PATTERN.NOT_MOVING;

    public LayerMask obstacleLayerMask;
}

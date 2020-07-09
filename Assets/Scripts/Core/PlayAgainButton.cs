using System.Collections;
using System.Collections.Generic;
using DevJJ.Entertainment.Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayAgainButton : MonoBehaviour , IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        GameManager.state = GameState.Begin;
    }
}

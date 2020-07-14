using UnityEngine;

namespace DevJJ.Entertainment.Assets.Scripts
{
    internal interface ISelectionResponse
    {
        void OnSelect(Transform selection);
        void OnDeselect(Transform selection);
    }
}
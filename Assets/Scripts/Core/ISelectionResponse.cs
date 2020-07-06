using UnityEngine;

namespace DevJJ.Entertainment.Assets.Scripts.Core
{
    internal interface ISelectionResponse
    {
        void OnSelect(Transform selection);
        void OnDeselect(Transform selection);
    }
}
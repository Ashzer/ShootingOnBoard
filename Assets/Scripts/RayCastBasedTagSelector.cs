using UnityEngine;

namespace DevJJ.Entertainment.Assets.Scripts
{
    public class RayCastBasedTagSelector : MonoBehaviour, ISelector
    {
        public Transform selection;

        public void Check(Ray ray, string selectableTag)
        {

            this.selection = null;
            if (Physics.Raycast(ray, out var hit))
            {
                var selection = hit.transform;
                //Debug.Log($"{selection.tag} clicked");
                if (selection.CompareTag(selectableTag))
                {
                    this.selection = selection;
                }
            }
        }

        public Transform GetSelection()
        {
            return this.selection;
        }
    }
}
using UnityEngine;

namespace DevJJ.Entertainment.Assets.Scripts.Core
{
    public class ScoreCount : MonoBehaviour
    {
        private void OnTriggerExit(Component other)
        {
            other.gameObject.tag = "Falling Piece";
            Destroy(other.gameObject, 30f);
        }
    }
}

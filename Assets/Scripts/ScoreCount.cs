using UnityEngine;

namespace DevJJ.Entertainment.Assets.Scripts
{
    public class ScoreCount : MonoBehaviour
    {
        [SerializeField] private GameObject _fallingPieceContainer;

        private void OnTriggerExit(Component other)
        {
            other.gameObject.tag = "Falling Piece";
            other.transform.SetParent(_fallingPieceContainer.transform);
            Destroy(other.gameObject, 30f);
        }
    }
}

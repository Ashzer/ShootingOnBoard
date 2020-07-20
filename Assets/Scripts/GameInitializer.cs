using UnityEngine;

namespace DevJJ.Entertainment.Assets.Scripts
{
    public class GameInitializer : MonoBehaviour, IGameInitializer
    {
        [SerializeField] private GameObject _redContainer;
        [SerializeField] private GameObject _blueContainer;
        [SerializeField] private GameObject _redPiece;
        [SerializeField] private GameObject _bluePiece;

        public void InitializeGame()
        {
            var piecesInitLocation = new int[10, 2] { { 2, 4 }, { 8, 4 }, { 12, 4 }, { 18, 4 }, { 2, 8 }, { 8, 8 }, { 12, 8 }, { 18, 8 }, { 5, 6 }, { 15, 6 } };

            foreach (var child in _blueContainer.GetComponentsInChildren<Transform>())
            {
                if (child.name == _blueContainer.name) continue;
                Object.Destroy(child.gameObject);
            }

            foreach (var child in _redContainer.GetComponentsInChildren<Transform>())
            {
                if (child.name == _redContainer.name) continue;
                Object.Destroy(child.gameObject);
            }

            for (var i = 0; i < piecesInitLocation.GetLength(0); i++)
            {
                Object.Instantiate((Object) _redPiece, _redContainer.transform.position + new Vector3(piecesInitLocation[i, 0], 0, -piecesInitLocation[i, 1]), Quaternion.identity, _redContainer.transform);
                Object.Instantiate((Object) _bluePiece, _blueContainer.transform.position + new Vector3(piecesInitLocation[i, 0], 0, piecesInitLocation[i, 1]), Quaternion.identity, _blueContainer.transform);
            }
        }
    }
}
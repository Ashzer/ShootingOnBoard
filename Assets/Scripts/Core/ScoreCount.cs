using UnityEngine;

namespace DevJJ.Entertainment.Assets.Scripts.Core
{
    public class ScoreCount : MonoBehaviour
    {
        /*
        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "Red Pieces":
                    GameManager.AddRed();
                    break;
                case "Blue Pieces":
                    GameManager.AddBlue();
                    break;
            }
            
            //Debug.Log($"{other.tag} has entered");
        }
        */
        

        private void OnTriggerExit(Component other)
        {
            /*
            switch (other.tag)
            {
                case "Red Pieces":
                    GameManager.SubRed();
                    break;
                case "Blue Pieces":
                    GameManager.SubBlue();
                    break;
            }
            */

            other.gameObject.tag = "Falling Piece";
            Destroy(other.gameObject, 30f);
        }
    }
}

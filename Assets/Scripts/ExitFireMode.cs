namespace DevJJ.Entertainment.Assets.Scripts
{
    public class ExitFireMode
    {
        internal void ExitFire()
        {
            if (FireButton._buttonPressed) return;
            switch (GameManager.State)
            {
                case GameState.BlueTeamFire:
                    GameManager.State = GameState.BlueTeamSelection;
                    break;
                case GameState.RedTeamFire:
                    GameManager.State = GameState.RedTeamSelection;
                    break;
            }
        }
    }
}
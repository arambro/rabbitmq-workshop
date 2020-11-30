namespace game.Messages
{
    public class ValidGoal : Goal
    {
        public ValidGoal()
        {

        }

        public ValidGoal(Goal goal)
        {
            this.Team = goal.Team;
            this.PlayerNumber = goal.PlayerNumber;
            this.PlayerName = goal.PlayerName;
        }
    }
}

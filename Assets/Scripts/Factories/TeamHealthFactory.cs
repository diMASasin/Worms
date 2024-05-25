using UnityEngine;
using WormComponents;

namespace Factories
{
    public class TeamHealthFactory : MonoBehaviour
    {
        public void Create(CycledList<Team> teams, TeamHealth prefab)
        {
            foreach (var team in teams)
            {
                var teamHealth = Instantiate(prefab, transform);
                teamHealth.Init(team.Color, team);
            }
        }
    }
}

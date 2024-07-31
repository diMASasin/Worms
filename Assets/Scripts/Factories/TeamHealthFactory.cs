using UnityEngine;
using WormComponents;

namespace Factories
{
    public class TeamHealthFactory : MonoBehaviour
    {
        [SerializeField] private Transform _parent;
        
        public void Create(CycledList<Team> teams, TeamHealth prefab)
        {
            foreach (var team in teams)
            {
                var teamHealth = Instantiate(prefab, _parent);
                teamHealth.Init(team.Color, team);
            }
        }
    }
}

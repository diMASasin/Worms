using System.Collections.Generic;
using Configs;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private WeaponConfig[] _weaponConfigs;
    [SerializeField] private ProjectilePoolAbstract[] _pools;
    [SerializeField] private WeaponSelector _weaponSelector;
    [SerializeField] private TurnTimer _turnTimer;
    [SerializeField] private FollowingCamera _followingCamera;
    
    private List<Weapon> _weaponList = new();

    public IReadOnlyList<Weapon> Weapon => _weaponList;

    private void Awake()
    {
        CreateWeapon();
        _weaponSelector.Init(_weaponList);
        _turnTimer.Init();
        _followingCamera.Init();
    }

    private void CreateWeapon()
    {
        for (int i = 0; i < _weaponConfigs.Length; i++)
        {
            Weapon newWeapon = new(_weaponConfigs[i], _pools[i]);
            _weaponList.Add(newWeapon);
        }
    }
}

using System;
using System.Collections.Generic;
using EventProviders;
using UnityEngine;
using WormComponents;
using static UnityEngine.Object;

namespace Factories
{
    public class WormInfoFactory : IDisposable
    {
        private readonly WormInformationView _wormInfoPrefab;
        private readonly IWormEventsProvider _wormEvents;
        private List<IFixedTickable> _fixedTickables = new();

        public WormInfoFactory(WormInformationView prefab, IWormEventsProvider wormEvents)
        {
            _wormInfoPrefab = prefab;
            _wormEvents = wormEvents;
            
            _wormEvents.WormCreated += CreateInfoView;
        }
        
        public void Dispose()
        {
            _wormEvents.WormCreated -= CreateInfoView;
        }

        public void LateTick()
        {
            foreach (var tickable in _fixedTickables)
                tickable.LateTick();
        }

        private void CreateInfoView(IWorm worm, Color teamColor, string wormName)
        {
            WormInformationView wormInfo = Instantiate(_wormInfoPrefab);
            wormInfo.Init(worm, teamColor, wormName);
            
            FollowingObject followingObject = new(wormInfo.transform, new Vector2(0, 1.4f));
            followingObject.Follow(worm.Transform);
            
            _fixedTickables.Add(followingObject);
        }
    }
}
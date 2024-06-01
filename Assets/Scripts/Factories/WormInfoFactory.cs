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

        private void CreateInfoView(IWorm worm, Color teamColor, string wormName)
        {
            WormInformationView wormInfo = Instantiate(_wormInfoPrefab);
            wormInfo.Init(worm, teamColor, wormName);
        }
    }
}
using UnityEngine;

namespace Services
{
    public class AllServices
    {
        private static AllServices _instance;
        public static AllServices Container
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AllServices();
                }

                return _instance;
            }
        }

        public AllServices()
        {
            Debug.Log($"adsdas");
        }

        public void RegisterSingle<TService>(TService implementation) where TService : IService => 
            Implementation<TService>.ServiceInstance = implementation;

        public TService Single<TService>() where TService : IService => 
            Implementation<TService>.ServiceInstance;

        private static class Implementation<TService> where TService : IService
        {
            public static TService ServiceInstance;
        }
    }
}
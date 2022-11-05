using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace CodeBase.Infrastructure.AssetsManagement
{
    public class AddressablesService : MonoBehaviour, IAssetsService
    {
        private const string ProviderId = "UnityEngine.ResourceManagement.ResourceProviders.BundledAssetProvider";

        #region Singleton

        private void Awake() => Instance ??= this;
        public static AddressablesService Instance { get; private set; }

        #endregion

        [SerializeField] private Sprite spritePlaceholder;
        
        private Dictionary<string, AsyncOperationHandle<Sprite>> _spawnedSprites;
        private Sprite _spritePlaceholder;
        private List<IResourceLocation> _allLocations;

        
        [ItemCanBeNull]
        public async Task<Sprite> GetSpriteById(string id)
        {
            var operation = Addressables.LoadAssetAsync<Sprite>($"{id}");

            if (operation.OperationException != null)
                operation = Addressables.LoadAssetAsync<Sprite>(spritePlaceholder.name);

            _spawnedSprites ??= new Dictionary<string, AsyncOperationHandle<Sprite>>();

            if (!_spawnedSprites.ContainsKey(id)) 
                _spawnedSprites.Add(id, operation);
            else 
                return await _spawnedSprites[id].Task;

            return await operation.Task;
        }

        public void ReleaseSprite(string id)
        {
            if (!_spawnedSprites.ContainsKey(id)) return;
            
            Addressables.Release(_spawnedSprites[id]);
            _spawnedSprites.Remove(id);
        }

        public async Task<List<string>> GetAddressablesList<T>()
        {
            _allLocations ??= await GetAllLocations();
            return _allLocations
                .Where(l => l.ResourceType == typeof(T))
                .Select(l => l.PrimaryKey)
                .ToList();
        }

        private static async Task<List<IResourceLocation>> GetAllLocations()
        {
            if (!Addressables.ResourceLocators.Any())
                await Addressables.InitializeAsync().Task;

            var result = new List<IResourceLocation>();
            var map = Addressables.ResourceLocators.First() as ResourceLocationMap;
            foreach (var locations in map.Locations.Values)
                result.AddRange(locations.Where(l => l.ProviderId == ProviderId));

            return result.Distinct().ToList();
        }
    }
}
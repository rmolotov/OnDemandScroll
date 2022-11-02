using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CodeBase.Infrastructure
{
    public class AssetsService : MonoBehaviour
    {
        #region Singleton

        private void Awake() => Instance ??= this;
        public static AssetsService Instance { get; private set; }

        #endregion

        [SerializeField] private Sprite spritePlaceholder;
        
        private Dictionary<string, AsyncOperationHandle<Sprite>> _spawnedSprites;
        
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
    }
}
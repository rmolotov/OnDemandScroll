using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CodeBase.Infrastructure.AssetsManagement
{
    public class ResourcesService : MonoBehaviour, IAssetsService
    {
        #region Singleton

        private void Awake() => Instance ??= this;
        public static ResourcesService Instance { get; private set; }

        #endregion
        
        [SerializeField] private Sprite spritePlaceholder;
        [SerializeField] private string spritesFolder;

        private Dictionary<string, Task<Sprite>> _spawnedSprites;


        [ItemCanBeNull]
        public async Task<Sprite> GetSpriteById(string id)
        {
            if (_spawnedSprites.ContainsKey(id))
                return await _spawnedSprites[id];
            
            var request = Resources.LoadAsync<Sprite>(spritesFolder + $"/{id}");

            await Task.Run(async () =>
            {
                while (!request.isDone) await Task.Delay(10);
            });

            var result = Task.FromResult(request != null
                ? (Sprite) request.asset
                : spritePlaceholder
            );
            
            _spawnedSprites ??= new Dictionary<string, Task<Sprite>>();
            _spawnedSprites.Add(id, result);

            return await result;
        }

        public void ReleaseSprite(string id)
        {
            if (!_spawnedSprites.ContainsKey(id)) return;
            
            Addressables.Release(_spawnedSprites[id]);
            _spawnedSprites.Remove(id);
        }

        public Task<int> GetSpritesCount()
        {
            return Task.FromResult(9000);
        }
    }
}
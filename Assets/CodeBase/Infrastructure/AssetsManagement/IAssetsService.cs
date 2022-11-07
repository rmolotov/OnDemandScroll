using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Infrastructure.AssetsManagement
{
    public interface IAssetsService
    {
        public static IAssetsService Instance { get; protected set; }

        public async Task<List<string>> GetAssetsList<T>() =>
            await Task.FromResult<List<string>>(new List<string>());
        public async Task<Sprite> GetSpriteById(string id) => 
            await Task.FromResult<Sprite>(null);

        public void ReleaseSprite(string id);
    }
}
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace CodeBase.Infrastructure.AssetsManagement
{
    public class ResourcesService : MonoBehaviour, IAssetsService
    {
        #region Singleton

        private void Awake() => IAssetsService.Instance ??= this;

        #endregion

        [SerializeField] private Sprite spritePlaceholder;
        
        [Header("Resources subfolders")]
        [SerializeField] private string spritesFolder = "images";

        private Dictionary<string, Sprite> _spawnedSprites;


        [ItemCanBeNull]
        public async Task<Sprite> GetSpriteById(string id)
        {
            _spawnedSprites ??= new Dictionary<string, Sprite>();
            
            if (_spawnedSprites.ContainsKey(id))
                return await Task.FromResult(_spawnedSprites[id]);

            var request = Resources.LoadAsync<Sprite>(spritesFolder + $"/{id}");

            await Task.Factory.StartNew(async () =>
            {
                while (!request.isDone) await Task.Delay(10);
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

            var result = request != null
                ? (Sprite) request.asset
                : spritePlaceholder;

            _spawnedSprites.Add(id, result);

            return await Task.FromResult(result);
        }

        public void ReleaseSprite(string id)
        {
            if (!_spawnedSprites.ContainsKey(id)) return;

            var s = _spawnedSprites[id];
            _spawnedSprites.Remove(id);
            Resources.UnloadAsset(s);
            Resources.UnloadUnusedAssets();
        }

        public async Task<List<string>> GetAssetsList<T>()
        {
            var path = $"{Application.dataPath}/Resources/{spritesFolder}/";
            
            var filesList = Directory.GetFiles(path)
                .Where(s => !s.EndsWith(".meta"))
                .Select(s => s[(s.LastIndexOf('/')+1)..s.LastIndexOf(".", StringComparison.Ordinal)])
                .ToList();
            return await Task.FromResult(filesList);
        }
    }
}
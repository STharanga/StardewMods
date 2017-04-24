﻿using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Content.Utilities;
using StardewModdingAPI.Content.Plugins;

namespace StardewModdingAPI.Content
{
    class ContentRegistry : IContentRegistry
    {
        private IMod Mod;
        private IMonitor Monitor;
        private string ModPath;
        public ContentRegistry(IMonitor monitor, IMod mod)
        {
            Monitor = monitor;
            Mod = mod;
            ModPath = Mod.Helper.DirectoryPath.Replace(ExtendibleContentManager.ModContent.RootDirectory,"");
        }
        public T Load<T>(string assetName)
        {
            return ExtendibleContentManager.ModContent.Load<T>(Path.Combine(ModPath, assetName));
        }
        public void RegisterContentHandler(IContentHandler handler)
        {
            Monitor.Log("Custom content handler added by mod: "+Mod.ModManifest.Name, LogLevel.Trace);
            ExtendibleContentManager.AddContentHandler(handler);
        }
        public void RegisterTexturePatch(string asset, Texture2D patch, Rectangle destination, Rectangle? source)
        {
            Monitor.Log("Texture patch registered by mod: " + Mod.ModManifest.Name, LogLevel.Trace);
            if (!TextureInjector.AssetMap.ContainsKey(asset))
                TextureInjector.AssetMap.Add(asset, new List<TextureData>());
            TextureInjector.AssetMap[asset].Add(new TextureData(patch, destination, source));
            if (TextureInjector.AssetCache.ContainsKey(asset))
                TextureInjector.AssetCache.Remove(asset);
        }
        public void RegisterTexturePatch(string asset, string patch, Rectangle destination, Rectangle? source)
        {
            RegisterTexturePatch(asset, Load<Texture2D>(patch), destination, source);
        }
        public void RegisterDictionaryPatch<Tkey,TValue>(string asset, Dictionary<Tkey,TValue> patch)
        {
            if (!DictionaryInjector.AssetMap.ContainsKey(asset))
                DictionaryInjector.AssetMap.Add(asset, new List<object>());
            DictionaryInjector.AssetMap[asset].Add(patch);
            if (DictionaryInjector.AssetCache.ContainsKey(asset))
                DictionaryInjector.AssetCache.Remove(asset);
        }
        public void RegisterDictionaryPatch<TKey,TValue>(string asset, string patch)
        {
            RegisterDictionaryPatch(asset, Load<Dictionary<TKey, TValue>>(patch));
        }
        public void RegisterXnbReplacement(string asset, string replacement)
        {
            Monitor.Log("Xnb replacement registered by mod: " + Mod.ModManifest.Name, LogLevel.Trace);
            XnbLoader.AssetMap.Add(asset, Path.Combine(ModPath,replacement));
        }
        public void RegisterLoader<T>(string asset, AssetLoader<T> loader)
        {
            Monitor.Log("Asset loader registered by mod: " + Mod.ModManifest.Name, LogLevel.Trace);
            DelegatedContentHandler.AssetLoadMap.Add(asset, loader);
        }
        public void RegisterLoader<T>(AssetLoader<T> loader)
        {
            Monitor.Log("Type loader registered by mod: " + Mod.ModManifest.Name, LogLevel.Trace);
            DelegatedContentHandler.TypeLoadMap.Add(typeof(T), loader);
        }
        public void RegisterInjector<T>(string asset, AssetInjector<T> injector)
        {
            Monitor.Log("Asset injector registered by mod: " + Mod.ModManifest.Name, LogLevel.Trace);
            if (!DelegatedContentHandler.AssetInjectMap.ContainsKey(asset))
                DelegatedContentHandler.AssetInjectMap.Add(asset, new List<Delegate>());
            DelegatedContentHandler.AssetInjectMap[asset].Add(injector);
        }
        /// <summary>
        /// If none of the build in content handlers are sufficient, and making a custom one is overkill, this method lets you handle the injection for a specific type of asset
        /// </summary>
        /// <typeparam name="T">The Type the asset is loaded as</typeparam>
        /// <param name="injector">The delegate assigned to handle loading for this type</param>
        public void RegisterInjector<T>(AssetInjector<T> injector)
        {
            Monitor.Log("Type injector registered by mod: " + Mod.ModManifest.Name, LogLevel.Trace);
            if (!DelegatedContentHandler.TypeInjectMap.ContainsKey(typeof(T)))
                DelegatedContentHandler.TypeInjectMap.Add(typeof(T), new List<Delegate>());
            DelegatedContentHandler.TypeInjectMap[typeof(T)].Add(injector);
        }
    }
}

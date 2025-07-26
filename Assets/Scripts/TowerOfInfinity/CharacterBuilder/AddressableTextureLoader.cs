using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TowerOfInfinity.CharacterBuilder
{
    public static class AddressableTextureLoader
    {
        public static async Task<Texture2D> LoadTextureAsync(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                Debug.LogWarning("Address is null or empty. Cannot load texture.");
                return null;
            }

            AsyncOperationHandle<Texture2D> handle = Addressables.LoadAssetAsync<Texture2D>(address);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }
            else
            {
                Debug.LogError($"Failed to load texture from address: {address}. Error: {handle.OperationException?.Message}");
                return null;
            }
        }

        public static void ReleaseTexture(Texture2D texture)
        {
            if (texture != null)
            {
                Addressables.Release(texture);
            }
        }
    }
}
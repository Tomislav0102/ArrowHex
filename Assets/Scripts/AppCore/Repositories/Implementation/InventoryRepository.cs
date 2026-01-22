using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public class InventoryRepository : RepositoryBase, IInventoryRepository
{
    [UnityEngine.Scripting.Preserve]
    [UsedImplicitly]
    public InventoryRepository() { }

    public async UniTask<(bool Success, string DataAsText)> ItemBuyAsync(ItemBuyData itemBuyData)
    {
        string repositoryName = nameof(InventoryRepository);
        string methodName = nameof(ItemBuyAsync);

        #region Request
        string payload = JsonConvert.SerializeObject(itemBuyData);
        var request = CreatePostRequest(payload, apiConfig.ItemBuyUrl, true, repositoryName, methodName);
        #endregion

        var (success, dataAsText) = await DoRequest(request);
        if (!success)
        {
            Debug.Log($"{repositoryName}::{methodName} failed dataAsText= {dataAsText}");
        }
        return (Success: success, DataAsText: dataAsText);
    }

    public async UniTask<(bool Success, string DataAsText)> ItemEquipAsync(ItemEquipData itemEquipData)
    {
        string repositoryName = nameof(InventoryRepository);
        string methodName = nameof(ItemEquipAsync);

        #region Request
        string payload = JsonConvert.SerializeObject(itemEquipData);
        var request = CreatePostRequest(payload, apiConfig.ItemEquipUrl, true, repositoryName, methodName);
        #endregion

        var (success, dataAsText) = await DoRequest(request);
        if (!success)
        {
            Debug.Log($"{repositoryName}::{methodName} failed dataAsText= {dataAsText}");
        }
        return (Success: success, DataAsText: dataAsText);
    }
}

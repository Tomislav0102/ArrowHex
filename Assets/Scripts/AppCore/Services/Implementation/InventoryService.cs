using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using VContainer;

public class InventoryService : BaseService, IInventoryService
{
    [Inject] private IInventoryRepository inventoryRepository;

    [UnityEngine.Scripting.Preserve]
    [UsedImplicitly]
    public InventoryService() { }
    
    public async UniTask<(bool Success, RequestErrorInfo ErrorInfo)> PostItemBuyAsync(ItemBuyData itemBuyData)
    {
        string serviceName = nameof(InventoryService);
        string methodName = nameof(PostItemBuyAsync);

        Debug.Log($"{serviceName}::{methodName} data= {itemBuyData}");

        var (success, dataAsText) = await inventoryRepository.ItemBuyAsync(itemBuyData);
        var errorInfo = RequestErrorInfo.GeneralError;
        try
        {
            errorInfo = serializationService.Deserialize<RequestErrorInfo>(dataAsText);
        }
        catch (Exception e)
        {
            // ignored
            Debug.LogWarning($"{serviceName}::{methodName} " +
                             $"parsing error info failed! {e.Message}"
            );
        }
        return (Success: success, ErrorInfo: errorInfo);
    }

    public async UniTask<(bool Success, RequestErrorInfo ErrorInfo)> UpdateItemEquipAsync(ItemEquipData itemEquipData)
    {
        string serviceName = nameof(InventoryService);
        string methodName = nameof(UpdateItemEquipAsync);

        Debug.Log($"{serviceName}::{methodName} data= {itemEquipData}");

        var (success, dataAsText) = await inventoryRepository.ItemEquipAsync(itemEquipData);
        var errorInfo = RequestErrorInfo.GeneralError;
        try
        {
            errorInfo = serializationService.Deserialize<RequestErrorInfo>(dataAsText);
        }
        catch (Exception e)
        {
            // ignored
            Debug.LogWarning($"{serviceName}::{methodName} " +
                             $"parsing error info failed! {e.Message}"
            );
        }
        return (Success: success, ErrorInfo: errorInfo);
    }
}

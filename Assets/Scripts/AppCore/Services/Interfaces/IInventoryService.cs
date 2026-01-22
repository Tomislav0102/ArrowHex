using Cysharp.Threading.Tasks;

public interface IInventoryService
{
    UniTask<(bool Success, RequestErrorInfo ErrorInfo)> PostItemBuyAsync(ItemBuyData itemBuyData);
    UniTask<(bool Success, RequestErrorInfo ErrorInfo)> UpdateItemEquipAsync(ItemEquipData itemEquipData);
}

using Cysharp.Threading.Tasks;

public interface IInventoryRepository
{
    UniTask<(bool Success, string DataAsText)> ItemBuyAsync(ItemBuyData itemBuyData);
    UniTask<(bool Success, string DataAsText)> ItemEquipAsync(ItemEquipData itemEquipData);
}

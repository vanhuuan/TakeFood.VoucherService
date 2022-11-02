using TakeFood.VoucherService.ViewModel.Dtos.Topping;

namespace TakeFood.VoucherService.Service
{
    public interface IToppingService
    {
        Task CreateTopping(string StoreID, CreateToppingDto topping);
        Task UpdateTopping(string ID, CreateToppingDto topping);
        Task<ToppingViewDto> GetToppingByID(string ID);
        Task<List<ToppingViewDto>> GetAllToppingByStoreID(string storeID, string state);
        Task DeleteTopping(string ID);
    }
}

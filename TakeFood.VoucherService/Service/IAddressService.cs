using TakeFood.VoucherService.ViewModel.Dtos.Address;

namespace TakeFood.VoucherService.Service
{
    public interface IAddressService
    {
        Task CreateAddress(AddressDto address);
        Task DeleteAddress(String id);
    }
}

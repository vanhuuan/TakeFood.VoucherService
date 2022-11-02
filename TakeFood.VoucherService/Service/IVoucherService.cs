using TakeFood.VoucherService.ViewModel.Dtos.Voucher;

namespace TakeFood.VoucherService.Service;

public interface IVoucherService
{
    Task CreateVoucherAsync(CreateVoucherDto dto, string ownerId);
    Task UpdateVoucherAsync(UpdateVoucherDto dto, string ownerId);
    Task<List<VoucherCardDto>> GetAllStoreVoucherOkeAsync(string storeId);
}

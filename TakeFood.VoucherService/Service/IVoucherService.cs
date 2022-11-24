using TakeFood.VoucherService.ViewModel.Dtos.Voucher;

namespace TakeFood.VoucherService.Service;

public interface IVoucherService
{
    Task CreateVoucherAsync(CreateVoucherDto dto, string ownerId);
    Task CreateSystemVoucherAsync(CreateVoucherDto dto);
    Task UpdateVoucherAsync(UpdateVoucherDto dto, string ownerId);
    Task DeleteVoucherAsync(string voucherId, string ownerId);
    Task<List<VoucherCardDto>> GetAllStoreVoucherOkeAsync(string storeId);
    Task<VoucherPagingResponse> GetPagingVoucher(GetPagingVoucherDto dto, string ownerId);
    Task<VoucherPagingResponse> GetPagingSystemVoucher(GetPagingVoucherDto dto);
     
}

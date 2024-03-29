﻿using TakeFood.VoucherService.ViewModel.Dtos.Voucher;

namespace TakeFood.VoucherService.Service;

public interface IVoucherService
{
    Task CreateVoucherAsync(CreateVoucherDto dto, string ownerId);
    Task CreateSystemVoucherAsync(CreateVoucherDto dto);
    Task UpdateVoucherAsync(UpdateVoucherDto dto, string ownerId);
    Task UpdateSystemVoucherAsync(UpdateVoucherDto dto);
    Task DeleteVoucherAsync(string voucherId, string ownerId);
    Task<List<VoucherCardDto>> GetAllStoreVoucherOkeAsync(string storeId);
    Task<VoucherPagingResponse> GetPagingVoucher(GetPagingVoucherDto dto, string ownerId);
    Task<VoucherPagingResponse> GetPagingSystemVoucher(GetPagingVoucherDto dto);
    Task<VoucherPagingResponse> GetPagingStoreVoucher(GetPagingVoucherDto dto, string storeID, string status);
    Task<UpdateVoucherDto> GetVoucherByID(string id);
    Task DeleteSystemVoucher(string id);
}

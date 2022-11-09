namespace TakeFood.VoucherService.ViewModel.Dtos.Voucher;

public class VoucherPagingResponse
{
    public int Total { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public List<VoucherCardDto> Cards { get; set; }
}

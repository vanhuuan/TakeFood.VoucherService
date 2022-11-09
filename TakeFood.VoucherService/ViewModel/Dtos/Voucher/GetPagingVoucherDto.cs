using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TakeFood.VoucherService.ViewModel.Dtos.Voucher;

public class GetPagingVoucherDto
{
    [Required]
    [FromQuery]
    public int PageNumber { get; set; }
    [Required]
    public int PageSize { get; set; }
    [FromQuery]
    public DateTime? StartDate { get; set; }
    [FromQuery]
    public DateTime? EndDate { get; set; }
    [Required]
    [FromQuery]
    public String QueryType { get; set; }
    [Required]
    [FromQuery]
    public String QueryString { get; set; }
}

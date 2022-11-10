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
    /// <summary>
    /// Code/Name
    /// </summary>
    [Required]
    [FromQuery]
    public String QueryType { get; set; }
    /// <summary>
    /// Text to search
    /// </summary>
    [Required]
    [FromQuery]
    public String QueryString { get; set; }
    /// <summary>
    /// CreateDate StartDate EndDate Name Code
    /// </summary>
    [Required]
    [FromQuery]
    public String SortBy { get; set; }
    /// <summary>
    /// Desc Asc
    /// </summary>
    [Required]
    [FromQuery]
    public String SortType { get; set; }
}

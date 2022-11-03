using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TakeFood.VoucherService.ViewModel.Dtos.Voucher;

public class VoucherCardDto
{
    /// <summary>
    /// Voucher Id
    /// </summary>
    [JsonPropertyName("voucherId")]
    [Required]
    public string VoucherId { get; set; }
    /// <summary>
    /// Voucher name
    /// </summary>
    [JsonPropertyName("name")]
    [Required]
    public string Name { get; set; }
    /// <summary>
    /// Voucher description
    /// </summary>
    [JsonPropertyName("description")]
    [Required]
    public string Description { get; set; }
    [JsonPropertyName("minSpend")]
    [Required]
    public Double MinSpend { get; set; }
    [JsonPropertyName("amount")]
    [Required]
    public Double Amount { get; set; }
    [JsonPropertyName("maxDiscount")]
    [Required]
    public Double MaxDiscount { get; set; }
}

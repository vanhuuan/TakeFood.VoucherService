using Microsoft.AspNetCore.Mvc;
using StoreService.Middleware;
using StoreService.Service;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using TakeFood.VoucherService.Model.Entities;
using TakeFood.VoucherService.Service;
using TakeFood.VoucherService.ViewModel.Dtos.Voucher;

namespace TakeFood.VoucherService.Controllers;

public class VoucherController : BaseController
{
    public IVoucherService VoucherService { get; set; }
    public IJwtService JwtService { get; set; }
    public VoucherController(IVoucherService VoucherService, IJwtService jwtService)
    {
        this.VoucherService = VoucherService;
        this.JwtService = jwtService;
    }

    [HttpPost]
    [Authorize(roles: Roles.ShopeOwner)]
    [Route("AddVoucher")]
    public async Task<IActionResult> AddVoucherAsync([FromBody] CreateVoucherDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await VoucherService.CreateVoucherAsync(dto, GetId());
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Authorize(roles: Roles.Admin)]
    [Route("AddSystemVoucher")]
    public async Task<IActionResult> AddSystemVoucherAsync([FromBody] CreateVoucherDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await VoucherService.CreateSystemVoucherAsync(dto);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut]
    [Authorize(roles: Roles.ShopeOwner)]
    [Route("UpdateVoucher")]
    public async Task<IActionResult> UpdateVoucherAsync([FromBody] UpdateVoucherDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await VoucherService.UpdateVoucherAsync(dto, GetId());
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    [Authorize]
    [Route("GetVoucher")]
    public async Task<IActionResult> GetVoucherAsync([Required] string storeId)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var rs = await VoucherService.GetAllStoreVoucherOkeAsync(storeId);
            return Ok(rs);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    /// <summary>
    /// Page number 1 -> ....
    /// Page size > 0 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>

    [HttpGet]
    [Authorize(roles: Roles.ShopeOwner)]
    [Route("GetPagingVoucher")]
    public async Task<IActionResult> GetPagingVoucherAsync(GetPagingVoucherDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var rs = await VoucherService.GetPagingVoucher(dto, GetId());
            return Ok(rs);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    [Authorize(roles: Roles.Admin)]
    [Route("GetPagingSystemVoucher")]
    public async Task<IActionResult> GetPagingSystemVoucherAsync(GetPagingVoucherDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var rs = await VoucherService.GetPagingSystemVoucher(dto);
            return Ok(rs);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete]
    [Authorize(roles: Roles.ShopeOwner)]
    [Route("DeleteVoucher")]
    public async Task<IActionResult> GetPagingVoucherAsync([Required] string voucherId)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await VoucherService.DeleteVoucherAsync(voucherId, GetId());
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    [Route("GetPagingStoreVoucher")]
    public async Task<JsonResult> GetPagingStoreVoucher([FromQuery] GetPagingVoucherDto dto, string storeID,[Optional] string status)
    {
        try
        {
            var rs = await VoucherService.GetPagingStoreVoucher(dto, storeID, status);
            return new JsonResult(rs);
        }catch(Exception e)
        {
            return new JsonResult(e);
        }
    }

    [HttpGet]
    [Route("GetVoucherByID")]
    public async Task<JsonResult> GetVoucherByID(string ID)
    {
        try
        {
            var rs = await VoucherService.GetVoucherByID(ID);
            return new JsonResult(rs);
        }catch(Exception e)
        {
            return new JsonResult(e);
        }
    }

    public string GetId()
    {
        String token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()!;
        return JwtService.GetId(token);
    }
    public string GetId(string token)
    {
        return JwtService.GetId(token);
    }
}

using MongoDB.Driver;
using StoreService.Model.Entities.Store;
using StoreService.Model.Entities.Voucher;
using StoreService.Model.Repository;
using TakeFood.VoucherService.ViewModel.Dtos.Voucher;

namespace TakeFood.VoucherService.Service.Implement;

public class VouchersService : IVoucherService
{
    private IMongoRepository<Voucher> voucherRepository { get; set; }

    private IMongoRepository<Store> storeRepository { get; set; }

    public VouchersService(IMongoRepository<Voucher> voucherRepository, IMongoRepository<Store> storeRepository)
    {
        this.voucherRepository = voucherRepository;
        this.storeRepository = storeRepository;
    }
    public async Task CreateVoucherAsync(CreateVoucherDto dto, string ownerId)
    {
        var store = await storeRepository.FindByIdAsync(dto.StoreId);
        if (store == null || store.OwnerId != ownerId)
        {
            throw new Exception("You're not the owner or store not exist");
        }

        var checkCode = await voucherRepository.FindOneAsync(x => x.Code == dto.Code);
        if (checkCode != null)
        {
            throw new Exception("Code existed");
        }

        if (dto.StartDay < DateTime.Now || dto.ExpireDay < DateTime.Now || dto.StartDay < dto.StartDay)
        {
            throw new Exception("Unexecpted Datetime");
        }

        if (dto.Amount <= 0 || dto.MaxDiscount <= 0 || dto.MinSpend < 0)
        {
            throw new Exception("Money can't not be negative");
        }

        var voucher = new Voucher()
        {
            Amount = dto.Amount,
            StoreId = store.Id,
            StartDay = dto.StartDay,
            ExpireDay = dto.ExpireDay,
            Code = dto.Code,
            Description = dto.Description,
            MaxDiscount = dto.MaxDiscount,
            MinSpend = dto.MinSpend,
            Name = dto.Name,
            Type = false
        };

        await voucherRepository.InsertAsync(voucher);
    }

    public async Task CreateSystemVoucherAsync(CreateVoucherDto dto)
    {
        var checkCode = await voucherRepository.FindOneAsync(x => x.Code == dto.Code);
        if (checkCode != null)
        {
            throw new Exception("Code existed");
        }

        if (dto.StartDay < DateTime.Now || dto.ExpireDay < DateTime.Now || dto.StartDay < dto.StartDay)
        {
            throw new Exception("Unexecpted Datetime");
        }

        if (dto.Amount <= 0 || dto.MaxDiscount <= 0 || dto.MinSpend < 0)
        {
            throw new Exception("Money can't not be negative");
        }

        var voucher = new Voucher()
        {
            Amount = dto.Amount,
            StoreId = "0",
            StartDay = dto.StartDay,
            ExpireDay = dto.ExpireDay,
            Code = dto.Code,
            Description = dto.Description,
            MaxDiscount = dto.MaxDiscount,
            MinSpend = dto.MinSpend,
            Name = dto.Name,
            Type = true
        };

        await voucherRepository.InsertAsync(voucher);
    }

    public async Task<List<VoucherCardDto>> GetAllStoreVoucherOkeAsync(string storeId)
    {
        var now = DateTime.Now;
        var listVoucher = await voucherRepository.FindAsync(x => (x.StoreId == storeId || x.Type == true) && (x.StartDay <= now && x.ExpireDay >= now)); // Lay voucher cua cua hang hoac voucher cua he thong
        var rs = new List<VoucherCardDto>();
        foreach (var voucher in listVoucher)
        {
            rs.Add(new VoucherCardDto()
            {
                MaxDiscount = voucher.MaxDiscount,
                MinSpend = voucher.MinSpend,
                Amount = voucher.Amount,
                Description = voucher.Description,
                Name = voucher.Name,
                VoucherId = voucher.Id,
                StartDate = voucher.StartDay,
                EndDate = voucher.ExpireDay,
                Code = voucher.Code,
            });
        }
        return rs;
    }

    public async Task UpdateVoucherAsync(UpdateVoucherDto dto, string ownerId)
    {
        var store = await storeRepository.FindByIdAsync(dto.StoreId);
        if (store == null || store.OwnerId != ownerId)
        {
            throw new Exception("You're not the owner or store not exist");
        }

        if (dto.ExpireDay < DateTime.Now || dto.StartDay < dto.StartDay)
        {
            throw new Exception("Unexecpted Datetime");
        }

        if (dto.Amount <= 0 || dto.MaxDiscount <= 0 || dto.MinSpend < 0)
        {
            throw new Exception("Money can't not be negative");
        }
        var voucher = await voucherRepository.FindByIdAsync(dto.VoucherId);
        if (voucher != null)
        {
            voucher.Name = dto.Name;
            voucher.Amount = dto.Amount;
            voucher.StoreId = store.Id;
            voucher.StartDay = dto.StartDay;
            voucher.ExpireDay = dto.ExpireDay;
            voucher.Code = dto.Code;
            voucher.Description = dto.Description;
            voucher.MaxDiscount = dto.MaxDiscount;
            voucher.MinSpend = dto.MinSpend;
            await voucherRepository.UpdateAsync(voucher);
        }
        else
        {
            throw new Exception("Voucher is not exist");
        }
    }

    public async Task<VoucherPagingResponse> GetPagingVoucher(GetPagingVoucherDto dto, string uid)
    {
        var store = await storeRepository.FindOneAsync(x => x.OwnerId == uid);
        var filter = CreateFilter(dto.StartDate, dto.EndDate, dto.QueryString, dto.QueryType, store.Id);
        if (dto.PageNumber <= 0 || dto.PageSize <= 0)
        {
            throw new Exception("Pagenumber or pagesize can not be  zero or negative");
        }
        var rs = await voucherRepository.GetPagingAsync(filter, dto.PageNumber - 1, dto.PageSize);
        var list = new List<VoucherCardDto>();
        foreach (var voucher in rs.Data)
        {
            list.Add(new VoucherCardDto()
            {
                MaxDiscount = voucher.MaxDiscount,
                MinSpend = voucher.MinSpend,
                Amount = voucher.Amount,
                Description = voucher.Description,
                Name = voucher.Name,
                VoucherId = voucher.Id,
                CreateDate = voucher.CreatedDate!.Value,
                StartDate = voucher.StartDay,
                EndDate = voucher.ExpireDay,
                Code = voucher.Code,
                Type = voucher.Type
            });
        }
        switch (dto.SortBy)
        {
            case "CreateDate": list = list.OrderBy(x => x.CreateDate).ToList(); break;
            case "StartDate": list = list.OrderBy(x => x.StartDate).ToList(); break;
            case "EndDate": list = list.OrderBy(x => x.EndDate).ToList(); break;
            case "Name": list = list.OrderBy(x => x.Name).ToList(); break;
            case "Code": list = list.OrderBy(x => x.Code).ToList(); break;
        }
        switch (dto.SortType)
        {
            case "Desc": list.Reverse(); break;
        }
        var info = new VoucherPagingResponse()
        {
            Total = rs.Count,
            PageIndex = dto.PageNumber,
            PageSize = dto.PageSize,
            Cards = list
        };
        return info;
    }

    private FilterDefinition<Voucher> CreateFilter(DateTime? startDate, DateTime? endDate, string query, string queryType, string storeId)
    {
        var filter = Builders<Voucher>.Filter.Eq(x => x.StoreId, storeId);
        if (startDate != null && endDate != null)
        {
            filter &= Builders<Voucher>.Filter.Gte(x => x.StartDay, startDate);
            filter &= Builders<Voucher>.Filter.Lte(x => x.ExpireDay, endDate);
        }
        switch (queryType)
        {
            case "Code": filter &= Builders<Voucher>.Filter.Where(x => x.Code.Contains(query)); break;
            case "Name": filter &= Builders<Voucher>.Filter.Where(x => x.Name.Contains(query)); break;
            default: filter &= Builders<Voucher>.Filter.StringIn(x => x.Code, query); break;
        }
        return filter;
    }

    public async Task DeleteVoucherAsync(string voucherId, string ownerId)
    {
        var store = await storeRepository.FindOneAsync(x => x.OwnerId == ownerId);
        var voucher = await voucherRepository.FindByIdAsync(voucherId);
        if (store.Id != voucher.StoreId)
        {
            throw new Exception("Can't delete voucher");
        }
        await voucherRepository.DeleteAsync(voucher.Id);
    }

    public async Task<VoucherPagingResponse> GetPagingSystemVoucher(GetPagingVoucherDto dto)
    {
        var filter = CreateSystemFilter(dto.StartDate, dto.EndDate, dto.QueryString, dto.QueryType);
        var sort = CreateSortFilter(dto.SortType, dto.SortBy);
        if (dto.PageNumber <= 0 || dto.PageSize <= 0)
        {
            throw new Exception("Pagenumber or pagesize can not be  zero or negative");
        }
        var rs = await voucherRepository.GetPagingAsync(filter, dto.PageNumber - 1, dto.PageSize, sort);
        var list = new List<VoucherCardDto>();
        foreach (var voucher in rs.Data)
        {
            list.Add(new VoucherCardDto()
            {
                MaxDiscount = voucher.MaxDiscount,
                MinSpend = voucher.MinSpend,
                Amount = voucher.Amount,
                Description = voucher.Description,
                Name = voucher.Name,
                VoucherId = voucher.Id,
                CreateDate = voucher.CreatedDate!.Value,
                StartDate = voucher.StartDay,
                EndDate = voucher.ExpireDay,
                Code = voucher.Code,
            });
        }
        int stt = 0;
        foreach (var i in list)
        {
            stt++;
            i.Id = stt;
            i.Stt = stt;
        }
        var info = new VoucherPagingResponse()
        {
            Total = rs.Count,
            PageIndex = dto.PageNumber,
            PageSize = dto.PageSize,
            Cards = list
        };
        return info;
    }

    private FilterDefinition<Voucher> CreateSystemFilter(DateTime? startDate, DateTime? endDate, string query, string queryType)
    {
        var filter = Builders<Voucher>.Filter.Eq(x => x.Type, true);
        if (startDate != null && endDate != null)
        {
            filter &= Builders<Voucher>.Filter.Gte(x => x.StartDay, startDate);
            filter &= Builders<Voucher>.Filter.Lte(x => x.ExpireDay, endDate);
        }
        if (queryType != "All")
        {
            switch (queryType)
            {

                default: filter &= Builders<Voucher>.Filter.StringIn(x => x.Code, query); break;
            }
        }
        return filter;
    }

    private SortDefinition<Voucher> CreateSortFilter(string sortType, string sortBy)
    {
        var filter = Builders<Voucher>.Sort.Ascending(x => x.Id);
        if (sortType == "Desc")
        {
            switch (sortBy)
            {
                case "CreateDate": filter = Builders<Voucher>.Sort.Descending(x => x.Code); break;
                case "StartDate": filter = Builders<Voucher>.Sort.Descending(x => x.Code); break;
                case "EndDate": filter = Builders<Voucher>.Sort.Descending(x => x.Code); break;
                case "Name": filter = Builders<Voucher>.Sort.Descending(x => x.Code); break;
                case "Code": filter = Builders<Voucher>.Sort.Descending(x => x.Name); break;
                default: filter = Builders<Voucher>.Sort.Descending(x => x.Id); break;
            }
        }
        else
        {
            switch (sortBy)
            {
                case "CreateDate": filter = Builders<Voucher>.Sort.Ascending(x => x.CreatedDate); break;
                case "StartDate": filter = Builders<Voucher>.Sort.Ascending(x => x.StartDay); break;
                case "EndDate": filter = Builders<Voucher>.Sort.Ascending(x => x.ExpireDay); break;
                case "Name": filter = Builders<Voucher>.Sort.Ascending(x => x.Name); break;
                case "Code": filter = Builders<Voucher>.Sort.Ascending(x => x.Code); break;
                default: filter = Builders<Voucher>.Sort.Ascending(x => x.Id); break;
            }
        }
        return filter;
    }

    public async Task<VoucherPagingResponse> GetPagingStoreVoucher(GetPagingVoucherDto dto, string storeID, string status)
    {
        if (status != "Expired" && status != "Using") status = "";
        var filter = CreateStoreFilter(dto.StartDate, dto.EndDate, storeID, status, dto.QueryString, dto.QueryType);
        if (dto.PageNumber <= 0 || dto.PageSize <= 0)
        {
            throw new Exception("Pagenumber or pagesize can not be  zero or negative");
        }
        var rs = await voucherRepository.GetPagingAsync(filter, dto.PageNumber - 1, dto.PageSize);
        var list = new List<VoucherCardDto>();
        foreach (var voucher in rs.Data)
        {
            list.Add(new VoucherCardDto()
            {
                MaxDiscount = voucher.MaxDiscount,
                MinSpend = voucher.MinSpend,
                Amount = voucher.Amount,
                Description = voucher.Description,
                Name = voucher.Name,
                VoucherId = voucher.Id,
                CreateDate = voucher.CreatedDate!.Value,
                StartDate = voucher.StartDay,
                EndDate = voucher.ExpireDay,
                Code = voucher.Code,
                Type = voucher.Type
            });
        }
        switch (dto.SortBy)
        {
            case "CreateDate": list = list.OrderBy(x => x.CreateDate).ToList(); break;
            case "StartDate": list = list.OrderBy(x => x.StartDate).ToList(); break;
            case "EndDate": list = list.OrderBy(x => x.EndDate).ToList(); break;
            case "Name": list = list.OrderBy(x => x.Name).ToList(); break;
            case "Code": list = list.OrderBy(x => x.Code).ToList(); break;
        }
        switch (dto.SortType)
        {
            case "Desc": list.Reverse(); break;
        }
        var info = new VoucherPagingResponse()
        {
            Total = rs.Count,
            PageIndex = dto.PageNumber,
            PageSize = dto.PageSize,
            Cards = list
        };
        return info;

    }

    private FilterDefinition<Voucher> CreateStoreFilter(DateTime? startDate, DateTime? endDate, string storeID, string status, string query, string queryType)
    {
        var filter = Builders<Voucher>.Filter.Eq(x => x.StoreId, storeID);
        if (status == "Expired") filter &= Builders<Voucher>.Filter.Lte(x => x.ExpireDay, DateTime.Now);
        if (status == "Using") filter &= Builders<Voucher>.Filter.Gte(x => x.ExpireDay, DateTime.Now);
        if (startDate != null && endDate != null)
        {
            filter &= Builders<Voucher>.Filter.Gte(x => x.StartDay, startDate);
            filter &= Builders<Voucher>.Filter.Lte(x => x.ExpireDay, endDate);
        }
        if (queryType != "All")
        {
            switch (queryType)
            {
                case "Code": filter &= Builders<Voucher>.Filter.Where(x => x.Code.Contains(query)); break;
                case "Name": filter &= Builders<Voucher>.Filter.Where(x => x.Name.Contains(query)); break;
                default: filter &= Builders<Voucher>.Filter.StringIn(x => x.Code, query); break;
            }
        }
        return filter;
    }

    public async Task<UpdateVoucherDto> GetVoucherByID(string id)
    {
        Voucher voucher = await voucherRepository.FindByIdAsync(id);

        UpdateVoucherDto updateVoucherDto = new()
        {
            VoucherId = voucher.Id,
            Name = voucher.Name,
            Description = voucher.Description,
            MinSpend = voucher.MinSpend,
            Amount = voucher.Amount,
            MaxDiscount = voucher.MaxDiscount,
            StartDay = voucher.StartDay,
            ExpireDay = voucher.ExpireDay,
            StoreId = voucher.StoreId,
            Code = voucher.Code
        };

        return updateVoucherDto;
    }

    public async Task UpdateSystemVoucherAsync(UpdateVoucherDto dto)
    {
        if (dto.ExpireDay < DateTime.Now || dto.StartDay < dto.StartDay)
        {
            throw new Exception("Unexecpted Datetime");
        }

        if (dto.Amount <= 0 || dto.MaxDiscount <= 0 || dto.MinSpend < 0)
        {
            throw new Exception("Money can't not be negative");
        }
        var voucher = await voucherRepository.FindByIdAsync(dto.VoucherId);
        if (voucher != null && voucher.Type)
        {
            voucher.Name = dto.Name;
            voucher.Amount = dto.Amount;
            voucher.StoreId = "System";
            voucher.StartDay = dto.StartDay;
            voucher.ExpireDay = dto.ExpireDay;
            voucher.Code = dto.Code;
            voucher.Description = dto.Description;
            voucher.MaxDiscount = dto.MaxDiscount;
            voucher.MinSpend = dto.MinSpend;
            await voucherRepository.UpdateAsync(voucher);
        }
        else
        {
            throw new Exception("Voucher is not exist");
        }
    }
}

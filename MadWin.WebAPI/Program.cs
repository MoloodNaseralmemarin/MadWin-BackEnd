
using MadWin.Application.Repositories;
using MadWin.Application.Services;
using MadWin.Core.Convertors;
using MadWin.Core.Interfaces;
using MadWin.Core.Settings;
using MadWin.Infrastructure.Data;
using MadWin.Infrastructure.Repositories;
using MadWin.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Shop2City.Core.Services.Permissions;
using Shop2City.Core.Services.ProductImages;
using Shop2City.Core.Services.Products;
using Shop2City.Core.Services.Properties;
using Shop2City.Core.Services.PropertyTechnicalProducts;
using Shop2City.Core.Services.PropertyTechnicals;
using Shop2City.Core.Services.PropertyTitles;
using Shop2City.Core.Services.Transactions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ────── Database Context ──────
builder.Services.AddDbContext<MadWinDBContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Production");
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserPanelService, UserPanelService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderWidthPartRepository, OrderWidthPartRepository>();
builder.Services.AddScoped<ICurtainComponentProductGroupRepository, CurtainComponentProductGroupRepository>();
builder.Services.AddScoped<ICurtainComponentDetailService, CurtainComponentDetailService>();
builder.Services.AddScoped<ICurtainComponentDetailRepository, CurtainComponentDetailRepository>();
builder.Services.AddScoped<IDeliveryMethodRepository, DeliveryMethodRepository>();
builder.Services.AddScoped<IDeliveryMethodService, DeliveryMethodService>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IFactorRepository, FactorRepository>();
builder.Services.AddScoped<IFactorService, FactorService>();
builder.Services.AddScoped<IFactorDetailRepository, FactorDetailRepository>();
builder.Services.AddScoped<IFactorDetailService, FactorDetailService>();
builder.Services.AddScoped<IUserDiscountCodeRepository, UserDiscountCodeRepository>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductGroupRepository, ProductGroupRepository>();
builder.Services.AddScoped<IProductGroupService, ProductGroupService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<IPropertyTechnicalProductService, PropertyTechnicalProductService>();
builder.Services.AddScoped<ICommissionRateRepository, CommissionRateRepository>();
builder.Services.AddScoped<ICurtainComponentRepository, CurtainComponentRepository>();
builder.Services.AddScoped<ICommissionRatesService, CommissionRatesService>();
builder.Services.AddScoped<ICurtainComponentService, CurtainComponentService>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IPropertyTitleService, PropertyTitleService>();
builder.Services.AddScoped<IPropertyTechnicalService, PropertyTechnicalService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ISmsRepository, SmsRepository>();
builder.Services.AddScoped<ISmsSenderService, SmsSenderService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IReportService, ReportService>();
// ────── Settings Bindings ──────
builder.Services.Configure<SmsSettings>(builder.Configuration.GetSection("SmsSettings"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MadWin API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

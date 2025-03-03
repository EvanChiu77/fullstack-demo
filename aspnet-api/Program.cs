using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;
using IBuyPower.Demo.Data;
using IBuyPower.Demo.Models;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

// 1. 註冊 EF Core，使用 MSSQL 連線字串
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. 註冊 Redis 連線 (單一連線)
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    // 連線docker-compose中的Redis，此處使用服務名稱 "redis" 與預設 port 6379
    var configuration = ConfigurationOptions.Parse("redis:6379", true);
    return ConnectionMultiplexer.Connect(configuration);
});

// 新增 CORS 服務設定
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowLocalhost3000");

//-----------------------------------------------
//  CRUD API
//-----------------------------------------------

// GET /api/products-取得所有產品，並使用Redis
app.MapGet("/api/products", async (AppDbContext db, IConnectionMultiplexer redis) =>
{
    var cache = redis.GetDatabase();
    string cacheKey = "products";

    //嘗試從Redis取得快取資料
    var cachedProducts = await cache.StringGetAsync(cacheKey);
    if (!cachedProducts.IsNullOrEmpty)
    {
        var products = JsonSerializer.Deserialize<List<Product>>(cachedProducts);
        return Results.Ok(products);
    }

    //若快取無資料，則從MSSQL取得資料
    var dbProducts = await db.Products.ToListAsync();

    //將資料快取五分鐘
    await cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(dbProducts), TimeSpan.FromMinutes(5));

    return Results.Ok(dbProducts);
}).AddLoggingFilter();

// GET /api/products/{id} - 取得單筆產品
app.MapGet("/api/products/{id}", async (AppDbContext db, int id) =>
{
    var product = await db.Products.FindAsync(id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
}).AddLoggingFilter();

// POST /api/products - 新增產品
app.MapPost("/api/products", async (AppDbContext db, Product product, IConnectionMultiplexer redis) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();

    // 新增完，刪除快取(1)
    var cache = redis.GetDatabase();
    await cache.KeyDeleteAsync("products");

    return Results.Created($"/api/products/{product.Id}", product);
}).AddLoggingFilter();

// PUT /api/products/{id} - 更新產品
app.MapPut("/api/products/{id}", async (AppDbContext db, int id, Product updatedProduct, IConnectionMultiplexer redis) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null)
        return Results.NotFound();

    product.Name = updatedProduct.Name;
    product.Price = updatedProduct.Price;
    await db.SaveChangesAsync();

    // 更新完，重新查詢最新產品清單(2)
    var products = await db.Products.ToListAsync();
    var cache = redis.GetDatabase();
    await cache.StringSetAsync("products", JsonSerializer.Serialize(products), TimeSpan.FromMinutes(5));


    return Results.NoContent();
}).AddLoggingFilter();

// DELETE /api/products/{id} - 刪除產品
app.MapDelete("/api/products/{id}", async (AppDbContext db, int id, IConnectionMultiplexer redis) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null)
        return Results.NotFound();

    db.Products.Remove(product);
    await db.SaveChangesAsync();

    // 刪除完，刪除快取
    var cache = redis.GetDatabase();
    await cache.KeyDeleteAsync("products");

    return Results.NoContent();
}).AddLoggingFilter();

// ---------------------
// 購買 API Redis Test
// ---------------------

// POST /api/purchase/{id} - 購買產品
app.MapPost("/api/purchase/{id}", async (AppDbContext db, IConnectionMultiplexer redis, int id) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null)
        return Results.NotFound("Product not found");

    if (product.Quantity <= 0)
    {
        // 如果庫存不足，發布 Pub/Sub 訊息告知前端「已完售」
        var sub = redis.GetSubscriber();
        await sub.PublishAsync("productSoldOut", $"Product {id} sold out");
        return Results.BadRequest("Product sold out");
    }

    // 減少庫存並更新資料庫
    product.Quantity -= 1;
    await db.SaveChangesAsync();

    // 清除 Redis 快取以確保下次 GET 取得最新資料
    var cache = redis.GetDatabase();
    await cache.KeyDeleteAsync("products");

    return Results.Ok(product);
}).AddLoggingFilter();

app.Run();
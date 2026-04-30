using CebuCrmApi.Data;
using System.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. 【修改】動態判斷 SQLite 資料庫路徑
// 如果是開發環境 (本機)，存在當前目錄；如果是正式環境 (Render)，存在持久化硬碟目錄
var dbPath = builder.Environment.IsDevelopment()
    ? "cebucrm.db"
    : "/var/data/cebucrm.db";

builder.Services.AddDbContext<CrmDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddControllers();

// 註冊 YARP 服務
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// ... 前面的 builder 設定 (AddControllers, DbContext 等) ...

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 【修正 1】合併並簡化 CORS 設定
builder.Services.AddCors(options =>
{
    // 我們建立一個統一的 Policy 叫做 "AllowFrontend"
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173",
                "https://ceub-crm.onrender.com" // 你的 Render 網址
              )
              .AllowAnyHeader()
              .AllowAnyMethod();

        // 如果你在開發階段真的不想管 Port，想完全開放 (不建議放上線用)：
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// --- 應用程式初始化與 Middleware 設定 ---

// 【初始化資料庫與測試資料】
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CrmDbContext>();

    //context.Database.EnsureDeleted();
    MarkPropertyCategoryMigrationIfColumnAlreadyExists(context);
    context.Database.Migrate();
    // 記得：確保你已經執行過 dotnet ef database update 了！
    DbInitializer.Initialize(context);
}

// 【Swagger 設定】 (強制開啟，方便 Render 上也能看 API)
app.UseSwagger();
app.UseSwaggerUI();

// 【修正 2】確保 CORS 在正確的位置！
// 必須放在 UseAuthorization 和 MapControllers 之前
app.UseCors("AllowFrontend");

app.UseAuthorization();

// cebu-crm 本身的 API 路由
app.MapControllers();

// 啟動 YARP 反向代理 (如果有設定的話)
app.MapReverseProxy(); 

app.Run();
static void MarkPropertyCategoryMigrationIfColumnAlreadyExists(CrmDbContext context)
{
    const string migrationId = "20260429000100_AddPropertyCategory";
    const string productVersion = "10.0.3";

    var connection = context.Database.GetDbConnection();
    var shouldClose = connection.State != ConnectionState.Open;

    if (shouldClose)
    {
        connection.Open();
    }

    try
    {
        if (!TableExists(connection, "__EFMigrationsHistory") || !TableExists(connection, "Properties"))
        {
            return;
        }

        if (!ColumnExists(connection, "Properties", "Category") || MigrationRecorded(connection, migrationId))
        {
            return;
        }

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
            VALUES ($migrationId, $productVersion);
            """;

        var migrationParameter = command.CreateParameter();
        migrationParameter.ParameterName = "$migrationId";
        migrationParameter.Value = migrationId;
        command.Parameters.Add(migrationParameter);

        var versionParameter = command.CreateParameter();
        versionParameter.ParameterName = "$productVersion";
        versionParameter.Value = productVersion;
        command.Parameters.Add(versionParameter);

        command.ExecuteNonQuery();
    }
    finally
    {
        if (shouldClose)
        {
            connection.Close();
        }
    }
}

static bool TableExists(System.Data.Common.DbConnection connection, string tableName)
{
    using var command = connection.CreateCommand();
    command.CommandText = """
        SELECT COUNT(*)
        FROM "sqlite_master"
        WHERE "type" = 'table' AND "name" = $tableName;
        """;

    var parameter = command.CreateParameter();
    parameter.ParameterName = "$tableName";
    parameter.Value = tableName;
    command.Parameters.Add(parameter);

    return Convert.ToInt32(command.ExecuteScalar()) > 0;
}

static bool ColumnExists(System.Data.Common.DbConnection connection, string tableName, string columnName)
{
    using var command = connection.CreateCommand();
    command.CommandText = $"PRAGMA table_info(\"{tableName}\");";

    using var reader = command.ExecuteReader();
    while (reader.Read())
    {
        if (string.Equals(reader["name"]?.ToString(), columnName, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
    }

    return false;
}

static bool MigrationRecorded(System.Data.Common.DbConnection connection, string migrationId)
{
    using var command = connection.CreateCommand();
    command.CommandText = """
        SELECT COUNT(*)
        FROM "__EFMigrationsHistory"
        WHERE "MigrationId" = $migrationId;
        """;

    var parameter = command.CreateParameter();
    parameter.ParameterName = "$migrationId";
    parameter.Value = migrationId;
    command.Parameters.Add(parameter);

    return Convert.ToInt32(command.ExecuteScalar()) > 0;
}


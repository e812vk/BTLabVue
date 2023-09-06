using webapi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddSingleton<DataAccessor>();

builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "BTLabVue.log"));
var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
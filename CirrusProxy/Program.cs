const string CLIENT_ENDPOINT = @"http://localhost:5002";
const string SERVICE_ENDPOINT = "http://localhost:38223/api/SmartContracts/";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        b =>
        {
            b.WithOrigins(CLIENT_ENDPOINT).AllowAnyMethod().AllowAnyHeader();
        });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(s => new HttpClient() { BaseAddress = new Uri(SERVICE_ENDPOINT) });
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.Run();

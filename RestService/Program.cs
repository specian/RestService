using RestService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<RestServiceContext>(options => 
	options.UseSqlServer(builder.Configuration.GetConnectionString("Argal")));
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IRestServiceRepo, SqlRestServiceRepo>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();

using GdPicture14.WEB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

DocuViewareLicensing.RegisterKEY("");
// DocuVieware Core Configuration
DocuViewareManager.SetupConfiguration(true, DocuViewareSessionStateMode.File, Path.Combine(Directory.GetCurrentDirectory(), "Cache"), "/", "api/docuvieware3");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

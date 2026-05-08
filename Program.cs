using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Source - https://stackoverflow.com/a/59096114
// Posted by Andrei, modified by community. See post 'Timeline' for change history
// Retrieved 2026-05-08, License - CC BY-SA 4.0
// allows API consumers to use string enum values instead of raw ints
builder.Services.AddMvc().AddJsonOptions(opts =>
{
    var enumConverter = new JsonStringEnumConverter(JsonNamingPolicy.CamelCase);
    opts.JsonSerializerOptions.Converters.Add(enumConverter); 
});

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

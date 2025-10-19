using FitApi.Api;

var builder = WebApplication.CreateBuilder(args);
builder.InjectDependencies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.MapControllers();
app.UseHttpsRedirection();
app.Run();

public partial class Program;
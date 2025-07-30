using EMailSender;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Email Sending API", Version = "v1" });
    
    // sendAt alanı için özel format
    c.MapType<DateTime>(() => new Microsoft.OpenApi.Models.OpenApiSchema { Type = "string", Format = "date-time" });
    
    // XML documentation'ı dahil et
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Register EmailSender service
builder.Services.AddScoped<EmailSender>();

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

// EmailHelper test endpoint'i
app.MapGet("/test-emailhelper", () =>
{
    // Console'a yazacak, tarayıcıda görmek için response olarak da döndürüyoruz
    EmailHelperExamples.RunExamples();
    
    return Results.Ok(new 
    { 
        message = "EmailHelper örnekleri çalıştırıldı. Konsol çıktısını kontrol edin.",
        timestamp = DateTime.UtcNow 
    });
});

app.Run(); 
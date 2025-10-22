using System.Xml.Serialization;
using System.IO;
using Microsoft.AspNetCore.Mvc;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//Uso de ChatGPT
app.UseHttpsRedirection();

var list = new List<object>();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapPost("/", (HttpContext context) =>
{
    bool xml = false;
    if (context.Request.Headers.TryGetValue("xml", out var xmlHeader))
    {
        bool.TryParse(xmlHeader.ToString(), out xml);
    }
    if (xml)
    {
        var xmlSerializer = new XmlSerializer(typeof(List<object>));
        using var stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, list);
        return Results.Content(stringWriter.ToString(), "application/xml");
    }
    return Results.Ok(list);

});

app.MapPut("/", ([FromForm] int quantity, [FromForm] string type) =>
{
    if (quantity <= 0)
    {
        return Results.BadRequest(new { error = "La cantidad debe ser superior a cero" });
    }
    type = type.ToLower();
    if (type != "int" && type != "float")
    {
        return Results.BadRequest(new { error = "El tipo debe ser 'int' o 'float'" });
    }
    var random = new Random();
    if (type == "int")
    {
        for (int i = 0; i < quantity; i++)
        {
            list.Add(random.Next());
        }
    }
    else 
    {
        for (int i = 0; i < quantity; i++)
        {
            list.Add(random.NextSingle());
        }
    }
    return Results.Ok(list);
}).DisableAntiforgery();

app.MapDelete("/", ([FromForm] int quantity) =>
{
    if (quantity <= 0)
    {
        return Results.BadRequest(new { error = "Por favor. La cantidad debe ser mayor a cero" });
    }
    if (list.Count < quantity)
    {
        return Results.BadRequest(new { error = $"La lista solo tiene {list.Count} elementos, no se pueden eliminar {quantity}" });
    }
    for (int i = 0; i < quantity; i++)
    {
        list.RemoveAt(0);
    }
    return Results.Ok(list);
}).DisableAntiforgery();

app.MapPatch("/", () =>
{
    list.Clear();
    return Results.Ok(list);
});

app.Run();


using System.Text;
using System.Xml.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var aplicacion = builder.Build();

aplicacion.UseSwagger();
aplicacion.UseSwaggerUI();

aplicacion.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

aplicacion.MapPost("/include/{position}", async (
    int position,
    string value,
    HttpContext contexto) =>
{
    if (position < 0)
    {
        return Results.BadRequest(new { error = "'position' Por favor, debe ser 0 o mayor" });
    }

    if (string.IsNullOrWhiteSpace(value))
    {
        return Results.BadRequest(new { error = "'value' Por favor, no puede estar vacío" });
    }

    var formulario = await contexto.Request.ReadFormAsync();
    var texto = formulario["text"].ToString();

    if (string.IsNullOrWhiteSpace(texto))
    {
        return Results.BadRequest(new { error = "'text' Por favor, no puede estar vacío" });
    }

    var encabezadoXml = contexto.Request.Headers["xml"].ToString();
    var esXml = !string.IsNullOrEmpty(encabezadoXml) && bool.Parse(encabezadoXml);

    var palabras = texto.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var listaPalabras = palabras.ToList();

    if (position > listaPalabras.Count)
    {
        listaPalabras.Add(value);
    }
    else
    {
        listaPalabras.Insert(position, value);
    }

    var textoNuevo = string.Join(" ", listaPalabras);

    if (esXml)
    {
        var resultado = new Resultado { Ori = texto, New = textoNuevo };
        return Results.Content(SerializarAXml(resultado), "application/xml");
    }

    return Results.Ok(new { ori = texto, @new = textoNuevo });
})
.WithName("Include")
.WithOpenApi();

aplicacion.MapPut("/replace/{length}", async (
    int length,
    string value,
    HttpContext contexto) =>
{
    if (length <= 0)
    {
        return Results.BadRequest(new { error = "'length' Por favor, debe ser mayor que 0" });
    }

    if (string.IsNullOrWhiteSpace(value))
    {
        return Results.BadRequest(new { error = "'value' Por favor, no puede estar vacío" });
    }

    var formulario = await contexto.Request.ReadFormAsync();
    var texto = formulario["text"].ToString();

    if (string.IsNullOrWhiteSpace(texto))
    {
        return Results.BadRequest(new { error = "'text' Por favor, no puede estar vacío" });
    }

    var encabezadoXml = contexto.Request.Headers["xml"].ToString();
    var esXml = !string.IsNullOrEmpty(encabezadoXml) && bool.Parse(encabezadoXml);

    var palabras = texto.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var palabrasNuevas = palabras.Select(palabra =>
    {
        var palabraLimpia = new string(palabra.Where(char.IsLetterOrDigit).ToArray());
        return palabraLimpia.Length == length ? value : palabra;
    }).ToArray();

    var textoNuevo = string.Join(" ", palabrasNuevas);

    if (esXml)
    {
        var resultado = new Resultado { Ori = texto, New = textoNuevo };
        return Results.Content(SerializarAXml(resultado), "application/xml");
    }

    return Results.Ok(new { ori = texto, @new = textoNuevo });
})
.WithName("Replace")
.WithOpenApi();

aplicacion.MapDelete("/erase/{length}", async (
    int length,
    HttpContext contexto) =>
{
    if (length <= 0)
    {
        return Results.BadRequest(new { error = "'length' Por favor, debe ser mayor que 0" });
    }

    var formulario = await contexto.Request.ReadFormAsync();
    var texto = formulario["text"].ToString();

    if (string.IsNullOrWhiteSpace(texto))
    {
        return Results.BadRequest(new { error = "'text' Por favor, no puede estar vacío" });
    }

    var encabezadoXml = contexto.Request.Headers["xml"].ToString();
    var esXml = !string.IsNullOrEmpty(encabezadoXml) && bool.Parse(encabezadoXml);

    var palabras = texto.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var palabrasFiltradas = palabras.Where(palabra =>
    {
        var palabraLimpia = new string(palabra.Where(char.IsLetterOrDigit).ToArray());
        return palabraLimpia.Length != length;
    }).ToArray();

    var textoNuevo = string.Join(" ", palabrasFiltradas);

    if (esXml)
    {
        var resultado = new Resultado { Ori = texto, New = textoNuevo };
        return Results.Content(SerializarAXml(resultado), "application/xml");
    }

    return Results.Ok(new { ori = texto, @new = textoNuevo });
})
.WithName("Erase")
.WithOpenApi();

aplicacion.Run();

static string SerializarAXml(Resultado resultado)
{
    var serializador = new XmlSerializer(typeof(Resultado));
    using var escritor = new StringWriter();
    serializador.Serialize(escritor, resultado);
    return escritor.ToString();
}

[XmlRoot("Result")]
public class Resultado
{
    [XmlElement("Ori")]
    public string Ori { get; set; } = string.Empty;

    [XmlElement("New")]
    public string New { get; set; } = string.Empty;
}

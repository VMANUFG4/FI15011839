# Tarea Programada 3 - TP3 Minimal API

**Nombre:** Victor Fuentes Gonzalez  
**Carné:** FI15011839  

---

## Comandos de dotnet utilizados (CLI).

**Respuesta:**
mkdir PP3, cd PP3, dotnet new sln -n PracticaProgramada3, dotnet new web -n PracAPI -f net8.0, dotnet sln add PracAPI/PracAPI.csproj, cd PracAPI, dotnet build, dotnet run

## Recursos Consultados

- https://chat.openai.com/

---

## Prompts utilizados

### ChatGPT

**Prompt:**  
Ocupo poder validar parámetros en mi endpoint, en este caso números negativos, strings vacíos, etc., y poder retornar un status 400 Bad Request con un mensaje JSON personalizado {  "error":  "mensaje"  }. Por lo tanto ocupo saber como se hace esto con Minimal API.

**Respuesta:**  
Usa Results.BadRequest() con objetos anónimos y retorna temprano (early return):
```csharp
aplicacion.MapPost("/crear/{id}", (int id, string nombre, int edad) =>
{
    // Validar id negativo
    if (id < 0)
        return Results.BadRequest(new { error = "El ID no puede ser negativo." });

    // Validar nombre vacío o nulo
    if (string.IsNullOrWhiteSpace(nombre))
        return Results.BadRequest(new { error = "El nombre es obligatorio." });

    // Validar edad inválida
    if (edad <= 0)
        return Results.BadRequest(new { error = "La edad debe ser mayor que cero." });

    // Si las validaciones son correctas
    return Results.Ok(new { mensaje = "Registro creado correctamente." });
});
```
Results.BadRequest() automáticamente establece el status code 400. El objeto anónimo new { error = "..." } se serializa a JSON con la estructura que necesitas.
Puedes usar string.IsNullOrWhiteSpace() para validar strings, ya que considera tanto nulos, vacíos, o solo espacios en blanco.

---

### ChatGPT

**Prompt:**  
Ocupo que me ayudes con lo siguiente: ocupo buscar todas las palabras de una longitud específica en una oración, pero ojo, ocupo que sea ignorando la puntuación. Para darte más contexto, tenemos 'hola,' y 'chao'. Como puedes ver, las 2 tienen 4 letras, pero mi pregunta es: ¿cómo puedo limpiar cada palabra de puntuación y luego poder filtrarlas o también reemplazarlas por su longitud real?  

**Respuesta:**  
Usa LINQ con Where() y char.IsLetterOrDigit() para limpiar la puntuación:
```csharp
string texto = "Hello, world! This is a test.";
int longitudBuscada = 4;

// Dividir el texto en palabras, eliminando espacios extra
var palabras = texto.Split(' ', StringSplitOptions.RemoveEmptyEntries);

// --- Filtrar palabras ---
var palabrasFiltradas = palabras
    .Where(p =>
    {
        // Quitar signos de puntuación y dejar solo letras o números
        var limpia = new string(p.Where(char.IsLetterOrDigit).ToArray());
        // Mantener las palabras que NO tienen la longitud especificada
        return limpia.Length != longitudBuscada;
    })
    .ToArray();

// --- Reemplazar palabras ---
var palabrasReemplazadas = palabras
    .Select(p =>
    {
        var limpia = new string(p.Where(char.IsLetterOrDigit).ToArray());
        // Reemplazar las que coincidan con la longitud buscada
        return limpia.Length == longitudBuscada ? "REEMPLAZO" : p;
    })
    .ToArray();

string resultado = string.Join(" ", palabrasFiltradas);
```
char.IsLetterOrDigit() identifica letras y números, ignorando puntuación, espacios y símbolos.

---

### ChatGPT

**Prompt:**  
Para un trabajo Minimal API en .NET que necesito retornar XML en mi API ya con una estructura específica, en mi clase C#, lo que busco es que el XML tenga nombres de elementos personalizados con los atributos que tú me recomiendes. Básicamente, necesito saber cómo serializo un objeto a XML y poder retornarlo con el content-type indicado.

**Respuesta:**
Para serializar a XML con nombres personalizados, debes usar XmlSerializer junto con los atributos de serialización XML:
```csharp  
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

// Definición de la clase con los atributos XML
[XmlRoot("Persona")]
public class Persona
{
    [XmlElement("Nombre")]
    public string Nombre { get; set; }

    [XmlElement("Edad")]
    public int Edad { get; set; }
}

// Método genérico para serializar cualquier objeto a XML
static string SerializarAXml<T>(T objeto)
{
    var serializador = new XmlSerializer(typeof(T));
    using var escritor = new StringWriter();
    serializador.Serialize(escritor, objeto);
    return escritor.ToString();
}

// Ejemplo de uso
var persona = new Persona { Nombre = "Ana", Edad = 30 };
string xmlGenerado = SerializarAXml(persona);

Console.WriteLine(xmlGenerado);
```
---

**Prompt:**
Si tengo un header HTTP que es opcional llamado 'xml' que puede ser true o false. En caso de que no venga, debe ser false por defecto. ¿Cómo puedo leerlo en C# y convertirlo a booleano? 

**Respuesta:**
Accede a los headers a través de HttpContext y valida antes de parsear:
```csharp
app.MapGet("/datos", (HttpContext contexto) =>
{
    // Leer el valor del encabezado "xml"
    var valorHeader = contexto.Request.Headers["xml"].ToString();

    // Intentar convertir el valor a bool de forma segura
    bool esXml = bool.TryParse(valorHeader, out var resultado) && resultado;

    // Si el header indica que se desea XML
    if (esXml)
    {
        var contenidoXml = "<Datos><Mensaje>Respuesta en formato XML</Mensaje></Datos>";
        return Results.Content(contenidoXml, "application/xml");
    }

    // Por defecto, devolver JSON
    return Results.Ok(new { formato = "JSON", mensaje = "Respuesta en formato JSON" });
});
```
La expresión !string.IsNullOrEmpty(encabezadoXml) && bool.Parse(encabezadoXml) evalúa primero si el header existe y no está vacío. Si es así, entonces intenta parsearlo. Si el header no existe, el && hace "short-circuit" y retorna false sin intentar parsear.

---

**Pregunta:**
¿Es posible enviar valores en el Body (por ejemplo, en el Form) del Request de tipo GET?

**Respuesta:**
No, las peticiones GET no permiten enviar datos en el cuerpo (body) del request.
Por diseño, este tipo de solicitud se usa únicamente para obtener información, no para enviarla.

---

**Pregunta:**
¿Qué ventajas y desventajas observa con el Minimal API si se compara con la opción de utilizar Controllers?

**Respuesta:**
Depende del tipo y tamaño del proyecto. Las Minimal APIs son una excelente opción para aplicaciones pequeñas o servicios que necesitan ser creados rápidamente, ya que su estructura es ligera, directa y requiere menos código y archivos que los Controllers tradicionales. Esto las hace más fáciles de mantener en proyectos simples o de prueba.
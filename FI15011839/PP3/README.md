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
    // Validar id
    if (id < 0)
    {
        return Results.BadRequest(new { error = "El ID no puede ser negativo" });
    }
    
    // Validar nombre
    if (string.IsNullOrWhiteSpace(nombre))
    {
        return Results.BadRequest(new { error = "El nombre es obligatorio" });
    }
    
    // Validar edad
    if (edad <= 0)
    {
        return Results.BadRequest(new { error = "La edad debe ser mayor a 0" });
    }
    
    // Si pasa todas las validaciones
    return Results.Ok(new { mensaje = "Creado exitosamente" });
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

var palabras = texto.Split(' ', StringSplitOptions.RemoveEmptyEntries);

// Para filtrar (eliminar)
var palabrasFiltradas = palabras.Where(palabra =>
{
    // Crear versión limpia sin puntuación
    var palabraLimpia = new string(palabra.Where(char.IsLetterOrDigit).ToArray());
    return palabraLimpia.Length != longitudBuscada; // mantener las que NO tienen esa longitud
}).ToArray();

// Para reemplazar
var palabrasReemplazadas = palabras.Select(palabra =>
{
    var palabraLimpia = new string(palabra.Where(char.IsLetterOrDigit).ToArray());
    return palabraLimpia.Length == longitudBuscada ? "REEMPLAZO" : palabra;
}).ToArray();

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
using System.Xml.Serialization;
using System.Text;

// Define tu clase con atributos XML
[XmlRoot("Persona")]
public class Persona
{
    [XmlElement("Nombre")]
    public string Nombre { get; set; }
    
    [XmlElement("Edad")]
    public int Edad { get; set; }
}

// Método para serializar
static string SerializarAXml<T>(T objeto)
{
    var serializador = new XmlSerializer(typeof(T));
    using var escritor = new StringWriter();
    serializador.Serialize(escritor, objeto);
    return escritor.ToString();
}
```
---

**Prompt:**
Si tengo un header HTTP que es opcional llamado 'xml' que puede ser true o false. En caso de que no venga, debe ser false por defecto. ¿Cómo puedo leerlo en C# y convertirlo a booleano? 

**Respuesta:**
Accede a los headers a través de HttpContext y valida antes de parsear:
```csharp
aplicacion.MapGet("/datos", (HttpContext contexto) =>
{
    // Leer el header
    var encabezadoXml = contexto.Request.Headers["xml"].ToString();
    
    // Validar y convertir (false por defecto)
    var esXml = !string.IsNullOrEmpty(encabezadoXml) && bool.Parse(encabezadoXml);
    
    if (esXml)
    {
        return Results.Content("<Datos>XML</Datos>", "application/xml");
    }
    
    return Results.Ok(new { formato = "JSON" });
});
```
La expresión !string.IsNullOrEmpty(encabezadoXml) && bool.Parse(encabezadoXml) evalúa primero si el header existe y no está vacío. Si es así, entonces intenta parsearlo. Si el header no existe, el && hace "short-circuit" y retorna false sin intentar parsear.

---

**Pregunta:**
¿Es posible enviar valores en el Body (por ejemplo, en el Form) del Request de tipo GET?

**Respuesta:**
 No, en los métodos GET no se puede mandar información en el body; lo normal es mandar los datos por la URL.

---

**Pregunta:**
¿Qué ventajas y desventajas observa con el Minimal API si se compara con la opción de utilizar Controllers?

**Respuesta:**
Depende, ya que para proyectos que no requieran tanto trabajo (es decir, pequeños), el Minimal API es más sencillo y rápido porque va directo al punto sin necesidad de crear tantas clases. Diría que, en fin, depende de qué tanto se proyecte que va a crecer el proyecto. Para proyectos grandes, personalmente usaría Controllers, pero para proyectos pequeños usaría Minimal API.
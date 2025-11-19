# Tarea Programada 4 - TP4 Books

**Nombre:** Victor Fuentes Gonzalez 
**Carnet:** FI15011839 

---

## Comandos de dotnet utilizados (CLI).

**Respuesta:**
mkdir PP4, cd PP4, dotnet new sln -n PracticaProgramada4, dotnet new console -n PPLibros -f net8.0, dotnet sln add PPLibros/PPLibros.csproj, mkdir Models, mkdir Data, dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 9.0.0, dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0, dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.0, dotnet add package CsvHelper, dotnet list package, dotnet restore, dotnet build, dotnet ef migrations add InitialCreate, dotnet ef database update, dotnet run

## Recursos Consultados

- https://chat.openai.com/

---

## Prompts utilizados

### ChatGPT

**Prompt:**  
Me ayudas con un método para configurar un DbContext para utilizar una base de datos SQLite books.db dentro de su respectiva carpeta Data, con la estrategia de Code First .NET 8.

**Respuesta:**  
Usa Results.BadRequest() con objetos anónimos y retorna temprano (early return):
```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlite("Data Source=Data/books.db");
}
```
El método OnConfiguring permite definir la configuración de conexión para el contexto de Entity Framework.

---

### ChatGPT

**Prompt:**  
Me colaboras, ocupo saber cómo puedo definir el orden en que se crean las columnas de una tabla usando Fluent API en Entity Framework Core?  

**Respuesta:**  
En Entity Framework Core, se puede controlar el orden en el que las columnas aparecen en una tabla mediante el método .HasColumnOrder(), dentro del método OnModelCreating.
Este método forma parte de la configuración Fluent API y permite asignar un número que representa la posición que ocupará cada columna dentro de la tabla en la base de datos.
El valor asignado indica la prioridad de aparición:

Un número menor representa una columna que se mostrará antes.

Un número mayor representa una columna que aparecerá después.

Esta práctica se utiliza principalmente para mantener una estructura ordenada y coherente en la base de datos, facilitando su lectura y asegurando que las columnas sigan un orden lógico definido por el desarrollador.

---

### ChatGPT

**Prompt:**  
¿Cómo puedo hacer que mi programa de console net8.0 cargue los datos desde un archivo CSV la primera vez que se ejecuta y, en las siguientes ejecuciones, genere archivos TSV a partir de la base de datos?
---
**Respuesta:**
Para lograrlo, se pueden invocar dos métodos distintos dependiendo del estado de la base de datos:
uno encargado de la carga inicial y otro de la generación de archivos.

Por ejemplo:
LoadDataFromCsv(context);
GenerateTsvFiles(context);

El primero se ejecuta cuando la base de datos está vacía y necesita llenarse con la información del archivo CSV.
El segundo se ejecuta cuando ya existen registros, permitiendo generar los archivos TSV con base en los datos almacenados.

---

**Prompt:**
Para dividir una cadena de texto con etiquetas separadas por “|” y quitar los espacios que sobran en C#, pero ojo, antes de guardarlas en la base de datos. 
---
**Respuesta:**
```csharp
var partes = texto.Split('|');
foreach (var item in partes)
{
    var limpio = item.Trim();
    // Guardar en la base de datos
}
```
---

**Prompt:**
¿Cómo se puede generar un archivo TSV desde los datos de una base de datos usando C#?

**Respuesta:**
```csharp
using var writer = new StreamWriter("ruta/datos.tsv");
writer.WriteLine("Columna1\tColumna2\tColumna3");
```

---

**Prompt:**
Me ayudas con el código para configurar la lectura de un archivo CSV para que reconozca los encabezados y elimine los espacios en blanco, utilizando C#?

**Respuesta:**
```csharp
var config = new CsvConfiguration(CultureInfo.InvariantCulture)
{
    HasHeaderRecord = true,
    TrimOptions = TrimOptions.Trim
};
```

---

**Prompt:**
Si ocupo crear un archivo TSV por grupo de datos y escribir los encabezados antes del contenido, me ayudas con el código en C#.
---
**Respuesta:**
```csharp
using var writer = new StreamWriter(rutaArchivo);
writer.WriteLine("Columna1\tColumna2\tColumna3");
```
---

**Prompt:**
Utilizando Entity Framework, ¿cómo se puede obtener los datos de una entidad con sus relaciones con el fin de evitar múltiples consultas?

**Respuesta:**
```csharp
var datos = contexto.Entidades
    .Include(e => e.Relacion1)
    .Include(e => e.Relacion2)
    .ThenInclude(r => r.SubRelacion)
    .ToList();
```
---

**Pregunta:**
¿Cómo cree que resultaría el uso de la estrategia de Code First para crear y actualizar una base de datos de tipo NoSQL (como por ejemplo MongoDB)? ¿Y con Database First? ¿Cree que habría complicaciones con las Foreign Keys?
---
**Respuesta:**
 En este caso, Code First y Database First no van de la mejor manera con NoSQL, ya que no hay tablas ni relaciones; por lo tanto, sí habría problemas con las Foreign Keys.

---

**Pregunta:**
¿Cuál carácter, además de la coma (,) y el Tab (\t), se podría usar para separar valores en un archivo de texto con el objetivo de ser interpretado como una tabla (matriz)? ¿Qué extensión le pondría y por qué? Por ejemplo: Pipe (|) con extensión .pipe.
---
**Respuesta:**
Sería el carácter | y la extensión .pipe, más que todo porque es poco usado en los datos.
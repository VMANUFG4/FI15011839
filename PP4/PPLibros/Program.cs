using PPLibros.Data;
using PPLibros.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace PPLibros
{
    class Program
    {
        static void Main(string[] args)
        {
            using var context = new BooksContext();

            if (!context.Authors.Any())
            {
                Console.WriteLine("La base de datos está vacía, por lo que será llenada a partir de los datos del archivo CSV.\n");
                Console.Write("Procesando... ");
                LoadDataFromCsv(context);
                Console.WriteLine("Listo.\n");
            }
            else
            {
                Console.WriteLine("La base de datos se está leyendo para crear los archivos TSV.\n");
                Console.Write("Procesando... ");
                GenerateTsvFiles(context);
                Console.WriteLine("Listo.\n");
            }
        }

        static void LoadDataFromCsv(BooksContext context)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim
            };

            using var reader = new StreamReader("Data/books.csv");
            using var csv = new CsvReader(reader, config);

            var records = csv.GetRecords<dynamic>().ToList();

            foreach (var record in records)
            {
                string authorName = record.Author;
                string titleName = record.Title;
                string tagsString = record.Tags;

                var author = context.Authors.FirstOrDefault(a => a.AuthorName == authorName);
                if (author == null)
                {
                    author = new Author { AuthorName = authorName };
                    context.Authors.Add(author);
                    context.SaveChanges();
                }

                var title = new Title
                {
                    TitleName = titleName,
                    AuthorId = author.AuthorId
                };
                context.Titles.Add(title);
                context.SaveChanges();

                var tagNames = tagsString.Split('|');
                foreach (var tagName in tagNames)
                {
                    var trimmedTagName = tagName.Trim();

                    var tag = context.Tags.FirstOrDefault(t => t.TagName == trimmedTagName);
                    if (tag == null)
                    {
                        tag = new Tag { TagName = trimmedTagName };
                        context.Tags.Add(tag);
                        context.SaveChanges();
                    }

                    var titleTag = new TitleTag
                    {
                        TitleId = title.TitleId,
                        TagId = tag.TagId
                    };
                    context.TitleTags.Add(titleTag);
                }

                context.SaveChanges();
            }
        }

        static void GenerateTsvFiles(BooksContext context)
        {
            var titles = context.Titles
                .Include(t => t.Author)
                .Include(t => t.TitleTags)
                .ThenInclude(tt => tt.Tag)
                .ToList(); 

            var data = titles
                .SelectMany(t => t.TitleTags.Select(tt => new
                {
                    AuthorName = t.Author.AuthorName,
                    TitleName = t.TitleName,
                    TagName = tt.Tag.TagName
                }))
                .ToList();

            var groupedByLetter = data
                .GroupBy(d => d.AuthorName[0])
                .ToList();

            foreach (var group in groupedByLetter)
            {
                var fileName = $"Data/{group.Key}.tsv";

                var sortedData = group
                    .OrderByDescending(d => d.AuthorName)
                    .ThenByDescending(d => d.TitleName)
                    .ThenByDescending(d => d.TagName)
                    .ToList();

                using var writer = new StreamWriter(fileName);

                writer.WriteLine("AuthorName\tTitleName\tTagName");

                foreach (var item in sortedData)
                {
                    writer.WriteLine($"{item.AuthorName}\t{item.TitleName}\t{item.TagName}");
                }
            }
        }
    }
}

namespace MVC.Models;

public class TheModel
{
        // Inicializado para evitar advertencias de non-nullable
        public string Phrase { get; set; } = string.Empty;

        // Inicialización correcta de Dictionary
        public Dictionary<char, int> Counts { get; set; } = new Dictionary<char, int>();

        // Inicializados para evitar advertencias de non-nullable
        public string Lower { get; set; } = string.Empty;
        public string Upper { get; set; } = string.Empty;
}

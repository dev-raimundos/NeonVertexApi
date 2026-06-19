namespace NeonVertexApi.App.Modules.TodoList.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string Category { get; set; }
        public required string Name { get; set; }
        public string? ImageUrl { get; set; } = null;
    }
}

namespace NeonVertexApi.App.Modules.TodoList.Models
{
    public class ListItem
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string CreatedAt { get; set; }
        public required string CreatedBy { get; set; }
    }
}

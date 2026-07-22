public class ReorderColumnsRequest
{
    public int ProjectId { get; set; }
    public List<int> OrderedColumnIds { get; set; } = new();
}
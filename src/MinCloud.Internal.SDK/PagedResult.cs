namespace MinCloud.Internal.SDK;

public interface IPagedResult<T>
{
    IEnumerable<T> Items { get; set; }
    int Offset { get; set; }
    int Limit { get; set; }
    int TotalCount { get; set; }
    bool HasMore { get; set; }
}
public class PagedResult<T> : IPagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int Offset { get; set; }
    public int Limit { get; set; }
    public int TotalCount { get; set; }
    public bool HasMore { get; set; }
}
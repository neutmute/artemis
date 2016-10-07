namespace Artemis
{
    public interface ISearchableService<out TSearchResponse>
    {
        TSearchResponse Search(string searchText, SearchTextFormat format = SearchTextFormat.Structured);
    }
}
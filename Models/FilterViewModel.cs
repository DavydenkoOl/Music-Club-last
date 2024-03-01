namespace Music_Club.Models
{
    public class FilterViewModel
    {
        public string? SelectedExecutor { get; set; }

        public string? SelectedGenres { get; set; }

        public string? SearchedData { get; set; }

        public FilterViewModel(string SelectedExecutor, string SelectedGenres, string SearchedData)
        {
            this.SelectedExecutor = SelectedExecutor;
            this.SelectedGenres = SelectedGenres;
            this.SearchedData = SearchedData;
        }

        public FilterViewModel() { }
    }
}

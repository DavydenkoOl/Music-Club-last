namespace Music_Club.Models
{
    public enum SortState
    {
        TitleAsc,
        TitleDesc
    }

    public class SortViewModel
    {
        public SortState titleSort { get; set; }
        public SortState current { get; set; }

        public SortViewModel(SortState sortOrder)
        {
            titleSort = sortOrder == SortState.TitleAsc ? SortState.TitleDesc : SortState.TitleAsc;
            current = sortOrder;
        }
        public SortViewModel() { }
    }
}
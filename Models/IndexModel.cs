namespace Music_Club.Models
{
    public class IndexModel
    {
        public SortViewModel? sortViewModel { get; set; }
        public PageViewModel? pageViewModel { get; set; }
        public FilterViewModel? filterViewModel { get; set; }
        public List<MusicClip>? musicClips { get; set; }
        public MusicClip? activeClip { get; set; }
    }
}

namespace Music_Club.Models
{
    public class CreateRecommendation
    {
        public static async Task<List<MusicClip>> createRecomendation(List<MusicClip> musics, int countRecom = 1)
        {
            if (musics == null)
                return null;

            var clips = new List<int>();
            var result = new List<MusicClip>();
            var random = new Random();

            for (int i = 0; i < countRecom; i++)
            {
                var randomIndx = random.Next(0, musics.Count());
                if (clips.Contains(randomIndx))
                {
                    i--;
                    continue;
                }
                result.Add(musics[randomIndx]);
                clips.Add(randomIndx);

            }

            return result;
        }
    }
}

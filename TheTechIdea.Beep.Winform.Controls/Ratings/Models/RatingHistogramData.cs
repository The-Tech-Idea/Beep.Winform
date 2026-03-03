using System.Collections.Generic;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Models
{
    /// <summary>
    /// Holds aggregated histogram data for a rating control 
    /// (e.g. how many users gave 1 star, 2 stars, … 5 stars).
    /// </summary>
    public class RatingHistogramData
    {
        /// <summary>
        /// Maps star value (1-based) to vote count.
        /// Key = star position (1 … StarCount), Value = number of votes.
        /// </summary>
        public Dictionary<int, int> Distribution { get; set; } = new Dictionary<int, int>();

        /// <summary>Total number of ratings submitted.</summary>
        public int TotalCount => Distribution.Values.Sum();

        /// <summary>Weighted average rating computed from Distribution.</summary>
        public float AverageRating
        {
            get
            {
                int total = TotalCount;
                if (total == 0) return 0f;
                float sum = Distribution.Sum(kv => kv.Key * kv.Value);
                return sum / total;
            }
        }

        /// <summary>Returns the fraction (0–1) of votes for a given star value.</summary>
        public float GetFraction(int starValue)
        {
            int total = TotalCount;
            if (total == 0) return 0f;
            return Distribution.TryGetValue(starValue, out int count) ? (float)count / total : 0f;
        }

        /// <summary>Returns the count of votes for a given star value.</summary>
        public int GetCount(int starValue)
            => Distribution.TryGetValue(starValue, out int count) ? count : 0;

        /// <summary>Sets the vote count for a given star value.</summary>
        public void SetCount(int starValue, int count)
            => Distribution[starValue] = count;

        /// <summary>Factory: build from a simple array indexed 0-based (index 0 = 1-star).</summary>
        public static RatingHistogramData FromArray(int[] counts)
        {
            var data = new RatingHistogramData();
            for (int i = 0; i < counts.Length; i++)
                data.Distribution[i + 1] = counts[i];
            return data;
        }
    }
}

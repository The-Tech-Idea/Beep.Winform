using System;
using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.TextFields.Models
{
    /// <summary>
    /// Represents a single search match result
    /// </summary>
    public class SearchMatch
    {
        /// <summary>
        /// Starting position of the match in the text
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// Length of the matched text
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// The matched text
        /// </summary>
        public string MatchedText { get; set; } = string.Empty;

        /// <summary>
        /// Line number where the match was found (0-based)
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Column position within the line (0-based)
        /// </summary>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// End index of the match
        /// </summary>
        public int EndIndex => StartIndex + Length;

        /// <summary>
        /// Creates a new search match
        /// </summary>
        public SearchMatch() { }

        /// <summary>
        /// Creates a new search match with values
        /// </summary>
        public SearchMatch(int startIndex, int length, string matchedText)
        {
            StartIndex = startIndex;
            Length = length;
            MatchedText = matchedText;
        }

        /// <summary>
        /// Checks if a position is within this match
        /// </summary>
        public bool ContainsPosition(int position)
        {
            return position >= StartIndex && position < EndIndex;
        }

        public override string ToString()
        {
            return $"Match at {StartIndex}: \"{MatchedText}\" (Line {LineNumber + 1}, Col {ColumnNumber + 1})";
        }
    }

    /// <summary>
    /// Represents the complete results of a search operation
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// All matches found
        /// </summary>
        public List<SearchMatch> Matches { get; set; } = new List<SearchMatch>();

        /// <summary>
        /// The search text that was used
        /// </summary>
        public string SearchText { get; set; } = string.Empty;

        /// <summary>
        /// Current match index (for navigation)
        /// </summary>
        public int CurrentMatchIndex { get; set; } = -1;

        /// <summary>
        /// Total number of matches found
        /// </summary>
        public int TotalMatches => Matches.Count;

        /// <summary>
        /// Whether any matches were found
        /// </summary>
        public bool HasMatches => Matches.Count > 0;

        /// <summary>
        /// Gets the current match (or null if none)
        /// </summary>
        public SearchMatch CurrentMatch => 
            CurrentMatchIndex >= 0 && CurrentMatchIndex < Matches.Count 
                ? Matches[CurrentMatchIndex] 
                : null;

        /// <summary>
        /// Time taken to perform the search
        /// </summary>
        public TimeSpan SearchDuration { get; set; }

        /// <summary>
        /// Whether the search wrapped around
        /// </summary>
        public bool WrappedAround { get; set; }

        /// <summary>
        /// Move to the next match
        /// </summary>
        /// <returns>True if moved to next, false if at end</returns>
        public bool MoveNext()
        {
            if (!HasMatches) return false;
            
            if (CurrentMatchIndex < Matches.Count - 1)
            {
                CurrentMatchIndex++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Move to the previous match
        /// </summary>
        /// <returns>True if moved to previous, false if at beginning</returns>
        public bool MovePrevious()
        {
            if (!HasMatches) return false;
            
            if (CurrentMatchIndex > 0)
            {
                CurrentMatchIndex--;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Move to next match with wrap-around
        /// </summary>
        public void MoveNextWrap()
        {
            if (!HasMatches) return;
            
            CurrentMatchIndex = (CurrentMatchIndex + 1) % Matches.Count;
        }

        /// <summary>
        /// Move to previous match with wrap-around
        /// </summary>
        public void MovePreviousWrap()
        {
            if (!HasMatches) return;
            
            CurrentMatchIndex = CurrentMatchIndex <= 0 
                ? Matches.Count - 1 
                : CurrentMatchIndex - 1;
        }

        /// <summary>
        /// Find the match closest to a given position
        /// </summary>
        public int FindMatchNearPosition(int position, SearchDirection direction)
        {
            if (!HasMatches) return -1;

            if (direction == SearchDirection.Forward)
            {
                for (int i = 0; i < Matches.Count; i++)
                {
                    if (Matches[i].StartIndex >= position)
                        return i;
                }
                return 0; // Wrap to first
            }
            else
            {
                for (int i = Matches.Count - 1; i >= 0; i--)
                {
                    if (Matches[i].StartIndex < position)
                        return i;
                }
                return Matches.Count - 1; // Wrap to last
            }
        }

        /// <summary>
        /// Clear all results
        /// </summary>
        public void Clear()
        {
            Matches.Clear();
            CurrentMatchIndex = -1;
            SearchText = string.Empty;
            WrappedAround = false;
        }

        /// <summary>
        /// Gets display text for current position (e.g., "3 of 15")
        /// </summary>
        public string GetPositionText()
        {
            if (!HasMatches)
                return "No matches";
            
            return $"{CurrentMatchIndex + 1} of {TotalMatches}";
        }
    }

    /// <summary>
    /// Event arguments for search events
    /// </summary>
    public class SearchEventArgs : EventArgs
    {
        public SearchOptions Options { get; set; }
        public SearchResult Result { get; set; }
        public bool Cancelled { get; set; }

        public SearchEventArgs(SearchOptions options, SearchResult result)
        {
            Options = options;
            Result = result;
        }
    }

    /// <summary>
    /// Event arguments for replace events
    /// </summary>
    public class ReplaceEventArgs : EventArgs
    {
        public SearchMatch Match { get; set; }
        public string OldText { get; set; }
        public string NewText { get; set; }
        public int ReplacementCount { get; set; }

        public ReplaceEventArgs(SearchMatch match, string oldText, string newText)
        {
            Match = match;
            OldText = oldText;
            NewText = newText;
            ReplacementCount = 1;
        }
    }
}


﻿using System;
using System.Collections.Generic;
using System.Linq;
using UndertaleModLib.Models;

namespace UndertaleModLib.Diff
{
    public static class StringDiffer
    {
        public record StringDiffData(StringDiffType DiffType, string Content)
        {
            public override string ToString() => (DiffType == StringDiffType.Added ? "+" : "-") + Content;
        }

        public enum StringDiffType
        {
            Added,
            Removed
        }

        public static List<StringDiffData> DiffStrings(UndertaleData modified, UndertaleData original)
        {
            List<StringDiffData> diffData = new List<StringDiffData>();
            List<UndertaleString> shallow = original.Strings.ToList();

            foreach (UndertaleString str in modified.Strings)
            {
                if (shallow.All(x => x.Content != str.Content))
                    diffData.Add(new StringDiffData(StringDiffType.Added, str.Content));
                
                shallow.RemoveAll(x => x.Content == str.Content);
            }
            
            diffData.AddRange(shallow.Select(x => new StringDiffData(StringDiffType.Removed, x.Content)));

            return diffData;
        }

        public static List<StringDiffData> Deserialize(IEnumerable<string> diffs)
        {
            List<StringDiffData> diffData = new List<StringDiffData>();

            foreach (string diff in diffs)
            {
                StringDiffType diffType = diff[0] == '+' ? StringDiffType.Added : StringDiffType.Removed;
                
                diffData.Add(new StringDiffData(diffType, diff.Remove(0, 1)));
            }

            return diffData;
        }

        public static void ApplyStringDiff(UndertaleData data, List<StringDiffData> diffs)
        {
            foreach (StringDiffData diffData in diffs)
            {
                switch (diffData.DiffType)
                {
                    case StringDiffType.Added:
                        data.Strings.Add(new UndertaleString(diffData.Content));
                        break;
                    
                    case StringDiffType.Removed:
                        data.Strings.Remove(data.Strings.FirstOrDefault(x => x.Content == diffData.Content));
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException(nameof(diffData.DiffType));
                }
            }
        }
    }
}
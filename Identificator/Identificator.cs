using BooruSharp.Booru;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identificator
{
    public class Identificator
    {
        public struct CharacInfo
        {
            public CharacInfo(string source, string eyesColor, string hairColor)
            {
                this.source = source;
                this.eyesColor = eyesColor;
                this.hairColor = hairColor;
            }
            public readonly string source;
            public readonly string eyesColor;
            public readonly string hairColor;
        }

        public readonly string[] colors = new string[] {
            "black", "brown", "yellow", "green", "red", "grey", "blue", "white", "purple", "pink", "silver", "blonde"
        };

        public readonly char[] split = new char[] {
            '(', ')', '_', ' ', ',', '.', '[', ']', '#', '{', '}', '|'
        };

        public async Task<string[]> CorrectName(string query)
        {
            BooruSharp.Search.Tag.SearchResult[] res = (await new Lolibooru().GetTags(query)).Where(delegate (BooruSharp.Search.Tag.SearchResult result) { return (result.type == BooruSharp.Search.Tag.TagType.Character); }).ToArray();
            if (res.Length > 0)
                return (res.Select(delegate (BooruSharp.Search.Tag.SearchResult result) { return (result.name); }).ToArray());
            List<string> othersRes = new List<string>();
            string[] splitQuery = query.Split(split);
            foreach (string q in splitQuery)
            {
                othersRes.AddRange((await new Lolibooru().GetTags(q)).Where(delegate (BooruSharp.Search.Tag.SearchResult result) { return (result.type == BooruSharp.Search.Tag.TagType.Character); }).Select(delegate (BooruSharp.Search.Tag.SearchResult result) { return (result.name); }).ToList());
            }
            othersRes = othersRes.Distinct().ToList();
            List<string> bestRes = new List<string>();
            foreach (string s in othersRes)
            {
                bool isEverywhere = true;
                foreach (string s2 in s.Split(split))
                {
                    if (!splitQuery.Contains(s2))
                    {
                        isEverywhere = false;
                        break;
                    }
                }
                if (isEverywhere)
                    bestRes.Add(s);
            }
            if (bestRes.Count > 0)
                return (bestRes.ToArray());
            return (othersRes.ToArray());
        }
    
        public async Task<CharacInfo> GetAnime(string firstName, string lastName)
        {
            return (await GetAnime(lastName.ToLower() + "_" + firstName.ToLower()));
        }

        public async Task<CharacInfo> GetAnime(string query)
        {
            Gelbooru booru = new Gelbooru();
            Dictionary<string, int> tags = new Dictionary<string, int>();
            for (int i = 1; i <= 100; i++)
            {
                try
                {
                    foreach (string s in (await booru.GetImage(i, query, "1girl")).tags)
                    {
                        if (tags.ContainsKey(s))
                            tags[s]++;
                        else
                            tags.Add(s, 1);
                    }
                }
                catch (BooruSharp.Search.InvalidTags)
                {
                    break;
                }
            }
            string source = null;
            int sourcePercent = 0;
            foreach (var item in tags)
            {
                try
                {
                    if ((await booru.GetTag(item.Key)).type == BooruSharp.Search.Tag.TagType.Copyright && item.Value > sourcePercent)
                    {
                        source = item.Key;
                        sourcePercent = item.Value;
                    }
                }
                catch (BooruSharp.Search.InvalidTags)
                { }
            }
            string hair = null, eyes = null;
            int hairPercent = 0, eyesPercent = 0;
            foreach (var item in tags)
            {
                string[] parts = item.Key.Split('_');
                if (parts.Length == 2)
                {
                    if (parts[1] == "hair" && colors.Contains(parts[0]) && item.Value > hairPercent)
                    {
                        hair = parts[0];
                        hairPercent = item.Value;
                    }
                    else if (parts[1] == "eyes" && colors.Contains(parts[0]) && item.Value > eyesPercent)
                    {
                        eyes = parts[0];
                        eyesPercent = item.Value;
                    }
                }
            }
            return (new CharacInfo(source, eyes, hair));
        }
    }
}

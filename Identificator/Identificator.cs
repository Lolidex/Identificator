using BooruSharp.Booru;
using System;
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
            "red", "yellow", "grey", "green", "blue", "orange", "white", "purple", "pink"
        };

        public async Task<CharacInfo> GetAnime(string firstName, string lastName)
        {
            DateTime start = DateTime.Now;
            string query = lastName.ToLower() + "_" + firstName.ToLower();
            Gelbooru booru = new Gelbooru();
            int nbMax = await booru.GetNbImage(query, "1girl");
            if (nbMax > 100)
                nbMax = 100;
            Dictionary<string, int> tags = new Dictionary<string, int>();
            for (int i = 1; i < nbMax; i++)
            {
                foreach (string s in (await booru.GetImage(i, query, "1girl")).tags)
                {
                    if (tags.ContainsKey(s))
                        tags[s]++;
                    else
                        tags.Add(s, 1);
                }
            }
            tags = tags.Where(x => (x.Value * 98 / nbMax) > 50).ToDictionary(x => x.Key, x => x.Value);
            string source = null;
            foreach (var item in tags)
            {
                if ((await booru.GetTag(item.Key)).type == BooruSharp.Search.Tag.TagType.Copyright)
                {
                    source = item.Key;
                    break;
                }
            }
            string hair = null, eyes = null;
            foreach (var item in tags)
            {
                string[] parts = item.Key.Split('_');
                if (parts.Length == 2)
                {
                    if (parts[1] == "hair" && colors.Contains(parts[0]))
                        hair = parts[0];
                    else if (parts[1] == "eyes" && colors.Contains(parts[0]))
                        eyes = parts[0];
                }
            }
            return (new CharacInfo(source, eyes, hair));
        }
    }
}

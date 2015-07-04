using WDict = System.Collections.Generic.Dictionary<string, uint>;
using TDict = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, uint>>;

using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Markov
{
    static public class MarkovHelper
    {
        static public TDict BuildTDict(string s, int size)
        {
            TDict t = new TDict();
            string prev = "";
            foreach (string word in Chunk(s, size) )
            {
                if (t.ContainsKey(prev))
                {
                    WDict w = t[prev];
                    if ( w.ContainsKey(word) )
                        w[word] += 1;
                    else
                        w.Add(word, 1);
                }
                else
                    t.Add( prev, new WDict(){{word, 1}} );

                prev = word;
            }

            return t;
        }

        static public string[] Chunk(string s, int size)
        {
            string[] ls = s.Split(' ');
            List<string> chunk = new List<string>();

            for (int i = 0; i < ls.Length - size; ++i)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append( ls.Skip(i).Take(size).Aggregate( (w, k) => w + " " + k) );
                chunk.Add( sb.ToString() );
            }

            return chunk.ToArray();
        }

        static public string BuildString(TDict t, int len, bool exact)
        {
            string last;
            List<string> ucStr = new List<string>();
            StringBuilder sb = new StringBuilder();

            foreach ( string word in t.Keys.Skip(1) )
            {
                if ( char.IsUpper( word.First() ) )
                    ucStr.Add(word);
            }

            if (ucStr.Count > 0)
                sb.Append( ucStr.ElementAt(Program.r.Next(0, ucStr.Count) ) );

            last = sb.ToString();
            sb.Append(" ");

            WDict w = new WDict();
 
            for (uint i = 0; i < len; ++i)
            {
                if (t.ContainsKey(last))
                    w = t[last];
                else
                    w = t[""];

                last = MarkovHelper.Choose(w);
                sb.Append( last.Split(' ').Last() ).Append(" ");
            }

            if (!exact)
            {
                while (last.Last() != '.')
                {
                    if (t.ContainsKey(last))
                        w = t[last];
                    else
                        w = t[""];

                    last = MarkovHelper.Choose(w);
                    sb.Append(last.Split(' ').Last()).Append(" ");
                }
            }

            return sb.ToString();
        }

        static private string Choose(WDict w)
        {
            long total = w.Sum(t => t.Value);

            while (true)
            {
                int i = Program.r.Next(0, w.Count);
                double c = Program.r.NextDouble();
                System.Collections.Generic.KeyValuePair<string, uint> k = w.ElementAt(i);

                if ( c < (double)k.Value / total )
                    return k.Key;
            }
        }
    }
}

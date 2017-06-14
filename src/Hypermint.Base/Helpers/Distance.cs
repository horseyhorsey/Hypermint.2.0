using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hypermint.Base.Helpers
{
    public class Distance
    {
        /// <summary>
        /// Compute Levenshtein Distance
        /// </summary>
        /// <param name="s">String 1</param>
        /// <param name="t">String 2</param>
        /// <returns>Distance between the two strings.
        /// The larger the number, the bigger the difference.
        /// </returns>
        public int LD(string s, string t)
        {
            var n = s.Length; //length of s
            var m = t.Length; //length of t

            var d = new int[n + 1, m + 1]; // matrix

            // Step 1
            if (n == 0) return m;
            if (m == 0) return n;

            // Step 2
            for (var i = 0; i <= n; d[i, 0] = i++) ;
            for (var j = 0; j <= m; d[0, j] = j++) ;

            // Step 3
            for (var i = 1; i <= n; i++)
            {
                //Step 4
                for (var j = 1; j <= m; j++)
                {
                    // Step 5
                    var cost = (t.Substring(j - 1, 1) == s.Substring(i - 1, 1) ? 0 : 1); // cost

                    // Step 6
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            // Step 7
            return d[n, m];

        }
    }
}

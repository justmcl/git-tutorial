namespace SpeedTest {
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;
    using System;

    internal class Program {
        #region Methods

        private static void Main (string[] args) {
            var watch = new Stopwatch ();
            const long count = 1000;
            const int length = 1000;
            string comparestring = StringDistance.GenerateRandomString (length);
            var strlist = new string[count];
            var steps = new int[count];
            // prepare string[] for comparison  

            Parallel.For (0, count, i => strlist[i] = StringDistance.GenerateRandomString (length));

            Console.WriteLine ("已经生成了" + count + "个长度为" + length + "的字符串");

            watch.Start ();
            for (int i = 0; i < count; i++) {
                steps[i] = StringDistance.LevenshteinDistance (comparestring, strlist[i]);
            }
            watch.Stop ();
            Console.WriteLine ("完成非并行计算,耗时(ms)" + watch.ElapsedMilliseconds);
            Console.WriteLine ("性能比" + 100000d / watch.ElapsedMilliseconds);
            watch.Reset ();
            watch.Start ();
            Parallel.For (
                0, count, delegate (long i) { steps[i] = StringDistance.LevenshteinDistance (comparestring, strlist[i]); });
            watch.Stop ();
            Console.WriteLine ("完成并行计算,耗时(ms)" + watch.ElapsedMilliseconds);
            Console.WriteLine ("性能比" + 100000d / watch.ElapsedMilliseconds);
            Console.ReadKey ();
        }

        #endregion
    }

    internal class StringDistance {
        #region Public Methods

        public static string GenerateRandomString (int length) {
            var r = new Random ((int) DateTime.Now.Ticks);
            var sb = new StringBuilder (length);
            for (int i = 0; i < length; i++) {
                int c = r.Next (97, 123);
                sb.Append (Char.ConvertFromUtf32 (c));
            }
            return sb.ToString ();
        }

        public static int LevenshteinDistance (string str1, string str2) {
            var scratchDistanceMatrix = new int[str1.Length + 1, str2.Length + 1];
            // distance matrix contains one extra row and column for the seed values         
            for (int i = 0; i <= str1.Length; i++) {
                scratchDistanceMatrix[i, 0] = i;
            }
            for (int j = 0; j <= str2.Length; j++) {
                scratchDistanceMatrix[0, j] = j;
            }
            for (int i = 1; i <= str1.Length; i++) {
                int str1Index = i - 1;
                for (int j = 1; j <= str2.Length; j++) {
                    int str2Index = j - 1;
                    int cost = (str1[str1Index] == str2[str2Index]) ? 0 : 1;
                    int deletion = (i == 0) ? 1 : scratchDistanceMatrix[i - 1, j] + 1;
                    int insertion = (j == 0) ? 1 : scratchDistanceMatrix[i, j - 1] + 1;
                    int substitution = (i == 0 || j == 0) ? cost : scratchDistanceMatrix[i - 1, j - 1] + cost;
                    scratchDistanceMatrix[i, j] = Math.Min (Math.Min (deletion, insertion), substitution);
                    // Check for Transposition  
                    if (i > 1 && j > 1 && (str1[str1Index] == str2[str2Index - 1]) &&
                        (str1[str1Index - 1] == str2[str2Index])) {
                        scratchDistanceMatrix[i, j] = Math.Min (
                            scratchDistanceMatrix[i, j], scratchDistanceMatrix[i - 2, j - 2] + cost);
                    }
                }
            }
            // Levenshtein distance is the bottom right element       
            return scratchDistanceMatrix[str1.Length, str2.Length];
        }

        #endregion
    }
}
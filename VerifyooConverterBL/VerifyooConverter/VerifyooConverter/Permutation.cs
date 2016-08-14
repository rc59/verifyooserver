using System.Collections.Generic;

namespace VerifyooConverter
{
    internal class Permutation
    {
        public int Idx1;
        public int Idx2;
        public int Idx3;
        public int Idx4;

        public List<int> ListPermutationIndexes;

        public Permutation(int idx1, int idx2, int idx3, int idx4) {
            Idx1 = idx1;
            Idx2 = idx2;
            Idx3 = idx3;
            Idx4 = idx4;

            ListPermutationIndexes = new List<int>();
            ListPermutationIndexes.Add(Idx1);
            ListPermutationIndexes.Add(Idx2);
            ListPermutationIndexes.Add(Idx3);
            ListPermutationIndexes.Add(Idx4);
        }    
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCompression
{
    public class CharacterFrequency : IComparable
    {
        #region Properties
        public int Character { get; set; }
        public int Frequency { get; set; }
        #endregion

        #region Constructors
        public CharacterFrequency()
        {

        }
        #endregion


        #region Public Methods
        public void Increment()
        {
            Frequency++;
        }

        public static void WriteArray(StreamWriter writer, CharacterFrequency[] arr)
        {
            foreach (var charFreq in arr)
            {
                writer.WriteLine($"avcii: {charFreq.Character} frequency: {charFreq.Frequency}");
            }
        }

        public static CharacterFrequency[] BuildEmptyArray(int size)
        {
            CharacterFrequency[] arr = new CharacterFrequency[size];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = new CharacterFrequency
                {
                    Character = Convert.ToChar(i),
                    Frequency = 0
                };
            }
            return arr;
        }

        public int CompareTo(object obj)
        {
            var charFrequency = (CharacterFrequency)obj;

            if (this.Frequency > charFrequency.Frequency)
                return 1;
            else if (this.Frequency < charFrequency.Frequency)
                return -1;
            else // ==
                return 0;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCompression
{
    public class CharacterEncoding
    {
        #region Constants
        public const string END_OF_TABLE = "/"; //ASCII: 47
        #endregion
        #region Properties
        public byte Character { get; set; }
        public string Encoding { get; set; }
        #endregion

        #region Constructors
        public CharacterEncoding(byte ch, string encoding)
        {
            Character = ch;
            Encoding = encoding.ToString();
        }
        #endregion

        public static CharacterEncoding[] BuildEmptyArray(int size)
        {
            CharacterEncoding[] arr = new CharacterEncoding[size];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = new CharacterEncoding((byte)i, string.Empty);
            }
            return arr;
        }


        /// <summary>
        /// Writes character encodings to file. Does not write unused characters.
        /// </summary>
        /// <param name="writer">
        /// </param>
        /// <param name="arr"></param>
        public static void WriteEncoding(StreamWriter writer, CharacterEncoding[] arr)
        {
            foreach (var cEnc in arr)
            {
                if (!String.IsNullOrWhiteSpace(cEnc.Encoding))
                {
                    writer.Write($"{cEnc.Character}_{cEnc.Encoding}__");
                }
            }
            writer.Write(END_OF_TABLE);
        }
        //use | for setting bits like byte|32 is on. treat like a [8].
        //write to disk after each full byte but make sure to put overflow into next byte.
        //b = b|32;
    }
}

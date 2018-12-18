using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCompression
{
    class Program
    {
        #region Main
        static void Main(string[] args)
        {
            const byte INPUT = 0;
            const byte OUTPUT = 1;
            const byte MODE = 2;

            CompressionInputValidation(args);


            var compressionManager = new CompressionManager
            {
                FileRead = args[INPUT],
                FileWrite = args[OUTPUT],
                CharFrequencies = CharacterFrequency.BuildEmptyArray(256),
                CharEncodings = CharacterEncoding.BuildEmptyArray(256)
            };

            if (args[MODE].ToUpper() == "COMPRESS")
            {
                compressionManager.Compress();
            }
            else if (args[MODE].ToUpper() == "COMPRESSTEST")
            {
                compressionManager.CompressTest();
            }
            else if (args[MODE].ToUpper() == "DECOMPRESS")
            {
                compressionManager.Decompress();
            }
            else
            {
                Console.WriteLine("Please enter COMPRESS or DECOMPRESS after both files");
                Console.ReadKey();
                Environment.Exit(0);
            }
            #endregion
        }
        #region Private Methods
        /// <summary>
        /// Validates only 1 argument is passed.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static bool ValidateArgs(string[] args)
        {
            if (args.Length != 3)
                return false;
            return true;
        }

        /// <summary>
        /// checks if file passed exists in the current executable directory.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static bool DoesFileExist(string fileName)
        {
            if (!File.Exists(fileName))
                return false;
            return true;
        }

        /// <summary>
        /// Validates command line parameters.
        /// </summary>
        /// <param name="args"></param>
        private static void CompressionInputValidation(string[] args)
        {
            if (!ValidateArgs(args))
            {
                Console.WriteLine("Please enter only the file to be compressed");
                Console.ReadLine();
                Environment.Exit(0);
            }

            if (!DoesFileExist(args[0]))
            {
                Console.WriteLine("File entered does not exist");
                Console.ReadLine();
                Environment.Exit(0);
            }
            //add a check here for if file is compressed or not 
        }
        #endregion
    }
}


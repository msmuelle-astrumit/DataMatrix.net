/*
DataMatrix.Net

DataMatrix.Net - .net library for decoding DataMatrix codes.
Copyright (C) 2009/2010 Michael Faschinger

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public
License as published by the Free Software Foundation; either
version 3.0 of the License, or (at your option) any later version.
You can also redistribute and/or modify it under the terms of the
GNU Lesser General Public License as published by the Free Software
Foundation; either version 3.0 of the License or (at your option)
any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
General Public License or the GNU Lesser General Public License 
for more details.

You should have received a copy of the GNU General Public
License and the GNU Lesser General Public License along with this 
library; if not, write to the Free Software Foundation, Inc., 
51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

Contact: Michael Faschinger - michfasch@gmx.at
 
*/

using System;
using System.Collections.Generic;
using System.Text;
using DataMatrix.net;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace DataMatrixTest
{
    class Program
    {
        private static string testVal = "Hello World!";

        static void Main(string[] args)
        {
            TestMatrixEnDecoder();
            TestMosaicEnDecoder();
            TestGS1EnDecoder();
        }

        private static void TestMatrixEnDecoder()
        {
            string fileName = "encodedImg.png";
            DmtxImageEncoder encoder = new DmtxImageEncoder();
            DmtxImageEncoderOptions options = new DmtxImageEncoderOptions();
            options.ModuleSize = 8;
            options.MarginSize = 4;
            options.BackColor = Color.White;
            options.ForeColor = Color.Green;
            Bitmap encodedBitmap = encoder.EncodeImage(testVal);
            encodedBitmap.Save(fileName, ImageFormat.Png);

            DmtxImageDecoder decoder = new DmtxImageDecoder();
            List<string> codes = decoder.DecodeImage((Bitmap)Bitmap.FromFile(fileName), 1, new TimeSpan(0, 0, 3));
            foreach (string code in codes)
            {
                Console.WriteLine("Decoded:\n" + code);
            }

            string s = encoder.EncodeSvgImage("DataMatrix.net rocks!!one!eleven!!111!eins!!!!", 7, 7, Color.FromArgb(100, 255, 0, 0), Color.Turquoise);
            TextWriter tw = new StreamWriter("encodedImg.svg");
            tw.Write(s);
            tw.Flush();
            tw.Close();

            TestRawEncoder("HELLO WORLD");
            new DmtxImageEncoder().EncodeImage("HELLO WORLD").Save("helloWorld.png");

            for(int i = 1; i < 10; i++)
            {
                var encodedData = Guid.NewGuid().ToString();
                Bitmap source = encoder.EncodeImage(encodedData);
                var decodedData = decoder.DecodeImage(source);
                if(decodedData.Count != 1 || decodedData[0] != encodedData)
                    throw new Exception("Encoding or decoding failed!");
            }
        }

        private static void TestRawEncoder(string text)
        {
            DmtxImageEncoder encoder = new DmtxImageEncoder();
            bool[,] rawData = encoder.EncodeRawData(text);
            Console.WriteLine("================");
            Console.WriteLine();
            for (int rowIdx = 0; rowIdx < rawData.GetLength(1); rowIdx++)
            {
                for (int colIdx = 0; colIdx < rawData.GetLength(0); colIdx++)
                {
                    Console.Write(rawData[colIdx, rowIdx] ? "X" : " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("================");
        }


        private static void TestMosaicEnDecoder()
        {
            string fileName = "encodedMosaicImg.png";
            DmtxImageEncoder encoder = new DmtxImageEncoder();
            DmtxImageEncoderOptions options = new DmtxImageEncoderOptions();
            options.ModuleSize = 8;
            options.MarginSize = 4;
            Bitmap encodedBitmap = encoder.EncodeImageMosaic(testVal);
            encodedBitmap.Save(fileName, ImageFormat.Png);

            DmtxImageDecoder decoder = new DmtxImageDecoder();
            List<string> codes = decoder.DecodeImageMosaic((Bitmap)Bitmap.FromFile(fileName), 1, new TimeSpan(0, 0, 3));
            foreach (string code in codes)
            {
                Console.WriteLine("Decoded:\n" + code);
            }

            Console.Read();
        }

        private static void TestGS1EnDecoder()
        {
            string fileName1 = "gs1DataMatrix1.png";
            string fileName2 = "gs1DataMatrix2.gif";
            string gs1Code1 = "10AC3454G3";
            string gs1Code2 = "010761234567890017100503";
            DmtxImageEncoder encoder = new DmtxImageEncoder();
            DmtxImageEncoderOptions options = new DmtxImageEncoderOptions();
            options.ModuleSize = 8;
            options.MarginSize = 30;
            options.BackColor = Color.White;
            options.ForeColor = Color.Black;
            options.Scheme = DmtxScheme.DmtxSchemeAsciiGS1;
            Bitmap encodedBitmap1 = encoder.EncodeImage(gs1Code1, options);
            encodedBitmap1.Save(fileName1, ImageFormat.Png);
            Bitmap encodedBitmap2 = encoder.EncodeImage(gs1Code2, options);
            encodedBitmap2.Save(fileName2, ImageFormat.Gif);
            DmtxImageDecoder decoder = new DmtxImageDecoder();
            List<string> decodedCodes1 = decoder.DecodeImage(encodedBitmap1, 1, new TimeSpan(0, 0, 5));
            List<string> decodedCodes2 = decoder.DecodeImage(encodedBitmap2, 1, new TimeSpan(0, 0, 5));
            if (decodedCodes1 != null && decodedCodes1.Count == 1)
            {
                Console.WriteLine("Encoded code 1: {0}, decoded code 1: {1}, codes are equal: {2}", gs1Code1, decodedCodes1[0], gs1Code1.Equals(decodedCodes1[0]));
            }
            if (decodedCodes2 != null && decodedCodes2.Count == 1)
            {
                Console.WriteLine("Encoded code 2: {0}, decoded code 2: {1}, codes are equal: {2}", gs1Code2, decodedCodes2[0], gs1Code2.Equals(decodedCodes2[0]));
            }
            Console.Read();
        }
    }
}

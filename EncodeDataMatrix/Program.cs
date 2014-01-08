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
using System.IO;

namespace EncodeDataMatrix
{
    class Program
    {
        static List<string> helpArgs = new List<string>(new string[] { "-h", "-H", "-?", "--help", "/h", "/H", "/?" });
        static List<string> supportedArgs = new List<string>(new string[] { "-i", "-c", "-o", "-s", "-b", "-f", "-t", "-m" });

        static void Main(string[] args)
        {
            if (args.Length == 0 || (args.Length == 1 && IsHelp(args[0])))
            {
                ShowHelp();
                return;
            }
            SortedDictionary<string, string> argumentList = null;
            try
            {
                argumentList = ParseArguments(args);
            }
            catch (Exception)
            {
                Console.WriteLine("Error parsing arguments, aborting");
            }

            DmtxImageEncoderOptions encoderOptions = null;
            try
            {
                encoderOptions = GetEncoderOptions(argumentList);
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid arguments, aborting");
            }
            string input = "";
            try
            {
                input = GetInputString(argumentList);
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid input, aborting");
                return;
            }

            if (!argumentList.ContainsKey("-o"))
            {
                Console.WriteLine("No output file specified, aborting");
                return;
            }

            string outputFileName = argumentList["-o"];
            if (!outputFileName.ToLower().EndsWith(".jpg") &&
                !outputFileName.ToLower().EndsWith(".gif") &&
                !outputFileName.ToLower().EndsWith(".png") &&
                !outputFileName.ToLower().EndsWith(".bmp") &&
                !outputFileName.ToLower().EndsWith(".svg"))
            {
                Console.WriteLine("File type not supported!");
                return;
            }

            DmtxImageEncoder encoder = new DmtxImageEncoder();
            if (outputFileName.ToLower().EndsWith(".svg"))
            {
                string output = encoder.EncodeSvgImage(input, encoderOptions);
                try
                {
                    TextWriter tw = new StreamWriter(outputFileName);
                    tw.Write(output);
                    tw.Close();
                    Console.WriteLine("Output created successfully!");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error writing to output file: " + ex.Message);
                    return;
                }
            }
            Image outputImage = null;
            if (argumentList.ContainsKey("-t") && argumentList["-t"] == "Mosaic")
            {
                encoderOptions.BackColor = Color.White;
                encoderOptions.ForeColor = Color.Black;
                outputImage = encoder.EncodeImageMosaic(input, encoderOptions);
            }
            else if (!argumentList.ContainsKey("-t") || (argumentList.ContainsKey("-t") && argumentList["-t"] == "Matrix"))
            {
                outputImage = encoder.EncodeImage(input, encoderOptions);
            }
            else
            {
                Console.WriteLine("Invalid output type, only 'Matrix' and 'Mosaic' are supported!");
                return;
            }
            try
            {
                outputImage.Save(outputFileName);
                Console.WriteLine("Output created successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to output file: " + ex.Message);
                return;
            }
        }

        private static string GetInputString(SortedDictionary<string, string> argumentList)
        {
            if (argumentList.ContainsKey("-i") && argumentList.ContainsKey("-c"))
            {
                Console.WriteLine("You can only pass one of -i and -c arguments");
                throw new ArgumentException("Invalid input arguments!");
            }
            if (argumentList.ContainsKey("-c"))
            {
                return argumentList["-c"];
            }
            if (argumentList.ContainsKey("-i"))
            {
                string fileName = argumentList["-i"];
                if (!File.Exists(fileName))
                {
                    Console.WriteLine("Input file does not exist!");
                    throw new ArgumentException("Input file not found!");
                }
                try
                {
                    TextReader tr = new StreamReader(fileName);
                    string result = tr.ReadToEnd();
                    tr.Close();
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error opening input file");
                    throw ex;
                }
            }
            Console.WriteLine("Error, either -i or -c must be passed!");
            throw new ArgumentException("No input specified");
        }

        private static DmtxImageEncoderOptions GetEncoderOptions(SortedDictionary<string, string> argumentList)
        {
            DmtxImageEncoderOptions result = new DmtxImageEncoderOptions();
            if (argumentList.ContainsKey("-b"))
            {
                result.BackColor = GetColorFromString(argumentList["-b"]);
            }
            if (argumentList.ContainsKey("-f"))
            {
                result.ForeColor = GetColorFromString(argumentList["-f"]);
            }
            if (argumentList.ContainsKey("-s"))
            {
                int symbolSize = result.ModuleSize;
                bool parsingSuccessful = int.TryParse(argumentList["-s"], out symbolSize);
                if (!parsingSuccessful)
                {
                    throw new Exception("Invalid symbol size!");
                }
                result.ModuleSize = symbolSize;
            }
            if (argumentList.ContainsKey("-m"))
            {
                int marginWidth = result.MarginSize;
                bool parsingSuccessful = int.TryParse(argumentList["-m"], out marginWidth);
                if (!parsingSuccessful)
                {
                    throw new Exception("Invalid symbol size!");
                }
                result.MarginSize = marginWidth;
            }
            return result;
        }

        private static Color GetColorFromString(string colorAsString)
        {
            if (colorAsString.Length != 6)
            {
                Console.WriteLine("Invalid color string!");
                throw new ArgumentException("Invalid color string!");
            }
            int red, green, blue;
            bool parseSuccessful = int.TryParse(colorAsString.Substring(0, 2), out red);
            if (!parseSuccessful)
            {
                Console.WriteLine("Invalid color string!");
                throw new ArgumentException("Invalid color string!");
            }
            parseSuccessful = int.TryParse(colorAsString.Substring(2, 2), out green);
            if (!parseSuccessful)
            {
                Console.WriteLine("Invalid color string!");
                throw new ArgumentException("Invalid color string!");
            }
            parseSuccessful = int.TryParse(colorAsString.Substring(4, 2), out blue);
            if (!parseSuccessful)
            {
                Console.WriteLine("Invalid color string!");
                throw new ArgumentException("Invalid color string!");
            }
            return Color.FromArgb(red, green, blue);
        }

        private static SortedDictionary<string, string> ParseArguments(string[] args)
        {
            SortedDictionary<string, string> result = new SortedDictionary<string, string>();
            for (int argCount = 0; argCount < args.Length - 1; argCount += 2)
            {
                string key = args[argCount].ToLower();
                string val = args[argCount + 1];
                if (result.ContainsKey(key))
                {
                    Console.WriteLine("Duplicate argument '" + key + "'");
                    throw new ArgumentException("Duplicate arguments passed!");
                }
                if (!supportedArgs.Contains(key))
                {
                    Console.WriteLine("Argument '" + key + "' is not supported!");
                    throw new ArgumentException("Not supported argument!");
                }
                result.Add(key, val);
            }
            return result;
        }

        private static void ShowHelp()
        {
            Console.WriteLine("EncodeDataMatrix.exe - DataMatrix encoding tool usage:");
            Console.WriteLine("./EncodeDataMatrix.exe [options] -o outputFile -c \"text to encode\"");
            Console.WriteLine("or");
            Console.WriteLine("./EncodeDataMatrix.exe [options] -o outputFile -i [Input File]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("\t-b rrggbb\t Background color, default is white (FFFFFF) - ignored if output type is mosaic");
            Console.WriteLine("\t   example: -b A900FF");
            Console.WriteLine("\t-f rrggbb\t Foreground color, default is black (000000) - ignored if output type is mosaic");
            Console.WriteLine("\t   example: -f 00FF1C");
            Console.WriteLine("\t-s x\t Symbol size in pixels, default is 5");
            Console.WriteLine("\t   example: -s 10");
            Console.WriteLine("\t-m x\t Margin width in pixels, default is 10");
            Console.WriteLine("\t   example: -m 20");
            Console.WriteLine("\t-t {Matrix|Mosaic} output type, default is Matrix");
            Console.WriteLine("\t   example: -t Mosaic");
            Console.WriteLine();
            Console.WriteLine("The output file type is recoginzed automatically based on the file extension.");
            Console.WriteLine("Supported file types are: png, gif, bmp, jpeg, svg (Matrix only)");
        }

        private static bool IsHelp(string arg)
        {
            return helpArgs.Contains(arg);
        }
    }
}

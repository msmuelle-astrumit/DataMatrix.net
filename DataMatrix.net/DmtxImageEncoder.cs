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

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Globalization;

namespace DataMatrix.net
{
    public class DmtxImageEncoder
    {
        public static readonly int DefaultDotSize = 5;
        public static readonly int DefaultMargin = 10;
        public static readonly Color DefaultBackColor = Color.White;
        public static readonly Color DefaultForeColor = Color.Black;

        public Bitmap EncodeImageMosaic(string val)
        {
            return EncodeImageMosaic(val, DefaultDotSize);
        }

        public Bitmap EncodeImageMosaic(string val, int dotSize)
        {
            return EncodeImageMosaic(val, dotSize, DefaultMargin);
        }

        public Bitmap EncodeImageMosaic(string val, int dotSize, int margin)
        {
            DmtxImageEncoderOptions options = new DmtxImageEncoderOptions {MarginSize = margin, ModuleSize = dotSize};
            return EncodeImageMosaic(val, options);
        }

        public Bitmap EncodeImageMosaic(string val, DmtxImageEncoderOptions options)
        {
            return EncodeImage(val, options, true);
        }

        private Bitmap EncodeImage(string val, DmtxImageEncoderOptions options, bool isMosaic)
        {
            DmtxEncode encode = new DmtxEncode
                                    {
                                        ModuleSize = options.ModuleSize,
                                        MarginSize = options.MarginSize,
                                        SizeIdxRequest = options.SizeIdx
                                    };
            byte[] valAsByteArray = GetRawDataAndSetEncoding(val, options, encode);
            if (isMosaic)
            {
                encode.EncodeDataMosaic(valAsByteArray);
            }
            else
            {
                encode.EncodeDataMatrix(options.ForeColor, options.BackColor, valAsByteArray);
            }
            return CopyDataToBitmap(encode.Image.Pxl, encode.Image.Width, encode.Image.Height);
        }

        private static byte[] GetRawDataAndSetEncoding(string code, DmtxImageEncoderOptions options, DmtxEncode encode)
        {
            byte[] result = options.Encoding.GetBytes(code);
            encode.Scheme = options.Scheme;
            if (options.Scheme == DmtxScheme.DmtxSchemeAsciiGS1)
            {
                List<byte> prefixedRawData = new List<byte>(new[] { (byte)232 });
                prefixedRawData.AddRange(result);
                result = prefixedRawData.ToArray();
                encode.Scheme = DmtxScheme.DmtxSchemeAscii;
            }
            return result;
        }

        public Bitmap EncodeImage(string val)
        {
            return EncodeImage(val, DefaultDotSize, DefaultMargin);
        }

        public Bitmap EncodeImage(string val, int dotSize)
        {
            return EncodeImage(val, dotSize, DefaultMargin);
        }

        public Bitmap EncodeImage(string val, int dotSize, int margin)
        {
            DmtxImageEncoderOptions options = new DmtxImageEncoderOptions {MarginSize = margin, ModuleSize = dotSize};
            return EncodeImage(val, options);
        }

        public Bitmap EncodeImage(string val, DmtxImageEncoderOptions options)
        {
            return EncodeImage(val, options, false);
        }

        public string EncodeSvgImage(string val)
        {
            return EncodeSvgImage(val, DefaultDotSize, DefaultMargin, DefaultForeColor, DefaultBackColor);
        }

        public string EncodeSvgImage(string val, int dotSize)
        {
            return EncodeSvgImage(val, dotSize, DefaultMargin, DefaultForeColor, DefaultBackColor);
        }

        public string EncodeSvgImage(string val, int dotSize, int margin)
        {
            return EncodeSvgImage(val, dotSize, margin, DefaultForeColor, DefaultBackColor);
        }

        public string EncodeSvgImage(string val, int dotSize, int margin, Color foreColor, Color backColor)
        {
            DmtxImageEncoderOptions options = new DmtxImageEncoderOptions
                                                  {
                                                      ModuleSize = dotSize,
                                                      MarginSize = margin,
                                                      ForeColor = foreColor,
                                                      BackColor = backColor
                                                  };
            return EncodeSvgImage(val, options);
        }

        public bool[,] EncodeRawData(string val)
        {
            return EncodeRawData(val, new DmtxImageEncoderOptions());
        }

        public bool[,] EncodeRawData(string val, DmtxImageEncoderOptions options)
        {
            DmtxEncode encode = new DmtxEncode
                                    {
                                        ModuleSize = 1,
                                        MarginSize = 0,
                                        SizeIdxRequest = options.SizeIdx,
                                        Scheme = options.Scheme
                                    };

            byte[] valAsByteArray = GetRawDataAndSetEncoding(val, options, encode);

            encode.EncodeDataMatrixRaw(valAsByteArray);

            return encode.RawData;
        }

        public string EncodeSvgImage(string val, DmtxImageEncoderOptions options)
        {
            DmtxEncode encode = new DmtxEncode
                                    {
                                        ModuleSize = options.ModuleSize,
                                        MarginSize = options.MarginSize,
                                        SizeIdxRequest = options.SizeIdx,
                                        Scheme = options.Scheme
                                    };

            byte[] valAsByteArray = GetRawDataAndSetEncoding(val, options, encode);

            encode.EncodeDataMatrix(options.ForeColor, options.BackColor, valAsByteArray);

            return EncodeSvgFile(encode, "", options.ModuleSize, options.MarginSize, options.ForeColor, options.BackColor);
        }

        internal static Bitmap CopyDataToBitmap(byte[] data, int width, int height)
        {
            data = InsertPaddingBytes(data, width, height, 24);

            Bitmap bmp = new Bitmap(width, height);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        private static byte[] InsertPaddingBytes(byte[] data, int width, int height, int bitsPerPixel)
        {
            int paddedWidth = 4 * ((width * bitsPerPixel + 31) / 32);
            int padding = paddedWidth - 3 * width;
            if (padding == 0)
            {
                return data;
            }
            byte[] newData = new byte[paddedWidth * height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    newData[i * paddedWidth + 3 * j] = data[3 * (i * width + j)];
                    newData[i * paddedWidth + 3 * j + 1] = data[3 * (i * width + j) + 1];
                    newData[i * paddedWidth + 3 * j + 2] = data[3 * (i * width + j) + 2];
                }
                for (int k = 0; k < padding; k++)
                {
                    newData[i * paddedWidth + 3 * k] = 255;
                    newData[i * paddedWidth + 3 * k + 1] = 255;
                    newData[i * paddedWidth + 3 * k + 2] = 255;
                }
            }
            return newData;
        }

        private static NumberFormatInfo _dotFormatProvider;

        internal string EncodeSvgFile(DmtxEncode enc, string format, int moduleSize, int margin, Color foreColor, Color backColor)
        {
            bool defineOnly = false;
            string idString = null;
            string style;
            string outputString = "";

            if (_dotFormatProvider == null)
            {
                _dotFormatProvider = new NumberFormatInfo {NumberDecimalSeparator = "."};
            }

            if (format == "svg:")
            {
                defineOnly = true;
                idString = format.Substring(4);
            }

            if (string.IsNullOrEmpty(idString))
            {
                idString = "dmtx_0001";
            }

            int width = 2 * enc.MarginSize + (enc.Region.SymbolCols * enc.ModuleSize);
            int height = 2 * enc.MarginSize + (enc.Region.SymbolRows * enc.ModuleSize);

            int symbolCols = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolCols, enc.Region.SizeIdx);
            int symbolRows = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolRows, enc.Region.SizeIdx);

            /* Print SVG Header */
            if (!defineOnly)
            {
                outputString += string.Format(
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>\n" +
                    "<!-- Created with DataMatrix.net (http://datamatrixnet.sourceforge.net/) -->\n" +
          "<svg\n" +
             "xmlns:svg=\"http://www.w3.org/2000/svg\"\n" +
             "xmlns=\"http://www.w3.org/2000/svg\"\n" +
             "xmlns:xlink=\"http://www.w3.org/1999/xlink\"\n" +
             "version=\"1.0\"\n" +
             "width=\"{0}\"\n" +
             "height=\"{1}\"\n" +
             "id=\"svg2\">\n" +
            "<defs>\n" +
            "<symbol id=\"{2}\">\n" +
                 "    <desc>Layout:{0}x%{1} Symbol:{3}x{4} Data Matrix</desc>\n", width, height, idString, symbolCols, symbolRows);
            }

            if (backColor != Color.White)
            {
                style = string.Format("style=\"fill:#{0}{1}{2};fill-opacity:{3};stroke:none\" ",
                              backColor.R.ToString("X2"), backColor.G.ToString("X2"), backColor.B.ToString("X2"), ((double)backColor.A / (double)byte.MaxValue).ToString("0.##", _dotFormatProvider));
                outputString += string.Format("    <rect width=\"{0}\" height=\"{1}\" x=\"0\" y=\"0\" {2}/>\n",
                      width, height, style);
            }

            /* Write Data Matrix ON modules */
            for (int row = 0; row < enc.Region.SymbolRows; row++)
            {
                int rowInv = enc.Region.SymbolRows - row - 1;
                for (int col = 0; col < enc.Region.SymbolCols; col++)
                {
                    int module = enc.Message.SymbolModuleStatus(enc.Region.SizeIdx, row, col);
                    style = string.Format("style=\"fill:#{0}{1}{2};fill-opacity:{3};stroke:none\" ",
                          foreColor.R.ToString("X2"), foreColor.G.ToString("X2"), foreColor.B.ToString("X2"), ((double)foreColor.A / (double)byte.MaxValue).ToString("0.##", _dotFormatProvider));

                    if ((module & DmtxConstants.DmtxModuleOn) != 0)
                    {
                        outputString += string.Format("    <rect width=\"{0}\" height=\"{1}\" x=\"{2}\" y=\"{3}\" {4}/>\n",
                              moduleSize, moduleSize,
                              col * moduleSize + margin,
                              rowInv * moduleSize + margin, style);
                    }
                }
            }

            outputString += "  </symbol>\n";

            /* Close SVG document */
            if (!defineOnly)
            {
                outputString += string.Format("</defs>\n" +
            "<use xlink:href=\"#{0}\" x='0' y='0' style=\"fill:#000000;fill-opacity:1;stroke:none\" />\n" +
          "\n</svg>\n", idString);
            }

            return outputString;
        }
    }
}

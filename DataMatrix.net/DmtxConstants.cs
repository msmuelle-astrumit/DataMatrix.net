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

namespace DataMatrix.net
{
    internal static class DmtxConstants
    {
        internal static readonly double DmtxAlmostZero = 0.000001;

        internal static readonly int DmtxModuleOff = 0x00;
        internal static readonly int DmtxModuleOnRed = 0x01;
        internal static readonly int DmtxModuleOnGreen = 0x02;
        internal static readonly int DmtxModuleOnBlue = 0x04;
        internal static readonly int DmtxModuleOnRGB = 0x07; /* OnRed | OnGreen | OnBlue */
        internal static readonly int DmtxModuleOn = 0x07;
        internal static readonly int DmtxModuleUnsure = 0x08;
        internal static readonly int DmtxModuleAssigned = 0x10;
        internal static readonly int DmtxModuleVisited = 0x20;
        internal static readonly int DmtxModuleData = 0x40;

        internal static readonly byte DmtxCharAsciiPad = 129;
        internal static readonly byte DmtxCharAsciiUpperShift = 235;
        internal static readonly byte DmtxCharTripletShift1 = 0;
        internal static readonly byte DmtxCharTripletShift2 = 1;
        internal static readonly byte DmtxCharTripletShift3 = 2;
        internal static readonly byte DmtxCharFNC1 = 232;
        internal static readonly byte DmtxCharStructuredAppend = 233;
        internal static readonly byte DmtxChar05Macro = 236;
        internal static readonly byte DmtxChar06Macro = 237;

        internal static readonly int DmtxC40TextBasicSet = 0;
        internal static readonly int DmtxC40TextShift1 = 1;
        internal static readonly int DmtxC40TextShift2 = 2;
        internal static readonly int DmtxC40TextShift3 = 3;

        internal static readonly int DmtxCharTripletUnlatch = 254;
        internal static readonly int DmtxCharEdifactUnlatch = 31;

        internal static readonly byte DmtxCharC40Latch = 230;
        internal static readonly byte DmtxCharTextLatch = 239;
        internal static readonly byte DmtxCharX12Latch = 238;
        internal static readonly byte DmtxCharEdifactLatch = 240;
        internal static readonly byte DmtxCharBase256Latch = 231;

        internal static readonly int[] SymbolRows = new int[] { 10, 12, 14, 16, 18, 20,  22,  24,  26,
                                                 32, 36, 40,  44,  48,  52,
                                                 64, 72, 80,  88,  96, 104,
                                                        120, 132, 144,
                                                  8,  8, 12,  12,  16,  16 };

        internal static readonly int[] SymbolCols = new int[] { 10, 12, 14, 16, 18, 20,  22,  24,  26,
                                                 32, 36, 40,  44,  48,  52,
                                                 64, 72, 80,  88,  96, 104,
                                                        120, 132, 144,
                                                 18, 32, 26,  36,  36,  48 };

        internal static readonly int[] DataRegionRows = new int[] { 8, 10, 12, 14, 16, 18, 20, 22, 24,
                                                    14, 16, 18, 20, 22, 24,
                                                    14, 16, 18, 20, 22, 24,
                                                            18, 20, 22,
                                                     6,  6, 10, 10, 14, 14 };

        internal static readonly int[] DataRegionCols = new int[] { 8, 10, 12, 14, 16, 18, 20, 22, 24,
                                                    14, 16, 18, 20, 22, 24,
                                                    14, 16, 18, 20, 22, 24,
                                                            18, 20, 22,
                                                    16, 14, 24, 16, 16, 22 };

        internal static readonly int[] HorizDataRegions = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                                    2, 2, 2, 2, 2, 2,
                                                    4, 4, 4, 4, 4, 4,
                                                          6, 6, 6,
                                                    1, 2, 1, 2, 2, 2 };

        internal static readonly int[] InterleavedBlocks = new int[] { 1, 1, 1, 1, 1, 1, 1,  1, 1,
                                                     1, 1, 1, 1,  1, 2,
                                                     2, 4, 4, 4,  4, 6,
                                                           6, 8, 10,
                                                     1, 1, 1, 1,  1, 1 };

        internal static readonly int[] SymbolDataWords = new int[] { 3, 5, 8,  12,   18,   22,   30,   36,  44,
                                                    62,   86,  114,  144,  174, 204,
                                                   280,  368,  456,  576,  696, 816,
                                                              1050, 1304, 1558,
                                                     5,   10,   16,   22,   32,  49 };

        internal static readonly int[] BlockErrorWords = new int[] { 5, 7, 10, 12, 14, 18, 20, 24, 28,
                                                    36, 42, 48, 56, 68, 42,
                                                    56, 36, 48, 56, 68, 56,
                                                            68, 62, 62,
                                                     7, 11, 14, 18, 24, 28 };

        internal static readonly int[] BlockMaxCorrectable = new int[] { 2, 3, 5,  6,  7,  9,  10,  12,  14,
                                                       18, 21, 24,  28,  34,  21,
                                                       28, 18, 24,  28,  34,  28,
                                                               34,  31,  31,
                                                   3,  5,  7,   9,  12,  14 };
        internal static readonly int DmtxSymbolSquareCount = 24;
        internal static readonly int DmtxSymbolRectCount = 6;
        internal static readonly int DmtxUndefined = -1;

        internal static readonly int[] DmtxPatternX = new int[] { -1, 0, 1, 1, 1, 0, -1, -1 };
        internal static readonly int[] DmtxPatternY = new int[] { -1, -1, -1, 0, 1, 1, 1, 0 };
        internal static readonly DmtxPointFlow DmtxBlankEdge = new DmtxPointFlow() { Plane = 0, Arrive = 0, Depart = 0, Mag = DmtxConstants.DmtxUndefined, Loc = new DmtxPixelLoc() { X = -1, Y = -1 } };

        internal static readonly int DmtxHoughRes = 180;
        internal static readonly int DmtxNeighborNone = 8;

        internal static readonly int[] rHvX =
    {  256,  256,  256,  256,  255,  255,  255,  254,  254,  253,  252,  251,  250,  249,  248,
       247,  246,  245,  243,  242,  241,  239,  237,  236,  234,  232,  230,  228,  226,  224,
       222,  219,  217,  215,  212,  210,  207,  204,  202,  199,  196,  193,  190,  187,  184,
       181,  178,  175,  171,  168,  165,  161,  158,  154,  150,  147,  143,  139,  136,  132,
       128,  124,  120,  116,  112,  108,  104,  100,   96,   92,   88,   83,   79,   75,   71,
        66,   62,   58,   53,   49,   44,   40,   36,   31,   27,   22,   18,   13,    9,    4,
         0,   -4,   -9,  -13,  -18,  -22,  -27,  -31,  -36,  -40,  -44,  -49,  -53,  -58,  -62,
       -66,  -71,  -75,  -79,  -83,  -88,  -92,  -96, -100, -104, -108, -112, -116, -120, -124,
      -128, -132, -136, -139, -143, -147, -150, -154, -158, -161, -165, -168, -171, -175, -178,
      -181, -184, -187, -190, -193, -196, -199, -202, -204, -207, -210, -212, -215, -217, -219,
      -222, -224, -226, -228, -230, -232, -234, -236, -237, -239, -241, -242, -243, -245, -246,
      -247, -248, -249, -250, -251, -252, -253, -254, -254, -255, -255, -255, -256, -256, -256 };

        internal static readonly int[] rHvY =
    {    0,    4,    9,   13,   18,   22,   27,   31,   36,   40,   44,   49,   53,   58,   62,
        66,   71,   75,   79,   83,   88,   92,   96,  100,  104,  108,  112,  116,  120,  124,
       128,  132,  136,  139,  143,  147,  150,  154,  158,  161,  165,  168,  171,  175,  178,
       181,  184,  187,  190,  193,  196,  199,  202,  204,  207,  210,  212,  215,  217,  219,
       222,  224,  226,  228,  230,  232,  234,  236,  237,  239,  241,  242,  243,  245,  246,
       247,  248,  249,  250,  251,  252,  253,  254,  254,  255,  255,  255,  256,  256,  256,
       256,  256,  256,  256,  255,  255,  255,  254,  254,  253,  252,  251,  250,  249,  248,
       247,  246,  245,  243,  242,  241,  239,  237,  236,  234,  232,  230,  228,  226,  224,
       222,  219,  217,  215,  212,  210,  207,  204,  202,  199,  196,  193,  190,  187,  184,
       181,  178,  175,  171,  168,  165,  161,  158,  154,  150,  147,  143,  139,  136,  132,
       128,  124,  120,  116,  112,  108,  104,  100,   96,   92,   88,   83,   79,   75,   71,
        66,   62,   58,   53,   49,   44,   40,   36,   31,   27,   22,   18,   13,    9,    4 };

        internal static readonly int[] aLogVal =
   {   1,   2,   4,   8,  16,  32,  64, 128,  45,  90, 180,  69, 138,  57, 114, 228,
     229, 231, 227, 235, 251, 219, 155,  27,  54, 108, 216, 157,  23,  46,  92, 184,
      93, 186,  89, 178,  73, 146,   9,  18,  36,  72, 144,  13,  26,  52, 104, 208,
     141,  55, 110, 220, 149,   7,  14,  28,  56, 112, 224, 237, 247, 195, 171, 123,
     246, 193, 175, 115, 230, 225, 239, 243, 203, 187,  91, 182,  65, 130,  41,  82,
     164, 101, 202, 185,  95, 190,  81, 162, 105, 210, 137,  63, 126, 252, 213, 135,
      35,  70, 140,  53, 106, 212, 133,  39,  78, 156,  21,  42,  84, 168, 125, 250,
     217, 159,  19,  38,  76, 152,  29,  58, 116, 232, 253, 215, 131,  43,  86, 172,
     117, 234, 249, 223, 147,  11,  22,  44,  88, 176,  77, 154,  25,  50, 100, 200,
     189,  87, 174, 113, 226, 233, 255, 211, 139,  59, 118, 236, 245, 199, 163, 107,
     214, 129,  47,  94, 188,  85, 170, 121, 242, 201, 191,  83, 166,  97, 194, 169,
     127, 254, 209, 143,  51, 102, 204, 181,  71, 142,  49,  98, 196, 165, 103, 206,
     177,  79, 158,  17,  34,  68, 136,  61, 122, 244, 197, 167,  99, 198, 161, 111,
     222, 145,  15,  30,  60, 120, 240, 205, 183,  67, 134,  33,  66, 132,  37,  74,
     148,   5,  10,  20,  40,  80, 160, 109, 218, 153,  31,  62, 124, 248, 221, 151,
       3,   6,  12,  24,  48,  96, 192, 173, 119, 238, 241, 207, 179,  75, 150,   1 };

         internal static readonly int[] logVal =
   {-255, 255,   1, 240,   2, 225, 241,  53,   3,  38, 226, 133, 242,  43,  54, 210,
       4, 195,  39, 114, 227, 106, 134,  28, 243, 140,  44,  23,  55, 118, 211, 234,
       5, 219, 196,  96,  40, 222, 115, 103, 228,  78, 107, 125, 135,   8,  29, 162,
     244, 186, 141, 180,  45,  99,  24,  49,  56,  13, 119, 153, 212, 199, 235,  91,
       6,  76, 220, 217, 197,  11,  97, 184,  41,  36, 223, 253, 116, 138, 104, 193,
     229,  86,  79, 171, 108, 165, 126, 145, 136,  34,   9,  74,  30,  32, 163,  84,
     245, 173, 187, 204, 142,  81, 181, 190,  46,  88, 100, 159,  25, 231,  50, 207,
      57, 147,  14,  67, 120, 128, 154, 248, 213, 167, 200,  63, 236, 110,  92, 176,
       7, 161,  77, 124, 221, 102, 218,  95, 198,  90,  12, 152,  98,  48, 185, 179,
      42, 209,  37, 132, 224,  52, 254, 239, 117, 233, 139,  22, 105,  27, 194, 113,
     230, 206,  87, 158,  80, 189, 172, 203, 109, 175, 166,  62, 127, 247, 146,  66,
     137, 192,  35, 252,  10, 183,  75, 216,  31,  83,  33,  73, 164, 144,  85, 170,
     246,  65, 174,  61, 188, 202, 205, 157, 143, 169,  82,  72, 182, 215, 191, 251,
      47, 178,  89, 151, 101,  94, 160, 123,  26, 112, 232,  21,  51, 238, 208, 131,
      58,  69, 148,  18,  15,  16,  68,  17, 121, 149, 129,  19, 155,  59, 249,  70,
     214, 250, 168,  71, 201, 156,  64,  60, 237, 130, 111,  20,  93, 122, 177, 150 };

    }

    internal enum DmtxFormat
    {
        Matrix,
        Mosaic,
    }

    internal enum DmtxSymAttribute
    {
        DmtxSymAttribSymbolRows,
        DmtxSymAttribSymbolCols,
        DmtxSymAttribDataRegionRows,
        DmtxSymAttribDataRegionCols,
        DmtxSymAttribHorizDataRegions,
        DmtxSymAttribVertDataRegions,
        DmtxSymAttribMappingMatrixRows,
        DmtxSymAttribMappingMatrixCols,
        DmtxSymAttribInterleavedBlocks,
        DmtxSymAttribBlockErrorWords,
        DmtxSymAttribBlockMaxCorrectable,
        DmtxSymAttribSymbolDataWords,
        DmtxSymAttribSymbolErrorWords,
        DmtxSymAttribSymbolMaxCorrectable
    }

    public enum DmtxSymbolSize
    {
        DmtxSymbolRectAuto = -3,
        DmtxSymbolSquareAuto = -2,
        DmtxSymbolShapeAuto = -1,
        DmtxSymbol10x10 = 0,
        DmtxSymbol12x12,
        DmtxSymbol14x14,
        DmtxSymbol16x16,
        DmtxSymbol18x18,
        DmtxSymbol20x20,
        DmtxSymbol22x22,
        DmtxSymbol24x24,
        DmtxSymbol26x26,
        DmtxSymbol32x32,
        DmtxSymbol36x36,
        DmtxSymbol40x40,
        DmtxSymbol44x44,
        DmtxSymbol48x48,
        DmtxSymbol52x52,
        DmtxSymbol64x64,
        DmtxSymbol72x72,
        DmtxSymbol80x80,
        DmtxSymbol88x88,
        DmtxSymbol96x96,
        DmtxSymbol104x104,
        DmtxSymbol120x120,
        DmtxSymbol132x132,
        DmtxSymbol144x144,
        DmtxSymbol8x18,
        DmtxSymbol8x32,
        DmtxSymbol12x26,
        DmtxSymbol12x36,
        DmtxSymbol16x36,
        DmtxSymbol16x48
    }

    internal enum DmtxFlip
    {
        DmtxFlipNone = 0x00,
        DmtxFlipX = 0x01 << 0,
        DmtxFlipY = 0x01 << 1
    }

    internal enum DmtxPackOrder
    {
        /* Custom format */
        DmtxPackCustom = 100,
        /* 1 bpp */
        DmtxPack1bppK = 200,
        /* 8 bpp grayscale */
        DmtxPack8bppK = 300,
        /* 16 bpp formats */
        DmtxPack16bppRGB = 400,
        DmtxPack16bppRGBX,
        DmtxPack16bppXRGB,
        DmtxPack16bppBGR,
        DmtxPack16bppBGRX,
        DmtxPack16bppXBGR,
        DmtxPack16bppYCbCr,
        /* 24 bpp formats */
        DmtxPack24bppRGB = 500,
        DmtxPack24bppBGR,
        DmtxPack24bppYCbCr,
        /* 32 bpp formats */
        DmtxPack32bppRGBX = 600,
        DmtxPack32bppXRGB,
        DmtxPack32bppBGRX,
        DmtxPack32bppXBGR,
        DmtxPack32bppCMYK
    }

    internal enum DmtxRange
    {
        DmtxRangeGood,
        DmtxRangeBad,
        DmtxRangeEnd
    }

    internal enum DmtxDirection
    {
        DmtxDirNone = 0x00,
        DmtxDirUp = 0x01 << 0,
        DmtxDirLeft = 0x01 << 1,
        DmtxDirDown = 0x01 << 2,
        DmtxDirRight = 0x01 << 3,
        DmtxDirHorizontal = DmtxDirLeft | DmtxDirRight,
        DmtxDirVertical = DmtxDirUp | DmtxDirDown,
        DmtxDirRightUp = DmtxDirRight | DmtxDirUp,
        DmtxDirLeftDown = DmtxDirLeft | DmtxDirDown
    }

    public enum DmtxScheme
    {
        DmtxSchemeAutoFast = -2,
        DmtxSchemeAutoBest = -1,
        DmtxSchemeAscii = 0,
        DmtxSchemeC40,
        DmtxSchemeText,
        DmtxSchemeX12,
        DmtxSchemeEdifact,
        DmtxSchemeBase256,
        DmtxSchemeAsciiGS1
    }

    internal enum DmtxMaskBit
    {
        DmtxMaskBit8 = 0x01 << 0,
        DmtxMaskBit7 = 0x01 << 1,
        DmtxMaskBit6 = 0x01 << 2,
        DmtxMaskBit5 = 0x01 << 3,
        DmtxMaskBit4 = 0x01 << 4,
        DmtxMaskBit3 = 0x01 << 5,
        DmtxMaskBit2 = 0x01 << 6,
        DmtxMaskBit1 = 0x01 << 7
    }

    internal enum DmtxEdge
    {
        DmtxEdgeTop = 0x01 << 0,
        DmtxEdgeBottom = 0x01 << 1,
        DmtxEdgeLeft = 0x01 << 2,
        DmtxEdgeRight = 0x01 << 3
    }

    enum DmtxChannelStatus
    {
        DmtxChannelValid = 0x00,
        DmtxChannelUnsupportedChar = 0x01 << 0,
        DmtxChannelCannotUnlatch = 0x01 << 1
    }

    enum DmtxUnlatch
    {
        Explicit,
        Implicit
    }
}

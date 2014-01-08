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

namespace DataMatrix.net
{
    internal static class DmtxCommon
    {
        internal static void GenReedSolEcc(DmtxMessage message, DmtxSymbolSize sizeIdx)
        {
            byte[] g = new byte[69];
            byte[] b = new byte[68];

            int symbolDataWords = GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolDataWords, sizeIdx);
            int symbolErrorWords = GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolErrorWords, sizeIdx);
            int symbolTotalWords = symbolDataWords + symbolErrorWords;
            int blockErrorWords = GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribBlockErrorWords, sizeIdx);
            int step = GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribInterleavedBlocks, sizeIdx);
            if (blockErrorWords != symbolErrorWords / step)
            {
                throw new Exception("Error generation reed solomon error correction");
            }

            for (int gI = 0; gI < g.Length; gI++)
            {
                g[gI] = 0x01;
            }

            // Generate ECC polynomia
            for (int i = 1; i <= blockErrorWords; i++)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    g[j] = GfDoublify(g[j], i);     // g[j] *= 2**i
                    if (j > 0)
                        g[j] = GfSum(g[j], g[j - 1]);  // g[j] += g[j-1]
                }
            }

            // Populate error codeword array
            for (int block = 0; block < step; block++)
            {
                for (int bI = 0; bI < b.Length; bI++)
                {
                    b[bI] = 0;
                }

                for (int i = block; i < symbolDataWords; i += step)
                {
                    int val = GfSum(b[blockErrorWords - 1], message.Code[i]);
                    for (int j = blockErrorWords - 1; j > 0; j--)
                    {
                        b[j] = GfSum(b[j - 1], GfProduct(g[j], val));
                    }
                    b[0] = GfProduct(g[0], val);
                }

                int blockDataWords = GetBlockDataSize(sizeIdx, block);
                int bIndex = blockErrorWords;

                for (int i = block + (step * blockDataWords); i < symbolTotalWords; i += step)
                {
                    message.Code[i] = b[--bIndex];
                }

                if (bIndex != 0)
                {
                    throw new Exception("Error generation error correction code!");
                }
            }
        }

        private static byte GfProduct(byte a, int b)
        {
            if (a == 0 || b == 0)
                return 0;
            
            return (byte)DmtxConstants.aLogVal[(DmtxConstants.logVal[a] + DmtxConstants.logVal[b]) % 255];
        }

        private static byte GfSum(byte a, byte b)
        {
            return (byte)(a ^ b);
        }

        private static byte GfDoublify(byte a, int b)
        {
            if (a == 0) /* XXX this is right, right? */
                return 0;
            if (b == 0)
                return a; /* XXX this is right, right? */
            
            return (byte)DmtxConstants.aLogVal[(DmtxConstants.logVal[a] + b) % 255];
        }

        internal static int GetSymbolAttribute(DmtxSymAttribute attribute, DmtxSymbolSize sizeIdx)
        {
            if (sizeIdx < 0 || (int)sizeIdx >= DmtxConstants.DmtxSymbolSquareCount + DmtxConstants.DmtxSymbolRectCount)
                return DmtxConstants.DmtxUndefined;

            switch (attribute)
            {
                case DmtxSymAttribute.DmtxSymAttribSymbolRows:
                    return DmtxConstants.SymbolRows[(int)sizeIdx];
                case DmtxSymAttribute.DmtxSymAttribSymbolCols:
                    return DmtxConstants.SymbolCols[(int)sizeIdx];
                case DmtxSymAttribute.DmtxSymAttribDataRegionRows:
                    return DmtxConstants.DataRegionRows[(int)sizeIdx];
                case DmtxSymAttribute.DmtxSymAttribDataRegionCols:
                    return DmtxConstants.DataRegionCols[(int)sizeIdx];
                case DmtxSymAttribute.DmtxSymAttribHorizDataRegions:
                    return DmtxConstants.HorizDataRegions[(int)sizeIdx];
                case DmtxSymAttribute.DmtxSymAttribVertDataRegions:
                    return ((int)sizeIdx < DmtxConstants.DmtxSymbolSquareCount) ? DmtxConstants.HorizDataRegions[(int)sizeIdx] : 1;
                case DmtxSymAttribute.DmtxSymAttribMappingMatrixRows:
                    return DmtxConstants.DataRegionRows[(int)sizeIdx] *
                          GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribVertDataRegions, sizeIdx);
                case DmtxSymAttribute.DmtxSymAttribMappingMatrixCols:
                    return DmtxConstants.DataRegionCols[(int)sizeIdx] * DmtxConstants.HorizDataRegions[(int)sizeIdx];
                case DmtxSymAttribute.DmtxSymAttribInterleavedBlocks:
                    return DmtxConstants.InterleavedBlocks[(int)sizeIdx];
                case DmtxSymAttribute.DmtxSymAttribBlockErrorWords:
                    return DmtxConstants.BlockErrorWords[(int)sizeIdx];
                case DmtxSymAttribute.DmtxSymAttribBlockMaxCorrectable:
                    return DmtxConstants.BlockMaxCorrectable[(int)sizeIdx];
                case DmtxSymAttribute.DmtxSymAttribSymbolDataWords:
                    return DmtxConstants.SymbolDataWords[(int)sizeIdx];
                case DmtxSymAttribute.DmtxSymAttribSymbolErrorWords:
                    return DmtxConstants.BlockErrorWords[(int)sizeIdx] * DmtxConstants.InterleavedBlocks[(int)sizeIdx];
                case DmtxSymAttribute.DmtxSymAttribSymbolMaxCorrectable:
                    return DmtxConstants.BlockMaxCorrectable[(int)sizeIdx] * DmtxConstants.InterleavedBlocks[(int)sizeIdx];
            }
            return DmtxConstants.DmtxUndefined;
        }

        internal static int GetBlockDataSize(DmtxSymbolSize sizeIdx, int blockIdx)
        {
            int symbolDataWords = GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolDataWords, sizeIdx);
            int interleavedBlocks = GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribInterleavedBlocks, sizeIdx);
            int count = symbolDataWords / interleavedBlocks;

            if (symbolDataWords < 1 || interleavedBlocks < 1)
                return DmtxConstants.DmtxUndefined;

            return (sizeIdx == DmtxSymbolSize.DmtxSymbol144x144 && blockIdx < 8) ? count + 1 : count;
        }

        internal static DmtxSymbolSize FindCorrectSymbolSize(int dataWords, DmtxSymbolSize sizeIdxRequest)
        {
            DmtxSymbolSize sizeIdx;
            if (dataWords <= 0)
            {
                return DmtxSymbolSize.DmtxSymbolShapeAuto;
            }

            if (sizeIdxRequest == DmtxSymbolSize.DmtxSymbolSquareAuto || sizeIdxRequest == DmtxSymbolSize.DmtxSymbolRectAuto)
            {
                DmtxSymbolSize idxBeg;
                DmtxSymbolSize idxEnd;
                if (sizeIdxRequest == DmtxSymbolSize.DmtxSymbolSquareAuto)
                {
                    idxBeg = 0;
                    idxEnd = (DmtxSymbolSize)DmtxConstants.DmtxSymbolSquareCount;
                }
                else
                {
                    idxBeg = (DmtxSymbolSize)DmtxConstants.DmtxSymbolSquareCount;
                    idxEnd = (DmtxSymbolSize)(DmtxConstants.DmtxSymbolSquareCount + DmtxConstants.DmtxSymbolRectCount);
                }

                for (sizeIdx = idxBeg; sizeIdx < idxEnd; sizeIdx++)
                {
                    if (GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolDataWords, sizeIdx) >= dataWords)
                        break;
                }

                if (sizeIdx == idxEnd)
                {
                    return DmtxSymbolSize.DmtxSymbolShapeAuto;
                }
            }
            else
            {
                sizeIdx = sizeIdxRequest;
            }

            if (dataWords > GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolDataWords, sizeIdx))
            {
                return DmtxSymbolSize.DmtxSymbolShapeAuto;
            }

            return sizeIdx;
        }

        internal static int GetBitsPerPixel(DmtxPackOrder pack)
        {
            switch (pack)
            {
                case DmtxPackOrder.DmtxPack1bppK:
                    return 1;
                case DmtxPackOrder.DmtxPack8bppK:
                    return 8;
                case DmtxPackOrder.DmtxPack16bppRGB:
                case DmtxPackOrder.DmtxPack16bppRGBX:
                case DmtxPackOrder.DmtxPack16bppXRGB:
                case DmtxPackOrder.DmtxPack16bppBGR:
                case DmtxPackOrder.DmtxPack16bppBGRX:
                case DmtxPackOrder.DmtxPack16bppXBGR:
                case DmtxPackOrder.DmtxPack16bppYCbCr:
                    return 16;
                case DmtxPackOrder.DmtxPack24bppRGB:
                case DmtxPackOrder.DmtxPack24bppBGR:
                case DmtxPackOrder.DmtxPack24bppYCbCr:
                    return 24;
                case DmtxPackOrder.DmtxPack32bppRGBX:
                case DmtxPackOrder.DmtxPack32bppXRGB:
                case DmtxPackOrder.DmtxPack32bppBGRX:
                case DmtxPackOrder.DmtxPack32bppXBGR:
                case DmtxPackOrder.DmtxPack32bppCMYK:
                    return 32;
            }

            return DmtxConstants.DmtxUndefined;
        }

        internal static T Min<T>(T x, T y) where T : IComparable<T>
        {
            return x.CompareTo(y) < 0 ? x : y;
        }

        internal static T Max<T>(T x, T y) where T : IComparable<T>
        {
            return x.CompareTo(y) < 0 ? y : x;
        }

        internal static bool DecodeCheckErrors(byte[] code, int codeIndex, DmtxSymbolSize sizeIdx, int fix)
        {
            byte[] data = new byte[255];

            int interleavedBlocks = GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribInterleavedBlocks, sizeIdx);
            int blockErrorWords = GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribBlockErrorWords, sizeIdx);

            const int fixedErr = 0;
            int fixedErrSum = 0;
            for (int i = 0; i < interleavedBlocks; i++)
            {
                int blockTotalWords = blockErrorWords + GetBlockDataSize(sizeIdx, i);

                int j;
                for (j = 0; j < blockTotalWords; j++)
                    data[j] = code[j * interleavedBlocks + i];

                fixedErrSum += fixedErr;

                for (j = 0; j < blockTotalWords; j++)
                    code[j * interleavedBlocks + i] = data[j];
            }

            if (fix != DmtxConstants.DmtxUndefined && fix >= 0 && fix < fixedErrSum)
            {
                return false;
            }

            return true;
        }

        internal static double RightAngleTrueness(DmtxVector2 c0, DmtxVector2 c1, DmtxVector2 c2, double angle)
        {
            DmtxVector2 vA = (c0 - c1);
            DmtxVector2 vB = (c2 - c1);
            vA.Norm();
            vB.Norm();

            DmtxMatrix3 m = DmtxMatrix3.Rotate(angle);
            vB *= m;

            return vA.Dot(vB);
        }

    }
}

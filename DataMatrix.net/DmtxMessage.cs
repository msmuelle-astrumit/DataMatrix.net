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
    internal class DmtxMessage
    {
        #region Fields
        int _outputIdx;     /* Internal index used to store output progress */

        #endregion

        #region Constructors
        internal DmtxMessage(DmtxSymbolSize sizeIdx, DmtxFormat symbolFormat)
        {
            if (symbolFormat != DmtxFormat.Matrix && symbolFormat != DmtxFormat.Mosaic)
            {
                throw new ArgumentException("Only DmtxFormats Matrix and Mosaic are currently supported");
            }
            int mappingRows = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribMappingMatrixRows, sizeIdx);
            int mappingCols = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribMappingMatrixCols, sizeIdx);

            this.Array = new byte[mappingCols * mappingRows];

            int codeSize = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolDataWords, sizeIdx) + DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolErrorWords, sizeIdx);
            this.Code = new byte[codeSize];

            this.Output = new byte[10 * codeSize];
        }
        #endregion

        #region Methods
        internal void DecodeDataStream(DmtxSymbolSize sizeIdx, byte[] outputStart)
        {
            bool macro = false;

            this.Output = outputStart ?? this.Output;
            this._outputIdx = 0;

            byte[] ptr = this.Code;
            int dataEndIndex = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolDataWords, sizeIdx);

            /* Print macro header if first codeword triggers it */
            if (ptr[0] == DmtxConstants.DmtxChar05Macro || ptr[0] == DmtxConstants.DmtxChar06Macro)
            {
                PushOutputMacroHeader(ptr[0]);
                macro = true;
            }

            for (int codeIter = 0; codeIter < dataEndIndex; )
            {

                DmtxScheme encScheme = GetEncodationScheme(this.Code[codeIter]);
                if (encScheme != DmtxScheme.DmtxSchemeAscii)
                    codeIter++;

                switch (encScheme)
                {
                    case DmtxScheme.DmtxSchemeAscii:
                        codeIter = DecodeSchemeAscii(codeIter, dataEndIndex);
                        break;
                    case DmtxScheme.DmtxSchemeC40:
                    case DmtxScheme.DmtxSchemeText:
                        codeIter = DecodeSchemeC40Text(codeIter, dataEndIndex, encScheme);
                        break;
                    case DmtxScheme.DmtxSchemeX12:
                        codeIter = DecodeSchemeX12(codeIter, dataEndIndex);
                        break;
                    case DmtxScheme.DmtxSchemeEdifact:
                        codeIter = DecodeSchemeEdifact(codeIter, dataEndIndex);
                        break;
                    case DmtxScheme.DmtxSchemeBase256:
                        codeIter = DecodeSchemeBase256(codeIter, dataEndIndex);
                        break;
                }
            }

            /* Print macro trailer if required */
            if (macro)
            {
                PushOutputMacroTrailer();
            }
        }

        private void PushOutputMacroHeader(byte macroType)
        {
            PushOutputWord((byte)'[');
            PushOutputWord((byte)')');
            PushOutputWord((byte)'>');
            PushOutputWord(30); /* ASCII RS */
            PushOutputWord((byte)'0');

            if (macroType == DmtxConstants.DmtxChar05Macro)
            {
                PushOutputWord((byte)'5');
            }
            else if (macroType == DmtxConstants.DmtxChar06Macro)
            {
                PushOutputWord((byte)'6');
            }
            else
            {
                throw new ArgumentException("Macro Header only supported for char05 and char06");
            }

            PushOutputWord(29); /* ASCII GS */
        }

        void PushOutputMacroTrailer()
        {
            PushOutputWord(30); /* ASCII RS */
            PushOutputWord(4);  /* ASCII EOT */
        }

        void PushOutputWord(byte value)
        {
            this.Output[this._outputIdx++] = value;
        }

        static DmtxScheme GetEncodationScheme(byte val)
        {
            if (val == DmtxConstants.DmtxCharC40Latch)
            {
                return DmtxScheme.DmtxSchemeC40;
            }
            if (val == DmtxConstants.DmtxCharBase256Latch)
            {
                return DmtxScheme.DmtxSchemeBase256;
            }
            if (val == DmtxConstants.DmtxCharEdifactLatch)
            {
                return DmtxScheme.DmtxSchemeEdifact;
            }
            if (val == DmtxConstants.DmtxCharTextLatch)
            {
                return DmtxScheme.DmtxSchemeText;
            }
            if (val == DmtxConstants.DmtxCharX12Latch)
            {
                return DmtxScheme.DmtxSchemeX12;
            }
            return DmtxScheme.DmtxSchemeAscii;
        }

        int DecodeSchemeAscii(int startIndex, int endIndex)
        {
            bool upperShift = false;

            while (startIndex < endIndex)
            {

                byte codeword = this.Code[startIndex];

                if (GetEncodationScheme(this.Code[startIndex]) != DmtxScheme.DmtxSchemeAscii)
                    return startIndex;
                
                startIndex++;

                if (upperShift)
                {
                    PushOutputWord((byte)(codeword + 127));
                    upperShift = false;
                }
                else if (codeword == DmtxConstants.DmtxCharAsciiUpperShift)
                {
                    upperShift = true;
                }
                else if (codeword == DmtxConstants.DmtxCharAsciiPad)
                {
                    this.PadCount = endIndex - startIndex;
                    return endIndex;
                }
                else if (codeword <= 128)
                {
                    PushOutputWord((byte)(codeword - 1));
                }
                else if (codeword <= 229)
                {
                    byte digits = (byte)(codeword - 130);
                    PushOutputWord((byte)(digits / 10 + '0'));
                    PushOutputWord((byte)(digits - (digits / 10) * 10 + '0'));
                }
            }

            return startIndex;
        }

        int DecodeSchemeC40Text(int startIndex, int endIndex, DmtxScheme encScheme)
        {
            int[] c40Values = new int[3];
            C40TextState state = new C40TextState {Shift = DmtxConstants.DmtxC40TextBasicSet, UpperShift = false};


            if (!(encScheme == DmtxScheme.DmtxSchemeC40 || encScheme == DmtxScheme.DmtxSchemeText))
            {
                throw new ArgumentException("Invalid scheme selected for decodind!");
            }

            while (startIndex < endIndex)
            {

                /* FIXME Also check that ptr+1 is safe to access */
                int packed = (this.Code[startIndex] << 8) | this.Code[startIndex + 1];
                c40Values[0] = ((packed - 1) / 1600);
                c40Values[1] = ((packed - 1) / 40) % 40;
                c40Values[2] = (packed - 1) % 40;
                startIndex += 2;

                int i;
                for (i = 0; i < 3; i++)
                {
                    if (state.Shift == DmtxConstants.DmtxC40TextBasicSet)
                    { /* Basic set */
                        if (c40Values[i] <= 2)
                        {
                            state.Shift = c40Values[i] + 1;
                        }
                        else if (c40Values[i] == 3)
                        {
                            PushOutputC40TextWord(ref state, ' ');
                        }
                        else if (c40Values[i] <= 13)
                        {
                            PushOutputC40TextWord(ref state, c40Values[i] - 13 + '9'); /* 0-9 */
                        }
                        else if (c40Values[i] <= 39)
                        {
                            if (encScheme == DmtxScheme.DmtxSchemeC40)
                            {
                                PushOutputC40TextWord(ref state, c40Values[i] - 39 + 'Z'); /* A-Z */
                            }
                            else if (encScheme == DmtxScheme.DmtxSchemeText)
                            {
                                PushOutputC40TextWord(ref state, c40Values[i] - 39 + 'z'); /* a-z */
                            }
                        }
                    }
                    else if (state.Shift == DmtxConstants.DmtxC40TextShift1)
                    { /* Shift 1 set */
                        PushOutputC40TextWord(ref state, c40Values[i]); /* ASCII 0 - 31 */
                    }
                    else if (state.Shift == DmtxConstants.DmtxC40TextShift2)
                    { /* Shift 2 set */
                        if (c40Values[i] <= 14)
                        {
                            PushOutputC40TextWord(ref state, c40Values[i] + 33); /* ASCII 33 - 47 */
                        }
                        else if (c40Values[i] <= 21)
                        {
                            PushOutputC40TextWord(ref state, c40Values[i] + 43); /* ASCII 58 - 64 */
                        }
                        else if (c40Values[i] <= 26)
                        {
                            PushOutputC40TextWord(ref state, c40Values[i] + 69); /* ASCII 91 - 95 */
                        }
                        else if (c40Values[i] == 27)
                        {
                            PushOutputC40TextWord(ref state, 0x1d); /* FNC1 -- XXX depends on position? */
                        }
                        else if (c40Values[i] == 30)
                        {
                            state.UpperShift = true;
                            state.Shift = DmtxConstants.DmtxC40TextBasicSet;
                        }
                    }
                    else if (state.Shift == DmtxConstants.DmtxC40TextShift3)
                    { /* Shift 3 set */
                        if (encScheme == DmtxScheme.DmtxSchemeC40)
                        {
                            PushOutputC40TextWord(ref state, c40Values[i] + 96);
                        }
                        else if (encScheme == DmtxScheme.DmtxSchemeText)
                        {
                            if (c40Values[i] == 0)
                                PushOutputC40TextWord(ref state, c40Values[i] + 96);
                            else if (c40Values[i] <= 26)
                                PushOutputC40TextWord(ref state, c40Values[i] - 26 + 'Z'); /* A-Z */
                            else
                                PushOutputC40TextWord(ref state, c40Values[i] - 31 + 127); /* { | } ~ DEL */
                        }
                    }
                }

                /* Unlatch if codeword 254 follows 2 codewords in C40/Text encodation */
                if (this.Code[startIndex] == DmtxConstants.DmtxCharTripletUnlatch)
                    return startIndex + 1;

                /* Unlatch is implied if only one codeword remains */
                if (endIndex - startIndex == 1)
                    return startIndex;
            }
            return startIndex;
        }

        void PushOutputC40TextWord(ref C40TextState state, int value)
        {
            if (!(value >= 0 && value < 256))
            {
                throw new ArgumentException("Invalid value: Exceeds range for conversion to byte");
            }

            this.Output[this._outputIdx] = (byte)value;

            if (state.UpperShift)
            {
                if (!(value >= 0 && value < 256))
                {
                    throw new ArgumentException("Invalid value: Exceeds range for conversion to upper case character");
                }
                this.Output[this._outputIdx] += 128;
            }

            this._outputIdx++;

            state.Shift = DmtxConstants.DmtxC40TextBasicSet;
            state.UpperShift = false;
        }

        private int DecodeSchemeX12(int startIndex, int endIndex)
        {
            int[] x12Values = new int[3];

            while (startIndex < endIndex)
            {

                /* FIXME Also check that ptr+1 is safe to access */
                int packed = (this.Code[startIndex] << 8) | this.Code[startIndex + 1];
                x12Values[0] = ((packed - 1) / 1600);
                x12Values[1] = ((packed - 1) / 40) % 40;
                x12Values[2] = (packed - 1) % 40;
                startIndex += 2;

                for (int i = 0; i < 3; i++)
                {
                    if (x12Values[i] == 0)
                        PushOutputWord(13);
                    else if (x12Values[i] == 1)
                        PushOutputWord(42);
                    else if (x12Values[i] == 2)
                        PushOutputWord(62);
                    else if (x12Values[i] == 3)
                        PushOutputWord(32);
                    else if (x12Values[i] <= 13)
                        PushOutputWord((byte)(x12Values[i] + 44));
                    else if (x12Values[i] <= 90)
                        PushOutputWord((byte)(x12Values[i] + 51));
                }

                /* Unlatch if codeword 254 follows 2 codewords in C40/Text encodation */
                if (this.Code[startIndex] == DmtxConstants.DmtxCharTripletUnlatch)
                    return startIndex + 1;

                /* Unlatch is implied if only one codeword remains */
                if (endIndex - startIndex == 1)
                    return startIndex;
            }

            return startIndex;
        }

        int DecodeSchemeEdifact(int startIndex, int endIndex)
        {
            byte[] unpacked = new byte[4];

            while (startIndex < endIndex)
            {

                /* FIXME Also check that ptr+2 is safe to access -- shouldn't be a
                   problem because I'm guessing you can guarantee there will always
                   be at least 3 error codewords */
                unpacked[0] = (byte)((this.Code[startIndex] & 0xfc) >> 2);
                unpacked[1] = (byte)((this.Code[startIndex] & 0x03) << 4 | (this.Code[startIndex + 1] & 0xf0) >> 4);
                unpacked[2] = (byte)((this.Code[startIndex + 1] & 0x0f) << 2 | (this.Code[startIndex + 2] & 0xc0) >> 6);
                unpacked[3] = (byte)(this.Code[startIndex + 2] & 0x3f);

                for (int i = 0; i < 4; i++)
                {

                    /* Advance input ptr (4th value comes from already-read 3rd byte) */
                    if (i < 3)
                        startIndex++;

                    /* Test for unlatch condition */
                    if (unpacked[i] == DmtxConstants.DmtxCharEdifactUnlatch)
                    {
                        if (this.Output[_outputIdx] != 0)
                        {/* XXX dirty why? */
                            throw new Exception("Error decoding edifact scheme");
                        }
                        return startIndex;
                    }

                    PushOutputWord((byte)(unpacked[i] ^ (((unpacked[i] & 0x20) ^ 0x20) << 1)));
                }

                /* Unlatch is implied if fewer than 3 codewords remain */
                if (endIndex - startIndex < 3)
                {
                    return startIndex;
                }
            }
            return startIndex;
        }

        int DecodeSchemeBase256(int startIndex, int endIndex)
        {
            int tempEndIndex;

            /* Find positional index used for unrandomizing */
            int idx = startIndex + 1;

            int d0 = UnRandomize255State(this.Code[startIndex++], idx++);
            if (d0 == 0)
            {
                tempEndIndex = endIndex;
            }
            else if (d0 <= 249)
            {
                tempEndIndex = startIndex + d0;
            }
            else
            {
                int d1 = UnRandomize255State(this.Code[startIndex++], idx++);
                tempEndIndex = startIndex + (d0 - 249) * 250 + d1;
            }

            if (tempEndIndex > endIndex)
            {
                throw new Exception("Error decoding scheme base 256");
            }

            while (startIndex < tempEndIndex)
            {
                PushOutputWord(UnRandomize255State(this.Code[startIndex++], idx++));
            }

            return startIndex;
        }

        internal static byte UnRandomize255State(byte value, int idx)
        {
            int pseudoRandom = ((149 * idx) % 255) + 1;
            int tmp = value - pseudoRandom;
            if (tmp < 0)
                tmp += 256;

            if (tmp < 0 || tmp >= 256)
            {
                throw new Exception("Error unrandomizing 255 state");
            }

            return (byte)tmp;
        }

        internal int SymbolModuleStatus(DmtxSymbolSize sizeIdx, int symbolRow, int symbolCol)
        {
            int dataRegionRows = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribDataRegionRows, sizeIdx);
            int dataRegionCols = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribDataRegionCols, sizeIdx);
            int symbolRows = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolRows, sizeIdx);
            int mappingCols = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribMappingMatrixCols, sizeIdx);

            int symbolRowReverse = symbolRows - symbolRow - 1;
            int mappingRow = symbolRowReverse - 1 - 2 * (symbolRowReverse / (dataRegionRows + 2));
            int mappingCol = symbolCol - 1 - 2 * (symbolCol / (dataRegionCols + 2));

            /* Solid portion of alignment patterns */
            if (symbolRow % (dataRegionRows + 2) == 0 || symbolCol % (dataRegionCols + 2) == 0)
            {
                return (DmtxConstants.DmtxModuleOnRGB | (DmtxConstants.DmtxModuleData == 0 ? 1 : 0)); // check if unary not is correct
            }

            /* Horinzontal calibration bars */
            if ((symbolRow + 1) % (dataRegionRows + 2) == 0)
            {
                return (((symbolCol & 0x01) != 0) ? 0 : DmtxConstants.DmtxModuleOnRGB) | (DmtxConstants.DmtxModuleData == 0 ? 1 : 0);
            }

            /* Vertical calibration bars */
            if ((symbolCol + 1) % (dataRegionCols + 2) == 0)
            {
                return (((symbolRow & 0x01) != 0) ? 0 : DmtxConstants.DmtxModuleOnRGB) | (DmtxConstants.DmtxModuleData == 0 ? 1 : 0);
            }

            /* Data modules */
            return (this.Array[mappingRow * mappingCols + mappingCol] | DmtxConstants.DmtxModuleData);
        }

        #endregion

        #region Properties

        internal int PadCount { get; set; }

        internal byte[] Array { get; set; }

        internal byte[] Code { get; set; }

        internal byte[] Output { get; set; }

        internal int ArraySize
        {
            get
            {
                return this.Array.Length;
            }
        }

        internal int CodeSize
        {
            get
            {
                return this.Code.Length;
            }
        }

        internal int OutputSize
        {
            get
            {
                return this.Output.Length;
            }
        }
        #endregion
    }
}

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
    internal class DmtxImage
    {
        #region Fields

        int _rowPadBytes;

        #endregion

        #region Constructor
        internal DmtxImage(byte[] pxl, int width, int height, DmtxPackOrder pack)
        {
            this.BitsPerChannel = new int[4];
            this.ChannelStart = new int[4];
            if (pxl == null || width < 1 || height < 1)
            {
                throw new ArgumentException("Cannot create image of size null");
            }

            this.Pxl = pxl;
            this.Width = width;
            this.Height = height;
            this.PixelPacking = pack;
            this.BitsPerPixel = DmtxCommon.GetBitsPerPixel(pack);
            this.BytesPerPixel = this.BitsPerPixel / 8;
            this._rowPadBytes = 0;
            this.RowSizeBytes = this.Width * this.BytesPerPixel + this._rowPadBytes;
            this.ImageFlip = DmtxFlip.DmtxFlipNone;

            /* Leave channelStart[] and bitsPerChannel[] with zeros from calloc */
            this.ChannelCount = 0;

            switch (pack)
            {
                case DmtxPackOrder.DmtxPackCustom:
                    break;
                case DmtxPackOrder.DmtxPack1bppK:
                    throw new ArgumentException("Cannot create image: not supported pack order!");
                case DmtxPackOrder.DmtxPack8bppK:
                    SetChannel(0, 8);
                    break;
                case DmtxPackOrder.DmtxPack16bppRGB:
                case DmtxPackOrder.DmtxPack16bppBGR:
                case DmtxPackOrder.DmtxPack16bppYCbCr:
                    SetChannel(0, 5);
                    SetChannel(5, 5);
                    SetChannel(10, 5);
                    break;
                case DmtxPackOrder.DmtxPack24bppRGB:
                case DmtxPackOrder.DmtxPack24bppBGR:
                case DmtxPackOrder.DmtxPack24bppYCbCr:
                case DmtxPackOrder.DmtxPack32bppRGBX:
                case DmtxPackOrder.DmtxPack32bppBGRX:
                    SetChannel(0, 8);
                    SetChannel(8, 8);
                    SetChannel(16, 8);
                    break;
                case DmtxPackOrder.DmtxPack16bppRGBX:
                case DmtxPackOrder.DmtxPack16bppBGRX:
                    SetChannel(0, 5);
                    SetChannel(5, 5);
                    SetChannel(10, 5);
                    break;
                case DmtxPackOrder.DmtxPack16bppXRGB:
                case DmtxPackOrder.DmtxPack16bppXBGR:
                    SetChannel(1, 5);
                    SetChannel(6, 5);
                    SetChannel(11, 5);
                    break;
                case DmtxPackOrder.DmtxPack32bppXRGB:
                case DmtxPackOrder.DmtxPack32bppXBGR:
                    SetChannel(8, 8);
                    SetChannel(16, 8);
                    SetChannel(24, 8);
                    break;
                case DmtxPackOrder.DmtxPack32bppCMYK:
                    SetChannel(0, 8);
                    SetChannel(8, 8);
                    SetChannel(16, 8);
                    SetChannel(24, 8);
                    break;
                default:
                    throw new ArgumentException("Cannot create image: Invalid Pack Order");
            }
        }
        #endregion

        #region Methods
        internal bool SetChannel(int channelStart, int bitsPerChannel)
        {
            if (this.ChannelCount >= 4) /* IMAGE_MAX_CHANNEL */
                return false;

            /* New channel extends beyond pixel data */

            this.BitsPerChannel[this.ChannelCount] = bitsPerChannel;
            this.ChannelStart[this.ChannelCount] = channelStart;
            (this.ChannelCount)++;

            return true;
        }

        internal int GetByteOffset(int x, int y)
        {
            if (this.ImageFlip == DmtxFlip.DmtxFlipX)
            {
                throw new ArgumentException("DmtxFlipX is not an option!");
            }

            if (!ContainsInt(0, x, y))
                return DmtxConstants.DmtxUndefined;

            if (this.ImageFlip == DmtxFlip.DmtxFlipY)
                return (y * this.RowSizeBytes + x * this.BytesPerPixel);

            return ((this.Height - y - 1) * this.RowSizeBytes + x * this.BytesPerPixel);
        }

        internal bool GetPixelValue(int x, int y, int channel, ref int value)
        {
            if (channel >= this.ChannelCount)
            {
                throw new ArgumentException("Channel greater than channel count!");
            }

            int offset = GetByteOffset(x, y);
            if (offset == DmtxConstants.DmtxUndefined)
            {
                return false;
            }

            switch (this.BitsPerChannel[channel])
            {
                case 1:
                    break;
                case 5:
                    break;
                case 8:
                    if (this.ChannelStart[channel] % 8 != 0 || this.BitsPerPixel % 8 != 0)
                    {
                        throw new Exception("Error getting pixel value");
                    }
                    value = this.Pxl[offset + channel];
                    break;
            }

            return true;
        }

        internal bool SetPixelValue(int x, int y, int channel, byte value)
        {
            if (channel >= this.ChannelCount)
            {
                throw new ArgumentException("Channel greater than channel count!");
            }

            int offset = GetByteOffset(x, y);
            if (offset == DmtxConstants.DmtxUndefined)
            {
                return false;
            }

            switch (this.BitsPerChannel[channel])
            {
                case 1:
                    break;
                case 5:
                    break;
                case 8:
                    if (this.ChannelStart[channel] % 8 != 0 || this.BitsPerPixel % 8 != 0)
                    {
                        throw new Exception("Error getting pixel value");
                    }
                    this.Pxl[offset + channel] = value;
                    break;
            }

            return true;
        }

        internal bool ContainsInt(int margin, int x, int y)
        {
            if (x - margin >= 0 && x + margin < this.Width &&
                  y - margin >= 0 && y + margin < this.Height)
                return true;

            return false;
        }

        internal bool ContainsFloat(double x, double y)
        {
            if (x >= 0.0 && x < this.Width && y >= 0.0 && y < this.Height)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Properties

        internal int Width { get; set; }

        internal int Height { get; set; }

        internal DmtxPackOrder PixelPacking { get; set; }

        internal int BitsPerPixel { get; set; }

        internal int BytesPerPixel { get; set; }

        internal int RowPadBytes
        {
            get { return _rowPadBytes; }
            set
            {
                _rowPadBytes = value;
                this.RowSizeBytes = this.Width * (this.BitsPerPixel / 8) + this._rowPadBytes;
            }
        }

        internal int RowSizeBytes { get; set; }

        internal DmtxFlip ImageFlip { get; set; }

        internal int ChannelCount { get; set; }

        internal int[] ChannelStart { get; set; }

        internal int[] BitsPerChannel { get; set; }

        internal byte[] Pxl { get; set; }

        #endregion
    }
}

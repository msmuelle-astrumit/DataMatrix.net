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
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace DataMatrix.net
{
    public class DmtxImageDecoder
    {
        /// <summary>
        /// returns a list of all decoded DataMatrix codes in the image provided
        /// </summary>
        public List<string> DecodeImage(Bitmap image)
        {
            return DecodeImage(image, int.MaxValue, TimeSpan.MaxValue);
        }

        /// <summary>
        /// returns a list of all decoded DataMatrix codes in the image provided
        /// that can be found in the given time span
        /// </summary>
        public List<string> DecodeImage(Bitmap image, TimeSpan timeSpan)
        {
            return DecodeImage(image, int.MaxValue, timeSpan);
        }

        /// <summary>
        /// returns a list of all decoded DataMatrix codes in the image provided
        /// </summary>
        public List<string> DecodeImageMosaic(Bitmap image)
        {
            return DecodeImageMosaic(image, int.MaxValue, TimeSpan.MaxValue);
        }

        /// <summary>
        /// returns a list of DataMatrix codes in the image provided that can be
        /// found in the given time span, but no more than maxResultCount codes
        /// (useful, if you e.g. expect only one code to be in the image)
        /// </summary>
        public List<string> DecodeImageMosaic(Bitmap image, int maxResultCount, TimeSpan timeOut)
        {
            return DecodeImage(image, maxResultCount, timeOut, true);
        }

        /// <summary>
        /// returns a list of all decoded DataMatrix codes in the image provided
        /// that can be found in the given time span
        /// </summary>
        public List<string> DecodeImageMosaic(Bitmap image, TimeSpan timeSpan)
        {
            return DecodeImage(image, int.MaxValue, timeSpan);
        }

        /// <summary>
        /// returns a list of DataMatrix codes in the image provided that can be
        /// found in the given time span, but no more than maxResultCount codes
        /// (useful, if you e.g. expect only one code to be in the image)
        /// </summary>
        public List<string> DecodeImage(Bitmap image, int maxResultCount, TimeSpan timeOut)
        {
            return DecodeImage(image, maxResultCount, timeOut, false);
        }

        private List<string> DecodeImage(Bitmap image, int maxResultCount, TimeSpan timeOut, bool isMosaic)
        {
            List<string> result = new List<string>();
            int stride;
            byte[] rawImg = ImageToByteArray(image, out stride);
            DmtxImage dmtxImg = new DmtxImage(rawImg, image.Width, image.Height, DmtxPackOrder.DmtxPack24bppRGB);
            dmtxImg.RowPadBytes = stride % 3;
            DmtxDecode decode = new DmtxDecode(dmtxImg, 1);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while (true)
            {
                if (stopWatch.Elapsed > timeOut)
                {
                    break;
                }
                DmtxRegion region = decode.RegionFindNext(timeOut);
                if (region != null)
                {
                    DmtxMessage msg = isMosaic ? decode.MosaicRegion(region, -1) : decode.MatrixRegion(region, -1);
                    string message = Encoding.ASCII.GetString(msg.Output, 0, msg.Output.Length);
                    message = message.Substring(0, message.IndexOf('\0'));
                    if (!result.Contains(message))
                    {
                        result.Add(message);
                        if (result.Count >= maxResultCount)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            return result;
        }


        private byte[] ImageToByteArray(Bitmap b, out int stride)
        {
            Rectangle rect = new Rectangle(0, 0, b.Width, b.Height);
            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                byte[] pxl = new byte[bd.Stride * b.Height];
                Marshal.Copy(bd.Scan0, pxl, 0, bd.Stride * b.Height);
                stride = bd.Stride;
                return pxl;
            }
            finally
            {
                b.UnlockBits(bd);
            }
        }
    }
}

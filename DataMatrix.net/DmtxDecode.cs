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

namespace DataMatrix.net
{
    internal class DmtxDecode
    {
        #region Fields
        int _edgeMin;
        int _edgeMax;
        int _scanGap;
        double _squareDevn;
        DmtxSymbolSize _sizeIdxExpected;
        int _edgeThresh;

        /* Image modifiers */
        int _xMin;
        int _yMin;
        int _xMax;
        int _yMax;
        int _scale;
        /* Internals */
        byte[] _cache;
        DmtxImage _image;
        DmtxScanGrid _grid;
        #endregion

        #region Constructors
        internal DmtxDecode(DmtxImage img, int scale)
        {

            int width = img.Width / scale;
            int height = img.Height / scale;

            this._edgeMin = DmtxConstants.DmtxUndefined;
            this._edgeMax = DmtxConstants.DmtxUndefined;
            this._scanGap = 1;
            this._squareDevn = Math.Cos(50.0 * (Math.PI / 180.0));
            this._sizeIdxExpected = DmtxSymbolSize.DmtxSymbolShapeAuto;
            this._edgeThresh = 10;

            this._xMin = 0;
            this._xMax = width - 1;
            this._yMin = 0;
            this._yMax = height - 1;
            this._scale = scale;

            this._cache = new byte[width * height];

            this._image = img;
            ValidateSettingsAndInitScanGrid();
        }
        #endregion

        #region Methods
        private void ValidateSettingsAndInitScanGrid()
        {
            if (this._squareDevn <= 0.0 || this._squareDevn >= 1.0)
            {
                throw new ArgumentException("Invalid decode settings!");
            }

            if (this._scanGap < 1)
            {
                throw new ArgumentException("Invalid decode settings!");
            }

            if (this._edgeThresh < 1 || this._edgeThresh > 100)
            {
                throw new ArgumentException("Invalid decode settings!");
            }

            /* Reinitialize scangrid in case any inputs changed */
            this._grid = new DmtxScanGrid(this);
        }

        internal int GetCacheIndex(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                throw new ArgumentException("Error: x and/or y outside cache size");
            }

            return y * Width + x;
        }

        bool GetPixelValue(int x, int y, int channel, ref int value)
        {
            int xUnscaled = x * this._scale;
            int yUnscaled = y * this._scale;

            return this._image.GetPixelValue(xUnscaled, yUnscaled, channel, ref value);

        }

        internal void CacheFillQuad(DmtxPixelLoc p0, DmtxPixelLoc p1, DmtxPixelLoc p2, DmtxPixelLoc p3)
        {
            DmtxBresLine[] lines = new DmtxBresLine[4];
            DmtxPixelLoc pEmpty = new DmtxPixelLoc { X = 0, Y = 0 };
            int posY;
            int i, idx;

            lines[0] = new DmtxBresLine(p0, p1, pEmpty);
            lines[1] = new DmtxBresLine(p1, p2, pEmpty);
            lines[2] = new DmtxBresLine(p2, p3, pEmpty);
            lines[3] = new DmtxBresLine(p3, p0, pEmpty);

            int minY = this._yMax;
            int maxY = 0;

            minY = DmtxCommon.Min(minY, p0.Y);
            maxY = DmtxCommon.Max(maxY, p0.Y);
            minY = DmtxCommon.Min(minY, p1.Y);
            maxY = DmtxCommon.Max(maxY, p1.Y);
            minY = DmtxCommon.Min(minY, p2.Y);
            maxY = DmtxCommon.Max(maxY, p2.Y);
            minY = DmtxCommon.Min(minY, p3.Y);
            maxY = DmtxCommon.Max(maxY, p3.Y);

            int sizeY = maxY - minY + 1;

            int[] scanlineMin = new int[sizeY];
            int[] scanlineMax = new int[sizeY];

            for (i = 0; i < sizeY; i++)
            {
                scanlineMin[i] = this._xMax;
            }

            for (i = 0; i < 4; i++)
            {
                while (lines[i].Loc.X != lines[i].Loc1.X || lines[i].Loc.Y != lines[i].Loc1.Y)
                {
                    idx = lines[i].Loc.Y - minY;
                    scanlineMin[idx] = DmtxCommon.Min(scanlineMin[idx], lines[i].Loc.X);
                    scanlineMax[idx] = DmtxCommon.Max(scanlineMax[idx], lines[i].Loc.X);
                    lines[i].Step(1, 0);
                }
            }

            for (posY = minY; posY < maxY && posY < this._yMax; posY++)
            {
                idx = posY - minY;
                int posX;
                for (posX = scanlineMin[idx]; posX < scanlineMax[idx] && posX < this._xMax; posX++)
                {
                    if (posX < 0 || posY < 0)
                    {
                        continue;
                    }
                    try
                    {
                        int cacheIndex = GetCacheIndex(posX, posY);
                        this._cache[cacheIndex] |= 0x80;
                    }
                    catch
                    {
                        // FIXXXME: log here as soon as there is a logger
                    }
                }
            }
        }

        internal DmtxMessage MosaicRegion(DmtxRegion reg, int fix)
        {
            int colorPlane = reg.FlowBegin.Plane;

            reg.FlowBegin.Plane = 0; /* kind of a hack */
            DmtxMessage rMsg = MatrixRegion(reg, fix);

            reg.FlowBegin.Plane = 1; /* kind of a hack */
            DmtxMessage gMsg = MatrixRegion(reg, fix);

            reg.FlowBegin.Plane = 2; /* kind of a hack */
            DmtxMessage bMsg = MatrixRegion(reg, fix);

            reg.FlowBegin.Plane = colorPlane;

            DmtxMessage oMsg = new DmtxMessage(reg.SizeIdx, DmtxFormat.Mosaic);

            List<byte> totalMessage = new List<byte>();
            for (int i = 0; i < bMsg.OutputSize; i++)
            {
                if (bMsg.Output[i] == 0)
                {
                    break;
                }
                totalMessage.Add(bMsg.Output[i]);
            }
            for (int i = 0; i < gMsg.OutputSize; i++)
            {
                if (gMsg.Output[i] == 0)
                {
                    break;
                }
                totalMessage.Add(gMsg.Output[i]);
            }
            for (int i = 0; i < rMsg.OutputSize; i++)
            {
                if (rMsg.Output[i] == 0)
                {
                    break;
                }
                totalMessage.Add(rMsg.Output[i]);
            }
            totalMessage.Add(0);
            oMsg.Output = totalMessage.ToArray();
            return oMsg;
        }

        internal DmtxMessage MatrixRegion(DmtxRegion reg, int fix)
        {
            DmtxMessage result = new DmtxMessage(reg.SizeIdx, DmtxFormat.Matrix);
            DmtxVector2 topLeft = new DmtxVector2();
            DmtxVector2 topRight = new DmtxVector2();
            DmtxVector2 bottomLeft = new DmtxVector2();
            DmtxVector2 bottomRight = new DmtxVector2();
            DmtxPixelLoc pxTopLeft = new DmtxPixelLoc();
            DmtxPixelLoc pxTopRight = new DmtxPixelLoc();
            DmtxPixelLoc pxBottomLeft = new DmtxPixelLoc();
            DmtxPixelLoc pxBottomRight = new DmtxPixelLoc();

            if (!PopulateArrayFromMatrix(reg, result))
            {
                throw new Exception("Populating Array from matrix failed!");
            }

            /* maybe place remaining logic into new dmtxDecodePopulatedArray()
               function so other people can pass in their own arrays */

            ModulePlacementEcc200(result.Array, result.Code,
                  reg.SizeIdx, DmtxConstants.DmtxModuleOnRed | DmtxConstants.DmtxModuleOnGreen | DmtxConstants.DmtxModuleOnBlue);

            if (DmtxCommon.DecodeCheckErrors(result.Code, 0, reg.SizeIdx, fix) != true)
            {
                return null;
            }

            topLeft.X = bottomLeft.X = topLeft.Y = topRight.Y = -0.1;
            topRight.X = bottomRight.X = bottomLeft.Y = bottomRight.Y = 1.1;

            topLeft *= reg.Fit2Raw;
            topRight *= reg.Fit2Raw;
            bottomLeft *= reg.Fit2Raw;
            bottomLeft *= reg.Fit2Raw;

            pxTopLeft.X = (int)(0.5 + topLeft.X);
            pxTopLeft.Y = (int)(0.5 + topLeft.Y);
            pxBottomLeft.X = (int)(0.5 + bottomLeft.X);
            pxBottomLeft.Y = (int)(0.5 + bottomLeft.Y);
            pxTopRight.X = (int)(0.5 + topRight.X);
            pxTopRight.Y = (int)(0.5 + topRight.Y);
            pxBottomRight.X = (int)(0.5 + bottomRight.X);
            pxBottomRight.Y = (int)(0.5 + bottomRight.Y);

            CacheFillQuad(pxTopLeft, pxTopRight, pxBottomRight, pxBottomLeft);

            result.DecodeDataStream(reg.SizeIdx, null);

            return result;
        }

        private bool PopulateArrayFromMatrix(DmtxRegion reg, DmtxMessage msg)
        {
            /* Capture number of regions present in barcode */
            int xRegionTotal = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribHorizDataRegions, reg.SizeIdx);
            int yRegionTotal = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribVertDataRegions, reg.SizeIdx);

            /* Capture region dimensions (not including border modules) */
            int mapWidth = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribDataRegionCols, reg.SizeIdx);
            int mapHeight = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribDataRegionRows, reg.SizeIdx);

            int weightFactor = 2 * (mapHeight + mapWidth + 2);
            if (weightFactor <= 0)
            {
                throw new ArgumentException("PopulateArrayFromMatrix error: Weight Factor must be greater 0");
            }

            /* Tally module changes for each region in each direction */
            for (int yRegionCount = 0; yRegionCount < yRegionTotal; yRegionCount++)
            {

                /* Y location of mapping region origin in symbol coordinates */
                int yOrigin = yRegionCount * (mapHeight + 2) + 1;

                for (int xRegionCount = 0; xRegionCount < xRegionTotal; xRegionCount++)
                {
                    int[,] tally = new int[24, 24]; /* Large enough to map largest single region */
                    /* X location of mapping region origin in symbol coordinates */
                    int xOrigin = xRegionCount * (mapWidth + 2) + 1;

                    for (int i = 0; i < 24; i++)
                    {
                        for (int j = 0; j < 24; j++)
                        {
                            tally[i, j] = 0;
                        }
                    }
                    TallyModuleJumps(reg, tally, xOrigin, yOrigin, mapWidth, mapHeight, DmtxDirection.DmtxDirUp);
                    TallyModuleJumps(reg, tally, xOrigin, yOrigin, mapWidth, mapHeight, DmtxDirection.DmtxDirLeft);
                    TallyModuleJumps(reg, tally, xOrigin, yOrigin, mapWidth, mapHeight, DmtxDirection.DmtxDirDown);
                    TallyModuleJumps(reg, tally, xOrigin, yOrigin, mapWidth, mapHeight, DmtxDirection.DmtxDirRight);

                    /* Decide module status based on final tallies */
                    for (int mapRow = 0; mapRow < mapHeight; mapRow++)
                    {
                        for (int mapCol = 0; mapCol < mapWidth; mapCol++)
                        {

                            int rowTmp = (yRegionCount * mapHeight) + mapRow;
                            rowTmp = yRegionTotal * mapHeight - rowTmp - 1;
                            int colTmp = (xRegionCount * mapWidth) + mapCol;
                            int idx = (rowTmp * xRegionTotal * mapWidth) + colTmp;

                            if (tally[mapRow, mapCol] / (double)weightFactor >= 0.5)
                                msg.Array[idx] = (byte)DmtxConstants.DmtxModuleOnRGB;
                            else
                                msg.Array[idx] = (byte)DmtxConstants.DmtxModuleOff;

                            msg.Array[idx] |= (byte)DmtxConstants.DmtxModuleAssigned;
                        }
                    }
                }
            }

            return true;
        }

        private void TallyModuleJumps(DmtxRegion reg, int[,] tally, int xOrigin, int yOrigin, int mapWidth, int mapHeight, DmtxDirection dir)
        {
            int extent;
            int lineStart, lineStop;
            int travelStart, travelStop;
            int line;

            if (!(dir == DmtxDirection.DmtxDirUp || dir == DmtxDirection.DmtxDirLeft || dir == DmtxDirection.DmtxDirDown || dir == DmtxDirection.DmtxDirRight))
            {
                throw new ArgumentException("Only orthogonal directions are allowed in tally module jumps!");
            }

            int travelStep = (dir == DmtxDirection.DmtxDirUp || dir == DmtxDirection.DmtxDirRight) ? 1 : -1;

            /* Abstract row and column progress using pointers to allow grid
               traversal in all 4 directions using same logic */
            bool horizontal = false;
            if ((dir & DmtxDirection.DmtxDirHorizontal) != 0x00)
            {
                horizontal = true;
                extent = mapWidth;
                lineStart = yOrigin;
                lineStop = yOrigin + mapHeight;
                travelStart = (travelStep == 1) ? xOrigin - 1 : xOrigin + mapWidth;
                travelStop = (travelStep == 1) ? xOrigin + mapWidth : xOrigin - 1;
            }
            else
            {
                extent = mapHeight;
                lineStart = xOrigin;
                lineStop = xOrigin + mapWidth;
                travelStart = (travelStep == 1) ? yOrigin - 1 : yOrigin + mapHeight;
                travelStop = (travelStep == 1) ? yOrigin + mapHeight : yOrigin - 1;
            }

            bool darkOnLight = (reg.OffColor > reg.OnColor);
            int jumpThreshold = Math.Abs((int)(0.4 * (reg.OffColor - reg.OnColor) + 0.5));

            if (jumpThreshold < 0)
            {
                throw new Exception("Negative jump threshold is not allowed in tally module jumps");
            }


            for (line = lineStart; line < lineStop; line++)
            {

                /* Capture tModule for each leading border module as normal but
                   decide status based on predictable barcode border pattern */

                int travel = travelStart;
                int color = horizontal ? this.ReadModuleColor(reg, line, travel, reg.SizeIdx, reg.FlowBegin.Plane) 
                                : this.ReadModuleColor(reg, travel, line, reg.SizeIdx, reg.FlowBegin.Plane);
                int tModule = (darkOnLight) ? reg.OffColor - color : color - reg.OffColor;

                int statusModule = (travelStep == 1 || (line & 0x01) == 0) ? DmtxConstants.DmtxModuleOnRGB : DmtxConstants.DmtxModuleOff;

                int weight = extent;

                while ((travel += travelStep) != travelStop)
                {

                    int tPrev = tModule;
                    int statusPrev = statusModule;

                    /* For normal data-bearing modules capture color and decide
                       module status based on comparison to previous "known" module */

                    color = horizontal ? this.ReadModuleColor(reg, line, travel, reg.SizeIdx, reg.FlowBegin.Plane) 
                        : this.ReadModuleColor(reg, travel, line, reg.SizeIdx, reg.FlowBegin.Plane);
                    tModule = (darkOnLight) ? reg.OffColor - color : color - reg.OffColor;

                    if (statusPrev == DmtxConstants.DmtxModuleOnRGB)
                    {
                        statusModule = tModule < tPrev - jumpThreshold ? DmtxConstants.DmtxModuleOff : DmtxConstants.DmtxModuleOnRGB;
                    }
                    else if (statusPrev == DmtxConstants.DmtxModuleOff)
                    {
                        statusModule = tModule > tPrev + jumpThreshold ? DmtxConstants.DmtxModuleOnRGB : DmtxConstants.DmtxModuleOff;
                    }
                    int mapCol;
                    int mapRow;
                    if (horizontal)
                    {
                        mapRow = line - yOrigin;
                        mapCol = travel - xOrigin;
                    }
                    else
                    {
                        mapRow = travel - yOrigin;
                        mapCol = line - xOrigin;
                    }
                    if (!(mapRow < 24 && mapCol < 24))
                    {
                        throw new Exception("Tally module mump failed, index out of range!");
                    }

                    if (statusModule == DmtxConstants.DmtxModuleOnRGB)
                    {
                        tally[mapRow, mapCol] += (2 * weight);
                    }

                    weight--;
                }

                if (weight != 0)
                {
                    throw new Exception("Tally module jump failed, weight <> 0!");
                }
            }
        }

        private int ReadModuleColor(DmtxRegion reg, int symbolRow, int symbolCol, DmtxSymbolSize sizeIdx, int colorPlane)
        {
            int i;
            int color;
            double[] sampleX = { 0.5, 0.4, 0.5, 0.6, 0.5 };
            double[] sampleY = { 0.5, 0.5, 0.4, 0.5, 0.6 };
            DmtxVector2 p = new DmtxVector2();

            int symbolRows = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolRows, sizeIdx);
            int symbolCols = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolCols, sizeIdx);

            int colorTmp = color = 0;
            for (i = 0; i < 5; i++)
            {

                p.X = (1.0 / symbolCols) * (symbolCol + sampleX[i]);
                p.Y = (1.0 / symbolRows) * (symbolRow + sampleY[i]);

                p *= reg.Fit2Raw;

                GetPixelValue((int)(p.X + 0.5), (int)(p.Y + 0.5), colorPlane, ref colorTmp);
                color += colorTmp;
            }

            return color / 5;
        }

        internal static int ModulePlacementEcc200(byte[] modules, byte[] codewords, DmtxSymbolSize sizeIdx, int moduleOnColor)
        {
            if ((moduleOnColor & (DmtxConstants.DmtxModuleOnRed | DmtxConstants.DmtxModuleOnGreen | DmtxConstants.DmtxModuleOnBlue)) == 0)
            {
                throw new Exception("Error with module placement ECC 200");
            }

            int mappingRows = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribMappingMatrixRows, sizeIdx);
            int mappingCols = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribMappingMatrixCols, sizeIdx);

            /* Start in the nominal location for the 8th bit of the first character */
            int chr = 0;
            int row = 4;
            int col = 0;

            do
            {
                /* Repeatedly first check for one of the special corner cases */
                if ((row == mappingRows) && (col == 0))
                    PatternShapeSpecial1(modules, mappingRows, mappingCols, codewords, chr++, moduleOnColor);
                else if ((row == mappingRows - 2) && (col == 0) && (mappingCols % 4 != 0))
                    PatternShapeSpecial2(modules, mappingRows, mappingCols, codewords, chr++, moduleOnColor);
                else if ((row == mappingRows - 2) && (col == 0) && (mappingCols % 8 == 4))
                    PatternShapeSpecial3(modules, mappingRows, mappingCols, codewords, chr++, moduleOnColor);
                else if ((row == mappingRows + 4) && (col == 2) && (mappingCols % 8 == 0))
                    PatternShapeSpecial4(modules, mappingRows, mappingCols, codewords, chr++, moduleOnColor);

                /* Sweep upward diagonally, inserting successive characters */
                do
                {
                    if ((row < mappingRows) && (col >= 0) && (modules[row * mappingCols + col] & DmtxConstants.DmtxModuleVisited) == 0)
                        PatternShapeStandard(modules, mappingRows, mappingCols, row, col, codewords, chr++, moduleOnColor);
                    row -= 2;
                    col += 2;
                } while ((row >= 0) && (col < mappingCols));
                row += 1;
                col += 3;

                /* Sweep downward diagonally, inserting successive characters */
                do
                {
                    if ((row >= 0) && (col < mappingCols) && (modules[row * mappingCols + col] & DmtxConstants.DmtxModuleVisited) == 0)
                        PatternShapeStandard(modules, mappingRows, mappingCols, row, col, codewords, chr++, moduleOnColor);
                    row += 2;
                    col -= 2;
                } while ((row < mappingRows) && (col >= 0));
                row += 3;
                col += 1;
                /* ... until the entire modules array is scanned */
            } while ((row < mappingRows) || (col < mappingCols));

            /* If lower righthand corner is untouched then fill in the fixed pattern */
            if ((modules[mappingRows * mappingCols - 1] &
                  DmtxConstants.DmtxModuleVisited) == 0)
            {

                modules[mappingRows * mappingCols - 1] |= (byte)moduleOnColor;
                modules[(mappingRows * mappingCols) - mappingCols - 2] |= (byte)moduleOnColor;
            } /* XXX should this fixed pattern also be used in reading somehow? */

            /* XXX compare that chr == region->dataSize here */
            return chr; /* XXX number of codewords read off */
        }

        internal static void PatternShapeStandard(byte[] modules, int mappingRows, int mappingCols, int row, int col, byte[] codeword, int codeWordIndex, int moduleOnColor)
        {
            PlaceModule(modules, mappingRows, mappingCols, row - 2, col - 2, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit1, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, row - 2, col - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit2, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, row - 1, col - 2, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit3, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, row - 1, col - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit4, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, row - 1, col, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit5, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, row, col - 2, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit6, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, row, col - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit7, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, row, col, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit8, moduleOnColor);
        }

        internal static void PatternShapeSpecial1(byte[] modules, int mappingRows, int mappingCols, byte[] codeword, int codeWordIndex, int moduleOnColor)
        {
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 1, 0, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit1, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 1, 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit2, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 1, 2, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit3, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 2, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit4, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit5, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 1, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit6, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 2, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit7, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 3, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit8, moduleOnColor);
        }

        internal static void PatternShapeSpecial2(byte[] modules, int mappingRows, int mappingCols, byte[] codeword, int codeWordIndex, int moduleOnColor)
        {
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 3, 0, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit1, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 2, 0, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit2, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 1, 0, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit3, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 4, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit4, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 3, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit5, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 2, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit6, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit7, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 1, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit8, moduleOnColor);
        }

        internal static void PatternShapeSpecial3(byte[] modules, int mappingRows, int mappingCols, byte[] codeword, int codeWordIndex, int moduleOnColor)
        {
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 3, 0, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit1, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 2, 0, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit2, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 1, 0, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit3, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 2, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit4, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit5, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 1, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit6, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 2, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit7, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 3, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit8, moduleOnColor);
        }

        internal static void PatternShapeSpecial4(byte[] modules, int mappingRows, int mappingCols, byte[] codeword, int codeWordIndex, int moduleOnColor)
        {
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 1, 0, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit1, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, mappingRows - 1, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit2, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 3, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit3, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 2, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit4, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 0, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit5, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 1, mappingCols - 3, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit6, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 1, mappingCols - 2, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit7, moduleOnColor);
            PlaceModule(modules, mappingRows, mappingCols, 1, mappingCols - 1, codeword, codeWordIndex, DmtxMaskBit.DmtxMaskBit8, moduleOnColor);
        }

        internal static void PlaceModule(byte[] modules, int mappingRows, int mappingCols, int row, int col, byte[] codeword, int codeWordIndex, DmtxMaskBit mask, int moduleOnColor)
        {
            if (row < 0)
            {
                row += mappingRows;
                col += 4 - ((mappingRows + 4) % 8);
            }
            if (col < 0)
            {
                col += mappingCols;
                row += 4 - ((mappingCols + 4) % 8);
            }

            /* If module has already been assigned then we are decoding the pattern into codewords */
            if ((modules[row * mappingCols + col] & DmtxConstants.DmtxModuleAssigned) != 0)
            {
                if ((modules[row * mappingCols + col] & moduleOnColor) != 0)
                    codeword[codeWordIndex] |= (byte)mask;
                else
                    codeword[codeWordIndex] &= (byte)(0xff ^ (int)mask);
            }
            /* Otherwise we are encoding the codewords into a pattern */
            else
            {
                if ((codeword[codeWordIndex] & (byte)mask) != 0x00)
                    modules[row * mappingCols + col] |= (byte)moduleOnColor;

                modules[row * mappingCols + col] |= (byte)DmtxConstants.DmtxModuleAssigned;
            }

            modules[row * mappingCols + col] |= (byte)DmtxConstants.DmtxModuleVisited;
        }

        internal DmtxRegion RegionFindNext(TimeSpan timeout)
        {
            DmtxPixelLoc loc = new DmtxPixelLoc();
            DateTime startTime = DateTime.Now;
            /* Continue until we find a region or run out of chances */
            for (; ; )
            {
                DmtxRange locStatus = this._grid.PopGridLocation(ref loc);
                if (locStatus == DmtxRange.DmtxRangeEnd)
                    break;

                /* Scan location for presence of valid barcode region */
                DmtxRegion reg = RegionScanPixel(loc.X, loc.Y);
                if (reg != null)
                    return reg;

                /* Ran out of time? */
                if (DateTime.Now - startTime > timeout)
                {
                    break;
                }
            }

            return null;
        }

        DmtxRegion RegionScanPixel(int x, int y)
        {
            DmtxRegion reg = new DmtxRegion();
            DmtxPixelLoc loc = new DmtxPixelLoc {X = x, Y = y};


            int cacheIndex = this.DecodeGetCache(loc.X, loc.Y);
            if (cacheIndex == -1)
                return null;

            if (this._cache[cacheIndex] != 0x00)
                return null;

            /* Test for presence of any reasonable edge at this location */
            DmtxPointFlow flowBegin = this.MatrixRegionSeekEdge(loc);
            if (flowBegin.Mag < (int)(this._edgeThresh * 7.65 + 0.5))
                return null;

            /* Determine barcode orientation */
            if (MatrixRegionOrientation(reg, flowBegin) == false)
                return null;
            if (RegionUpdateXfrms(reg) == false)
                return null;

            /* Define top edge */
            if (MatrixRegionAlignCalibEdge(reg, DmtxEdge.DmtxEdgeTop) == false)
                return null;
            if (RegionUpdateXfrms(reg) == false)
                return null;

            /* Define right edge */
            if (MatrixRegionAlignCalibEdge(reg, DmtxEdge.DmtxEdgeRight) == false)
                return null;
            if (RegionUpdateXfrms(reg) == false)
                return null;

            //CALLBACK_MATRIX(&reg);

            /* Calculate the best fitting symbol size */
            if (MatrixRegionFindSize(reg) == false)
                return null;

            /* Found a valid matrix region */
            return new DmtxRegion(reg);
        }

        int DecodeGetCache(int x, int y)
        {
            int width = this.Width;
            int height = this.Height;

            if (x < 0 || x >= width || y < 0 || y >= height)
                return DmtxConstants.DmtxUndefined;
            return y * width + x;
        }

        DmtxPointFlow MatrixRegionSeekEdge(DmtxPixelLoc loc)
        {
            DmtxPointFlow[] flowPlane = new DmtxPointFlow[3];

            int channelCount = _image.ChannelCount;

            /* Find whether red, green, or blue shows the strongest edge */
            int strongIdx = 0;
            for (int i = 0; i < channelCount; i++)
            {
                flowPlane[i] = GetPointFlow(i, loc, DmtxConstants.DmtxNeighborNone);
                if (i > 0 && flowPlane[i].Mag > flowPlane[strongIdx].Mag)
                    strongIdx = i;
            }

            if (flowPlane[strongIdx].Mag < 10)
                return DmtxConstants.DmtxBlankEdge;

            DmtxPointFlow flow = flowPlane[strongIdx];

            DmtxPointFlow flowPos = this.FindStrongestNeighbor(flow, +1);
            DmtxPointFlow flowNeg = this.FindStrongestNeighbor(flow, -1);
            if (flowPos.Mag != 0 && flowNeg.Mag != 0)
            {
                DmtxPointFlow flowPosBack = FindStrongestNeighbor(flowPos, -1);
                DmtxPointFlow flowNegBack = FindStrongestNeighbor(flowNeg, +1);
                if (flowPos.Arrive == (flowPosBack.Arrive + 4) % 8 &&
                      flowNeg.Arrive == (flowNegBack.Arrive + 4) % 8)
                {
                    flow.Arrive = DmtxConstants.DmtxNeighborNone;
                    //CALLBACK_POINT_PLOT(flow.Loc, 1, 1, 1);
                    return flow;
                }
            }

            return DmtxConstants.DmtxBlankEdge;
        }

        DmtxPointFlow FindStrongestNeighbor(DmtxPointFlow center, int sign)
        {
            DmtxPixelLoc loc = new DmtxPixelLoc();
            DmtxPointFlow[] flow = new DmtxPointFlow[8];

            int attempt = (sign < 0) ? center.Depart : (center.Depart + 4) % 8;

            int occupied = 0;
            int strongIdx = DmtxConstants.DmtxUndefined;
            for (int i = 0; i < 8; i++)
            {

                loc.X = center.Loc.X + DmtxConstants.DmtxPatternX[i];
                loc.Y = center.Loc.Y + DmtxConstants.DmtxPatternY[i];
                int cacheIndex = DecodeGetCache(loc.X, loc.Y);
                if (cacheIndex == DmtxConstants.DmtxUndefined)
                {
                    continue;
                }

                if ((this._cache[cacheIndex] & 0x80) != 0x00)
                {
                    if (++occupied > 2)
                        return DmtxConstants.DmtxBlankEdge;
                    continue;
                }

                int attemptDiff = Math.Abs(attempt - i);
                if (attemptDiff > 4)
                    attemptDiff = 8 - attemptDiff;
                if (attemptDiff > 1)
                    continue;

                flow[i] = GetPointFlow(center.Plane, loc, i);

                if (strongIdx == DmtxConstants.DmtxUndefined || flow[i].Mag > flow[strongIdx].Mag ||
                      (flow[i].Mag == flow[strongIdx].Mag && ((i & 0x01) != 0)))
                {
                    strongIdx = i;
                }
            }

            return (strongIdx == DmtxConstants.DmtxUndefined) ? DmtxConstants.DmtxBlankEdge : flow[strongIdx];
        }

        DmtxPointFlow GetPointFlow(int colorPlane, DmtxPixelLoc loc, int arrive)
        {
            int[] coefficient = new[] { 0, 1, 2, 1, 0, -1, -2, -1 };
            int patternIdx;
            int compass;
            int[] mag = new int[4];
            int[] colorPattern = new int[8];
            DmtxPointFlow flow = new DmtxPointFlow();

            for (patternIdx = 0; patternIdx < 8; patternIdx++)
            {
                int xAdjust = loc.X + DmtxConstants.DmtxPatternX[patternIdx];
                int yAdjust = loc.Y + DmtxConstants.DmtxPatternY[patternIdx];
                bool err = GetPixelValue(xAdjust, yAdjust, colorPlane, ref colorPattern[patternIdx]);
                if (err == false)
                {
                    return DmtxConstants.DmtxBlankEdge;
                }
            }

            /* Calculate this pixel's flow intensity for each direction (-45, 0, 45, 90) */
            int compassMax = 0;
            for (compass = 0; compass < 4; compass++)
            {

                /* Add portion from each position in the convolution matrix pattern */
                for (patternIdx = 0; patternIdx < 8; patternIdx++)
                {

                    int coefficientIdx = (patternIdx - compass + 8) % 8;
                    if (coefficient[coefficientIdx] == 0)
                        continue;

                    int color = colorPattern[patternIdx];

                    switch (coefficient[coefficientIdx])
                    {
                        case 2:
                            mag[compass] += 2 * color;
                            break;
                        case 1:
                            mag[compass] += color;
                            break;
                        case -2:
                            mag[compass] -= 2 * color;
                            break;
                        case -1:
                            mag[compass] -= color;
                            break;
                    }
                }

                /* Identify strongest compass flow */
                if (compass != 0 && Math.Abs(mag[compass]) > Math.Abs(mag[compassMax]))
                    compassMax = compass;
            }

            /* Convert signed compass direction into unique flow directions (0-7) */
            flow.Plane = colorPlane;
            flow.Arrive = arrive;
            flow.Depart = (mag[compassMax] > 0) ? compassMax + 4 : compassMax;
            flow.Mag = Math.Abs(mag[compassMax]);
            flow.Loc = loc;

            return flow;
        }

        bool MatrixRegionFindSize(DmtxRegion reg)
        {
            DmtxSymbolSize sizeIdxBeg, sizeIdxEnd;
            DmtxSymbolSize sizeIdx;
            int bestColorOffAvg;

            DmtxSymbolSize bestSizeIdx = DmtxSymbolSize.DmtxSymbolShapeAuto;
            int bestContrast = 0;
            int bestColorOnAvg = bestColorOffAvg = 0;

            if (this._sizeIdxExpected == DmtxSymbolSize.DmtxSymbolShapeAuto)
            {
                sizeIdxBeg = 0;
                sizeIdxEnd = (DmtxSymbolSize)(DmtxConstants.DmtxSymbolSquareCount + DmtxConstants.DmtxSymbolRectCount);
            }
            else if (this._sizeIdxExpected == DmtxSymbolSize.DmtxSymbolSquareAuto)
            {
                sizeIdxBeg = 0;
                sizeIdxEnd = (DmtxSymbolSize)DmtxConstants.DmtxSymbolSquareCount;
            }
            else if (this._sizeIdxExpected == DmtxSymbolSize.DmtxSymbolRectAuto)
            {
                sizeIdxBeg = (DmtxSymbolSize)DmtxConstants.DmtxSymbolSquareCount;
                sizeIdxEnd = (DmtxSymbolSize)(DmtxConstants.DmtxSymbolSquareCount + DmtxConstants.DmtxSymbolRectCount);
            }
            else
            {
                sizeIdxBeg = this._sizeIdxExpected;
                sizeIdxEnd = this._sizeIdxExpected + 1;
            }

            /* Test each barcode size to find best contrast in calibration modules */
            for (sizeIdx = sizeIdxBeg; sizeIdx < sizeIdxEnd; sizeIdx++)
            {

                int symbolRows = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolRows, sizeIdx);
                int symbolCols = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolCols, sizeIdx);
                int colorOffAvg;
                int colorOnAvg = colorOffAvg = 0;

                /* Sum module colors along horizontal calibration bar */
                int row = symbolRows - 1;
                int col;
                int color;
                for (col = 0; col < symbolCols; col++)
                {
                    color = ReadModuleColor(reg, row, col, sizeIdx, reg.FlowBegin.Plane);
                    if ((col & 0x01) != 0x00)
                        colorOffAvg += color;
                    else
                        colorOnAvg += color;
                }

                /* Sum module colors along vertical calibration bar */
                col = symbolCols - 1;
                for (row = 0; row < symbolRows; row++)
                {
                    color = ReadModuleColor(reg, row, col, sizeIdx, reg.FlowBegin.Plane);
                    if ((row & 0x01) != 0x00)
                        colorOffAvg += color;
                    else
                        colorOnAvg += color;
                }

                colorOnAvg = (colorOnAvg * 2) / (symbolRows + symbolCols);
                colorOffAvg = (colorOffAvg * 2) / (symbolRows + symbolCols);

                int contrast = Math.Abs(colorOnAvg - colorOffAvg);
                if (contrast < 20)
                    continue;

                if (contrast > bestContrast)
                {
                    bestContrast = contrast;
                    bestSizeIdx = sizeIdx;
                    bestColorOnAvg = colorOnAvg;
                    bestColorOffAvg = colorOffAvg;
                }
            }

            /* If no sizes produced acceptable contrast then call it quits */
            if (bestSizeIdx == DmtxSymbolSize.DmtxSymbolShapeAuto || bestContrast < 20)
                return false;

            reg.SizeIdx = bestSizeIdx;
            reg.OnColor = bestColorOnAvg;
            reg.OffColor = bestColorOffAvg;

            reg.SymbolRows = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolRows, reg.SizeIdx);
            reg.SymbolCols = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribSymbolCols, reg.SizeIdx);
            reg.MappingRows = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribMappingMatrixRows, reg.SizeIdx);
            reg.MappingCols = DmtxCommon.GetSymbolAttribute(DmtxSymAttribute.DmtxSymAttribMappingMatrixCols, reg.SizeIdx);

            /* Tally jumps on horizontal calibration bar to verify sizeIdx */
            int jumpCount = this.CountJumpTally(reg, 0, reg.SymbolRows - 1, DmtxDirection.DmtxDirRight);
            int errors = Math.Abs(1 + jumpCount - reg.SymbolCols);
            if (jumpCount < 0 || errors > 2)
                return false;

            /* Tally jumps on vertical calibration bar to verify sizeIdx */
            jumpCount = CountJumpTally(reg, reg.SymbolCols - 1, 0, DmtxDirection.DmtxDirUp);
            errors = Math.Abs(1 + jumpCount - reg.SymbolRows);
            if (jumpCount < 0 || errors > 2)
                return false;

            /* Tally jumps on horizontal finder bar to verify sizeIdx */
            errors = CountJumpTally(reg, 0, 0, DmtxDirection.DmtxDirRight);
            if (jumpCount < 0 || errors > 2)
                return false;

            /* Tally jumps on vertical finder bar to verify sizeIdx */
            errors = CountJumpTally(reg, 0, 0, DmtxDirection.DmtxDirUp);
            if (errors < 0 || errors > 2)
                return false;

            /* Tally jumps on surrounding whitespace, else fail */
            errors = CountJumpTally(reg, 0, -1, DmtxDirection.DmtxDirRight);
            if (errors < 0 || errors > 2)
                return false;

            errors = CountJumpTally(reg, -1, 0, DmtxDirection.DmtxDirUp);
            if (errors < 0 || errors > 2)
                return false;

            errors = CountJumpTally(reg, 0, reg.SymbolRows, DmtxDirection.DmtxDirRight);
            if (errors < 0 || errors > 2)
                return false;

            errors = CountJumpTally(reg, reg.SymbolCols, 0, DmtxDirection.DmtxDirUp);
            if (errors < 0 || errors > 2)
                return false;

            return true;
        }

        int CountJumpTally(DmtxRegion reg, int xStart, int yStart, DmtxDirection dir)
        {
            int x, xInc = 0;
            int y, yInc = 0;
            int state = DmtxConstants.DmtxModuleOn;
            int jumpCount = 0;

            if (xStart != 0 && yStart != 0)
            {
                throw new Exception("CountJumpTally failed, xStart or yStart must be zero!");
            }

            if (dir == DmtxDirection.DmtxDirRight)
            {
                xInc = 1;
            }
            else
            {
                yInc = 1;
            }

            if (xStart == -1 || xStart == reg.SymbolCols ||
                  yStart == -1 || yStart == reg.SymbolRows)
            {
                state = DmtxConstants.DmtxModuleOff;
            }

            bool darkOnLight = (reg.OffColor > reg.OnColor);
            int jumpThreshold = Math.Abs((int)(0.4 * (reg.OnColor - reg.OffColor) + 0.5));
            int color = this.ReadModuleColor(reg, yStart, xStart, reg.SizeIdx, reg.FlowBegin.Plane);
            int tModule = (darkOnLight) ? reg.OffColor - color : color - reg.OffColor;

            for (x = xStart + xInc, y = yStart + yInc;
                  (dir == DmtxDirection.DmtxDirRight && x < reg.SymbolCols) ||
                  (dir == DmtxDirection.DmtxDirUp && y < reg.SymbolRows);
                  x += xInc, y += yInc)
            {

                int tPrev = tModule;
                color = ReadModuleColor(reg, y, x, reg.SizeIdx, reg.FlowBegin.Plane);
                tModule = (darkOnLight) ? reg.OffColor - color : color - reg.OffColor;

                if (state == DmtxConstants.DmtxModuleOff)
                {
                    if (tModule > tPrev + jumpThreshold)
                    {
                        jumpCount++;
                        state = DmtxConstants.DmtxModuleOn;
                    }
                }
                else
                {
                    if (tModule < tPrev - jumpThreshold)
                    {
                        jumpCount++;
                        state = DmtxConstants.DmtxModuleOff;
                    }
                }
            }

            return jumpCount;
        }

        bool MatrixRegionOrientation(DmtxRegion reg, DmtxPointFlow begin)
        {
            int cross;
            DmtxSymbolSize symbolShape;
            int maxDiagonal;
            DmtxBestLine line2X;

            if (this._sizeIdxExpected == DmtxSymbolSize.DmtxSymbolSquareAuto ||
                  (this._sizeIdxExpected >= DmtxSymbolSize.DmtxSymbol10x10 &&
                  this._sizeIdxExpected <= DmtxSymbolSize.DmtxSymbol144x144))
                symbolShape = DmtxSymbolSize.DmtxSymbolSquareAuto;
            else if (this._sizeIdxExpected == DmtxSymbolSize.DmtxSymbolRectAuto ||
                  (this._sizeIdxExpected >= DmtxSymbolSize.DmtxSymbol8x18 &&
                  this._sizeIdxExpected <= DmtxSymbolSize.DmtxSymbol16x48))
                symbolShape = DmtxSymbolSize.DmtxSymbolRectAuto;
            else
                symbolShape = DmtxSymbolSize.DmtxSymbolShapeAuto;

            if (_edgeMax != DmtxConstants.DmtxUndefined)
            {
                if (symbolShape == DmtxSymbolSize.DmtxSymbolRectAuto)
                    maxDiagonal = (int)(1.23 * _edgeMax + 0.5); /* sqrt(5/4) + 10% */
                else
                    maxDiagonal = (int)(1.56 * _edgeMax + 0.5); /* sqrt(2) + 10% */
            }
            else
            {
                maxDiagonal = DmtxConstants.DmtxUndefined;
            }

            /* Follow to end in both directions */
            bool err = this.TrailBlazeContinuous(reg, begin, maxDiagonal);
            if (err == false || reg.StepsTotal < 40)
            {
                TrailClear(reg, 0x40);
                return false;
            }

            /* Filter out region candidates that are smaller than expected */
            if (this._edgeMin != DmtxConstants.DmtxUndefined)
            {
                int scale = _scale;

                int minArea;
                if (symbolShape == DmtxSymbolSize.DmtxSymbolSquareAuto)
                    minArea = (this._edgeMin * this._edgeMin) / (scale * scale);
                else
                    minArea = (2 * this._edgeMin * this._edgeMin) / (scale * scale);

                if ((reg.BoundMax.X - reg.BoundMin.X) * (reg.BoundMax.Y - reg.BoundMin.Y) < minArea)
                {
                    TrailClear(reg, 0x40);
                    return false;
                }
            }

            DmtxBestLine line1X = this.FindBestSolidLine(reg, 0, 0, 1, DmtxConstants.DmtxUndefined);
            if (line1X.Mag < 5)
            {
                TrailClear(reg, 0x40);
                return false;
            }

            this.FindTravelLimits(reg, ref line1X);
            if (line1X.DistSq < 100 || line1X.Devn * 10 >= Math.Sqrt(line1X.DistSq))
            {
                TrailClear(reg, 0x40);
                return false;
            }
            if (!(line1X.StepPos >= line1X.StepNeg))
            {
                throw new Exception("Error calculating matrix region orientation");
            }

            DmtxFollow fTmp = this.FollowSeek(reg, line1X.StepPos + 5);
            DmtxBestLine line2P = this.FindBestSolidLine(reg, fTmp.Step, line1X.StepNeg, 1, line1X.Angle);

            fTmp = FollowSeek(reg, line1X.StepNeg - 5);
            DmtxBestLine line2N = this.FindBestSolidLine(reg, fTmp.Step, line1X.StepPos, -1, line1X.Angle);
            if (DmtxCommon.Max(line2P.Mag, line2N.Mag) < 5)
                return false;

            if (line2P.Mag > line2N.Mag)
            {
                line2X = line2P;
                this.FindTravelLimits(reg, ref line2X);
                if (line2X.DistSq < 100 || line2X.Devn * 10 >= Math.Sqrt(line2X.DistSq))
                    return false;

                cross = ((line1X.LocPos.X - line1X.LocNeg.X) * (line2X.LocPos.Y - line2X.LocNeg.Y)) -
                      ((line1X.LocPos.Y - line1X.LocNeg.Y) * (line2X.LocPos.X - line2X.LocNeg.X));
                if (cross > 0)
                {
                    /* Condition 2 */
                    reg.Polarity = +1;
                    reg.LocR = line2X.LocPos;
                    reg.StepR = line2X.StepPos;
                    reg.LocT = line1X.LocNeg;
                    reg.StepT = line1X.StepNeg;
                    reg.LeftLoc = line1X.LocBeg;
                    reg.LeftAngle = line1X.Angle;
                    reg.BottomLoc = line2X.LocBeg;
                    reg.BottomAngle = line2X.Angle;
                    reg.LeftLine = line1X;
                    reg.BottomLine = line2X;
                }
                else
                {
                    /* Condition 3 */
                    reg.Polarity = -1;
                    reg.LocR = line1X.LocNeg;
                    reg.StepR = line1X.StepNeg;
                    reg.LocT = line2X.LocPos;
                    reg.StepT = line2X.StepPos;
                    reg.LeftLoc = line2X.LocBeg;
                    reg.LeftAngle = line2X.Angle;
                    reg.BottomLoc = line1X.LocBeg;
                    reg.BottomAngle = line1X.Angle;
                    reg.LeftLine = line2X;
                    reg.BottomLine = line1X;
                }
            }
            else
            {
                line2X = line2N;
                this.FindTravelLimits(reg, ref line2X);
                if (line2X.DistSq < 100 || line2X.Devn / Math.Sqrt(line2X.DistSq) >= 0.1)
                    return false;

                cross = ((line1X.LocNeg.X - line1X.LocPos.X) * (line2X.LocNeg.Y - line2X.LocPos.Y)) -
                      ((line1X.LocNeg.Y - line1X.LocPos.Y) * (line2X.LocNeg.X - line2X.LocPos.X));
                if (cross > 0)
                {
                    /* Condition 1 */
                    reg.Polarity = -1;
                    reg.LocR = line2X.LocNeg;
                    reg.StepR = line2X.StepNeg;
                    reg.LocT = line1X.LocPos;
                    reg.StepT = line1X.StepPos;
                    reg.LeftLoc = line1X.LocBeg;
                    reg.LeftAngle = line1X.Angle;
                    reg.BottomLoc = line2X.LocBeg;
                    reg.BottomAngle = line2X.Angle;
                    reg.LeftLine = line1X;
                    reg.BottomLine = line2X;
                }
                else
                {
                    /* Condition 4 */
                    reg.Polarity = +1;
                    reg.LocR = line1X.LocPos;
                    reg.StepR = line1X.StepPos;
                    reg.LocT = line2X.LocNeg;
                    reg.StepT = line2X.StepNeg;
                    reg.LeftLoc = line2X.LocBeg;
                    reg.LeftAngle = line2X.Angle;
                    reg.BottomLoc = line1X.LocBeg;
                    reg.BottomAngle = line1X.Angle;
                    reg.LeftLine = line2X;
                    reg.BottomLine = line1X;
                }
            }

            reg.LeftKnown = reg.BottomKnown = 1;

            return true;
        }

        DmtxBestLine FindBestSolidLine(DmtxRegion reg, int step0, int step1, int streamDir, int houghAvoid)
        {
            int[,] hough = new int[3, DmtxConstants.DmtxHoughRes];
            char[] houghTest = new char[DmtxConstants.DmtxHoughRes];
            int i;
            int step;
            int sign = 0;
            int tripSteps = 0;
            DmtxBestLine line = new DmtxBestLine();

            int angleBest = 0;
            int hOffsetBest = 0;

            /* Always follow path flowing away from the trail start */
            if (step0 != 0)
            {
                if (step0 > 0)
                {
                    sign = +1;
                    tripSteps = (step1 - step0 + reg.StepsTotal) % reg.StepsTotal;
                }
                else
                {
                    sign = -1;
                    tripSteps = (step0 - step1 + reg.StepsTotal) % reg.StepsTotal;
                }
                if (tripSteps == 0)
                    tripSteps = reg.StepsTotal;
            }
            else if (step1 != 0)
            {
                sign = (step1 > 0) ? +1 : -1;
                tripSteps = Math.Abs(step1);
            }
            else if (step1 == 0)
            {
                sign = +1;
                tripSteps = reg.StepsTotal;
            }
            if (sign != streamDir)
            {
                throw new Exception("Sign must equal stream direction!");
            }

            DmtxFollow follow = this.FollowSeek(reg, step0);
            DmtxPixelLoc rHp = follow.Loc;

            line.StepBeg = line.StepPos = line.StepNeg = step0;
            line.LocBeg = follow.Loc;
            line.LocPos = follow.Loc;
            line.LocNeg = follow.Loc;

            /* Predetermine which angles to test */
            for (i = 0; i < DmtxConstants.DmtxHoughRes; i++)
            {
                if (houghAvoid == DmtxConstants.DmtxUndefined)
                {
                    houghTest[i] = (char)1;
                }
                else
                {
                    int houghMin = (houghAvoid + DmtxConstants.DmtxHoughRes / 6) % DmtxConstants.DmtxHoughRes;
                    int houghMax = (houghAvoid - DmtxConstants.DmtxHoughRes / 6 + DmtxConstants.DmtxHoughRes) % DmtxConstants.DmtxHoughRes;
                    if (houghMin > houghMax)
                        houghTest[i] = (i > houghMin || i < houghMax) ? (char)1 : (char)0;
                    else
                        houghTest[i] = (i > houghMin && i < houghMax) ? (char)1 : (char)0;
                }
            }

            /* Test each angle for steps along path */
            for (step = 0; step < tripSteps; step++)
            {

                int xDiff = follow.Loc.X - rHp.X;
                int yDiff = follow.Loc.Y - rHp.Y;

                /* Increment Hough accumulator */
                for (i = 0; i < DmtxConstants.DmtxHoughRes; i++)
                {

                    if (houghTest[i] == 0)
                        continue;

                    int dH = (DmtxConstants.rHvX[i] * yDiff) - (DmtxConstants.rHvY[i] * xDiff);
                    if (dH >= -384 && dH <= 384)
                    {
                        int hOffset;
                        if (dH > 128)
                            hOffset = 2;
                        else if (dH >= -128)
                            hOffset = 1;
                        else
                            hOffset = 0;

                        hough[hOffset, i]++;

                        /* New angle takes over lead */
                        if (hough[hOffset, i] > hough[hOffsetBest, angleBest])
                        {
                            angleBest = i;
                            hOffsetBest = hOffset;
                        }
                    }
                }

                /*    CALLBACK_POINT_PLOT(follow.loc, (sign > 1) ? 4 : 3, 1, 2); */

                follow = FollowStep(reg, follow, sign);
            }

            line.Angle = angleBest;
            line.HOffset = hOffsetBest;
            line.Mag = hough[hOffsetBest, angleBest];

            return line;
        }

        DmtxFollow FollowSeek(DmtxRegion reg, int seek)
        {
            int i;
            DmtxFollow follow = new DmtxFollow {Loc = reg.FlowBegin.Loc, Step = 0, Ptr = this._cache};

            follow.PtrIndex = DecodeGetCache(follow.Loc.X, follow.Loc.Y);

            int sign = (seek > 0) ? +1 : -1;
            for (i = 0; i != seek; i += sign)
            {
                follow = FollowStep(reg, follow, sign);
                if (Math.Abs(follow.Step) > reg.StepsTotal)
                {
                    throw new Exception("Follow step count larger total step count!");
                }
            }
            return follow;
        }


        bool TrailBlazeContinuous(DmtxRegion reg, DmtxPointFlow flowBegin, int maxDiagonal)
        {
            int negAssigns;
            int sign;
            DmtxPixelLoc boundMax;

            DmtxPixelLoc boundMin = boundMax = flowBegin.Loc;
            int cacheBegIndex = DecodeGetCache(flowBegin.Loc.X, flowBegin.Loc.Y);
            this._cache[cacheBegIndex] = 0x80 | 0x40;

            reg.FlowBegin = flowBegin;

            int posAssigns = negAssigns = 0;
            for (sign = 1; sign >= -1; sign -= 2)
            {

                DmtxPointFlow flow = flowBegin;
                int cacheIndex = cacheBegIndex;

                int steps;
                for (steps = 0; ; steps++)
                {

                    if (maxDiagonal != DmtxConstants.DmtxUndefined && (boundMax.X - boundMin.X > maxDiagonal ||
                          boundMax.Y - boundMin.Y > maxDiagonal))
                        break;

                    /* Find the strongest eligible neighbor */
                    DmtxPointFlow flowNext = FindStrongestNeighbor(flow, sign);
                    if (flowNext.Mag < 50)
                        break;

                    /* Get the neighbor's cache location */
                    int cacheNextIndex = DecodeGetCache(flowNext.Loc.X, flowNext.Loc.Y);
                    if ((this._cache[cacheNextIndex] & 0x80) != 0)
                    {
                        throw new Exception("Error creating Trail Blaze");
                    }

                    /* Mark departure from current location. If flowing downstream
                     * (sign < 0) then departure vector here is the arrival vector
                     * of the next location. Upstream flow uses the opposite rule. */
                    this._cache[cacheIndex] |= (sign < 0) ? (byte)(flowNext.Arrive) : (byte)(flowNext.Arrive << 3);

                    /* Mark known direction for next location */
                    /* If testing downstream (sign < 0) then next upstream is opposite of next arrival */
                    /* If testing upstream (sign > 0) then next downstream is opposite of next arrival */
                    this._cache[cacheNextIndex] = (sign < 0) ? (byte)(((flowNext.Arrive + 4) % 8) << 3) : (byte)((flowNext.Arrive + 4) % 8);
                    this._cache[cacheNextIndex] |= (0x80 | 0x40); /* Mark location as visited and assigned */

                    if (sign > 0)
                        posAssigns++;
                    else
                        negAssigns++;
                    cacheIndex = cacheNextIndex;
                    flow = flowNext;

                    if (flow.Loc.X > boundMax.X)
                        boundMax.X = flow.Loc.X;
                    else if (flow.Loc.X < boundMin.X)
                        boundMin.X = flow.Loc.X;
                    if (flow.Loc.Y > boundMax.Y)
                        boundMax.Y = flow.Loc.Y;
                    else if (flow.Loc.Y < boundMin.Y)
                        boundMin.Y = flow.Loc.Y;

                    /*       CALLBACK_POINT_PLOT(flow.loc, (sign > 0) ? 2 : 3, 1, 2); */
                }

                if (sign > 0)
                {
                    reg.FinalPos = flow.Loc;
                    reg.JumpToNeg = steps;
                }
                else
                {
                    reg.FinalNeg = flow.Loc;
                    reg.JumpToPos = steps;
                }
            }
            reg.StepsTotal = reg.JumpToPos + reg.JumpToNeg;
            reg.BoundMin = boundMin;
            reg.BoundMax = boundMax;

            /* Clear "visited" bit from trail */
            int clears = this.TrailClear(reg, 0x80);
            if (posAssigns + negAssigns != clears - 1)
            {
                throw new Exception("Error cleaning after trail blaze continuous");
            }

            /* XXX clean this up ... redundant test above */
            if (maxDiagonal != DmtxConstants.DmtxUndefined && (boundMax.X - boundMin.X > maxDiagonal ||
                  boundMax.Y - boundMin.Y > maxDiagonal))
                return false;

            return true;
        }

        int TrailClear(DmtxRegion reg, int clearMask)
        {
            if ((clearMask | 0xff) != 0xff)
            {
                throw new Exception("TrailClear mask is invalid!");
            }

            /* Clear "visited" bit from trail */
            int clears = 0;
            DmtxFollow follow = this.FollowSeek(reg, 0);
            while (Math.Abs(follow.Step) <= reg.StepsTotal)
            {
                if ((follow.CurrentPtr & clearMask) == 0x00)
                {
                    throw new Exception("Error performing TrailClear");
                }
                follow.CurrentPtr &= (byte)(clearMask ^ 0xff);
                follow = FollowStep(reg, follow, +1);
                clears++;
            }

            return clears;
        }

        DmtxFollow FollowStep(DmtxRegion reg, DmtxFollow followBeg, int sign)
        {
            int stepMod;
            DmtxFollow follow = new DmtxFollow();


            if (Math.Abs(sign) != 1)
            {
                throw new Exception("Invalid parameter 'sign', can only be -1 or +1");
            }

            int factor = reg.StepsTotal + 1;
            if (sign > 0)
                stepMod = (factor + (followBeg.Step % factor)) % factor;
            else
                stepMod = (factor - (followBeg.Step % factor)) % factor;

            /* End of positive trail -- magic jump */
            if (sign > 0 && stepMod == reg.JumpToNeg)
            {
                follow.Loc = reg.FinalNeg;
            }
            /* End of negative trail -- magic jump */
            else if (sign < 0 && stepMod == reg.JumpToPos)
            {
                follow.Loc = reg.FinalPos;
            }
            /* Trail in progress -- normal jump */
            else
            {
                int patternIdx = (sign < 0) ? followBeg.Neighbor & 0x07 : ((followBeg.Neighbor & 0x38) >> 3);
                follow.Loc = new DmtxPixelLoc { X = followBeg.Loc.X + DmtxConstants.DmtxPatternX[patternIdx], Y = followBeg.Loc.Y + DmtxConstants.DmtxPatternY[patternIdx] };
            }

            follow.Step = followBeg.Step + sign;
            follow.Ptr = this._cache;
            follow.PtrIndex = DecodeGetCache(follow.Loc.X, follow.Loc.Y);

            return follow;
        }

        void FindTravelLimits(DmtxRegion reg, ref DmtxBestLine line)
        {
            int i;
            int negTravel;
            int posWanderMin, posWanderMax, posWanderMinLock, posWanderMaxLock;
            int negWanderMin, negWanderMax, negWanderMinLock, negWanderMaxLock;
            DmtxFollow followNeg;
            DmtxPixelLoc negMax;

            /* line->stepBeg is already known to sit on the best Hough line */
            DmtxFollow followPos = followNeg = this.FollowSeek(reg, line.StepBeg);
            DmtxPixelLoc loc0 = followPos.Loc;

            int cosAngle = DmtxConstants.rHvX[line.Angle];
            int sinAngle = DmtxConstants.rHvY[line.Angle];

            int distSqMax = 0;
            DmtxPixelLoc posMax = negMax = followPos.Loc;

            int posTravel = negTravel = 0;
            int posWander = posWanderMin = posWanderMax = posWanderMinLock = posWanderMaxLock = 0;
            int negWander = negWanderMin = negWanderMax = negWanderMinLock = negWanderMaxLock = 0;

            for (i = 0; i < reg.StepsTotal / 2; i++)
            {

                bool posRunning = (i < 10 || Math.Abs(posWander) < Math.Abs(posTravel));
                bool negRunning = (i < 10 || Math.Abs(negWander) < Math.Abs(negTravel));

                int distSq;
                int xDiff;
                int yDiff;
                if (posRunning)
                {
                    xDiff = followPos.Loc.X - loc0.X;
                    yDiff = followPos.Loc.Y - loc0.Y;
                    posTravel = (cosAngle * xDiff) + (sinAngle * yDiff);
                    posWander = (cosAngle * yDiff) - (sinAngle * xDiff);

                    if (posWander >= -3 * 256 && posWander <= 3 * 256)
                    {
                        distSq = DistanceSquared(followPos.Loc, negMax);
                        if (distSq > distSqMax)
                        {
                            posMax = followPos.Loc;
                            distSqMax = distSq;
                            line.StepPos = followPos.Step;
                            line.LocPos = followPos.Loc;
                            posWanderMinLock = posWanderMin;
                            posWanderMaxLock = posWanderMax;
                        }
                    }
                    else
                    {
                        posWanderMin = DmtxCommon.Min(posWanderMin, posWander);
                        posWanderMax = DmtxCommon.Max(posWanderMax, posWander);
                    }
                }
                else if (!negRunning)
                {
                    break;
                }

                if (negRunning)
                {
                    xDiff = followNeg.Loc.X - loc0.X;
                    yDiff = followNeg.Loc.Y - loc0.Y;
                    negTravel = (cosAngle * xDiff) + (sinAngle * yDiff);
                    negWander = (cosAngle * yDiff) - (sinAngle * xDiff);

                    if (negWander >= -3 * 256 && negWander < 3 * 256)
                    {
                        distSq = DistanceSquared(followNeg.Loc, posMax);
                        if (distSq > distSqMax)
                        {
                            negMax = followNeg.Loc;
                            distSqMax = distSq;
                            line.StepNeg = followNeg.Step;
                            line.LocNeg = followNeg.Loc;
                            negWanderMinLock = negWanderMin;
                            negWanderMaxLock = negWanderMax;
                        }
                    }
                    else
                    {
                        negWanderMin = DmtxCommon.Min(negWanderMin, negWander);
                        negWanderMax = DmtxCommon.Max(negWanderMax, negWander);
                    }
                }

                followPos = FollowStep(reg, followPos, +1);
                followNeg = FollowStep(reg, followNeg, -1);
            }
            line.Devn = DmtxCommon.Max(posWanderMaxLock - posWanderMinLock, negWanderMaxLock - negWanderMinLock) / 256;
            line.DistSq = distSqMax;

            return;
        }

        int DistanceSquared(DmtxPixelLoc a, DmtxPixelLoc b)
        {
            int xDelta = a.X - b.X;
            int yDelta = a.Y - b.Y;

            return (xDelta * xDelta) + (yDelta * yDelta);
        }

        bool RegionUpdateXfrms(DmtxRegion reg)
        {
            DmtxRay2 rLeft = new DmtxRay2();
            DmtxRay2 rBottom = new DmtxRay2();
            DmtxRay2 rTop = new DmtxRay2();
            DmtxRay2 rRight = new DmtxRay2();
            DmtxVector2 p00 = new DmtxVector2();
            DmtxVector2 p10 = new DmtxVector2();
            DmtxVector2 p11 = new DmtxVector2();
            DmtxVector2 p01 = new DmtxVector2();

            if (!(reg.LeftKnown != 0 && reg.BottomKnown != 0))
            {
                throw new ArgumentException("Error updating Xfrms!");
            }

            /* Build ray representing left edge */
            rLeft.P.X = reg.LeftLoc.X;
            rLeft.P.Y = reg.LeftLoc.Y;
            double radians = reg.LeftAngle * (Math.PI / DmtxConstants.DmtxHoughRes);
            rLeft.V.X = Math.Cos(radians);
            rLeft.V.Y = Math.Sin(radians);
            rLeft.TMin = 0.0;
            rLeft.TMax = rLeft.V.Norm();

            /* Build ray representing bottom edge */
            rBottom.P.X = reg.BottomLoc.X;
            rBottom.P.Y = reg.BottomLoc.Y;
            radians = reg.BottomAngle * (Math.PI / DmtxConstants.DmtxHoughRes);
            rBottom.V.X = Math.Cos(radians);
            rBottom.V.Y = Math.Sin(radians);
            rBottom.TMin = 0.0;
            rBottom.TMax = rBottom.V.Norm();

            /* Build ray representing top edge */
            if (reg.TopKnown != 0)
            {
                rTop.P.X = reg.TopLoc.X;
                rTop.P.Y = reg.TopLoc.Y;
                radians = reg.TopAngle * (Math.PI / DmtxConstants.DmtxHoughRes);
                rTop.V.X = Math.Cos(radians);
                rTop.V.Y = Math.Sin(radians);
                rTop.TMin = 0.0;
                rTop.TMax = rTop.V.Norm();
            }
            else
            {
                rTop.P.X = reg.LocT.X;
                rTop.P.Y = reg.LocT.Y;
                radians = reg.BottomAngle * (Math.PI / DmtxConstants.DmtxHoughRes);
                rTop.V.X = Math.Cos(radians);
                rTop.V.Y = Math.Sin(radians);
                rTop.TMin = 0.0;
                rTop.TMax = rBottom.TMax;
            }

            /* Build ray representing right edge */
            if (reg.RightKnown != 0)
            {
                rRight.P.X = reg.RightLoc.X;
                rRight.P.Y = reg.RightLoc.Y;
                radians = reg.RightAngle * (Math.PI / DmtxConstants.DmtxHoughRes);
                rRight.V.X = Math.Cos(radians);
                rRight.V.Y = Math.Sin(radians);
                rRight.TMin = 0.0;
                rRight.TMax = rRight.V.Norm();
            }
            else
            {
                rRight.P.X = reg.LocR.X;
                rRight.P.Y = reg.LocR.Y;
                radians = reg.LeftAngle * (Math.PI / DmtxConstants.DmtxHoughRes);
                rRight.V.X = Math.Cos(radians);
                rRight.V.Y = Math.Sin(radians);
                rRight.TMin = 0.0;
                rRight.TMax = rLeft.TMax;
            }

            /* Calculate 4 corners, real or imagined */
            if (!p00.Intersect(rLeft, rBottom))
                return false;

            if (!p10.Intersect(rBottom, rRight))
                return false;

            if (!p11.Intersect(rRight, rTop))
                return false;

            if (!p01.Intersect(rTop, rLeft))
                return false;

            if (!RegionUpdateCorners(reg, p00, p10, p11, p01))
                return false;

            return true;
        }

        bool RegionUpdateCorners(DmtxRegion reg, DmtxVector2 p00,
     DmtxVector2 p10, DmtxVector2 p11, DmtxVector2 p01)
        {
            double xMax = this.Width - 1;
            double yMax = this.Height - 1;

            if (p00.X < 0.0 || p00.Y < 0.0 || p00.X > xMax || p00.Y > yMax ||
                  p01.X < 0.0 || p01.Y < 0.0 || p01.X > xMax || p01.Y > yMax ||
                  p10.X < 0.0 || p10.Y < 0.0 || p10.X > xMax || p10.Y > yMax)
                return false;

            DmtxVector2 vOT = p01 - p00;
            DmtxVector2 vOR = p10 - p00;
            DmtxVector2 vTX = p11 - p01;
            DmtxVector2 vRX = p11 - p10;
            double dimOT = vOT.Mag();
            double dimOR = vOR.Mag();
            double dimTX = vTX.Mag();
            double dimRX = vRX.Mag();

            /* Verify that sides are reasonably long */
            if (dimOT <= 8.0 || dimOR <= 8.0 || dimTX <= 8.0 || dimRX <= 8.0)
                return false;

            /* Verify that the 4 corners define a reasonably fat quadrilateral */
            double ratio = dimOT / dimRX;
            if (ratio <= 0.5 || ratio >= 2.0)
                return false;

            ratio = dimOR / dimTX;
            if (ratio <= 0.5 || ratio >= 2.0)
                return false;

            /* Verify this is not a bowtie shape */
            if (vOR.Cross(vRX) <= 0.0 || vOT.Cross(vTX) >= 0.0)
                return false;

            if (DmtxCommon.RightAngleTrueness(p00, p10, p11, Math.PI / 2.0) <= this._squareDevn)
                return false;
            if (DmtxCommon.RightAngleTrueness(p10, p11, p01, Math.PI / 2.0) <= this._squareDevn)
                return false;

            /* Calculate values needed for transformations */
            double tx = -1 * p00.X;
            double ty = -1 * p00.Y;
            DmtxMatrix3 mtxy = DmtxMatrix3.Translate(tx, ty);

            double phi = Math.Atan2(vOT.X, vOT.Y);
            DmtxMatrix3 mphi = DmtxMatrix3.Rotate(phi);
            DmtxMatrix3 m = mtxy * mphi;

            DmtxVector2 vTmp = p10 * m;
            double shx = -vTmp.Y / vTmp.X;
            DmtxMatrix3 mshx = DmtxMatrix3.Shear(0.0, shx);
            m *= mshx;

            double scx = 1.0 / vTmp.X;
            DmtxMatrix3 mscx = DmtxMatrix3.Scale(scx, 1.0);
            m *= mscx;
            vTmp = p11 * m;

            double scy = 1.0 / vTmp.Y;
            DmtxMatrix3 mscy = DmtxMatrix3.Scale(1.0, scy);
            m *= mscy;

            vTmp = p11 * m;
            double skx = vTmp.X;
            DmtxMatrix3 mskx = DmtxMatrix3.LineSkewSide(1.0, skx, 1.0);
            m *= mskx;

            vTmp = p01 * m;
            double sky = vTmp.Y;
            DmtxMatrix3 msky = DmtxMatrix3.LineSkewTop(sky, 1.0, 1.0);
            reg.Raw2Fit = m * msky;

            /* Create inverse matrix by reverse (avoid straight matrix inversion) */
            msky = DmtxMatrix3.LineSkewTopInv(sky, 1.0, 1.0);
            mskx = DmtxMatrix3.LineSkewSideInv(1.0, skx, 1.0);
            m = msky * mskx;

            DmtxMatrix3 mscxy = DmtxMatrix3.Scale(1.0 / scx, 1.0 / scy);
            m *= mscxy;

            mshx = DmtxMatrix3.Shear(0.0, -shx);
            m *= mshx;

            mphi = DmtxMatrix3.Rotate(-phi);
            m *= mphi;

            mtxy = DmtxMatrix3.Translate(-tx, -ty);
            reg.Fit2Raw = m * mtxy;

            return true;
        }

        bool MatrixRegionAlignCalibEdge(DmtxRegion reg, DmtxEdge edgeLoc)
        {
            int streamDir;
            int avoidAngle;
            DmtxSymbolSize symbolShape;
            DmtxVector2 pTmp = new DmtxVector2();
            DmtxPixelLoc loc1 = new DmtxPixelLoc();
            DmtxPixelLoc locOrigin = new DmtxPixelLoc();
            DmtxFollow follow;

            /* Determine pixel coordinates of origin */
            pTmp.X = 0.0;
            pTmp.Y = 0.0;
            pTmp *= reg.Fit2Raw;
            locOrigin.X = (int)(pTmp.X + 0.5);
            locOrigin.Y = (int)(pTmp.Y + 0.5);

            if (this._sizeIdxExpected == DmtxSymbolSize.DmtxSymbolSquareAuto ||
                  (this._sizeIdxExpected >= DmtxSymbolSize.DmtxSymbol10x10 &&
                  this._sizeIdxExpected <= DmtxSymbolSize.DmtxSymbol144x144))
                symbolShape = DmtxSymbolSize.DmtxSymbolSquareAuto;
            else if (this._sizeIdxExpected == DmtxSymbolSize.DmtxSymbolRectAuto ||
                  (this._sizeIdxExpected >= DmtxSymbolSize.DmtxSymbol8x18 &&
                  this._sizeIdxExpected <= DmtxSymbolSize.DmtxSymbol16x48))
                symbolShape = DmtxSymbolSize.DmtxSymbolRectAuto;
            else
                symbolShape = DmtxSymbolSize.DmtxSymbolShapeAuto;

            /* Determine end locations of test line */
            if (edgeLoc == DmtxEdge.DmtxEdgeTop)
            {
                streamDir = reg.Polarity * -1;
                avoidAngle = reg.LeftLine.Angle;
                follow = FollowSeekLoc(reg.LocT);
                pTmp.X = 0.8;
                pTmp.Y = (symbolShape == DmtxSymbolSize.DmtxSymbolRectAuto) ? 0.2 : 0.6;
            }
            else
            {
                streamDir = reg.Polarity;
                avoidAngle = reg.BottomLine.Angle;
                follow = FollowSeekLoc(reg.LocR);
                pTmp.X = (symbolShape == DmtxSymbolSize.DmtxSymbolSquareAuto) ? 0.7 : 0.9;
                pTmp.Y = 0.8;
            }

            pTmp *= reg.Fit2Raw;
            loc1.X = (int)(pTmp.X + 0.5);
            loc1.Y = (int)(pTmp.Y + 0.5);

            DmtxPixelLoc loc0 = follow.Loc;
            DmtxBresLine line = new DmtxBresLine(loc0, loc1, locOrigin);
            int steps = this.TrailBlazeGapped(reg, line, streamDir);

            DmtxBestLine bestLine = this.FindBestSolidLine2(loc0, steps, streamDir, avoidAngle);

            if (edgeLoc == DmtxEdge.DmtxEdgeTop)
            {
                reg.TopKnown = 1;
                reg.TopAngle = bestLine.Angle;
                reg.TopLoc = bestLine.LocBeg;
            }
            else
            {
                reg.RightKnown = 1;
                reg.RightAngle = bestLine.Angle;
                reg.RightLoc = bestLine.LocBeg;
            }

            return true;
        }

        int TrailBlazeGapped(DmtxRegion reg, DmtxBresLine line, int streamDir)
        {
            int distSq;
            int travel = 0;
            int outward = 0;
            int[] dirMap = { 0, 1, 2, 7, 8, 3, 6, 5, 4 };

            DmtxPixelLoc loc0 = line.Loc;
            DmtxPointFlow flow = this.GetPointFlow(reg.FlowBegin.Plane, loc0, DmtxConstants.DmtxNeighborNone);
            int distSqMax = (line.XDelta * line.XDelta) + (line.YDelta * line.YDelta);
            int steps = 0;
            bool onEdge = true;

            DmtxPixelLoc beforeStep = loc0;
            int beforeCacheIndex = DecodeGetCache(loc0.X, loc0.Y);
            if (beforeCacheIndex == -1)
                return 0;
            
            this._cache[beforeCacheIndex] = 0;

            do
            {
                if (onEdge)
                {
                    DmtxPointFlow flowNext = FindStrongestNeighbor(flow, streamDir);
                    if (flowNext.Mag == DmtxConstants.DmtxUndefined)
                        break;

                    (new DmtxBresLine(line)).GetStep(flowNext.Loc, ref travel, ref outward);
                    if (flowNext.Mag < 50 || outward < 0 || (outward == 0 && travel < 0))
                    {
                        onEdge = false;
                    }
                    else
                    {
                        line.Step(travel, outward);
                        flow = flowNext;
                    }
                }

                if (!onEdge)
                {
                    line.Step(1, 0);
                    flow = GetPointFlow(reg.FlowBegin.Plane, line.Loc, DmtxConstants.DmtxNeighborNone);
                    if (flow.Mag > 50)
                        onEdge = true;
                }

                DmtxPixelLoc afterStep = line.Loc;
                int afterCacheIndex = DecodeGetCache(afterStep.X, afterStep.Y);
                if (afterCacheIndex == -1)
                    break;

                /* Determine step direction using pure magic */
                int xStep = afterStep.X - beforeStep.X;
                int yStep = afterStep.Y - beforeStep.Y;
                if (Math.Abs(xStep) > 1 || Math.Abs(yStep) > 1)
                {
                    throw new Exception("Invalid step directions!");
                }
                int stepDir = dirMap[3 * yStep + xStep + 4];

                if (stepDir == 8)
                {
                    throw new Exception("Invalid step direction!");
                }
                if (streamDir < 0)
                {
                    this._cache[beforeCacheIndex] |= (byte)(0x40 | stepDir);
                    this._cache[afterCacheIndex] = (byte)(((stepDir + 4) % 8) << 3);
                }
                else
                {
                    this._cache[beforeCacheIndex] |= (byte)(0x40 | (stepDir << 3));
                    this._cache[afterCacheIndex] = (byte)((stepDir + 4) % 8);
                }

                /* Guaranteed to have taken one step since top of loop */
                int xDiff = line.Loc.X - loc0.X;
                int yDiff = line.Loc.Y - loc0.Y;
                distSq = (xDiff * xDiff) + (yDiff * yDiff);

                beforeStep = line.Loc;
                beforeCacheIndex = afterCacheIndex;
                steps++;

            } while (distSq < distSqMax);

            return steps;
        }

        DmtxBestLine FindBestSolidLine2(DmtxPixelLoc loc0, int tripSteps, int sign, int houghAvoid)
        {
            int[,] hough = new int[3, DmtxConstants.DmtxHoughRes];
            char[] houghTest = new char[DmtxConstants.DmtxHoughRes];
            int i;
            int step;
            DmtxBestLine line = new DmtxBestLine();

            int angleBest = 0;
            int hOffsetBest = 0;

            DmtxFollow follow = this.FollowSeekLoc(loc0);
            DmtxPixelLoc rHp = line.LocBeg = line.LocPos = line.LocNeg = follow.Loc;
            line.StepBeg = line.StepPos = line.StepNeg = 0;

            /* Predetermine which angles to test */
            for (i = 0; i < DmtxConstants.DmtxHoughRes; i++)
            {
                if (houghAvoid == DmtxConstants.DmtxUndefined)
                {
                    houghTest[i] = (char)1;
                }
                else
                {
                    int houghMin = (houghAvoid + DmtxConstants.DmtxHoughRes / 6) % DmtxConstants.DmtxHoughRes;
                    int houghMax = (houghAvoid - DmtxConstants.DmtxHoughRes / 6 + DmtxConstants.DmtxHoughRes) % DmtxConstants.DmtxHoughRes;
                    if (houghMin > houghMax)
                        houghTest[i] = (i > houghMin || i < houghMax) ? (char)1 : (char)0;
                    else
                        houghTest[i] = (i > houghMin && i < houghMax) ? (char)1 : (char)0;
                }
            }

            /* Test each angle for steps along path */
            for (step = 0; step < tripSteps; step++)
            {

                int xDiff = follow.Loc.X - rHp.X;
                int yDiff = follow.Loc.Y - rHp.Y;

                /* Increment Hough accumulator */
                for (i = 0; i < DmtxConstants.DmtxHoughRes; i++)
                {

                    if (houghTest[i] == 0)
                        continue;

                    int dH = (DmtxConstants.rHvX[i] * yDiff) - (DmtxConstants.rHvY[i] * xDiff);
                    if (dH >= -384 && dH <= 384)
                    {
                        int hOffset;
                        if (dH > 128)
                            hOffset = 2;
                        else if (dH >= -128)
                            hOffset = 1;
                        else
                            hOffset = 0;

                        hough[hOffset, i]++;

                        /* New angle takes over lead */
                        if (hough[hOffset, i] > hough[hOffsetBest, angleBest])
                        {
                            angleBest = i;
                            hOffsetBest = hOffset;
                        }
                    }
                }
                follow = FollowStep2(follow, sign);
            }

            line.Angle = angleBest;
            line.HOffset = hOffsetBest;
            line.Mag = hough[hOffsetBest, angleBest];

            return line;
        }

        DmtxFollow FollowStep2(DmtxFollow followBeg, int sign)
        {
            DmtxFollow follow = new DmtxFollow();

            if (Math.Abs(sign) != 1)
            {
                throw new Exception("Invalid parameter 'sign', can only be -1 or +1");
            }
            if ((followBeg.Neighbor & 0x40) == 0x00)
            {
                throw new Exception("Invalid value for neighbor!");
            }

            int patternIdx = (sign < 0) ? followBeg.Neighbor & 0x07 : ((followBeg.Neighbor & 0x38) >> 3);
            follow.Loc = new DmtxPixelLoc { X = followBeg.Loc.X + DmtxConstants.DmtxPatternX[patternIdx], Y = followBeg.Loc.Y + DmtxConstants.DmtxPatternY[patternIdx] };

            follow.Step = followBeg.Step + sign;
            follow.Ptr = this._cache;
            follow.PtrIndex = DecodeGetCache(follow.Loc.X, follow.Loc.Y);

            return follow;
        }

        DmtxFollow FollowSeekLoc(DmtxPixelLoc loc)
        {
            DmtxFollow follow = new DmtxFollow {Loc = loc, Step = 0, Ptr = this._cache};

            follow.PtrIndex = DecodeGetCache(follow.Loc.X, follow.Loc.Y);

            return follow;
        }
        #endregion

        #region Properties
        internal int EdgeMin
        {
            get { return _edgeMin; }
            set
            {
                _edgeMin = value;
                ValidateSettingsAndInitScanGrid();
            }
        }

        internal int EdgeMax
        {
            get { return _edgeMax; }
            set { _edgeMax = value; ValidateSettingsAndInitScanGrid(); }
        }

        internal int ScanGap
        {
            get { return _scanGap; }
            set { _scanGap = value; ValidateSettingsAndInitScanGrid(); }
        }

        internal int SquareDevn
        {
            get { return (int)(Math.Acos(this._squareDevn) * 180.0 / Math.PI); }
            set { _squareDevn = Math.Cos(value * (Math.PI / 180.0)); ValidateSettingsAndInitScanGrid(); }
        }

        internal DmtxSymbolSize SizeIdxExpected
        {
            get { return _sizeIdxExpected; }
            set { _sizeIdxExpected = value; ValidateSettingsAndInitScanGrid(); }
        }

        internal int EdgeThresh
        {
            get { return _edgeThresh; }
            set { _edgeThresh = value; ValidateSettingsAndInitScanGrid(); }
        }

        internal int XMin
        {
            get { return _xMin; }
            set { _xMin = value; ValidateSettingsAndInitScanGrid(); }
        }

        internal int XMax
        {
            get { return _xMax; }
            set { _xMax = value; ValidateSettingsAndInitScanGrid(); }
        }

        internal int YMin
        {
            get { return _yMin; }
            set { _yMin = value; ValidateSettingsAndInitScanGrid(); }
        }

        internal int YMax
        {
            get { return _yMax; }
            set { _yMax = value; ValidateSettingsAndInitScanGrid(); }
        }

        internal int Scale
        {
            get { return _scale; }
            set { _scale = value; ValidateSettingsAndInitScanGrid(); }
        }

        internal byte[] Cache
        {
            get { return _cache; }
            set { _cache = value; ValidateSettingsAndInitScanGrid(); }
        }

        internal DmtxImage Image
        {
            get { return _image; }
            set { _image = value; ValidateSettingsAndInitScanGrid(); }
        }


        internal DmtxScanGrid Grid
        {
            get { return _grid; }
            set { _grid = value; }
        }

        internal int Height
        {
            get
            {
                return _image.Height / _scale;
            }
        }


        internal int Width
        {
            get
            {
                return _image.Width / _scale;
            }
        }
        #endregion
    }
}

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
    internal class DmtxRegion
    {
        #region Constructors

        internal DmtxRegion()
        {
        }

        internal DmtxRegion(DmtxRegion src)
        {
            this.BottomAngle = src.BottomAngle;
            this.BottomKnown = src.BottomKnown;
            this.BottomLine = src.BottomLine;
            this.BottomLoc = src.BottomLoc;
            this.BoundMax = src.BoundMax;
            this.BoundMin = src.BoundMin;
            this.FinalNeg = src.FinalNeg;
            this.FinalPos = src.FinalPos;
            this.Fit2Raw = new DmtxMatrix3(src.Fit2Raw);
            this.FlowBegin = src.FlowBegin;
            this.JumpToNeg = src.JumpToNeg;
            this.JumpToPos = src.JumpToPos;
            this.LeftAngle = src.LeftAngle;
            this.LeftKnown = src.LeftKnown;
            this.LeftLine = src.LeftLine;
            this.LeftLoc = src.LeftLoc;
            this.LocR = src.LocR;
            this.LocT = src.LocT;
            this.MappingCols = src.MappingCols;
            this.MappingRows = src.MappingRows;
            this.OffColor = src.OffColor;
            this.OnColor = src.OnColor;
            this.Polarity = src.Polarity;
            this.Raw2Fit = new DmtxMatrix3(src.Raw2Fit);
            this.RightAngle = src.RightAngle;
            this.RightKnown = src.RightKnown;
            this.RightLoc = src.RightLoc;
            this.SizeIdx = src.SizeIdx;
            this.StepR = src.StepR;
            this.StepsTotal = src.StepsTotal;
            this.StepT = src.StepT;
            this.SymbolCols = src.SymbolCols;
            this.SymbolRows = src.SymbolRows;
            this.TopAngle = src.TopAngle;
            this.TopKnown = src.TopKnown;
            this.TopLoc = src.TopLoc;
        }
        #endregion

        #region Methods
        #endregion

        #region Properties

        internal int JumpToPos { get; set; }

        internal int JumpToNeg { get; set; }

        internal int StepsTotal { get; set; }

        internal DmtxPixelLoc FinalPos { get; set; }

        internal DmtxPixelLoc FinalNeg { get; set; }

        internal DmtxPixelLoc BoundMin { get; set; }

        internal DmtxPixelLoc BoundMax { get; set; }

        internal DmtxPointFlow FlowBegin { get; set; }

        internal int Polarity { get; set; }

        internal int StepR { get; set; }

        internal int StepT { get; set; }

        internal DmtxPixelLoc LocR { get; set; }

        internal DmtxPixelLoc LocT { get; set; }

        internal int LeftKnown { get; set; }

        internal int LeftAngle { get; set; }

        internal DmtxPixelLoc LeftLoc { get; set; }

        internal DmtxBestLine LeftLine { get; set; }

        internal int BottomKnown { get; set; }

        internal int BottomAngle { get; set; }

        internal DmtxPixelLoc BottomLoc { get; set; }

        internal DmtxBestLine BottomLine { get; set; }

        internal int TopKnown { get; set; }

        internal int TopAngle { get; set; }

        internal DmtxPixelLoc TopLoc { get; set; }

        internal int RightKnown { get; set; }

        internal int RightAngle { get; set; }

        internal DmtxPixelLoc RightLoc { get; set; }

        internal int OnColor { get; set; }

        internal int OffColor { get; set; }

        internal DmtxSymbolSize SizeIdx { get; set; }

        internal int SymbolRows { get; set; }

        internal int SymbolCols { get; set; }

        internal int MappingRows { get; set; }

        internal int MappingCols { get; set; }

        internal DmtxMatrix3 Raw2Fit { get; set; }

        internal DmtxMatrix3 Fit2Raw { get; set; }

        #endregion
    }
}

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
    internal struct DmtxBresLine
    {
        #region Fields
        int _xStep;
        int _yStep;
        int _xDelta;
        int _yDelta;
        bool _steep;
        int _xOut;
        int _yOut;
        int _travel;
        int _outward;
        int _error;
        DmtxPixelLoc _loc;
        DmtxPixelLoc _loc0;
        DmtxPixelLoc _loc1;
        #endregion

        #region Constructors
        internal DmtxBresLine(DmtxBresLine orig)
        {
            this._error = orig._error;
            this._loc = new DmtxPixelLoc { X = orig._loc.X, Y = orig._loc.Y };
            this._loc0 = new DmtxPixelLoc { X = orig._loc0.X, Y = orig._loc0.Y };
            this._loc1 = new DmtxPixelLoc { X = orig._loc1.X, Y = orig._loc1.Y };
            this._outward = orig._outward;
            this._steep = orig._steep;
            this._travel = orig._travel;
            this._xDelta = orig._xDelta;
            this._xOut = orig._xOut;
            this._xStep = orig._xStep;
            this._yDelta = orig._yDelta;
            this._yOut = orig._yOut;
            this._yStep = orig._yStep;
        }

        internal DmtxBresLine(DmtxPixelLoc loc0, DmtxPixelLoc loc1, DmtxPixelLoc locInside)
        {
            int cp;
            DmtxPixelLoc locBeg, locEnd;


            /* Values that stay the same after initialization */
            this._loc0 = loc0;
            this._loc1 = loc1;
            this._xStep = (loc0.X < loc1.X) ? +1 : -1;
            this._yStep = (loc0.Y < loc1.Y) ? +1 : -1;
            this._xDelta = Math.Abs(loc1.X - loc0.X);
            this._yDelta = Math.Abs(loc1.Y - loc0.Y);
            this._steep = (this._yDelta > this._xDelta);

            /* Take cross product to determine outward step */
            if (this._steep)
            {
                /* Point first vector up to get correct sign */
                if (loc0.Y < loc1.Y)
                {
                    locBeg = loc0;
                    locEnd = loc1;
                }
                else
                {
                    locBeg = loc1;
                    locEnd = loc0;
                }
                cp = (((locEnd.X - locBeg.X) * (locInside.Y - locEnd.Y)) -
                      ((locEnd.Y - locBeg.Y) * (locInside.X - locEnd.X)));

                this._xOut = (cp > 0) ? +1 : -1;
                this._yOut = 0;
            }
            else
            {
                /* Point first vector left to get correct sign */
                if (loc0.X > loc1.X)
                {
                    locBeg = loc0;
                    locEnd = loc1;
                }
                else
                {
                    locBeg = loc1;
                    locEnd = loc0;
                }
                cp = (((locEnd.X - locBeg.X) * (locInside.Y - locEnd.Y)) -
                      ((locEnd.Y - locBeg.Y) * (locInside.X - locEnd.X)));

                this._xOut = 0;
                this._yOut = (cp > 0) ? +1 : -1;
            }

            /* Values that change while stepping through line */
            this._loc = loc0;
            this._travel = 0;
            this._outward = 0;
            this._error = (this._steep) ? this._yDelta / 2 : this._xDelta / 2;
        }
        #endregion

        #region Methods
        internal bool GetStep(DmtxPixelLoc target, ref int travel, ref int outward)
        {
            /* Determine necessary step along and outward from Bresenham line */
            if (this._steep)
            {
                travel = (this._yStep > 0) ? target.Y - this._loc.Y : this._loc.Y - target.Y;
                Step(travel, 0);
                outward = (this._xOut > 0) ? target.X - this._loc.X : this._loc.X - target.X;
                if (this._yOut != 0)
                {
                    throw new Exception("Invald yOut value for bresline step!");
                }
            }
            else
            {
                travel = (this._xStep > 0) ? target.X - this._loc.X : this._loc.X - target.X;
                Step(travel, 0);
                outward = (this._yOut > 0) ? target.Y - this._loc.Y : this._loc.Y - target.Y;
                if (this._xOut != 0)
                {
                    throw new Exception("Invald xOut value for bresline step!");
                }
            }

            return true;
        }


        internal bool Step(int travel, int outward)
        {
            int i;

            if (Math.Abs(travel) >= 2)
            {
                throw new ArgumentException("Invalid value for 'travel' in BaseLineStep!");
            }

            /* Perform forward step */
            if (travel > 0)
            {
                this._travel++;
                if (this._steep)
                {
                    this._loc = new DmtxPixelLoc() { X = this._loc.X, Y = this._loc.Y + this._yStep };
                    this._error -= this._xDelta;
                    if (this._error < 0)
                    {
                        this._loc = new DmtxPixelLoc() { X = this._loc.X + this._xStep, Y = this._loc.Y };
                        this._error += this._yDelta;
                    }
                }
                else
                {
                    this._loc = new DmtxPixelLoc() { X = this._loc.X + this._xStep, Y = this._loc.Y };
                    this._error -= this._yDelta;
                    if (this._error < 0)
                    {
                        this._loc = new DmtxPixelLoc() { X = this._loc.X, Y = this._loc.Y + this._yStep };
                        this._error += this._xDelta;
                    }
                }
            }
            else if (travel < 0)
            {
                this._travel--;
                if (this._steep)
                {
                    this._loc = new DmtxPixelLoc() { X = this._loc.X, Y = this._loc.Y - this._yStep };
                    this._error += this._xDelta;
                    if (this.Error >= this.YDelta)
                    {
                        this._loc = new DmtxPixelLoc() { X = this._loc.X - this._xStep, Y = this._loc.Y };
                        this._error -= this._yDelta;
                    }
                }
                else
                {
                    this._loc = new DmtxPixelLoc() { X = this._loc.X - this._xStep, Y = this._loc.Y };
                    this._error += this._yDelta;
                    if (this._error >= this._xDelta)
                    {
                        this._loc = new DmtxPixelLoc() { X = this._loc.X, Y = this._loc.Y - this._yStep };
                        this._error -= this._xDelta;
                    }
                }
            }

            for (i = 0; i < outward; i++)
            {
                /* Outward steps */
                this._outward++;
                this._loc = new DmtxPixelLoc() { X = this._loc.X + this._xOut, Y = this._loc.Y + this._yOut };
            }

            return true;
        }
        #endregion

        #region Properties
        internal int XStep
        {
            get { return _xStep; }
            set { _xStep = value; }
        }

        internal int YStep
        {
            get { return _yStep; }
            set { _yStep = value; }
        }

        internal int XDelta
        {
            get { return _xDelta; }
            set { _xDelta = value; }
        }

        internal int YDelta
        {
            get { return _yDelta; }
            set { _yDelta = value; }
        }

        internal bool Steep
        {
            get { return _steep; }
            set { _steep = value; }
        }

        internal int XOut
        {
            get { return _xOut; }
            set { _xOut = value; }
        }

        internal int YOut
        {
            get { return _yOut; }
            set { _yOut = value; }
        }

        internal int Travel
        {
            get { return _travel; }
            set { _travel = value; }
        }

        internal int Outward
        {
            get { return _outward; }
            set { _outward = value; }
        }

        internal int Error
        {
            get { return _error; }
            set { _error = value; }
        }

        internal DmtxPixelLoc Loc
        {
            get { return _loc; }
            set { _loc = value; }
        }


        internal DmtxPixelLoc Loc0
        {
            get { return _loc0; }
            set { _loc0 = value; }
        }

        internal DmtxPixelLoc Loc1
        {
            get { return _loc1; }
            set { _loc1 = value; }
        }
        #endregion
    }
}

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
    internal class DmtxMatrix3
    {
        #region Fields
        double[,] _data;
        #endregion

        #region Constructors
        DmtxMatrix3()
        {
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="src">the matrix to copy</param>
        internal DmtxMatrix3(DmtxMatrix3 src)
        {
            _data = new[,] { { src[0, 0], src[0, 1], src[0, 2] }, { src[1, 0], src[1, 1], src[1, 2] }, { src[2, 0], src[2, 1], src[2, 2] } };
        }
        #endregion

        #region internal Static Methods
        /// <summary>
        /// creates a 3x3 identitiy matrix:<para />
        /// 1 0 0<para />
        /// 0 1 0<para />
        /// 0 0 1
        /// </summary>
        internal static DmtxMatrix3 Identity()
        {
            return Translate(0, 0);
        }

        /// <summary>
        /// generates a 3x3 translate transformation matrix
        /// 1 0 0<para />
        /// 0 1 0<para />
        /// tx ty 1
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        internal static DmtxMatrix3 Translate(double tx, double ty)
        {
            DmtxMatrix3 result = new DmtxMatrix3 {_data = new[,] {{1.0, 0.0, 0.0}, {0.0, 1.0, 0.0}, {tx, ty, 1.0}}};
            return result;
        }

        /// <summary>
        /// generates a 3x3 rotate transformation matrix
        /// cos(angle) sin(angle) 0<para />
        /// -sin(angle) cos(angle) 0<para />
        /// 0 0 1
        /// </summary>
        /// <param name="angle"></param>
        internal static DmtxMatrix3 Rotate(double angle)
        {
            DmtxMatrix3 result = new DmtxMatrix3 { _data = new[,] { 
                {Math.Cos(angle), Math.Sin(angle), 0.0}, 
                {-Math.Sin(angle), Math.Cos(angle), 0.0}, {0, 0, 1.0} }};
            return result;
        }

        /// <summary>
        /// generates a 3x3 scale transformation matrix
        /// sx 0 0<para />
        /// 0 sy 0<para />
        /// 0 0 1
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        /// <returns></returns>
        internal static DmtxMatrix3 Scale(double sx, double sy)
        {
            DmtxMatrix3 result = new DmtxMatrix3 {_data = new[,] {{sx, 0.0, 0.0}, {0.0, sy, 0.0}, {0, 0, 1.0}}};
            return result;
        }

        /// <summary>
        /// generates a 3x3 shear transformation matrix
        /// 0 shx 0<para />
        /// shy 0 0<para />
        /// 0 0 1
        /// </summary>
        /// <returns></returns>
        internal static DmtxMatrix3 Shear(double shx, double shy)
        {
            DmtxMatrix3 result = new DmtxMatrix3 {_data = new[,] {{1.0, shy, 0.0}, {shx, 1.0, 0.0}, {0, 0, 1.0}}};
            return result;
        }

        /// <summary>
        /// generates a 3x3 top line skew transformation matrix
        /// b1/b0 0 (b1-b0)/(sz * b0)<para />
        /// 0 sz/b0 0<para />
        /// 0 0 1
        /// </summary>
        /// <returns></returns>
        internal static DmtxMatrix3 LineSkewTop(double b0, double b1, double sz)
        {
            if (b0 < DmtxConstants.DmtxAlmostZero)
            {
                throw new ArgumentException("b0 must be larger than zero in top line skew transformation");
            }
            DmtxMatrix3 result = new DmtxMatrix3
                {_data = new[,] {{b1/b0, 0.0, (b1 - b0)/(sz*b0)}, {0.0, sz/b0, 0.0}, {0, 0, 1.0}}};
            return result;
        }


        /// <summary>
        /// generates a 3x3 top line skew transformation matrix (inverse)
        /// b0/b1 0 (b0-b1)/(sz * b1)<para />
        /// 0 b0/sz 0<para />
        /// 0 0 1
        /// </summary>
        /// <returns></returns>
        internal static DmtxMatrix3 LineSkewTopInv(double b0, double b1, double sz)
        {
            if (b1 < DmtxConstants.DmtxAlmostZero)
            {
                throw new ArgumentException("b1 must be larger than zero in top line skew transformation (inverse)");
            }
            DmtxMatrix3 result = new DmtxMatrix3
                                     {_data = new[,] {{b0/b1, 0.0, (b0 - b1)/(sz*b1)}, {0.0, b0/sz, 0.0}, {0, 0, 1.0}}};
            return result;
        }

        /// <summary>
        /// generates a 3x3 side line skew transformation matrix (inverse)
        /// sz/b0 0 0<para />
        /// 0 b1/b0 (b1-b0)/(sz*b0)<para />
        /// 0 0 1
        /// </summary>
        /// <returns></returns>
        internal static DmtxMatrix3 LineSkewSide(double b0, double b1, double sz)
        {
            if (b0 < DmtxConstants.DmtxAlmostZero)
            {
                throw new ArgumentException("b0 must be larger than zero in side line skew transformation (inverse)");
            }
            DmtxMatrix3 result = new DmtxMatrix3
                                     {_data = new[,] {{sz/b0, 0.0, 0.0}, {0.0, b1/b0, (b1 - b0)/(sz*b0)}, {0, 0, 1.0}}};
            return result;
        }

        /// <summary>
        /// generates a 3x3 side line skew transformation matrix (inverse)
        /// b0/sz 0 0<para />
        /// 0 b0/b1 (b0 - b1) / (sz * b1)<para />
        /// 0 0 1
        /// </summary>
        /// <returns></returns>
        internal static DmtxMatrix3 LineSkewSideInv(double b0, double b1, double sz)
        {
            if (b1 < DmtxConstants.DmtxAlmostZero)
            {
                throw new ArgumentException("b1 must be larger than zero in top line skew transformation (inverse)");
            }
            DmtxMatrix3 result = new DmtxMatrix3
                                     {_data = new[,] {{b0/sz, 0.0, 0.0}, {0.0, b0/b1, (b0 - b1)/(sz*b1)}, {0, 0, 1.0}}};
            return result;
        }

        public static DmtxMatrix3 operator *(DmtxMatrix3 m1, DmtxMatrix3 m2)
        {
            DmtxMatrix3 result = new DmtxMatrix3 {_data = new[,] {{0.0, 0, 0}, {0, 0, 0}, {0, 0, 0}}};

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        result[i, j] += m1[i, k] * m2[k, j];
                    }
                }
            }
            return result;
        }

        public static DmtxVector2 operator *(DmtxVector2 vector, DmtxMatrix3 matrix)
        {
            double w = Math.Abs(vector.X * matrix[0, 2] + vector.Y * matrix[1, 2] + matrix[2, 2]);
            if (w <= DmtxConstants.DmtxAlmostZero)
            {
                throw new ArgumentException("Multiplication of vector and matrix resulted in invalid result");
            }
            DmtxVector2 result = new DmtxVector2((vector.X*matrix[0,0] + vector.Y * matrix[1,0] + matrix[2,0])/w,
                (vector.X * matrix[0,1] + vector.Y * matrix[1,1] + matrix[2,1])/w);
            return result;
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\n{3}\t{4}\t{5}\n{6}\t{7}\t{8}\n", _data[0, 0], _data[0, 1], _data[0, 2], _data[1, 0], _data[1, 1], _data[1, 2], _data[2, 0], _data[2, 1], _data[2, 2]);
        }
        #endregion

        #region Properties
        internal double this[int i, int j]
        {
            get
            {
                return _data[i, j];
            }
            set
            {
                _data[i, j] = value;
            }
        }
        #endregion
    }
}

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
    internal class DmtxChannel
    {
        byte[] _encodedWords;

        internal byte[] Input { get; set; }

        internal DmtxScheme EncScheme { get; set; }

        internal DmtxChannelStatus Invalid { get; set; }

        internal int InputIndex { get; set; }

        internal int EncodedLength { get; set; }

        internal int CurrentLength { get; set; }

        internal int FirstCodeWord { get; set; }

        internal byte[] EncodedWords
        {
            get { return this._encodedWords ?? (this._encodedWords = new byte[1558]); }
        }
    }

    internal class DmtxChannelGroup
    {
        DmtxChannel[] _channels;

        internal DmtxChannel[] Channels
        {
            get
            {
                if (_channels == null)
                {
                    _channels = new DmtxChannel[6];
                    for (int i = 0; i < 6; i++)
                    {
                        _channels[i] = new DmtxChannel();
                    }
                }
                return _channels;
            }
        }
    }
}

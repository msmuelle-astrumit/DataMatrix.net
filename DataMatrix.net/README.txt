DataMatrix.Net - .net library for decoding DataMatrix codes.
by Michael Faschinger <michfasch@gmx.at>
https://sourceforge.net/projects/datamatrixnet


=======================
What is DataMatrix.net?
=======================

DataMatrix.net is a decoder and encoder library for DataMatrix codes. It 
started as a port of the libdmtx library (http://www.libdmtx.org) to C#/.net.

=========================
How to use DataMatrix.net
=========================

There are only two public classes in the library, which are:
DmtxImageEncoder and
DmtxImageDecoder
(both in namespace DataMatrix.net). The handling of these classes should be
rather intuitive.

==============================================================
What is the state of DataMatrix.net, what should be done next?
==============================================================
Hence libdtmx was already in a rather mature state when I ported it,
DataMatrix.net is in a rather mature state, too. Nonetheless, there is a list
of todos that should be tackled soon:
- improve the code documentation
- improve exception handling and error messages
- support additional 2D-barcodes (QRCode)
- support fetching and decoding images from a web cam or imaging device
- make the code more object-oriented. The original code was written in C and
(though it was rather object-oriented code) this can still be seen in the port
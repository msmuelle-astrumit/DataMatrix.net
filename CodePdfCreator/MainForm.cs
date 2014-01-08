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
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace DataMatrixCreator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            comboBoxCodeType.Items.Add(CodeType.Code128.ToString());
            comboBoxCodeType.Items.Add(CodeType.Code39.ToString());
            comboBoxCodeType.Items.Add(CodeType.Codabar.ToString());
            comboBoxCodeType.Items.Add(CodeType.Datamatrix.ToString());
            comboBoxCodeType.Items.Add(CodeType.EAN.ToString());
            comboBoxCodeType.Items.Add(CodeType.Inter25.ToString());
            comboBoxCodeType.Items.Add(CodeType.PDF417.ToString());
            comboBoxCodeType.Items.Add(CodeType.Postnet.ToString());
            comboBoxCodeType.SelectedIndex = 0;
        } 

        private void GenerateCodes(string code, string startVal, int count, float codeWidth, float codeHeight, int maxColCount, int maxRowCount, string fileName, CodeType codeType, CountType countType, float verticalMargin, float horizontalMargin, float borderTop, float borderLeft, float borderRight, float borderBottom)
        {
            Document doc = new Document();
            PdfWriter writer;
            try
            {
                if (File.Exists(fileName))
                {
                    DialogResult res = MessageBox.Show(string.Format("File {0} already exists. Do you want to overwrite the file?", fileName), "File exists", MessageBoxButtons.YesNo);
                    if (res != DialogResult.Yes)
                    {
                        return;
                    }
                }
                writer = PdfWriter.GetInstance(doc, new FileStream(fileName, FileMode.Create));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating file: " + ex.Message);
                return;
            }
            doc.Open();
            if (doc.PageSize.Width < codeWidth)
            {
                codeWidth = doc.PageSize.Width;
            }
            if (doc.PageSize.Height < codeHeight)
            {
                codeHeight = doc.PageSize.Height;
            }
            string codeToWrite = code;
            try
            {
                float top = borderTop;
                float left = borderLeft;
                int colIndex = 0;
                int rowIndex = 0;
                for (int codeCount = 0; codeCount < count; codeCount++)
                {
                    codeToWrite = GetCode(code, startVal, codeCount, countType);
                    AddCode(doc, codeToWrite, codeWidth, codeHeight, left, top, codeType, writer.DirectContent);
                    top += horizontalMargin + codeHeight;
                    if (++rowIndex > maxRowCount)
                    {
                        left = borderLeft;
                        top = borderTop;
                        colIndex = 0;
                        rowIndex = 0;
                        doc.NewPage();
                    }
                    if (top + codeHeight > doc.PageSize.Height - borderBottom)
                    {
                        top = borderTop;
                        rowIndex = 0;
                        colIndex++;
                        left = borderLeft + colIndex * (codeWidth + verticalMargin);
                        if (colIndex >= maxColCount || (left + codeWidth) > doc.PageSize.Width - borderRight)
                        {
                            left = borderLeft;
                            top = borderTop;
                            colIndex = 0;
                            rowIndex = 0;
                            doc.NewPage();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error generating code '{0}': {1}", codeToWrite, ex.Message));
            }
            finally
            {
                doc.Close();
            }
        }

        private string GetCode(string code, string startVal, int codeCount, CountType countType)
        {
            int replacementLength = code.LastIndexOf('#') - code.IndexOf('#') + 1;
            string textToReplace = new string('#', replacementLength);
            switch (countType)
            {
                case CountType.NumericDecimal:
                    int startValInt = int.Parse(startVal);
                    string result = code.Replace(textToReplace, (startValInt + codeCount).ToString(new string('0', replacementLength)));
                    return result;
                case CountType.NumericHexadecimal:
                    break;
                case CountType.Alphabetical:
                    break;
            }
            return code;
        }

        private void AddCode(Document doc, string text, float targetWidth, float targetHeight, float absoluteX, float absoluteY, CodeType codeType, PdfContentByte pcb)
        {
            Image image = GetImage(text, codeType, pcb);
            float widthRatio = image.Width / targetWidth;
            float heightRatio = image.Height / targetHeight;
            if (widthRatio > heightRatio)
            {
                image.ScalePercent(100.0F / widthRatio);
            }
            else
            {
                image.ScalePercent(100.0F / heightRatio);
            }
            image.SetAbsolutePosition(absoluteX, doc.PageSize.Height - absoluteY - image.ScaledHeight);
            doc.Add(image);
        }

        private Image GetImage(string text, CodeType codeType, PdfContentByte pcb)
        {
            switch (codeType)
            {
                case CodeType.Code128:
                    Barcode128 barcode128 = new Barcode128 {AltText = text, Code = text};
                    return barcode128.CreateImageWithBarcode(pcb, iTextSharp.text.Color.BLACK, iTextSharp.text.Color.BLACK);
                case CodeType.Code39:
                    Barcode39 barcode39 = new Barcode39 {Code = text, AltText = text};
                    return barcode39.CreateImageWithBarcode(pcb, iTextSharp.text.Color.BLACK, iTextSharp.text.Color.BLACK);
                case CodeType.Codabar:
                    BarcodeCodabar barcodeCodabar = new BarcodeCodabar {Code = text, AltText = text};
                    return barcodeCodabar.CreateImageWithBarcode(pcb, iTextSharp.text.Color.BLACK, iTextSharp.text.Color.BLACK);
                case CodeType.Datamatrix:
                    BarcodeDatamatrix barcodeDatamatrix = new BarcodeDatamatrix();
                    barcodeDatamatrix.Generate(text);
                    return barcodeDatamatrix.CreateImage();
                case CodeType.EAN:
                    BarcodeEAN barcodeEAN = new BarcodeEAN {CodeType = Barcode.EAN13, Code = text, AltText = text};
                    return barcodeEAN.CreateImageWithBarcode(pcb, iTextSharp.text.Color.BLACK, iTextSharp.text.Color.BLACK);
                case CodeType.Inter25:
                    BarcodeInter25 barcodeInter25 = new BarcodeInter25
                                                        {AltText = text, Code = text, GenerateChecksum = true};
                    return barcodeInter25.CreateImageWithBarcode(pcb, iTextSharp.text.Color.BLACK, iTextSharp.text.Color.BLACK);
                case CodeType.PDF417:
                    BarcodePDF417 barcodePDF417 = new BarcodePDF417();
                    barcodePDF417.SetText(text);
                    return barcodePDF417.GetImage();
                case CodeType.Postnet:
                    BarcodePostnet barcodePostnet = new BarcodePostnet {AltText = text, Code = text};
                    return barcodePostnet.CreateImageWithBarcode(pcb, iTextSharp.text.Color.BLACK, iTextSharp.text.Color.BLACK);
            }
            return null;
        }

        private enum CodeType
        {
            Code128,
            Code39,
            Codabar,
            Datamatrix,
            EAN,
            Inter25,
            PDF417,
            Postnet
        }

        private enum CountType
        {
            NumericDecimal,
            NumericHexadecimal,
            Alphabetical
        }

        private void ButtonGenerateClick(object sender, EventArgs e)
        {
            try
            {
                GenerateCodes(textBoxStaticCode.Text,
                    textBoxVariableCodeStart.Text,
                    int.Parse(textBoxNumCodes.Text),
                    float.Parse(textBoxCodeWidth.Text),
                    float.Parse(textBoxCodeHeight.Text),
                    int.Parse(textBoxColumnCount.Text),
                    int.Parse(textBoxRowCount.Text),
                    textBoxFilename.Text,
                    (CodeType)Enum.Parse(typeof(CodeType), comboBoxCodeType.SelectedItem.ToString()),
                    radioButtonDecimal.Checked ? CountType.NumericDecimal : radioButtonHexadecimal.Checked ? CountType.NumericHexadecimal : CountType.Alphabetical,
                    float.Parse(textBoxVerticalMargin.Text),
                    float.Parse(textBoxHorizontalMargin.Text),
                    float.Parse(textBoxTopBorder.Text),
                    float.Parse(textBoxLeftBorder.Text),
                    float.Parse(textBoxRightBorder.Text),
                    float.Parse(textBoxBottomBorder.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error generating output file: {0} at {1}", ex.Message, ex.StackTrace));
                return;
            }
            DialogResult res = MessageBox.Show("Output generated successfully. Do you want to view the file now?", "Success", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                try
                {
                    System.Diagnostics.Process.Start(textBoxFilename.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Error opening file: {0} at {1}", ex.Message, ex.StackTrace));
                }
            }
        }
    }
}

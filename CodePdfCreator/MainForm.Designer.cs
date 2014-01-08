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

namespace DataMatrixCreator
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxCode = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxNumCodes = new System.Windows.Forms.TextBox();
            this.labelCountType = new System.Windows.Forms.Label();
            this.radioButtonDecimal = new System.Windows.Forms.RadioButton();
            this.radioButtonHexadecimal = new System.Windows.Forms.RadioButton();
            this.radioButtonAlphabetic = new System.Windows.Forms.RadioButton();
            this.labelCodeType = new System.Windows.Forms.Label();
            this.labelVarCode = new System.Windows.Forms.Label();
            this.labelCode = new System.Windows.Forms.Label();
            this.textBoxStaticCode = new System.Windows.Forms.TextBox();
            this.textBoxVariableCodeStart = new System.Windows.Forms.TextBox();
            this.comboBoxCodeType = new System.Windows.Forms.ComboBox();
            this.groupBoxLayout = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxRowCount = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxColumnCount = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxLeftBorder = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxRightBorder = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxCodeHeight = new System.Windows.Forms.TextBox();
            this.textBoxVerticalMargin = new System.Windows.Forms.TextBox();
            this.textBoxHorizontalMargin = new System.Windows.Forms.TextBox();
            this.textBoxTopBorder = new System.Windows.Forms.TextBox();
            this.textBoxBottomBorder = new System.Windows.Forms.TextBox();
            this.textBoxCodeWidth = new System.Windows.Forms.TextBox();
            this.buttonGenerate = new System.Windows.Forms.Button();
            this.labelFilename = new System.Windows.Forms.Label();
            this.textBoxFilename = new System.Windows.Forms.TextBox();
            this.groupBoxCode.SuspendLayout();
            this.groupBoxLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxCode
            // 
            this.groupBoxCode.Controls.Add(this.label11);
            this.groupBoxCode.Controls.Add(this.textBoxNumCodes);
            this.groupBoxCode.Controls.Add(this.labelCountType);
            this.groupBoxCode.Controls.Add(this.radioButtonDecimal);
            this.groupBoxCode.Controls.Add(this.radioButtonHexadecimal);
            this.groupBoxCode.Controls.Add(this.radioButtonAlphabetic);
            this.groupBoxCode.Controls.Add(this.labelCodeType);
            this.groupBoxCode.Controls.Add(this.labelVarCode);
            this.groupBoxCode.Controls.Add(this.labelCode);
            this.groupBoxCode.Controls.Add(this.textBoxStaticCode);
            this.groupBoxCode.Controls.Add(this.textBoxVariableCodeStart);
            this.groupBoxCode.Controls.Add(this.comboBoxCodeType);
            this.groupBoxCode.Location = new System.Drawing.Point(12, 12);
            this.groupBoxCode.Name = "groupBoxCode";
            this.groupBoxCode.Size = new System.Drawing.Size(219, 292);
            this.groupBoxCode.TabIndex = 0;
            this.groupBoxCode.TabStop = false;
            this.groupBoxCode.Text = "Code";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 250);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(88, 13);
            this.label11.TabIndex = 21;
            this.label11.Text = "Number of codes";
            // 
            // textBoxNumCodes
            // 
            this.textBoxNumCodes.Location = new System.Drawing.Point(113, 247);
            this.textBoxNumCodes.Name = "textBoxNumCodes";
            this.textBoxNumCodes.Size = new System.Drawing.Size(100, 20);
            this.textBoxNumCodes.TabIndex = 4;
            this.textBoxNumCodes.Text = "100";
            this.textBoxNumCodes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelCountType
            // 
            this.labelCountType.AutoSize = true;
            this.labelCountType.Location = new System.Drawing.Point(11, 143);
            this.labelCountType.Name = "labelCountType";
            this.labelCountType.Size = new System.Drawing.Size(116, 13);
            this.labelCountType.TabIndex = 7;
            this.labelCountType.Text = "Variable code part type";
            // 
            // radioButtonDecimal
            // 
            this.radioButtonDecimal.AutoSize = true;
            this.radioButtonDecimal.Checked = true;
            this.radioButtonDecimal.Location = new System.Drawing.Point(131, 119);
            this.radioButtonDecimal.Name = "radioButtonDecimal";
            this.radioButtonDecimal.Size = new System.Drawing.Size(63, 17);
            this.radioButtonDecimal.TabIndex = 2;
            this.radioButtonDecimal.TabStop = true;
            this.radioButtonDecimal.Text = "Decimal";
            this.radioButtonDecimal.UseVisualStyleBackColor = true;
            // 
            // radioButtonHexadecimal
            // 
            this.radioButtonHexadecimal.AutoSize = true;
            this.radioButtonHexadecimal.Location = new System.Drawing.Point(131, 142);
            this.radioButtonHexadecimal.Name = "radioButtonHexadecimal";
            this.radioButtonHexadecimal.Size = new System.Drawing.Size(86, 17);
            this.radioButtonHexadecimal.TabIndex = 2;
            this.radioButtonHexadecimal.Text = "Hexadecimal";
            this.radioButtonHexadecimal.UseVisualStyleBackColor = true;
            // 
            // radioButtonAlphabetic
            // 
            this.radioButtonAlphabetic.AutoSize = true;
            this.radioButtonAlphabetic.Location = new System.Drawing.Point(131, 165);
            this.radioButtonAlphabetic.Name = "radioButtonAlphabetic";
            this.radioButtonAlphabetic.Size = new System.Drawing.Size(75, 17);
            this.radioButtonAlphabetic.TabIndex = 2;
            this.radioButtonAlphabetic.Text = "Alphabetic";
            this.radioButtonAlphabetic.UseVisualStyleBackColor = true;
            // 
            // labelCodeType
            // 
            this.labelCodeType.AutoSize = true;
            this.labelCodeType.Location = new System.Drawing.Point(6, 190);
            this.labelCodeType.Name = "labelCodeType";
            this.labelCodeType.Size = new System.Drawing.Size(62, 13);
            this.labelCodeType.TabIndex = 6;
            this.labelCodeType.Text = "Code Type:";
            // 
            // labelVarCode
            // 
            this.labelVarCode.AutoSize = true;
            this.labelVarCode.Location = new System.Drawing.Point(9, 67);
            this.labelVarCode.Name = "labelVarCode";
            this.labelVarCode.Size = new System.Drawing.Size(148, 13);
            this.labelVarCode.TabIndex = 5;
            this.labelVarCode.Text = "Variable code part start value:";
            // 
            // labelCode
            // 
            this.labelCode.AutoSize = true;
            this.labelCode.Location = new System.Drawing.Point(6, 16);
            this.labelCode.Name = "labelCode";
            this.labelCode.Size = new System.Drawing.Size(163, 13);
            this.labelCode.TabIndex = 4;
            this.labelCode.Text = "Static code part (placeholder \'#\'):";
            // 
            // textBoxStaticCode
            // 
            this.textBoxStaticCode.Location = new System.Drawing.Point(9, 31);
            this.textBoxStaticCode.Name = "textBoxStaticCode";
            this.textBoxStaticCode.Size = new System.Drawing.Size(204, 20);
            this.textBoxStaticCode.TabIndex = 0;
            this.textBoxStaticCode.Text = "0123456789####";
            this.textBoxStaticCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxVariableCodeStart
            // 
            this.textBoxVariableCodeStart.Location = new System.Drawing.Point(9, 82);
            this.textBoxVariableCodeStart.Name = "textBoxVariableCodeStart";
            this.textBoxVariableCodeStart.Size = new System.Drawing.Size(204, 20);
            this.textBoxVariableCodeStart.TabIndex = 1;
            this.textBoxVariableCodeStart.Text = "0100";
            this.textBoxVariableCodeStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // comboBoxCodeType
            // 
            this.comboBoxCodeType.FormattingEnabled = true;
            this.comboBoxCodeType.Location = new System.Drawing.Point(9, 207);
            this.comboBoxCodeType.Name = "comboBoxCodeType";
            this.comboBoxCodeType.Size = new System.Drawing.Size(204, 21);
            this.comboBoxCodeType.TabIndex = 3;
            // 
            // groupBoxLayout
            // 
            this.groupBoxLayout.Controls.Add(this.label10);
            this.groupBoxLayout.Controls.Add(this.textBoxRowCount);
            this.groupBoxLayout.Controls.Add(this.label9);
            this.groupBoxLayout.Controls.Add(this.textBoxColumnCount);
            this.groupBoxLayout.Controls.Add(this.label8);
            this.groupBoxLayout.Controls.Add(this.textBoxLeftBorder);
            this.groupBoxLayout.Controls.Add(this.label7);
            this.groupBoxLayout.Controls.Add(this.textBoxRightBorder);
            this.groupBoxLayout.Controls.Add(this.label6);
            this.groupBoxLayout.Controls.Add(this.label5);
            this.groupBoxLayout.Controls.Add(this.label4);
            this.groupBoxLayout.Controls.Add(this.label3);
            this.groupBoxLayout.Controls.Add(this.label2);
            this.groupBoxLayout.Controls.Add(this.label1);
            this.groupBoxLayout.Controls.Add(this.textBoxCodeHeight);
            this.groupBoxLayout.Controls.Add(this.textBoxVerticalMargin);
            this.groupBoxLayout.Controls.Add(this.textBoxHorizontalMargin);
            this.groupBoxLayout.Controls.Add(this.textBoxTopBorder);
            this.groupBoxLayout.Controls.Add(this.textBoxBottomBorder);
            this.groupBoxLayout.Controls.Add(this.textBoxCodeWidth);
            this.groupBoxLayout.Location = new System.Drawing.Point(237, 12);
            this.groupBoxLayout.Name = "groupBoxLayout";
            this.groupBoxLayout.Size = new System.Drawing.Size(223, 292);
            this.groupBoxLayout.TabIndex = 1;
            this.groupBoxLayout.TabStop = false;
            this.groupBoxLayout.Text = "Layout";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 154);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "Rows per page";
            // 
            // textBoxRowCount
            // 
            this.textBoxRowCount.Location = new System.Drawing.Point(116, 151);
            this.textBoxRowCount.Name = "textBoxRowCount";
            this.textBoxRowCount.Size = new System.Drawing.Size(100, 20);
            this.textBoxRowCount.TabIndex = 15;
            this.textBoxRowCount.Text = "20";
            this.textBoxRowCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 129);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(92, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Columns per page";
            // 
            // textBoxColumnCount
            // 
            this.textBoxColumnCount.Location = new System.Drawing.Point(116, 126);
            this.textBoxColumnCount.Name = "textBoxColumnCount";
            this.textBoxColumnCount.Size = new System.Drawing.Size(100, 20);
            this.textBoxColumnCount.TabIndex = 13;
            this.textBoxColumnCount.Text = "3";
            this.textBoxColumnCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 231);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Left page border";
            // 
            // textBoxLeftBorder
            // 
            this.textBoxLeftBorder.Location = new System.Drawing.Point(116, 228);
            this.textBoxLeftBorder.Name = "textBoxLeftBorder";
            this.textBoxLeftBorder.Size = new System.Drawing.Size(100, 20);
            this.textBoxLeftBorder.TabIndex = 22;
            this.textBoxLeftBorder.Text = "60";
            this.textBoxLeftBorder.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 257);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Right page border";
            // 
            // textBoxRightBorder
            // 
            this.textBoxRightBorder.Location = new System.Drawing.Point(116, 254);
            this.textBoxRightBorder.Name = "textBoxRightBorder";
            this.textBoxRightBorder.Size = new System.Drawing.Size(100, 20);
            this.textBoxRightBorder.TabIndex = 24;
            this.textBoxRightBorder.Text = "60";
            this.textBoxRightBorder.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 206);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Bottom page border";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 179);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Top page border";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Horizontal margin";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Vertical margin";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Code height";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Code width";
            // 
            // textBoxCodeHeight
            // 
            this.textBoxCodeHeight.Location = new System.Drawing.Point(116, 48);
            this.textBoxCodeHeight.Name = "textBoxCodeHeight";
            this.textBoxCodeHeight.Size = new System.Drawing.Size(100, 20);
            this.textBoxCodeHeight.TabIndex = 8;
            this.textBoxCodeHeight.Text = "30";
            this.textBoxCodeHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxVerticalMargin
            // 
            this.textBoxVerticalMargin.Location = new System.Drawing.Point(116, 74);
            this.textBoxVerticalMargin.Name = "textBoxVerticalMargin";
            this.textBoxVerticalMargin.Size = new System.Drawing.Size(100, 20);
            this.textBoxVerticalMargin.TabIndex = 9;
            this.textBoxVerticalMargin.Text = "10";
            this.textBoxVerticalMargin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxHorizontalMargin
            // 
            this.textBoxHorizontalMargin.Location = new System.Drawing.Point(116, 100);
            this.textBoxHorizontalMargin.Name = "textBoxHorizontalMargin";
            this.textBoxHorizontalMargin.Size = new System.Drawing.Size(100, 20);
            this.textBoxHorizontalMargin.TabIndex = 11;
            this.textBoxHorizontalMargin.Text = "20";
            this.textBoxHorizontalMargin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxTopBorder
            // 
            this.textBoxTopBorder.Location = new System.Drawing.Point(116, 176);
            this.textBoxTopBorder.Name = "textBoxTopBorder";
            this.textBoxTopBorder.Size = new System.Drawing.Size(100, 20);
            this.textBoxTopBorder.TabIndex = 17;
            this.textBoxTopBorder.Text = "60";
            this.textBoxTopBorder.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxBottomBorder
            // 
            this.textBoxBottomBorder.Location = new System.Drawing.Point(116, 203);
            this.textBoxBottomBorder.Name = "textBoxBottomBorder";
            this.textBoxBottomBorder.Size = new System.Drawing.Size(100, 20);
            this.textBoxBottomBorder.TabIndex = 20;
            this.textBoxBottomBorder.Text = "60";
            this.textBoxBottomBorder.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxCodeWidth
            // 
            this.textBoxCodeWidth.Location = new System.Drawing.Point(116, 24);
            this.textBoxCodeWidth.Name = "textBoxCodeWidth";
            this.textBoxCodeWidth.Size = new System.Drawing.Size(100, 20);
            this.textBoxCodeWidth.TabIndex = 6;
            this.textBoxCodeWidth.Text = "90";
            this.textBoxCodeWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // buttonGenerate
            // 
            this.buttonGenerate.Location = new System.Drawing.Point(237, 310);
            this.buttonGenerate.Name = "buttonGenerate";
            this.buttonGenerate.Size = new System.Drawing.Size(223, 23);
            this.buttonGenerate.TabIndex = 26;
            this.buttonGenerate.Text = "Generate Output";
            this.buttonGenerate.UseVisualStyleBackColor = true;
            this.buttonGenerate.Click += new System.EventHandler(this.ButtonGenerateClick);
            // 
            // labelFilename
            // 
            this.labelFilename.AutoSize = true;
            this.labelFilename.Location = new System.Drawing.Point(12, 315);
            this.labelFilename.Name = "labelFilename";
            this.labelFilename.Size = new System.Drawing.Size(49, 13);
            this.labelFilename.TabIndex = 22;
            this.labelFilename.Text = "Filename";
            // 
            // textBoxFilename
            // 
            this.textBoxFilename.Location = new System.Drawing.Point(83, 312);
            this.textBoxFilename.Name = "textBoxFilename";
            this.textBoxFilename.Size = new System.Drawing.Size(148, 20);
            this.textBoxFilename.TabIndex = 25;
            this.textBoxFilename.Text = "output.pdf";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(466, 345);
            this.Controls.Add(this.textBoxFilename);
            this.Controls.Add(this.buttonGenerate);
            this.Controls.Add(this.labelFilename);
            this.Controls.Add(this.groupBoxLayout);
            this.Controls.Add(this.groupBoxCode);
            this.Name = "MainForm";
            this.Text = "Code PDF Generator";
            this.groupBoxCode.ResumeLayout(false);
            this.groupBoxCode.PerformLayout();
            this.groupBoxLayout.ResumeLayout(false);
            this.groupBoxLayout.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxCode;
        private System.Windows.Forms.Label labelCodeType;
        private System.Windows.Forms.Label labelVarCode;
        private System.Windows.Forms.Label labelCode;
        private System.Windows.Forms.TextBox textBoxStaticCode;
        private System.Windows.Forms.TextBox textBoxVariableCodeStart;
        private System.Windows.Forms.ComboBox comboBoxCodeType;
        private System.Windows.Forms.Label labelCountType;
        private System.Windows.Forms.RadioButton radioButtonDecimal;
        private System.Windows.Forms.RadioButton radioButtonHexadecimal;
        private System.Windows.Forms.RadioButton radioButtonAlphabetic;
        private System.Windows.Forms.GroupBox groupBoxLayout;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxCodeHeight;
        private System.Windows.Forms.TextBox textBoxVerticalMargin;
        private System.Windows.Forms.TextBox textBoxHorizontalMargin;
        private System.Windows.Forms.TextBox textBoxTopBorder;
        private System.Windows.Forms.TextBox textBoxBottomBorder;
        private System.Windows.Forms.TextBox textBoxCodeWidth;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxLeftBorder;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxRightBorder;
        private System.Windows.Forms.Button buttonGenerate;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxNumCodes;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxRowCount;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxColumnCount;
        private System.Windows.Forms.Label labelFilename;
        private System.Windows.Forms.TextBox textBoxFilename;
    }
}


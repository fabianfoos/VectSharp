﻿/*
    VectSharp - A light library for C# vector graphics.
    Copyright (C) 2020-2022 Giorgio Bianchini

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, version 3.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using VectSharp.Filters;

namespace VectSharp.SVG
{
    internal static class MatrixUtils
    {
        public static double[] Multiply(double[,] matrix, double[] vector)
        {
            double[] tbr = new double[2];

            tbr[0] = matrix[0, 0] * vector[0] + matrix[0, 1] * vector[1] + matrix[0, 2];
            tbr[1] = matrix[1, 0] * vector[0] + matrix[1, 1] * vector[1] + matrix[1, 2];

            return tbr;
        }

        public static Point Multiply(double[,] matrix, Point vector)
        {
            double[] tbr = new double[2];

            tbr[0] = matrix[0, 0] * vector.X + matrix[0, 1] * vector.Y + matrix[0, 2];
            tbr[1] = matrix[1, 0] * vector.X + matrix[1, 1] * vector.Y + matrix[1, 2];

            return new Point(tbr[0], tbr[1]);
        }

        public static double[,] Multiply(double[,] matrix1, double[,] matrix2)
        {
            double[,] tbr = new double[3, 3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        tbr[i, j] += matrix1[i, k] * matrix2[k, j];
                    }
                }
            }

            return tbr;
        }

        public static double[,] Rotate(double[,] matrix, double angle)
        {
            double[,] rotationMatrix = new double[3, 3];
            rotationMatrix[0, 0] = Math.Cos(angle);
            rotationMatrix[0, 1] = -Math.Sin(angle);
            rotationMatrix[1, 0] = Math.Sin(angle);
            rotationMatrix[1, 1] = Math.Cos(angle);
            rotationMatrix[2, 2] = 1;

            return Multiply(matrix, rotationMatrix);
        }

        public static double[,] Translate(double[,] matrix, double x, double y)
        {
            double[,] translationMatrix = new double[3, 3];
            translationMatrix[0, 0] = 1;
            translationMatrix[0, 2] = x;
            translationMatrix[1, 1] = 1;
            translationMatrix[1, 2] = y;
            translationMatrix[2, 2] = 1;

            return Multiply(matrix, translationMatrix);
        }

        public static double[,] Scale(double[,] matrix, double scaleX, double scaleY)
        {
            double[,] scaleMatrix = new double[3, 3];
            scaleMatrix[0, 0] = scaleX;
            scaleMatrix[1, 1] = scaleY;
            scaleMatrix[2, 2] = 1;

            return Multiply(matrix, scaleMatrix);
        }

        public static double[,] Identity = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };

        public static double[,] Invert(double[,] m)
        {
            double[,] tbr = new double[3, 3];

            tbr[0, 0] = (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1]) / (m[0, 0] * m[1, 1] * m[2, 2] - m[0, 0] * m[1, 2] * m[2, 1] - m[1, 0] * m[0, 1] * m[2, 2] + m[2, 0] * m[0, 1] * m[1, 2] + m[1, 0] * m[0, 2] * m[2, 1] - m[2, 0] * m[0, 2] * m[1, 1]);
            tbr[0, 1] = -(m[0, 1] * m[2, 2] - m[0, 2] * m[2, 1]) / (m[0, 0] * m[1, 1] * m[2, 2] - m[0, 0] * m[1, 2] * m[2, 1] - m[1, 0] * m[0, 1] * m[2, 2] + m[2, 0] * m[0, 1] * m[1, 2] + m[1, 0] * m[0, 2] * m[2, 1] - m[2, 0] * m[0, 2] * m[1, 1]);
            tbr[0, 2] = (m[0, 1] * m[1, 2] - m[0, 2] * m[1, 1]) / (m[0, 0] * m[1, 1] * m[2, 2] - m[0, 0] * m[1, 2] * m[2, 1] - m[1, 0] * m[0, 1] * m[2, 2] + m[2, 0] * m[0, 1] * m[1, 2] + m[1, 0] * m[0, 2] * m[2, 1] - m[2, 0] * m[0, 2] * m[1, 1]);
            tbr[1, 0] = -(m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0]) / (m[0, 0] * m[1, 1] * m[2, 2] - m[0, 0] * m[1, 2] * m[2, 1] - m[1, 0] * m[0, 1] * m[2, 2] + m[2, 0] * m[0, 1] * m[1, 2] + m[1, 0] * m[0, 2] * m[2, 1] - m[2, 0] * m[0, 2] * m[1, 1]);
            tbr[1, 1] = (m[0, 0] * m[2, 2] - m[0, 2] * m[2, 0]) / (m[0, 0] * m[1, 1] * m[2, 2] - m[0, 0] * m[1, 2] * m[2, 1] - m[1, 0] * m[0, 1] * m[2, 2] + m[2, 0] * m[0, 1] * m[1, 2] + m[1, 0] * m[0, 2] * m[2, 1] - m[2, 0] * m[0, 2] * m[1, 1]);
            tbr[1, 2] = -(m[0, 0] * m[1, 2] - m[0, 2] * m[1, 0]) / (m[0, 0] * m[1, 1] * m[2, 2] - m[0, 0] * m[1, 2] * m[2, 1] - m[1, 0] * m[0, 1] * m[2, 2] + m[2, 0] * m[0, 1] * m[1, 2] + m[1, 0] * m[0, 2] * m[2, 1] - m[2, 0] * m[0, 2] * m[1, 1]);
            tbr[2, 0] = (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]) / (m[0, 0] * m[1, 1] * m[2, 2] - m[0, 0] * m[1, 2] * m[2, 1] - m[1, 0] * m[0, 1] * m[2, 2] + m[2, 0] * m[0, 1] * m[1, 2] + m[1, 0] * m[0, 2] * m[2, 1] - m[2, 0] * m[0, 2] * m[1, 1]);
            tbr[2, 1] = -(m[0, 0] * m[2, 1] - m[0, 1] * m[2, 0]) / (m[0, 0] * m[1, 1] * m[2, 2] - m[0, 0] * m[1, 2] * m[2, 1] - m[1, 0] * m[0, 1] * m[2, 2] + m[2, 0] * m[0, 1] * m[1, 2] + m[1, 0] * m[0, 2] * m[2, 1] - m[2, 0] * m[0, 2] * m[1, 1]);
            tbr[2, 2] = (m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0]) / (m[0, 0] * m[1, 1] * m[2, 2] - m[0, 0] * m[1, 2] * m[2, 1] - m[1, 0] * m[0, 1] * m[2, 2] + m[2, 0] * m[0, 1] * m[1, 2] + m[1, 0] * m[0, 2] * m[2, 1] - m[2, 0] * m[0, 2] * m[1, 1]);

            return tbr;
        }
    }

    internal class SVGFigure
    {
        public Point StartPoint { get; set; }
        public Point CurrentPoint { get; set; }

        public string Data { get; set; }
        public int PointCount { get; set; } = 0;
    }

    internal class SVGPathObject
    {
        public List<SVGFigure> Figures { get; set; } = new List<SVGFigure>();
    }


    internal class SVGContext : IGraphicsContext
    {
        public Dictionary<string, FontFamily> UsedFontFamilies;
        public Dictionary<string, HashSet<char>> UsedChars;


        public const string SVGNamespace = "http://www.w3.org/2000/svg";

        public double Width { get; }

        public double Height { get; }

        public Font Font { get; set; }
        public TextBaselines TextBaseline { get; set; }

        public Brush FillStyle { get; private set; }

        public Brush StrokeStyle { get; private set; }

        public double LineWidth { get; set; }
        public LineCaps LineCap { private get; set; }
        public LineJoins LineJoin { private get; set; }

        private LineDash _lineDash;

        private SVGPathObject currentPath;
        private SVGFigure currentFigure;

        public XmlDocument Document;
        private XmlElement currentElement;

        public string Tag { get; set; }

        private double[,] _transform;
        private Stack<double[,]> states;

        private string _currClipPath;
        private Stack<string> clipPaths;

        private bool TextToPaths = false;
        private SVGContextInterpreter.TextOptions TextOption = SVGContextInterpreter.TextOptions.SubsetFonts;

        private Dictionary<string, string> linkDestinations;

        XmlElement definitions;
        Dictionary<Brush, string> gradients;

        private SVGContextInterpreter.FilterOption FilterOption;

        private bool UseStyles;
        internal Dictionary<string, Dictionary<string, string>> Styles;
        XmlElement StyleElement;

        private string GetClass(Dictionary<string, string> style)
        {
            foreach (KeyValuePair<string, Dictionary<string, string>> styleClass in Styles)
            {
                if (styleClass.Value.Count == style.Count)
                {
                    bool mismatch = false;

                    foreach (KeyValuePair<string, string> styleItem in style)
                    {
                        if (!styleClass.Value.TryGetValue(styleItem.Key, out string value))
                        {
                            mismatch = true;
                            break;
                        }
                        else if (value != styleItem.Value)
                        {
                            mismatch = true;
                            break;
                        }
                    }

                    if (!mismatch)
                    {
                        return styleClass.Key;
                    }
                }
            }

            string tbr = "class" + (Styles.Count + 1).ToString();

            Styles.Add(tbr, style);

            return tbr;
        }

        public SVGContext(double width, double height, bool textToPaths, SVGContextInterpreter.TextOptions textOption, Dictionary<string, string> linkDestinations, SVGContextInterpreter.FilterOption filterOption, bool useStyles)
        {
            this.linkDestinations = linkDestinations;

            this.Width = width;
            this.Height = height;

            currentPath = new SVGPathObject();
            currentFigure = new SVGFigure();

            StrokeStyle = Colour.FromRgba(0, 0, 0, 0);
            FillStyle = Colour.FromRgb(0, 0, 0);
            LineWidth = 1;

            LineCap = LineCaps.Butt;
            LineJoin = LineJoins.Miter;
            _lineDash = new LineDash(0, 0, 0);

            Font = new Font(FontFamily.ResolveFontFamily(FontFamily.StandardFontFamilies.Helvetica), 12);

            TextBaseline = TextBaselines.Top;

            _transform = new double[3, 3];

            _transform[0, 0] = 1;
            _transform[1, 1] = 1;
            _transform[2, 2] = 1;

            states = new Stack<double[,]>();

            clipPaths = new Stack<string>();
            clipPaths.Push(null);
            _currClipPath = null;

            UsedFontFamilies = new Dictionary<string, FontFamily>();
            UsedChars = new Dictionary<string, HashSet<char>>();

            Document = new XmlDocument();

            Document.InsertBefore(Document.CreateXmlDeclaration("1.0", "UTF-8", null), Document.DocumentElement);

            currentElement = Document.CreateElement(null, "svg", SVGNamespace);
            currentElement.SetAttribute("xmlns:xlink", "http://www.w3.org/1999/xlink");
            currentElement.SetAttribute("viewBox", "0 0 " + width.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + height.ToString(System.Globalization.CultureInfo.InvariantCulture));
            currentElement.SetAttribute("version", "1.1");
            Document.AppendChild(currentElement);

            definitions = Document.CreateElement("defs", SVGNamespace);
            gradients = new Dictionary<Brush, string>();
            currentElement.AppendChild(definitions);

            this.UseStyles = useStyles;
            StyleElement = Document.CreateElement("style", SVGNamespace);
            StyleElement.InnerText = "\t";
            currentElement.AppendChild(StyleElement);
            Styles = new Dictionary<string, Dictionary<string, string>>();

            this.TextToPaths = textToPaths;
            this.TextOption = textOption;
            this.FilterOption = filterOption;
        }


        private void PathText(string text, double x, double y)
        {
            GraphicsPath textPath = new GraphicsPath().AddText(x, y, text, Font, TextBaseline);

            for (int j = 0; j < textPath.Segments.Count; j++)
            {
                switch (textPath.Segments[j].Type)
                {
                    case VectSharp.SegmentType.Move:
                        this.MoveTo(textPath.Segments[j].Point.X, textPath.Segments[j].Point.Y);
                        break;
                    case VectSharp.SegmentType.Line:
                        this.LineTo(textPath.Segments[j].Point.X, textPath.Segments[j].Point.Y);
                        break;
                    case VectSharp.SegmentType.CubicBezier:
                        this.CubicBezierTo(textPath.Segments[j].Points[0].X, textPath.Segments[j].Points[0].Y, textPath.Segments[j].Points[1].X, textPath.Segments[j].Points[1].Y, textPath.Segments[j].Points[2].X, textPath.Segments[j].Points[2].Y);
                        break;
                    case VectSharp.SegmentType.Close:
                        this.Close();
                        break;
                }
            }
        }

        public void Close()
        {
            if (currentFigure.PointCount > 0)
            {
                currentFigure.Data += "Z ";
                currentPath.Figures.Add(currentFigure);

                currentFigure = new SVGFigure();
            }
        }

        public void CubicBezierTo(double p1X, double p1Y, double p2X, double p2Y, double p3X, double p3Y)
        {
            if (currentFigure.PointCount == 0)
            {
                currentFigure.StartPoint = new Point(p1X, p1Y);
            }

            currentFigure.CurrentPoint = new Point(p3X, p3Y);
            currentFigure.Data += "C " + p1X.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + p1Y.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", " +
                p2X.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + p2Y.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", " +
                p3X.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + p3Y.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
            currentFigure.PointCount += 3;
        }

        public void Fill()
        {
            if (currentFigure.PointCount > 0)
            {
                currentPath.Figures.Add(currentFigure);
            }

            XmlElement currElement = currentElement;

            if (!string.IsNullOrEmpty(_currClipPath))
            {
                currentElement = Document.CreateElement("g", SVGNamespace);
                currentElement.SetAttribute("clip-path", _currClipPath);
                currElement.AppendChild(currentElement);
            }

            XmlElement path = Document.CreateElement("path", SVGNamespace);
            path.SetAttribute("d", currentPath.Figures.Aggregate("", (a, b) => a + b.Data));

            string gradientName = null;

            {
                if (FillStyle is LinearGradientBrush linearGradient)
                {
                    if (!gradients.TryGetValue(linearGradient, out gradientName))
                    {
                        gradientName = "gradient" + (gradients.Count + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);

                        XmlElement gradientElement = linearGradient.ToLinearGradient(Document, gradientName);
                        this.definitions.AppendChild(gradientElement);

                        gradients.Add(linearGradient, gradientName);
                    }
                }
                else if (FillStyle is RadialGradientBrush radialGradient)
                {
                    if (!gradients.TryGetValue(radialGradient, out gradientName))
                    {
                        gradientName = "gradient" + (gradients.Count + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);

                        XmlElement gradientElement = radialGradient.ToRadialGradient(Document, gradientName);
                        this.definitions.AppendChild(gradientElement);

                        gradients.Add(radialGradient, gradientName);
                    }
                }
            }

            if (!UseStyles)
            {
                path.SetAttribute("stroke", "none");

                if (FillStyle is SolidColourBrush solid)
                {
                    path.SetAttribute("fill", solid.Colour.ToCSSString(false));
                    path.SetAttribute("fill-opacity", solid.A.ToString(System.Globalization.CultureInfo.InvariantCulture));
                }
                else if (FillStyle is LinearGradientBrush linearGradient)
                {
                    path.SetAttribute("fill", "url(#" + gradientName + ")");
                }
                else if (FillStyle is RadialGradientBrush radialGradient)
                {
                    path.SetAttribute("fill", "url(#" + gradientName + ")");
                }

                path.SetAttribute("transform", "matrix(" + _transform[0, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "," + _transform[0, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "," + _transform[0, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + ")");
            }
            else
            {
                Dictionary<string, string> style = new Dictionary<string, string>();

                style.Add("stroke", "none");

                if (FillStyle is SolidColourBrush solid)
                {
                    style.Add("fill", solid.Colour.ToCSSString(false));
                    style.Add("fill-opacity", solid.A.ToString(System.Globalization.CultureInfo.InvariantCulture));
                }
                else if (FillStyle is LinearGradientBrush linearGradient)
                {
                    style.Add("fill", "url(#" + gradientName + ")");
                }
                else if (FillStyle is RadialGradientBrush radialGradient)
                {
                    style.Add("fill", "url(#" + gradientName + ")");
                }

                style.Add("transform", "matrix(" + _transform[0, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "," + _transform[0, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "," + _transform[0, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + ")");

                string className = GetClass(style);

                path.SetAttribute("class", className);
            }

            if (!string.IsNullOrEmpty(Tag))
            {
                path.SetAttribute("id", Tag);
            }

            if (!string.IsNullOrEmpty(this.Tag) && this.linkDestinations.TryGetValue(this.Tag, out string destination) && !string.IsNullOrEmpty(destination))
            {
                XmlElement aElement = Document.CreateElement("a", SVGNamespace);
                aElement.SetAttribute("href", destination);
                currentElement.AppendChild(aElement);
                currentElement = aElement;
            }

            currentElement.AppendChild(path);

            currentElement = currElement;

            currentPath = new SVGPathObject();
            currentFigure = new SVGFigure();

        }

        public void FillText(string text, double x, double y)
        {
            if (!TextToPaths)
            {
                if (!UsedFontFamilies.ContainsKey(Font.FontFamily.FileName))
                {
                    UsedFontFamilies.Add(Font.FontFamily.FileName, Font.FontFamily);
                    UsedChars.Add(Font.FontFamily.FileName, new HashSet<char>());
                }

                UsedChars[Font.FontFamily.FileName].UnionWith(text);

                Font.DetailedFontMetrics metrics = Font.MeasureTextAdvanced(text);

                double[,] currTransform = null;
                double[,] deltaTransform = MatrixUtils.Identity;

                switch (TextBaseline)
                {
                    case TextBaselines.Baseline:
                        currTransform = MatrixUtils.Translate(_transform, x - metrics.LeftSideBearing, y);
                        deltaTransform = MatrixUtils.Translate(deltaTransform, x - metrics.LeftSideBearing, y);
                        break;
                    case TextBaselines.Top:
                        currTransform = MatrixUtils.Translate(_transform, x - metrics.LeftSideBearing, y + metrics.Top);
                        deltaTransform = MatrixUtils.Translate(deltaTransform, x - metrics.LeftSideBearing, y + metrics.Top);
                        break;
                    case TextBaselines.Bottom:
                        currTransform = MatrixUtils.Translate(_transform, x - metrics.LeftSideBearing, y + metrics.Bottom);
                        deltaTransform = MatrixUtils.Translate(deltaTransform, x - metrics.LeftSideBearing, y + metrics.Bottom);
                        break;
                    case TextBaselines.Middle:
                        currTransform = MatrixUtils.Translate(_transform, x - metrics.LeftSideBearing, y + (metrics.Top + metrics.Bottom) * 0.5);
                        deltaTransform = MatrixUtils.Translate(deltaTransform, x - metrics.LeftSideBearing, y + (metrics.Top + metrics.Bottom) * 0.5);
                        break;
                    default:
                        currTransform = MatrixUtils.Translate(_transform, x - metrics.LeftSideBearing, y);
                        deltaTransform = MatrixUtils.Translate(deltaTransform, x - metrics.LeftSideBearing, y);
                        break;
                }

                XmlElement currElement = currentElement;

                if (!string.IsNullOrEmpty(_currClipPath))
                {
                    currentElement = Document.CreateElement("g", SVGNamespace);
                    currentElement.SetAttribute("clip-path", _currClipPath);
                    currElement.AppendChild(currentElement);
                }

                XmlElement textElement = Document.CreateElement("text", SVGNamespace);

                string gradientName = null;

                {
                    if (FillStyle is LinearGradientBrush linearGradient)
                    {
                        gradientName = "g" + Guid.NewGuid().ToString("N");

                        XmlElement gradientElement = linearGradient.ToLinearGradient(Document, gradientName);

                        deltaTransform = MatrixUtils.Invert(deltaTransform);

                        gradientElement.SetAttribute("gradientTransform", "matrix(" + deltaTransform[0, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + deltaTransform[1, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                        "," + deltaTransform[0, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + deltaTransform[1, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                        "," + deltaTransform[0, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + deltaTransform[1, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + ")");

                        this.definitions.AppendChild(gradientElement);
                    }
                    else if (FillStyle is RadialGradientBrush radialGradient)
                    {
                        gradientName = "g" + Guid.NewGuid().ToString("N");

                        XmlElement gradientElement = radialGradient.ToRadialGradient(Document, gradientName);

                        deltaTransform = MatrixUtils.Invert(deltaTransform);

                        gradientElement.SetAttribute("gradientTransform", "matrix(" + deltaTransform[0, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + deltaTransform[1, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                        "," + deltaTransform[0, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + deltaTransform[1, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                        "," + deltaTransform[0, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + deltaTransform[1, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + ")");

                        this.definitions.AppendChild(gradientElement);
                    }
                }

                textElement.SetAttribute("x", "0");
                textElement.SetAttribute("y", "0");

                textElement.SetAttribute("transform", "matrix(" + currTransform[0, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + currTransform[1, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                        "," + currTransform[0, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + currTransform[1, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                        "," + currTransform[0, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + currTransform[1, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + ")");

                if (!UseStyles)
                {
                    textElement.SetAttribute("stroke", "none");

                    if (FillStyle is SolidColourBrush solid)
                    {
                        textElement.SetAttribute("fill", solid.Colour.ToCSSString(false));
                        textElement.SetAttribute("fill-opacity", solid.A.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else if (FillStyle is LinearGradientBrush linearGradient)
                    {
                        textElement.SetAttribute("fill", "url(#" + gradientName + ")");
                    }
                    else if (FillStyle is RadialGradientBrush radialGradient)
                    {
                        textElement.SetAttribute("fill", "url(#" + gradientName + ")");
                    }

                    textElement.SetAttribute("font-size", Font.FontSize.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    textElement.SetAttribute("font-family", Font.FontFamily.FileName);

                    if (Font.FontFamily.IsBold)
                    {
                        textElement.SetAttribute("font-weight", "bold");
                    }
                    else
                    {
                        textElement.SetAttribute("font-weight", "regular");
                    }

                    if (Font.FontFamily.IsItalic)
                    {
                        textElement.SetAttribute("font-style", "italic");
                    }
                    else
                    {
                        textElement.SetAttribute("font-style", "normal");
                    }

                    if (Font.FontFamily.IsOblique)
                    {
                        textElement.SetAttribute("font-style", "oblique");
                    }
                }
                else
                {
                    Dictionary<string, string> style = new Dictionary<string, string>();

                    style.Add("stroke", "none");

                    if (FillStyle is SolidColourBrush solid)
                    {
                        style.Add("fill", solid.Colour.ToCSSString(false));
                        style.Add("fill-opacity", solid.A.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else if (FillStyle is LinearGradientBrush linearGradient)
                    {
                        style.Add("fill", "url(#" + gradientName + ")");
                    }
                    else if (FillStyle is RadialGradientBrush radialGradient)
                    {
                        style.Add("fill", "url(#" + gradientName + ")");
                    }

                    style.Add("font-size", Font.FontSize.ToString(System.Globalization.CultureInfo.InvariantCulture) + "px");
                    style.Add("font-family", Font.FontFamily.FileName);

                    if (Font.FontFamily.IsBold)
                    {
                        style.Add("font-weight", "bold");
                    }
                    else
                    {
                        style.Add("font-weight", "regular");
                    }

                    if (Font.FontFamily.IsItalic)
                    {
                        style["font-style"] = "italic";
                    }
                    else
                    {
                        style["font-style"] = "normal";
                    }

                    if (Font.FontFamily.IsOblique)
                    {
                        style["font-style"] = "oblique";
                    }

                    string className = GetClass(style);

                    textElement.SetAttribute("class", className);
                }

                ProcessText(text, textElement);

                if (!string.IsNullOrEmpty(Tag))
                {
                    textElement.SetAttribute("id", Tag);
                }

                if (!string.IsNullOrEmpty(this.Tag) && this.linkDestinations.TryGetValue(this.Tag, out string destination) && !string.IsNullOrEmpty(destination))
                {
                    XmlElement aElement = Document.CreateElement("a", SVGNamespace);
                    aElement.SetAttribute("href", destination);
                    currentElement.AppendChild(aElement);
                    currentElement = aElement;
                }

                currentElement.AppendChild(textElement);

                currentElement = currElement;
            }
            else
            {
                PathText(text, x, y);
                Fill();
            }
        }

        public void LineTo(double x, double y)
        {
            if (currentFigure.PointCount == 0)
            {
                currentFigure.StartPoint = new Point(x, y);
            }

            currentFigure.CurrentPoint = new Point(x, y);
            currentFigure.Data += "L " + x.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + y.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
            currentFigure.PointCount++;
        }

        public void MoveTo(double x, double y)
        {
            if (currentFigure.PointCount > 0)
            {
                currentPath.Figures.Add(currentFigure);
            }

            currentFigure = new SVGFigure();
            currentFigure.CurrentPoint = new Point(x, y);
            currentFigure.StartPoint = new Point(x, y);
            currentFigure.Data += "M " + x.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + y.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
            currentFigure.PointCount = 1;
        }

        public void Rectangle(double x0, double y0, double width, double height)
        {
            MoveTo(x0, y0);
            LineTo(x0 + width, y0);
            LineTo(x0 + width, y0 + height);
            LineTo(x0, y0 + height);
            Close();
        }

        public void Restore()
        {
            _transform = states.Pop();
            _currClipPath = clipPaths.Pop();
            currentPath = new SVGPathObject();
            currentFigure = new SVGFigure();
        }

        public void Rotate(double angle)
        {
            _transform = MatrixUtils.Rotate(_transform, angle);
            currentPath = new SVGPathObject(); currentPath = new SVGPathObject();
            currentFigure = new SVGFigure();
        }

        public void Save()
        {
            states.Push((double[,])_transform.Clone());
            clipPaths.Push(_currClipPath);
        }

        public void Scale(double scaleX, double scaleY)
        {
            _transform = MatrixUtils.Scale(_transform, scaleX, scaleY);
            currentPath = new SVGPathObject();
            currentFigure = new SVGFigure();
        }

        public void SetFillStyle((int r, int g, int b, double a) style)
        {
            FillStyle = Colour.FromRgba(style);
        }

        public void SetFillStyle(Brush style)
        {
            FillStyle = style;
        }

        public void SetLineDash(LineDash dash)
        {
            _lineDash = dash;
        }

        public void SetStrokeStyle((int r, int g, int b, double a) style)
        {
            StrokeStyle = Colour.FromRgba(style);
        }

        public void SetStrokeStyle(Brush style)
        {
            StrokeStyle = style;
        }

        public void Stroke()
        {
            if (currentFigure.PointCount > 0)
            {
                currentPath.Figures.Add(currentFigure);
            }

            XmlElement currElement = currentElement;

            if (!string.IsNullOrEmpty(_currClipPath))
            {
                currentElement = Document.CreateElement("g", SVGNamespace);
                currentElement.SetAttribute("clip-path", _currClipPath);
                currElement.AppendChild(currentElement);
            }

            XmlElement path = Document.CreateElement("path", SVGNamespace);
            path.SetAttribute("d", currentPath.Figures.Aggregate("", (a, b) => a + b.Data));

            string gradientName = null;

            {
                if (StrokeStyle is LinearGradientBrush linearGradient)
                {
                    if (!gradients.TryGetValue(linearGradient, out gradientName))
                    {
                        gradientName = "gradient" + (gradients.Count + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);

                        XmlElement gradientElement = linearGradient.ToLinearGradient(Document, gradientName);
                        this.definitions.AppendChild(gradientElement);

                        gradients.Add(linearGradient, gradientName);
                    }
                }
                else if (StrokeStyle is RadialGradientBrush radialGradient)
                {
                    if (!gradients.TryGetValue(radialGradient, out gradientName))
                    {
                        gradientName = "gradient" + (gradients.Count + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);

                        XmlElement gradientElement = radialGradient.ToRadialGradient(Document, gradientName);
                        this.definitions.AppendChild(gradientElement);

                        gradients.Add(radialGradient, gradientName);
                    }
                }
            }

            if (!UseStyles)
            {

                if (StrokeStyle is SolidColourBrush solid)
                {
                    path.SetAttribute("stroke", solid.Colour.ToCSSString(false));
                    path.SetAttribute("stroke-opacity", solid.A.ToString(System.Globalization.CultureInfo.InvariantCulture));
                }
                else if (StrokeStyle is LinearGradientBrush linearGradient)
                {
                    path.SetAttribute("stroke", "url(#" + gradientName + ")");
                }
                else if (StrokeStyle is RadialGradientBrush radialGradient)
                {
                    path.SetAttribute("stroke", "url(#" + gradientName + ")");
                }

                path.SetAttribute("stroke-width", LineWidth.ToString(System.Globalization.CultureInfo.InvariantCulture));

                switch (LineCap)
                {
                    case LineCaps.Butt:
                        path.SetAttribute("stroke-linecap", "butt");
                        break;
                    case LineCaps.Round:
                        path.SetAttribute("stroke-linecap", "round");
                        break;
                    case LineCaps.Square:
                        path.SetAttribute("stroke-linecap", "square");
                        break;
                }

                switch (LineJoin)
                {
                    case LineJoins.Bevel:
                        path.SetAttribute("stroke-linejoin", "bevel");
                        break;
                    case LineJoins.Round:
                        path.SetAttribute("stroke-linejoin", "round");
                        break;
                    case LineJoins.Miter:
                        path.SetAttribute("stroke-linejoin", "miter");
                        break;
                }

                if (_lineDash.Phase != 0 || _lineDash.UnitsOn != 0 || _lineDash.UnitsOff != 0)
                {
                    path.SetAttribute("stroke-dasharray", _lineDash.UnitsOn.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + _lineDash.UnitsOff.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    path.SetAttribute("stroke-dashoffset", _lineDash.Phase.ToString(System.Globalization.CultureInfo.InvariantCulture));
                }

                path.SetAttribute("fill", "none");
                path.SetAttribute("transform", "matrix(" + _transform[0, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "," + _transform[0, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "," + _transform[0, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + ")");

            }
            else
            {
                Dictionary<string, string> style = new Dictionary<string, string>();

                if (StrokeStyle is SolidColourBrush solid)
                {
                    style.Add("stroke", solid.Colour.ToCSSString(false));
                    style.Add("stroke-opacity", solid.A.ToString(System.Globalization.CultureInfo.InvariantCulture));
                }
                else if (StrokeStyle is LinearGradientBrush linearGradient)
                {
                    style.Add("stroke", "url(#" + gradientName + ")");
                }
                else if (StrokeStyle is RadialGradientBrush radialGradient)
                {
                    style.Add("stroke", "url(#" + gradientName + ")");
                }

                style.Add("stroke-width", LineWidth.ToString(System.Globalization.CultureInfo.InvariantCulture) + "px");

                switch (LineCap)
                {
                    case LineCaps.Butt:
                        style.Add("stroke-linecap", "butt");
                        break;
                    case LineCaps.Round:
                        style.Add("stroke-linecap", "round");
                        break;
                    case LineCaps.Square:
                        style.Add("stroke-linecap", "square");
                        break;
                }

                switch (LineJoin)
                {
                    case LineJoins.Bevel:
                        style.Add("stroke-linejoin", "bevel");
                        break;
                    case LineJoins.Round:
                        style.Add("stroke-linejoin", "round");
                        break;
                    case LineJoins.Miter:
                        style.Add("stroke-linejoin", "miter");
                        break;
                }

                if (_lineDash.Phase != 0 || _lineDash.UnitsOn != 0 || _lineDash.UnitsOff != 0)
                {
                    style.Add("stroke-dasharray", _lineDash.UnitsOn.ToString(System.Globalization.CultureInfo.InvariantCulture) + "px " + _lineDash.UnitsOff.ToString(System.Globalization.CultureInfo.InvariantCulture) + "px");
                    style.Add("stroke-dashoffset", _lineDash.Phase.ToString(System.Globalization.CultureInfo.InvariantCulture) + "px");
                }

                style.Add("fill", "none");
                style.Add("transform", "matrix(" + _transform[0, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "," + _transform[0, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "," + _transform[0, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + ")");

                string className = GetClass(style);
                path.SetAttribute("class", className);

            }

            if (!string.IsNullOrEmpty(Tag))
            {
                path.SetAttribute("id", Tag);
            }

            if (!string.IsNullOrEmpty(this.Tag) && this.linkDestinations.TryGetValue(this.Tag, out string destination) && !string.IsNullOrEmpty(destination))
            {
                XmlElement aElement = Document.CreateElement("a", SVGNamespace);
                aElement.SetAttribute("href", destination);
                currentElement.AppendChild(aElement);
                currentElement = aElement;
            }

            currentElement.AppendChild(path);

            currentElement = currElement;

            currentPath = new SVGPathObject();
            currentFigure = new SVGFigure();
        }

        public void StrokeText(string text, double x, double y)
        {
            if (!TextToPaths)
            {
                if (!UsedFontFamilies.ContainsKey(Font.FontFamily.FileName))
                {
                    UsedFontFamilies.Add(Font.FontFamily.FileName, Font.FontFamily);
                    UsedChars.Add(Font.FontFamily.FileName, new HashSet<char>());
                }

                UsedChars[Font.FontFamily.FileName].UnionWith(text);

                Font.DetailedFontMetrics metrics = Font.MeasureTextAdvanced(text);

                double[,] currTransform = null;
                double[,] deltaTransform = MatrixUtils.Identity;

                switch (TextBaseline)
                {
                    case TextBaselines.Baseline:
                        currTransform = MatrixUtils.Translate(_transform, x - metrics.LeftSideBearing, y);
                        deltaTransform = MatrixUtils.Translate(deltaTransform, x - metrics.LeftSideBearing, y);
                        break;
                    case TextBaselines.Top:
                        currTransform = MatrixUtils.Translate(_transform, x - metrics.LeftSideBearing, y + metrics.Top);
                        deltaTransform = MatrixUtils.Translate(deltaTransform, x - metrics.LeftSideBearing, y + metrics.Top);
                        break;
                    case TextBaselines.Bottom:
                        currTransform = MatrixUtils.Translate(_transform, x - metrics.LeftSideBearing, y + metrics.Bottom);
                        deltaTransform = MatrixUtils.Translate(deltaTransform, x - metrics.LeftSideBearing, y + metrics.Bottom);
                        break;
                    case TextBaselines.Middle:
                        currTransform = MatrixUtils.Translate(_transform, x - metrics.LeftSideBearing, y + (metrics.Top + metrics.Bottom) * 0.5);
                        deltaTransform = MatrixUtils.Translate(deltaTransform, x - metrics.LeftSideBearing, y + (metrics.Top + metrics.Bottom) * 0.5);
                        break;
                    default:
                        currTransform = MatrixUtils.Translate(_transform, x - metrics.LeftSideBearing, y);
                        deltaTransform = MatrixUtils.Translate(deltaTransform, x - metrics.LeftSideBearing, y);
                        break;
                }

                XmlElement currElement = currentElement;

                if (!string.IsNullOrEmpty(_currClipPath))
                {
                    currentElement = Document.CreateElement("g", SVGNamespace);
                    currentElement.SetAttribute("clip-path", _currClipPath);
                    currElement.AppendChild(currentElement);
                }

                XmlElement textElement = Document.CreateElement("text", SVGNamespace);

                string gradientName = null;

                {
                    if (StrokeStyle is LinearGradientBrush linearGradient)
                    {
                        gradientName = "g" + Guid.NewGuid().ToString("N");

                        XmlElement gradientElement = linearGradient.ToLinearGradient(Document, gradientName);

                        deltaTransform = MatrixUtils.Invert(deltaTransform);

                        gradientElement.SetAttribute("gradientTransform", "matrix(" + deltaTransform[0, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + deltaTransform[1, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                        "," + deltaTransform[0, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + deltaTransform[1, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                        "," + deltaTransform[0, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + deltaTransform[1, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + ")");

                        this.definitions.AppendChild(gradientElement);
                    }
                    else if (StrokeStyle is RadialGradientBrush radialGradient)
                    {
                        gradientName = "g" + Guid.NewGuid().ToString("N");

                        XmlElement gradientElement = radialGradient.ToRadialGradient(Document, gradientName);

                        deltaTransform = MatrixUtils.Invert(deltaTransform);

                        gradientElement.SetAttribute("gradientTransform", "matrix(" + deltaTransform[0, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + deltaTransform[1, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                        "," + deltaTransform[0, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + deltaTransform[1, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                        "," + deltaTransform[0, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + deltaTransform[1, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + ")");

                        this.definitions.AppendChild(gradientElement);
                    }
                }

                textElement.SetAttribute("x", "0");
                textElement.SetAttribute("y", "0");

                textElement.SetAttribute("transform", "matrix(" + currTransform[0, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + currTransform[1, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                        "," + currTransform[0, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + currTransform[1, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                        "," + currTransform[0, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + currTransform[1, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + ")");

                if (!UseStyles)
                {
                    if (StrokeStyle is SolidColourBrush solid)
                    {
                        textElement.SetAttribute("stroke", solid.Colour.ToCSSString(false));
                        textElement.SetAttribute("stroke-opacity", solid.A.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else if (StrokeStyle is LinearGradientBrush linearGradient)
                    {
                        textElement.SetAttribute("stroke", "url(#" + gradientName + ")");
                    }
                    else if (StrokeStyle is RadialGradientBrush radialGradient)
                    {
                        textElement.SetAttribute("stroke", "url(#" + gradientName + ")");
                    }

                    textElement.SetAttribute("stroke-width", LineWidth.ToString(System.Globalization.CultureInfo.InvariantCulture));

                    switch (LineCap)
                    {
                        case LineCaps.Butt:
                            textElement.SetAttribute("stroke-linecap", "butt");
                            break;
                        case LineCaps.Round:
                            textElement.SetAttribute("stroke-linecap", "round");
                            break;
                        case LineCaps.Square:
                            textElement.SetAttribute("stroke-linecap", "square");
                            break;
                    }

                    switch (LineJoin)
                    {
                        case LineJoins.Bevel:
                            textElement.SetAttribute("stroke-linejoin", "bevel");
                            break;
                        case LineJoins.Round:
                            textElement.SetAttribute("stroke-linejoin", "round");
                            break;
                        case LineJoins.Miter:
                            textElement.SetAttribute("stroke-linejoin", "miter");
                            break;
                    }

                    if (_lineDash.Phase != 0 || _lineDash.UnitsOn != 0 || _lineDash.UnitsOff != 0)
                    {
                        textElement.SetAttribute("stroke-dasharray", _lineDash.UnitsOn.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + _lineDash.UnitsOff.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        textElement.SetAttribute("stroke-dashoffset", _lineDash.Phase.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    textElement.SetAttribute("fill", "none");

                    textElement.SetAttribute("font-size", Font.FontSize.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    textElement.SetAttribute("font-family", Font.FontFamily.FileName);

                    if (Font.FontFamily.IsBold)
                    {
                        textElement.SetAttribute("font-weight", "bold");
                    }
                    else
                    {
                        textElement.SetAttribute("font-weight", "regular");
                    }

                    if (Font.FontFamily.IsItalic)
                    {
                        textElement.SetAttribute("font-style", "italic");
                    }
                    else
                    {
                        textElement.SetAttribute("font-style", "normal");
                    }

                    if (Font.FontFamily.IsOblique)
                    {
                        textElement.SetAttribute("font-style", "oblique");
                    }
                }
                else
                {
                    Dictionary<string, string> style = new Dictionary<string, string>();

                    if (StrokeStyle is SolidColourBrush solid)
                    {
                        style.Add("stroke", solid.Colour.ToCSSString(false));
                        style.Add("stroke-opacity", solid.A.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else if (StrokeStyle is LinearGradientBrush linearGradient)
                    {
                        style.Add("stroke", "url(#" + gradientName + ")");
                    }
                    else if (StrokeStyle is RadialGradientBrush radialGradient)
                    {
                        style.Add("stroke", "url(#" + gradientName + ")");
                    }

                    style.Add("stroke-width", LineWidth.ToString(System.Globalization.CultureInfo.InvariantCulture) + "px");

                    switch (LineCap)
                    {
                        case LineCaps.Butt:
                            style.Add("stroke-linecap", "butt");
                            break;
                        case LineCaps.Round:
                            style.Add("stroke-linecap", "round");
                            break;
                        case LineCaps.Square:
                            style.Add("stroke-linecap", "square");
                            break;
                    }

                    switch (LineJoin)
                    {
                        case LineJoins.Bevel:
                            style.Add("stroke-linejoin", "bevel");
                            break;
                        case LineJoins.Round:
                            style.Add("stroke-linejoin", "round");
                            break;
                        case LineJoins.Miter:
                            style.Add("stroke-linejoin", "miter");
                            break;
                    }

                    if (_lineDash.Phase != 0 || _lineDash.UnitsOn != 0 || _lineDash.UnitsOff != 0)
                    {
                        style.Add("stroke-dasharray", _lineDash.UnitsOn.ToString(System.Globalization.CultureInfo.InvariantCulture) + "px " + _lineDash.UnitsOff.ToString(System.Globalization.CultureInfo.InvariantCulture) + "px");
                        style.Add("stroke-dashoffset", _lineDash.Phase.ToString(System.Globalization.CultureInfo.InvariantCulture) + "px");
                    }
                    style.Add("fill", "none");

                    style.Add("font-size", Font.FontSize.ToString(System.Globalization.CultureInfo.InvariantCulture) + "px");
                    style.Add("font-family", Font.FontFamily.FileName);

                    if (Font.FontFamily.IsBold)
                    {
                        style.Add("font-weight", "bold");
                    }
                    else
                    {
                        style.Add("font-weight", "regular");
                    }

                    if (Font.FontFamily.IsItalic)
                    {
                        style["font-style"] = "italic";
                    }
                    else
                    {
                        style["font-style"] = "normal";
                    }

                    if (Font.FontFamily.IsOblique)
                    {
                        style["font-style"] = "oblique";
                    }

                    string className = GetClass(style);

                    textElement.SetAttribute("class", className);
                }

                ProcessText(text, textElement);

                if (!string.IsNullOrEmpty(Tag))
                {
                    textElement.SetAttribute("id", Tag);
                }

                if (!string.IsNullOrEmpty(this.Tag) && this.linkDestinations.TryGetValue(this.Tag, out string destination) && !string.IsNullOrEmpty(destination))
                {
                    XmlElement aElement = Document.CreateElement("a", SVGNamespace);
                    aElement.SetAttribute("href", destination);
                    currentElement.AppendChild(aElement);
                    currentElement = aElement;
                }

                currentElement.AppendChild(textElement);

                currentElement = currElement;
            }
            else
            {
                PathText(text, x, y);
                Stroke();
            }
        }

        public void Transform(double a, double b, double c, double d, double e, double f)
        {
            double[,] transfMatrix = new double[3, 3] { { a, c, e }, { b, d, f }, { 0, 0, 1 } };
            _transform = MatrixUtils.Multiply(_transform, transfMatrix);

            currentPath = new SVGPathObject();
            currentFigure = new SVGFigure();
        }

        public void Translate(double x, double y)
        {
            _transform = MatrixUtils.Translate(_transform, x, y);

            currentPath = new SVGPathObject();
            currentFigure = new SVGFigure();
        }

        public void SetClippingPath()
        {
            if (currentFigure.PointCount > 0)
            {
                currentPath.Figures.Add(currentFigure);
            }

            XmlElement clipPath = Document.CreateElement("clipPath", SVGNamespace);
            string id = Guid.NewGuid().ToString("N");
            clipPath.SetAttribute("id", id);

            if (!string.IsNullOrEmpty(_currClipPath))
            {
                clipPath.SetAttribute("clip-path", _currClipPath);
            }

            XmlElement path = Document.CreateElement("path", SVGNamespace);
            path.SetAttribute("d", currentPath.Figures.Aggregate("", (a, b) => a + b.Data));

            if (StrokeStyle is SolidColourBrush solid)
            {
                path.SetAttribute("stroke", solid.Colour.ToCSSString(false));
                path.SetAttribute("stroke-opacity", solid.A.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            else if (StrokeStyle is LinearGradientBrush linearGradient)
            {
                string gradientName;

                if (!gradients.TryGetValue(linearGradient, out gradientName))
                {
                    gradientName = "gradient" + (gradients.Count + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);

                    XmlElement gradientElement = linearGradient.ToLinearGradient(Document, gradientName);
                    this.definitions.AppendChild(gradientElement);

                    gradients.Add(linearGradient, gradientName);
                }

                path.SetAttribute("stroke", "url(#" + gradientName + ")");
            }
            else if (StrokeStyle is RadialGradientBrush radialGradient)
            {
                string gradientName;

                if (!gradients.TryGetValue(radialGradient, out gradientName))
                {
                    gradientName = "gradient" + (gradients.Count + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);

                    XmlElement gradientElement = radialGradient.ToRadialGradient(Document, gradientName);
                    this.definitions.AppendChild(gradientElement);

                    gradients.Add(radialGradient, gradientName);
                }

                path.SetAttribute("stroke", "url(#" + gradientName + ")");
            }

            path.SetAttribute("stroke-width", LineWidth.ToString(System.Globalization.CultureInfo.InvariantCulture));

            switch (LineCap)
            {
                case LineCaps.Butt:
                    path.SetAttribute("stroke-linecap", "butt");
                    break;
                case LineCaps.Round:
                    path.SetAttribute("stroke-linecap", "round");
                    break;
                case LineCaps.Square:
                    path.SetAttribute("stroke-linecap", "square");
                    break;
            }

            switch (LineJoin)
            {
                case LineJoins.Bevel:
                    path.SetAttribute("stroke-linejoin", "bevel");
                    break;
                case LineJoins.Round:
                    path.SetAttribute("stroke-linejoin", "round");
                    break;
                case LineJoins.Miter:
                    path.SetAttribute("stroke-linejoin", "miter");
                    break;
            }

            if (_lineDash.Phase != 0 || _lineDash.UnitsOn != 0 || _lineDash.UnitsOff != 0)
            {
                path.SetAttribute("stroke-dasharray", _lineDash.UnitsOn.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + _lineDash.UnitsOff.ToString(System.Globalization.CultureInfo.InvariantCulture));
                path.SetAttribute("stroke-dashoffset", _lineDash.Phase.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }

            path.SetAttribute("fill", "none");
            path.SetAttribute("transform", "matrix(" + _transform[0, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                "," + _transform[0, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                "," + _transform[0, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + ")");

            clipPath.AppendChild(path);

            currentElement.AppendChild(clipPath);

            _currClipPath = "url(#" + id + ")";

            currentPath = new SVGPathObject();
            currentFigure = new SVGFigure();
        }

        public void DrawRasterImage(int sourceX, int sourceY, int sourceWidth, int sourceHeight, double destinationX, double destinationY, double destinationWidth, double destinationHeight, RasterImage image)
        {
            Save();

            MoveTo(destinationX, destinationY);
            LineTo(destinationX + destinationWidth, destinationY);
            LineTo(destinationX + destinationWidth, destinationY + destinationHeight);
            LineTo(destinationX, destinationY + destinationHeight);
            Close();
            SetClippingPath();

            double sourceRectX = (double)sourceX / image.Width;
            double sourceRectY = (double)sourceY / image.Height;
            double sourceRectWidth = (double)sourceWidth / image.Width;
            double sourceRectHeight = (double)sourceHeight / image.Height;

            double scaleX = destinationWidth / sourceRectWidth;
            double scaleY = destinationHeight / sourceRectHeight;

            double translationX = destinationX / scaleX - sourceRectX;
            double translationY = destinationY / scaleY - sourceRectY;

            Scale(scaleX, scaleY);
            Translate(translationX, translationY);

            XmlElement currElement = currentElement;

            if (!string.IsNullOrEmpty(_currClipPath))
            {
                currentElement = Document.CreateElement("g", SVGNamespace);
                currentElement.SetAttribute("clip-path", _currClipPath);
                currElement.AppendChild(currentElement);
            }

            XmlElement img = Document.CreateElement("image", SVGNamespace);
            img.SetAttribute("x", "0");
            img.SetAttribute("y", "0");

            img.SetAttribute("width", "1");
            img.SetAttribute("height", "1");

            img.SetAttribute("preserveAspectRatio", "none");

            if (image.Interpolate)
            {
                img.SetAttribute("image-rendering", "optimizeQuality");
            }
            else
            {
                img.SetAttribute("image-rendering", "pixelated");
            }

            img.SetAttribute("transform", "matrix(" + _transform[0, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                "," + _transform[0, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                "," + _transform[0, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + ")");

            if (!string.IsNullOrEmpty(Tag))
            {
                img.SetAttribute("id", Tag);
            }

            img.SetAttribute("href", "http://www.w3.org/1999/xlink", "data:image/png;base64," + Convert.ToBase64String(image.PNGStream.ToArray()));

            if (!string.IsNullOrEmpty(this.Tag) && this.linkDestinations.TryGetValue(this.Tag, out string destination) && !string.IsNullOrEmpty(destination))
            {
                XmlElement aElement = Document.CreateElement("a", SVGNamespace);
                aElement.SetAttribute("href", destination);
                currentElement.AppendChild(aElement);
                currentElement = aElement;
            }

            currentElement.AppendChild(img);

            currentElement = currElement;

            Restore();
        }

        private void ProcessText(string text, XmlNode parent)
        {
            if (Font.EnableKerning && this.TextOption == SVGContextInterpreter.TextOptions.SubsetFonts)
            {
                List<(string, Point)> tSpans = new List<(string, Point)>();

                StringBuilder currentRun = new StringBuilder();
                Point currentKerning = new Point();

                Point currentGlyphPlacementDelta = new Point();
                Point currentGlyphAdvanceDelta = new Point();
                Point nextGlyphPlacementDelta = new Point();
                Point nextGlyphAdvanceDelta = new Point();

                for (int i = 0; i < text.Length; i++)
                {
                    if (i < text.Length - 1)
                    {
                        currentGlyphPlacementDelta = nextGlyphPlacementDelta;
                        currentGlyphAdvanceDelta = nextGlyphAdvanceDelta;
                        nextGlyphAdvanceDelta = new Point();
                        nextGlyphPlacementDelta = new Point();

                        TrueTypeFile.PairKerning kerning = Font.FontFamily.TrueTypeFile.Get1000EmKerning(text[i], text[i + 1]);

                        if (kerning != null)
                        {
                            currentGlyphPlacementDelta = new Point(currentGlyphPlacementDelta.X + kerning.Glyph1Placement.X, currentGlyphPlacementDelta.Y + kerning.Glyph1Placement.Y);
                            currentGlyphAdvanceDelta = new Point(currentGlyphAdvanceDelta.X + kerning.Glyph1Advance.X, currentGlyphAdvanceDelta.Y + kerning.Glyph1Advance.Y);

                            nextGlyphPlacementDelta = new Point(nextGlyphPlacementDelta.X + kerning.Glyph2Placement.X, nextGlyphPlacementDelta.Y + kerning.Glyph2Placement.Y);
                            nextGlyphAdvanceDelta = new Point(nextGlyphAdvanceDelta.X + kerning.Glyph2Advance.X, nextGlyphAdvanceDelta.Y + kerning.Glyph2Advance.Y);
                        }
                    }

                    if (currentGlyphPlacementDelta.X != 0 || currentGlyphPlacementDelta.Y != 0 || currentGlyphAdvanceDelta.X != 0 || currentGlyphAdvanceDelta.Y != 0)
                    {
                        if (currentRun.Length > 0)
                        {
                            tSpans.Add((currentRun.ToString(), currentKerning));

                            tSpans.Add((text[i].ToString(), new Point(currentGlyphPlacementDelta.X * Font.FontSize / 1000, currentGlyphPlacementDelta.Y * Font.FontSize / 1000)));

                            currentRun.Clear();
                            currentKerning = new Point((currentGlyphAdvanceDelta.X - currentGlyphPlacementDelta.X) * Font.FontSize / 1000, (currentGlyphAdvanceDelta.Y - currentGlyphPlacementDelta.Y) * Font.FontSize / 1000);
                        }
                        else
                        {
                            tSpans.Add((text[i].ToString(), new Point(currentGlyphPlacementDelta.X * Font.FontSize / 1000 + currentKerning.X, currentGlyphPlacementDelta.Y * Font.FontSize / 1000 + currentKerning.Y)));

                            currentRun.Clear();
                            currentKerning = new Point((currentGlyphAdvanceDelta.X - currentGlyphPlacementDelta.X) * Font.FontSize / 1000, (currentGlyphAdvanceDelta.Y - currentGlyphPlacementDelta.Y) * Font.FontSize / 1000);
                        }
                    }
                    else
                    {
                        currentRun.Append(text[i]);
                    }
                }

                if (currentRun.Length > 0)
                {
                    tSpans.Add((currentRun.ToString(), currentKerning));
                }

                for (int i = 0; i < tSpans.Count; i++)
                {
                    XmlElement tspanElement = Document.CreateElement("tspan", SVGNamespace);
                    tspanElement.InnerText = tSpans[i].Item1.Replace(" ", "\u00A0");

                    if (tSpans[i].Item2.X != 0)
                    {
                        tspanElement.SetAttribute("dx", tSpans[i].Item2.X.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }

                    if (tSpans[i].Item2.Y != 0)
                    {
                        tspanElement.SetAttribute("dy", tSpans[i].Item2.Y.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }

                    parent.AppendChild(tspanElement);
                }

            }
            else
            {
                parent.InnerText = text.Replace(" ", "\u00A0");
            }
        }

        public void DrawFilteredGraphics(Graphics graphics, IFilter filter)
        {
            if (FilterOption.Operation == SVGContextInterpreter.FilterOption.FilterOperations.IgnoreAll)
            {
                graphics.CopyToIGraphicsContext(this);
            }
            else if (FilterOption.Operation == SVGContextInterpreter.FilterOption.FilterOperations.SkipAll)
            {

            }
            else
            {
                bool rasterisationNeeded = false;
                bool justDraw = false;

                if (FilterOption.Operation == SVGContextInterpreter.FilterOption.FilterOperations.RasteriseAll)
                {
                    rasterisationNeeded = true;
                    justDraw = false;
                }
                else
                {
                    if (FilterOption.Operation == SVGContextInterpreter.FilterOption.FilterOperations.RasteriseIfNecessary)
                    {
                        justDraw = false;
                        rasterisationNeeded = true;
                    }
                    else if (FilterOption.Operation == SVGContextInterpreter.FilterOption.FilterOperations.NeverRasteriseAndIgnore)
                    {
                        rasterisationNeeded = false;
                        justDraw = true;
                    }
                    else if (FilterOption.Operation == SVGContextInterpreter.FilterOption.FilterOperations.NeverRasteriseAndSkip)
                    {
                        rasterisationNeeded = false;
                        justDraw = false;
                    }

                    if (filter is MaskFilter mask)
                    {
                        rasterisationNeeded = false;
                        justDraw = false;

                        XmlElement currElement = currentElement;

                        if (!string.IsNullOrEmpty(_currClipPath))
                        {
                            currentElement = Document.CreateElement("g", SVGNamespace);
                            currentElement.SetAttribute("clip-path", _currClipPath);
                            currElement.AppendChild(currentElement);
                        }

                        XmlElement currentElement2 = currentElement;

                        string filterGuid = Guid.NewGuid().ToString("N");

                        Rectangle bounds = graphics.GetBounds();

                        Point p1 = new Point(bounds.Location.X, bounds.Location.Y);
                        Point p2 = new Point(bounds.Location.X + bounds.Size.Width, bounds.Location.Y);
                        Point p3 = new Point(bounds.Location.X + bounds.Size.Width, bounds.Location.Y + bounds.Size.Height);
                        Point p4 = new Point(bounds.Location.X, bounds.Location.Y + bounds.Size.Height);

                        /*p1 = MatrixUtils.Multiply(_transform, p1);
                        p2 = MatrixUtils.Multiply(_transform, p2);
                        p3 = MatrixUtils.Multiply(_transform, p3);
                        p4 = MatrixUtils.Multiply(_transform, p4);*/

                        bounds = Point.Bounds(p1, p2, p3, p4);

                        XmlElement maskElement = Document.CreateElement("mask", SVGNamespace);
                        maskElement.SetAttribute("id", filterGuid);
                        maskElement.SetAttribute("maskUnits", "userSpaceOnUse");
                        maskElement.SetAttribute("x", bounds.Location.X.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        maskElement.SetAttribute("y", bounds.Location.Y.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        maskElement.SetAttribute("width", bounds.Size.Width.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        maskElement.SetAttribute("height", bounds.Size.Height.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        this.definitions.AppendChild(maskElement);

                        currentElement = maskElement;

                        double[,] currTransform = _transform;
                        _transform = MatrixUtils.Identity;

                        mask.Mask.CopyToIGraphicsContext(this);

                        _transform = currTransform;

                        currentElement = Document.CreateElement("g", SVGNamespace);
                        currentElement.SetAttribute("mask", "url(#" + filterGuid + ")");
                        currentElement2.AppendChild(currentElement);

                        currentElement.SetAttribute("transform", "matrix(" + _transform[0, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "," + _transform[0, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "," + _transform[0, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + ")");
                        currTransform = _transform;
                        _transform = MatrixUtils.Identity;

                        graphics.CopyToIGraphicsContext(this);

                        _transform = currTransform;

                        currentElement = currElement;
                    }
                    if (filter is GaussianBlurFilter gauss)
                    {
                        rasterisationNeeded = false;
                        justDraw = false;
                        XmlElement currElement = currentElement;

                        if (!string.IsNullOrEmpty(_currClipPath))
                        {
                            currentElement = Document.CreateElement("g", SVGNamespace);
                            currentElement.SetAttribute("clip-path", _currClipPath);
                            currElement.AppendChild(currentElement);
                        }

                        Rectangle bounds = graphics.GetBounds();

                        Point p1 = new Point(bounds.Location.X - gauss.StandardDeviation * 3, bounds.Location.Y - gauss.StandardDeviation * 3);
                        Point p2 = new Point(bounds.Location.X + bounds.Size.Width + gauss.StandardDeviation * 3, bounds.Location.Y - gauss.StandardDeviation * 3);
                        Point p3 = new Point(bounds.Location.X + bounds.Size.Width + gauss.StandardDeviation * 3, bounds.Location.Y + bounds.Size.Height + gauss.StandardDeviation * 3);
                        Point p4 = new Point(bounds.Location.X - gauss.StandardDeviation * 3, bounds.Location.Y + bounds.Size.Height + gauss.StandardDeviation * 3);

                        /*p1 = MatrixUtils.Multiply(_transform, p1);
                        p2 = MatrixUtils.Multiply(_transform, p2);
                        p3 = MatrixUtils.Multiply(_transform, p3);
                        p4 = MatrixUtils.Multiply(_transform, p4);*/

                        bounds = Point.Bounds(p1, p2, p3, p4);

                        string filterGuid = Guid.NewGuid().ToString("N");

                        XmlElement filterElement = Document.CreateElement("filter", SVGNamespace);
                        filterElement.SetAttribute("id", filterGuid);
                        filterElement.SetAttribute("color-interpolation-filters", "sRGB");
                        filterElement.SetAttribute("filterUnits", "userSpaceOnUse");
                        filterElement.SetAttribute("x", bounds.Location.X.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        filterElement.SetAttribute("y", bounds.Location.Y.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        filterElement.SetAttribute("width", bounds.Size.Width.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        filterElement.SetAttribute("height", bounds.Size.Height.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        this.definitions.AppendChild(filterElement);

                        XmlElement feElement = Document.CreateElement("feGaussianBlur", SVGNamespace);
                        feElement.SetAttribute("stdDeviation", gauss.StandardDeviation.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        filterElement.AppendChild(feElement);

                        XmlElement currentElement2 = currentElement;

                        currentElement = Document.CreateElement("g", SVGNamespace);
                        currentElement.SetAttribute("filter", "url(#" + filterGuid + ")");
                        currentElement2.AppendChild(currentElement);

                        currentElement.SetAttribute("transform", "matrix(" + _transform[0, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "," + _transform[0, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "," + _transform[0, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + ")");
                        double[,] currTransform = _transform;
                        _transform = MatrixUtils.Identity;

                        graphics.CopyToIGraphicsContext(this);

                        currentElement = currElement;
                    }
                    else if (filter is ColourMatrixFilter cmf)
                    {
                        rasterisationNeeded = false;
                        justDraw = false;
                        XmlElement currElement = currentElement;

                        if (!string.IsNullOrEmpty(_currClipPath))
                        {
                            currentElement = Document.CreateElement("g", SVGNamespace);
                            currentElement.SetAttribute("clip-path", _currClipPath);
                            currElement.AppendChild(currentElement);
                        }

                        Rectangle bounds = graphics.GetBounds();

                        Point p1 = new Point(bounds.Location.X, bounds.Location.Y);
                        Point p2 = new Point(bounds.Location.X + bounds.Size.Width, bounds.Location.Y);
                        Point p3 = new Point(bounds.Location.X + bounds.Size.Width, bounds.Location.Y + bounds.Size.Height);
                        Point p4 = new Point(bounds.Location.X, bounds.Location.Y + bounds.Size.Height);

                        /* p1 = MatrixUtils.Multiply(_transform, p1);
                         p2 = MatrixUtils.Multiply(_transform, p2);
                         p3 = MatrixUtils.Multiply(_transform, p3);
                         p4 = MatrixUtils.Multiply(_transform, p4);*/

                        bounds = Point.Bounds(p1, p2, p3, p4);

                        string filterGuid = Guid.NewGuid().ToString("N");

                        XmlElement filterElement = Document.CreateElement("filter", SVGNamespace);
                        filterElement.SetAttribute("id", filterGuid);
                        filterElement.SetAttribute("color-interpolation-filters", "sRGB");
                        filterElement.SetAttribute("filterUnits", "userSpaceOnUse");
                        filterElement.SetAttribute("x", bounds.Location.X.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        filterElement.SetAttribute("y", bounds.Location.Y.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        filterElement.SetAttribute("width", bounds.Size.Width.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        filterElement.SetAttribute("height", bounds.Size.Height.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        this.definitions.AppendChild(filterElement);

                        XmlElement feElement = Document.CreateElement("feColorMatrix", SVGNamespace);
                        feElement.SetAttribute("type", "matrix");

                        StringBuilder matrix = new StringBuilder();

                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                matrix.Append(cmf.ColourMatrix[i, j].ToString(System.Globalization.CultureInfo.InvariantCulture));
                                if (i != 3 || j != 4)
                                {
                                    matrix.Append(" ");
                                }
                            }
                        }

                        feElement.SetAttribute("values", matrix.ToString());
                        filterElement.AppendChild(feElement);

                        XmlElement currentElement2 = currentElement;

                        currentElement = Document.CreateElement("g", SVGNamespace);
                        currentElement.SetAttribute("filter", "url(#" + filterGuid + ")");
                        currentElement2.AppendChild(currentElement);

                        currentElement.SetAttribute("transform", "matrix(" + _transform[0, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "," + _transform[0, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "," + _transform[0, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + ")");
                        double[,] currTransform = _transform;
                        _transform = MatrixUtils.Identity;

                        graphics.CopyToIGraphicsContext(this);

                        currentElement = currElement;
                    }
                    else if (filter is CompositeLocationInvariantFilter comp)
                    {
                        bool allSupported = true;

                        foreach (IFilter filter2 in comp.Filters)
                        {
                            if (!(filter2 is GaussianBlurFilter) && !(filter2 is ColourMatrixFilter))
                            {
                                allSupported = false;
                                break;
                            }
                        }

                        if (allSupported)
                        {
                            rasterisationNeeded = false;
                            justDraw = false;

                            XmlElement currElement = currentElement;

                            if (!string.IsNullOrEmpty(_currClipPath))
                            {
                                currentElement = Document.CreateElement("g", SVGNamespace);
                                currentElement.SetAttribute("clip-path", _currClipPath);
                                currElement.AppendChild(currentElement);
                            }

                            Rectangle bounds = graphics.GetBounds();

                            Point p1 = new Point(bounds.Location.X - comp.TopLeftMargin.X, bounds.Location.Y - comp.TopLeftMargin.Y);
                            Point p2 = new Point(bounds.Location.X + bounds.Size.Width + comp.BottomRightMargin.X, bounds.Location.Y - comp.TopLeftMargin.Y);
                            Point p3 = new Point(bounds.Location.X + bounds.Size.Width + comp.BottomRightMargin.X, bounds.Location.Y + bounds.Size.Height + comp.BottomRightMargin.Y);
                            Point p4 = new Point(bounds.Location.X - comp.TopLeftMargin.X, bounds.Location.Y + bounds.Size.Height + comp.BottomRightMargin.Y);

                            /* p1 = MatrixUtils.Multiply(_transform, p1);
                             p2 = MatrixUtils.Multiply(_transform, p2);
                             p3 = MatrixUtils.Multiply(_transform, p3);
                             p4 = MatrixUtils.Multiply(_transform, p4);*/

                            bounds = Point.Bounds(p1, p2, p3, p4);

                            string filterGuid = Guid.NewGuid().ToString("N");

                            XmlElement filterElement = Document.CreateElement("filter", SVGNamespace);
                            filterElement.SetAttribute("id", filterGuid);
                            filterElement.SetAttribute("color-interpolation-filters", "sRGB");
                            filterElement.SetAttribute("filterUnits", "userSpaceOnUse");
                            filterElement.SetAttribute("x", bounds.Location.X.ToString(System.Globalization.CultureInfo.InvariantCulture));
                            filterElement.SetAttribute("y", bounds.Location.Y.ToString(System.Globalization.CultureInfo.InvariantCulture));
                            filterElement.SetAttribute("width", bounds.Size.Width.ToString(System.Globalization.CultureInfo.InvariantCulture));
                            filterElement.SetAttribute("height", bounds.Size.Height.ToString(System.Globalization.CultureInfo.InvariantCulture));
                            this.definitions.AppendChild(filterElement);

                            foreach (IFilter filter2 in comp.Filters)
                            {
                                if (filter2 is GaussianBlurFilter gauss2)
                                {
                                    XmlElement feElement = Document.CreateElement("feGaussianBlur", SVGNamespace);
                                    feElement.SetAttribute("stdDeviation", gauss2.StandardDeviation.ToString(System.Globalization.CultureInfo.InvariantCulture));
                                    filterElement.AppendChild(feElement);
                                }
                                else if (filter2 is ColourMatrixFilter cmf2)
                                {
                                    XmlElement feElement = Document.CreateElement("feColorMatrix", SVGNamespace);
                                    feElement.SetAttribute("type", "matrix");

                                    StringBuilder matrix = new StringBuilder();

                                    for (int i = 0; i < 4; i++)
                                    {
                                        for (int j = 0; j < 5; j++)
                                        {
                                            matrix.Append(cmf2.ColourMatrix[i, j].ToString(System.Globalization.CultureInfo.InvariantCulture));
                                            if (i != 3 || j != 4)
                                            {
                                                matrix.Append(" ");
                                            }
                                        }
                                    }

                                    feElement.SetAttribute("values", matrix.ToString());
                                    filterElement.AppendChild(feElement);
                                }
                            }

                            XmlElement currentElement2 = currentElement;

                            currentElement = Document.CreateElement("g", SVGNamespace);
                            currentElement.SetAttribute("filter", "url(#" + filterGuid + ")");
                            currentElement2.AppendChild(currentElement);

                            currentElement.SetAttribute("transform", "matrix(" + _transform[0, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 0].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "," + _transform[0, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 1].ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "," + _transform[0, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + _transform[1, 2].ToString(System.Globalization.CultureInfo.InvariantCulture) + ")");
                            double[,] currTransform = _transform;
                            _transform = MatrixUtils.Identity;

                            graphics.CopyToIGraphicsContext(this);

                            currentElement = currElement;


                        }
                    }
                }

                if (rasterisationNeeded)
                {
                    double scale = FilterOption.RasterisationResolution;

                    Rectangle bounds = graphics.GetBounds();

                    bounds = new Rectangle(bounds.Location.X - filter.TopLeftMargin.X, bounds.Location.Y - filter.TopLeftMargin.Y, bounds.Size.Width + filter.TopLeftMargin.X + filter.BottomRightMargin.X, bounds.Size.Height + filter.TopLeftMargin.Y + filter.BottomRightMargin.Y);

                    if (bounds.Size.Width > 0 && bounds.Size.Height > 0)
                    {
                        if (!FilterOption.RasterisationResolutionRelative)
                        {
                            scale = scale / Math.Min(bounds.Size.Width, bounds.Size.Height);
                        }

                        if (graphics.TryRasterise(bounds, scale, true, out RasterImage rasterised))
                        {
                            RasterImage filtered = null;

                            if (filter is ILocationInvariantFilter locInvFilter)
                            {
                                filtered = locInvFilter.Filter(rasterised, scale);
                            }
                            else if (filter is IFilterWithLocation filterWithLoc)
                            {
                                filtered = filterWithLoc.Filter(rasterised, bounds, scale);
                            }

                            if (filtered != null)
                            {
                                rasterised.Dispose();

                                DrawRasterImage(0, 0, filtered.Width, filtered.Height, bounds.Location.X, bounds.Location.Y, bounds.Size.Width, bounds.Size.Height, filtered);
                            }
                        }
                        else
                        {
                            throw new NotImplementedException(@"The filter could not be rasterised! You can avoid this error by doing one of the following:
 • Add a reference to VectSharp.Raster or VectSharp.Raster.ImageSharp (you may also need to add a using directive somewhere to force the assembly to be loaded).
 • Provide your own implementation of Graphics.RasterisationMethod.
 • Set the FilterOption.Operation to ""NeverRasteriseAndIgnore"", ""NeverRasteriseAndSkip"", ""IgnoreAll"" or ""SkipAll"".");
                        }
                    }
                }

                if (justDraw)
                {
                    graphics.CopyToIGraphicsContext(this);
                }
            }
        }
    }


    /// <summary>
    /// Contains methods to render a <see cref="Page"/> as an SVG file.
    /// </summary>
    public static class SVGContextInterpreter
    {

        /// <summary>
        /// Render the page to an SVG file.
        /// </summary>
        /// <param name="page">The <see cref="Page"/> to render.</param>
        /// <param name="fileName">The full path to the file to save. If it exists, it will be overwritten.</param>
        /// <param name="textOption">Defines whether the used fonts should be included in the file.</param>
        /// <param name="linkDestinations">A dictionary associating element tags to link targets. If this is provided, objects that have been drawn with a tag contained in the dictionary will become hyperlink to the destination specified in the dictionary. If the destination starts with a hash (#), it is interpreted as the tag of another object in the current document; otherwise, it is interpreted as an external URI.</param>
        /// <param name="filterOption">Defines how and whether image filters should be rasterised when rendering the image.</param>
        /// <param name="useStyles">If this is <see langword="false"/>, presentation attributes are set as attributes on SVG elements. If this is <see langword="true"/>, CSS classes are used to set presentation attributes.</param>
        public static void SaveAsSVG(this Page page, string fileName, TextOptions textOption = TextOptions.SubsetFonts, Dictionary<string, string> linkDestinations = null, FilterOption filterOption = default, bool useStyles = false)
        {
            using (FileStream sr = new FileStream(fileName, FileMode.Create))
            {
                page.SaveAsSVG(sr, textOption, linkDestinations, filterOption, useStyles);
            }
        }

        /// <summary>
        /// Defines whether the used fonts should be included in the file.
        /// </summary>
        public enum TextOptions
        {
            /// <summary>
            /// Embeds the full font files.
            /// </summary>
            EmbedFonts,

            /// <summary>
            /// Embeds subsetted font files containing only the glyphs for the characters that have been used.
            /// </summary>
            SubsetFonts,

            /// <summary>
            /// Does not embed any font file and converts all text items into paths.
            /// </summary>
            ConvertIntoPaths,

            /// <summary>
            /// Does not embed any font file, but still encodes text items as such.
            /// </summary>
            DoNotEmbed
        }

        /// <summary>
        /// Determines how and whether image filters are rasterised.
        /// </summary>
        public class FilterOption
        {
            /// <summary>
            /// Defines whether image filters should be rasterised or not.
            /// </summary>
            public enum FilterOperations
            {
                /// <summary>
                /// Image filters will always be rasterised.
                /// </summary>
                RasteriseAll,

                /// <summary>
                /// Image filters will only be rasterised if they are not supported natively by the output file format.
                /// </summary>
                RasteriseIfNecessary,

                /// <summary>
                /// Image filters will never be rasterised; for filters that are not supported, the filter will be ignored.
                /// </summary>
                NeverRasteriseAndIgnore,

                /// <summary>
                /// Image filters will never be rasterised; if an image should be drawn with an unsupported filter, the image will not be drawn at all.
                /// </summary>
                NeverRasteriseAndSkip,

                /// <summary>
                /// All image filters (supported and unsupported) will be ignored.
                /// </summary>
                IgnoreAll,

                /// <summary>
                /// All the images that should be drawn with a filter will be ignored.
                /// </summary>
                SkipAll
            }

            /// <summary>
            /// Defines whether image filters should be rasterised or not.
            /// </summary>
            public FilterOperations Operation { get; } = FilterOperations.RasteriseIfNecessary;

            /// <summary>
            /// The resolution that will be used to rasterise image filters. Depending on the value of <see cref="RasterisationResolutionRelative"/>, this can either be an absolute resolution (i.e. a size in pixel), or a scale factor that is applied to the image size in graphics units.
            /// </summary>
            public double RasterisationResolution { get; } = 1;

            /// <summary>
            /// Determines whether the value of <see cref="RasterisationResolution"/> is absolute (i.e. a size in pixel), or relative (i.e. a scale factor that is applied to the image size in graphics units).
            /// </summary>
            public bool RasterisationResolutionRelative { get; } = true;

            /// <summary>
            /// The default options for image filter rasterisation.
            /// </summary>
            public static FilterOption Default = new FilterOption(FilterOperations.RasteriseIfNecessary, 1, true);

            /// <summary>
            /// Create a new <see cref="FilterOption"/> object.
            /// </summary>
            /// <param name="operation">Defines whether image filters should be rasterised or not.</param>
            /// <param name="rasterisationResolution">The resolution that will be used to rasterise image filters. Depending on the value of <see cref="RasterisationResolutionRelative"/>, this can either be an absolute resolution (i.e. a size in pixel), or a scale factor that is applied to the image size in graphics units.</param>
            /// <param name="rasterisationResolutionRelative">Determines whether the value of <see cref="RasterisationResolution"/> is absolute (i.e. a size in pixel), or relative (i.e. a scale factor that is applied to the image size in graphics units).</param>
            public FilterOption(FilterOperations operation, double rasterisationResolution, bool rasterisationResolutionRelative)
            {
                this.Operation = operation;
                this.RasterisationResolution = rasterisationResolution;
                this.RasterisationResolutionRelative = rasterisationResolutionRelative;
            }
        }

        /// <summary>
        /// Render the page to an SVG stream.
        /// </summary>
        /// <param name="page">The <see cref="Page"/> to render.</param>
        /// <param name="stream">The stream to which the SVG data will be written.</param>
        /// <param name="textOption">Defines whether the used fonts should be included in the file.</param>
        /// <param name="linkDestinations">A dictionary associating element tags to link targets. If this is provided, objects that have been drawn with a tag contained in the dictionary will become hyperlink to the destination specified in the dictionary. If the destination starts with a hash (#), it is interpreted as the tag of another object in the current document; otherwise, it is interpreted as an external URI.</param>
        /// <param name="filterOption">Defines how and whether image filters should be rasterised when rendering the image.</param>
        /// <param name="useStyles">If this is <see langword="false"/>, presentation attributes are set as attributes on SVG elements. If this is <see langword="true"/>, CSS classes are used to set presentation attributes.</param>
        public static void SaveAsSVG(this Page page, Stream stream, TextOptions textOption = TextOptions.SubsetFonts, Dictionary<string, string> linkDestinations = null, FilterOption filterOption = default, bool useStyles = false)
        {
            if (linkDestinations == null)
            {
                linkDestinations = new Dictionary<string, string>();
            }

            if (filterOption == null)
            {
                filterOption = FilterOption.Default;
            }

            bool textToPaths = textOption == TextOptions.ConvertIntoPaths;

            SVGContext ctx = new SVGContext(page.Width, page.Height, textToPaths, textOption, linkDestinations, filterOption, useStyles);

            ctx.Rectangle(0, 0, page.Width, page.Height);
            ctx.SetFillStyle(page.Background);
            ctx.Fill();
            ctx.SetFillStyle((0, 0, 0, 1));

            page.Graphics.CopyToIGraphicsContext(ctx);

            if (!textToPaths && textOption != TextOptions.DoNotEmbed)
            {
                bool subsetFonts = textOption == TextOptions.SubsetFonts;

                StringBuilder cssFonts = new StringBuilder();

                Dictionary<string, string> newFontFamilies = new Dictionary<string, string>();

                foreach (KeyValuePair<string, FontFamily> kvp in ctx.UsedFontFamilies)
                {
                    TrueTypeFile subsettedFont;

                    if (subsetFonts)
                    {
                        newFontFamilies[kvp.Key] = kvp.Value.TrueTypeFile.GetFontFamilyName() + "-" + Guid.NewGuid().ToString();
                        subsettedFont = kvp.Value.TrueTypeFile.SubsetFont(new string(ctx.UsedChars[kvp.Key].ToArray()));
                    }
                    else
                    {
                        newFontFamilies[kvp.Key] = kvp.Value.TrueTypeFile.GetFontName();
                        subsettedFont = kvp.Value.TrueTypeFile;
                    }

                    byte[] fontBytes;

                    using (MemoryStream fontStream = new MemoryStream((int)subsettedFont.FontStream.Length))
                    {
                        subsettedFont.FontStream.Seek(0, SeekOrigin.Begin);
                        subsettedFont.FontStream.CopyTo(fontStream);

                        fontBytes = fontStream.ToArray();
                    }


                    cssFonts.Append("\n    @font-face\n    {\n      font-family: \"" + newFontFamilies[kvp.Key] + "\";\n      src: url(\"data:font/ttf;charset=utf-8;base64,");
                    cssFonts.Append(Convert.ToBase64String(fontBytes));
                    cssFonts.Append("\");\n    }\n  ");
                }

                XmlElement style = ctx.Document.CreateElement("style", SVGContext.SVGNamespace);
                style.InnerText = cssFonts.ToString();

                XmlNode svgElement = ctx.Document.GetElementsByTagName("svg")[0];

                svgElement.InsertBefore(style, svgElement.FirstChild);

                HashSet<string> updatedClasses = new HashSet<string>();

                foreach (XmlNode text in ctx.Document.GetElementsByTagName("text"))
                {
                    string fontFamily;

                    if (!useStyles)
                    {
                        fontFamily = text.Attributes["font-family"].Value;
                    }
                    else
                    {
                        fontFamily = ctx.Styles[text.Attributes["class"].Value]["font-family"];
                    }

                    if (!useStyles || updatedClasses.Add(text.Attributes["class"].Value))
                    {

                        string fallbackFontFamily = "";

                        switch (fontFamily)
                        {
                            case "Helvetica":
                            case "Helvetica-Bold":
                            case "Helvetica-Oblique":
                            case "Helvetica-BoldOblique":
                                fallbackFontFamily = "sans-serif";
                                break;

                            case "Times-Roman":
                            case "Times-Bold":
                            case "Times-Italic":
                            case "Times-BoldItalic":
                                fallbackFontFamily = "serif";
                                break;

                            case "Courier":
                            case "Courier-Bold":
                            case "Courier-Oblique":
                            case "Courier-BoldOblique":
                                fallbackFontFamily = "monospace";
                                break;

                            default:
                                if (ctx.UsedFontFamilies[fontFamily].TrueTypeFile.IsFixedPitch())
                                {
                                    fallbackFontFamily = "monospace";
                                }
                                else if (ctx.UsedFontFamilies[fontFamily].TrueTypeFile.IsScript() || ctx.UsedFontFamilies[fontFamily].TrueTypeFile.IsSerif())
                                {
                                    fallbackFontFamily = "serif";
                                }
                                else
                                {
                                    fallbackFontFamily = "sans-serif";
                                }
                                break;
                        }

                        if (!useStyles)
                        {
                            if (!string.IsNullOrEmpty(fallbackFontFamily))
                            {
                                ((XmlElement)text).SetAttribute("font-family", newFontFamilies[fontFamily] + ", " + fallbackFontFamily);
                            }
                            else
                            {
                                ((XmlElement)text).SetAttribute("font-family", newFontFamilies[fontFamily]);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(fallbackFontFamily))
                            {
                                ctx.Styles[text.Attributes["class"].Value]["font-family"] = newFontFamilies[fontFamily] + ", " + fallbackFontFamily;
                            }
                            else
                            {
                                ctx.Styles[text.Attributes["class"].Value]["font-family"] = newFontFamilies[fontFamily];
                            }
                        }
                    }
                }
            }
            else if (!textToPaths && textOption == TextOptions.DoNotEmbed)
            {
                HashSet<string> updatedClasses = new HashSet<string>();

                foreach (XmlNode text in ctx.Document.GetElementsByTagName("text"))
                {
                    string fontFamily;

                    if (!useStyles)
                    {
                        fontFamily = text.Attributes["font-family"].Value;
                    }
                    else
                    {
                        fontFamily = ctx.Styles[text.Attributes["class"].Value]["font-family"];
                    }

                    if (!useStyles || updatedClasses.Add(text.Attributes["class"].Value))
                    {

                        string newFontFamily = ctx.UsedFontFamilies[fontFamily].TrueTypeFile.GetFontFamilyName();

                        switch (fontFamily)
                        {
                            case "Helvetica":
                            case "Helvetica-Bold":
                            case "Helvetica-Oblique":
                            case "Helvetica-BoldOblique":
                                newFontFamily = newFontFamily + ", sans-serif";
                                break;

                            case "Times-Roman":
                            case "Times-Bold":
                            case "Times-Italic":
                            case "Times-BoldItalic":
                                newFontFamily = newFontFamily + ", serif";
                                break;

                            case "Courier":
                            case "Courier-Bold":
                            case "Courier-Oblique":
                            case "Courier-BoldOblique":
                                newFontFamily = newFontFamily + ", monospace";
                                break;

                            default:
                                if (ctx.UsedFontFamilies[fontFamily].TrueTypeFile.IsFixedPitch())
                                {
                                    newFontFamily = newFontFamily + ", monospace";
                                }
                                else if (ctx.UsedFontFamilies[fontFamily].TrueTypeFile.IsScript())
                                {
                                    newFontFamily = newFontFamily + ", cursive";
                                }
                                else if (ctx.UsedFontFamilies[fontFamily].TrueTypeFile.IsSerif())
                                {
                                    newFontFamily = newFontFamily + ", serif";
                                }
                                else
                                {
                                    newFontFamily = newFontFamily + ", sans-serif";
                                }
                                break;
                        }

                        if (!useStyles)
                        {
                            ((XmlElement)text).SetAttribute("font-family", newFontFamily);
                        }
                        else
                        {
                            ctx.Styles[text.Attributes["class"].Value]["font-family"] = newFontFamily;
                        }
                    }
                }
            }

            if (useStyles)
            {
                StringBuilder classStyle = new StringBuilder();

                if (ctx.Styles.Count > 0)
                {
                    classStyle.Append("\n  ");
                }

                foreach (KeyValuePair<string, Dictionary<string, string>> style in ctx.Styles)
                {
                    classStyle.Append("  .");
                    classStyle.Append(style.Key);
                    classStyle.Append("\n");
                    classStyle.Append("    {\n");

                    foreach (KeyValuePair<string, string> kvp in style.Value)
                    {
                        classStyle.Append("      ");
                        classStyle.Append(kvp.Key);
                        classStyle.Append(": ");
                        classStyle.Append(kvp.Value);
                        classStyle.Append(";\n");
                    }

                    classStyle.Append("    }\n  ");
                }


                XmlElement styleElement = ctx.Document.CreateElement("style", SVGContext.SVGNamespace);
                styleElement.InnerText = classStyle.ToString();

                XmlNode svgElement = ctx.Document.GetElementsByTagName("svg")[0];

                svgElement.InsertBefore(styleElement, svgElement.FirstChild);
            }



            ctx.Document.DocumentElement.SetAttribute("style", "font-synthesis: none;");

            WriteXMLToStream(ctx.Document.DocumentElement, stream);

            //ctx.Document.Save(stream);
        }

        internal static XmlElement ToLinearGradient(this LinearGradientBrush brush, XmlDocument document, string gradientId)
        {
            XmlElement gradient = document.CreateElement("linearGradient", SVGContext.SVGNamespace);

            gradient.SetAttribute("id", gradientId);

            gradient.SetAttribute("gradientUnits", "userSpaceOnUse");

            gradient.SetAttribute("x1", brush.StartPoint.X.ToString(System.Globalization.CultureInfo.InvariantCulture));
            gradient.SetAttribute("y1", brush.StartPoint.Y.ToString(System.Globalization.CultureInfo.InvariantCulture));
            gradient.SetAttribute("x2", brush.EndPoint.X.ToString(System.Globalization.CultureInfo.InvariantCulture));
            gradient.SetAttribute("y2", brush.EndPoint.Y.ToString(System.Globalization.CultureInfo.InvariantCulture));

            foreach (GradientStop stop in brush.GradientStops)
            {
                XmlElement gradientStop = document.CreateElement("stop", SVGContext.SVGNamespace);

                gradientStop.SetAttribute("offset", stop.Offset.ToString(System.Globalization.CultureInfo.InvariantCulture));
                gradientStop.SetAttribute("stop-color", stop.Colour.ToCSSString(false));
                gradientStop.SetAttribute("stop-opacity", stop.Colour.A.ToString(System.Globalization.CultureInfo.InvariantCulture));

                gradient.AppendChild(gradientStop);
            }

            return gradient;
        }

        internal static XmlElement ToRadialGradient(this RadialGradientBrush brush, XmlDocument document, string gradientId)
        {
            XmlElement gradient = document.CreateElement("radialGradient", SVGContext.SVGNamespace);

            gradient.SetAttribute("id", gradientId);

            gradient.SetAttribute("gradientUnits", "userSpaceOnUse");

            gradient.SetAttribute("cx", brush.Centre.X.ToString(System.Globalization.CultureInfo.InvariantCulture));
            gradient.SetAttribute("cy", brush.Centre.Y.ToString(System.Globalization.CultureInfo.InvariantCulture));
            gradient.SetAttribute("r", brush.Radius.ToString(System.Globalization.CultureInfo.InvariantCulture));
            gradient.SetAttribute("fx", brush.FocalPoint.X.ToString(System.Globalization.CultureInfo.InvariantCulture));
            gradient.SetAttribute("fy", brush.FocalPoint.Y.ToString(System.Globalization.CultureInfo.InvariantCulture));

            foreach (GradientStop stop in brush.GradientStops)
            {
                XmlElement gradientStop = document.CreateElement("stop", SVGContext.SVGNamespace);

                gradientStop.SetAttribute("offset", stop.Offset.ToString(System.Globalization.CultureInfo.InvariantCulture));
                gradientStop.SetAttribute("stop-color", stop.Colour.ToCSSString(false));
                gradientStop.SetAttribute("stop-opacity", stop.Colour.A.ToString(System.Globalization.CultureInfo.InvariantCulture));

                gradient.AppendChild(gradientStop);
            }

            return gradient;
        }

        // Adapted from http://www.ericwhite.com/blog/2011/05/09/custom-formatting-of-xml-using-linq-to-xml-2/
        private static void WriteStartElement(XmlWriter writer, XmlElement e)
        {
            writer.WriteStartElement(e.Prefix, e.LocalName, e.NamespaceURI);

            foreach (XmlAttribute a in e.Attributes)
            {
                writer.WriteAttributeString(a.Prefix, a.LocalName, a.NamespaceURI, a.Value);
            }
        }

        private static void WriteElement(XmlWriter writer, XmlElement e)
        {
            if (e.Name == "text")
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = false;
                settings.OmitXmlDeclaration = true;
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;

                WriteStartElement(writer, e);

                StringBuilder sb = new StringBuilder();

                using (XmlWriter newWriter = XmlWriter.Create(sb, settings))
                {
                    foreach (XmlNode n in e.ChildNodes)
                    {
                        n.WriteTo(newWriter);
                    }
                }

                writer.WriteRaw(sb.ToString().Replace(" xmlns=\"http://www.w3.org/2000/svg\">", ">"));

                writer.WriteEndElement();
            }
            else
            {
                WriteStartElement(writer, e);
                foreach (XmlNode n in e.ChildNodes)
                {
                    if (n is XmlElement element)
                    {
                        WriteElement(writer, element);
                    }
                    else
                    {
                        n.WriteTo(writer);
                    }
                }
                writer.WriteEndElement();
            }
        }

        private static void WriteXMLToStream(XmlElement element, Stream output)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(output, settings))
            {
                WriteElement(writer, element);
            }
        }
    }
}

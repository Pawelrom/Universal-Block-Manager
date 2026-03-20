using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using SkiaSharp;
using BitmapToVector;

namespace UniversalBlockManager.Shared.Services
{
    public class VectorizationService
    {
        public string TraceImageToSvgPath(string imagePath, double threshold = 0.5)
        {
            using (var stream = File.OpenRead(imagePath))
            using (var bitmap = SKBitmap.Decode(stream))
            {
                return TraceBitmapToSvgPath(bitmap, threshold);
            }
        }

        public string TraceBitmapToSvgPath(SKBitmap bitmap, double threshold = 0.5)
        {
            var potraceBitmap = PotraceBitmap.Create(bitmap.Width, bitmap.Height);
            FillPotraceBitmap(potraceBitmap, bitmap, (byte)(threshold * 255));

            var param = new PotraceParam { TurdSize = 5, AlphaMax = 1.0, OptiCurve = true }; 
            var state = BitmapToVector.Potrace.Trace(param, potraceBitmap);

            return ConvertPotraceToSvgPath(state);
        }

        private void FillPotraceBitmap(PotraceBitmap potraceBitmap, SKBitmap bitmap, byte threshold)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var color = bitmap.GetPixel(x, y);
                    byte gray = (byte)(0.299 * color.Red + 0.587 * color.Green + 0.114 * color.Blue);
                    
                    if (gray < threshold)
                    {
                        potraceBitmap.SetBlack(x, y);
                    }
                    else
                    {
                        potraceBitmap.SetWhite(x, y);
                    }
                }
            }
        }

        private string ConvertPotraceToSvgPath(PotraceState state)
        {
            var sb = new StringBuilder();
            var path = state.Plist;
            
            while (path != null)
            {
                ConvertPathRecursive(path, sb);
                path = path.Next;
            }

            return sb.ToString().Trim();
        }

        private void ConvertPathRecursive(PotracePath path, StringBuilder sb)
        {
            var curve = path.Curve;
            if (curve.N > 0)
            {
                // Each curve is a loop. The end point of segment i is always in curve.C[i][2].
                // The start point of the whole curve is the end point of the last segment.
                var startPoint = curve.C[curve.N - 1][2];
                sb.Append(string.Format(CultureInfo.InvariantCulture, "M {0:F2} {1:F2} ", startPoint.X, startPoint.Y));

                for (int i = 0; i < curve.N; i++)
                {
                    int tag = curve.Tag[i];
                    if (tag == 2) // POTRACE_CORNER
                    {
                        // In this port, Corner uses C[i][1] as corner vertex and C[i][2] as end point.
                        // C[i][0] is unused (0,0).
                        sb.Append(string.Format(CultureInfo.InvariantCulture, "L {0:F2} {1:F2} ", curve.C[i][1].X, curve.C[i][1].Y));
                        sb.Append(string.Format(CultureInfo.InvariantCulture, "L {0:F2} {1:F2} ", curve.C[i][2].X, curve.C[i][2].Y));
                    }
                    else if (tag == 1) // POTRACE_CURVETO
                    {
                        // Curveto uses C[i][0], C[i][1] as control points and C[i][2] as end point.
                        sb.Append(string.Format(CultureInfo.InvariantCulture, "C {0:F2} {1:F2} {2:F2} {3:F2} {4:F2} {5:F2} ", 
                            curve.C[i][0].X, curve.C[i][0].Y, 
                            curve.C[i][1].X, curve.C[i][1].Y, 
                            curve.C[i][2].X, curve.C[i][2].Y));
                    }
                }
                sb.Append("Z ");
            }

            var child = path.ChildList;
            while (child != null)
            {
                ConvertPathRecursive(child, sb);
                child = child.Sibling;
            }
        }
    }
}

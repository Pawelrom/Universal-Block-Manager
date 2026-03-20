using System;
using System.Collections.Generic;
using System.Linq;
using BitmapToVector;

class Program
{
    static void Main()
    {
        try
        {
            // Simple L shape to force a corner
            var bm = PotraceBitmap.Create(10, 10);
            for(int i=2; i<8; i++) {
                bm.SetBlack(2, i);
                bm.SetBlack(i, 8);
            }

            var param = new PotraceParam { TurdSize = 0, AlphaMax = 0 }; // AlphaMax=0 forces corners
            var state = Potrace.Trace(param, bm);

            var path = state.Plist;
            while (path != null)
            {
                var curve = path.Curve;
                Console.WriteLine($"N: {curve.N}");
                for (int i = 0; i < curve.N; i++)
                {
                    Console.WriteLine($"  Segment {i} Tag: {curve.Tag[i]}");
                    for (int c = 0; c < 3; c++)
                    {
                        if (curve.C[i][c] != null)
                            Console.WriteLine($"    C[{i}][{c}]: {curve.C[i][c].X}, {curve.C[i][c].Y}");
                    }
                }
                path = path.Next;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

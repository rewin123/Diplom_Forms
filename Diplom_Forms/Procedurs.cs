using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Video.FFMPEG;

namespace Diplom_Forms
{
    static class Procedurs
    {
        public static Vector3[,] Medium(string path, int steps = 200)
        {
            VideoFileReader reader = new VideoFileReader();
            reader.Open(path);

            Vector3[,] data = new Vector3[reader.Width, reader.Height];

            Vector3[,] temp = new Vector3[reader.Width, reader.Height];

            data.ForEach(() => new Vector3());
            temp.ForEach(() => new Vector3());

            for(int i = 0;i < steps;i++)
            {
                using (Bitmap img = reader.ReadVideoFrame(i * (int)reader.FrameCount / steps))
                {
                    temp.WriteRGB(img);
                    data.ForEach(temp, (m, v) => m.Add(v));
                }
            }

            data.ForEach((m) => m.Divide(steps));

            reader.Close();

            return data;
        }

        public static void MorfSubtract(Morf morf, List<Vector3> avrs, Vector3[,] img, float[,] result)
        {
            int width = img.GetLength(0);
            int height = img.GetLength(1);

            for(int i = 0;i < avrs.Count;i++)
            {
                Vector3 av = avrs[i];
                MorfRegion region = morf.regions[i];
                region.ForEach(img, result, (v) => (v - av).Magnitude());
            }
            
        }

        public static void BlockSum(float[,] input, float[,] output, int size)
        {
            int width = input.GetLength(0);
            int height = input.GetLength(1);
            for(int x = 0;x < width;x++)
            {
                for(int y = 0;y < height;y++)
                {
                    output[x / size * size, y / size * size] += input[x, y];
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    output[x,y] = output[x / size * size, y / size * size];
                }
            }
        }
    }
}

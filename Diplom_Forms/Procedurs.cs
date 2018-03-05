using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Video.FFMPEG;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Diplom_Forms
{
    static class Procedurs
    {
        public static Vector3[,] Medium(string path, int steps = 100)
        {
            VideoFileReader reader = new VideoFileReader();
            reader.Open(path);

            Vector3[,] data = new Vector3[reader.Width, reader.Height];

            var temp = new Image<Rgb, float>(reader.Width,reader.Height);
            

            for(int i = 0;i < steps;i++)
            {
                using (Bitmap img = reader.ReadVideoFrame(i * (int)reader.FrameCount / steps))
                {
                    var arr = new Image<Rgb, float>(img);
                    temp.Accumulate(arr);
                    //temp.WriteRGB(img);
                    //data.ForEach(temp, (m, v) => m.Add(v));
                }
            }
            
            for(int x = 0;x < reader.Width;x++)
            {
                for(int y = 0;y < reader.Height;y++)
                {
                    Rgb rgb = temp[y, x];
                    data[x, y] = new Vector3((float)rgb.Red / steps / 255, (float)rgb.Green / steps / 255, (float)rgb.Blue / steps / 255);
                }
            }

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

        public static void BinaryThiken(this byte[,] arr, byte[,] target)
        {
            int dwidth = arr.GetLength(0) - 1;
            int dheight = arr.GetLength(1) - 1;
            for (int x = 1; x < dwidth; x++)
            {
                for(int y = 1;y < dheight;y++)
                {
                    target[x, y] =(byte)(arr[x, y] | arr[x - 1, y] | arr[x + 1, y] | arr[x, y - 1] | arr[x, y + 1]);
                }
            }
        }
    }
}

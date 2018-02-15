using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Diplom_Forms
{
    public static class ImageDataConverter
    {
        public static float[,] GetGrey(Bitmap input, int startX, int startY, int w, int h, int offset, int scansize)
        {
            const int PixelWidth = 3;
            const PixelFormat PixelFormat = PixelFormat.Format32bppArgb;
            const float max = 3 * 255;

            float[,] rgbArray = new float[w, h];

            Bitmap image = input.Clone(new Rectangle(0, 0, input.Width, input.Height), PixelFormat);

            // En garde!
            if (image == null) throw new ArgumentNullException("image");
            if (rgbArray == null) throw new ArgumentNullException("rgbArray");
            if (startX < 0 || startX + w > image.Width) throw new ArgumentOutOfRangeException("startX");
            if (startY < 0 || startY + h > image.Height) throw new ArgumentOutOfRangeException("startY");
            if (w < 0 || w > scansize || w > image.Width) throw new ArgumentOutOfRangeException("w");
            if (h < 0 || (rgbArray.Length < offset + h * scansize) || h > image.Height) throw new ArgumentOutOfRangeException("h");

            BitmapData data = image.LockBits(new Rectangle(startX, startY, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat);
            
            try
            {
                byte[] pixelData = new byte[3];
                int index = 0;
                for (int y = 0;y < h;y++)
                {
                    for(int x = 0;x < w;x++)
                    {
                        Marshal.ReadByte(data.Scan0, index); //alpha
                        index++;

                        byte r = Marshal.ReadByte(data.Scan0,index);
                        index++;
                        byte g = Marshal.ReadByte(data.Scan0,index);
                        index++;
                        byte b = Marshal.ReadByte(data.Scan0,index);
                        index++;

                        rgbArray[x, y] = (r + g + b) / max;
                    }
                }
                
            }
            finally
            {
                image.UnlockBits(data);
            }

            return rgbArray;
        }

        public static Vector3[,] GetRGB(Bitmap input, int startX, int startY, int w, int h, int offset, int scansize)
        {
            const int PixelWidth = 3;
            const PixelFormat PixelFormat = PixelFormat.Format32bppArgb;
            const float max = 3 * 255;

            Vector3[,] rgbArray = new Vector3[w, h];

            Bitmap image = input.Clone(new Rectangle(0, 0, input.Width, input.Height), PixelFormat);

            BitmapData data = image.LockBits(new Rectangle(startX, startY, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat);

            try
            {
                //byte[] pixelData = new byte[3];
                int index = 0;
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        byte b = Marshal.ReadByte(data.Scan0, index);
                        index++;
                        byte g = Marshal.ReadByte(data.Scan0, index);
                        index++;
                        byte r = Marshal.ReadByte(data.Scan0, index);
                        index++;

                        byte a = Marshal.ReadByte(data.Scan0, index); //alpha
                        index++;

                        rgbArray[x, y] = new Vector3(r / 255.0f, g / 255.0f, b / 255.0f);
                        rgbArray[x, y].alpha = a / 255.0f;
                    }
                }

            }
            finally
            {
                image.UnlockBits(data);
            }

            return rgbArray;
        }

        public static void WriteRGB(this Vector3[,] rgbArray, Bitmap input)
        {
            const PixelFormat PixelFormat = PixelFormat.Format32bppArgb;

            Bitmap image = input.Clone(new Rectangle(0, 0, input.Width, input.Height), PixelFormat);

            int w = rgbArray.GetLength(0);
            int h = rgbArray.GetLength(1);

            BitmapData data = image.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat);

            try
            {
                //byte[] pixelData = new byte[3];
                int index = 0;
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        byte b = Marshal.ReadByte(data.Scan0, index);
                        index++;
                        byte g = Marshal.ReadByte(data.Scan0, index);
                        index++;
                        byte r = Marshal.ReadByte(data.Scan0, index);
                        index++;

                        byte a = Marshal.ReadByte(data.Scan0, index); //alpha
                        index++;

                        rgbArray[x, y].x = r / 255.0f;
                        rgbArray[x, y].y = g / 255.0f;
                        rgbArray[x, y].z = b / 255.0f;
                        rgbArray[x, y].alpha = a / 255.0f;
                    }
                }

            }
            finally
            {
                image.UnlockBits(data);
            }
            
        }

        public static Vector3[,] GetRGB(this Bitmap input)
        {
            return GetRGB(input, 0, 0, input.Width, input.Height, 0, input.Width);
        }

        public static Bitmap GetImage(this float[,] array)
        {

            int width = array.GetLength(0);
            int height = array.GetLength(1);
            Bitmap map = new Bitmap(width, height);

            BitmapData data = map.LockBits(new Rectangle(0, 0, map.Width, map.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* byte_data = (byte*)data.Scan0.ToPointer();
                int pos;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pos = (y * width + x) * 4;
                        byte_data[pos] = (byte)(array[x, y] * 255);
                        pos++;
                        byte_data[pos] = (byte)(array[x, y] * 255);
                        pos++;
                        byte_data[pos] = (byte)(array[x, y] * 255);
                        pos++;
                        byte_data[pos] = 255;
                        pos++;

                    }
                }
            }

            map.UnlockBits(data);

            return map;
        }

        public static Bitmap GetImage(this Vector3[,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            Bitmap map = new Bitmap(width, height);

            BitmapData data = map.LockBits(new Rectangle(0, 0, map.Width, map.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* byte_data = (byte*)data.Scan0.ToPointer();
                int pos;
                for(int y = 0;y < height;y++)
                {
                    for(int x = 0;x < width;x++)
                    {
                        pos = (y * width + x) * 4;
                        byte_data[pos] = (byte)(array[x, y].z * 255);
                        pos++;
                        byte_data[pos] = (byte)(array[x, y].y * 255);
                        pos++;
                        byte_data[pos] = (byte)(array[x, y].x * 255);
                        pos++;
                        byte_data[pos] = 255;
                        pos++;

                    }
                }
            }

            map.UnlockBits(data);

            return map;
        }



        public static void RegMax(float[,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);

            float max = 0;
            for(int x = 0;x < width;x++)
            {
                for(int y = 0;y < height;y++)
                {
                    max = Math.Max(array[x, y],max);
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    array[x, y] /= max;
                }
            }
        }

        public static void RegMaximum(this float[,] array)
        {
            RegMax(array);
        }

        public static void LevelBinar(this float[,] array, float level)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            for(int x = 0;x < width;x++)
            {
                for(int y = 0;y < height;y++)
                {
                    array[x, y] = array[x, y] >= level ? 1 : 0;
                }
            }
        }

        public static void Binary(this float[] array, float level)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = array[i] > level ? 1 : 0;
        }

        public static Vector3[,] HSV(this Vector3[,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            Vector3[,] hsv = new Vector3[width, height];

            for(int x = 0;x < width;x++)
            {
                for(int y = 0;y < height;y++)
                {
                    Vector3 now = new Vector3();

                    Vector3 input = array[x, y];
                    float max = Math.Max(input.x, Math.Max(input.y, input.z));
                    float min = Math.Min(input.x, Math.Min(input.y, input.z));
                    if (min == max)
                        now.x = 0;
                    else if(max == input.x && input.y >= input.z)
                    {
                        now.x = 60 * (input.y - input.z) / (max - min);
                    }
                    else if(max == input.x && input.y < input.z)
                    {
                        now.x = 60 * (input.y - input.z) / (max - min) + 360;
                    }
                    else if(max == input.y)
                    {
                        now.x = 60 * (input.z - input.x) / (max - min) + 120;
                    }
                    else if(max == input.z)
                    {
                        now.x = 60 * (input.x - input.y) / (max - min) + 240;
                    }

                    if (max != 0)
                        now.y = 1 - min / max;
                    now.z = max;

                    hsv[x, y] = now;
                }
            }

            return hsv;
        }

        public static Vector3 HSV(this Vector3 input)
        {
            Vector3 now = new Vector3();
            
            float max = Math.Max(input.x, Math.Max(input.y, input.z));
            float min = Math.Min(input.x, Math.Min(input.y, input.z));
            if (min == max)
                now.x = 0;
            else if (max == input.x && input.y >= input.z)
            {
                now.x = 60 * (input.y - input.z) / (max - min);
            }
            else if (max == input.x && input.y < input.z)
            {
                now.x = 60 * (input.y - input.z) / (max - min) + 360;
            }
            else if (max == input.y)
            {
                now.x = 60 * (input.z - input.x) / (max - min) + 120;
            }
            else if (max == input.z)
            {
                now.x = 60 * (input.x - input.y) / (max - min) + 240;
            }

            if (max != 0)
                now.y = 1 - min / max;
            now.z = max;

            return now;
        }

        public static Vector3 RGB(this Vector3 input)
        {

            int Hi = (int)(input.x / 60) % 6;
            float Vmin = ((1 - input.y) * input.z) / 1;
            float a = (input.z - Vmin) * ((int)input.x % 60) / 60.0f;
            float Vinc = Vmin + a;
            float Vdec = input.z - a;

            switch (Hi)
            {
                case 0:
                    return new Vector3(input.z, Vinc, Vmin);
                case 1:
                    return new Vector3(Vdec, input.z, Vmin);
                case 2:
                    return new Vector3(Vmin, input.z, Vinc);
                case 3:
                    return new Vector3(Vmin, Vdec, input.z);
                case 4:
                    return new Vector3(Vinc, Vmin, input.z);
                case 5:
                    return new Vector3(input.z, Vmin, Vdec);
                default:
                    return new Vector3(0, 0, 0);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom_Forms
{
    public class MorfRegion
    {
        List<int> xpos = new List<int>();
        List<int> ypos = new List<int>();

        public void AddPoint(int x, int y)
        {
            xpos.Add(x);
            ypos.Add(y);
        }

        public static MorfRegion FromRect(int x0, int y0, int width, int height)
        {
            MorfRegion m = new MorfRegion();
            for(int x = 0;x < width;x++)
            {
                for(int y = 0;y < height;y++)
                {
                    m.xpos.Add(x + x0);
                    m.ypos.Add(y + y0);
                }
            }
            return m;
        }

        public float GetAverage(float[,] mass)
        {
            double average = 0;
            int count = xpos.Count;
            for(int i = 0;i < count;i++)
            {
                average += mass[xpos[i], ypos[i]];
            }
            average /= count;
            return (float)average;
        }

        public float GetSqrError(float[,] mass)
        {
            float average = GetAverage(mass);
            int count = xpos.Count;
            double error = 0;
            float now = 0;

            if (count == 1)
                return 0;

            for (int i = 0; i < count; i++)
            {
                now = mass[xpos[i], ypos[i]] - average;
                error += now * now;
            }
            error = Math.Sqrt(error / count / (count - 1));
            return (float)error;
        }

        public Vector3 GetAverage(Vector3[,] mass)
        {
            double x = 0;
            double y = 0;
            double z = 0;
            int count = xpos.Count;
            for (int i = 0; i < count; i++)
            {
                Vector3 v = mass[xpos[i], ypos[i]];
                x += v.x;
                y += v.y;
                z += v.z;
            }
            Vector3 average = new Vector3((float)x, (float)y, (float)z);
            if (count > 0)
            {
                average.Divide(count);
            }
            return average;
        }

        public void Fill(Vector3 v, Vector3[,] map)
        {
            for(int i = 0;i < xpos.Count;i++)
            {
                map[xpos[i], ypos[i]] = new Vector3(v);
            }
        }

        public void Fill<T>(T val, T[,] map)
        {
            for (int i = 0; i < xpos.Count; i++)
            {
                map[xpos[i], ypos[i]] = val;
            }
        }

        public float GetSqrError(Vector3[,] mass)
        {
            Vector3 average = GetAverage(mass);
            int count = xpos.Count;
            double error = 0;

            if (count == 1)
                return 0;
            
            for (int i = 0; i < count; i++)
            {

                error += (mass[xpos[i], ypos[i]] - average).Magnitude();
                //error = Math.Max((mass[xpos[i], ypos[i]] - average).Magnitude(),error);
            }
            error = error / count;
            return (float)error;
        }

        public int Size
        {
            get
            {
                return xpos.Count;
            }
        }

        public void Union(MorfRegion region)
        {
            xpos.AddRange(region.xpos);
            ypos.AddRange(region.ypos);
        }

        public void Clear()
        {
            xpos.Clear();
            ypos.Clear();
        }

        public void ForEach<T,R>(T[,] data, R[,] result, Func<T, R> func)
        {
            for(int i = 0;i < xpos.Count;i++)
            {
                result[xpos[i], ypos[i]] = func(data[xpos[i], ypos[i]]);
            }
        }

        public Rectangle GetSize()
        {
            int xmin = xpos.Min();
            int ymin = ypos.Min();
            int xmax = xpos.Max();
            int ymax = ypos.Max();
            return new Rectangle(xmin, ymin, xmax - xmin, ymax - ymin);
        }

        //public List<Region> Split(byte[,] temp_arr)
        //{
        //    List<Region> regions = new List<Region>();
            

        //    int width = temp_arr.GetLength(0);
        //    int height = temp_arr.GetLength(1);
        //    for(int x = 0;x < width;x++)
        //    {
        //        for(int y = 0;y < height;y++)
        //        {
        //            temp_arr[x, y] = 0;
        //        }
        //    }
        //    Fill<byte>(1, temp_arr);

        //    int[,] indexes = new int[width, height];

        //    for(int x = 1;x < width;x++)
        //    {
        //        for(int y = 0;y < height;y++)
        //        {

        //        }
        //    }
        //}
    }
}

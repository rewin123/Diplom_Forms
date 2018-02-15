using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Diplom_Forms
{
    public class Vector3
    {
        public float alpha;
        public float x; //r
        public float y; //g
        public float z; //b

        public Vector3(float x = 0, float y = 0, float z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            
        }

        public Vector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public static Vector3 operator+(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public static Vector3 operator *(Vector3 v1, float val)
        {
            return new Vector3(v1.x * val, v1.y * val, v1.z * val);
        }

        public static Vector3 operator /(Vector3 v1, float val)
        {
            return new Vector3(v1.x / val, v1.y / val, v1.z / val);
        }

        public void Add(Vector3 v)
        {
            x += v.x;
            y += v.y;
            z += v.z;
        }

        public void Retract(Vector3 v)
        {
            x -= v.x;
            y -= v.y;
            z -= v.z;
        }

        public void Multiply(float val)
        {
            x *= val;
            y *= val;
            z *= val;
        }

        public void Divide(float val)
        {
            x /= val;
            y /= val;
            z /= val;
        }

        public Color GetColor()
        {
            return Color.FromArgb((int)(x * 255), (int)(y * 255), (int)(z * 255));
        }

        public SolidBrush GetBrush()
        {
            return new SolidBrush(GetColor());
        }

        public float Magnitude()
        {
            return x * x + y * y + z * z;
        }

        public override string ToString()
        {
            return x + " " + y + " " + z;
        }

        public void Normilize()
        {
            float amp = (float)Math.Sqrt(Magnitude());
            if (amp != 0)
            {
                x /= amp;
                y /= amp;
                z /= amp;
            }
        }

        public static Vector3[,] GetArray(float[,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            Vector3[,] map = new Vector3[width, height];
            float val;
            for(int x = 0;x < width;x++)
            {
                for(int y = 0;y < height;y++)
                {
                    val = array[x, y];
                    map[x, y] = new Vector3(val,val,val);
                }
            }
            return map;
        }

        public static Vector3[] GetArray(float[] array)
        {
            int width = array.GetLength(0);
            Vector3[] map = new Vector3[width];
            float val;
            for (int x = 0; x < width; x++)
            {
                    val = array[x];
                    map[x] = new Vector3(val, val, val);
            }
            return map;
        }

        public void ToZero()
        {
            x = 0;
            y = 0;
            z = 0;
        }
    }
}

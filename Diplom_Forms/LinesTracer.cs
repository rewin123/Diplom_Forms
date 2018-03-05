using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace Diplom_Forms
{
    class LinesTracer
    {
        List<Rectangle> cars = new List<Rectangle>();
        List<int> indexers = new List<int>();

        int global_indexer = 1;

        public void TrackAndWrite(List<Rectangle> new_cars, ref Bitmap map, float max_dr, float max_dsize = 0.2f)
        {
            int dx;
            int dy;
            float size;
            bool founded;
            for(int i = 0;i < cars.Count;i++)
            {
                founded = false;
                var car = cars[i];
                size = car.Width * car.Height;
                double rmin = double.MaxValue;
                int jbest = -1;
                for (int j = 0;j < new_cars.Count;j++)
                {
                    dx = car.X - new_cars[j].X;
                    dy = car.Y - new_cars[j].Y;
                    double r = Math.Sqrt(dx * dx + dy * dy);
                    if(r < max_dr && Math.Abs(1 - size / new_cars[j].Width / new_cars[j].Height) < max_dsize)
                    {
                        if (rmin > r)
                        {
                            founded = true;
                            jbest = j;
                            rmin = r;
                            //cars[i] = new_cars[j];
                            //new_cars.RemoveAt(j);
                        }
                    }
                }
                
                if(!founded)
                {
                    cars.RemoveAt(i);
                    indexers.RemoveAt(i);
                    i--;
                    continue;
                }
                else
                {
                    cars[i] = new_cars[jbest];
                    new_cars.RemoveAt(jbest);
                }
            }

            string path = "Data\\";

            for (int i = 0; i < new_cars.Count; i++)
            {
                global_indexer++;
                indexers.Add(global_indexer);
                cars.Add(new_cars[i]);

                if (!Directory.Exists(path + indexers.Last().ToString()))
                    Directory.CreateDirectory(path + indexers.Last().ToString());
            }

            for(int i = 0;i < cars.Count;i++)
            {
                map.Clone(cars[i], map.PixelFormat).Save(path + indexers[i] + "\\" + Directory.GetFiles(path + indexers[i].ToString()).Length + ".png");
            }
        }
    }
}

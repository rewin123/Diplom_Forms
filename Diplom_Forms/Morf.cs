using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom_Forms
{
    public class Morf
    {
        public List<MorfRegion> regions = new List<MorfRegion>();

        public Vector3[,] AverageMap(Vector3[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            Vector3[,] result = new Vector3[width, height];

            for(int i = 0;i < regions.Count;i++)
            {
                Vector3 avr = regions[i].GetAverage(map);
                regions[i].Fill(avr, result);
            }

            return result;
        }

        public Vector3[,] DistInAvr(Vector3[,] map, Vector3 color)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            Vector3[,] result = new Vector3[width, height];

            List<float> dist = new List<float>();

            for (int i = 0; i < regions.Count; i++)
            {
                Vector3 avr = regions[i].GetAverage(map);
                dist.Add((avr - color).Magnitude());
            }

            //int minIndex = 0;
            for (int i = 0; i < regions.Count; i++)
            {
                float val = 1 / (1 + dist[i]);
                val = val > 0.92f ? val : 0;
                regions[i].Fill(new Vector3(val,val,val), result);
                
            }

            //regions[minIndex].Fill(new Vector3(1, 1, 1), result);

            return result;
        }

        public Vector3[,] BestInAvr(Vector3[,] map, Vector3 color)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            Vector3[,] result = new Vector3[width, height];

            List<float> dist = new List<float>();

            for (int i = 0; i < regions.Count; i++)
            {
                Vector3 avr = regions[i].GetAverage(map);
                dist.Add((avr - color).Magnitude());
            }

            int minIndex = 0;
            for(int i = 0;i < regions.Count;i++)
            {
                regions[i].Fill(new Vector3(), result);
                if (dist[minIndex] > dist[i])
                    minIndex = i;
            }

            regions[minIndex].Fill(new Vector3(1, 1, 1), result);

            return result;
        }

        public static Morf GenerateKMean(Vector3[,] map, uint count)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            MorfRegion[,,] regions = new MorfRegion[count + 1, count + 1, count + 1];
            for(int x = 0;x < count + 1;x++)
            {
                for(int y = 0;y < count + 1;y++)
                {
                    for(int z = 0;z < count + 1;z++)
                    {
                        regions[x, y, z] = new MorfRegion();
                    }
                }
            }

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;

            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    minX = Math.Min(minX, map[x, y].x);
                    minY = Math.Min(minY, map[x, y].y);
                    minZ = Math.Min(minZ, map[x, y].z);

                    maxX = Math.Max(maxX, map[x, y].x);
                    maxY = Math.Max(maxY, map[x, y].y);
                    maxZ = Math.Max(maxZ, map[x, y].z);
                }
            }

            float kX = count / (maxX - minX);
            float kY = count / (maxY - minY);
            float kZ = count / (maxZ - minZ);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 v = map[x, y];
                    int xpos = (int)((v.x - minX) * kX);
                    int ypos = (int)((v.y - minY) * kY);
                    int zpos = (int)((v.z - minZ) * kZ);

                    regions[xpos, ypos, zpos].AddPoint(x, y);
                }
            }

            Morf morf = new Morf();
            
            for (int x = 0; x < count + 1; x++)
            {
                for (int y = 0; y < count + 1; y++)
                {
                    for (int z = 0; z < count + 1; z++)
                    {
                        morf.regions.Add(regions[x, y, z]);
                    }
                }
            }

            return morf;
        }

        public static Morf GenerateKMean(float[,] map, uint count)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            MorfRegion[] regions = new MorfRegion[count + 1];
            for (int x = 0; x < count + 1; x++)
            {
                regions[x] = new MorfRegion();
            }

            float minX = float.MaxValue;

            float maxX = float.MinValue;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    minX = Math.Min(minX, map[x, y]);

                    maxX = Math.Max(maxX, map[x, y]);
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int xpos = (int)((map[x, y] - minX) / (maxX - minX) * count);

                    regions[xpos].AddPoint(x, y);
                }
            }

            Morf morf = new Morf();

            for (int x = 0; x < count + 1; x++)
            {
                morf.regions.Add(regions[x]);
            }

            return morf;
        }


        public static Morf GenerateBinar(byte[,] map)
        {
            List<MorfRegion> regions = new List<MorfRegion>();
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            int[,] reg_map = new int[width, height];
            if(map[0,0] == 1)
            {
                MorfRegion morfRegion = new MorfRegion();
                morfRegion.AddPoint(0, 0);
                reg_map[0, 0] = 1;
                regions.Add(morfRegion);
            }
            for(int y = 1; y < height;y++)
            {
                if(map[0,y] == 1)
                {
                    if(reg_map[0, y - 1] != 0)
                    {
                        reg_map[0, y] = reg_map[0, y - 1];
                        regions[reg_map[0, y] - 1].AddPoint(0, y);
                    }
                    else
                    {
                        MorfRegion morfRegion = new MorfRegion();
                        morfRegion.AddPoint(0, y);
                        regions.Add(morfRegion);
                        reg_map[0, y] = regions.Count;
                    }
                }
            }

            for (int x = 1; x < width; x++)
            {
                if (map[x, 0] == 1)
                {
                    if (reg_map[x - 1, 0] != 0)
                    {
                        reg_map[x, 0] = reg_map[x - 1, 0];
                        regions[reg_map[x, 0] - 1].AddPoint(x, 0);
                    }
                    else
                    {
                        MorfRegion morfRegion = new MorfRegion();
                        morfRegion.AddPoint(x, 0);
                        regions.Add(morfRegion);
                        reg_map[x, 0] = regions.Count;
                    }
                }

                for (int y = 1; y < height; y++)
                {
                    if (map[x, y] == 1)
                    {
                        if (reg_map[x, y - 1] != 0)
                        {
                            if (reg_map[x - 1, y] != 0)
                            {
                                if (reg_map[x - 1, y] != reg_map[x, y - 1])
                                {
                                    #region Объединение множеств
                                    reg_map[x, y] = reg_map[x, y - 1];
                                    int old = reg_map[x - 1, y] - 1;
                                    regions[reg_map[x, y] - 1].AddPoint(x, y);
                                    regions[old].Fill<int>(reg_map[x, y], reg_map);
                                    regions[reg_map[x, y] - 1].Union(regions[old]);
                                    regions[old].Clear();
                                    #endregion
                                }
                                else
                                {
                                    reg_map[x, y] = reg_map[x, y - 1];
                                    regions[reg_map[x, y] - 1].AddPoint(x, y);
                                }
                            }
                            else
                            {
                                reg_map[x, y] = reg_map[x, y - 1];
                                regions[reg_map[x, y] - 1].AddPoint(x, y);
                            }
                        }
                        else if (reg_map[x - 1, y] != 0)
                        {
                            reg_map[x, y] = reg_map[x - 1, y];
                            regions[reg_map[x, y] - 1].AddPoint(x, y);
                        }
                        else
                        {
                            MorfRegion morfRegion = new MorfRegion();
                            morfRegion.AddPoint(x, y);
                            regions.Add(morfRegion);
                            reg_map[x, y] = regions.Count;
                        }
                    }
                }
            }

            Morf morf = new Morf();
            morf.regions = regions;
            morf.RemoveEmptyRegions();
            return morf;
        }

        public void RemoveEmptyRegions()
        {
            for(int i = 0;i < regions.Count;i++)
            {
                if(regions[i].Size == 0)
                {
                    regions.RemoveAt(i);
                    i--;
                }
            }
        }

        public MorfRegion BiggesRegion
        {
            get
            {
                int max_size = 0;
                int maxIndex = 0;
                for (int i = 0; i < regions.Count; i++)
                {
                    if (regions[i].Size > max_size)
                    {
                        max_size = regions[i].Size;
                        maxIndex = i;
                    }
                }
                return regions[maxIndex];
            }
        }

        public int BiggestRegionIndex
        {
            get
            {
                int max_size = 0;
                int maxIndex = 0;
                for (int i = 0; i < regions.Count; i++)
                {
                    if (regions[i].Size > max_size)
                    {
                        max_size = regions[i].Size;
                        maxIndex = i;
                    }
                }
                return maxIndex;
            }
        }

    }
}

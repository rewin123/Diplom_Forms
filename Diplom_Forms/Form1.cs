using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diplom_Forms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                VideoProcessig processig = new VideoProcessig(openFileDialog1.FileName, (map) => map);
                processig.Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImageProcessing proc = new ImageProcessing(Procedurs.Medium(openFileDialog1.FileName).GetImage());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Vector3[,] val = Procedurs.Medium(openFileDialog1.FileName);
                Morf morf = Morf.GenerateKMean(val,5);
                morf.RemoveEmptyRegions();
                for(int i = 0;i < morf.regions.Count;i++)
                {
                    morf.regions[i].Fill(morf.regions[i].GetAverage(val), val);
                }
                var proc = new ImageProcessing(val.GetImage());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var rgb = Procedurs.Medium(openFileDialog1.FileName);
                var hsv = rgb.HSV();
                hsv.ForEach((v) => v.z = 0.5f);
                rgb.ForEach(hsv, (r, h) => h.RGB());
                ImageProcessing processing = new ImageProcessing(rgb.GetImage());
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Vector3[,] val = Procedurs.Medium(openFileDialog1.FileName);
                Morf morf = Morf.GenerateKMean(val, 5);
                morf.RemoveEmptyRegions();
                List<Vector3> avrs = new List<Vector3>();
                for (int i = 0; i < morf.regions.Count; i++)
                {
                    avrs.Add(morf.regions[i].GetAverage(val));
                }
                float[,] temp = new float[val.GetLength(0), val.GetLength(1)];
                
                VideoProcessig videoProcessig = new VideoProcessig(openFileDialog1.FileName, (map) =>
                {
                    val.WriteRGB(map);
                    Procedurs.MorfSubtract(morf, avrs, val, temp);
                    return temp;
                });
                videoProcessig.Show();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Vector3[,] val = Procedurs.Medium(openFileDialog1.FileName);
                Morf morf = Morf.GenerateKMean(val, 5);
                morf.RemoveEmptyRegions();
                List<Vector3> avrs = new List<Vector3>();
                for (int i = 0; i < morf.regions.Count; i++)
                {
                    avrs.Add(morf.regions[i].GetAverage(val));
                }
                float[,] temp = new float[val.GetLength(0), val.GetLength(1)];
                float[,] temp2 = new float[val.GetLength(0), val.GetLength(1)];

                VideoProcessig videoProcessig = new VideoProcessig(openFileDialog1.FileName, (map) =>
                {
                    val.WriteRGB(map);
                    Procedurs.MorfSubtract(morf, avrs, val, temp);
                    temp2.ForEach(() => 0);
                    Procedurs.BlockSum(temp, temp2, 5);
                    temp2.RegMaximum();
                    return temp2;
                });
                videoProcessig.Show();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Vector3[,] val = Procedurs.Medium(openFileDialog1.FileName);
                Morf morf = Morf.GenerateKMean(val, 5);
                morf.RemoveEmptyRegions();
                List<Vector3> avrs = new List<Vector3>();
                for (int i = 0; i < morf.regions.Count; i++)
                {
                    avrs.Add(morf.regions[i].GetAverage(val));
                }
                float[,] temp = new float[val.GetLength(0), val.GetLength(1)];
                float[,] temp2 = new float[val.GetLength(0), val.GetLength(1)];
                byte[,] b_temp = new byte[val.GetLength(0), val.GetLength(1)];

                VideoProcessig videoProcessig = new VideoProcessig(openFileDialog1.FileName, (map) =>
                {
                    val.WriteRGB(map);
                    Procedurs.MorfSubtract(morf, avrs, val, temp);
                    b_temp.ForEach(temp, (b, v) => (byte)(v > 0.2f ? 1 : 0));

                    Morf mm = Morf.GenerateBinar(b_temp);

                    using (Graphics gr = Graphics.FromImage(map))
                    {
                        for (int i = 0; i < mm.regions.Count; i++)
                        {
                            var rect = mm.regions[i].GetSize();
                            if (rect.Width * rect.Height > 200)
                            {
                                gr.DrawRectangle(Pens.Green, rect);
                            }
                        }
                    }

                    return map;
                });
                videoProcessig.Show();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                
                VideoProcessig videoProcessig = new VideoProcessig(openFileDialog1.FileName, (map) =>
                {
                    Vector3[,] arr = map.GetRGB();
                    Morf m = Morf.GenerateKMean(arr,5);
                    for(int i = 0;i < m.regions.Count;i++)
                    {
                        m.regions[i].Fill(m.regions[i].GetAverage(arr), arr);
                    }

                    return arr;
                });
                videoProcessig.Show();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                VideoProcessig videoProcessig = new VideoProcessig(openFileDialog1.FileName, (map) =>
                {
                    Vector3[,] arr = map.GetRGB();
                    float[,] sobol = new float[map.Width, map.Height];
                    int dwidth = map.Width - 1;
                    int dheight = map.Height - 1;
                    for(int x = 1;x < dwidth;x++)
                    {
                        for(int y = 1;y < dheight;y++)
                        {
                            sobol[x, y] = (arr[x, y] - arr[x, y - 1]).Magnitude() + (arr[x, y] - arr[x - 1, y]).Magnitude();
                        }
                    }

                    sobol.RegMaximum();

                    return sobol;
                });
                videoProcessig.Show();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu;
using Emgu.CV.Structure;
using Emgu.CV.Features2D;

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
                Image<Gray, Byte> last = null;
                Image<Gray, float> flowX = null;
                Image<Gray, float> flowY = null;

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
                byte[,] b_temp2 = new byte[val.GetLength(0), val.GetLength(1)];

                LinesTracer tracer = new LinesTracer();
                List<Rectangle> cars = new List<Rectangle>();

                VideoProcessig videoProcessig = new VideoProcessig(openFileDialog1.FileName, (map) =>
                {
                    val.WriteRGB(map);
                    Procedurs.MorfSubtract(morf, avrs, val, temp);
                    b_temp.ForEach(temp, (b, v) => (byte)(v > 0.2f ? 1 : 0));
                    b_temp.BinaryThiken(b_temp2);
                    b_temp2.BinaryThiken(b_temp);
                    b_temp.BinaryThiken(b_temp2);
                    b_temp2.BinaryThiken(b_temp);

                    Image<Gray, Byte> image = new Image<Gray, byte>(map);
                    
                    if(last == null)
                    {
                        last = image;
                        return map;
                    }

                    CvInvoke.CalcOpticalFlowFarneback(last,image, flowX, flowY, 0.5, 3, 10, 3, 5, 1.5, Emgu.CV.CvEnum.OpticalflowFarnebackFlag.Default);
                    CvInvoke.AccumulateSquare(flowX, flowY);
                    CvInvoke.Sobel(flowY, flowX, Emgu.CV.CvEnum.DepthType.Cv32F, 1, 0);

                    Morf mm = Morf.GenerateBinar(b_temp);

                    cars.Clear();

                    using (Graphics gr = Graphics.FromImage(map))
                    {
                        for (int i = 0; i < mm.regions.Count; i++)
                        {
                            var rect = mm.regions[i].GetSize();
                            if (rect.Width * rect.Height > 1000)
                            {
                                gr.DrawRectangle(Pens.Green, rect);
                                cars.Add(rect);
                            }
                        }
                    }

                    tracer.TrackAndWrite(cars, ref map, 0.2f * map.Width);

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
                    m.RemoveEmptyRegions();
                    for(int i = 0;i < m.regions.Count;i++)
                    {

                        if (m.regions[i].Size < 20000)
                        {
                            m.regions[i].Fill(new Vector3(1, 1, 1), arr);
                        }
                        else m.regions[i].Fill(m.regions[i].GetAverage(arr), arr);
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
                    float temp = 0;
                    for(int x = 1;x < dwidth;x++)
                    {
                        for(int y = 1;y < dheight;y++)
                        {
                            sobol[x,y] = Math.Abs(2 * arr[x - 1, y].Magnitude() + arr[x - 1, y - 1].Magnitude() + arr[x - 1, y + 1].Magnitude()
                            - 2 * arr[x + 1, y].Magnitude() - arr[x + 1, y - 1].Magnitude() - arr[x + 1, y + 1].Magnitude());

                            sobol[x, y] += Math.Abs(2 * arr[x, y - 1].Magnitude() + arr[x - 1, y - 1].Magnitude() + arr[x + 1, y - 1].Magnitude()
                            - 2 * arr[x, y + 1].Magnitude() - arr[x - 1, y + 1].Magnitude() - arr[x + 1, y + 1].Magnitude());

                            sobol[x, y] /= 9;

                            sobol[x,y] = (byte)(sobol[x, y] > 0.1 ? 1 : 0);

                        }
                    }
                    

                    return sobol;
                });
                videoProcessig.Show();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                VideoProcessig videoProcessig = new VideoProcessig(openFileDialog1.FileName, (map) =>
                {
                    Image<Gray, Byte> image = new Image<Gray, byte>(map);
                    var result = image.Canny(80, 100);
                    return result.Bitmap;
                });
                videoProcessig.Show();
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image<Gray, Byte> last = null;
                Image<Gray, float> flowX = null;
                Image<Gray, float> flowY = null;
                Image<Gray, byte> draw = null;
                MKeyPoint[] keys;

                Accord.Video.FFMPEG.VideoFileWriter wr = new Accord.Video.FFMPEG.VideoFileWriter();
                
                VideoProcessig videoProcessig = new VideoProcessig(openFileDialog1.FileName, (map) =>
                {
                    Image<Gray, Byte> image = new Image<Gray, byte>(map);

                    if(last == null)
                    {
                        last = image;
                        flowX = new Image<Gray, float>(map.Size);
                        flowY = new Image<Gray, float>(map.Size);
                        draw = new Image<Gray, byte>(map.Size);
                        wr.Open("result.avi", map.Width, map.Height);
                        return map;
                    }
                    else
                    {
                        CvInvoke.CalcOpticalFlowFarneback(last, image, flowX, flowY, 0.5, 3, 10, 3, 5, 1.5, Emgu.CV.CvEnum.OpticalflowFarnebackFlag.Default);
                        last = image;
                    }

                    CvInvoke.AccumulateSquare(flowX, flowY);
                    CvInvoke.Canny(flowY.Convert<byte>(FloatToByte), draw, 40, 50);
                    

                    var result = draw.Bitmap;
                    wr.WriteVideoFrame(result);
                    return result;
                });
                videoProcessig.Show();
            }

            byte FloatToByte(float val)
            {
                return (byte)val;
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image<Gray, Byte> last = null;
                Image<Gray, float> flowX = null;
                Image<Gray, float> flowY = null;
                Image<Gray, float> draw = null;

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
                    Image<Gray, byte> image = new Image<Gray, byte>(map.Size);
                    temp.RegMaximum();
                    for(int x = 0;x < map.Width;x++)
                    {
                        for(int y = 0;y < map.Height;y++)
                        {
                            Gray g = image[y, x];
                            g.Intensity = temp[x, y] * 255;
                            image[y, x] = g;
                        }
                    }

                    if (last == null)
                    {
                        last = image;
                        flowX = new Image<Gray, float>(map.Size);
                        flowY = new Image<Gray, float>(map.Size);
                        draw = new Image<Gray, float>(map.Size);
                    }
                    else
                    {
                        CvInvoke.CalcOpticalFlowFarneback(last, image, flowX, flowY, 0.5, 3, 10, 3, 5, 1.5, Emgu.CV.CvEnum.OpticalflowFarnebackFlag.Default);
                        last = image;
                    }

                    double max = 0;
                    for (int y = 0; y < map.Height; y++)
                    {
                        for (int x = 0; x < map.Width; x++)
                        {
                            Gray g = draw[y, x];
                            double value = Math.Abs(flowX[y, x].Intensity) + Math.Abs(flowY[y, x].Intensity);
                            max = Math.Max(value, max);
                            g.Intensity = value;
                            draw[y, x] = g;
                        }
                    }

                    for (int y = 0; y < map.Height; y++)
                    {
                        for (int x = 0; x < map.Width; x++)
                        {
                            Gray g = draw[y, x];
                            g.Intensity /= max;
                            g.Intensity *= 255;
                            draw[y, x] = g;
                        }
                    }



                    var result = draw.Bitmap;
                    return result;
                });

                
                videoProcessig.Show();
            }
        }
    }
}

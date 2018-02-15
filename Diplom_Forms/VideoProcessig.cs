using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Video.FFMPEG;

namespace Diplom_Forms
{
    public partial class VideoProcessig : Form
    {
        VideoFileReader reader;
        Func<Bitmap, Bitmap> func;
        int pos = 0;
        public VideoProcessig(string path, Func<Bitmap,Bitmap> func)
        {
            Configure(path);
            this.func = func;
            InitializeComponent();

            this.Shown += VideoProcessig_Shown;
        }

        public VideoProcessig(string path, Func<Bitmap, Vector3[,]> func)
        {
            Configure(path);
            this.func = (map) => func(map).GetImage();
            InitializeComponent();

            this.Shown += VideoProcessig_Shown;
        }

        public VideoProcessig(string path, Func<Bitmap, float[,]> func)
        {
            Configure(path);
            this.func = (map) => func(map).GetImage();
            InitializeComponent();

            this.Shown += VideoProcessig_Shown;
        }

        void Configure(string path)
        {
            reader = new VideoFileReader();
            reader.Open(path);
        }

        private void VideoProcessig_Shown(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.Image = func(reader.ReadVideoFrame(pos));
            pos++;
            if(pos >= reader.FrameCount)
            {
                reader.Close();
            }
        }
    }
}

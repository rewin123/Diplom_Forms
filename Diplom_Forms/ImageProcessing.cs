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
    public partial class ImageProcessing : Form
    {
        public ImageProcessing(Bitmap img)
        {
            InitializeComponent();

            pictureBox1.Image = img;
            Show();
        }

        public ImageProcessing()
        {
            InitializeComponent();
        }

        public Image Picture
        {
            get
            {
                return pictureBox1.Image;
            }
            set
            {
                pictureBox1.Image = value;
            }
        }
    }
}

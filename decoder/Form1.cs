using System.IO;
using System.Windows.Forms;

namespace decoder
{
    public partial class Form1 : Form
    {
        //declaring variables
        string path = "";
        string message;
        Bitmap displayPPM;//creating bitmap

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            //when open button is clicked it will call openFile function defined below
            openFile();
        }
        public void openFile()
        {
            openFileDialog1.Filter = "Picture Files |  *.ppm"; //limits to ppm files

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {//when user clicks "OK" the name of file chosen is saved as path 
                {
                    path = openFileDialog1.FileName;

                }
                try
                {//creating bitmap from FromPPM function defined below
                    pictureBox1.Image = FromPPM();

                    Bitmap FromPPM()
                    {
                        StreamReader sr = new StreamReader(path); //using stored file name in path to read file

                        //reading header info
                        string format = sr.ReadLine(); //PPM3 or PPM6
                        string comment = sr.ReadLine();
                        string widthHeight = sr.ReadLine(); //PPM dimensions
                        string maxCV = sr.ReadLine(); //CV = color value

                        //storing dimension data into an array to split into separate ints
                        string[] dimArray = widthHeight.Split();
                        int width = int.Parse(dimArray[0]);
                        int height = int.Parse(dimArray[1]);
                        //will be used for dimensions of altered image
                        pictureBox1.Height = height;
                        pictureBox1.Width = width;

                        //assigning bitmap dimensions
                        displayPPM = new Bitmap(width, height);

                        //declaring vars to store PPM RGB data
                        int red;
                        int green;
                        int blue;

                        //looping thru pixels
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                //collect ppm elements
                                red = int.Parse(sr.ReadLine());
                                green = int.Parse(sr.ReadLine());
                                blue = int.Parse(sr.ReadLine());

                                //convert RGB values to color
                                Color pixelColor = Color.FromArgb(red, green, blue);

                                //draw pixel
                                displayPPM.SetPixel(x, y, pixelColor);
                            }
                        }
                        //return the bitmap
                        return displayPPM;
                    }
                }
                catch (Exception error)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(error.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {//when user clicks the decode button

            //declaring variables
            Color byteColor = new Color();
            byte r;
            byte g;
            byte b;
            int i;
            int j;

            //looping through pixels of image starting from known starting point of message to decode
            for (i = 1; i < displayPPM.Height; i++)
            {
                for (j = 1; j < displayPPM.Width; j++)
                {
                    //collecting pixel data at each location
                    byteColor = displayPPM.GetPixel(i, j);
                    r = byteColor.R;
                    g = byteColor.G;
                    b = byteColor.B;

                    //cheks to see if blue channel value (blueVal) corresponds with known value range for message
                    if (b > 43 && b < 91)
                    {

                        int blueVal = byteColor.B;
                        message += (char)blueVal; //casting int blue val as corresponding char and adding to message

                    }
                    else if (byteColor.B == 32)//checking for space char in message
                    {
                        message += ' ';
                    }
                    else
                    {
                        //stops looking for message 
                        break;
                    }

                }
            }
            //output to user
            richTextBox1.Text = message;
        }
    }
}

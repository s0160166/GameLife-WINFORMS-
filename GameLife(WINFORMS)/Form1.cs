using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using Game_Of_Life;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace GameLife_WINFORMS_
{
    public partial class Form1 : Form

    {
        public object locker = new object();
        public GameLoop gameLoop;
        public Bitmap image1;
        public delegate void Draw_Map_Delegate();
        public Draw_Map_Delegate myDelegate;

        public Form1()
        {

            InitializeComponent();
            image1 = new Bitmap(this.PictureBox1.Width, this.PictureBox1.Height);
            Graphics grfx = Graphics.FromImage(image1);

            gameLoop = new GameLoop(grfx,PictureBox1, image1, locker, this, label7, label8, label9, label4);

            Thread draw_thread = new Thread(gameLoop.Draw_Map);
            draw_thread.Name = "draw_thread";
            draw_thread.Start();

            Thread male_thread = new Thread(gameLoop.Male);
            male_thread.Name = "male_thread";
            male_thread.Start();

            Thread female_thread = new Thread(gameLoop.Female);
            female_thread.Name = "female_thread";
            female_thread.Start();

            /*
            Thread move_thread = new Thread(gameLoop.Move_Bacterials);
            move_thread.Name = "move_thread";
            move_thread.Start();

            Thread eat_food_thread = new Thread(gameLoop.Eat_food);
            eat_food_thread.Name = "eat_thread";
            eat_food_thread.Start();

            Thread create_food_thread = new Thread(gameLoop.Create_Food);
            create_food_thread.Name = "create_food_thread";
            create_food_thread.Start();

            Thread check_bacteria_thread = new Thread(gameLoop.Check_Bacterias);
            check_bacteria_thread.Name = "check_bacteria_thread";
            check_bacteria_thread.Start();

            Thread create_bacteria_thread = new Thread(gameLoop.Create_Bacterias);
            create_bacteria_thread.Name = "create_bacteria_thread";
            create_bacteria_thread.Start();

            myDelegate = new Draw_Map_Delegate(Reset_PictureBox);
            

            Thread reset_thread = new Thread(Reset_PictureBox);
            reset_thread.Name = "reset_thread";
            reset_thread.Start(); 
            */
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}

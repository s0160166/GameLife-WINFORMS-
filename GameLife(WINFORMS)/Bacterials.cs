using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Game_Of_Life
{
    public class Bacterials
    {
        public int food = 200;
        public int x;
        public int y;
        public Random rnd;
        public bool is_alive = true;
        public string gender;
        public SolidBrush color;
        public Bacterials(int x, int y, string gender, Random rnd) {
            this.x = x;
            this.y = y;
            this.rnd = rnd;
            this.gender = gender;
            if (this.gender == "Male")
            {
                this.color = new SolidBrush(Color.Blue);
            }
            else
            {
                this.color = new SolidBrush(Color.Red);
            }
        }
        public void Draw (Graphics grfx)
        {   
            if (is_alive)
            {
                grfx.FillEllipse(this.color, x, y, 10, 10);
            }
        }
        public void Move ()
        {
            if (is_alive)
            {
                if (food > 0)
                {
                    int direction = rnd.Next(0, 2);
                    int sign = rnd.Next(0, 2);
                    if (direction == 1)
                    {
                        if (sign == 1)
                        {
                            x += 5;
                        }
                        else
                        {
                            x -= 5;
                        }
                    }
                    else
                    {
                        if (sign == 1)
                        {
                            y += 5;
                        }
                        else
                        {
                            y -= 5;
                        }
                    }
                    food--;
                }
                else { is_alive = false; }
            }
        }
    }

}

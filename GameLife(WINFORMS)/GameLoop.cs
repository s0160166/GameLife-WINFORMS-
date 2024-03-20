using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;


namespace Game_Of_Life
{
    // ДОБАВИТЬ НЕВОЗМОЖНОСТЬ ПОЯВЛЕНИЯ НОВЫХ КЛЕТОК НА УЖЕ ЗАНЯТОЙ ТЕРРИТОРИИ, А ТО НЕТ СМЫСЛА.
    public class GameLoop
    {
        int total_create = 0;
        public List<(int, int)> food = new List<(int, int)>();
        public List<Bacterials> bacterials = new List<Bacterials>();
        public List<Bacterials> bacterials_male = new List<Bacterials>();
        public List<Bacterials> bacterials_female = new List<Bacterials>();
        public Random rnd = new Random();
        public object locker = new object();
        public Color white = Color.FromArgb(255,255,255,255);
        public Color green = Color.FromArgb(255, 0, 255, 0);
        public Color red = Color.FromArgb(255, 255, 0, 0);
        public Bitmap image1;
        public Color blue = Color.FromArgb(255, 0, 0, 255);
        public int Width;
        public int Height;
        public Form form1;
        public Label label1;
        public Label label2;
        public Label label3;
        public Label label4;
        public Graphics grfx;
        public object locker2 = new object();

        public PictureBox PictureBox1;

        public GameLoop(Graphics grfx, PictureBox PictureBox1, Bitmap image1, object locker, Form form1,Label label1, Label label2, Label label3, Label label4)
        {
            this.PictureBox1 = PictureBox1;
            this.image1 = image1;
            this.locker = locker;
            this.form1 = form1;
            this.label1 = label1;
            this.grfx = grfx;
            this.label2 = label2;
            this.label3 = label3;
            this.label4 = label4;
            Width = this.PictureBox1.Width;
            Height = this.PictureBox1.Height;
        }


        public void Draw_Map()
        {
            SolidBrush greenBrush = new SolidBrush(Color.Green);
            SolidBrush blueBrush = new SolidBrush(Color.Blue);

            Bacterials Adam = new Bacterials(Width / 2 - 100, Height / 2 - 100, "Male", rnd);
            bacterials.Add(Adam);
            bacterials_male.Add(Adam);
            Bacterials Eva = new Bacterials(Width / 2 - 95, Height / 2 - 95, "Female", rnd);
            bacterials.Add(Eva);
            bacterials_female.Add(Eva);

            Create_Food();

            while (food.Count>0)
            {
                if (Monitor.TryEnter(locker))
                {
                    grfx.Clear(Color.White);
                    Draw_Food(grfx, greenBrush);
                    Draw_Bacterials();
                    Monitor.Exit(locker);
                }

                Create_Food();
                Eat_food();
                Create_Bacterias(grfx);

                if (Monitor.TryEnter(locker))
                {
                    form1.Invoke((MethodInvoker)delegate// делегируем отрисовку GUI основному потоку, в котором обрабатывается
                    {
                        try
                        {
                            PictureBox1.Image = image1;
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    });
                    Monitor.Exit(locker);
                }
                Display();
                Thread.Sleep(100);
            }
            form1.Invoke((MethodInvoker)delegate
            {
                label4.Text = "Game Over";
            });
        }

        public void Display()
        {
            form1.Invoke((MethodInvoker)delegate
            {
                label1.Text = bacterials_male.Count.ToString();
                label2.Text = bacterials_female.Count.ToString();
                label3.Text = food.Count.ToString();
            });
        }
        public void Create_Food()
        {
                for (int i = 0; i < rnd.Next(10, 101); i++)
                {
                    int x = rnd.Next(10, Width - 10);
                    int y = rnd.Next(10, Height - 10);
                    Color pixelColor1 = red;
                    Color pixelColor2 = red;
                    Color pixelColor3 = red;
                    Color pixelColor4 = red;

                    if (Monitor.TryEnter(locker))
                    {
                        try
                        {
                            pixelColor1 = image1.GetPixel(x, y);
                            pixelColor2 = image1.GetPixel(x + 10, y);
                            pixelColor3 = image1.GetPixel(x, y + 10);
                            pixelColor4 = image1.GetPixel(x + 10, y + 10);
                            Monitor.Exit(locker);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    try
                    {
                        if (!(pixelColor1 == red || pixelColor1 == blue || pixelColor2 == red || pixelColor2 == blue || pixelColor3 == red || pixelColor3 == blue || pixelColor4 == red || pixelColor4 == blue))
                        {
                            food.Add((x, y));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                }
        }

        public void Draw_Food(Graphics grfx, SolidBrush greenBrush)
        {
            for (int i = 0; i < food.Count; i++)
            {
                grfx.FillEllipse(greenBrush, food[i].Item1, food[i].Item2, 10, 10);
            }

        }

        public void Eat_food()
        {
                Color pixelColor1 = red;
                Color pixelColor2 = red;
                Color pixelColor3 = red;
                Color pixelColor4 = red;
                
                for (int i = 0; i < bacterials.Count; i++)
                {
                    int x = bacterials[i].x;
                    int y = bacterials[i].y;

                    if (x < Width - 10 && y < Height - 10 && x > 10 && y > 10)
                    {
                        if (Monitor.TryEnter(locker))
                        {
                            try
                            {
                                pixelColor1 = image1.GetPixel(x, y);
                                pixelColor2 = image1.GetPixel(x + 10, y);
                                pixelColor3 = image1.GetPixel(x, y + 10);
                                pixelColor4 = image1.GetPixel(x + 10, y + 10);
                            }
                            catch
                            {
                                continue;
                            }
                            Monitor.Exit(locker);
                        }
                        try
                        {
                            if (pixelColor1 == green || pixelColor2 == green || pixelColor3 == green || pixelColor4 == green)
                            {
                                bacterials[i].food += 20;
                                bacterials[i].food = 100 % bacterials[i].food;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
                for (int i = 0; i < food.Count; i++)
                {
                    int x = food[i].Item1;
                    int y = food[i].Item2;

                    pixelColor1 = white;
                    pixelColor2 = white;
                    pixelColor3 = white;
                    pixelColor4 = white;

                    if (Monitor.TryEnter(locker))
                    {
                        try
                        {
                            pixelColor1 = image1.GetPixel(x, y);
                            pixelColor2 = image1.GetPixel(x + 10, y);
                            pixelColor3 = image1.GetPixel(x, y + 10);
                            pixelColor4 = image1.GetPixel(x + 10, y + 10);
                        }
                        catch
                        {
                            continue;
                        }
                        Monitor.Exit(locker);
                    }
                    try
                    {
                        if (pixelColor1 == red || pixelColor1 == blue || pixelColor2 == red || pixelColor2 == blue || pixelColor3 == red || pixelColor3 == blue || pixelColor4 == red || pixelColor4 == blue)
                        {
                            this.food.RemoveAt(i);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            
        }

        public void Draw_Bacterials()
        {
            for (int i = 0; i < bacterials.Count; i++)
            {
                grfx.FillEllipse(bacterials[i].color, bacterials[i].x, bacterials[i].y, 10, 10);
            }
        }

        public void Create_Bacterias(Graphics grfx)
        {
                List<Bacterials> new_bacterials = new List<Bacterials>();
                List<Bacterials> new_bacterials_male = new List<Bacterials>();
                List<Bacterials> new_bacterials_female = new List<Bacterials>();

                for (int i = 0; i < this.bacterials_male.Count; i++)
                {
                    int male_x = bacterials_male[i].x + 5;
                    int male_y = bacterials_male[i].y + 5;
                    for (int j = 0; j < this.bacterials_female.Count; j++)
                    {
                        int female_x = bacterials_female[j].x + 5;
                        int female_y = bacterials_female[j].y + 5;
                        var distance = Math.Sqrt(Math.Pow(male_x - female_x, 2) + Math.Pow(male_y - female_y, 2));
                        if (distance <= 10)
                        {
                            //System.Console.WriteLine("male " + i.ToString() + " sex with " + j.ToString());
                            total_create++;
                            int new_male_count = rnd.Next(0, 5);
                            int new_female_count = 4 - new_male_count;
                            //System.Console.WriteLine("new_male_count " + new_male_count.ToString());
                            //System.Console.WriteLine("ew_female_count " + new_female_count.ToString());
                            
                            for (int k = 0; k < new_male_count; k++)
                            {
                                int new_bacterials_x = male_x + rnd.Next(-20, 20);
                                int new_bacterials_y = male_y + rnd.Next(-20, 20);
                                if (10 < new_bacterials_x && new_bacterials_x < Width - 10 && 10 < new_bacterials_y && new_bacterials_y < Height - 10)
                                {
                                    Color pixelColor1 = red;
                                    Color pixelColor2 = red;
                                    Color pixelColor3 = red;
                                    Color pixelColor4 = red;
                                    try
                                    {
                                        if (Monitor.TryEnter(locker))
                                        {
                                            try
                                            {
                                                pixelColor1 = image1.GetPixel(new_bacterials_x, new_bacterials_y);
                                                pixelColor2 = image1.GetPixel(new_bacterials_x + 10, new_bacterials_y);
                                                pixelColor3 = image1.GetPixel(new_bacterials_x, new_bacterials_y + 10);
                                                pixelColor4 = image1.GetPixel(new_bacterials_x + 10, new_bacterials_y + 10);
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                            Monitor.Exit(locker);
                                        }
                                        if (!(pixelColor1 == red || pixelColor2 == red || pixelColor3 == red || pixelColor4 == red || pixelColor1 == blue || pixelColor2 == blue || pixelColor3 == blue && pixelColor4 == blue))
                                        {
                                            Bacterials new_bacterial = new Bacterials(new_bacterials_x, new_bacterials_y, "Male", rnd);
                                            grfx.FillEllipse(new_bacterial.color, new_bacterial.x, new_bacterial.y, 10, 10);
                                            new_bacterials.Add(new_bacterial);
                                            new_bacterials_male.Add(new_bacterial);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Console.WriteLine(ex.ToString());
                                    }
                                }
                            }
                            for (int k = 0; k < new_female_count; k++)
                            {
                                int new_bacterials_x = female_x + rnd.Next(-20, 20);
                                int new_bacterials_y = female_y + rnd.Next(-20, 20);
                                if (10 < new_bacterials_x && new_bacterials_x < Width - 10 && 10 < new_bacterials_y && new_bacterials_y < Height - 10)
                                {
                                    Color pixelColor1 = red;
                                    Color pixelColor2 = red;
                                    Color pixelColor3 = red;
                                    Color pixelColor4 = red;
                                    try
                                    {
                                        if (Monitor.TryEnter(locker))
                                        {
                                            try
                                            {
                                                pixelColor1 = image1.GetPixel(new_bacterials_x, new_bacterials_y);
                                                pixelColor2 = image1.GetPixel(new_bacterials_x + 10, new_bacterials_y);
                                                pixelColor3 = image1.GetPixel(new_bacterials_x, new_bacterials_y + 10);
                                                pixelColor4 = image1.GetPixel(new_bacterials_x + 10, new_bacterials_y + 10);
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                            Monitor.Exit(locker);
                                        }
                                        if (!(pixelColor1 == red || pixelColor2 == red || pixelColor3 == red || pixelColor4 == red || pixelColor1 == blue || pixelColor2 == blue || pixelColor3 == blue && pixelColor4 == blue))
                                        {
                                            Bacterials new_bacterial = new Bacterials(new_bacterials_x, new_bacterials_y, "Female", rnd);
                                            grfx.FillEllipse(new_bacterial.color, new_bacterial.x, new_bacterial.y, 10, 10);
                                            new_bacterials.Add(new_bacterial);
                                            new_bacterials_female.Add(new_bacterial);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        System.Console.WriteLine(ex.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < new_bacterials.Count; i++)
                {
                    this.bacterials.Add(new_bacterials[i]);
                }
                for (int i = 0; i < new_bacterials_male.Count; i++)
                {
                    this.bacterials_male.Add(new_bacterials_male[i]);
                }
                for (int i = 0; i < new_bacterials_female.Count; i++)
                {
                    this.bacterials_female.Add(new_bacterials_female[i]);
                }
        }

        public void Male()
        {
            while (true)
            {
                //Проверка на жизнеспособность
                for (int i = 0; i < bacterials_male.Count; i++)
                {
                    try
                    {
                        if (!bacterials_male[i].is_alive || bacterials_male[i].x > Width || bacterials_male[i].y > Height || bacterials_male[i].y < 0 || bacterials_male[i].x < 0)
                        {
                            bacterials_male.RemoveAt(i);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                // Движение
                for (int i = 0; i < bacterials_male.Count; i++)
                {
                    bacterials_male[i].Move();
                }
                Thread.Sleep(100);
            }
        }
        public void Female()
        {
            while (true)
            {
                //Проверка на жизнеспособность
                for (int i = 0; i < bacterials_female.Count; i++)
                {
                    try
                    {
                        if (!bacterials_female[i].is_alive || bacterials_female[i].x > Width || bacterials_female[i].y > Height || bacterials_male[i].y < 0 || bacterials_male[i].x < 0)
                        {
                            bacterials_female.RemoveAt(i);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                // Движение
                for (int i = 0; i < bacterials_female.Count; i++)
                {
                    bacterials_female[i].Move();
                }
                Thread.Sleep(100);
            }
        }
        public void Check_Bacterias()
        {
            
                for (int i = 0; i < bacterials.Count; i++)
                {
                    try
                    {
                        if (!bacterials[i].is_alive || bacterials[i].x > Width || bacterials[i].y > Height || bacterials[i].y < 0 || bacterials[i].x < 0)
                        {
                            bacterials.RemoveAt(i);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                for (int i = 0; i < bacterials_male.Count; i++)
                {
                    try
                    {
                        if (!bacterials_male[i].is_alive || bacterials_male[i].x > Width || bacterials_male[i].y > Height || bacterials_male[i].y < 0 || bacterials_male[i].x < 0)
                        {
                            bacterials_male.RemoveAt(i);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                for (int i = 0; i < bacterials_female.Count; i++)
                {
                    try
                    {
                        if (!bacterials_female[i].is_alive || bacterials_female[i].x > Width || bacterials_female[i].y > Height || bacterials_female[i].y < 0 || bacterials_female[i].x < 0)
                        {
                            bacterials_female.RemoveAt(i);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
        }
    }

}

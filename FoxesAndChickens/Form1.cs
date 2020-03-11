using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;

namespace FoxesAndChickens
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.White;
            moves[0] = new List<List<coords>>();
            moves[1] = new List<List<coords>>();
            pictureBox1.Width = this.Width - 10;
            pictureBox1.Height = this.Height - 30;
            pictureBox1.Location = new Point(5, 5);
            label1.BackColor = Color.Transparent;
            chickenNumLabel.BackColor = Color.Transparent;
            pictureBox1.Controls.Add(label1);
            pictureBox1.Controls.Add(chickenNumLabel);
            areaCellHeight = pictureBox1.Height / 7 - 2;
            areaCellWidth = pictureBox1.Width / 7 - 2;

        }
        Bitmap bitmap;
        Graphics g;
        int areaCellWidth = 100;
        int areaCellHeight = 100;
        SolidBrush b = new SolidBrush(Color.White);
        Pen p = new Pen(Color.Black);
        Random rnd = new Random();
        struct coords
        {
            public coords(int x, int y, int fx, int fy)
            {
                this.x = x;
                this.y = y;
                this.fx = fx;
                this.fy = fy;
            }
            public int x;
            public int y;
            public int fx;
            public int fy;
        }
        char[,] area = new char[9, 9];
        Fox[] foxes = new Fox[2];
        
        List<List<coords>>[] moves = new List<List<coords>>[2];

        private void Form1_Load(object sender, EventArgs e)
        {
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bitmap);
            drawArea();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (i == 0 || j == 0 || i == 8 || j == 8 ||
                        ((i < 3 || i > 5) && (j < 3 || j > 5)))
                    {
                        area[i, j] = 'N';
                    }
                    else
                    {
                        area[i, j] = '0';
                    }
                }
            }

            area[3, 3] = 'f';
            foxes[0] = new Fox(3, 3);
            g.DrawImage(Images.fox, areaCellWidth * 2, areaCellHeight * 2, areaCellWidth, areaCellHeight);
            area[5, 3] = 'f';
            g.DrawImage(Images.fox, areaCellWidth * 4, areaCellHeight * 2, areaCellWidth, areaCellHeight);
            foxes[1] = new Fox(5, 3);

            for (int i = 1; i < 8; i++)
            {
                for (int j = 4; j < 6; j++)
                {
                    area[i, j] = 'c';
                    g.DrawImage(Images.chicken, (i - 1) * areaCellWidth, (j - 1) * areaCellHeight, areaCellWidth, areaCellHeight);
                }
            }

            for (int i = 3; i < 6; i++)
            {
                for (int j = 6; j < 8; j++)
                {
                    area[i, j] = 'c';
                    g.DrawImage(Images.chicken, (i - 1) * areaCellWidth, (j - 1) * areaCellHeight, areaCellWidth, areaCellHeight);
                }
            }

            pictureBox1.Image = bitmap;
        }

        int[] maxIndex = new int[2];

        void setFox(Fox fox, int x, int y)
        {
            fox.x = x;
            fox.y = y;
            // заменяется по последним координатам лисы в самом длинном ходу
            area[x, y] = 'f';
        }

        private void foxGoes() 
        {
            int foxNum = 0;
            int[] startX = new int[2];
            int[] startY = new int[2];
            for (int k = 0; k < foxes.Length; k++)
            {
                int count = 0;
                startX[k] = foxes[k].x;
                startY[k] = foxes[k].y;
                moves[k].Add(new List<coords>());
                //составления массива путей и поиск в нем максимально длинного пути
                perebor(foxes[k], ref count, k, startX[k], startY[k], new List<coords>());
                maxIndex[k] = 0;
                for (int i = 0; i < moves[k].Count; i++)
                {
                    if (moves[k][i].Count > moves[k][maxIndex[k]].Count)
                    {
                        maxIndex[k] = i;
                    }

                    
                }
                
            }
            //если максимальные пути у обоих лис нулевые, прописываются рандомные пути
            int[] foxCanWalkCount = new int[2];
            if (moves[0][maxIndex[0]].Count == 0 && moves[1][maxIndex[1]].Count == 0)
            {
                for (int k = 0; k < 2; k++)
                {
                    bool foxCanWalk = false;
                    int fx1 = 0, fy1 = 0;
                    // если у рандомной лисы есть куда идти, то она идет в рандомную сторону,
                    // если идти некуда, остается на месте
                    for (int i = 0; i < 4; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                fx1 = startX[k] - 1;
                                fy1 = startY[k];
                                break;
                            case 1:
                                fx1 = startX[k];
                                fy1 = startY[k] - 1;
                                break;
                            case 2:
                                fx1 = startX[k] + 1;
                                fy1 = startY[k];
                                break;
                            case 3:
                                fx1 = startX[k];
                                fy1 = startY[k] + 1;
                                break;
                        }
                        if (area[fx1, fy1] == '0') foxCanWalk = true;
                        if (area[fx1, fy1] != '0') foxCanWalkCount[k]++;
                    }

                    if (foxCanWalk)
                    {
                        do
                        {
                            switch (rnd.Next(0, 4))
                            {
                                case 0:
                                    fx1 = startX[k] - 1;
                                    fy1 = startY[k];
                                    break;
                                case 1:
                                    fx1 = startX[k];
                                    fy1 = startY[k] - 1;
                                    break;
                                case 2:
                                    fx1 = startX[k] + 1;
                                    fy1 = startY[k];
                                    break;
                                case 3:
                                    fx1 = startX[k];
                                    fy1 = startY[k] + 1;
                                    break;
                            }
                        } while (area[fx1, fy1] != '0');

                        moves[k].Add(new List<coords>());
                        moves[k][moves[k].Count - 1].Add(new coords(0, 0, fx1, fy1));
                        maxIndex[k] = moves[k].Count - 1;
                    }
                    
                }
            }

            if (moves[0][maxIndex[0]].Count > moves[1][maxIndex[1]].Count)
            {
                //выбор лисы для хода
                foxNum = 0;

            }
            else if (moves[0][maxIndex[0]].Count == moves[1][maxIndex[1]].Count)
            {
                foxNum = rnd.Next(0, 2);
            }
            else foxNum = 1;
  
            /////чистка куриц с поля
            if (foxCanWalkCount[0] != 4 || foxCanWalkCount[1] != 4)
            {
                if (moves[foxNum][maxIndex[foxNum]][moves[foxNum][maxIndex[foxNum]].Count - 1].x != 0)
                {
                    for (int i = 0; i < moves[foxNum][maxIndex[foxNum]].Count; i++)
                    {
                        area[moves[foxNum][maxIndex[foxNum]][i].x, moves[foxNum][maxIndex[foxNum]][i].y] = '0';
                    }
                }

                setFox(foxes[foxNum],
                    moves[foxNum][maxIndex[foxNum]][moves[foxNum][maxIndex[foxNum]].Count - 1].fx,
                    moves[foxNum][maxIndex[foxNum]][moves[foxNum][maxIndex[foxNum]].Count - 1].fy);
                area[startX[foxNum], startY[foxNum]] = '0';
            }
            

            moves[0].Clear();
            moves[1].Clear();

        }

        private void perebor(Fox fox, ref int count, int k, int prevX, int prevY, List<coords> moveList)
        {
            count++;
            List<coords> move = new List<coords>();
            //копирование предыдущего массива ходов
            for (int i = 0; i < moveList.Count; i++)
            {
                move.Add(moveList[i]);
            }
            //перебор ходов
            int fx1 = 0, fy1 = 0;
            int error = 0;
            for (int d = 0; d < 4; d++)
            {
                switch (d)
                {
                    case 0:
                        fx1 = fox.x - 1;
                        fy1 = fox.y;
                        break;
                    case 1:
                        fx1 = fox.x;
                        fy1 = fox.y - 1;
                        break;
                    case 2:
                        fx1 = fox.x + 1;
                        fy1 = fox.y;
                        break;
                    case 3:
                        fx1 = fox.x;
                        fy1 = fox.y + 1;
                        break;
                }
                if (area[fx1, fy1] == 'c')
                {
                    if (area[fx1 - fox.x + fx1, fy1 - fox.y + fy1] == '0')
                    {
                        prevX = fox.x;
                        prevY = fox.y;
                        setFox(fox, fx1 - fox.x + fx1, fy1 - fox.y + fy1);
                        move.Add(new coords(fx1, fy1, fox.x, fox.y));
                        moves[k].Add(new List<coords>());
                        for (int i = 0; i < move.Count; i++)
                        {
                            moves[k][count].Add(move[i]);
                        }
                        //стирать убитых куриц и передвинутую лису
                        area[fx1, fy1] = '0';
                        perebor(fox, ref count, k, prevX, prevY, move);
                        move.Clear();
                        
                    }
                    else error++;
                }
                else error++;
                if (error == 4)
                {
                    
                    setFox(fox, prevX, prevY);
                    for (int i = 0; i < moves[k].Count; i++)
                    {  
                        for (int j = 0; j < moves[k][i].Count; j++)
                        {
                            area[moves[k][i][j].x, moves[k][i][j].y] = 'c';
                            area[moves[k][i][j].fx, moves[k][i][j].fy] = '0';
                        }
                    }
                }
            }
        }

        private void drawArea()
        {
            g.FillRectangle(b, 0, 0, pictureBox1.Width, pictureBox1.Height);
            for (int i = 0; i < 7; i++)
            {
                if (i > 1 && i < 5)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        g.DrawRectangle(p, i * areaCellWidth, j * areaCellHeight, areaCellWidth, areaCellHeight);
                    }
                }
                else
                {
                    for (int j = 2; j < 5; j++)
                    {
                        g.DrawRectangle(p, i * areaCellWidth, j * areaCellHeight, areaCellWidth, areaCellHeight);
                    }
                }
            }
            pictureBox1.Image = bitmap;
        }

        private void draw()
        {
            drawArea();
            for (int i = 1; i < 8; i++)
            {
                for (int j = 1; j < 8; j++)
                {
                    if (area[i, j] == 'c')
                    {
                        g.DrawImage(Images.chicken, (i - 1) * areaCellWidth, (j - 1) * areaCellHeight, areaCellWidth, areaCellHeight);
                        //g.DrawString("К", font, new SolidBrush(Color.Black), new PointF((i - 1) * 60, (j - 1) * 60));
                    }
                    if (area[i, j] == 'f')
                    {
                        g.DrawImage(Images.fox, (i - 1) * areaCellWidth, (j - 1) * areaCellHeight, areaCellWidth, areaCellHeight);
                        //g.DrawString("Л", font, new SolidBrush(Color.Black), new PointF((i - 1) * 60, (j - 1) * 60));
                    }
                }
            }
            pictureBox1.Image = bitmap;
        }

        bool clicked = false;
        int cx, cy, cx1, cy1;

        private void ChickenNumLabel_TextChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(chickenNumLabel.Text) < 15  &&
                Convert.ToInt32(chickenNumLabel.Text) > 12)
            {
                chickenNumLabel.ForeColor = Color.Orange;
            }
            if (Convert.ToInt32(chickenNumLabel.Text) < 12)
            {
                chickenNumLabel.ForeColor = Color.Red;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Width = this.Width - 10;
            pictureBox1.Height = this.Height - 30;
            if (pictureBox1.Width > pictureBox1.Height)
            {
                pictureBox1.Width = pictureBox1.Height;
            }
            if (pictureBox1.Width < pictureBox1.Height)
            {
                pictureBox1.Height = pictureBox1.Width;
            }
            pictureBox1.Location = new Point((Width - pictureBox1.Width) / 2, (Height - pictureBox1.Height) / 2 - 5);
            
            areaCellHeight = pictureBox1.Height / 7 - 2;
            areaCellWidth = pictureBox1.Width / 7 - 2;
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bitmap);
            draw();
        }

        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!clicked)
            {
                clicked = true;
                cx = e.X / areaCellWidth + 1;
                cy = e.Y / areaCellHeight + 1;
                if (area[cx, cy] == 'c')
                    g.DrawRectangle(new Pen(Color.Green, 3),
                        e.X / areaCellWidth * areaCellWidth + 1,
                        e.Y / areaCellHeight * areaCellHeight + 1,
                        areaCellWidth - 3, areaCellHeight - 3);
                else
                {
                    clicked = false;
                    return;
                }
                pictureBox1.Image = bitmap;
            } else
            {
                draw();
                clicked = false;
                cx1 = e.X / areaCellWidth + 1;
                cy1 = e.Y / areaCellHeight + 1;

                startMove(cx, cy, cx1, cy1);
            }
           

        }

        void startMove(int x, int y, int x1, int y1)
        {
            if (Math.Abs(x1 - x) > 1 ||
                Math.Abs(y1 - y) > 1 ||
                (x1 == x && y1 == y) ||
                (x != x1 && y != y1) ||
                (y < y1) ||
                area[x1, y1] != '0')
            {
                return;
            }

            if (area[x, y] == 'c' && (
                area[x + 1, y] == '0' ||
                area[x - 1, y] == '0' ||
                area[x, y - 1] == '0')
                )
            {
                area[x, y] = '0';
                area[x1, y1] = 'c';

                foxGoes();
            }

            draw();
            checkWinOrLose();
        }

        void checkWinOrLose()
        {
            //win
            bool win = true;
            for (int i = 3; i < 6; i++)
                for (int j = 1; j < 4; j++)
                    if (area[i, j] != 'c')
                        win = false;

            if (win)
            {
                MessageBox.Show("Поздравляю, вы победили!");
                this.Close();
            }

            //lose
            int chickenNum = 0;
            for (int i = 1; i < 8; i++)
                for (int j = 1; j < 8; j++)
                    if (area[i, j] == 'c')
                        chickenNum++;

            chickenNumLabel.Text = chickenNum.ToString();
            if (chickenNum < 9)
            {
                MessageBox.Show("Вы проиграли");
                this.Close();
            }
                    
        }
    }
}

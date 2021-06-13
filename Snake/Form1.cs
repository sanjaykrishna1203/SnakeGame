using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using System.Linq;

namespace Snake
{
        

    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();
        public int time;
        public int SleepTimer;




        public Form1()
        {
            InitializeComponent();

            //Set settings to default
            new Settings();
           
        

            //Set game speed and start timer
            gameTimer.Interval = 1000/ Settings.Speed;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();
        }

        private void StartGame()
        {
            lblGameOver.Visible = false;
            
            

            //Set settings to default
            new Settings();


            //Create new player object
            Snake.Clear();
            Circle head = new Circle {X = 10, Y = 5};
            Snake.Add(head);


            lblScore.Text = Settings.Score.ToString();
            GenerateFood();


        }

        private void GenerateFood()
        {
            int maxXPos = pbCanvas.Size.Width / Settings.Width;
            int maxYPos = pbCanvas.Size.Height / Settings.Height;

            Random random = new Random();
            food = new Circle {X = random.Next(0, maxXPos), Y = random.Next(0, maxYPos)};
            for (var i = 1; i < Snake.Count; i++)
            {
                while (Snake[i].X == food.X && Snake[i].Y == food.Y)
                {
                    food = new Circle { X = random.Next(0, maxXPos), Y = random.Next(0, maxYPos) };
                    
                }
            }

        }

        private void UpdateScreen(object sender, EventArgs e)
        {
            //Check for Game Over
            if (Settings.GameOver)
            {

               
                
            }
            else
            {
                Thread.Sleep(SleepTimer);
                if (Input.KeyPressed(Keys.Right) && Settings.direction != Direction.Left)
                    Settings.direction = Direction.Right;
                else if (Input.KeyPressed(Keys.Left) && Settings.direction != Direction.Right)
                    Settings.direction = Direction.Left;
                else if (Input.KeyPressed(Keys.Up) && Settings.direction != Direction.Down)
                    Settings.direction = Direction.Up;
                else if (Input.KeyPressed(Keys.Down) && Settings.direction != Direction.Up)
                    Settings.direction = Direction.Down;

                MovePlayer();
                HighScore();

            }

            pbCanvas.Invalidate();

        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (!Settings.GameOver)
            {
                //Set colour of snake

                //Draw snake
                for (int i = 0; i < Snake.Count; i++)
                {
                    Brush snakeColour;
                    if (i == 0)
                        snakeColour = Brushes.Black;    
                    else
                        snakeColour = Brushes.Green;   

                    //Draw snake
                    canvas.FillEllipse(snakeColour,
                        new Rectangle(Snake[i].X * Settings.Width,
                                      Snake[i].Y * Settings.Height,
                                      Settings.Width, Settings.Height));


                    //Draw Food
                    canvas.FillEllipse(Brushes.Red,
                        new Rectangle(food.X * Settings.Width,
                             food.Y * Settings.Height, Settings.Width, Settings.Height));

                }
            }
            else
            {
                string gameOver = "Game Over \nYour final score is: " + Settings.Score + "\nPress Start to Play Again";
                lblGameOver.Text = gameOver;
                lblGameOver.Visible = true;
               
            }
        }


        private void MovePlayer()
        {
            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                //Move head
                if (i == 0)
                {
                    switch (Settings.direction)
                    {
                        case Direction.Right:
                            Snake[i].X++;
                            break;
                        case Direction.Left:
                            Snake[i].X--;
                            break;
                        case Direction.Up:
                            Snake[i].Y--;
                            break;
                        case Direction.Down:
                            Snake[i].Y++;
                            break;
                    }


                    //Get maximum X and Y Pos
                    int maxXPos = pbCanvas.Size.Width / Settings.Width;
                    int maxYPos = pbCanvas.Size.Height /Settings.Height;

                    //Detect collission with game borders.
                    if (Snake[i].X < 0 || Snake[i].Y < 0
                        || Snake[i].X >= maxXPos || Snake[i].Y >= maxYPos)
                    {
                        Die();
                    }


                    //Detect collission with body
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].X == Snake[j].X &&
                           Snake[i].Y == Snake[j].Y)
                        {
                            Die();
                        }
                    }

                    //Detect collision with food piece
                    if (Snake[0].X == food.X && Snake[0].Y == food.Y)
                    {
                        Eat();
                    }

                }
                else
                {
                    //Move body
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }

        private void Eat()
        {
            //Add circle to body
            Circle circle = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };
            Snake.Add(circle);

            //Update Score
            Settings.Score += Settings.Points;
            lblScore.Text = Settings.Score.ToString();
            

            GenerateFood();
        }

        private void Die()
        {
            Settings.GameOver = true;
            button1.Enabled = true;
            //SpeedBox.Enabled = true;
            panel2.Enabled = true;
            File.AppendAllText(@"Score.txt",
                                  Settings.Score.ToString() + Environment.NewLine);

        }



     

       

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (!(btnEasy.Checked || btnMedium.Checked || btnHard.Checked))
            {
               
                btnMedium.Checked = true;
                StartGame();
                new Settings();
                button1.Enabled = false;
                panel2.Enabled = false;

            }
            else
            {
                StartGame();
                new Settings();
                button1.Enabled = false;
                panel2.Enabled = false;
            }
            

            }

        private void btnEasy_CheckedChanged(object sender, EventArgs e)
        {
            if (btnEasy.Checked)
            {
                SleepTimer = 150;
                Settings.Points = 75;
            }
        }

        private void btnMedium_CheckedChanged(object sender, EventArgs e)
        {
            if (btnMedium.Checked)
            {
                SleepTimer = 75;
                Settings.Points = 100;
            }
        }

        private void btnHard_CheckedChanged(object sender, EventArgs e)
        {
            if (btnHard.Checked)
            {
                SleepTimer = 0;
                Settings.Points = 150;
            }
        }

        public void HighScore()
        {
            var ScoreList = new List<int>();

                foreach (string line in File.ReadLines(@"Score.txt"))
                {
                    ScoreList.Add(Convert.ToInt32(line));
                }
                var maxNum = ScoreList.Max();
                lblHighScoreValue.Text = maxNum.ToString();
            
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            File.WriteAllText(@"Score.txt", String.Empty);
            File.AppendAllText(@"Score.txt",
                                  "0" + Environment.NewLine);
            HighScore();
        }
       
    }

 }

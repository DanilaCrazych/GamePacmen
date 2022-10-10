using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;

namespace GamePacmen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();

        bool goLeft, goRight, goUp, goDown;
        bool noLeft, noRight, noUp, noDown;

        int speed = 8;

        Rect pacmanHitBox;

        int ghostSpeed = 5;
        int ghostMoveStep = 130;
        int currentGhostStep;
        int score = 0;


        public MainWindow()
        {
            InitializeComponent();

            GameSetUp();
        }

        private void CanvasKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Left && noLeft == false)
            {
                goRight = goUp = goDown = false;
                noRight = noRight = noDown = false;

                goLeft = true;
                PacMan.RenderTransform = new RotateTransform(-180, PacMan.Width / 2, PacMan.Height / 2);
            }

            if (e.Key == Key.Right && noRight == false)
            {
                noLeft = noUp = noDown = false;
                goLeft = goUp = goDown = false;

                goRight = true;
                PacMan.RenderTransform = new RotateTransform(0, PacMan.Width / 2, PacMan.Height / 2);
            }

            if (e.Key == Key.Up && noUp == false)
            {
                noLeft = noRight = noDown = false;
                goLeft = goRight = goDown = false;

                goUp = true;
                PacMan.RenderTransform = new RotateTransform(-90, PacMan.Width / 2, PacMan.Height / 2);
            }

            if (e.Key == Key.Down && noDown == false)
            {
                noLeft = noRight = noUp = false;
                goLeft = goRight = goUp = false;

                goDown = true;
                PacMan.RenderTransform = new RotateTransform(90, PacMan.Width / 2, PacMan.Height / 2);
            }

        }

        private void GameSetUp()
        {

            MyCanvas.Focus();

            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();
            currentGhostStep = ghostMoveStep;

            ImageBrush pacmanImage = new ImageBrush();
            pacmanImage.ImageSource = new BitmapImage(new Uri("E:/C#/GamePacmen/GamePacmen/images/pacman.jpg"));
            PacMan.Fill = pacmanImage;

            ImageBrush redManImage = new ImageBrush();
            redManImage.ImageSource = new BitmapImage(new Uri("E:/C#/GamePacmen/GamePacmen/images/red.jpg"));
            RedMan.Fill = redManImage;

            ImageBrush orangeManImage = new ImageBrush();
            orangeManImage.ImageSource = new BitmapImage(new Uri("E:/C#/GamePacmen/GamePacmen/images/orange.jpg"));
            OrangeMan.Fill = orangeManImage;

            ImageBrush pinkManImage = new ImageBrush();
            pinkManImage.ImageSource = new BitmapImage(new Uri("E:/C#/GamePacmen/GamePacmen/images/pink.jpg"));
            PinkMan.Fill = pinkManImage;
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            txtScore.Content = "Score: " + score;
            if (goRight)
            {
                Canvas.SetLeft(PacMan, Canvas.GetLeft(PacMan) + speed);
            }

            if (goLeft)
            {
                Canvas.SetLeft(PacMan, Canvas.GetLeft(PacMan) - speed);
            }

            if (goUp)
            {
                Canvas.SetTop(PacMan, Canvas.GetTop(PacMan) - speed);
            }

            if (goDown)
            {
                Canvas.SetTop(PacMan, Canvas.GetTop(PacMan) + speed);
            }

            if (goDown && Canvas.GetTop(PacMan) + 80 > Application.Current.MainWindow.Height)
            {
                noDown = true;
                goDown = false;
            }
            if (goUp && Canvas.GetTop(PacMan) < 1)
            {
                noUp = true;
                goUp = false;
            }
            if (goLeft && Canvas.GetLeft(PacMan) - 10 < 1)
            {
                noLeft = true;
                goLeft = false;
            }
            if (goRight && Canvas.GetLeft(PacMan) + 70 > Application.Current.MainWindow.Width)
            {
                noRight = true;
                goRight = false;
            }
            pacmanHitBox = new Rect(Canvas.GetLeft(PacMan), Canvas.GetTop(PacMan), PacMan.Width, PacMan.Height);

            //Проверка стен
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {

                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                if ((string)x.Tag == "Wall")
                {
                    if (goLeft == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetLeft(PacMan, Canvas.GetLeft(PacMan) + 10);
                        noLeft = true;
                        goLeft = false;
                    }
                    if (goRight == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetLeft(PacMan, Canvas.GetLeft(PacMan) - 10);
                        noRight = true;
                        goRight = false;
                    }
                    if (goDown == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetTop(PacMan, Canvas.GetTop(PacMan) - 10);
                        noDown = true;
                        goDown = false;
                    }
                    if (goUp == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetTop(PacMan, Canvas.GetTop(PacMan) + 10);
                        noUp = true;
                        goUp = false;
                    }
                }




                if ((string)x.Tag == "Coin")
                {
                    if (pacmanHitBox.IntersectsWith(hitBox) && x.Visibility == Visibility.Visible)
                    {
                        x.Visibility = Visibility.Hidden;
                        score++;
                    }
                }

                if ((string)x.Tag == "ghost")
                {
                    if (pacmanHitBox.IntersectsWith(hitBox))
                    {
                        GameOver("Ты умер. Начни снова!");
                    }
                    
                    
                    if (x.Name.ToString() == "PinkMan")
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - ghostSpeed);

                    }
                    else
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + ghostSpeed);
                    }

                    currentGhostStep--;

                    if (currentGhostStep < 1)
                    {
                        currentGhostStep = ghostMoveStep;
                        ghostSpeed = -ghostSpeed;
                    }

                    if (score == 33)
                    {
                        GameOver("Ты победил!");
                    }


                }


                void GameOver(string message)
                {

                    gameTimer.Stop();
                    MessageBox.Show(message, "Информация!");

                    System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();

                }
            }
        }
    }
}

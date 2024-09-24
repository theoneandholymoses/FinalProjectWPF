using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace FinalProjectWPF.Projects.CatchTheEgg.Models
{
    internal class GameManager
    {
        public int GeneralSpeed { get; set; }
        public int Score { get; set; }
        public double Hearts { get; set; }
        public Basket PlayerBasket { get; set; }
        private List<Egg> Eggs = new List<Egg>();
        private int EggRate = 0;
        private int SpecialEggRate = 0;
        private bool DoubleSkill = false;
        private Random rand = new Random();
        private Canvas MyCanvas;
        private List<Rectangle> itemsToRemove = new List<Rectangle>();

        public GameManager(Canvas canvas, Basket basket, int level)
        {
            MyCanvas = canvas;
            Score = 0;
            Hearts = 6;
            GeneralSpeed = level;
            PlayerBasket = basket;

            PlayerBasket.Size = MyCanvas.ActualWidth * 0.1;
        }
        // modified
        public void UpdateGame()
        {
            EggCreationRate();
            double windowHeight = MyCanvas.ActualHeight;
            double windowEnd = windowHeight - 65;
            int windowEndPoint = (int)windowEnd;

            foreach (Egg egg in Eggs)
            {
                if (!egg.IsMissed)
                {
                    Canvas.SetTop(egg.ItemShape, Canvas.GetTop(egg.ItemShape) + GeneralSpeed);
                }
                if (Canvas.GetTop(egg.ItemShape) > windowEndPoint && !(egg.IsMissed))
                {

                    if (egg is not Poop && egg is not Heart)
                    {
                        egg.IsMissed = true;
                        Hearts -= 0.5;
                        egg.ImgSrc = "../../../Projects/CatchTheEgg/Assets/BrokenEgg.png";
                        DelayAndRemoveEgg(egg);
                    }
                }

                EggCatched(egg);
            }

            RemoveEggsFromScreen();
        }

        public void IncreaseGameLevel()
        {
            switch (Score)
            {
                case 25:
                    {
                        GeneralSpeed += 5;
                        break;
                    }
                case 50:
                    {
                        GeneralSpeed += 5;
                        break;
                    }
                case 100:
                    {
                        GeneralSpeed += 5;
                        break;
                    }
            }
        }

        private void EggCreationRate()
        {
            EggRate++;
            if (EggRate % 25 == 0)
            {
                if (EggRate % 150 == 0)
                {
                    MakeItem(7);
                }
                else if (SpecialEggRate != 15)
                {
                    MakeItem(1);

                    SpecialEggRate++;
                }
                else
                {
                    MakeItem(rand.Next(2, 7));
                    SpecialEggRate = 0;
                }
            }
        }

        // modified
        private void MakeItem(int i)
        {
            double windowWidth = MyCanvas.ActualWidth;
            double windowEnd = windowWidth - 50;
            int windowEndPoint = (int)windowEnd;
            Egg newItem = null;

            switch (i)
            {
                case 1:
                    newItem = new Egg(rand.Next(50, windowEndPoint), "../../../Projects/CatchTheEgg/Assets/Egg.png");
                    break;
                case 2:
                    newItem = new SpecialEgg(rand.Next(50, windowEndPoint), "../../../Projects/CatchTheEgg/Assets/FreezEgg.png", "freeze");
                    break;
                case 3:
                    newItem = new SpecialEgg(rand.Next(50, windowEndPoint), "../../../Projects/CatchTheEgg/Assets/MultiplePointsEgg.png", "MultiplePoints");
                    break;
                case 4:
                    newItem = new SpecialEgg(rand.Next(50, windowEndPoint), "../../../Projects/CatchTheEgg/Assets/Basket.png", "GrowBasket");
                    break;
                case 5:
                    newItem = new SpecialEgg(rand.Next(50, windowEndPoint), "../../../Projects/CatchTheEgg/Assets/GoldenEgg.png", "GoldenEgg");
                    break;
                case 6:
                    newItem = new Heart(rand.Next(50, windowEndPoint), "../../../Projects/CatchTheEgg/Assets/Heart.png");
                    break;
                case 7:
                    newItem = new Poop(rand.Next(50, windowEndPoint), "../../../Projects/CatchTheEgg/Assets/Poop.png");
                    break;
            }

            if (newItem != null)
            {
                if (newItem.ItemShape.Name == "special")
                {
                    newItem.ItemShape.Width = 60;
                    newItem.ItemShape.Height = 70;
                }
                Canvas.SetLeft(newItem.ItemShape, newItem.Position);
                Canvas.SetTop(newItem.ItemShape, 0);
                MyCanvas.Children.Add(newItem.ItemShape);
                Eggs.Add(newItem);
            }
        }

        private async void DelayAndRemoveEgg(Egg egg)
        {
            await Task.Delay(600);
            itemsToRemove.Add(egg.ItemShape);
        }
        // modified
        private void EggCatched(Egg egg)
        {
            double windowHeight = MyCanvas.ActualHeight;
            double basketPosition = windowHeight - 100;
            int basketPositionPoint = (int)basketPosition;

            Rect eggRect = new Rect(Canvas.GetLeft(egg.ItemShape), Canvas.GetTop(egg.ItemShape), egg.ItemShape.Width, egg.ItemShape.Height);
            Rect basketRect = new Rect(PlayerBasket.Position, basketPositionPoint, PlayerBasket.Size, PlayerBasket.Size);

            if (basketRect.IntersectsWith(eggRect))
            {
                itemsToRemove.Add(egg.ItemShape);
                if (egg is SpecialEgg specialEgg)
                {
                    ApplySpecialSkill(specialEgg.Skill);
                }
                if (egg is Poop)
                {
                    Hearts--;
                }
                if (egg is Heart)
                {
                    if (Hearts == 6)
                    {
                        Score += 5;
                    }
                    else if (Hearts <= 5)
                    {
                        Hearts++;
                    }
                    else
                    {
                        Hearts = 6;
                        Score += 2;
                    }
                }
                else
                {
                    if (!DoubleSkill) {Score++;}
                    else { Score += 2; }
                    if (Score <= 100)
                    {
                        IncreaseGameLevel();
                    }
                }
            }
        }

        private async void ApplySpecialSkill(string skill)
        {
            switch (skill)
            {
                case "freeze":
                    GeneralSpeed /= 2;
                    await Task.Delay(10000);
                    GeneralSpeed *= 2;
                    break;

                case "MultiplePoints":
                    DoubleSkill = true;
                    await Task.Delay(10000);
                    DoubleSkill = false;
                    break;

                case "GrowBasket":
                    PlayerBasket.Size *= 2;
                    await Task.Delay(10000);
                    PlayerBasket.Size /= 2;
                    break;

                case "GoldenEgg":
                    Score += 10;
                    break;
            }
        }

        private void RemoveEggsFromScreen()
        {
            foreach (Rectangle item in itemsToRemove)
            {
                MyCanvas.Children.Remove(item);
                Eggs.RemoveAll(x => x.ItemShape == item);
            }
        }

        public bool IsGameOver()
        {
            return Hearts <= 0;
        }

        public void ResetGameValues()
        {
            Score = 0;
            Hearts = 5;
            EggRate = 0;
            SpecialEggRate = 0;
            GeneralSpeed = 10;
            foreach (var egg in Eggs)
            {
                MyCanvas.Children.Remove(egg.ItemShape);
            }

            Eggs.Clear();
            PlayerBasket.Size = 80;
        }

    }
}

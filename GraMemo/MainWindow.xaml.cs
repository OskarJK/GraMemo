using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MemoGame
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private DateTime startTime;
        private List<Button> buttons;
        private string[] words = { "Keys", "Kluczyki", "Potato", "Ziemniaki", "Glass", "Szkło", "Candies", "Cukierki",
                                   "Cup", "Kubek", "Games", "Gry", "Apple", "Jabłko", "Music", "Muzyka" };
        private Button firstClicked, secondClicked;
        private int matchesFound;
        private List<TimeSpan> gameResults;

        private Dictionary<string, string> wordPairs = new Dictionary<string, string>
        {
            {"Keys", "Kluczyki"},
            {"Potato", "Ziemniaki"},
            {"Glass", "Szkło"},
            {"Candies", "Cukierki"},
            {"Cup", "Kubek"},
            {"Games", "Gry"},
            {"Apple", "Jabłko"},
            {"Music", "Muzyka"}
        };

        private Dictionary<string, Brush> pairColors = new Dictionary<string, Brush>
        {
            {"Keys", Brushes.LightGreen},
            {"Kluczyki", Brushes.LightGreen},
            {"Potato", Brushes.LightBlue},
            {"Ziemniaki", Brushes.LightBlue},
            {"Glass", Brushes.LightCoral},
            {"Szkło", Brushes.LightCoral},
            {"Candies", Brushes.LightGoldenrodYellow},
            {"Cukierki", Brushes.LightGoldenrodYellow},
            {"Cup", Brushes.LightPink},
            {"Kubek", Brushes.LightPink},
            {"Games", Brushes.LightSkyBlue},
            {"Gry", Brushes.LightSkyBlue},
            {"Apple", Brushes.LightSalmon},
            {"Jabłko", Brushes.LightSalmon},
            {"Music", Brushes.LightSeaGreen},
            {"Muzyka", Brushes.LightSeaGreen}
        };

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            buttons = new List<Button>();
            gameResults = new List<TimeSpan>();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            startTime = DateTime.Now;
            TimerTextBlock.Text = "00:00";
            timer.Start();

            ShuffleWords();
            GameGrid.Children.Clear();
            firstClicked = null;
            secondClicked = null;
            matchesFound = 0;

            for (int i = 0; i < 16; i++)
            {
                Button button = new Button
                {
                    Content = "",
                    Tag = words[i],
                    Background = Brushes.LightGray,
                    FontSize = 24
                };
                button.Click += Button_Click;
                buttons.Add(button);
                GameGrid.Children.Add(button);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var elapsed = DateTime.Now - startTime;
            TimerTextBlock.Text = elapsed.ToString(@"mm\:ss");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (firstClicked != null && secondClicked != null)
                return;

            Button clickedButton = sender as Button;
            if (clickedButton == null)
                return;

            if (clickedButton.Content.ToString() != "")
                return;

            clickedButton.Content = clickedButton.Tag;

            if (firstClicked == null)
            {
                firstClicked = clickedButton;
                return;
            }

            secondClicked = clickedButton;

            // Check if the pair matches
            if (IsMatch(firstClicked.Tag.ToString(), secondClicked.Tag.ToString()))
            {
                var color = pairColors[firstClicked.Tag.ToString()];
                firstClicked.Background = color;
                secondClicked.Background = color;
                firstClicked = null;
                secondClicked = null;
                matchesFound++;

                if (matchesFound == 8) // All pairs found
                {
                    timer.Stop();
                    var elapsed = DateTime.Now - startTime;
                    gameResults.Add(elapsed);
                    UpdateResultsList();
                }
            }
            else
            {
                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                timer.Tick += (s, args) =>
                {
                    firstClicked.Content = "";
                    secondClicked.Content = "";
                    firstClicked = null;
                    secondClicked = null;
                    timer.Stop();
                };
                timer.Start();
            }
        }

        

        private bool IsMatch(string firstWord, string secondWord)
        {
            return (wordPairs.ContainsKey(firstWord) && wordPairs[firstWord] == secondWord) ||
                   (wordPairs.ContainsKey(secondWord) && wordPairs[secondWord] == firstWord);
        }

        private void ShuffleWords()
        {
            Random rng = new Random();
            int n = words.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = words[k];
                words[k] = words[n];
                words[n] = value;
            }
        }

        private void UpdateResultsList()
        {
            gameResults = gameResults.OrderBy(t => t).ToList();
            ResultsListBox.Items.Clear();
            ResultsListBox.Items.Add("Best Time: " + gameResults.First().ToString(@"mm\:ss"));
            ResultsListBox.Items.Add("All Times:");
            foreach (var result in gameResults)
            {
                ResultsListBox.Items.Add(result.ToString(@"mm\:ss"));
            }
        }
    }
}

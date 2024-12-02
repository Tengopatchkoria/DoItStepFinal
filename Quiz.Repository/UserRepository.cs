using Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static System.Formats.Asn1.AsnWriter;
using Home = Quiz.Repository.QuizRepository;

namespace Quiz.Repository
{
    public class UserRepository
    {
        private readonly string _filePath;
        private readonly List<User> _users;
        public User MainUser;
        private int _remainingSeconds = 120;
        private static System.Timers.Timer timer;

        public string QuizRelativePath()
        {
            string QuizRelativePath = Path.Combine("Data", "Quiz.json");
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.FullName;

            return Path.Combine(projectDirectory, QuizRelativePath);
        }

        public string UsersRelativePath()
        {
            string UserRelativePath = Path.Combine("Data", "Users.json");
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.FullName;

            return Path.Combine(projectDirectory, UserRelativePath);
        }
        public UserRepository(string filePath, string name, string mail)
        {
            _filePath = filePath;
            _users = LoadData();
            Console.Clear();
            foreach (var user in _users)
            {
                if (user.Name == name)
                {
                    MainUser = LoginUser(name);
                    Console.WriteLine($"==== Welcome Back {name} ====");
                    Console.WriteLine("1.Take a quiz \n2.Create a quiz \n3.Logout \n4.Get Personal Info");
                }
            }
            if (MainUser == null)
            {
                MainUser = RegUser(name, mail);
                Console.WriteLine($"=== Hello {name} ===\nIn this quiz app you can create your quizes or enjoy with already created ones" +
                $"\n1.Take a quiz \n2.Create a quiz \n3.Logout");
            }
        }

        public string GetInfo() => $"{MainUser.Name} With The {MainUser.Mail} Mail Has Best Score Of {MainUser.BestScore}";
        public void GetTop10()
        {
            var topUsers = _users
            .OrderByDescending(user => user.BestScore)
            .Take(10)
            .ToList();
            foreach (var user in topUsers)
            {
                Console.WriteLine($"User:{user.Name} Top Score:{user.BestScore}");
            }
            Home.UserHomePage(MainUser.Name, MainUser.Mail);
        }

        private User LoginUser(string name)
        {
            foreach (var user in _users)
            {
                if (user.Name == name) return user;
            }

            return null;
        }

        private User RegUser(string name, string mail)
        {
            User user = new()
            {
                Name = name,
                Mail = mail,
                BestScore = 0,
                id = (byte)((byte)_users.Count + 1)
            };
            _users.Add(user);
            SaveData();
            return user;
        }

        public void LogOut()
        {
            MainUser = null;
            Home.StartPage();
        }

        public static int UpdateScore(string userAnswer, string correctAnswer, int score)
        {
            if (userAnswer.Equals(correctAnswer, StringComparison.OrdinalIgnoreCase))
                return score += 20;
            else { return score -= 20; }
        }

        public void CreateQuiz()
        {
            List<Question> Qlist = new();
            QuestionRepository q = new();
            
            Console.Clear();
            Console.WriteLine("Create a name for your quiz");
            var QuizName = Console.ReadLine();

            q.CreateQuestionList(Qlist);

            Quizz quizz = new()
            {
                Id = 0,
                UserId = MainUser.id,
                QuestionList = Qlist
            };


            QuizRepository Qrepo = new(QuizRelativePath());
            Qrepo.AddQuiz(quizz);
            Home.UserHomePage(MainUser.Name, MainUser.Mail);
        }

        public void TakeQuiz(byte qId)
        {
            QuizRepository Qrepo = new(QuizRelativePath());
            var quiz = Qrepo.GetQuiz(qId);
            int score = 0;

            if (quiz.UserId == MainUser.id)
            {
                Console.Clear();
                Console.WriteLine("This Quiz is Created By You. Therefore You're Unable to Take it\n" +
                        "However You can Edit/Delete it\n" +
                        "1.Edit \n2.Delete \n3.Back");
                int command = int.Parse(Console.ReadLine().Trim());
                if(command == 3)
                    Home.UserHomePage(MainUser.Name, MainUser.Mail);
                else
                    Qrepo.ModifyQuiz(command, qId, MainUser);
            }

            var timer = new System.Timers.Timer(1000);
            timer.AutoReset = true;
            timer.Elapsed += OnTimedEvent;
            timer.Start();

            Console.Clear();
            foreach (var item in quiz.QuestionList)
            {
                if (_remainingSeconds <= 0) break;

                Console.WriteLine(item.QueStion);
                Console.WriteLine($"{item.AnswerOptions[0]}, {item.AnswerOptions[1]}, {item.AnswerOptions[2]}, {item.AnswerOptions[3]}");
                Console.WriteLine("Type Your answer");
                var userAnswer = Console.ReadLine();
                score = UpdateScore(userAnswer, item.CorrectAnswer, score);

                if (_remainingSeconds <= 0) break; 
            }

            if (_remainingSeconds > 0)
            {
                Console.WriteLine($"Nice job, You Scored {score}");
                if (score > MainUser.BestScore)
                {
                    MainUser.BestScore = score;
                    Console.WriteLine($"Nice Work, New Best Score of {score}");
                    SaveData();
                }
                Console.WriteLine("Top 10 Highest Scorers:");
                GetTop10();
                Thread.Sleep(5000);
                Home.UserHomePage(MainUser.Name, MainUser.Mail);
            }
            else
            {
                Console.WriteLine("Time's up! You failed the quiz.");
                Home.UserHomePage(MainUser.Name, MainUser.Mail);
            }

            timer.Stop();
            timer.Dispose();

        }

        private List<User> LoadData()
        {
            if (!File.Exists(_filePath))
                return new List<User>();

            string text = File.ReadAllText(_filePath);
            if (text == null)
                return new List<User>();
            return JsonSerializer.Deserialize<List<User>>(text);
        }
        private void SaveData()
        {
            string jsonString = JsonSerializer.Serialize(value: _users, options: new JsonSerializerOptions()
            {
                WriteIndented = true
            });

            File.WriteAllText(_filePath, jsonString);
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            _remainingSeconds--;
        }
    }
}

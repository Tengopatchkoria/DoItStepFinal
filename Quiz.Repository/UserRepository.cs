using Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Quiz.Repository
{
    public class UserRepository
    {
        private readonly string _filePath;
        private readonly List<User> _users;
        public User MainUser;

        public UserRepository(string filePath, string name, string mail)
        {
            _filePath = filePath;
            _users = LoadData();
            foreach (var user in _users)
            {
                if (user.Name == name)
                {
                    MainUser = LoginUser(name);
                }
            }
            if (MainUser == null)
            {
                MainUser = RegUser(name, mail);
            }
        }

        public string GetInfo() => $"{MainUser.Name} with the {MainUser.Mail} mail has best score of {MainUser.BestScore}";
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

        public static int UpdateScore(string userAnswer, string correctAnswer, int score)
        {
            if (userAnswer.Equals(correctAnswer))
                return score += 20;
            else { return score -= 20; }
        }

        public void CreateQuiz()
        {
            List<Question> Qlist = new();
            QuestionRepository q = new();

            q.CreateQuestionList(Qlist);

            Quizz quizz = new()
            {
                Id = 0,
                UserId = MainUser.id,
                QuestionList = Qlist
            };

            QuizRepository Qrepo = new(@"D:\Other\PROGRAMIREBA\C#\DoItStepFinal1\Quiz.Run\Data\Quiz.json");
            Qrepo.AddQuiz(quizz);
        }

        public void TakeQuiz(byte qId)
        {
            QuizRepository Qrepo = new(@"D:\Other\PROGRAMIREBA\C#\DoItStepFinal1\Quiz.Run\Data\Quiz.json");
            var quiz = Qrepo.GetQuiz(qId);
            int score = 0;

            if (quiz.UserId == this.MainUser.id)
            {
                Console.WriteLine("This Quiz is Created By You. Therefore You're Unable to Take it\n" +
                        "However You can Edit/Delete it\n" +
                        "Press 1 to Delete, Press 0 to Edit");
                int command = int.Parse(Console.ReadLine().Trim());
                Qrepo.ModifyQuiz(command, qId, MainUser.id);
                Environment.Exit(100);
            }

            foreach (var item in quiz.QuestionList)
            {
                Console.WriteLine(item.QueStion);
                Console.WriteLine($"{item.AnswerOptions[0]}, {item.AnswerOptions[1]}, {item.AnswerOptions[2]}, {item.AnswerOptions[3]}");
                Console.WriteLine("Type Your answer");
                var userAnswer = Console.ReadLine();
                score = UpdateScore(userAnswer, item.CorrectAnswer, score);
            }
            Console.WriteLine($"Nice job, You Scored {score}");
            if (score > this.MainUser.BestScore)
            {
                this.MainUser.BestScore = score;
                Console.WriteLine($"Nice Work, New Best Score of {score}");
                SaveData();
            }
        }

    }
}

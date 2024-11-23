using Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
            User[] userAr = new User[10];
            _users.Sort((x, y) => y.BestScore.CompareTo(x.BestScore));
            for (int i = 0; i < 10; i++)
            {
                foreach (var user in _users)
                    userAr[i] = user;
            }
            foreach (var user in userAr)
            {
                Console.WriteLine(user.Name, user.BestScore);
            }
        }

        private List<User> LoadData()
        {
            if (!File.Exists(_filePath))
                return new List<User>();

            string text = File.ReadAllText(_filePath);
            if (text != null)
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
            name.ToLower();
            foreach (var user in _users)
            {
                if (user.Name == name) return user;
            }

            return null;
        }

        private User RegUser(string name, string mail)
        {
            name.ToLower();

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

        public void CreateQuiz()
        {
            List<Question> Qlist = new();
            for (int i = 1; i < 6 ; i++)
            {
                Console.WriteLine("Enter a question");
                string question = Console.ReadLine();
                Console.WriteLine("Enter 4 Possible Answers (Separate each one with space)");
                string answerOptions = Console.ReadLine();
                Console.WriteLine("Enter Correct Answer");
                string correctAnswer = Console.ReadLine();

                var answerOptionsVar = answerOptions.Split(" ");
                var answerOptionsList = answerOptionsVar.ToList();

                if (!answerOptionsList.Contains(correctAnswer))
                {
                    throw new Exception("Correct answer is not on the options list");
                }

                Question fquestion = new()
                {
                    Id = (byte)i,
                    QueStion = question,
                    AnswerOptions = answerOptionsList,
                    CorrectAnswer = correctAnswer
                };
                Qlist.Add(fquestion);
            }
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
            var score = 0;

            if (quiz.UserId == this.MainUser.id)
                throw new Exception("Quizes cannot be Played by the owners");

            foreach (var item in quiz.QuestionList)
            {
                Console.WriteLine(item.QueStion);
                Console.WriteLine($"{item.AnswerOptions[0]}, {item.AnswerOptions[1]}, {item.AnswerOptions[2]}, {item.AnswerOptions[3]}");
                Console.WriteLine("Type Your answer");
                var userAnswer = Console.ReadLine();
                if (userAnswer.Equals(item.CorrectAnswer))
                    score += 20;
                else { score -= 20; }
            }

            if (score > this.MainUser.BestScore)
            {
                this.MainUser.BestScore = score;
                Console.WriteLine($"Nice Work, New Best Score of {score}");
            }
        }

    }
}

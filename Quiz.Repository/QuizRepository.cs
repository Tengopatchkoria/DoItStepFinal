using Quiz.Models;
using System.Text.Json;
using System.Timers;

namespace Quiz.Repository
{
    public class QuizRepository
    {
        private readonly string _filePath;
        private readonly List<Quizz> _quizes;
        public static string QuizRelativePath()
        {
            string QuizRelativePath = Path.Combine("Data", "Quiz.json");
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.FullName;

            return Path.Combine(projectDirectory, QuizRelativePath);
        }

        public static string UsersRelativePath()
        {
            string UserRelativePath = Path.Combine("Data", "Users.json");
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.FullName;

            return Path.Combine(projectDirectory, UserRelativePath);
        }
        public QuizRepository(string filepath)
        {
            _filePath = filepath;
            _quizes = LoadData();
        }

        public static void StartPage()
        {
            Console.Clear();
            Console.WriteLine("Hello, Welcome to my quiz application\nPlease enter your name and email to continue");
            Console.Write("Name:");
            string userName = Console.ReadLine();
            Console.Write("Mail:");
            string userMail = Console.ReadLine();
            UserRepository userRepo = new(UsersRelativePath(), userName, userMail);
            QuizRepository quizRepo = new(QuizRelativePath());

            InputManager(userRepo, quizRepo);
        }

        public static void UserHomePage(string userName, string userMail)
        {
            Console.Clear();
            UserRepository userRepo = new(UsersRelativePath(), userName, userMail);
            QuizRepository quizRepo = new(QuizRelativePath());

            InputManager(userRepo, quizRepo);
        }

        public static void InputManager(UserRepository userRepo, QuizRepository quizRepo)
        {
            var refNum = byte.Parse(Console.ReadLine());

            if (refNum == (byte)2)
            {
                userRepo.CreateQuiz();
            }
            else if (refNum == (byte)1)
            {
                if (quizRepo.GetLength() == 0)
                {
                    Console.WriteLine("Looks like there are no quizes avaliable. try creating your own one");
                    userRepo.CreateQuiz();
                }
                else
                {
                    Console.Clear();
                    var i = 1;
                    foreach (var item in quizRepo.GetAllQuizes())
                    {
                        Console.WriteLine($"{i}." + item.Name);
                        i++;
                    }
                    Console.WriteLine($"{i}.Back");
                    Console.WriteLine($"Pick a quiz");
                    var input = byte.Parse(Console.ReadLine());
                    if(input > quizRepo.GetLength())
                        UserHomePage(userRepo.MainUser.Name, userRepo.MainUser.Mail);
                    else
                        userRepo.TakeQuiz(input);
                }
            }
            else if (refNum == (byte)3)
                userRepo.LogOut();
            else if (refNum == (byte)4)
            {
                Console.WriteLine(userRepo.GetInfo());
                Thread.Sleep(5000);
                UserHomePage(userRepo.MainUser.Name, userRepo.MainUser.Mail);
            }

            else { throw new Exception("Wrong Input"); }
        }

        public Quizz GetQuiz(byte id) => _quizes.FirstOrDefault(x => x.Id == id);

        public List<Quizz> GetAllQuizes() => _quizes;
        public int GetLength() => _quizes.Count;

        public void ModifyQuiz(int command, byte qId, User MainUser)
        {
            if (command == 2)
            {
                DeleteQuiz(qId, MainUser.id);
                UserHomePage(MainUser.Name, MainUser.Mail);
            }
            else if (command == 1)
            {
                Console.WriteLine("Which question do you want to modify?");
                byte QuestionId = byte.Parse(Console.ReadLine());
                EditQuiz(QuestionId, qId, MainUser.id);
                UserHomePage(MainUser.Name, MainUser.Mail);
            }
        }

        public void DeleteQuiz(byte id, byte uId)
        {
            if (GetQuiz(id).UserId == uId)
                _quizes.Remove(GetQuiz(id));
            SaveData();

        }

        public void EditQuiz(byte id, byte qId, byte uId)
        {
            QuestionRepository q = new();

            if (GetQuiz(qId).UserId == uId)
                q.ReWriteQuestion(id, GetQuiz(qId).QuestionList);
            SaveData();
        }

        public void AddQuiz(Quizz quiz)
        {
            quiz.Id = (byte)((byte)_quizes.Count + 1);
            _quizes.Add(quiz);
            SaveData();
        }

        private List<Quizz> LoadData()
        {
            if (!File.Exists(_filePath))
                return new List<Quizz>();
            string text = File.ReadAllText(_filePath);
            if (text == null)
                return new List<Quizz>();
            return JsonSerializer.Deserialize<List<Quizz>>(text);
        }
        private void SaveData()
        {
            string jsonString = JsonSerializer.Serialize(value: _quizes, options: new JsonSerializerOptions()
            {
                WriteIndented = true
            });

            File.WriteAllText(_filePath, jsonString);
        }

    }
}

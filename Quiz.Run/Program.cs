using Quiz.Repository;
using System.Threading.Tasks.Sources;

namespace Quiz.Run
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, Welcome to my quiz application\nPlease enter your name and email to continue");
            Console.Write("Name:");
            string userName = Console.ReadLine();
            Console.Write("Mail:");
            string userMail = Console.ReadLine();

            UserRepository userRepo = new(@"D:\Other\PROGRAMIREBA\C#\DoItStepFinal1\Quiz.Run\Data\Users.json", userName, userMail);
            QuizRepository quizRepo = new(@"D:\Other\PROGRAMIREBA\C#\DoItStepFinal1\Quiz.Run\Data\Quiz.json");

            Console.WriteLine($"Hello {userName}\nIn this quiz app you can create your quizes or enjoy with already created ones" +
                $"\nType 0 to take a quiz\nType 1 to create your own");
            userRepo.GetTop10();
            var refNum = byte.Parse(Console.ReadLine());

            if (refNum == (byte)1)
            {
                userRepo.CreateQuiz();
            }
            else if (refNum == (byte)0)
            {
                if (quizRepo.GetLength() == 0)
                {
                    Console.WriteLine("Looks like there are no quizes avaliable. try creating your own one");
                    userRepo.CreateQuiz();
                }
                else
                {
                    Console.WriteLine($"Pick a Number between 1 and {quizRepo.GetLength()}");
                    var quizId = byte.Parse(Console.ReadLine());
                    userRepo.TakeQuiz(quizId);
                    Console.WriteLine("Now You can see whether or not you made it into the Top 10 Highest Scorers");
                    userRepo.GetTop10();
                }
            }
            else { throw new Exception("Wtf are you doing"); }

        }
    }
}

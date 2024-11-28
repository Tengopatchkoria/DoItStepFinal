using Quiz.Repository;
using Home = Quiz.Repository.QuizRepository;


namespace Quiz.Run
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string QuizRelativePath = Path.Combine("Data", "Quiz.json");
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.FullName;

            Console.WriteLine(Path.Combine(projectDirectory, QuizRelativePath));
            QuizRepository Qrepo = new(Path.Combine(projectDirectory, QuizRelativePath));
            Home.StartPage();
        }
    }
}

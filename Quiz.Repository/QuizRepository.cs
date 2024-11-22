using Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Quiz.Repository
{
    public class QuizRepository
    {
        private readonly string _filePath;
        private readonly List<Quizz> _quizes;

        public QuizRepository(string filepath)
        {
            _filePath = filepath;
            _quizes = LoadData();
        }

        public Quizz GetQuiz(byte id) => _quizes.FirstOrDefault(x => x.Id == id);

        public void DeleteQuiz(byte id, User user)
        {
            if (GetQuiz(id).UserId == user.id)
                _quizes.Remove(GetQuiz(id));
        }

        public void EditQuiz(byte id, byte q_id, User user)
        {
            if (GetQuiz(id).UserId == user.id)
            {
                string newQ = Console.ReadLine();
                GetQuiz(id).QuestionList.FirstOrDefault(x => x.Id == q_id).QueStion = newQ;
            }
        }

        private List<Quizz> LoadData()
        {
            if (!File.Exists(_filePath))
                return new List<Quizz>();

            string text = File.ReadAllText(_filePath);
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

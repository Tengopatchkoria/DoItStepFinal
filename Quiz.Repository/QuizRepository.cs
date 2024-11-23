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

        public byte GetLength() => (byte)_quizes.Count;

        public void DeleteQuiz(byte id, User user)
        {
            if (GetQuiz(id).UserId == user.id)
                _quizes.Remove(GetQuiz(id));
            SaveData();
        }

        public void EditQuiz(byte id, byte q_id, User user)
        {
            if (GetQuiz(id).UserId == user.id)
            {
                string newQ = Console.ReadLine();
                GetQuiz(id).QuestionList.FirstOrDefault(x => x.Id == q_id).QueStion = newQ;
            }
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
            if (text != null)
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

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

        public int GetLength() => _quizes.Count;

        public void ModifyQuiz(int command, byte qId, byte uId)
        {
            if (command == 1)
                DeleteQuiz(qId, uId);
            else if (command == 0)
            {
                Console.WriteLine("Which Question Do you want to modify?");
                byte QuestionId = byte.Parse(Console.ReadLine());
                EditQuiz(QuestionId, qId, uId);
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

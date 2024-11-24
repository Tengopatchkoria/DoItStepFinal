using Quiz.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Repository
{
    public class QuestionRepository
    {
        public void CreateQuestionList(List<Question> list)
        {
            for (int i = 1; i < 6; i++)
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
                list.Add(fquestion);
            }
        }
        public void ReWriteQuestion(byte qId, List<Question> list)
        {
            Console.WriteLine("Enter a new question");
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
                Id = qId,
                QueStion = question,
                AnswerOptions = answerOptionsList,
                CorrectAnswer = correctAnswer
            };
            list[(int)qId - 1] = fquestion;
        }
    }
}

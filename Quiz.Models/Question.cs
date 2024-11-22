using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Models
{
    public class Question
    {
        public byte Id { get; set; }
        public string QueStion { get; set; }
        public List<string> AnswerOptions { get; set; }
        public string CorrectAnswer { get; set; }
    }
}
